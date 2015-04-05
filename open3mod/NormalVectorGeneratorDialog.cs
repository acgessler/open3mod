using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// A child of |MeshDetailsDialog|. Tied to a single scene, but the mesh to which
    /// it applies can change dynamically.
    /// </summary>
    public sealed partial class NormalVectorGeneratorDialog : Form
    {
        private const float DefaultThresholdAngle = 45.0f;

        private readonly Scene _scene;
        private readonly String _baseText;

        private Mesh _mesh;
        private Mesh _previewMesh;
        private NormalVectorGenerator _generator;
        private float _thresholdAngleInDegrees = DefaultThresholdAngle;
        private Thread _updateThread;
        private readonly AutoResetEvent _syncEvent = new AutoResetEvent(false);

        /// <summary>
        /// Real-time updates means that normals are updated upon moving the
        /// Smoothness slider. This costs CPU resources and may be a bad idea
        /// for larger scenes, so we make it configurable.
        /// </summary>
        public bool RealtimeUpdateEnabled
        {
            get { return checkBoxRealtimePreview.Checked; }
        }

        public NormalVectorGeneratorDialog(Scene scene)
        {
            Debug.Assert(scene != null);
            _scene = scene;          
            InitializeComponent();
            _baseText = Text;
            buttonApply.Enabled = !checkBoxRealtimePreview.Checked;
            trackBarAngle.Value = (int) _thresholdAngleInDegrees;
        }

        /// <summary>
        /// Set mesh for which normals are computed.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="meshName"></param>
        public void SetMesh(Mesh mesh, string meshName)
        {
            Debug.Assert(mesh != null);
            Debug.Assert(meshName != null);
            if (mesh == _mesh)
            {
                return;
            }

            StopUpdateThread();
            if (_mesh != null)
            {
                Revert();
            }

            _mesh = mesh;
            _previewMesh = MeshUtil.DeepCopy(_mesh);
            _generator = new NormalVectorGenerator(_previewMesh);
            Text = string.Format("{0} - {1}", meshName, _baseText);

            OnChangeSmoothness(null, null);
        }

        /// <summary>
        /// Update normals in the current mesh and refresh the 3D view.
        /// </summary>
        private void UpdateNormals()
        {
            _generator.Compute(_thresholdAngleInDegrees);
            // Use BeginInvoke() to dispatch to the GUI/Render thread.
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => _scene.SetOverrideMesh(_mesh, _previewMesh)));
            }
            else _scene.SetOverrideMesh(_mesh, _previewMesh);
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
                        float lastThresholdAngleInDegrees = -1.0f;
                        while (true)
                        {
                            if (Math.Abs(lastThresholdAngleInDegrees - _thresholdAngleInDegrees) > 0.1f)
                            {
                                UpdateNormals();
                                lastThresholdAngleInDegrees = _thresholdAngleInDegrees;
                            }
                            else
                            {
                                try
                                {
                                    _syncEvent.WaitOne();
                                }
                                catch (ThreadInterruptedException)
                                {
                                    break;
                                }
                            }
                        }
                    });
                _updateThread.Start();
            }
            else
            {
                _syncEvent.Set();
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
            _updateThread.Interrupt();
            _updateThread.Join();
            _updateThread = null;
        }

        /// <summary>
        /// Update normals and create an UndoStack entry for the operation.
        /// </summary>
        private void Commit()
        {
            UpdateNormals();
            _scene.SetOverrideMesh(_mesh, null);
            Mesh originalMesh = MeshUtil.ShallowCopy(_mesh);
            _scene.UndoStack.PushAndDo("Compute Normals",
                () =>
                {
                    MeshUtil.ShallowCopy(_mesh, _previewMesh);
                    _scene.RequestRenderRefresh();
                },
                () =>
                {
                    MeshUtil.ShallowCopy(_mesh, originalMesh);
                    _scene.RequestRenderRefresh();
                });    
        }

        /// <summary>
        /// Revert all changes made to the mesh.
        /// </summary>
        private void Revert()
        {
            _scene.SetOverrideMesh(_mesh, null);
        }


        // Event handlers
        private void CheckBoxRealtimePreviewCheckedChanged(object sender, EventArgs e)
        {
            buttonApply.Enabled = !checkBoxRealtimePreview.Checked;
            if (!RealtimeUpdateEnabled)
            {
                StopUpdateThread();
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
            if (_updateThread == null)
            {
                UpdateNormals();
            }
        }

        private void OnOk(object sender, EventArgs e)
        {
            StopUpdateThread();
            Commit();
            // Do not Close() as this would dispose the dialog object.
            // MeshDetailsDialog keeps it and re-uses it.
            Hide();
        }    

        private void OnCancel(object sender, EventArgs e)
        {
            StopUpdateThread();
            Revert();
            // See note on OnOk().
            Hide();
        } 
    }
}
