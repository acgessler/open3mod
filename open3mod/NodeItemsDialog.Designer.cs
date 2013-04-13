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
            this.checkBoxShowAnimated = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.trafoMatrixViewControlGlobal = new open3mod.TrafoMatrixViewControl();
            this.trafoMatrixViewControlLocal = new open3mod.TrafoMatrixViewControl();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxShowAnimated
            // 
            this.checkBoxShowAnimated.AutoSize = true;
            this.checkBoxShowAnimated.Location = new System.Drawing.Point(18, 12);
            this.checkBoxShowAnimated.Name = "checkBoxShowAnimated";
            this.checkBoxShowAnimated.Size = new System.Drawing.Size(133, 17);
            this.checkBoxShowAnimated.TabIndex = 0;
            this.checkBoxShowAnimated.Text = "Show animated values";
            this.checkBoxShowAnimated.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trafoMatrixViewControlLocal);
            this.groupBox1.Location = new System.Drawing.Point(12, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 208);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Node Local Transformation";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.trafoMatrixViewControlGlobal);
            this.groupBox2.Location = new System.Drawing.Point(12, 284);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(368, 210);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Node Global Transformation";
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
            this.trafoMatrixViewControlLocal.Size = new System.Drawing.Size(352, 190);
            this.trafoMatrixViewControlLocal.TabIndex = 1;
            // 
            // NodeItemsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 508);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxShowAnimated);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NodeItemsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "<NodeName> Details";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxShowAnimated;
        private TrafoMatrixViewControl trafoMatrixViewControlLocal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private TrafoMatrixViewControl trafoMatrixViewControlGlobal;
    }
}