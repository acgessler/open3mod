using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        private int _nodeCount;

        private readonly HashSet<Node> _filter = new HashSet<Node>(); 


        private const int AutoExpandLevels = 4;
        private static Color DefaultBackColor = Color.White;
        private static Color PositiveBackColor = Color.GreenYellow;
        private static Color NegativeBackColor = Color.OrangeRed;
        private int _visibleNodes;
        private int _visibleMeshes;
        private int _visibleInstancedMeshes;
        private int _meshCountFullScene;
        private int _instancedMeshCountFullScene;

        public HierarchyInspectionView(Scene scene, TreeView tree)
        {
            _scene = scene;
            _tree = tree;

            Debug.Assert(_scene != null);
            Debug.Assert(_tree != null);
         
            AddNodes();
            CountMeshes();           
        }
        

        /// <summary>
        /// Get the number of nodes currently selected for rendering
        /// </summary>
        public int CountVisible
        {
            get {
                return _visibleNodes;
            }
        }

        /// <summary>
        /// Get the total number of nodes in the scene
        /// </summary>
        public int CountNodes
        {
            get { return _nodeCount; }
        }

        /// <summary>
        /// Get the number of unique visible meshes 
        /// </summary>
        public int CountVisibleMeshes
        {
            get { return _visibleMeshes; }
        }

        /// <summary>
        /// Get the number of visible mesh instances:
        /// CountVisibleInstancedMeshes >= CountVisibleMeshes
        /// </summary>
        public int CountVisibleInstancedMeshes
        {
            get { return _visibleInstancedMeshes; }
        }


        private void CountMeshes()
        {
            var counters = new List<int>(_scene.Raw.MeshCount);
            for (int i = 0; i < _scene.Raw.MeshCount; ++i )
            {
                counters.Add(0);    
            }
            
            CountMeshes(_scene.Raw.RootNode, counters);

            _visibleInstancedMeshes = _instancedMeshCountFullScene = counters.Sum();
            _visibleMeshes = _meshCountFullScene = counters.Count(i => i != 0);
        }


        private void CountMeshes(Node node, IList<int> counters)
        {
            Debug.Assert(counters.Count == _scene.Raw.MeshCount);

            if (node.Children != null)
            {
                foreach (var c in node.Children)
                {
                    CountMeshes(c, counters);
                }
            }

            if (node.MeshIndices != null)
            {
                foreach (var m in node.MeshIndices)
                {
                    ++counters[m];
                }
            }
        }


        private void AddNodes()
        {
            AddNodes(_scene.Raw.RootNode, null, 0);
            _visibleNodes = _nodeCount;
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
            
            ++_nodeCount;

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
                ResetHighlighting(_tree.Nodes[0]);

                // update statistics
                _visibleNodes = _nodeCount;
                _visibleMeshes = _meshCountFullScene;
                _visibleInstancedMeshes = _instancedMeshCountFullScene;
                return;
            }

            // if the selected item is a mesh, we render only the corresponding
            // parent node plus this mesh. Otherwise we include all child nodes.
            var itemAsNode = item as Node;
            if (itemAsNode != null)
            {
                var counters = new List<int>(_scene.Raw.MeshCount);
                for (int i = 0; i < _scene.Raw.MeshCount; ++i)
                {
                    counters.Add(0);
                }

                AddNodeToSet(_filter, itemAsNode);
                CountMeshes(itemAsNode, counters);

                // update statistics
                _visibleInstancedMeshes = counters.Sum();
                _visibleMeshes = counters.Count(i => i != 0);
            }

            //var itemAsMesh = (KeyValuePair<Node, Mesh>)item;
            // XXX

            _scene.SetVisibleNodes(_filter);
            UpdateHighlighting(_tree.Nodes[0]);

            _visibleNodes = _filter.Count;                     
        }

        private void ResetHighlighting(TreeNode n)
        {
            n.BackColor = DefaultBackColor;
            int i;
            for (i = 0; i < n.Nodes.Count; ++i)
            {
                ResetHighlighting(n.Nodes[i]);
            }
        }

        private void UpdateHighlighting(TreeNode n)
        {            
            if (n.Tag != null)
            {
                var node = n.Tag as Node ?? ((KeyValuePair<Node, Mesh>)n.Tag).Key;
                if (_filter.Contains(node))
                {
                    n.BackColor = PositiveBackColor;
                }
                else
                {
                    n.BackColor = DefaultBackColor;
                }
            }
            for (var i = 0; i < n.Nodes.Count; ++i)
            {
                UpdateHighlighting(n.Nodes[i]);
            }
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
