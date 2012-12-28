using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Populates the tree view in the scene inspector that shows the
    /// scene hierarchy and allows selection of arbitrary nodes to 
    /// limit the rendering to them and their children.
    /// </summary>
    public class HierarchyInspectionView
    {
        private readonly Scene _scene;
        private readonly TreeView _tree;


        private const int AutoExpandLevels = 4;


        public HierarchyInspectionView(Scene scene, TreeView tree)
        {
            _scene = scene;
            _tree = tree;

            Debug.Assert(_scene != null);
            Debug.Assert(_tree != null);

            AddNodes(scene.Raw.RootNode, null, 0);
        }

        private void AddNodes(Node node, TreeNode uiNode, int level)
        {
            Debug.Assert(node != null);

            var root = new TreeNode(node.Name);
            if(uiNode == null)
            {
                _tree.Nodes.Add(root);
            }
            else
            {
                uiNode.Nodes.Add(root);
            }        

            // add children
            if (node.Children != null)
            {

                foreach (var c in node.Children)
                {
                    AddNodes(c, root, level + 1);
                }
            }

            // add mesh nodes
            if (node.MeshIndices != null)
            {
                foreach (var m in node.MeshIndices)
                {
                    AddMeshNode(_scene.Raw.Meshes[m], m, root);
                }
            }

            if (level < AutoExpandLevels)
            {
                root.Expand();
            }
        }


        private void AddMeshNode(Mesh mesh, int id, TreeNode uiNode)
        {
            Debug.Assert(uiNode != null);
            Debug.Assert(mesh != null);

            var nod = new TreeNode("Mesh " + (!string.IsNullOrEmpty(mesh.Name) ? ("\"" + mesh.Name  + "\"") 
                : id.ToString(CultureInfo.InvariantCulture)) );

            uiNode.Nodes.Add(nod);

            var verts = new TreeNode(mesh.VertexCount + " Vertices");
            nod.Nodes.Add(verts);

            var triangles = new TreeNode(mesh.FaceCount + " Faces");
            nod.Nodes.Add(triangles);

            if (mesh.BoneCount > 0)
            {
                var bones = new TreeNode(mesh.BoneCount + " Bones");
                nod.Nodes.Add(bones);
            }
        }
    }
}
