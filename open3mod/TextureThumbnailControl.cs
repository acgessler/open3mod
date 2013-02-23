using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace open3mod
{
    public partial class TextureThumbnailControl : UserControl
    {
        private readonly TextureInspectionView _owner;
        private readonly Scene _scene;
        private readonly string _filePath;


        public TextureThumbnailControl(TextureInspectionView owner, Scene scene, string filePath)
        {
            _owner = owner;
            _scene = scene;
            _filePath = filePath;

            InitializeComponent();
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public void SetImage(Image image)
        {
            pictureBox.Image = image;
        }
    }
}
