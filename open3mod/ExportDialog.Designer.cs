namespace open3mod
{
    partial class ExportDialog
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
            this.buttonSelectFolder = new System.Windows.Forms.Button();
            this.comboBoxExportFormats = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBarExport = new System.Windows.Forms.ProgressBar();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxCopyTexturesToFolder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxCopyTexturesToSubfolder = new System.Windows.Forms.CheckBox();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxNoOverwriteConfirm = new System.Windows.Forms.CheckBox();
            this.checkBoxOpenExportedFile = new System.Windows.Forms.CheckBox();
            this.checkBoxIncludeSceneHierarchy = new System.Windows.Forms.CheckBox();
            this.checkBoxIncludeAnimations = new System.Windows.Forms.CheckBox();
            this.checkBoxUseRelativeTexturePaths = new System.Windows.Forms.CheckBox();
            this.buttonExportRun = new System.Windows.Forms.Button();
            this.labelExportStatus = new System.Windows.Forms.Label();
            this.textBoxExportLog = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelectFolder
            // 
            this.buttonSelectFolder.Location = new System.Drawing.Point(505, 25);
            this.buttonSelectFolder.Name = "buttonSelectFolder";
            this.buttonSelectFolder.Size = new System.Drawing.Size(40, 23);
            this.buttonSelectFolder.TabIndex = 1;
            this.buttonSelectFolder.Text = "...";
            this.buttonSelectFolder.UseVisualStyleBackColor = true;
            this.buttonSelectFolder.Click += new System.EventHandler(this.buttonSelectFolder_Click);
            // 
            // comboBoxExportFormats
            // 
            this.comboBoxExportFormats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExportFormats.FormattingEnabled = true;
            this.comboBoxExportFormats.Location = new System.Drawing.Point(105, 29);
            this.comboBoxExportFormats.Name = "comboBoxExportFormats";
            this.comboBoxExportFormats.Size = new System.Drawing.Size(348, 21);
            this.comboBoxExportFormats.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "File Format";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Path";
            // 
            // progressBarExport
            // 
            this.progressBarExport.Location = new System.Drawing.Point(136, 490);
            this.progressBarExport.Name = "progressBarExport";
            this.progressBarExport.Size = new System.Drawing.Size(464, 23);
            this.progressBarExport.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarExport.TabIndex = 7;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select output folder";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "File name";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(77, 58);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(421, 20);
            this.textBoxFileName.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxCopyTexturesToFolder);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.checkBoxCopyTexturesToSubfolder);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxFileName);
            this.groupBox1.Controls.Add(this.buttonSelectFolder);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxPath);
            this.groupBox1.Location = new System.Drawing.Point(31, 210);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(569, 166);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // textBoxCopyTexturesToFolder
            // 
            this.textBoxCopyTexturesToFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::open3mod.ExportSettings.Default, "CopyTexturesToFolder_Target", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxCopyTexturesToFolder.Location = new System.Drawing.Point(184, 132);
            this.textBoxCopyTexturesToFolder.Name = "textBoxCopyTexturesToFolder";
            this.textBoxCopyTexturesToFolder.Size = new System.Drawing.Size(314, 20);
            this.textBoxCopyTexturesToFolder.TabIndex = 11;
            this.textBoxCopyTexturesToFolder.Text = global::open3mod.ExportSettings.Default.CopyTexturesToFolder_Target;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(8, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(405, 26);
            this.label4.TabIndex = 10;
            this.label4.Text = "Note: some file formats (such as Obj) use multiple files for one scene. In such a" +
    " case,\r\nthe name given here is the name of the primary file.";
            // 
            // checkBoxCopyTexturesToSubfolder
            // 
            this.checkBoxCopyTexturesToSubfolder.AutoSize = true;
            this.checkBoxCopyTexturesToSubfolder.Checked = global::open3mod.ExportSettings.Default.CopyTexturesToFolder;
            this.checkBoxCopyTexturesToSubfolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCopyTexturesToSubfolder.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::open3mod.ExportSettings.Default, "CopyTexturesToFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxCopyTexturesToSubfolder.Location = new System.Drawing.Point(11, 135);
            this.checkBoxCopyTexturesToSubfolder.Name = "checkBoxCopyTexturesToSubfolder";
            this.checkBoxCopyTexturesToSubfolder.Size = new System.Drawing.Size(151, 17);
            this.checkBoxCopyTexturesToSubfolder.TabIndex = 7;
            this.checkBoxCopyTexturesToSubfolder.Text = "Copy textures to sub-folder";
            this.checkBoxCopyTexturesToSubfolder.UseVisualStyleBackColor = true;
            // 
            // textBoxPath
            // 
            this.textBoxPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::open3mod.ExportSettings.Default, "OutputPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxPath.Location = new System.Drawing.Point(77, 27);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(421, 20);
            this.textBoxPath.TabIndex = 2;
            this.textBoxPath.Text = global::open3mod.ExportSettings.Default.OutputPath;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxNoOverwriteConfirm);
            this.groupBox2.Controls.Add(this.checkBoxOpenExportedFile);
            this.groupBox2.Controls.Add(this.checkBoxIncludeSceneHierarchy);
            this.groupBox2.Controls.Add(this.checkBoxIncludeAnimations);
            this.groupBox2.Controls.Add(this.checkBoxUseRelativeTexturePaths);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.comboBoxExportFormats);
            this.groupBox2.Location = new System.Drawing.Point(31, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(569, 180);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Export ";
            // 
            // checkBoxNoOverwriteConfirm
            // 
            this.checkBoxNoOverwriteConfirm.AutoSize = true;
            this.checkBoxNoOverwriteConfirm.Location = new System.Drawing.Point(21, 151);
            this.checkBoxNoOverwriteConfirm.Name = "checkBoxNoOverwriteConfirm";
            this.checkBoxNoOverwriteConfirm.Size = new System.Drawing.Size(189, 17);
            this.checkBoxNoOverwriteConfirm.TabIndex = 17;
            this.checkBoxNoOverwriteConfirm.Text = "Overwrite files without confirmation";
            this.checkBoxNoOverwriteConfirm.UseVisualStyleBackColor = true;
            // 
            // checkBoxOpenExportedFile
            // 
            this.checkBoxOpenExportedFile.AutoSize = true;
            this.checkBoxOpenExportedFile.Checked = global::open3mod.ExportSettings.Default.OpenExportedFileInViewer;
            this.checkBoxOpenExportedFile.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::open3mod.ExportSettings.Default, "OpenExportedFileInViewer", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxOpenExportedFile.Location = new System.Drawing.Point(21, 128);
            this.checkBoxOpenExportedFile.Name = "checkBoxOpenExportedFile";
            this.checkBoxOpenExportedFile.Size = new System.Drawing.Size(157, 17);
            this.checkBoxOpenExportedFile.TabIndex = 16;
            this.checkBoxOpenExportedFile.Text = "Open exported file in viewer";
            this.checkBoxOpenExportedFile.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeSceneHierarchy
            // 
            this.checkBoxIncludeSceneHierarchy.AutoSize = true;
            this.checkBoxIncludeSceneHierarchy.Checked = global::open3mod.ExportSettings.Default.IncludeSceneHierarchy;
            this.checkBoxIncludeSceneHierarchy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIncludeSceneHierarchy.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::open3mod.ExportSettings.Default, "IncludeSceneHierarchy", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxIncludeSceneHierarchy.Location = new System.Drawing.Point(237, 71);
            this.checkBoxIncludeSceneHierarchy.Name = "checkBoxIncludeSceneHierarchy";
            this.checkBoxIncludeSceneHierarchy.Size = new System.Drawing.Size(139, 17);
            this.checkBoxIncludeSceneHierarchy.TabIndex = 10;
            this.checkBoxIncludeSceneHierarchy.Text = "Include scene hierarchy";
            this.checkBoxIncludeSceneHierarchy.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeAnimations
            // 
            this.checkBoxIncludeAnimations.AutoSize = true;
            this.checkBoxIncludeAnimations.Checked = global::open3mod.ExportSettings.Default.IncludeAnimations;
            this.checkBoxIncludeAnimations.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIncludeAnimations.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::open3mod.ExportSettings.Default, "IncludeAnimations", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxIncludeAnimations.Location = new System.Drawing.Point(237, 94);
            this.checkBoxIncludeAnimations.Name = "checkBoxIncludeAnimations";
            this.checkBoxIncludeAnimations.Size = new System.Drawing.Size(114, 17);
            this.checkBoxIncludeAnimations.TabIndex = 9;
            this.checkBoxIncludeAnimations.Text = "Include animations";
            this.checkBoxIncludeAnimations.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseRelativeTexturePaths
            // 
            this.checkBoxUseRelativeTexturePaths.AutoSize = true;
            this.checkBoxUseRelativeTexturePaths.Checked = global::open3mod.ExportSettings.Default.UseRelativeTexturePaths;
            this.checkBoxUseRelativeTexturePaths.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUseRelativeTexturePaths.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::open3mod.ExportSettings.Default, "UseRelativeTexturePaths", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxUseRelativeTexturePaths.Location = new System.Drawing.Point(21, 71);
            this.checkBoxUseRelativeTexturePaths.Name = "checkBoxUseRelativeTexturePaths";
            this.checkBoxUseRelativeTexturePaths.Size = new System.Drawing.Size(146, 17);
            this.checkBoxUseRelativeTexturePaths.TabIndex = 8;
            this.checkBoxUseRelativeTexturePaths.Text = "Use relative texture paths";
            this.checkBoxUseRelativeTexturePaths.UseVisualStyleBackColor = true;
            // 
            // buttonExportRun
            // 
            this.buttonExportRun.Location = new System.Drawing.Point(35, 490);
            this.buttonExportRun.Name = "buttonExportRun";
            this.buttonExportRun.Size = new System.Drawing.Size(95, 23);
            this.buttonExportRun.TabIndex = 12;
            this.buttonExportRun.Text = "Export";
            this.buttonExportRun.UseVisualStyleBackColor = true;
            this.buttonExportRun.Click += new System.EventHandler(this.buttonExport);
            // 
            // labelExportStatus
            // 
            this.labelExportStatus.AutoSize = true;
            this.labelExportStatus.Location = new System.Drawing.Point(139, 426);
            this.labelExportStatus.Name = "labelExportStatus";
            this.labelExportStatus.Size = new System.Drawing.Size(0, 13);
            this.labelExportStatus.TabIndex = 13;
            // 
            // textBoxExportLog
            // 
            this.textBoxExportLog.Location = new System.Drawing.Point(31, 382);
            this.textBoxExportLog.Multiline = true;
            this.textBoxExportLog.Name = "textBoxExportLog";
            this.textBoxExportLog.ReadOnly = true;
            this.textBoxExportLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxExportLog.Size = new System.Drawing.Size(569, 91);
            this.textBoxExportLog.TabIndex = 14;
            // 
            // ExportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 528);
            this.Controls.Add(this.textBoxExportLog);
            this.Controls.Add(this.labelExportStatus);
            this.Controls.Add(this.buttonExportRun);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBarExport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ExportDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSelectFolder;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.ComboBox comboBoxExportFormats;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBarExport;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxCopyTexturesToFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxCopyTexturesToSubfolder;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxUseRelativeTexturePaths;
        private System.Windows.Forms.CheckBox checkBoxIncludeSceneHierarchy;
        private System.Windows.Forms.CheckBox checkBoxIncludeAnimations;
        private System.Windows.Forms.Button buttonExportRun;
        private System.Windows.Forms.Label labelExportStatus;
        private System.Windows.Forms.TextBox textBoxExportLog;
        private System.Windows.Forms.CheckBox checkBoxNoOverwriteConfirm;
        private System.Windows.Forms.CheckBox checkBoxOpenExportedFile;
    }
}