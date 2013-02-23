using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private Texture _texture;
        private bool _selected;

        private static Image _loadError;
        private static Image _background;
   

        public TextureThumbnailControl(TextureInspectionView owner, Scene scene, string filePath)
        {
            _owner = owner;
            _scene = scene;
            _filePath = filePath;

            InitializeComponent();

            texCaptionLabel.Text = Path.GetFileName(filePath);

            pictureBox.BackgroundImage = GetBackgroundImage();
        }


        public string FilePath
        {
            get { return _filePath; }
        }

        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                if(_selected == value)
                {
                    return;
                }

                _selected = value;

                // adjust colors for selected elements
                BackColor = _selected ? Color.CornflowerBlue : Color.Empty;
                texCaptionLabel.ForeColor = _selected ? Color.White : Color.Black;
            }
        }

        public void SetTexture(Texture texture)
        {
            _texture = texture;
            var image = texture.Image;
            if(image == null)
            {
                image = GetLoadErrorImage();
            }

            Debug.Assert(image != null);

            pictureBox.Image = image;
        
            if(image.Width < pictureBox.Width && image.Height < pictureBox.Height)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else
            {
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private static Image GetLoadErrorImage()
        {
            if (_loadError != null)
            {
                return _loadError;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            // for some reason we need to keep the stream open for the _lifetime_ of the Image,
            // therefore the Dispose() is _not_ missing.
            var imageStream = assembly.GetManifestResourceStream("open3mod.Images.FailedToLoad.bmp");
            
            Debug.Assert(imageStream != null);
            _loadError = Image.FromStream(imageStream);
            

            return _loadError;
        }

        private static Image GetBackgroundImage()
        {
            if (_background != null)
            {
                return _background;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            // for some reason we need to keep the stream open for the _lifetime_ of the Image,
            // therefore the Dispose() is _not_ missing.
            var stream = assembly.GetManifestResourceStream("open3mod.Images.TextureTransparentBackground.png");

            Debug.Assert(stream != null);
            _background = Image.FromStream(stream);
            
            return _background;
        }

       
    }
}
