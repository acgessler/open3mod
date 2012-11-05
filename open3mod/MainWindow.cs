using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    public partial class MainWindow : Form
    {
        private Renderer _renderer;


        public MainWindow()
        {
            InitializeComponent();
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
        }

        private void OnGlResize(object sender, EventArgs e)
        {
            if (_renderer == null) // safeguard in case glControl's Load() wasn't fired yet
            {
                return;
            }
        }

        private void GlPaint(object sender, PaintEventArgs e)
        {
            if (_renderer == null) // safeguard in case glControl's Load() wasn't fired yet
            {
                return;
            }

            _renderer.Draw();
            glControl1.SwapBuffers();
        }
    }
}
