namespace open3mod
{
    partial class InspectionView
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
            this.tabControlInfoViewPicker = new System.Windows.Forms.TabControl();
            this.tabPageTree = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelNodeStats = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.treeViewNodeGraph = new System.Windows.Forms.TreeView();
            this.tabPageTextures = new System.Windows.Forms.TabPage();
            this.textureFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPageMaterials = new System.Windows.Forms.TabPage();
            this.tabPageAnimations = new System.Windows.Forms.TabPage();
            this.tabControlInfoViewPicker.SuspendLayout();
            this.tabPageTree.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPageTextures.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlInfoViewPicker
            // 
            this.tabControlInfoViewPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlInfoViewPicker.Controls.Add(this.tabPageTree);
            this.tabControlInfoViewPicker.Controls.Add(this.tabPageTextures);
            this.tabControlInfoViewPicker.Controls.Add(this.tabPageMaterials);
            this.tabControlInfoViewPicker.Controls.Add(this.tabPageAnimations);
            this.tabControlInfoViewPicker.Location = new System.Drawing.Point(3, 3);
            this.tabControlInfoViewPicker.Multiline = true;
            this.tabControlInfoViewPicker.Name = "tabControlInfoViewPicker";
            this.tabControlInfoViewPicker.SelectedIndex = 0;
            this.tabControlInfoViewPicker.Size = new System.Drawing.Size(354, 688);
            this.tabControlInfoViewPicker.TabIndex = 2;
            // 
            // tabPageTree
            // 
            this.tabPageTree.BackColor = System.Drawing.Color.Silver;
            this.tabPageTree.Controls.Add(this.panel2);
            this.tabPageTree.Controls.Add(this.panel1);
            this.tabPageTree.Controls.Add(this.textBoxFilter);
            this.tabPageTree.Controls.Add(this.label1);
            this.tabPageTree.Controls.Add(this.treeViewNodeGraph);
            this.tabPageTree.Location = new System.Drawing.Point(4, 22);
            this.tabPageTree.Name = "tabPageTree";
            this.tabPageTree.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTree.Size = new System.Drawing.Size(346, 662);
            this.tabPageTree.TabIndex = 0;
            this.tabPageTree.Text = "Tree";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel2.Controls.Add(this.linkLabel1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 617);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(340, 21);
            this.panel2.TabIndex = 4;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(188, 5);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(100, 13);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Go to hidden nodes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "p nodes are permanently hidden";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel1.Controls.Add(this.labelNodeStats);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 638);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 21);
            this.panel1.TabIndex = 3;
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
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.Location = new System.Drawing.Point(53, 7);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(287, 20);
            this.textBoxFilter.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Filter";
            // 
            // treeViewNodeGraph
            // 
            this.treeViewNodeGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewNodeGraph.BackColor = System.Drawing.Color.White;
            this.treeViewNodeGraph.Location = new System.Drawing.Point(0, 36);
            this.treeViewNodeGraph.Name = "treeViewNodeGraph";
            this.treeViewNodeGraph.Size = new System.Drawing.Size(340, 575);
            this.treeViewNodeGraph.TabIndex = 0;
            this.treeViewNodeGraph.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AfterSelect);
            // 
            // tabPageTextures
            // 
            this.tabPageTextures.Controls.Add(this.textureFlowPanel);
            this.tabPageTextures.Location = new System.Drawing.Point(4, 22);
            this.tabPageTextures.Name = "tabPageTextures";
            this.tabPageTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTextures.Size = new System.Drawing.Size(346, 662);
            this.tabPageTextures.TabIndex = 1;
            this.tabPageTextures.Text = "Textures";
            this.tabPageTextures.UseVisualStyleBackColor = true;
            // 
            // textureFlowPanel
            // 
            this.textureFlowPanel.AutoScroll = true;
            this.textureFlowPanel.AutoScrollMinSize = new System.Drawing.Size(0, 1000);
            this.textureFlowPanel.AutoSize = true;
            this.textureFlowPanel.BackColor = System.Drawing.Color.Transparent;
            this.textureFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureFlowPanel.Location = new System.Drawing.Point(3, 3);
            this.textureFlowPanel.Name = "textureFlowPanel";
            this.textureFlowPanel.Size = new System.Drawing.Size(340, 656);
            this.textureFlowPanel.TabIndex = 0;
            // 
            // tabPageMaterials
            // 
            this.tabPageMaterials.Location = new System.Drawing.Point(4, 22);
            this.tabPageMaterials.Name = "tabPageMaterials";
            this.tabPageMaterials.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMaterials.Size = new System.Drawing.Size(346, 662);
            this.tabPageMaterials.TabIndex = 3;
            this.tabPageMaterials.Text = "Materials";
            this.tabPageMaterials.UseVisualStyleBackColor = true;
            // 
            // tabPageAnimations
            // 
            this.tabPageAnimations.Location = new System.Drawing.Point(4, 22);
            this.tabPageAnimations.Name = "tabPageAnimations";
            this.tabPageAnimations.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAnimations.Size = new System.Drawing.Size(346, 662);
            this.tabPageAnimations.TabIndex = 2;
            this.tabPageAnimations.Text = "Animations";
            this.tabPageAnimations.UseVisualStyleBackColor = true;
            // 
            // InspectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlInfoViewPicker);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "InspectionView";
            this.Size = new System.Drawing.Size(357, 694);
            this.tabControlInfoViewPicker.ResumeLayout(false);
            this.tabPageTree.ResumeLayout(false);
            this.tabPageTree.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPageTextures.ResumeLayout(false);
            this.tabPageTextures.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlInfoViewPicker;
        private System.Windows.Forms.TabPage tabPageTree;
        private System.Windows.Forms.TreeView treeViewNodeGraph;
        private System.Windows.Forms.TabPage tabPageTextures;
        private System.Windows.Forms.TabPage tabPageAnimations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageMaterials;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelNodeStats;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.FlowLayoutPanel textureFlowPanel;
    }
}
