using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;

namespace open3mod
{
    public partial class MainWindow : Form
    {
        private readonly UiState _ui;
        private Renderer _renderer;
        private readonly FpsTracker _fps;
        private LogViewer _logViewer;
        private int _previousMousePosX = -1;
        private int _previousMousePosY = -1;
        private bool _mouseDown;

        private delegate void DelegateOpenFile(String s);           
        private readonly DelegateOpenFile _delegateOpenFile;
        private bool _forwardPressed;
        private bool _leftPressed;
        private bool _rightPressed;
        private bool _backPressed;
        private bool _upPressed;
        private bool _downPressed;

        public GLControl GlControl
        {
            get { return glControl1; }
        }

        public UiState UiState
        {
            get { return _ui; }
        }

        public FpsTracker Fps
        {
            get { return _fps; }
        }


        public MainWindow()
        {
            _ui = new UiState();
            _fps = new FpsTracker();

            // create delegate used for asynchronous calls to OpenFile
            _delegateOpenFile = this.OpenFile;

            OpenFile("../../../testdata/scenes/COLLADA.dae");

            InitializeComponent();

            // sync UI with UIState
            toolStripButtonShowFPS.CheckState = _ui.ShowFps ? CheckState.Checked : CheckState.Unchecked;
            toolStripButtonShowShaded.CheckState = _ui.RenderLit ? CheckState.Checked : CheckState.Unchecked;
            toolStripButtonShowTextures.CheckState = _ui.RenderTextured ? CheckState.Checked : CheckState.Unchecked;
            toolStripButtonWireframe.CheckState = _ui.RenderWireframe ? CheckState.Checked : CheckState.Unchecked;

            toolStripButtonFullView.CheckState = _ui.ActiveViewMode == UiState.ViewMode.Single ? CheckState.Checked : CheckState.Unchecked;
            toolStripButtonTwoViews.CheckState = _ui.ActiveViewMode == UiState.ViewMode.Two ? CheckState.Checked : CheckState.Unchecked;
            toolStripButtonFourViews.CheckState = _ui.ActiveViewMode == UiState.ViewMode.Four ? CheckState.Checked : CheckState.Unchecked;

            // manually register the MouseWheel handler
            glControl1.MouseWheel += OnMouseMove;

            // intercept all key events sent to children
            KeyPreview = true;
        }

        /// <summary>
        /// Open a particular 3D model
        /// </summary>
        /// <param name="s"></param>
        public void OpenFile(string s)
        {
            UiState.ActiveScene = new Scene(s);
        }


        private void Form1Load(object sender, EventArgs e)
        {

        }

        private void ToolsToolStripMenuItemClick(object sender, EventArgs e)
        {

        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            var ab = new About();
            ab.ShowDialog();
        }

        private void OnGlLoad(object sender, EventArgs e)
        {
            _renderer = new Renderer(this);

            // register Idle event so we get regular callbacks for drawing
            Application.Idle += ApplicationIdle;
        }


        private void OnGlResize(object sender, EventArgs e)
        {
            if (_renderer == null) // safeguard in case glControl's Load() wasn't fired yet
            {
                return;
            }

            _renderer.Resize();
        }

        private void GlPaint(object sender, PaintEventArgs e)
        {
            if (_renderer == null) // safeguard in case glControl's Load() wasn't fired yet
            {
                return;
            }

            FrameRender();
        }

        private void ApplicationIdle(object sender, EventArgs e)
        {
            // no guard needed -- we hooked into the event in Load handler
            while (glControl1.IsIdle)
            {
                FrameUpdate();
                FrameRender();
            }
        }

        private void FrameUpdate()
        {          
            _fps.Update();
            _renderer.Update();

            ProcessKeys();
        }

        private void ProcessKeys()
        {
            var cam = UiState.ActiveCameraController;
            if (cam == null)
            {
                return;
            }

            var dt = (float)_fps.LastFrameDelta;
            float x = 0.0f, y = 0.0f, z = 0.0f;

            bool changed = false;

            if(_forwardPressed)
            {
                changed = true;
                z += dt;
            }
            if (_backPressed)
            {
                changed = true;
                z -= dt;
            }

            if (_rightPressed)
            {
                changed = true;
                x += dt;
            }
            if (_leftPressed)
            {
                changed = true;
                x -= dt;
            }

            if (_upPressed)
            {
                changed = true;
                y += dt;
            }
            if (_downPressed)
            {
                changed = true;
                y -= dt;
            }

            if(!changed)
            {
                return;
            }

            cam.MovementKey(x, y, z);
        }

        private void FrameRender()
        {
            _renderer.Draw(_ui.ActiveScene);
            glControl1.SwapBuffers();
        }



        private void ToggleFps(object sender, EventArgs e)
        {
            _ui.ShowFps = !_ui.ShowFps;
        }

        private void ToggleShading(object sender, EventArgs e)
        {
            _ui.RenderLit = !_ui.RenderLit;
        }

        private void ToggleTextures(object sender, EventArgs e)
        {
            _ui.RenderTextured = !_ui.RenderTextured;
        }

