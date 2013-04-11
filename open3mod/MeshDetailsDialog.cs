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
using Assimp;

namespace open3mod
{
    public partial class MeshDetailsDialog : Form
    {
 
        public MeshDetailsDialog()
        {
            InitializeComponent();
        }


        public void SetMesh(Mesh mesh, string meshName) 
        {
            Debug.Assert(mesh != null);

            labelVertexCount.Text = mesh.VertexCount + " Vertices";
            labelFaceCount.Text = mesh.FaceCount + " Faces";
            Text = meshName + " - Details";

            checkedListBoxPerFace.CheckOnClick = false;
            checkedListBoxPerFace.SetItemCheckState(0,
                mesh.PrimitiveType.HasFlag(PrimitiveType.Triangle)
                ? CheckState.Checked
                : CheckState.Unchecked);

            checkedListBoxPerFace.SetItemCheckState(1,
                mesh.PrimitiveType.HasFlag(PrimitiveType.Line)
                ? CheckState.Checked
                : CheckState.Unchecked);

            checkedListBoxPerFace.SetItemCheckState(2,
                mesh.PrimitiveType.HasFlag(PrimitiveType.Point)
                ? CheckState.Checked
                : CheckState.Unchecked);

            checkedListBoxPerVertex.CheckOnClick = false;
            checkedListBoxPerVertex.SetItemCheckState(0, CheckState.Checked);
            checkedListBoxPerVertex.SetItemCheckState(1, mesh.HasNormals
                ? CheckState.Checked
                : CheckState.Unchecked);
            checkedListBoxPerVertex.SetItemCheckState(2, mesh.HasTangentBasis
                ? CheckState.Checked
                : CheckState.Unchecked);

            Debug.Assert(mesh.TextureCoordinateChannels.Length >= 4);
            for (var i = 0; i < 4; ++i)
            {
                checkedListBoxPerVertex.SetItemCheckState(3 + i, mesh.HasTextureCoords(i)
                    ? CheckState.Checked
                    : CheckState.Unchecked);
            }

            Debug.Assert(mesh.VertexColorChannels.Length >= 4);
            for (var i = 0; i < 4; ++i)
            {
                checkedListBoxPerVertex.SetItemCheckState(7 + i, mesh.HasVertexColors(i)
                    ? CheckState.Checked
                    : CheckState.Unchecked);
            }

            checkedListBoxPerVertex.SetItemCheckState(11, mesh.HasBones
                ? CheckState.Checked
                : CheckState.Unchecked);
        }
    }
}
