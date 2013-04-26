using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    public partial class TextureDetailsDialog : Form
    {
        public TextureDetailsDialog()
        {
            InitializeComponent();
        }


        public void SetTexture(TextureThumbnailControl tex)
        {
            var img = tex.Texture.Image;

            Text = Path.GetFileName(tex.FilePath) + " - Details"; 

            pictureBox1.Image = img;
            labelInfo.Text = string.Format("Size: {0} x {1} px", img.Width, img.Height);
            checkBoxHasAlpha.Checked = tex.Texture.HasAlpha == Texture.AlphaState.HasAlpha;

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
