using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Amib.Threading;
using Assimp;
using Action = System.Action;
using Timer = System.Windows.Forms.Timer;

namespace open3mod
{
    /// <summary>
    /// Dialog to compute normals for a single or multiple meshes.
    /// </summary>
    public sealed partial class NormalVectorGeneratorDialog : Form
    {
        private const float DefaultThresholdAngle = 45.0f;

        private readonly Scene _scene;

        private class ProcessedMesh
        {
            public Mesh Mesh { get; set; }
            public Mesh PreviewMesh { get; set; }
            public NormalVectorGenerator Generator { get; set; }
        };

        private readonly List<ProcessedMesh> _meshesToProcess; 
        private float _thresholdAngleInDegrees = DefaultThresholdAngle;
        private volatile float _lastCompletedUpdateAngle = -1.0f;
        private volatile bool _isInitialUpdate = true;
        private Thread _updateThread;
        private readonly AutoResetEvent _syncEvent = new AutoResetEvent(false);
        private bool _committed = false;

        /// <summary>
        /// Real-time updates means that normals are updated upon moving the
        /// Smoothness slider. This costs CPU resources and may be a bad idea
        /// for larger scenes, so we make it configurable.
        /// </summary>
        public bool RealtimeUpdateEnabled
        {
            get { return checkBoxRealtimePreview.Checked; }
        }

        /// <summary>
        /// Construct for a single mesh.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mesh"></param>
        /// <param name="meshName">Display name of the mesh for UI consistency</param>
        public NormalVectorGeneratorDialog(Scene scene, Mesh mesh, string meshName)
            : this(scene,new List<Mesh> { mesh }, meshName)
        {    }


        /// <summary>
        /// Construct for a set of meshes.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mesh"></param>
        /// <param name="description">String to display in title</param>
        public NormalVectorGeneratorDialog(Scene scene, IEnumerable<Mesh> mesh, string description)
        {
            Debug.Assert(scene != null);
            _scene = scene;
            _meshesToProcess = mesh.Select(m => new ProcessedMesh {Mesh = m}).ToList();

            InitializeComponent();
            buttonApply.Enabled = !checkBoxRealtimePreview.Checked;

            Text = string.Format("{0} - {1}", description, Text);
            // Keep UX disabled until initial update is done.
            foreach (Control control in Controls)
            {
                if (control != labelStatusText)
                {
                    control.Enabled = false;
                }
            }
            // This kicks of the update thread if real time updates are enabled
            trackBarAngle.Value = (int)_thresholdAngleInDegrees;
        }

        /// <summary>
        /// Use a separate thread pool for coarse ( = mesh level ) parallelization.
        /// 
        /// STP shows deadlocks on recursive use. This can be mitigated by setting priorities,
        /// but to use a separate thread pool is far safer. The extra cost of setting up a
        /// few threads is relatively minor given that the threads will afterwards run
        /// large jobs.
        /// </summary>
        private static readonly SmartThreadPool CoarseThreadPool = new SmartThreadPool();

        /// <summary>
        /// Update normals in the current mesh and refresh the 3D view.
        /// </summary>
        private void UpdateNormals(float angle)
        {
            SafeInvoke(new Action(() => labelStatusText.Text = _isInitialUpdate ? "Preparing ..." : "Updating ..."));
            _meshesToProcess.ParallelDo(
                entry =>
                {
                    if (entry.PreviewMesh == null)
                    {
                        entry.PreviewMesh = MeshUtil.DeepCopy(entry.Mesh);
                    }
                    if (entry.Generator == null)
                    {
                        entry.Generator = new NormalVectorGenerator(entry.PreviewMesh);
                    }
                    entry.Generator.Compute(angle);

                    // Use BeginInvoke() to dispatch the mesh override change to the GUI/Render thread.
                    if (InvokeRequired)
                    {
                        BeginInvoke(new Action(() => _scene.SetOverrideMesh(entry.Mesh, entry.PreviewMesh)));
                    }
                    else
                    {
                        _scene.SetOverrideMesh(entry.Mesh, entry.PreviewMesh);
                    }
                }, 1 /* granularity per-mesh */, CoarseThreadPool);
            Action closeAction = null;
            closeAction = new Action(
                () =>
                {
                    if (!trackBarAngle.Capture)
                    {
                        labelStatusText.Text = "";
                    }
                    else
                    {
                        // If the mouse is currently captured, do not reset text to avoid flickering.
                        // Instead, check again in 150ms.
                        var timer = new Timer { Interval = 150 };
                        timer.Tick += (sender, args) =>
                        {
                            timer.Enabled = false;
                            closeAction();
                        }
                    ;
                        timer.Start();
                    }
                    if (_isInitialUpdate)
                    {
                        foreach (Control control in Controls)
                        {
                            control.Enabled = true;
                        }
                        _isInitialUpdate = false;
                    }
                });
            SafeInvoke(closeAction);
        }

