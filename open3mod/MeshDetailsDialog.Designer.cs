namespace open3mod
{
    partial class MeshDetailsDialog
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
            this.checkedListBoxPerVertex = new System.Windows.Forms.CheckedListBox();
            this.labelVertexCount = new System.Windows.Forms.Label();
            this.labelFaceCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkedListBoxPerFace = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelMaterialName = new System.Windows.Forms.Label();
            this.pictureBoxMaterial = new System.Windows.Forms.PictureBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelBoneCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMaterial)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedListBoxPerVertex
            // 
            this.checkedListBoxPerVertex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxPerVertex.Enabled = false;
            this.checkedListBoxPerVertex.FormattingEnabled = true;
            this.checkedListBoxPerVertex.Items.AddRange(new object[] {
            "Xyz Positions",
            "Normals",
            "Tangent Space Basis",
            "Texture Coordinates #1",
            "Texture Coordinates #2",
            "Texture Coordinates #3",
            "Texture Coordinates #4",
            "Vertex Colors #1",
            "Vertex Colors #2",
            "Vertex Colors #3",
            "Vertex Colors #4",
            "Bone Weights"});
            this.checkedListBoxPerVertex.Location = new System.Drawing.Point(16, 28);
            this.checkedListBoxPerVertex.Name = "checkedListBoxPerVertex";
            this.checkedListBoxPerVertex.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkedListBoxPerVertex.Size = new System.Drawing.Size(154, 107);
            this.checkedListBoxPerVertex.TabIndex = 5;
            // 
            // labelVertexCount
            // 
            this.labelVertexCount.AutoSize = true;
            this.labelVertexCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVertexCount.Location = new System.Drawing.Point(61, 29);
            this.labelVertexCount.Name = "labelVertexCount";
            this.labelVertexCount.Size = new System.Drawing.Size(121, 13);
            this.labelVertexCount.TabIndex = 7;
            this.labelVertexCount.Text = "<aNumber> Vertices";
            // 
            // labelFaceCount
            // 
            this.labelFaceCount.AutoSize = true;
            this.labelFaceCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFaceCount.Location = new System.Drawing.Point(61, 16);
            this.labelFaceCount.Name = "labelFaceCount";
            this.labelFaceCount.Size = new System.Drawing.Size(109, 13);
            this.labelFaceCount.TabIndex = 8;
            this.labelFaceCount.Text = "<aNumber> Faces";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(261, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 9;
            // 
            // checkedListBoxPerFace
            // 
            this.checkedListBoxPerFace.Enabled = false;
            this.checkedListBoxPerFace.FormattingEnabled = true;
            this.checkedListBoxPerFace.Items.AddRange(new object[] {
            "Triangles",
            "Lines",
            "Points"});
            this.checkedListBoxPerFace.Location = new System.Drawing.Point(13, 28);
            this.checkedListBoxPerFace.Name = "checkedListBoxPerFace";
            this.checkedListBoxPerFace.Size = new System.Drawing.Size(136, 49);
            this.checkedListBoxPerFace.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelMaterialName);
            this.groupBox1.Controls.Add(this.pictureBoxMaterial);
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(407, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(196, 272);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Material";
            // 
            // labelMaterialName
            // 
            this.labelMaterialName.AutoSize = true;
            this.labelMaterialName.Location = new System.Drawing.Point(15, 26);
            this.labelMaterialName.Name = "labelMaterialName";
            this.labelMaterialName.Size = new System.Drawing.Size(85, 13);
            this.labelMaterialName.TabIndex = 3;
            this.labelMaterialName.Text = "<Material name>";
            // 
            // pictureBoxMaterial
            // 
            this.pictureBoxMaterial.Location = new System.Drawing.Point(18, 42);
            this.pictureBoxMaterial.Name = "pictureBoxMaterial";
            this.pictureBoxMaterial.Size = new System.Drawing.Size(162, 162);
            this.pictureBoxMaterial.TabIndex = 2;
            this.pictureBoxMaterial.TabStop = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(73, 213);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(107, 13);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Select in Material List";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnJumpToMaterial);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(18, 237);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(162, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Change";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.checkedListBoxPerVertex);
            this.groupBox2.Location = new System.Drawing.Point(211, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 272);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vertex Data";
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(16, 237);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(154, 23);
            this.button5.TabIndex = 9;
            this.button5.Text = "Vertex Colors to Texture";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(16, 179);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(154, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "Generate UV Coordinates";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(16, 150);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(154, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "Calculate Tangent Space";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 208);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(154, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Calculate Normals";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnGenerateNormals);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelBoneCount);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.labelVertexCount);
            this.groupBox3.Controls.Add(this.labelFaceCount);
            this.groupBox3.Location = new System.Drawing.Point(15, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(190, 67);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Statistics";
            // 
            // labelBoneCount
            // 
            this.labelBoneCount.AutoSize = true;
            this.labelBoneCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBoneCount.Location = new System.Drawing.Point(61, 42);
            this.labelBoneCount.Name = "labelBoneCount";
            this.labelBoneCount.Size = new System.Drawing.Size(110, 13);
            this.labelBoneCount.TabIndex = 12;
            this.labelBoneCount.Text = "<aNumber> Bones";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Bones:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Vertices:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Faces:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkedListBoxPerFace);
            this.groupBox4.Location = new System.Drawing.Point(15, 85);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(190, 199);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Face Data";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(16, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 42);
            this.panel1.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Green;
            this.label5.Location = new System.Drawing.Point(230, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "No issues";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.panel1);
            this.groupBox5.Location = new System.Drawing.Point(15, 304);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(588, 71);
            this.groupBox5.TabIndex = 17;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Issues";
            // 
            // MeshDetailsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 387);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MeshDetailsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "<meshName> Details";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMaterial)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxPerVertex;
        private System.Windows.Forms.Label labelVertexCount;
        private System.Windows.Forms.Label labelFaceCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox checkedListBoxPerFace;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelBoneCount;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBoxMaterial;
        private System.Windows.Forms.Label labelMaterialName;



    }
}