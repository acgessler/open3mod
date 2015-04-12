namespace open3mod
{
    sealed partial class NormalVectorGeneratorDialog
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.trackBarAngle = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.labelAngle = new System.Windows.Forms.Label();
            this.checkBoxRealtimePreview = new System.Windows.Forms.CheckBox();
            this.labelStatusText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngle)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(339, 134);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(128, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.OnOk);
            // 
            // buttonApply
            // 
            this.buttonApply.Enabled = false;
            this.buttonApply.Location = new System.Drawing.Point(138, 134);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(65, 23);
            this.buttonApply.TabIndex = 1;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.OnManualApply);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(258, 134);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.OnCancel);
            // 
            // trackBarAngle
            // 
            this.trackBarAngle.LargeChange = 10;
            this.trackBarAngle.Location = new System.Drawing.Point(24, 39);
            this.trackBarAngle.Maximum = 180;
            this.trackBarAngle.Name = "trackBarAngle";
            this.trackBarAngle.Size = new System.Drawing.Size(455, 45);
            this.trackBarAngle.TabIndex = 4;
            this.trackBarAngle.TickFrequency = 5;
            this.trackBarAngle.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarAngle.ValueChanged += new System.EventHandler(this.OnChangeSmoothness);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(278, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Smoothness (Maximum angle between neighboring faces)";
            // 
            // labelAngle
            // 
            this.labelAngle.AutoSize = true;
            this.labelAngle.Location = new System.Drawing.Point(230, 71);
            this.labelAngle.Name = "labelAngle";
            this.labelAngle.Size = new System.Drawing.Size(46, 13);
            this.labelAngle.TabIndex = 6;
            this.labelAngle.Text = "<Angle>";
            // 
            // checkBoxRealtimePreview
            // 
            this.checkBoxRealtimePreview.AutoSize = true;
            this.checkBoxRealtimePreview.Checked = global::CoreSettings.CoreSettings.Default.NormalVectorGeneratorRealtimePreview;
            this.checkBoxRealtimePreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRealtimePreview.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CoreSettings.CoreSettings.Default, "NormalVectorGeneratorRealtimePreview", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxRealtimePreview.Location = new System.Drawing.Point(24, 138);
            this.checkBoxRealtimePreview.Name = "checkBoxRealtimePreview";
            this.checkBoxRealtimePreview.Size = new System.Drawing.Size(108, 17);
            this.checkBoxRealtimePreview.TabIndex = 3;
            this.checkBoxRealtimePreview.Text = "Realtime Preview";
            this.checkBoxRealtimePreview.UseVisualStyleBackColor = true;
            this.checkBoxRealtimePreview.CheckedChanged += new System.EventHandler(this.CheckBoxRealtimePreviewCheckedChanged);
            // 
            // labelStatusText
            // 
            this.labelStatusText.AutoSize = true;
            this.labelStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusText.ForeColor = System.Drawing.Color.Green;
            this.labelStatusText.Location = new System.Drawing.Point(21, 111);
            this.labelStatusText.Name = "labelStatusText";
            this.labelStatusText.Size = new System.Drawing.Size(57, 13);
            this.labelStatusText.TabIndex = 7;
            this.labelStatusText.Text = "<Status>";
            // 
            // NormalVectorGeneratorDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(491, 169);
            this.Controls.Add(this.labelStatusText);
            this.Controls.Add(this.labelAngle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBarAngle);
            this.Controls.Add(this.checkBoxRealtimePreview);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NormalVectorGeneratorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generate Normal Vectors";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClose);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxRealtimePreview;
        private System.Windows.Forms.TrackBar trackBarAngle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelAngle;
        private System.Windows.Forms.Label labelStatusText;
    }
}