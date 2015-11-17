namespace open3mod
{
    partial class HierarchyInspectionView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HierarchyInspectionView));
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this._tree = new System.Windows.Forms.TreeView();
            this.imageListIconsHierarchy = new System.Windows.Forms.ImageList(this.components);
            this.labelNodeStats = new System.Windows.Forms.Label();
            this.labelHiddenCount = new System.Windows.Forms.Label();
            this.panelHiddenInfo = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelHitCount = new System.Windows.Forms.Label();
            this.contextMenuStripTreeNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.centerPivotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMesh = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemRenameMesh = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.detailsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.meshInfoPopup = new open3mod.MeshInfoPopup();
            this.nodeInfoPopup = new open3mod.NodeInfoPopup();
            this.panelHiddenInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStripTreeNode.SuspendLayout();
            this.contextMenuStripMesh.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(245, 3);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(54, 13);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Unhide all";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UnhideAllNodes);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.textBoxFilter.Location = new System.Drawing.Point(12, 7);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(287, 20);
            this.textBoxFilter.TabIndex = 7;
            this.textBoxFilter.Text = "Type to search";
            this.toolTip1.SetToolTip(this.textBoxFilter, "Enter search text here. Press Enter to lock search and to cycle through results.");
            this.textBoxFilter.Click += new System.EventHandler(this.OnClickSearchBox);
            this.textBoxFilter.TextChanged += new System.EventHandler(this.OnChangeFilterText);
            this.textBoxFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.textBoxFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.textBoxFilter.Leave += new System.EventHandler(this.OnStopFocusingOnSearch);
            // 
            // _tree
            // 
            this._tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tree.BackColor = System.Drawing.Color.White;
            this._tree.FullRowSelect = true;
            this._tree.HotTracking = true;
            this._tree.ImageIndex = 0;
            this._tree.ImageList = this.imageListIconsHierarchy;
            this._tree.Location = new System.Drawing.Point(0, 33);
            this._tree.Name = "_tree";
            this._tree.SelectedImageIndex = 0;
            this._tree.ShowNodeToolTips = true;
            this._tree.Size = new System.Drawing.Size(311, 573);
            this._tree.TabIndex = 5;
            this._tree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.BeforeExpand);
            this._tree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.BeforeExpand);
            this._tree.NodeMouseHover += new System.Windows.Forms.TreeNodeMouseHoverEventHandler(this.OnNodeHover);
            this._tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AfterSelect);
            this._tree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.AfterNodeDoubleClick);
            this._tree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this._tree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this._tree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            this._tree.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this._tree.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            // 
            // imageListIconsHierarchy
            // 
            this.imageListIconsHierarchy.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIconsHierarchy.ImageStream")));
            this.imageListIconsHierarchy.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListIconsHierarchy.Images.SetKeyName(0, "root.png");
            this.imageListIconsHierarchy.Images.SetKeyName(1, "normal.png");
            this.imageListIconsHierarchy.Images.SetKeyName(2, "joints.png");
            this.imageListIconsHierarchy.Images.SetKeyName(3, "mesh.png");
            this.imageListIconsHierarchy.Images.SetKeyName(4, "HierarchyIconHidden.png");
            // 
            // labelNodeStats
            // 
            this.labelNodeStats.AutoSize = true;
            this.labelNodeStats.Location = new System.Drawing.Point(9, 5);
            this.labelNodeStats.Name = "labelNodeStats";
            this.labelNodeStats.Size = new System.Drawing.Size(118, 13);
            this.labelNodeStats.TabIndex = 0;
            this.labelNodeStats.Text = "Showing m of n nodes. ";
            // 
            // labelHiddenCount
            // 
            this.labelHiddenCount.AutoSize = true;
            this.labelHiddenCount.Location = new System.Drawing.Point(10, 3);
            this.labelHiddenCount.Name = "labelHiddenCount";
            this.labelHiddenCount.Size = new System.Drawing.Size(158, 13);
            this.labelHiddenCount.TabIndex = 0;
            this.labelHiddenCount.Text = "p nodes are permanently hidden";
            // 
            // panelHiddenInfo
            // 
            this.panelHiddenInfo.BackColor = System.Drawing.Color.LemonChiffon;
            this.panelHiddenInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHiddenInfo.Controls.Add(this.linkLabel1);
            this.panelHiddenInfo.Controls.Add(this.labelHiddenCount);
            this.panelHiddenInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelHiddenInfo.Location = new System.Drawing.Point(0, 612);
            this.panelHiddenInfo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.panelHiddenInfo.Name = "panelHiddenInfo";
            this.panelHiddenInfo.Size = new System.Drawing.Size(311, 21);
            this.panelHiddenInfo.TabIndex = 9;
            this.panelHiddenInfo.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Cornsilk;
            this.panel1.Controls.Add(this.labelNodeStats);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 633);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(311, 21);
            this.panel1.TabIndex = 8;
            // 
            // labelHitCount
            // 
            this.labelHitCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHitCount.AutoSize = true;
            this.labelHitCount.BackColor = System.Drawing.Color.GreenYellow;
            this.labelHitCount.Location = new System.Drawing.Point(247, 10);
            this.labelHitCount.Name = "labelHitCount";
            this.labelHitCount.Size = new System.Drawing.Size(0, 13);
            this.labelHitCount.TabIndex = 10;
            // 
            // contextMenuStripTreeNode
            // 
            this.contextMenuStripTreeNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.centerPivotToolStripMenuItem,
            this.hideToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItemRename,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripSeparator2,
            this.detailsToolStripMenuItem});
            this.contextMenuStripTreeNode.Name = "contextMenuStripTreeNode";
            this.contextMenuStripTreeNode.Size = new System.Drawing.Size(176, 148);
            this.contextMenuStripTreeNode.Opening += new System.ComponentModel.CancelEventHandler(this.OpOpenNodeContextMenu);
            // 
            // centerPivotToolStripMenuItem
            // 
            this.centerPivotToolStripMenuItem.Name = "centerPivotToolStripMenuItem";
            this.centerPivotToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.centerPivotToolStripMenuItem.Text = "Center View (Pivot)";
            this.centerPivotToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuPivotNode);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.hideToolStripMenuItem.Text = "Hide from View";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuHideNode);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(172, 6);
            // 
            // toolStripMenuItemRename
            // 
            this.toolStripMenuItemRename.Name = "toolStripMenuItemRename";
            this.toolStripMenuItemRename.Size = new System.Drawing.Size(175, 22);
            this.toolStripMenuItemRename.Text = "Rename";
            this.toolStripMenuItemRename.Click += new System.EventHandler(this.OnRenameNode);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(175, 22);
            this.toolStripMenuItem1.Text = "Delete";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.OnDeleteNodePermanently);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(175, 22);
            this.toolStripMenuItem2.Text = "Delete all but this";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.DeleteAllButThisNode);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(172, 6);
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.detailsToolStripMenuItem.Text = "Details";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuShowDetails);
            // 
            // contextMenuStripMesh
            // 
            this.contextMenuStripMesh.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRenameMesh,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator3,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripSeparator4,
            this.detailsToolStripMenuItem1});
            this.contextMenuStripMesh.Name = "contextMenuStripMesh";
            this.contextMenuStripMesh.Size = new System.Drawing.Size(174, 170);
            // 
            // toolStripMenuItemRenameMesh
            // 
            this.toolStripMenuItemRenameMesh.Name = "toolStripMenuItemRenameMesh";
            this.toolStripMenuItemRenameMesh.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItemRenameMesh.Text = "Rename";
            this.toolStripMenuItemRenameMesh.Click += new System.EventHandler(this.OnRenameMesh);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::open3mod.Properties.Resources.FailedToLoad;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.OnDeleteMesh);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(170, 6);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItem3.Text = "Generate Normals";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.OnGenerateMeshNormals);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Enabled = false;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItem4.Text = "Generate Tangents";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Enabled = false;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItem5.Text = "Generate UVs";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(170, 6);
            // 
            // detailsToolStripMenuItem1
            // 
            this.detailsToolStripMenuItem1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.detailsToolStripMenuItem1.Name = "detailsToolStripMenuItem1";
            this.detailsToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.detailsToolStripMenuItem1.Text = "Mesh Editor";
            this.detailsToolStripMenuItem1.Click += new System.EventHandler(this.OnContextMenuShowDetails);
            // 
            // meshInfoPopup
            // 
            this.meshInfoPopup.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.meshInfoPopup.Location = new System.Drawing.Point(223, 335);
            this.meshInfoPopup.Name = "meshInfoPopup";
            this.meshInfoPopup.Size = new System.Drawing.Size(88, 90);
            this.meshInfoPopup.TabIndex = 12;
            // 
            // nodeInfoPopup
            // 
            this.nodeInfoPopup.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nodeInfoPopup.Location = new System.Drawing.Point(223, 43);
            this.nodeInfoPopup.Name = "nodeInfoPopup";
            this.nodeInfoPopup.Size = new System.Drawing.Size(88, 90);
            this.nodeInfoPopup.TabIndex = 11;
            // 
            // HierarchyInspectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.Controls.Add(this.meshInfoPopup);
            this.Controls.Add(this.nodeInfoPopup);
            this.Controls.Add(this.labelHitCount);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this._tree);
            this.Controls.Add(this.panelHiddenInfo);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "HierarchyInspectionView";
            this.Size = new System.Drawing.Size(311, 654);
            this.panelHiddenInfo.ResumeLayout(false);
            this.panelHiddenInfo.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStripTreeNode.ResumeLayout(false);
            this.contextMenuStripMesh.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.TreeView _tree;
        private System.Windows.Forms.Label labelNodeStats;
        private System.Windows.Forms.Label labelHiddenCount;
        private System.Windows.Forms.Panel panelHiddenInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelHitCount;
        private System.Windows.Forms.ImageList imageListIconsHierarchy;
        private NodeInfoPopup nodeInfoPopup;
        private MeshInfoPopup meshInfoPopup;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTreeNode;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMesh;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem centerPivotToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRename;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRenameMesh;
    }
}
