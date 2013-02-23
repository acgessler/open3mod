using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
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

        private static readonly SolidBrush _paintBrush = new SolidBrush(Color.CornflowerBlue);
        private static GraphicsPath _selectPath;

        public TextureThumbnailControl(TextureInspectionView owner, Scene scene, string filePath)
        {
            _owner = owner;
            _scene = scene;
            _filePath = filePath;

            InitializeComponent();

            texCaptionLabel.Text = Path.GetFileName(filePath);
            pictureBox.BackgroundImage = GetBackgroundImage();

            // forward Click()s on children to us
            foreach(var c in Controls)
            {
                var cc = c as Control;
                if(cc != null)
                {
                    cc.Click += (sender, args) => OnClick(new EventArgs());
                }
            }
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
                // [background color adjustments are handled by OnPaint()]
                //BackColor = _selected ? Color.CornflowerBlue : Color.Empty;
                texCaptionLabel.ForeColor = _selected ? Color.White : Color.Black;

                Invalidate();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if(_selected)
            {
                CreateGraphicsPathForSelection();

                Debug.Assert(_selectPath != null);
                e.Graphics.FillPath(_paintBrush, _selectPath);
            }
        }

        private void CreateGraphicsPathForSelection()
        {
            if (_selectPath != null)
            {
                return;
            }

            var w = Size.Width;
            var h = Size.Height;

            const int corner = 7;

            // this is an instance method relying on the control's Size to build
            // a GraphicsPath but it caches the result in the static _selectPath -
            // this is fine because it is assumed that all instances always have
            // the same Size at a time.
            _selectPath = RoundedRectangle.Create(1, 1, w-2, h-2, corner);
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
