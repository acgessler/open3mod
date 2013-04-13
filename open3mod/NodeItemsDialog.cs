using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            
        }
    }
}
