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

        private readonly HashSet<Node> _filter = new HashSet<Node>(); 


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

            var root = new TreeNode(node.Name) {Tag = node};
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
                    AddMeshNode(node, _scene.Raw.Meshes[m], m, root);
                }
            }

            if (level < AutoExpandLevels)
            {
                root.Expand();
            }
        }

        private void AddMeshNode(Node owner, Mesh mesh, int id, TreeNode uiNode)
        {
            Debug.Assert(uiNode != null);
            Debug.Assert(mesh != null);

            // meshes need not be named, in this case we number them
            var desc = "Mesh " + (!string.IsNullOrEmpty(mesh.Name)
                ? ("\"" + mesh.Name + "\"")
                : id.ToString(CultureInfo.InvariantCulture));

            var nod = new TreeNode(desc) {Tag = new KeyValuePair<Node, Mesh>(owner, mesh)};

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

        public void UpdateFilters()
        {
            var item = _tree.SelectedNode.Tag;
            _filter.Clear();

            if(item == _scene.Raw.RootNode)
            {
                _scene.SetVisibleNodes(null);
                return;
            }

            // if the selected item is a mesh, we render only the corresponding
            // parent node plus this mesh. Otherwise we include all child nodes.
            var itemAsNode = item as Node;
            if (itemAsNode != null)
            {
                AddNodeToSet(_filter, itemAsNode);
            }

            //var itemAsMesh = (KeyValuePair<Node, Mesh>)item;
            // XXX

            _scene.SetVisibleNodes(_filter);
        }

        private void AddNodeToSet(HashSet<Node> filter, Node itemAsNode)
        {
            filter.Add(itemAsNode);
            if (itemAsNode.Children == null)
            {
                return;
            }

            foreach(Node n in itemAsNode.Children)
            {
                AddNodeToSet(filter, n);
            }
        }
    }
}
