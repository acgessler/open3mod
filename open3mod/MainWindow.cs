using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    public partial class MainWindow : Form
    {
        private UiState _ui;
        private Renderer _renderer;
        private FpsTracker _fps;

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

            InitializeComponent();

            // sync UI
            toolStripButtonShowFPS.CheckState = _ui.ShowFps ? CheckState.Checked : CheckState.Unchecked;
            toolStripButtonShowShaded.CheckState = _ui.RenderLit ? CheckState.Checked : CheckState.Unchecked;
            toolStripButtonShowTextures.CheckState = _ui.RenderTextured ? CheckState.Checked : CheckState.Unchecked;
            toolStripButtonWireframe.CheckState = _ui.RenderWireframe ? CheckState.Checked : CheckState.Unchecked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
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
        }


        private void FrameRender()
        {
            _renderer.Draw();
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
    }
}
