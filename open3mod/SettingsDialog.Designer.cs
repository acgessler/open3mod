namespace open3mod
{
    partial class SettingsDialog
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxBFCulling = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxSetLightingMode = new System.Windows.Forms.ComboBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxSetBackend = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxLinearMIP = new System.Windows.Forms.CheckBox();
            this.comboBoxTexResolution = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxSetTextureFilter = new System.Windows.Forms.ComboBox();
            this.comboBoxSetMultiSampling = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.labelPleaseRestart = new System.Windows.Forms.Label();
            this.folderSetDisplay1 = new open3mod.FolderSetDisplay();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(1, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(540, 407);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.checkBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(532, 381);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Rendering";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxBFCulling);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.comboBoxSetLightingMode);
            this.groupBox1.Controls.Add(this.checkBox3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBoxSetBackend);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.checkBoxLinearMIP);
            this.groupBox1.Controls.Add(this.comboBoxTexResolution);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBoxSetTextureFilter);
            this.groupBox1.Controls.Add(this.comboBoxSetMultiSampling);
            this.groupBox1.Location = new System.Drawing.Point(21, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(491, 293);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Custom Settings";
            // 
            // checkBoxBFCulling
            // 
            this.checkBoxBFCulling.AutoSize = true;
            this.checkBoxBFCulling.Checked = global::open3mod.GraphicsSettings.Default.BackFaceCulling;
            this.checkBoxBFCulling.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::open3mod.GraphicsSettings.Default, "BackFaceCulling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxBFCulling.Location = new System.Drawing.Point(188, 253);
            this.checkBoxBFCulling.Name = "checkBoxBFCulling";
            this.checkBoxBFCulling.Size = new System.Drawing.Size(112, 17);
            this.checkBoxBFCulling.TabIndex = 13;
            this.checkBoxBFCulling.Text = "Back-Face Culling";
            this.checkBoxBFCulling.UseVisualStyleBackColor = true;
            this.checkBoxBFCulling.CheckedChanged += new System.EventHandler(this.checkBoxBFCulling_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 201);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Lighting Mode";
            // 
            // comboBoxSetLightingMode
            // 
            this.comboBoxSetLightingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSetLightingMode.FormattingEnabled = true;
            this.comboBoxSetLightingMode.Items.AddRange(new object[] {
            "Low Quality",
            "High Quality"});
            this.comboBoxSetLightingMode.Location = new System.Drawing.Point(186, 201);
            this.comboBoxSetLightingMode.Name = "comboBoxSetLightingMode";
            this.comboBoxSetLightingMode.Size = new System.Drawing.Size(153, 21);
            this.comboBoxSetLightingMode.TabIndex = 12;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Enabled = false;
            this.checkBox3.Location = new System.Drawing.Point(20, 253);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(152, 17);
            this.checkBox3.TabIndex = 10;
            this.checkBox3.Text = "Order-correct transparency";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label5.Location = new System.Drawing.Point(185, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(301, 39);
            this.label5.TabIndex = 9;
            this.label5.Text = "Note: some of the settings below are ignored for the \"Legacy\" \r\nbackend. Also, th" +
    "e legacy backend supports no GPU-based\r\nskinning so animations may run slowly.";
            // 
            // comboBoxSetBackend
            // 
            this.comboBoxSetBackend.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSetBackend.FormattingEnabled = true;
            this.comboBoxSetBackend.Items.AddRange(new object[] {
            "OpenGl Legacy / Fixed Function Pipeline"});
            this.comboBoxSetBackend.Location = new System.Drawing.Point(186, 35);
            this.comboBoxSetBackend.Name = "comboBoxSetBackend";
            this.comboBoxSetBackend.Size = new System.Drawing.Size(248, 21);
            this.comboBoxSetBackend.TabIndex = 8;
            this.comboBoxSetBackend.SelectedIndexChanged += new System.EventHandler(this.OnChangeRenderingBackend);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Rendering Backend";
            // 
            // checkBoxLinearMIP
            // 
            this.checkBoxLinearMIP.AutoSize = true;
            this.checkBoxLinearMIP.Checked = global::open3mod.GraphicsSettings.Default.UseMips;
            this.checkBoxLinearMIP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLinearMIP.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::open3mod.GraphicsSettings.Default, "UseMips", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxLinearMIP.Location = new System.Drawing.Point(362, 178);
            this.checkBoxLinearMIP.Name = "checkBoxLinearMIP";
            this.checkBoxLinearMIP.Size = new System.Drawing.Size(72, 17);
            this.checkBoxLinearMIP.TabIndex = 6;
            this.checkBoxLinearMIP.Text = "Use MIPs";
            this.checkBoxLinearMIP.UseVisualStyleBackColor = true;
            this.checkBoxLinearMIP.CheckedChanged += new System.EventHandler(this.OnChangeMipSettings);
            // 
            // comboBoxTexResolution
            // 
            this.comboBoxTexResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTexResolution.FormattingEnabled = true;
            this.comboBoxTexResolution.Items.AddRange(new object[] {
            "Full",
            "Medium",
            "Low"});
            this.comboBoxTexResolution.Location = new System.Drawing.Point(186, 119);
            this.comboBoxTexResolution.Name = "comboBoxTexResolution";
            this.comboBoxTexResolution.Size = new System.Drawing.Size(153, 21);
            this.comboBoxTexResolution.TabIndex = 5;
            this.comboBoxTexResolution.SelectedIndexChanged += new System.EventHandler(this.OnChangeTextureResolution);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Texture Resolution";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Multisampling (MSAA) Mode";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Texture Filter";
            // 
            // comboBoxSetTextureFilter
            // 
            this.comboBoxSetTextureFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSetTextureFilter.FormattingEnabled = true;
            this.comboBoxSetTextureFilter.Items.AddRange(new object[] {
            "None",
            "Linear",
            "Anisotropic Low",
            "Anisotropic High"});
            this.comboBoxSetTextureFilter.Location = new System.Drawing.Point(186, 174);
            this.comboBoxSetTextureFilter.Name = "comboBoxSetTextureFilter";
            this.comboBoxSetTextureFilter.Size = new System.Drawing.Size(153, 21);
            this.comboBoxSetTextureFilter.TabIndex = 3;
            this.comboBoxSetTextureFilter.SelectedIndexChanged += new System.EventHandler(this.OnChangeTextureFilter);
            // 
            // comboBoxSetMultiSampling
            // 
            this.comboBoxSetMultiSampling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSetMultiSampling.FormattingEnabled = true;
            this.comboBoxSetMultiSampling.Items.AddRange(new object[] {
            "None",
            "Slight",
            "Normal",
            "Maximum"});
            this.comboBoxSetMultiSampling.Location = new System.Drawing.Point(186, 147);
            this.comboBoxSetMultiSampling.Name = "comboBoxSetMultiSampling";
            this.comboBoxSetMultiSampling.Size = new System.Drawing.Size(153, 21);
            this.comboBoxSetMultiSampling.TabIndex = 2;
            this.comboBoxSetMultiSampling.SelectedIndexChanged += new System.EventHandler(this.OnChangeMultiSamplingMode);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(27, 25);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(398, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Automatically select rendering settings based on measured system performance";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.folderSetDisplay1);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(532, 381);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Import";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(177, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Additional search folders for textures";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(532, 381);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Animation";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(232, 163);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "TODO";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(430, 434);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnOk);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // labelPleaseRestart
            // 
            this.labelPleaseRestart.AutoSize = true;
            this.labelPleaseRestart.ForeColor = System.Drawing.Color.Red;
            this.labelPleaseRestart.Location = new System.Drawing.Point(23, 434);
            this.labelPleaseRestart.Name = "labelPleaseRestart";
            this.labelPleaseRestart.Size = new System.Drawing.Size(232, 13);
            this.labelPleaseRestart.TabIndex = 2;
            this.labelPleaseRestart.Text = "Please restart the application to see all changes";
            this.labelPleaseRestart.Visible = false;
            // 
            // folderSetDisplay1
            // 
            this.folderSetDisplay1.Location = new System.Drawing.Point(7, 217);
            this.folderSetDisplay1.Name = "folderSetDisplay1";
            this.folderSetDisplay1.Size = new System.Drawing.Size(499, 152);
            this.folderSetDisplay1.TabIndex = 4;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 465);
            this.Controls.Add(this.labelPleaseRestart);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "open3mod Settings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBoxSetTextureFilter;
        private System.Windows.Forms.ComboBox comboBoxSetMultiSampling;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxLinearMIP;
        private System.Windows.Forms.ComboBox comboBoxTexResolution;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxSetBackend;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxSetLightingMode;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelPleaseRestart;
        private System.Windows.Forms.CheckBox checkBoxBFCulling;
        private System.Windows.Forms.Label label7;
        private FolderSetDisplay folderSetDisplay1;
    }
}