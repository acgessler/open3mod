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
            this.label1 = new System.Windows.Forms.Label();
            this.labelVertexCount = new System.Windows.Forms.Label();
            this.labelFaceCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkedListBoxPerFace = new System.Windows.Forms.CheckedListBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // checkedListBoxPerVertex
            // 
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
            this.checkedListBoxPerVertex.Location = new System.Drawing.Point(15, 57);
            this.checkedListBoxPerVertex.Name = "checkedListBoxPerVertex";
            this.checkedListBoxPerVertex.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkedListBoxPerVertex.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.checkedListBoxPerVertex.Size = new System.Drawing.Size(157, 184);
            this.checkedListBoxPerVertex.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Per-vertex data:";
            // 
            // labelVertexCount
            // 
            this.labelVertexCount.AutoSize = true;
            this.labelVertexCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVertexCount.Location = new System.Drawing.Point(12, 13);
            this.labelVertexCount.Name = "labelVertexCount";
            this.labelVertexCount.Size = new System.Drawing.Size(121, 13);
            this.labelVertexCount.TabIndex = 7;
            this.labelVertexCount.Text = "<aNumber> Vertices";
            // 
            // labelFaceCount
            // 
            this.labelFaceCount.AutoSize = true;
            this.labelFaceCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFaceCount.Location = new System.Drawing.Point(12, 265);
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
            this.checkedListBoxPerFace.Location = new System.Drawing.Point(15, 291);
            this.checkedListBoxPerFace.Name = "checkedListBoxPerFace";
            this.checkedListBoxPerFace.Size = new System.Drawing.Size(160, 49);
            this.checkedListBoxPerFace.TabIndex = 10;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(92, 355);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(83, 13);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Jump to material";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnJumpToMaterial);
            // 
            // MeshDetailsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(187, 387);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.checkedListBoxPerFace);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelFaceCount);
            this.Controls.Add(this.labelVertexCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkedListBoxPerVertex);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MeshDetailsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "<meshName> Details";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxPerVertex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelVertexCount;
        private System.Windows.Forms.Label labelFaceCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox checkedListBoxPerFace;
        private System.Windows.Forms.LinkLabel linkLabel1;



    }
}