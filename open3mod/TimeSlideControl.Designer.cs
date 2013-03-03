namespace open3mod
{
    partial class TimeSlideControl
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
            this.panelTime = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelTime
            // 
            this.panelTime.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panelTime.Location = new System.Drawing.Point(3, 8);
            this.panelTime.Name = "panelTime";
            this.panelTime.Size = new System.Drawing.Size(356, 52);
            this.panelTime.TabIndex = 0;
            // 
            // TimeSlideControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelTime);
            this.Name = "TimeSlideControl";
            this.Size = new System.Drawing.Size(362, 63);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTime;
    }
}
