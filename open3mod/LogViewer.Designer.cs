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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogViewer));
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxFilterError = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxFilterWarning = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxFilterInformation = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxFilterVerbose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxSource = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.BackColor = System.Drawing.Color.White;
            this.richTextBox.DetectUrls = false;
            this.richTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox.Location = new System.Drawing.Point(0, 27);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox.Size = new System.Drawing.Size(794, 426);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(793, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkBoxFilterError,
            this.checkBoxFilterWarning,
            this.checkBoxFilterInformation,
            this.checkBoxFilterVerbose});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.filterToolStripMenuItem.Text = "Filter";
            this.filterToolStripMenuItem.Click += new System.EventHandler(this.filterToolStripMenuItem_Click);
            // 
            // checkBoxFilterError
            // 
            this.checkBoxFilterError.Checked = true;
            this.checkBoxFilterError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterError.Name = "checkBoxFilterError";
            this.checkBoxFilterError.Size = new System.Drawing.Size(162, 22);
            this.checkBoxFilterError.Text = "Errors";
            // 
            // checkBoxFilterWarning
            // 
            this.checkBoxFilterWarning.Checked = true;
            this.checkBoxFilterWarning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterWarning.Name = "checkBoxFilterWarning";
            this.checkBoxFilterWarning.Size = new System.Drawing.Size(162, 22);
            this.checkBoxFilterWarning.Text = "Warnings";
            // 
            // checkBoxFilterInformation
            // 
            this.checkBoxFilterInformation.Checked = true;
            this.checkBoxFilterInformation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterInformation.Name = "checkBoxFilterInformation";
            this.checkBoxFilterInformation.Size = new System.Drawing.Size(162, 22);
            this.checkBoxFilterInformation.Text = "Information";
            // 
            // checkBoxFilterVerbose
            // 
            this.checkBoxFilterVerbose.Checked = true;
            this.checkBoxFilterVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterVerbose.Name = "checkBoxFilterVerbose";
            this.checkBoxFilterVerbose.Size = new System.Drawing.Size(162, 22);
            this.checkBoxFilterVerbose.Text = "Verbose (Debug)";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.saveToFileToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.OnClearAll);
            // 
            // saveToFileToolStripMenuItem
            // 
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.saveToFileToolStripMenuItem.Text = "Save to File";
            this.saveToFileToolStripMenuItem.Click += new System.EventHandler(this.OnSave);
            // 
            // comboBoxSource
            // 
            this.comboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSource.FormattingEnabled = true;
            this.comboBoxSource.Location = new System.Drawing.Point(241, 3);
            this.comboBoxSource.Name = "comboBoxSource";
            this.comboBoxSource.Size = new System.Drawing.Size(327, 21);
            this.comboBoxSource.TabIndex = 6;
            this.comboBoxSource.SelectedIndexChanged += new System.EventHandler(this.ChangeLogSource);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(159, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Show log for";
            // 
            // LogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 453);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxSource);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "LogViewer";
            this.Text = "Log File Viewer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkBoxFilterError;
        private System.Windows.Forms.ToolStripMenuItem checkBoxFilterWarning;
        private System.Windows.Forms.ToolStripMenuItem checkBoxFilterInformation;
        private System.Windows.Forms.ToolStripMenuItem checkBoxFilterVerbose;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToFileToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxSource;
        private System.Windows.Forms.Label label1;
    }
}