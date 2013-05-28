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
            this.tabPageTextures = new System.Windows.Forms.TabPage();
            this.textureFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPageMaterials = new System.Windows.Forms.TabPage();
            this.materialFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPageAnimations = new System.Windows.Forms.TabPage();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControlInfoViewPicker.SuspendLayout();
            this.tabPageTextures.SuspendLayout();
            this.tabPageMaterials.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlInfoViewPicker
            // 
            this.tabControlInfoViewPicker.Controls.Add(this.tabPageTree);
            this.tabControlInfoViewPicker.Controls.Add(this.tabPageTextures);
            this.tabControlInfoViewPicker.Controls.Add(this.tabPageMaterials);
            this.tabControlInfoViewPicker.Controls.Add(this.tabPageAnimations);
            this.tabControlInfoViewPicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlInfoViewPicker.Location = new System.Drawing.Point(0, 0);
            this.tabControlInfoViewPicker.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlInfoViewPicker.Multiline = true;
            this.tabControlInfoViewPicker.Name = "tabControlInfoViewPicker";
            this.tabControlInfoViewPicker.SelectedIndex = 0;
            this.tabControlInfoViewPicker.Size = new System.Drawing.Size(357, 694);
            this.tabControlInfoViewPicker.TabIndex = 2;
            // 
            // tabPageTree
            // 
            this.tabPageTree.BackColor = System.Drawing.Color.Silver;
            this.tabPageTree.Location = new System.Drawing.Point(4, 22);
            this.tabPageTree.Name = "tabPageTree";
            this.tabPageTree.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTree.Size = new System.Drawing.Size(346, 662);
            this.tabPageTree.TabIndex = 0;
            this.tabPageTree.Text = "Tree";
            // 
            // tabPageTextures
            // 
            this.tabPageTextures.Controls.Add(this.textureFlowPanel);
            this.tabPageTextures.Location = new System.Drawing.Point(4, 22);
            this.tabPageTextures.Name = "tabPageTextures";
            this.tabPageTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTextures.Size = new System.Drawing.Size(349, 668);
            this.tabPageTextures.TabIndex = 1;
            this.tabPageTextures.Text = "Textures";
            this.tabPageTextures.UseVisualStyleBackColor = true;
            // 
            // textureFlowPanel
            // 
            this.textureFlowPanel.AllowDrop = true;
            this.textureFlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureFlowPanel.AutoScroll = true;
            this.textureFlowPanel.AutoScrollMinSize = new System.Drawing.Size(0, 1000);
            this.textureFlowPanel.AutoSize = true;
            this.textureFlowPanel.BackColor = System.Drawing.Color.Transparent;
            this.textureFlowPanel.Location = new System.Drawing.Point(3, 3);
            this.textureFlowPanel.Name = "textureFlowPanel";
            this.textureFlowPanel.Size = new System.Drawing.Size(343, 662);
            this.textureFlowPanel.TabIndex = 0;
            // 
            // tabPageMaterials
            // 
            this.tabPageMaterials.Controls.Add(this.materialFlowPanel);
            this.tabPageMaterials.Location = new System.Drawing.Point(4, 22);
            this.tabPageMaterials.Name = "tabPageMaterials";
            this.tabPageMaterials.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMaterials.Size = new System.Drawing.Size(346, 662);
            this.tabPageMaterials.TabIndex = 3;
            this.tabPageMaterials.Text = "Materials";
            this.tabPageMaterials.UseVisualStyleBackColor = true;
            // 
            // materialFlowPanel
            // 
            this.materialFlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialFlowPanel.AutoScroll = true;
            this.materialFlowPanel.AutoScrollMinSize = new System.Drawing.Size(0, 1000);
            this.materialFlowPanel.AutoSize = true;
            this.materialFlowPanel.BackColor = System.Drawing.Color.Transparent;
            this.materialFlowPanel.Location = new System.Drawing.Point(3, 3);
            this.materialFlowPanel.Name = "materialFlowPanel";
            this.materialFlowPanel.Size = new System.Drawing.Size(340, 656);
            this.materialFlowPanel.TabIndex = 1;
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
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // InspectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tabControlInfoViewPicker);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "InspectionView";
            this.Size = new System.Drawing.Size(357, 694);
            this.tabControlInfoViewPicker.ResumeLayout(false);
            this.tabPageTextures.ResumeLayout(false);
            this.tabPageTextures.PerformLayout();
            this.tabPageMaterials.ResumeLayout(false);
            this.tabPageMaterials.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlInfoViewPicker;
        private System.Windows.Forms.TabPage tabPageTextures;
        private System.Windows.Forms.TabPage tabPageAnimations;
        private System.Windows.Forms.TabPage tabPageMaterials;
        private System.Windows.Forms.FlowLayoutPanel textureFlowPanel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FlowLayoutPanel materialFlowPanel;
        private System.Windows.Forms.TabPage tabPageTree;
    }
}
