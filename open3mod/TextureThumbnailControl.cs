using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
    public sealed class TextureThumbnailControl : ThumbnailControlBase<TextureThumbnailControl>
    {
        private new readonly TextureInspectionView _owner;
        private readonly Scene _scene;
        private readonly string _filePath;
        private Texture _texture;

        private bool _replaced;
        private string _newFileId;

        private static Image _loadError;
        private static Image _background;
        private static Image _loadAnimImage;
        private Image _imageWithAlpha;
        private Image _imageWithoutAlpha;


        public TextureThumbnailControl(TextureInspectionView owner, Scene scene, string filePath)
            : base(owner, GetBackgroundImage(), Path.GetFileName(filePath))
        {
            _owner = owner;
            _scene = scene;
            _filePath = filePath;
         
            SetLoadingState();

            ContextMenuStrip = new ContextMenuStrip();

            var s = new ToolStripMenuItem("Show Transparency", null, OnContextMenuToggleAlpha);
            ContextMenuStrip.Items.Add(s);
            s.CheckOnClick = true;
            s.Checked = true;

            s = new ToolStripMenuItem("Zoom", null, OnContextMenuZoom);
            ContextMenuStrip.Items.Add(s);
            s.CheckOnClick = true;
            s.Checked = false;

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Details", null, OnContextMenuDetails));
        }



        private void OnContextMenuZoom(object sender, EventArgs eventArgs)
        {
            SetZoom();
        }


        private void OnContextMenuDetails(object sender, EventArgs eventArgs)
        {
            
        }


        private void OnContextMenuToggleAlpha(object sender, EventArgs eventArgs)
        {
            SetPictureBoxImage();
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
                _imageWithAlpha = image;

                if (GetState() == State.Good)
                {
                    SetPictureBoxImage();
                    SetZoom();
                }
                else
                {
                    pictureBox.Image = _imageWithAlpha;
                    pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                }

                Invalidate();
            }));

        }


        private void SetPictureBoxImage()
        {
            if (GetState() != State.Good)
            {
                return;
            }

            var item = ((ToolStripMenuItem)ContextMenuStrip.Items[0]);
            if (!Image.IsAlphaPixelFormat(_imageWithAlpha.PixelFormat))
            {
                item.Enabled = false;
                item.Checked = false;
                pictureBox.Image = _imageWithAlpha;
                return;
            }
            if (item.Checked)
            {
                pictureBox.Image = _imageWithAlpha;
            }
            else
            {
                if (_imageWithoutAlpha == null)
                {
                    var clone = new Bitmap(_imageWithAlpha.Width, _imageWithAlpha.Height, PixelFormat.Format24bppRgb);
                    using (var gr = Graphics.FromImage(clone)) {
                        gr.Clear(Color.White);
                        gr.DrawImage(_imageWithAlpha, new Rectangle(0, 0, clone.Width, clone.Height));
                    }

                    _imageWithoutAlpha = clone;
                }
                pictureBox.Image = _imageWithoutAlpha;
            }
        }



        private void SetZoom()
        {
            if (GetState() != State.Good)
            {
                return;
            }

            var item = ((ToolStripMenuItem)ContextMenuStrip.Items[1]);
            if(_imageWithAlpha.Width >= pictureBox.Width && _imageWithAlpha.Height >= pictureBox.Height)
            {
                item.Enabled = false;
                item.Checked = false;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                return;
            }
            
            if (_imageWithAlpha.Width < pictureBox.Width && _imageWithAlpha.Height < pictureBox.Height && !item.Checked)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else
            {
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
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
