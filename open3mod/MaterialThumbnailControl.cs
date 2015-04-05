///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [MaterialThumbnailControl.cs]
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

using System.Drawing;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Provides a selectable thumbnail control for materials, goes together with the TextureInspectionView class.
    /// </summary>
    public class MaterialThumbnailControl : ThumbnailControlBase<MaterialThumbnailControl>
    {
        private new readonly MaterialInspectionView _owner;
        private readonly Scene _scene;
        private readonly Material _material;
       
        private readonly object _lock = new object();

        private static Image _loadError;
        private static Image _background;
        private static Image _loadAnimImage;
        private MaterialPreviewRenderer _renderer;
        private bool _wantUpdate;

        private bool _superSample;


        public MaterialThumbnailControl(MaterialInspectionView owner, Scene scene, Material material)
            : base(owner, GetBackgroundImage(), material.HasName ? material.Name : "Unnamed Material")
        {
            _owner = owner;
            _scene = scene;
            _material = material;

            _superSample = true;         

            UpdatePreview();
            SetLoadingState();            
        }


        /// <summary>
        /// The material shown in the control.
        /// </summary>
        public Material Material
        {
            get { return _material; }
        }


        /// <summary>
        /// Indicates whether 2x supersampling is enabled for the material preview
        /// (this means the material preview is rendered in twice the picture box's
        ///  size)
        /// </summary>
        public bool SuperSample
        {
            get { return _superSample; }
            set
            {
                if (_superSample == value)
                {
                    return;
                }
                _superSample = value;
                UpdatePreview();
            }
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
                    (uint)pictureBox.Width * (uint)(SuperSample ? 2 : 1),
                    (uint)pictureBox.Height * (uint)(SuperSample ? 2 : 1));

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
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        pictureBox.Image = GetLoadErrorImage();
                        pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                    }                  
                }));

            };
        }



        protected override State GetState()
        {
            return State.Good;
        }


        private void SetLoadingState()
        {
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


        public static Image GetBackgroundImage()
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

        public Image GetCurrentPreviewImage()
        {
            return pictureBox.Image;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 