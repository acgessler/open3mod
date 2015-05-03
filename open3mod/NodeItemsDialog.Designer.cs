namespace open3mod
{
    partial class NodeItemsDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelMeshesTotal = new System.Windows.Forms.Label();
            this.labelMeshesDirect = new System.Windows.Forms.Label();
            this.labelChildrenTotal = new System.Windows.Forms.Label();
            this.labelChildrenDirect = new System.Windows.Forms.Label();
            this.checkBoxShowGlobalTransformation = new System.Windows.Forms.CheckBox();
            this.checkBoxShowAnimated = new System.Windows.Forms.CheckBox();
            this.trafoMatrixViewControlGlobal = new open3mod.TrafoMatrixViewControl();
            this.trafoMatrixViewControlLocal = new open3mod.TrafoMatrixViewControl();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trafoMatrixViewControlLocal);
            this.groupBox1.Location = new System.Drawing.Point(12, 137);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 210);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local Transformation";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.trafoMatrixViewControlGlobal);
            this.groupBox2.Location = new System.Drawing.Point(12, 392);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(368, 210);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Global Transformation";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Children (direct):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Children (total):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(190, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Meshes (owned):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(190, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Meshes (total):";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelMeshesTotal);
            this.groupBox3.Controls.Add(this.labelMeshesDirect);
            this.groupBox3.Controls.Add(this.labelChildrenTotal);
            this.groupBox3.Controls.Add(this.labelChildrenDirect);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(368, 82);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Statistics";
            // 
            // labelMeshesTotal
            // 
            this.labelMeshesTotal.AutoSize = true;
            this.labelMeshesTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMeshesTotal.Location = new System.Drawing.Point(278, 54);
            this.labelMeshesTotal.Name = "labelMeshesTotal";
            this.labelMeshesTotal.Size = new System.Drawing.Size(52, 13);
            this.labelMeshesTotal.TabIndex = 11;
            this.labelMeshesTotal.Text = "<numC>";
            // 
            // labelMeshesDirect
            // 
            this.labelMeshesDirect.AutoSize = true;
            this.labelMeshesDirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMeshesDirect.Location = new System.Drawing.Point(278, 28);
            this.labelMeshesDirect.Name = "labelMeshesDirect";
            this.labelMeshesDirect.Size = new System.Drawing.Size(52, 13);
            this.labelMeshesDirect.TabIndex = 10;
            this.labelMeshesDirect.Text = "<numC>";
            // 
            // labelChildrenTotal
            // 
            this.labelChildrenTotal.AutoSize = true;
            this.labelChildrenTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChildrenTotal.Location = new System.Drawing.Point(105, 54);
            this.labelChildrenTotal.Name = "labelChildrenTotal";
            this.labelChildrenTotal.Size = new System.Drawing.Size(52, 13);
            this.labelChildrenTotal.TabIndex = 9;
            this.labelChildrenTotal.Text = "<numC>";
            // 
            // labelChildrenDirect
            // 
            this.labelChildrenDirect.AutoSize = true;
            this.labelChildrenDirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChildrenDirect.Location = new System.Drawing.Point(105, 28);
            this.labelChildrenDirect.Name = "labelChildrenDirect";
            this.labelChildrenDirect.Size = new System.Drawing.Size(52, 13);
            this.labelChildrenDirect.TabIndex = 8;
            this.labelChildrenDirect.Text = "<numC>";
            // 
            // checkBoxShowGlobalTransformation
            // 
            this.checkBoxShowGlobalTransformation.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxShowGlobalTransformation.AutoSize = true;
            this.checkBoxShowGlobalTransformation.Checked = global::CoreSettings.CoreSettings.Default.ShowGlobalTrafo;
            this.checkBoxShowGlobalTransformation.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ShowGlobalTrafo", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowGlobalTransformation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxShowGlobalTransformation.Location = new System.Drawing.Point(218, 361);
            this.checkBoxShowGlobalTransformation.Name = "checkBoxShowGlobalTransformation";
            this.checkBoxShowGlobalTransformation.Size = new System.Drawing.Size(162, 23);
            this.checkBoxShowGlobalTransformation.TabIndex = 9;
            this.checkBoxShowGlobalTransformation.Text = "Show Global Transformation ...";
            this.checkBoxShowGlobalTransformation.UseVisualStyleBackColor = true;
            this.checkBoxShowGlobalTransformation.CheckedChanged += new System.EventHandler(this.OnToggleShowGlobalTrafo);
            // 
            // checkBoxShowAnimated
            // 
            this.checkBoxShowAnimated.AutoSize = true;
            this.checkBoxShowAnimated.Checked = global::CoreSettings.CoreSettings.Default.NodeInfoShowAnimatedTrafo;
            this.checkBoxShowAnimated.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowAnimated.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "NodeInfoShowAnimatedTrafo", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowAnimated.Location = new System.Drawing.Point(247, 114);
            this.checkBoxShowAnimated.Name = "checkBoxShowAnimated";
            this.checkBoxShowAnimated.Size = new System.Drawing.Size(133, 17);
            this.checkBoxShowAnimated.TabIndex = 0;
            this.checkBoxShowAnimated.Text = "Show animated values";
            this.checkBoxShowAnimated.UseVisualStyleBackColor = true;
            this.checkBoxShowAnimated.CheckedChanged += new System.EventHandler(this.OnChangeAnimationState);
            // 
            // trafoMatrixViewControlGlobal
            // 
            this.trafoMatrixViewControlGlobal.Location = new System.Drawing.Point(6, 20);
            this.trafoMatrixViewControlGlobal.Name = "trafoMatrixViewControlGlobal";
            this.trafoMatrixViewControlGlobal.Size = new System.Drawing.Size(352, 188);
            this.trafoMatrixViewControlGlobal.TabIndex = 1;
            // 
            // trafoMatrixViewControlLocal
            // 
            this.trafoMatrixViewControlLocal.Location = new System.Drawing.Point(6, 20);
            this.trafoMatrixViewControlLocal.Name = "trafoMatrixViewControlLocal";
            this.trafoMatrixViewControlLocal.Size = new System.Drawing.Size(352, 183);
            this.trafoMatrixViewControlLocal.TabIndex = 1;
            // 
            // NodeItemsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 615);
            this.Controls.Add(this.checkBoxShowGlobalTransformation);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxShowAnimated);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NodeItemsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "<NodeName> Details";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxShowAnimated;
        private TrafoMatrixViewControl trafoMatrixViewControlLocal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private TrafoMatrixViewControl trafoMatrixViewControlGlobal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelChildrenDirect;
        private System.Windows.Forms.Label labelChildrenTotal;
        private System.Windows.Forms.Label labelMeshesTotal;
        private System.Windows.Forms.Label labelMeshesDirect;
        private System.Windows.Forms.CheckBox checkBoxShowGlobalTransformation;
    }
}