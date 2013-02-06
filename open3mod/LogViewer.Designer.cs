namespace open3mod
{
    partial class LogViewer
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
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.checkBoxFilterError = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxFilterVerbose = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterInformation = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterWarning = new System.Windows.Forms.CheckBox();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.BackColor = System.Drawing.Color.White;
            this.richTextBox.DetectUrls = false;
            this.richTextBox.Location = new System.Drawing.Point(0, 0);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox.Size = new System.Drawing.Size(574, 522);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // checkBoxFilterError
            // 
            this.checkBoxFilterError.AutoSize = true;
            this.checkBoxFilterError.Checked = true;
            this.checkBoxFilterError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterError.Location = new System.Drawing.Point(21, 30);
            this.checkBoxFilterError.Name = "checkBoxFilterError";
            this.checkBoxFilterError.Size = new System.Drawing.Size(53, 17);
            this.checkBoxFilterError.TabIndex = 1;
            this.checkBoxFilterError.Text = "Errors";
            this.checkBoxFilterError.UseVisualStyleBackColor = true;
            this.checkBoxFilterError.CheckedChanged += new System.EventHandler(this.OnFilterChange);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxFilterVerbose);
            this.groupBox1.Controls.Add(this.checkBoxFilterInformation);
            this.groupBox1.Controls.Add(this.checkBoxFilterWarning);
            this.groupBox1.Controls.Add(this.checkBoxFilterError);
            this.groupBox1.Location = new System.Drawing.Point(12, 528);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 64);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter";
            // 
            // checkBoxFilterVerbose
            // 
            this.checkBoxFilterVerbose.AutoSize = true;
            this.checkBoxFilterVerbose.Checked = true;
            this.checkBoxFilterVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterVerbose.Location = new System.Drawing.Point(272, 30);
            this.checkBoxFilterVerbose.Name = "checkBoxFilterVerbose";
            this.checkBoxFilterVerbose.Size = new System.Drawing.Size(65, 17);
            this.checkBoxFilterVerbose.TabIndex = 3;
            this.checkBoxFilterVerbose.Text = "Verbose";
            this.checkBoxFilterVerbose.UseVisualStyleBackColor = true;
            this.checkBoxFilterVerbose.CheckedChanged += new System.EventHandler(this.OnFilterChange);
            // 
            // checkBoxFilterInformation
            // 
            this.checkBoxFilterInformation.AutoSize = true;
            this.checkBoxFilterInformation.Checked = true;
            this.checkBoxFilterInformation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterInformation.Location = new System.Drawing.Point(179, 30);
            this.checkBoxFilterInformation.Name = "checkBoxFilterInformation";
            this.checkBoxFilterInformation.Size = new System.Drawing.Size(78, 17);
            this.checkBoxFilterInformation.TabIndex = 3;
            this.checkBoxFilterInformation.Text = "Information";
            this.checkBoxFilterInformation.UseVisualStyleBackColor = true;
            this.checkBoxFilterInformation.CheckedChanged += new System.EventHandler(this.OnFilterChange);
            // 
            // checkBoxFilterWarning
            // 
            this.checkBoxFilterWarning.AutoSize = true;
            this.checkBoxFilterWarning.Checked = true;
            this.checkBoxFilterWarning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterWarning.Location = new System.Drawing.Point(93, 30);
            this.checkBoxFilterWarning.Name = "checkBoxFilterWarning";
            this.checkBoxFilterWarning.Size = new System.Drawing.Size(71, 17);
            this.checkBoxFilterWarning.TabIndex = 2;
            this.checkBoxFilterWarning.Text = "Warnings";
            this.checkBoxFilterWarning.UseVisualStyleBackColor = true;
            this.checkBoxFilterWarning.CheckedChanged += new System.EventHandler(this.OnFilterChange);
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearAll.Location = new System.Drawing.Point(469, 537);
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Size = new System.Drawing.Size(93, 23);
            this.buttonClearAll.TabIndex = 3;
            this.buttonClearAll.Text = "Clear All";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.OnClearAll);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(469, 566);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save to file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnSave);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            // 
            // LogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 608);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonClearAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.richTextBox);
            this.Name = "LogViewer";
            this.Text = "Log File Viewer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.CheckBox checkBoxFilterError;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxFilterVerbose;
        private System.Windows.Forms.CheckBox checkBoxFilterInformation;
        private System.Windows.Forms.CheckBox checkBoxFilterWarning;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}