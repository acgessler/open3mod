using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace open3mod
{
    /// <summary>
    /// Provides a selectable thumbnail control for textures, goes together with the TextureInspectionView class.
    /// </summary>
    public class TextureThumbnailControl : ThumbnailControlBase
    {
        private readonly TextureInspectionView _owner;
        private readonly Scene _scene;
        private readonly string _filePath;
        private Texture _texture;

        private bool _replaced;
        private string _newFileId;

        private static Image _loadError;
        private static Image _background;
        private static Image _loadAnimImage;


        public TextureThumbnailControl(TextureInspectionView owner, Scene scene, string filePath)
            : base(GetBackgroundImage(), Path.GetFileName(filePath))
        {
            _owner = owner;
            _scene = scene;
            _filePath = filePath;
         
            SetLoadingState();
        }


        public string FilePath
        {
            get { return _filePath; }
        }


        /// <summary>
        /// Called by TextureInspectionView when the texture pertaining
        /// to a thumbnail has been loaded (with success or not).
        /// </summary>
        /// <param name="texture">Texture to show a thumbnail for</param>
        public void SetTexture(Texture texture)
        {
            _texture = texture;
            var image = texture.Image ?? GetLoadErrorImage();

            Debug.Assert(image != null);

            BeginInvoke(new MethodInvoker(() =>
            {
                pictureBox.Image = image;

                if (image.Width < pictureBox.Width && image.Height < pictureBox.Height)
                {
                    pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                else
                {
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }

                Invalidate();
            }));

        }


        /// <summary>
        /// Changes the source texture path for this texture. This
        /// tries to load the requested file and sets it as new texture
        /// image. It then updates the thumbnail view. This is only
        /// valid if CanChangeTextureSource() is true.
        /// 
        /// TODO Texture instances are available from the very beginning,
        /// it's just that this part of the system doesn't get them 
        /// until loading is complete. So this is a design limitation
        /// that could probably be resolved.
        /// </summary>
        /// <param name="newFile"></param>
        public void ChangeTextureSource(string newFile)
        {
            Debug.Assert(CanChangeTextureSource());

            if (_texture.FileName == newFile)
            {
                return;
            }

            // note: it is important to keep _newFileId as a field just in case the
            // users drags and drops another texture onto the control while this
            // texture is still being loaded.
            _newFileId = _owner.Scene.TextureSet.Replace(_texture.FileName, newFile);

            // SetLoadingState() needs to be run on the GUI thread - AddCallback()
            // need not, but it needs to be sequenced after SetLoadingState().
            BeginInvoke(new MethodInvoker(() =>
            {
                SetLoadingState();

                _owner.Scene.TextureSet.AddCallback((id, tex) =>
                {
                    if (id == _newFileId)
                    {
                        var old = texCaptionLabel.Text;

                        BeginInvoke(new MethodInvoker(() =>
                        {
                            texCaptionLabel.Text = Path.GetFileName(newFile);

                            if (!_replaced)
                            {
                                labelOldTexture.Text = "was " + old;
                                texCaptionLabel.Top -= 4;

                                _replaced = true;
                            }
                        }));

                        SetTexture(tex);
                        return false;
                    }
                    return true;
                });
            }));

            Invalidate();
        }


        protected override State GetState()
        {
            if (_texture == null)
            {
                return State.Pending;
            }
            return _texture.State == Texture.TextureState.LoadingFailed ? State.Failed : State.Good;
        }


        private void SetLoadingState()
        {
            _texture = null;
            pictureBox.Image = GetLoadingImage();
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }


        public bool CanChangeTextureSource()
        {
            return _texture != null;
        }


        protected override void OnDragDrop(DragEventArgs e)
        {
            try
            {
                var a = (Array)e.Data.GetData(DataFormats.FileDrop);

                if (a != null)
                {
                    // Extract string from first array element
                    // (ignore all files except first if more than one file was dropped)
                    string s = a.GetValue(0).ToString();

                    // Do it async to avoid stalling the cursor
                    BeginInvoke((MethodInvoker)(() => ChangeTextureSource(s)), new object[] { this });
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in DragDrop function: " + ex.Message);
            }
        }


        protected override void OnDragEnter(DragEventArgs e)
        {
            if (!CanChangeTextureSource())
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            // only accept files for drag and drop
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
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
