///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [ThumbnailControlBase.cs]
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Windows.Forms;

namespace open3mod
{
    /// <summary>
    /// Base behavior and UI for the selectable thumbnails which we use for both
    /// texture and materials tabs.
    /// 
    /// See TextureThumbnailControl for example use.
    /// </summary>
    public abstract partial class ThumbnailControlBase<TDeriving> : UserControl 
            where TDeriving  : ThumbnailControlBase<TDeriving>  
    {
        protected readonly ThumbnailViewBase<TDeriving> _owner;
        private bool _selected;

        /// <summary>
        /// Enumerates possible abstract states for the thumbnail.
        /// This only affects the selection/highlighting behavior.
        /// See GetState()
        /// </summary>
        protected enum State
        {
            Failed, Pending, Good
        }


        private static readonly Color SelectionColor = Color.CornflowerBlue;
        private static readonly Color LoadingColor = Color.Chartreuse;
        private static readonly Color FailureColor = Color.Red;

        private static GraphicsPath _selectPath;
        private int _mouseOverCounter = 0;
        private int _mouseOverFadeTimer;


        /// <summary>
        /// Gives the time the hover selection background fades out after the mouse leaves the control.
        /// Unit: milli seconds.
        /// </summary>
        private const int FadeTime = 500;


        protected ThumbnailControlBase(ThumbnailViewBase<TDeriving> owner, Image backgroundImage, string initialCaption)
        {
            _owner = owner;
            InitializeComponent();

            labelOldTexture.Text = "";
            pictureBox.BackgroundImage = backgroundImage;
            texCaptionLabel.Text = initialCaption;

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

            MouseDown += (sender, args) => owner.SelectEntry((TDeriving)this);

            // TODO is there a better way to bubble events up?
            pictureBox.DoubleClick += (sender, args) => ((TDeriving) this).OnDoubleClick(args);
            labelOldTexture.DoubleClick += (sender, args) => ((TDeriving)this).OnDoubleClick(args);
            texCaptionLabel.DoubleClick += (sender, args) => ((TDeriving)this).OnDoubleClick(args);
        }

 
        protected void OnContextMenuOpen(object sender, EventArgs e)
        {
            _owner.SelectEntry((TDeriving)this);
        }


        public void OnSetTooltips(ToolTip tips)
        {
            var text = "Right-click for tools and options";
            tips.SetToolTip(this,text);

            tips.SetToolTip(pictureBox,text);
            tips.SetToolTip(labelOldTexture,text);
            tips.SetToolTip(texCaptionLabel,text);
        }



        protected abstract State GetState();


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
       

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var state = GetState();

            if(_selected || _mouseOverFadeTimer > 0 || _mouseOverCounter > 0 || state != State.Good)
            {
                CreateGraphicsPathForSelection();

                Debug.Assert(_selectPath != null);

                Color col = SelectionColor;
   
                if (_selected)
                {
                    col = SelectionColor;
                }
                else if (state == State.Pending)
                {
                    col = LoadingColor;
                }
                else if (state == State.Failed)
                {
                    col = FailureColor;
                }

                if(!_selected) {
                    float intensity = 0.5f * (_mouseOverCounter > 0 ? 1.0f : (float)_mouseOverFadeTimer/FadeTime);
                    if(intensity < 0.0f)
                    {
                        intensity = 0.0f;
                    }
                    if (state == State.Pending)
                    {
                        intensity += 0.4f;
                    }
                    else if (state == State.Failed)
                    {
                        intensity += 0.6f;
                    }
                    if (intensity > 1.0f)
                    {
                        intensity = 1.0f;
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
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 