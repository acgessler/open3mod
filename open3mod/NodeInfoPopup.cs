using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    public partial class NodeInfoPopup : UserControl
    {
        private HierarchyInspectionView _owner;

        public NodeInfoPopup()
        {
            InitializeComponent();
        }


        public HierarchyInspectionView Owner
        {
            set { _owner = value; }
        }


        /// <summary>
        /// Sets the contents of the info pop-up given an assimp node.
        /// 
        /// At the time this method is called, the node info popup's
        /// location has already been adjusted by the caller.
        /// </summary>
        public void Populate(Node node, NodePurpose purpose)
        {
            Debug.Assert(node != null);
            Debug.Assert(_owner != null);
            switch (purpose)
            {
                case NodePurpose.Joint:
                    labelCaption.Text = "Joint";
                    break;
                case NodePurpose.ImporterGenerated:
                    labelCaption.Text = "Root";
                    break;
                case NodePurpose.GenericMeshHolder:
                    labelCaption.Text = "Node";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("purpose");
            }

            //labelInfo;
        }
    }
}
