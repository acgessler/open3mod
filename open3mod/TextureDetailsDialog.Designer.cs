namespace open3mod
{
    partial class TextureDetailsDialog
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.checkBoxHasAlpha = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(665, 577);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(12, 17);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(101, 13);
            this.labelInfo.TabIndex = 3;
            this.labelInfo.Text = "Size: <W> x <H> px";
            // 
            // checkBoxHasAlpha
            // 
            this.checkBoxHasAlpha.AutoCheck = false;
            this.checkBoxHasAlpha.AutoSize = true;
            this.checkBoxHasAlpha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxHasAlpha.Location = new System.Drawing.Point(137, 15);
            this.checkBoxHasAlpha.Name = "checkBoxHasAlpha";
            this.checkBoxHasAlpha.Size = new System.Drawing.Size(114, 17);
            this.checkBoxHasAlpha.TabIndex = 4;
            this.checkBoxHasAlpha.Text = "Has Alpha Channel";
            this.checkBoxHasAlpha.UseVisualStyleBackColor = true;
            // 
            // TextureDetailsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 630);
            this.Controls.Add(this.checkBoxHasAlpha);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TextureDetailsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "<TexName> - Details";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.CheckBox checkBoxHasAlpha;
    }
}