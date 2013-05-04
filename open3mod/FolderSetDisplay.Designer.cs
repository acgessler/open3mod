namespace open3mod
{
    partial class FolderSetDisplay
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
            this.buttonRemoveFolder = new System.Windows.Forms.Button();
            this.buttonAddFolder = new System.Windows.Forms.Button();
            this.textBoxFolder = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.listBoxFolders = new System.Windows.Forms.ListBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // buttonRemoveFolder
            // 
            this.buttonRemoveFolder.Location = new System.Drawing.Point(397, 42);
            this.buttonRemoveFolder.Name = "buttonRemoveFolder";
            this.buttonRemoveFolder.Size = new System.Drawing.Size(89, 23);
            this.buttonRemoveFolder.TabIndex = 11;
            this.buttonRemoveFolder.Text = "Remove";
            this.buttonRemoveFolder.UseVisualStyleBackColor = true;
            this.buttonRemoveFolder.Click += new System.EventHandler(this.OnRemoveFolder);
            // 
            // buttonAddFolder
            // 
            this.buttonAddFolder.Location = new System.Drawing.Point(397, 13);
            this.buttonAddFolder.Name = "buttonAddFolder";
            this.buttonAddFolder.Size = new System.Drawing.Size(89, 23);
            this.buttonAddFolder.TabIndex = 10;
            this.buttonAddFolder.Text = "Add";
            this.buttonAddFolder.UseVisualStyleBackColor = true;
            this.buttonAddFolder.Click += new System.EventHandler(this.OnAddFolder);
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.Location = new System.Drawing.Point(14, 13);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(338, 20);
            this.textBoxFolder.TabIndex = 9;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(358, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(33, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnBrowse);
            // 
            // listBoxFolders
            // 
            this.listBoxFolders.FormattingEnabled = true;
            this.listBoxFolders.Location = new System.Drawing.Point(14, 42);
            this.listBoxFolders.Name = "listBoxFolders";
            this.listBoxFolders.Size = new System.Drawing.Size(377, 95);
            this.listBoxFolders.TabIndex = 7;
            this.listBoxFolders.SelectedIndexChanged += new System.EventHandler(this.OnSelectFolder);
            // 
            // FolderSetDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonRemoveFolder);
            this.Controls.Add(this.buttonAddFolder);
            this.Controls.Add(this.textBoxFolder);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBoxFolders);
            this.Name = "FolderSetDisplay";
            this.Size = new System.Drawing.Size(499, 152);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRemoveFolder;
        private System.Windows.Forms.Button buttonAddFolder;
        private System.Windows.Forms.TextBox textBoxFolder;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBoxFolders;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}