        private void ToggleWireframe(object sender, EventArgs e)
        {
            _ui.RenderWireframe = !_ui.RenderWireframe;
        }

        private void ToggleFullView(object sender, EventArgs e)
        {
            if (UiState.ActiveViewMode == UiState.ViewMode.Single) return;
            UiState.ActiveViewMode = UiState.ViewMode.Single;

            toolStripButtonFullView.CheckState = CheckState.Checked;
            toolStripButtonTwoViews.CheckState = CheckState.Unchecked;
            toolStripButtonFourViews.CheckState = CheckState.Unchecked;
        }

        private void ToggleTwoViews(object sender, EventArgs e)
        {
            if (UiState.ActiveViewMode == UiState.ViewMode.Two) return;
            UiState.ActiveViewMode = UiState.ViewMode.Two;

            toolStripButtonFullView.CheckState = CheckState.Unchecked;
            toolStripButtonTwoViews.CheckState = CheckState.Checked;
            toolStripButtonFourViews.CheckState = CheckState.Unchecked;
        }

        private void ToggleFourViews(object sender, EventArgs e)
        {
            if (UiState.ActiveViewMode == UiState.ViewMode.Four) return;
            UiState.ActiveViewMode = UiState.ViewMode.Four;

            toolStripButtonFullView.CheckState = CheckState.Unchecked;
            toolStripButtonTwoViews.CheckState = CheckState.Unchecked;
            toolStripButtonFourViews.CheckState = CheckState.Checked;
        }

        private void UpdateActiveViewIfNeeded(MouseEventArgs e)
        {
            var x = e.X / (float)glControl1.ClientSize.Width;
            var y = 1.0f - e.Y / (float)glControl1.ClientSize.Height;

            // check which viewport has been hit
            var index = UiState.ViewIndex.Index0;
            foreach(var view in _ui.ActiveViews)
            {
                if (view == null)
                {
                    ++index;
                    continue;
                }

                if (x >= view.Value.X && x <= view.Value.Z &&
                    y >= view.Value.Y && y <= view.Value.W)
                {
                    _ui.ActiveViewIndex = index;
                    break;
                }
            ++index;
            }           
        }

        private void OnShowLogViewer(object sender, EventArgs e)
        {
            if(_logViewer == null)
            {
                _logViewer = new LogViewer(this);
                _logViewer.Closed += (o, args) =>
                {
                    _logViewer = null;
                };
                _logViewer.Show();
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;

            _previousMousePosX = e.X;
            _previousMousePosY = e.Y;

            UpdateActiveViewIfNeeded(e);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {       
            if (e.Delta != 0 && UiState.ActiveCameraController != null)
            {
                UiState.ActiveCameraController.Scroll(e.Delta);
            }

            if(!_mouseDown)
            {
                return;
            }

            if (UiState.ActiveCameraController != null)
            {
                UiState.ActiveCameraController.MouseMove(e.X - _previousMousePosX, e.Y - _previousMousePosY);
            }
            _previousMousePosX = e.X;
            _previousMousePosY = e.Y;
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (_mouseDown)
            {
                Capture = true;
            }
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            Capture = false;
        }

        private void OnDrag(object sender, DragEventArgs e)
        {
            // code based on http://www.codeproject.com/Articles/3598/Drag-and-Drop
            try
            {
                var a = (Array)e.Data.GetData(DataFormats.FileDrop);

                if (a != null)
                {
                    // Extract string from first array element
                    // (ignore all files except first if number of files are dropped).
                    string s = a.GetValue(0).ToString();

                    // Call OpenFile asynchronously.
                    // Explorer instance from which file is dropped is not responding
                    // all the time when DragDrop handler is active, so we need to return
                    // immediately (especially if OpenFile shows MessageBox).

                    BeginInvoke(_delegateOpenFile, new Object[] { s });

                    // in the case Explorer overlaps this form
                    Activate();        
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in DragDrop function: " + ex.Message);
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // only accept files for drag and drop
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyData)
            {
                case Keys.W:
                case Keys.Up:
                    _forwardPressed = true;
                    break;

                case Keys.A:
                case Keys.Left:
                    _leftPressed = true;
                    break;

                case Keys.S:
                case Keys.Right:
                    _rightPressed = true;
                    break;

                case Keys.D:
                case Keys.Back:
                    _backPressed = true;
                    break;

                case Keys.PageUp:
                    _upPressed = true;
                    break;

                case Keys.PageDown:
                    _downPressed = true;
                    break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.KeyData)
            {
                case Keys.W:
                case Keys.Up:
                    _forwardPressed = false;
                    break;

                case Keys.A:
                case Keys.Left:
                    _leftPressed = false;
                    break;

                case Keys.S:
                case Keys.Right:
                    _rightPressed = false;
                    break;

                case Keys.D:
                case Keys.Back:
                    _backPressed = false;
                    break;

                case Keys.PageUp:
                    _upPressed = false;
                    break;

                case Keys.PageDown:
                    _downPressed = false;
                    break;
            }
        }

        private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }
    }
}
