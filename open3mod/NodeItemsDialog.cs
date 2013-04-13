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

namespace open3mod
{
    public partial class NodeItemsDialog : Form
    {
        private Node _node;

        public NodeItemsDialog()
        {
            InitializeComponent();
        }


        public void SetNode(MainWindow mainWindow, Node node)
        {
            _node = node;

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
    }
}