        private void SafeInvoke(Action action)
        {
            try
            {
                BeginInvoke(action);
            }
            catch (InvalidOperationException)
            {
                // Ignore. This happens if this is scheduled before the dialog constructor returns.
                return;
            }
        }

        /// <summary>
        /// Schedule a delayed update, starting the update thread if needed.
        /// </summary>
        private void ScheduleUpdateNormals()
        {
            if (_updateThread == null)
            {
                _updateThread = new Thread(
                    () =>
                    {
                        while (true)
                        {
                            try
                            {
                                float angle = _thresholdAngleInDegrees;
                                UpdateNormals(angle);
                                _lastCompletedUpdateAngle = angle;
                                _syncEvent.WaitOne();
                            }
                            catch (ThreadAbortException)
                            {
                                break;
                            }
                            catch (ThreadInterruptedException) { }
                        }
                    });
                _updateThread.Start();
            }
            else
            {
                _syncEvent.Set();
                _updateThread.Interrupt();
            }
        }

        /// <summary>
        /// Join and cleanup update thread.
        /// </summary>
        private void StopUpdateThread()
        {
            if (_updateThread == null)
            {
                return;
            }
            labelStatusText.Text = "";
            _updateThread.Abort();
            _updateThread.Join();
            _updateThread = null;
        }

        /// <summary>
        /// Update normals and create an UndoStack entry for the operation.
        /// </summary>
        private void Commit()
        {
            StopUpdateThread();
            if (Math.Abs(_lastCompletedUpdateAngle - _thresholdAngleInDegrees) > 0.001f)
            {
                UpdateNormals(_thresholdAngleInDegrees);
            }
            var meshesToProcess = _meshesToProcess.Select(_ => _).ToList();
            var originalMeshes = _meshesToProcess.Select(entry => MeshUtil.ShallowCopy(entry.Mesh)).ToList();
            _scene.UndoStack.PushAndDo("Compute Normals",
                () =>
                {                   
                    foreach (var entry in meshesToProcess)
                    {
                        MeshUtil.ShallowCopy(entry.Mesh, entry.PreviewMesh);
                    }
                },
                () =>
                {
                    meshesToProcess.ZipAction(originalMeshes,
                        (entry, origMesh) =>
                        {
                            MeshUtil.ShallowCopy(entry.Mesh, origMesh);
                        });
                },
                () =>
                {
                    foreach (var entry in meshesToProcess)
                    {
                        _scene.SetOverrideMesh(entry.Mesh, null);
                    }
                    _scene.RequestRenderRefresh();
                });
            _committed = true;
        }

        /// <summary>
        /// Revert all changes made to the mesh.
        /// </summary>
        private void Revert()
        {
            StopUpdateThread();
            foreach (var entry in _meshesToProcess)
            {
                _scene.SetOverrideMesh(entry.Mesh, null);
                MeshUtil.ClearMesh(entry.PreviewMesh);
            }
            _meshesToProcess.Clear();
        }

        // Event handlers
        private void CheckBoxRealtimePreviewCheckedChanged(object sender, EventArgs e)
        {
            buttonApply.Enabled = !checkBoxRealtimePreview.Checked;
            if (RealtimeUpdateEnabled)
            {
                ScheduleUpdateNormals();
            }
        }

        private void OnChangeSmoothness(object sender, EventArgs e)
        {
            _thresholdAngleInDegrees = trackBarAngle.Value;
            labelAngle.Text = string.Format("{0} Degrees", trackBarAngle.Value.ToString());
            if (RealtimeUpdateEnabled)
            {
                ScheduleUpdateNormals();
            }
        }

        private void OnManualApply(object sender, EventArgs e)
        {
            ScheduleUpdateNormals();
        }

        private void OnOk(object sender, EventArgs e)
        {
            Commit();
            Close();
        }    

        private void OnCancel(object sender, EventArgs e)
        {
            Revert();
            Close();
        }

        private void OnClose(object sender, FormClosingEventArgs e)
        {
            StopUpdateThread();
            if (!_committed)
            {
                Revert();
            }
        } 
    }
}
