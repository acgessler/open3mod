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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.ExitAppCheckbox = new System.Windows.Forms.CheckBox();
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxmergeDuplicateVertices = new System.Windows.Forms.CheckBox();
            this.checkBoxOptimize = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxRemoveDegenerates = new System.Windows.Forms.CheckBox();
            this.checkBoxFixWinding = new System.Windows.Forms.CheckBox();
            this.checkBoxSortByPrimitiveType = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxGenerateTangentSpace = new System.Windows.Forms.CheckBox();
            this.checkBoxComputeNormals = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPageTextures = new System.Windows.Forms.TabPage();
            this.checkBoxLoadTextures = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPageLeapMotion = new System.Windows.Forms.TabPage();
            this.linkLabelLM = new System.Windows.Forms.LinkLabel();
            this.label9 = new System.Windows.Forms.Label();
            this.SmoothingGroup = new System.Windows.Forms.GroupBox();
            this.SmoothLabel = new System.Windows.Forms.Label();
            this.ResponsiveLabel = new System.Windows.Forms.Label();
            this.RotationSmoothing = new System.Windows.Forms.CheckBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.TranslationSmoothing = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.labelPleaseRestart = new System.Windows.Forms.Label();
            this.folderSetDisplaySearchPaths = new open3mod.FolderSetDisplay();
            this.tabControl.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPageTextures.SuspendLayout();
            this.tabPageLeapMotion.SuspendLayout();
            this.SmoothingGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageGeneral);
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPageTextures);
            this.tabControl.Controls.Add(this.tabPageLeapMotion);
            this.tabControl.Location = new System.Drawing.Point(1, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(540, 407);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.ExitAppCheckbox);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(532, 381);
            this.tabPageGeneral.TabIndex = 4;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // ExitAppCheckbox
            // 
            this.ExitAppCheckbox.AutoSize = true;
            this.ExitAppCheckbox.Checked = global::CoreSettings.CoreSettings.Default.ExitOnTabClosing;
            this.ExitAppCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ExitOnTabClosing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ExitAppCheckbox.Location = new System.Drawing.Point(21, 33);
            this.ExitAppCheckbox.Name = "ExitAppCheckbox";
            this.ExitAppCheckbox.Size = new System.Drawing.Size(215, 17);
            this.ExitAppCheckbox.TabIndex = 0;
            this.ExitAppCheckbox.Text = "Exit application upon closing the last tab";
            this.ExitAppCheckbox.UseVisualStyleBackColor = true;
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
            "OpenGL Legacy",
            "OpenGL Modern"});
            this.comboBoxSetBackend.Location = new System.Drawing.Point(186, 35);
            this.comboBoxSetBackend.Name = "comboBoxSetBackend";
            this.comboBoxSetBackend.Size = new System.Drawing.Size(248, 21);
            this.comboBoxSetBackend.TabIndex = 8;
            this.comboBoxSetBackend.SelectedIndexChanged += new System.EventHandler(this.OnChangeRenderingBackend);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 35);
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
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(532, 381);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Import";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxmergeDuplicateVertices);
            this.groupBox4.Controls.Add(this.checkBoxOptimize);
            this.groupBox4.Location = new System.Drawing.Point(21, 243);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(477, 89);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Optimization";
            // 
            // checkBoxmergeDuplicateVertices
            // 
            this.checkBoxmergeDuplicateVertices.AutoSize = true;
            this.checkBoxmergeDuplicateVertices.Checked = global::CoreSettings.CoreSettings.Default.ImportMergeDuplicates;
            this.checkBoxmergeDuplicateVertices.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxmergeDuplicateVertices.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ImportMergeDuplicates", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxmergeDuplicateVertices.Location = new System.Drawing.Point(26, 55);
            this.checkBoxmergeDuplicateVertices.Name = "checkBoxmergeDuplicateVertices";
            this.checkBoxmergeDuplicateVertices.Size = new System.Drawing.Size(248, 17);
            this.checkBoxmergeDuplicateVertices.TabIndex = 9;
            this.checkBoxmergeDuplicateVertices.Text = "Merge duplicate vertices (highly recommended)";
            this.checkBoxmergeDuplicateVertices.UseVisualStyleBackColor = true;
            // 
            // checkBoxOptimize
            // 
            this.checkBoxOptimize.AutoSize = true;
            this.checkBoxOptimize.Checked = global::CoreSettings.CoreSettings.Default.ImportOptimize;
            this.checkBoxOptimize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOptimize.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ImportOptimize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxOptimize.Location = new System.Drawing.Point(26, 32);
            this.checkBoxOptimize.Name = "checkBoxOptimize";
            this.checkBoxOptimize.Size = new System.Drawing.Size(246, 17);
            this.checkBoxOptimize.TabIndex = 8;
            this.checkBoxOptimize.Text = "Optimize geometry for vertex cache throughput";
            this.checkBoxOptimize.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxRemoveDegenerates);
            this.groupBox3.Controls.Add(this.checkBoxFixWinding);
            this.groupBox3.Controls.Add(this.checkBoxSortByPrimitiveType);
            this.groupBox3.Location = new System.Drawing.Point(21, 126);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(477, 96);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Cleanup";
            // 
            // checkBoxRemoveDegenerates
            // 
            this.checkBoxRemoveDegenerates.AutoSize = true;
            this.checkBoxRemoveDegenerates.Checked = global::CoreSettings.CoreSettings.Default.ImportRemoveDegenerates;
            this.checkBoxRemoveDegenerates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRemoveDegenerates.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ImportRemoveDegenerates", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxRemoveDegenerates.Location = new System.Drawing.Point(26, 19);
            this.checkBoxRemoveDegenerates.Name = "checkBoxRemoveDegenerates";
            this.checkBoxRemoveDegenerates.Size = new System.Drawing.Size(169, 17);
            this.checkBoxRemoveDegenerates.TabIndex = 9;
            this.checkBoxRemoveDegenerates.Text = "Remove degenerate geometry";
            this.checkBoxRemoveDegenerates.UseVisualStyleBackColor = true;
            // 
            // checkBoxFixWinding
            // 
            this.checkBoxFixWinding.AutoSize = true;
            this.checkBoxFixWinding.Checked = global::CoreSettings.CoreSettings.Default.ImportFixInfacing;
            this.checkBoxFixWinding.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFixWinding.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ImportFixInfacing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxFixWinding.Location = new System.Drawing.Point(26, 42);
            this.checkBoxFixWinding.Name = "checkBoxFixWinding";
            this.checkBoxFixWinding.Size = new System.Drawing.Size(271, 17);
            this.checkBoxFixWinding.TabIndex = 10;
            this.checkBoxFixWinding.Text = "Attempt to correct face orientation / normal direction";
            this.checkBoxFixWinding.UseVisualStyleBackColor = true;
            // 
            // checkBoxSortByPrimitiveType
            // 
            this.checkBoxSortByPrimitiveType.AutoSize = true;
            this.checkBoxSortByPrimitiveType.Checked = global::CoreSettings.CoreSettings.Default.ImportSortByPType;
            this.checkBoxSortByPrimitiveType.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ImportSortByPType", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxSortByPrimitiveType.Location = new System.Drawing.Point(26, 65);
            this.checkBoxSortByPrimitiveType.Name = "checkBoxSortByPrimitiveType";
            this.checkBoxSortByPrimitiveType.Size = new System.Drawing.Size(288, 17);
            this.checkBoxSortByPrimitiveType.TabIndex = 7;
            this.checkBoxSortByPrimitiveType.Text = "Sort meshes by primitive types (i.e. line, points, triangles)";
            this.checkBoxSortByPrimitiveType.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxGenerateTangentSpace);
            this.groupBox2.Controls.Add(this.checkBoxComputeNormals);
            this.groupBox2.Location = new System.Drawing.Point(21, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(477, 88);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Derived data";
            // 
            // checkBoxGenerateTangentSpace
            // 
            this.checkBoxGenerateTangentSpace.AutoSize = true;
            this.checkBoxGenerateTangentSpace.Checked = global::CoreSettings.CoreSettings.Default.ImportGenTangents;
            this.checkBoxGenerateTangentSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGenerateTangentSpace.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ImportGenTangents", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxGenerateTangentSpace.Location = new System.Drawing.Point(26, 54);
            this.checkBoxGenerateTangentSpace.Name = "checkBoxGenerateTangentSpace";
            this.checkBoxGenerateTangentSpace.Size = new System.Drawing.Size(231, 17);
            this.checkBoxGenerateTangentSpace.TabIndex = 6;
            this.checkBoxGenerateTangentSpace.Text = "Compute tangent-space for normal mapping";
            this.checkBoxGenerateTangentSpace.UseVisualStyleBackColor = true;
            this.checkBoxGenerateTangentSpace.CheckedChanged += new System.EventHandler(this.checkBoxGenerateTangentSpace_CheckedChanged);
            // 
            // checkBoxComputeNormals
            // 
            this.checkBoxComputeNormals.AutoSize = true;
            this.checkBoxComputeNormals.Checked = global::CoreSettings.CoreSettings.Default.ImportGenNormals;
            this.checkBoxComputeNormals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxComputeNormals.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "ImportGenNormals", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxComputeNormals.Location = new System.Drawing.Point(26, 31);
            this.checkBoxComputeNormals.Name = "checkBoxComputeNormals";
            this.checkBoxComputeNormals.Size = new System.Drawing.Size(259, 17);
            this.checkBoxComputeNormals.TabIndex = 5;
            this.checkBoxComputeNormals.Text = "Compute smooth normals where none are present";
            this.checkBoxComputeNormals.UseVisualStyleBackColor = true;
            this.checkBoxComputeNormals.CheckedChanged += new System.EventHandler(this.checkBoxComputeNormals_CheckedChanged);
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
            // tabPageTextures
            // 
            this.tabPageTextures.Controls.Add(this.checkBoxLoadTextures);
            this.tabPageTextures.Controls.Add(this.label7);
            this.tabPageTextures.Controls.Add(this.folderSetDisplaySearchPaths);
            this.tabPageTextures.Location = new System.Drawing.Point(4, 22);
            this.tabPageTextures.Name = "tabPageTextures";
            this.tabPageTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTextures.Size = new System.Drawing.Size(532, 381);
            this.tabPageTextures.TabIndex = 3;
            this.tabPageTextures.Text = "Textures";
            this.tabPageTextures.UseVisualStyleBackColor = true;
            // 
            // checkBoxLoadTextures
            // 
            this.checkBoxLoadTextures.AutoSize = true;
            this.checkBoxLoadTextures.Checked = global::CoreSettings.CoreSettings.Default.LoadTextures;
            this.checkBoxLoadTextures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLoadTextures.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "LoadTextures", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxLoadTextures.Location = new System.Drawing.Point(31, 34);
            this.checkBoxLoadTextures.Name = "checkBoxLoadTextures";
            this.checkBoxLoadTextures.Size = new System.Drawing.Size(94, 17);
            this.checkBoxLoadTextures.TabIndex = 7;
            this.checkBoxLoadTextures.Text = "Load Textures";
            this.checkBoxLoadTextures.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(177, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Additional search folders for textures";
            // 
            // tabPageLeapMotion
            // 
            this.tabPageLeapMotion.Controls.Add(this.linkLabelLM);
            this.tabPageLeapMotion.Controls.Add(this.label9);
            this.tabPageLeapMotion.Controls.Add(this.SmoothingGroup);
            this.tabPageLeapMotion.Location = new System.Drawing.Point(4, 22);
            this.tabPageLeapMotion.Name = "tabPageLeapMotion";
            this.tabPageLeapMotion.Size = new System.Drawing.Size(532, 381);
            this.tabPageLeapMotion.TabIndex = 4;
            this.tabPageLeapMotion.Text = "Leap Motion";
            this.tabPageLeapMotion.UseVisualStyleBackColor = true;
            // 
            // linkLabelLM
            // 
            this.linkLabelLM.AutoSize = true;
            this.linkLabelLM.Location = new System.Drawing.Point(157, 35);
            this.linkLabelLM.Name = "linkLabelLM";
            this.linkLabelLM.Size = new System.Drawing.Size(108, 13);
            this.linkLabelLM.TabIndex = 3;
            this.linkLabelLM.TabStop = true;
            this.linkLabelLM.Text = "www.leapmotion.com";
            this.linkLabelLM.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLMWebsite);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(27, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(432, 26);
            this.label9.TabIndex = 2;
            this.label9.Text = "open3mod supports using the LeapMotion (tm) tracking device to control the 3D vie" +
    "wport. \r\nFor more information, go to ";
            // 
            // SmoothingGroup
            // 
            this.SmoothingGroup.Controls.Add(this.SmoothLabel);
            this.SmoothingGroup.Controls.Add(this.ResponsiveLabel);
            this.SmoothingGroup.Controls.Add(this.RotationSmoothing);
            this.SmoothingGroup.Controls.Add(this.trackBar1);
            this.SmoothingGroup.Controls.Add(this.TranslationSmoothing);
            this.SmoothingGroup.Location = new System.Drawing.Point(21, 85);
            this.SmoothingGroup.Margin = new System.Windows.Forms.Padding(10);
            this.SmoothingGroup.Name = "SmoothingGroup";
            this.SmoothingGroup.Size = new System.Drawing.Size(494, 182);
            this.SmoothingGroup.TabIndex = 1;
            this.SmoothingGroup.TabStop = false;
            this.SmoothingGroup.Text = "Smoothing";
            // 
            // SmoothLabel
            // 
            this.SmoothLabel.AutoSize = true;
            this.SmoothLabel.Enabled = false;
            this.SmoothLabel.Location = new System.Drawing.Point(433, 150);
            this.SmoothLabel.Name = "SmoothLabel";
            this.SmoothLabel.Size = new System.Drawing.Size(43, 13);
            this.SmoothLabel.TabIndex = 4;
            this.SmoothLabel.Text = "Smooth";
            // 
            // ResponsiveLabel
            // 
            this.ResponsiveLabel.AutoSize = true;
            this.ResponsiveLabel.Enabled = false;
            this.ResponsiveLabel.Location = new System.Drawing.Point(16, 150);
            this.ResponsiveLabel.Name = "ResponsiveLabel";
            this.ResponsiveLabel.Size = new System.Drawing.Size(63, 13);
            this.ResponsiveLabel.TabIndex = 3;
            this.ResponsiveLabel.Text = "Responsive";
            // 
            // RotationSmoothing
            // 
            this.RotationSmoothing.AutoSize = true;
            this.RotationSmoothing.Checked = global::CoreSettings.CoreSettings.Default.Leap_RotationSmoothing;
            this.RotationSmoothing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RotationSmoothing.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "Leap_RotationSmoothing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RotationSmoothing.Enabled = false;
            this.RotationSmoothing.Location = new System.Drawing.Point(19, 55);
            this.RotationSmoothing.Name = "RotationSmoothing";
            this.RotationSmoothing.Size = new System.Drawing.Size(139, 17);
            this.RotationSmoothing.TabIndex = 2;
            this.RotationSmoothing.Text = "Smooth Hands Rotation";
            this.RotationSmoothing.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.Color.White;
            this.trackBar1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::CoreSettings.CoreSettings.Default, "Leap_SmoothingWindowSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.trackBar1.Enabled = false;
            this.trackBar1.Location = new System.Drawing.Point(9, 88);
            this.trackBar1.Maximum = 120;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(467, 45);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.Value = global::CoreSettings.CoreSettings.Default.Leap_SmoothingWindowSize;
            // 
            // TranslationSmoothing
            // 
            this.TranslationSmoothing.AutoSize = true;
            this.TranslationSmoothing.Checked = global::CoreSettings.CoreSettings.Default.Leap_TranslationSmoothing;
            this.TranslationSmoothing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TranslationSmoothing.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "Leap_TranslationSmoothing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TranslationSmoothing.Enabled = false;
            this.TranslationSmoothing.Location = new System.Drawing.Point(19, 32);
            this.TranslationSmoothing.Name = "TranslationSmoothing";
            this.TranslationSmoothing.Size = new System.Drawing.Size(154, 17);
            this.TranslationSmoothing.TabIndex = 0;
            this.TranslationSmoothing.Text = "Smooth Hands Movements";
            this.TranslationSmoothing.UseVisualStyleBackColor = true;
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
            // folderSetDisplaySearchPaths
            // 
            this.folderSetDisplaySearchPaths.Folders = new string[0];
            this.folderSetDisplaySearchPaths.Location = new System.Drawing.Point(17, 93);
            this.folderSetDisplaySearchPaths.Name = "folderSetDisplaySearchPaths";
            this.folderSetDisplaySearchPaths.Size = new System.Drawing.Size(499, 152);
            this.folderSetDisplaySearchPaths.TabIndex = 6;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 465);
            this.Controls.Add(this.labelPleaseRestart);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "open3mod Settings";
            this.tabControl.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPageTextures.ResumeLayout(false);
            this.tabPageTextures.PerformLayout();
            this.tabPageLeapMotion.ResumeLayout(false);
            this.tabPageLeapMotion.PerformLayout();
            this.SmoothingGroup.ResumeLayout(false);
            this.SmoothingGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
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
        private System.Windows.Forms.CheckBox checkBoxComputeNormals;
        private System.Windows.Forms.CheckBox checkBoxGenerateTangentSpace;
        private System.Windows.Forms.CheckBox checkBoxSortByPrimitiveType;
        private System.Windows.Forms.CheckBox checkBoxOptimize;
        private System.Windows.Forms.CheckBox checkBoxRemoveDegenerates;
        private System.Windows.Forms.CheckBox checkBoxFixWinding;
        private System.Windows.Forms.TabPage tabPageTextures;
        private FolderSetDisplay folderSetDisplaySearchPaths;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxmergeDuplicateVertices;
        private System.Windows.Forms.CheckBox checkBoxLoadTextures;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.CheckBox ExitAppCheckbox;
        private System.Windows.Forms.TabPage tabPageLeapMotion;
        private System.Windows.Forms.CheckBox TranslationSmoothing;
        private System.Windows.Forms.GroupBox SmoothingGroup;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.CheckBox RotationSmoothing;
        private System.Windows.Forms.Label SmoothLabel;
        private System.Windows.Forms.Label ResponsiveLabel;
        private System.Windows.Forms.LinkLabel linkLabelLM;
        private System.Windows.Forms.Label label9;
    }
}