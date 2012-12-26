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
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.treeViewNodeGraph = new System.Windows.Forms.TreeView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Animations = new System.Windows.Forms.TabPage();
            this.tabControlInfoViewPicker.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlInfoViewPicker
            // 
            this.tabControlInfoViewPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlInfoViewPicker.Controls.Add(this.tabPage3);
            this.tabControlInfoViewPicker.Controls.Add(this.tabPage4);
            this.tabControlInfoViewPicker.Controls.Add(this.tabPage1);
            this.tabControlInfoViewPicker.Controls.Add(this.Animations);
            this.tabControlInfoViewPicker.Location = new System.Drawing.Point(3, 3);
            this.tabControlInfoViewPicker.Multiline = true;
            this.tabControlInfoViewPicker.Name = "tabControlInfoViewPicker";
            this.tabControlInfoViewPicker.SelectedIndex = 0;
            this.tabControlInfoViewPicker.Size = new System.Drawing.Size(462, 688);
            this.tabControlInfoViewPicker.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Silver;
            this.tabPage3.Controls.Add(this.textBoxFilter);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.treeViewNodeGraph);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(454, 662);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Tree";
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.Location = new System.Drawing.Point(53, 7);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(395, 20);
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
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // treeViewNodeGraph
            // 
            this.treeViewNodeGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewNodeGraph.Location = new System.Drawing.Point(0, 36);
            this.treeViewNodeGraph.Name = "treeViewNodeGraph";
            this.treeViewNodeGraph.Size = new System.Drawing.Size(458, 633);
            this.treeViewNodeGraph.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(454, 662);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Textures";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(454, 662);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Materials";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Animations
            // 
            this.Animations.Location = new System.Drawing.Point(4, 22);
            this.Animations.Name = "Animations";
            this.Animations.Padding = new System.Windows.Forms.Padding(3);
            this.Animations.Size = new System.Drawing.Size(454, 662);
            this.Animations.TabIndex = 2;
            this.Animations.Text = "Animations";
            this.Animations.UseVisualStyleBackColor = true;
            // 
            // InspectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlInfoViewPicker);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "InspectionView";
            this.Size = new System.Drawing.Size(465, 694);
            this.tabControlInfoViewPicker.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlInfoViewPicker;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TreeView treeViewNodeGraph;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage Animations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBoxFilter;
    }
}
