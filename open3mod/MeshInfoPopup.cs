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
    public partial class MeshInfoPopup : UserControl
    {
        private HierarchyInspectionView _owner;

        public MeshInfoPopup()
        {
            InitializeComponent();
        }


        public HierarchyInspectionView Owner
        {
            set { _owner = value; }
        }


        /// <summary>
        /// Sets the contents of the info pop-up given an assimp mesh.
        /// 
        /// At the time this method is called, the mesh info popup's
        /// location has already been adjusted by the caller.
        /// </summary>
        public void Populate(Mesh mesh)
        {
            Debug.Assert(mesh != null);
            Debug.Assert(_owner != null);

            labelInfo.Text = string.Format("{0} Vertices\n{1} Faces\n", mesh.VertexCount, mesh.FaceCount);
        }
    }
}
