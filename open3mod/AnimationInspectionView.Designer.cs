namespace open3mod
{
    sealed partial class AnimationInspectionView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimationInspectionView));
            this.textBoxGoto = new System.Windows.Forms.TextBox();
            this.labelGoto = new System.Windows.Forms.Label();
            this.buttonFaster = new System.Windows.Forms.Button();
            this.buttonSlower = new System.Windows.Forms.Button();
            this.checkBoxLoop = new System.Windows.Forms.CheckBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxAnimations = new System.Windows.Forms.ListBox();
            this.panelAnimTools = new System.Windows.Forms.Panel();
            this.labelGotoError = new System.Windows.Forms.Label();
            this.labelSpeedValue = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.timeSlideControl = new open3mod.TimeSlideControl();
            this.contextMenuStripAnims = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelAnimTools.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStripAnims.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxGoto
            // 
            this.textBoxGoto.Location = new System.Drawing.Point(41, 193);
            this.textBoxGoto.Name = "textBoxGoto";
            this.textBoxGoto.Size = new System.Drawing.Size(78, 20);
            this.textBoxGoto.TabIndex = 16;
            this.textBoxGoto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnGoTo);
            // 
            // labelGoto
            // 
            this.labelGoto.AutoSize = true;
            this.labelGoto.Location = new System.Drawing.Point(2, 196);
            this.labelGoto.Name = "labelGoto";
            this.labelGoto.Size = new System.Drawing.Size(33, 13);
            this.labelGoto.TabIndex = 15;
            this.labelGoto.Text = "Go to";
            // 
            // buttonFaster
            // 
            this.buttonFaster.FlatAppearance.BorderSize = 2;
            this.buttonFaster.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.buttonFaster.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonFaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFaster.Location = new System.Drawing.Point(163, 18);
            this.buttonFaster.Name = "buttonFaster";
            this.buttonFaster.Size = new System.Drawing.Size(60, 40);
            this.buttonFaster.TabIndex = 14;
            this.buttonFaster.Text = "Faster";
            this.buttonFaster.UseVisualStyleBackColor = true;
            this.buttonFaster.Click += new System.EventHandler(this.OnFaster);
            // 
            // buttonSlower
            // 
            this.buttonSlower.FlatAppearance.BorderSize = 2;
            this.buttonSlower.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.buttonSlower.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonSlower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSlower.Location = new System.Drawing.Point(16, 18);
            this.buttonSlower.Name = "buttonSlower";
            this.buttonSlower.Size = new System.Drawing.Size(60, 40);
            this.buttonSlower.TabIndex = 13;
            this.buttonSlower.Text = "Slower";
            this.buttonSlower.UseVisualStyleBackColor = true;
            this.buttonSlower.Click += new System.EventHandler(this.OnSlower);
            // 
            // checkBoxLoop
            // 
            this.checkBoxLoop.AutoSize = true;
            this.checkBoxLoop.Location = new System.Drawing.Point(6, 1);
            this.checkBoxLoop.Name = "checkBoxLoop";
            this.checkBoxLoop.Size = new System.Drawing.Size(50, 17);
            this.checkBoxLoop.TabIndex = 12;
            this.checkBoxLoop.Text = "Loop";
            this.checkBoxLoop.UseVisualStyleBackColor = true;
            this.checkBoxLoop.CheckedChanged += new System.EventHandler(this.OnChangeLooping);
            // 
            // buttonPlay
            // 
            this.buttonPlay.FlatAppearance.BorderSize = 2;
            this.buttonPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.buttonPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlay.Location = new System.Drawing.Point(82, 3);
            this.buttonPlay.MaximumSize = new System.Drawing.Size(100, 100);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(75, 68);
            this.buttonPlay.TabIndex = 11;
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.OnPlay);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Available Animations:";
            // 
            // listBoxAnimations
            // 
            this.listBoxAnimations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAnimations.FormattingEnabled = true;
            this.listBoxAnimations.Location = new System.Drawing.Point(3, 23);
            this.listBoxAnimations.Name = "listBoxAnimations";
            this.listBoxAnimations.ScrollAlwaysVisible = true;
            this.listBoxAnimations.Size = new System.Drawing.Size(335, 186);
            this.listBoxAnimations.TabIndex = 9;
            this.listBoxAnimations.SelectedIndexChanged += new System.EventHandler(this.OnChangeSelectedAnimation);
            this.listBoxAnimations.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnAnimationContextMenu);
            // 
            // panelAnimTools
            // 
            this.panelAnimTools.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAnimTools.Controls.Add(this.labelGotoError);
            this.panelAnimTools.Controls.Add(this.labelSpeedValue);
            this.panelAnimTools.Controls.Add(this.panel1);
            this.panelAnimTools.Controls.Add(this.labelSpeed);
            this.panelAnimTools.Controls.Add(this.textBoxGoto);
            this.panelAnimTools.Controls.Add(this.timeSlideControl);
            this.panelAnimTools.Controls.Add(this.checkBoxLoop);
            this.panelAnimTools.Controls.Add(this.labelGoto);
            this.panelAnimTools.Location = new System.Drawing.Point(6, 215);
            this.panelAnimTools.Name = "panelAnimTools";
            this.panelAnimTools.Size = new System.Drawing.Size(332, 458);
            this.panelAnimTools.TabIndex = 18;
            // 
            // labelGotoError
            // 
            this.labelGotoError.AutoSize = true;
            this.labelGotoError.Location = new System.Drawing.Point(125, 196);
            this.labelGotoError.Name = "labelGotoError";
            this.labelGotoError.Size = new System.Drawing.Size(0, 13);
            this.labelGotoError.TabIndex = 21;
            // 
            // labelSpeedValue
            // 
            this.labelSpeedValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSpeedValue.AutoSize = true;
            this.labelSpeedValue.Location = new System.Drawing.Point(291, 2);
            this.labelSpeedValue.Name = "labelSpeedValue";
            this.labelSpeedValue.Size = new System.Drawing.Size(0, 13);
            this.labelSpeedValue.TabIndex = 20;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel1.Controls.Add(this.buttonFaster);
            this.panel1.Controls.Add(this.buttonSlower);
            this.panel1.Controls.Add(this.buttonPlay);
            this.panel1.Location = new System.Drawing.Point(41, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(242, 77);
            this.panel1.TabIndex = 19;
            // 
            // labelSpeed
            // 
            this.labelSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSpeed.AutoSize = true;
            this.labelSpeed.Location = new System.Drawing.Point(251, 2);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(41, 13);
            this.labelSpeed.TabIndex = 18;
            this.labelSpeed.Text = "Speed:";
            // 
            // timeSlideControl
            // 
            this.timeSlideControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timeSlideControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeSlideControl.Location = new System.Drawing.Point(0, 113);
            this.timeSlideControl.Name = "timeSlideControl";
            this.timeSlideControl.Position = 0D;
            this.timeSlideControl.RangeMax = 0D;
            this.timeSlideControl.RangeMin = 0D;
            this.timeSlideControl.Size = new System.Drawing.Size(332, 67);
            this.timeSlideControl.TabIndex = 17;
            // 
            // contextMenuStripAnims
            // 
            this.contextMenuStripAnims.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.contextMenuStripAnims.Name = "contextMenuStripAnims";
            this.contextMenuStripAnims.Size = new System.Drawing.Size(118, 48);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.OnDeleteAnimation);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.OnRenameAnimation);
            // 
            // AnimationInspectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelAnimTools);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBoxAnimations);
            this.Name = "AnimationInspectionView";
            this.Size = new System.Drawing.Size(341, 676);
            this.panelAnimTools.ResumeLayout(false);
            this.panelAnimTools.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.contextMenuStripAnims.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxGoto;
        private System.Windows.Forms.Label labelGoto;
        private System.Windows.Forms.Button buttonFaster;
        private System.Windows.Forms.Button buttonSlower;
        private System.Windows.Forms.CheckBox checkBoxLoop;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBoxAnimations;
        private TimeSlideControl timeSlideControl;
        private System.Windows.Forms.Panel panelAnimTools;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelSpeed;
        private System.Windows.Forms.Label labelSpeedValue;
        private System.Windows.Forms.Label labelGotoError;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAnims;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;

    }
}
