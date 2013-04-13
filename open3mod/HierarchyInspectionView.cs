///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [HierarchyInspectionView.cs]
// (c) 2012-2013, Alexander C. Gessler
//
// Licensed under the terms and conditions of the 3-clause BSD license. See
// the LICENSE file in the root folder of the repository for the details.
//
// HIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///////////////////////////////////////////////////////////////////////////////////


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
    public enum NodePurpose
    {
        // note: this maps one by one to the TreeView's image indices
        Joint = 2,
        ImporterGenerated = 0,
        GenericMeshHolder = 1,
    }


    /// <summary>
    /// Populates the tree view in the scene inspector that shows the
    /// scene hierarchy and allows selection of arbitrary nodes to 
    /// limit the rendering to them and their children.
    /// </summary>
    public sealed partial class HierarchyInspectionView : UserControl
    {
        private readonly Scene _scene;
 
        private int _nodeCount;
        private readonly Dictionary<Node, List<Mesh>> _filterByMesh;

        private const int AutoExpandLevels = 4;
        private static new Color DefaultBackColor = Color.White;
        private static Color PositiveBackColor = Color.GreenYellow;
        private static Color SearchIterateBackColor = Color.Gold;
        private static Color NegativeBackColor = Color.OrangeRed;
        private int _visibleNodes;
        private int _visibleMeshes;
        private int _visibleInstancedMeshes;
        private int _meshCountFullScene;
        private int _instancedMeshCountFullScene;
        private string _searchText = "";
        private readonly string _searchInfoText;
        private readonly Color _searchInfoColor;

        private bool _isInSearchMode;
        private bool _searchLocked;
        private int _hitNodeCursor;
        private List<TreeNode> _hitNodes;

        private readonly HashSet<Node> _hidden = new HashSet<Node>();

        // static because all tabs share them - it is just annoying to have multiple
        // info dialogs open because it is impossible to keep track which belongs
        // to which tab.
        private static MeshDetailsDialog _meshDiag;
        private static NodeItemsDialog _nodeDiag;
        private int _targetLocY;
        private int _popupAnimFramesRemaining;
        private Timer _popupAnimTimer;
        private int _oldLocY;

        public HierarchyInspectionView(Scene scene, TabPage tabPageHierarchy)
        {
            _filterByMesh = new Dictionary<Node, List<Mesh>>();
            
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            Dock = DockStyle.Fill;

            InitializeComponent();
            _searchInfoText = textBoxFilter.Text;
            _searchInfoColor = textBoxFilter.ForeColor;

            _scene = scene;
            tabPageHierarchy.Controls.Add(this);

            Debug.Assert(_scene != null);

            labelHitCount.BackColor = PositiveBackColor;

            nodeInfoPopup.Owner = this;
            meshInfoPopup.Owner = this;

            HidePopups();
            AddNodes();
            CountMeshes();
            UpdateStatistics();
        }


        private void UpdateStatistics()
        {
            labelNodeStats.Text = string.Format("Showing {0} of {1} nodes ({2} meshes, {3} instances)",
                CountVisible,
                CountNodes,
                CountVisibleMeshes,
                CountVisibleInstancedMeshes);
        }


        /// <summary>
        /// Get the number of nodes currently selected for rendering
        /// </summary>
        public int CountVisible
        {
            get
            {
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
            for (int i = 0; i < _scene.Raw.MeshCount; ++i)
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
            _tree.BeginUpdate();
            AddNodes(_scene.Raw.RootNode, null, 0);
            _visibleNodes = _nodeCount;
            _tree.EndUpdate();
        }


        private bool AddNodes(Node node, TreeNode uiNode, int level)
        {
            Debug.Assert(node != null);

            // default node icon
            var index = (int)NodePurpose.GenericMeshHolder;
            var isSkeletonNode = false;

            // mark nodes introduced by assimp (i.e. nodes not present in the source file)
            if (node.Name.StartsWith("<") && node.Name.EndsWith(">"))
            {
                index = (int)NodePurpose.ImporterGenerated;
            }
            else
            {
                // mark skeleton nodes (joints) with a special icon. The algorithm to
                // detect them is easy: check whether if this node or any children 
                // carry meshes. if not, assume this is a joint.
                isSkeletonNode = node.MeshCount == 0;
            }

            var root = new TreeNode(node.Name) {Tag = node, ContextMenuStrip = contextMenuStripTreeNode};
            if (uiNode == null)
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
                    isSkeletonNode = AddNodes(c, root, level + 1) && isSkeletonNode;
                }
            }

            // add mesh nodes
            if (node.MeshCount != 0)
            {
                foreach (var m in node.MeshIndices)
                {
                    AddMeshNode(node, _scene.Raw.Meshes[m], m, root);
                }
            }

            if(isSkeletonNode)
            {
                index = (int)NodePurpose.Joint;
            }

            root.ImageIndex = root.SelectedImageIndex = index;

            if (level < AutoExpandLevels)
            {
                root.Expand();
            }

            return isSkeletonNode;
        }


        private void AddMeshNode(Node owner, Mesh mesh, int id, TreeNode uiNode)
        {
            Debug.Assert(uiNode != null);
            Debug.Assert(mesh != null);

            // meshes need not be named, in this case we number them
            var desc = "Mesh " + (!string.IsNullOrEmpty(mesh.Name)
                ? ("\"" + mesh.Name + "\"")
                : id.ToString(CultureInfo.InvariantCulture));

            var nod = new TreeNode(desc)
            {
                Tag = new KeyValuePair<Node, Mesh>(owner, mesh),
                ImageIndex = 3,
                ContextMenuStrip = contextMenuStripMesh
            };

            uiNode.Nodes.Add(nod);
        }


        private void UpdateFilters(TreeNode hoverNode = null)
        {
            var node = hoverNode ?? _tree.SelectedNode;
            var item = node == null ? _scene.Raw.RootNode : node.Tag;

            _filterByMesh.Clear();

            var overrideSkeleton = false;

            if (item == _scene.Raw.RootNode && !HasPermanentlyHiddenNodes)
            {
                _scene.SetVisibleNodes(null);

                // update statistics
                _visibleNodes = _nodeCount;
                _visibleMeshes = _meshCountFullScene;
                _visibleInstancedMeshes = _instancedMeshCountFullScene;

                HidePopups();
                return;
            }

            // if the selected item is a mesh, we render only the corresponding
            // parent node plus this mesh. Otherwise we include all child nodes.
            var itemAsNode = item as Node;
            if (itemAsNode != null && !IsNodePermanentlyHidden(itemAsNode))
            {
                var counters = new List<int>(_scene.Raw.MeshCount);
                for (int i = 0; i < _scene.Raw.MeshCount; ++i)
                {
                    counters.Add(0);
                }

                AddNodeToSet(_filterByMesh, itemAsNode);
                CountMeshes(itemAsNode, counters);

                // update statistics
                _visibleInstancedMeshes = counters.Sum();
                _visibleMeshes = counters.Count(i => i != 0);

                if (node != null && GetNodePurpose(node) == NodePurpose.Joint)
                {
                    overrideSkeleton = true;           
                }

                if (node != null)
                {
                    PopulateNodeInfoPopup(node);

                    if (_nodeDiag != null)
                    {
                        SetNodeDetailDialogInfo(itemAsNode);
                    }
                }
                else
                {
                    HidePopups();
                }

            }
            else if (item is KeyValuePair<Node, Mesh>)
            {
                var itemAsMesh = (KeyValuePair<Node, Mesh>) item;
                Debug.Assert(itemAsMesh.Key != null);
                Debug.Assert(itemAsMesh.Value != null);

                var arr = new List<Mesh> {itemAsMesh.Value};
                _filterByMesh.Add(itemAsMesh.Key, arr);

                _visibleMeshes = 1;
                _visibleInstancedMeshes = 1;

                PopulateMeshInfoPopup(node);
                if(_meshDiag != null && node != null)
                {
                    SetMeshDetailDialogInfo(itemAsMesh.Value, node.Text);
                }
            }
            else
            {
                HidePopups();
            }

            _scene.SetVisibleNodes(_filterByMesh);
        
            _visibleNodes = _filterByMesh.Count;
            UpdateStatistics();

            _scene.SetSkeletonVisibleOverride(overrideSkeleton);
        }


        private void HidePopups()
        {
            nodeInfoPopup.Visible = false;
            meshInfoPopup.Visible = false;
        }


        private void PopulateMeshInfoPopup(TreeNode node)
        {
            Debug.Assert(node != null && node.Tag is KeyValuePair<Node, Mesh>);

            var wasVisible = nodeInfoPopup.Visible || meshInfoPopup.Visible;
            if (nodeInfoPopup.Visible)
            {
                meshInfoPopup.Location = nodeInfoPopup.Location;
            }

            if(_tree.Width - node.Bounds.Right < 80)
            {
                meshInfoPopup.Visible = false;
                nodeInfoPopup.Visible = false;
                return;
            }

            meshInfoPopup.Visible = true;
            nodeInfoPopup.Visible = false;
            if (wasVisible)
            {
                AnimatePopup(node.Bounds.Top);
            }
            else
            {
                var loc = meshInfoPopup.Location;
                loc.Y = node.Bounds.Top;
                meshInfoPopup.Location = loc;
            }

            meshInfoPopup.Populate(((KeyValuePair<Node, Mesh>)node.Tag).Value);
        }


        private void PopulateNodeInfoPopup(TreeNode node)
        {
            Debug.Assert(node != null && node.Tag is Node);

            var wasVisible = nodeInfoPopup.Visible || meshInfoPopup.Visible;
            if (_tree.Width - node.Bounds.Right < 80)
            {
                meshInfoPopup.Visible = false;
                nodeInfoPopup.Visible = false;
                return;
            }

            if(meshInfoPopup.Visible)
            {
                nodeInfoPopup.Location = meshInfoPopup.Location;
            }

            meshInfoPopup.Visible = false;
            nodeInfoPopup.Visible = true;
            if(wasVisible)
            {
                AnimatePopup(node.Bounds.Top);            
            }
            else
            {
                var loc = nodeInfoPopup.Location;
                loc.Y = node.Bounds.Top;
                nodeInfoPopup.Location = loc;
            }

            nodeInfoPopup.Populate((Node)node.Tag, GetNodePurpose(node));
        }


        private void AnimatePopup(int targetLocY)
        {
            var c = (nodeInfoPopup.Visible ? (Control) nodeInfoPopup : meshInfoPopup);

            _targetLocY = targetLocY;
            _oldLocY = c.Location.Y;

            const int frameCount = 5;
            _popupAnimFramesRemaining = frameCount;
            
            if(_popupAnimTimer == null)
            {
                _popupAnimTimer = new Timer {Interval = 30};
                _popupAnimTimer.Tick += (sender, args) =>
                {
                    var cInner = (nodeInfoPopup.Visible ? (Control) nodeInfoPopup : meshInfoPopup);
                    --_popupAnimFramesRemaining;

                    var loc = cInner.Location;
                    loc.Y = _targetLocY - (int)((_targetLocY - _oldLocY) * ((double)_popupAnimFramesRemaining / frameCount));
                    cInner.Location = loc;

                    if (_popupAnimFramesRemaining == 0)
                    {
                        _popupAnimTimer.Stop();
                    }
                };
            }

            _popupAnimTimer.Start();
        }


        private void SetMeshDetailDialogInfo(Mesh mesh, string text)
        {
            if (_meshDiag == null)
            {
                _meshDiag = new MeshDetailsDialog();
                _meshDiag.FormClosed += (o, args) =>
                {
                    _meshDiag = null;
                };
                _meshDiag.Show();
            }
            else
            {
                _meshDiag.BringToFront();
            }

            _meshDiag.SetMesh(FindForm() as MainWindow, mesh, text);
        }


        private void SetNodeDetailDialogInfo(Node node)
        {
            if (_nodeDiag == null)
            {
                _nodeDiag = new NodeItemsDialog();
                _nodeDiag.FormClosed += (o, args) =>
                {
                    _nodeDiag = null;
                };
                _nodeDiag.Show();
            }
            else
            {
                _nodeDiag.BringToFront();
            }

            _nodeDiag.SetNode(FindForm() as MainWindow, _scene, node);
        }


        private static NodePurpose GetNodePurpose(TreeNode node)
        {
            // TODO: this is ugly
            return ImageIndexToNodePurpose(node.ImageIndex);
        }


        private static NodePurpose ImageIndexToNodePurpose(int imageIndex)
        {
            Debug.Assert(imageIndex < 4);
            return (NodePurpose) imageIndex;
        }


        private void UpdateTextSearch()
        {
            if (_searchText != "")
            {
                var nodes = new List<TreeNode>();
                UpdateHighlighting(_tree.Nodes[0], nodes);

                if (nodes.Count > 0)
                {
                    nodes[0].EnsureVisible();

                    _hitNodes = nodes;
                    _hitNodeCursor = 0;
                }
                else
                {
                    _hitNodes = null;
                    _hitNodeCursor = -1;
                }

                labelHitCount.Text = string.Format("{0} hit{1}", nodes.Count.ToString(CultureInfo.InvariantCulture), 
                    nodes.Count == 1 ? "" : "s");
            }
            else
            {
                ResetHighlighting(_tree.Nodes[0]);
                labelHitCount.Text = "";

                _hitNodes = null;
                _hitNodeCursor = -1;
            }
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


        private void UpdateHighlighting(TreeNode n, List<TreeNode> searchHitNodes = null)
        {
            if (n.Tag != null)
            {
                var node = n.Tag as Node;
                if (node != null)
                {
                    if (node.Name.ToLower().Contains(_searchText))
                    {
                        n.BackColor = PositiveBackColor;
                        if (searchHitNodes != null)
                        {
                            searchHitNodes.Add(n);
                        }
                    }
                    else
                    {
                        n.BackColor = DefaultBackColor;
                    }
                }
                else
                {
                    var nodeMesh = (KeyValuePair<Node, Mesh>) n.Tag;
                    if (nodeMesh.Value.Name.Length > 0 && nodeMesh.Value.Name.ToLower().Contains(_searchText))
                    {
                        n.BackColor = PositiveBackColor;
                        if (searchHitNodes != null)
                        {
                            searchHitNodes.Add(n);
                        }
                    }
                    else
                    {
                        n.BackColor = DefaultBackColor;
                    }
                }
            }
            for (var i = 0; i < n.Nodes.Count; ++i)
            {
                UpdateHighlighting(n.Nodes[i], searchHitNodes);
            }
        }


        private void AddNodeToSet(Dictionary<Node, List<Mesh>> filter, Node itemAsNode)
        {
            if (!IsNodePermanentlyHidden(itemAsNode))
            {
                filter.Add(itemAsNode, null);
            }
            if (itemAsNode.Children == null)
            {
                return;
            }

            foreach (Node n in itemAsNode.Children)
            {
                AddNodeToSet(filter, n);
            }
        }


        private void OnMouseLeave(object sender, EventArgs e)
        {
            UpdateFilters();
            //Capture = false;
        }


        private void OnMouseEnter(object sender, EventArgs e)
        {
            //Capture = true;
        }


        private void OnNodeHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            UpdateFilters(e.Node);           
        }



        private void AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }


        private void OnChangeFilterText(object sender, EventArgs e)
        {
            var str = _isInSearchMode ? textBoxFilter.Text.ToLower().Trim() : "";
            if (str != _searchText)
            {
                _searchText = str;
                _searchLocked = false;
                UpdateTextSearch();
            }
        }

      
        private void OnClickSearchBox(object sender, EventArgs e)
        {
            if (_isInSearchMode)
            {
                return;
            }

            _isInSearchMode = true;
            textBoxFilter.ForeColor = Color.Black;
            textBoxFilter.Text = "";
        }


        private void OnStopFocusingOnSearch(object sender, EventArgs e)
        {
            if (_searchLocked)
            {
                return;
            }
            _isInSearchMode = false;
            textBoxFilter.Text = _searchInfoText;
            textBoxFilter.ForeColor = _searchInfoColor;
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = false;  
            }
        }


        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) // 13 is newline and thus corresponds to the Enter key
            {
                e.Handled = true;

                if (sender == textBoxFilter && _hitNodes != null)
                {
                    _searchLocked = true;
                }

                if (_searchLocked)
                {
                    Debug.Assert(_hitNodes != null);

                    // fix the last iterated element's background color
                    _hitNodes[_hitNodeCursor > 0 ? _hitNodeCursor - 1 : _hitNodes.Count-1].BackColor = PositiveBackColor;

                    // select next search item
                    _tree.SelectedNode = _hitNodes[_hitNodeCursor];
                    _tree.SelectedNode.EnsureVisible();
                    _tree.SelectedNode.BackColor = SearchIterateBackColor;

                    UpdateFilters();
                    _hitNodeCursor = (_hitNodeCursor + 1)%_hitNodes.Count;
                }
            }
        }


        private void AfterNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ShowDetailsForTreeNode(e.Node);           
        }


        private void ShowDetailsForTreeNode(TreeNode node)
        {
            Debug.Assert(node != null);
            if (node.Tag is KeyValuePair<Node, Mesh>)
            {
                var mesh = ((KeyValuePair<Node, Mesh>)node.Tag).Value;
                Debug.Assert(mesh != null);

                SetMeshDetailDialogInfo(mesh, node.Text);
            }
            else if (node.Tag is Node)
            {
                SetNodeDetailDialogInfo((Node)node.Tag);
            }
        }


        private TreeNode GetTreeNodeForContextMenuEvent(object sender)
        {
            // get sender TreeNode --
            // http://www.windows-tech.info/3/61534a0f5205ea18.php
            var cms = sender as ContextMenuStrip ?? (ContextMenuStrip)((ToolStripMenuItem)sender).Owner;
            var treeView = (TreeView)cms.SourceControl;

            var node = _tree.GetNodeAt(treeView.PointToClient(cms.Location));
            Debug.Assert(node != null);
            return node;
        }


        private void OnContextMenuShowDetails(object sender, EventArgs e)
        {
            var node = GetTreeNodeForContextMenuEvent(sender);
            ShowDetailsForTreeNode(node);
        }


        private void OnContextMenuHideNode(object sender, EventArgs e)
        {
            var node = GetTreeNodeForContextMenuEvent(sender);
            HideSubhierarchyPermanently(node);
        }


    
        private bool HasPermanentlyHiddenNodes
        {
            get { return _hidden.Count != 0; }
        }


        private bool IsNodePermanentlyHidden(Node node)
        {
            return _hidden.Contains(node);
        }


        private void HideSubhierarchyPermanently(TreeNode root)
        {
            Debug.Assert(root.Tag is Node);
            var node = (Node) root.Tag;

            _hidden.Add(node);
            root.ImageIndex = root.SelectedImageIndex = 4;
            root.Collapse();

            UpdateFilters();
        }


        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // http://stackoverflow.com/questions/3166643/windows-forms-treeview-node-context-menu-problem
            // select a node on which the user invokes the context menu - this
            // avoids some ugly glitches with the popups and should also improve
            // user experience.
            if(e.Button != MouseButtons.Right)
            {
                return;
            }
            var treeNodeAtMousePosition = _tree.GetNodeAt(_tree.PointToClient(e.Location));
            var selectedTreeNode = _tree.SelectedNode;
            if (treeNodeAtMousePosition == null || treeNodeAtMousePosition == selectedTreeNode)
            {
                return;
            }

            _tree.SelectedNode = treeNodeAtMousePosition;
            UpdateFilters();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */