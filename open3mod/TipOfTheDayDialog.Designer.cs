namespace open3mod
{
    partial class TipOfTheDayDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TipOfTheDayDialog));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.checkBoxDoNotShowAgain = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTipText = new System.Windows.Forms.Label();
            this.pictureBoxTipPic = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTipPic)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(104, 223);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnNext);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 223);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Previous";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnPrevious);
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(430, 223);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(100, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // checkBoxDoNotShowAgain
            // 
            this.checkBoxDoNotShowAgain.AutoSize = true;
            this.checkBoxDoNotShowAgain.Checked = global::CoreSettings.CoreSettings.Default.ShowTipsOnStartup;
            this.checkBoxDoNotShowAgain.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ShowTipsOnStartup", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxDoNotShowAgain.Location = new System.Drawing.Point(321, 227);
            this.checkBoxDoNotShowAgain.Name = "checkBoxDoNotShowAgain";
            this.checkBoxDoNotShowAgain.Size = new System.Drawing.Size(103, 17);
            this.checkBoxDoNotShowAgain.TabIndex = 3;
            this.checkBoxDoNotShowAgain.Text = "Show on startup";
            this.checkBoxDoNotShowAgain.UseVisualStyleBackColor = true;
            this.checkBoxDoNotShowAgain.CheckedChanged += new System.EventHandler(this.OnChangeStartup);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.labelTipText);
            this.panel1.Controls.Add(this.pictureBoxTipPic);
            this.panel1.Location = new System.Drawing.Point(-1, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(543, 207);
            this.panel1.TabIndex = 4;
            // 
            // labelTipText
            // 
            this.labelTipText.AutoSize = true;
            this.labelTipText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTipText.Location = new System.Drawing.Point(229, 25);
            this.labelTipText.Name = "labelTipText";
            this.labelTipText.Size = new System.Drawing.Size(205, 96);
            this.labelTipText.TabIndex = 1;
            this.labelTipText.Text = "You can use the force to control \r\nalmost everything.\r\n\r\nJust make sure the force" +
    " is strong\r\nin you. Do not attempt to count\r\nMidi-Chlorians.";
            // 
            // pictureBoxTipPic
            // 
            this.pictureBoxTipPic.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxTipPic.Image")));
            this.pictureBoxTipPic.Location = new System.Drawing.Point(13, 12);
            this.pictureBoxTipPic.Name = "pictureBoxTipPic";
            this.pictureBoxTipPic.Size = new System.Drawing.Size(220, 179);
            this.pictureBoxTipPic.TabIndex = 0;
            this.pictureBoxTipPic.TabStop = false;
            // 
            // TipOfTheDayDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 258);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.checkBoxDoNotShowAgain);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TipOfTheDayDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tip of the Day";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClose);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTipPic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.CheckBox checkBoxDoNotShowAgain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBoxTipPic;
        private System.Windows.Forms.Label labelTipText;
    }
}