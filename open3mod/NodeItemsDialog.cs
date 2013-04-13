using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assimp;
using OpenTK;

namespace open3mod
{
    public partial class NodeItemsDialog : Form
    {
        private Node _node;
        private readonly Timer _timer;
        private Scene _scene;


        private const int TimerUpdateInterval = 100;

        public NodeItemsDialog()
        {
            InitializeComponent();
            _timer = new Timer {Interval = TimerUpdateInterval};
            _timer.Tick += TimerOnTick;
            if (checkBoxShowAnimated.Checked)
            {
                _timer.Start();
            }
        }


        public void SetNode(MainWindow mainWindow, Scene scene, Node node)
        {
            _node = node;
            _scene = scene;

            var matrix4X4 = _node.Transform;
            trafoMatrixViewControlLocal.SetMatrix(ref matrix4X4);

            var mat = Matrix4x4.Identity;
            var cur = node;
            while(cur != null)
            {
                var trafo = cur.Transform;
                trafo.Transpose();
                mat = trafo * mat;
                cur = cur.Parent;
            }
            mat.Transpose();
            trafoMatrixViewControlGlobal.SetMatrix(ref mat);

            Text = node.Name + " - Details";

            // populate statistics
            labelMeshesDirect.Text = node.MeshCount.ToString(CultureInfo.InvariantCulture);
            labelChildrenDirect.Text = node.ChildCount.ToString(CultureInfo.InvariantCulture);

            var meshTotal = 0;
            var childTotal = 0;
            CountMeshAndChildrenTotal(node, ref meshTotal, ref childTotal);

            labelMeshesTotal.Text = node.MeshCount.ToString(CultureInfo.InvariantCulture);
            labelChildrenTotal.Text = node.ChildCount.ToString(CultureInfo.InvariantCulture);
        }


        private void CountMeshAndChildrenTotal(Node node, ref int meshTotal, ref int childTotal)
        {
            Debug.Assert(node != null);

            meshTotal += node.MeshCount;
            childTotal += node.ChildCount;
            for (var i = 0; i < node.ChildCount; ++i)
            {
                CountMeshAndChildrenTotal(node.Children[i], ref meshTotal, ref childTotal);
            }
        }


        private void OnChangeAnimationState(object sender, EventArgs e)
        {
            if(checkBoxShowAnimated.Checked)
            {
                _timer.Start();
            }
            else
            {
                _timer.Stop();

                trafoMatrixViewControlGlobal.ResetAnimatedMatrix();
                trafoMatrixViewControlLocal.ResetAnimatedMatrix();
            }
        }


        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if(_scene == null)
            {
                return;
            }

            Debug.Assert(_node != null);
            var anim = _scene.SceneAnimator;
            if (anim.ActiveAnimation == -1)
            {
                trafoMatrixViewControlGlobal.ResetAnimatedMatrix();
                trafoMatrixViewControlLocal.ResetAnimatedMatrix();
            }
            else
            {
                Matrix4 m;
                Matrix4x4 mm;
                anim.GetGlobalTransform(_node, out m);
                OpenTkToAssimp.FromMatrix(ref m, out mm);
                trafoMatrixViewControlGlobal.SetAnimatedMatrix(ref mm);

                anim.GetLocalTransform(_node, out m);
                OpenTkToAssimp.FromMatrix(ref m, out mm);
                trafoMatrixViewControlLocal.SetAnimatedMatrix(ref mm);
            }
        }
    }
}
