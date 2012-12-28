using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;

namespace open3mod
{
    public partial class TabUiSkeleton : UserControl
    {
        public TabUiSkeleton()
        {
            InitializeComponent();
        }


        public SplitContainer GetSplitter()
        {
            return splitContainer1;
        }

        public InspectionView GetInspector()
        {
            return inspectionView1;
        }


        public void InjectGlControl(GLControl gl)
        {
            var s = splitContainer1.Controls[0];

            s.Controls.Add(gl);
            gl.Left = s.Left;
            gl.Top = s.Top;

            gl.Width = s.Width;
            gl.Height = s.Height;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
