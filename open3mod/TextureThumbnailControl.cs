///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [TextureThumbnailControl.cs]
// (c) 2012-2015, Open3Mod Contributors
//
// Licensed under the terms and conditions of the 3-clause BSD license. See
// the LICENSE file in the root folder of the repository for the details.
//
// HIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

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


        public string FilePath
        {
            get { return _filePath; }
        }

        public Texture Texture
        {
            get { return _texture; }
        }


        public TextureThumbnailControl(TextureInspectionView owner, Scene scene, string filePath)
            : base(owner, GetBackgroundImage(), Path.GetFileName(filePath))
        {
            _owner = owner;
            _scene = scene;
            _filePath = filePath;
         
            SetLoadingState();

            ContextMenuStrip = new ContextMenuStrip();
          
            
            var s = new ToolStripMenuItem("Zoom", null, OnContextMenuZoom);
            ContextMenuStrip.Items.Add(s);
            s.CheckOnClick = true;
            s.Checked = false;

            s = new ToolStripMenuItem("Show Transparency", null, OnContextMenuToggleAlpha);
            ContextMenuStrip.Items.Add(s);
            s.CheckOnClick = true;
            s.Checked = true;

            s = new ToolStripMenuItem("Mirror along X (U) axis", null, OnContextMenuMirrorX);
            ContextMenuStrip.Items.Add(s);
            s.CheckOnClick = true;
            s.Checked = false;
            s.Enabled = true;

            s = new ToolStripMenuItem("Mirror along Y (V) axis", null, OnContextMenuMirrorY);
            ContextMenuStrip.Items.Add(s);
            s.CheckOnClick = true;
            s.Checked = false;
            s.Enabled = true;
          
            ContextMenuStrip.Items.Add(new ToolStripSeparator());

            s = new ToolStripMenuItem("Delete", null, OnContextMenuDelete);
            ContextMenuStrip.Items.Add(s);
            s.Enabled = true;

            s = new ToolStripMenuItem("Rename", null, OnContextMenuRename);
            ContextMenuStrip.Items.Add(s);
            s.Enabled = true;
    

            ContextMenuStrip.Items.Add(new ToolStripSeparator());

            s = new ToolStripMenuItem("Texture Viewer", null, OnContextMenuDetails);
            ContextMenuStrip.Items.Add(s);
            s.Font = new System.Drawing.Font(
                DefaultFont.FontFamily,
                DefaultFont.Size,
                FontStyle.Bold,
                GraphicsUnit.Point
            );
            s.Enabled = false;

            s = new ToolStripMenuItem("Save", null, OnContextMenuExport);
            ContextMenuStrip.Items.Add(s);
            s.Enabled = false;
          
            ContextMenuStrip.Opened += OnContextMenuOpen;
            DoubleClick += OnContextMenuDetails;
        }



        private void OnContextMenuZoom(object sender, EventArgs eventArgs)
        {
            SetZoom();
        }


        private void OnContextMenuDetails(object sender, EventArgs eventArgs)
        {
            if (GetState() == State.Good)
            {
                _owner.ShowDetails(this);
            }
        }


        private void OnContextMenuMirrorX(object sender, EventArgs eventArgs)
        {
            if (GetState() != State.Good)
            {
                return;
            }

            _texture.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            _texture.ReleaseUpload();
            _texture.Upload();
            if (pictureBox.Image != _texture.Image)
            {
                pictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            else
            {
                pictureBox.Invalidate();
            }            
        }


        private void OnContextMenuMirrorY(object sender, EventArgs eventArgs)
        {
            if (GetState() != State.Good)
            {
                return;
            }
            _texture.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            _texture.ReleaseUpload();
            _texture.Upload();
            if (pictureBox.Image != _texture.Image)
            {
                pictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
            else
            {
                pictureBox.Invalidate();
            }           
        }


        private void OnContextMenuExport(object sender, EventArgs eventArgs)
        {
            var image = _texture.Image;
            var exporter = new TextureExporter(_texture); 
            
            var saver = new SaveFileDialog();
            saver.Title = "Specify file to export " + Path.GetFileName(_texture.OriginalTextureId) + " to";

            if (_texture.OriginalTextureId.Length > 0 && _texture.OriginalTextureId[0] == '*')
            {
                saver.FileName = "EmbeddedTexture_" + _texture.OriginalTextureId.Substring(1) + ".png";
            }
            else
            {
                saver.FileName = Path.GetFileName(_texture.OriginalTextureId);
            }

            var extensions = string.Join(";", exporter.GetExtensionList().Select(s => "*." + s).ToArray());
            saver.Filter = "Image Files ("+extensions+")|"+extensions+"|All files (*.*)|*.*";
            if(saver.ShowDialog(FindForm()) == DialogResult.OK)
            {
                if (!exporter.Export(saver.FileName))
                {
                    MessageBox.Show(FindForm(), "Failed to export to " + saver.FileName);
                }
            }          
        }


        private void OnContextMenuDelete(object sender, EventArgs eventArgs)
        {
            // This does *not* delete the texture file on disk.
           
            int oldIndex = -1;
            _scene.UndoStack.PushAndDo("Delete Texture",
                // Do
                () =>
                {
                    _scene.TextureSet.Delete(Texture.OriginalTextureId);
                    oldIndex = _owner.RemoveEntry(this);
                },
                // Undo
                () =>
                {
                    _scene.TextureSet.Add(Texture.OriginalTextureId);
                    _owner.AddEntry(this, oldIndex);
                },
                // Update
                () =>
                {
                    UpdateDependentMaterialPreviews();
                });
        }


        private void OnContextMenuRename(object sender, EventArgs eventArgs)
        {
            // This renames the texture file on disk. The rename only affects the file name, not the path
            // prefix or file extension.
            //
            // Editing folders is complicated because the texture might be outside of the
            // base folder from which the scene is loaded. Also, when exporting we generally
            // prefer to copy all scene textures to a single folder anyway.
            if (Texture == null)
            {
                return;
            }

            if (Texture.ActualLocation == null)
            {
                MessageBox.Show("This is an embedded texture that cannot be renamed.");
                return;
            }

            SafeRenamer renamer = new SafeRenamer(_scene);      
            RenameDialog dialog = new RenameDialog("", new HashSet<string>(), new HashSet<string>());
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("This will rename the actual image file on disk.", "Rename texture",
                    MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string newName = dialog.NewName;
                    string oldName = Texture.OriginalTextureId;
                    _scene.UndoStack.PushAndDo("Rename Texture File",
                        // Do
                        () => renamer.RenameTexture(Texture, newName),
                        // Undo
                        () => renamer.RenameTexture(Texture, oldName),
                        // Update
                        () => { texCaptionLabel.Text = Path.GetFileName(Texture.OriginalTextureId); });
                }
            }
        }


        private void OnContextMenuToggleAlpha(object sender, EventArgs eventArgs)
        {
            SetPictureBoxImage();
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

                    // Enable the remainder menu items
                    for (var i = 2; i < ContextMenuStrip.Items.Count; ++i)
                    {
                        ContextMenuStrip.Items[i].Enabled = true;
                    }
                }
                else
                {
                    pictureBox.Image = _imageWithAlpha;
                    pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

                    // Disable all menu items
                    foreach (var item in ContextMenuStrip.Items)
                    {
                        ((ToolStripItem)item).Enabled = false;
                    }
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

            if (Texture.OriginalTextureId == newFile)
            {
                return;
            }

            // note: it is important to keep _newFileId as a field just in case the
            // users drags and drops another texture onto the control while this
            // texture is still being loaded.
            _newFileId = _owner.Scene.TextureSet.Replace(Texture.OriginalTextureId, newFile);

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
                        UpdateDependentMaterialPreviews();
                        return false;
                    }
                    return true;
                });
            }));

            Invalidate();
        }

        private void UpdateDependentMaterialPreviews()
        {
            // TODO
        }


        protected override State GetState()
        {
            if (Texture == null)
            {
                return State.Pending;
            }
            return Texture.State == Texture.TextureState.LoadingFailed ? State.Failed : State.Good;
        }


        private void SetLoadingState()
        {
            _texture = null;
            pictureBox.Image = GetLoadingImage();
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }


        public bool CanChangeTextureSource()
        {
            return Texture != null;
        }


        protected override void OnDragDrop(DragEventArgs e)
        {
            try
            {
                var a = (Array)e.Data.GetData(DataFormats.FileDrop, false);

                if (a != null)
                {
                    // Extract string from first array element
                    // (ignore all files except first if more than one file was dropped)
                    string s = a.GetValue(0).ToString();

                    if (Directory.Exists(s))
                    {
                        // different behaviour when folders are dragged onto the panel:
                        // all FAILED textures are auto-matched against the file names
                        // in the folder in the hope that they can be resolved.
                        _owner.MatchWithFolder(s);
                        return;
                    }

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

/* vi: set shiftwidth=4 tabstop=4: */ 