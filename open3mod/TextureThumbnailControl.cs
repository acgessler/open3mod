///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TextureThumbnailControl.cs]
// (c) 2012-2013, Alexander C. Gessler
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
    /// <summary>
    /// Custom UI control to draw texture thumbnails in the texture inspector panel.
    /// Instances of this class flow in a FlowControlPanel. TextureInspectionView
    /// implements custom selection logic to handle texture selections. It also
    /// maintains the thumbnail set for a scene.
    /// </summary>
    public partial class TextureThumbnailControl : UserControl
    {
        private readonly TextureInspectionView _owner;
        private readonly Scene _scene;
        private readonly string _filePath;
        private Texture _texture;
        private bool _selected;

        private static Image _loadError;
        private static Image _background;

        private static readonly Color SelectionColor = Color.CornflowerBlue;
        private static readonly Color LoadingColor = Color.Chartreuse;
        private static readonly Color FailureColor = Color.Crimson;

        private static GraphicsPath _selectPath;
        private int _mouseOverCounter = 0;

        private int _mouseOverFadeTimer;
        private bool _replaced;

        // time the hover selection background fades out after the mouse leaves the control, in ms
        private const int FadeTime = 500;


        public TextureThumbnailControl(TextureInspectionView owner, Scene scene, string filePath)
        {
            _owner = owner;
            _scene = scene;
            _filePath = filePath;

            InitializeComponent();

            labelOldTexture.Text = "";
            texCaptionLabel.Text = Path.GetFileName(filePath);
            pictureBox.BackgroundImage = GetBackgroundImage();

            // forward Click()s on children to us
            // and use a ref counter for MouseEnter()/MouseLeave()
            foreach(var c in Controls)
            {
                var cc = c as Control;
                if(cc != null)
                {
                    cc.Click += (sender, args) => OnClick(new EventArgs());
                    cc.MouseEnter += (sender, args) => OnMouseEnter(new EventArgs());
                    cc.MouseLeave += (sender, args) => OnMouseLeave(new EventArgs());
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
                labelOldTexture.ForeColor = _selected ? Color.White : Color.DarkGray;

                Invalidate();
            }
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

            pictureBox.Image = image;
        
            if(image.Width < pictureBox.Width && image.Height < pictureBox.Height)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else
            {
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }

            Invalidate();
        }


        /// <summary>
        /// Change the source texture path for this texture. This
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

            var newFileId = _owner.Scene.TextureSet.Replace(_texture.FileName, newFile);

            BeginInvoke(new MethodInvoker(() => pictureBox.Image = null));

            _texture = null;
            _owner.Scene.TextureSet.AddCallback((id, tex) =>
            {
                if(id == newFileId)
                {
                    var old = texCaptionLabel.Text;

                    if (!_replaced)
                    {
                        _replaced = true;
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            texCaptionLabel.Text = Path.GetFileName(newFile);
                            labelOldTexture.Text = "was " + old;
                            texCaptionLabel.Top -= 4;
                        }));
                    }

                    SetTexture(tex);
                    return false;
                }
                return true;
            });

            Invalidate();
        }


        public bool CanChangeTextureSource()
        {
            return _texture != null;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if(_selected || _mouseOverFadeTimer > 0 || _mouseOverCounter > 0 || _texture == null 
                || _texture.State == Texture.TextureState.LoadingFailed)
            {
                CreateGraphicsPathForSelection();

                Debug.Assert(_selectPath != null);

                Color col = SelectionColor;
   
                if (_selected)
                {
                    col = SelectionColor;
                }
                else if (_texture == null)
                {
                    col = LoadingColor;
                }
                else if (_texture.State == Texture.TextureState.LoadingFailed)
                {
                    col = FailureColor;
                }

                if(!_selected) {
                    float intensity = 0.5f * (_mouseOverCounter > 0 ? 1.0f : (float)_mouseOverFadeTimer/FadeTime);
                    if(intensity < 0.0f)
                    {
                        intensity = 0.0f;
                    }
                    if (_texture == null || _texture.State == Texture.TextureState.LoadingFailed)
                    {
                        intensity += 0.4f;
                    }
                    col = Color.FromArgb((byte)(intensity * 255.0f), col);
                }

                e.Graphics.FillPath( new SolidBrush(col), _selectPath);
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


        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            _mouseOverFadeTimer = FadeTime;
            _mouseOverCounter = 1;
            Invalidate();
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            if (ClientRectangle.Contains(this.PointToClient(Control.MousePosition)))
            {
                return;
            }

            base.OnMouseLeave(e);

            _mouseOverCounter = 0;
            _mouseOverFadeTimer = FadeTime;

            var t = new Timer {Interval = 30};
            t.Tick += (sender, args) =>
            {                
                _mouseOverFadeTimer -= t.Interval;
                if (_mouseOverFadeTimer < 0)
                {
                    t.Stop();
                }

                Invalidate();
            };

            t.Start();
        }


        protected override void  OnDragDrop(DragEventArgs e)
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
                    BeginInvoke((MethodInvoker)(() => ChangeTextureSource(s)), new object[] {this});
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

/* vi: set shiftwidth=4 tabstop=4: */ 