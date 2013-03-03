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
            this.textBoxGoto = new System.Windows.Forms.TextBox();
            this.labelGoto = new System.Windows.Forms.Label();
            this.buttonFaster = new System.Windows.Forms.Button();
            this.buttonSlower = new System.Windows.Forms.Button();
            this.checkBoxLoop = new System.Windows.Forms.CheckBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxAnimations = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // textBoxGoto
            // 
            this.textBoxGoto.Location = new System.Drawing.Point(65, 452);
            this.textBoxGoto.Name = "textBoxGoto";
            this.textBoxGoto.Size = new System.Drawing.Size(78, 20);
            this.textBoxGoto.TabIndex = 16;
            // 
            // labelGoto
            // 
            this.labelGoto.AutoSize = true;
            this.labelGoto.Location = new System.Drawing.Point(26, 455);
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
            this.buttonFaster.Location = new System.Drawing.Point(212, 266);
            this.buttonFaster.Name = "buttonFaster";
            this.buttonFaster.Size = new System.Drawing.Size(60, 40);
            this.buttonFaster.TabIndex = 14;
            this.buttonFaster.Text = "Faster";
            this.buttonFaster.UseVisualStyleBackColor = true;
            // 
            // buttonSlower
            // 
            this.buttonSlower.FlatAppearance.BorderSize = 2;
            this.buttonSlower.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.buttonSlower.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonSlower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSlower.Location = new System.Drawing.Point(65, 266);
            this.buttonSlower.Name = "buttonSlower";
            this.buttonSlower.Size = new System.Drawing.Size(60, 40);
            this.buttonSlower.TabIndex = 13;
            this.buttonSlower.Text = "Slower";
            this.buttonSlower.UseVisualStyleBackColor = true;
            // 
            // checkBoxLoop
            // 
            this.checkBoxLoop.AutoSize = true;
            this.checkBoxLoop.Location = new System.Drawing.Point(12, 215);
            this.checkBoxLoop.Name = "checkBoxLoop";
            this.checkBoxLoop.Size = new System.Drawing.Size(50, 17);
            this.checkBoxLoop.TabIndex = 12;
            this.checkBoxLoop.Text = "Loop";
            this.checkBoxLoop.UseVisualStyleBackColor = true;
            // 
            // buttonPlay
            // 
            this.buttonPlay.FlatAppearance.BorderSize = 2;
            this.buttonPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.buttonPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlay.Location = new System.Drawing.Point(131, 253);
            this.buttonPlay.MaximumSize = new System.Drawing.Size(100, 100);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(75, 68);
            this.buttonPlay.TabIndex = 11;
            this.buttonPlay.Text = "Play/Stop";
            this.buttonPlay.UseVisualStyleBackColor = true;
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
            // 
            // AnimationInspectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxGoto);
            this.Controls.Add(this.labelGoto);
            this.Controls.Add(this.buttonFaster);
            this.Controls.Add(this.buttonSlower);
            this.Controls.Add(this.checkBoxLoop);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBoxAnimations);
            this.Name = "AnimationInspectionView";
            this.Size = new System.Drawing.Size(341, 676);
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

    }
}
