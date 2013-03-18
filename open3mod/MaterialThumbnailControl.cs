using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Provides a selectable thumbnail control for materials, goes together with the TextureInspectionView class.
    /// </summary>
    public class MaterialThumbnailControl : ThumbnailControlBase
    {
        private readonly MaterialInspectionView _owner;
        private readonly Scene _scene;
        private readonly Material _material;
        private readonly string _filePath;
        private Texture _texture;

        private bool _replaced;
        private string _newFileId;
        private object _lock = new object();

        private static Image _loadError;
        private static Image _background;
        private static Image _loadAnimImage;
        private MaterialPreviewRenderer _renderer;
        private bool _wantUpdate;


        public MaterialThumbnailControl(MaterialInspectionView owner, Scene scene, Material material)
            : base(GetBackgroundImage(), material.HasName ? material.Name : "Unnamed Material")
        {
            _owner = owner;
            _scene = scene;
            _material = material;

            UpdatePreview();

            SetLoadingState();
        }


        /// <summary>
        /// Requests the control to update the preview image.
        /// 
        /// This should be called when the source material changes in any way, for
        /// example if a texture is replaced.
        /// </summary>
        public void UpdatePreview()
        {
            lock (_lock)
            {
                _wantUpdate = true;
                if (_renderer != null)
                {
                    return;
                }
                _renderer = new MaterialPreviewRenderer(_owner.Window, _scene, _material,
                    (uint)pictureBox.Width,
                    (uint)pictureBox.Height);

                _wantUpdate = false;
            }
            _renderer.PreviewAvailable += me =>
            {
                var renderer = _renderer;
                lock (_lock)
                {
                    _renderer = null;
                    if (_wantUpdate)
                    {
                        UpdatePreview();
                    }
                }
        
                BeginInvoke(new MethodInvoker(() =>
                {

                    var image = renderer.PreviewImage;
                    if (image != null)
                    {
                        pictureBox.Image = image;
                    }
                    else
                    {
                        pictureBox.Image = GetLoadErrorImage();
                    }                  
                }));

            };
        }


        public Material Material
        {
            get { return _material; }
        }


        protected override State GetState()
        {
            return State.Good;
        }


        private void SetLoadingState()
        {
            _texture = null;
            pictureBox.Image = GetLoadingImage();
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }


        // hack: we make that public because it gets also used by the Renderer
        // to draw tabs which failed to load their scenes.
        public static Image GetLoadErrorImage()
        {
            if (_loadError != null)
            {
                return _loadError;
            }

            _loadError = ImageFromResource.Get("open3mod.Images.FailedToLoad.png");
            return _loadError;
        }


        private static Image GetBackgroundImage()
        {
            if (_background != null)
            {
                return _background;
            }

            _background = ImageFromResource.Get("open3mod.Images.TextureTransparentBackground.png");
            return _background;
        }


        private static Image GetLoadingImage()
        {
            if (_loadAnimImage != null)
            {
                return _loadAnimImage;
            }

            _loadAnimImage = ImageFromResource.Get("open3mod.Images.TextureLoading.gif");
            return _loadAnimImage;
        }
    }
}
