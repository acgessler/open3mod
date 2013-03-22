namespace open3mod
{
    partial class TabUiSkeleton
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.inspectionView1 = new open3mod.InspectionView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::CoreSettings.CoreSettings.Default, "InspectorSplitterPos", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.inspectionView1);
            this.splitContainer.Size = new System.Drawing.Size(793, 696);
            this.splitContainer.SplitterDistance = global::CoreSettings.CoreSettings.Default.InspectorSplitterPos;
            this.splitContainer.SplitterWidth = 3;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnSplitterMove);
            // 
            // inspectionView1
            // 
            this.inspectionView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inspectionView1.Enabled = false;
            this.inspectionView1.Location = new System.Drawing.Point(0, 0);
            this.inspectionView1.Margin = new System.Windows.Forms.Padding(0);
            this.inspectionView1.Name = "inspectionView1";
            this.inspectionView1.Size = new System.Drawing.Size(282, 693);
            this.inspectionView1.TabIndex = 0;
            this.inspectionView1.Load += new System.EventHandler(this.OnLoad);
            // 
            // TabUiSkeleton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "TabUiSkeleton";
            this.Size = new System.Drawing.Size(793, 696);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private InspectionView inspectionView1;
    }
}
