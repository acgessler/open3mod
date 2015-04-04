///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [TimeSlideControl.cs]
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
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace open3mod
{
    public partial class TimeSlideControl : UserControl
    {
        private double _rangeMin;
        private double _rangeMax;
        private double _pos;
        private double _mouseRelativePos;
        private bool _mouseEntered;
        private readonly Font _font;
        private readonly Pen _redPen;
        private readonly SolidBrush _lightGray;
        private readonly Pen _dimGrayPen;
        private readonly Pen _blackPen;


        public TimeSlideControl()
        {
            InitializeComponent();

            _font = new Font(FontFamily.GenericMonospace,9);
            _redPen = new Pen(new SolidBrush(Color.Red), 1);
            _lightGray = new SolidBrush(Color.LightGray);
            _dimGrayPen = new Pen(new SolidBrush(Color.DimGray), 1);
            _blackPen = new Pen(new SolidBrush(Color.Black), 1);
        }


        /// <summary>
        /// Minimum cursor value
        /// </summary>
        public double RangeMin
        {
            get { return _rangeMin; }
            set 
            { 
                Debug.Assert(value <= _rangeMax);
                _rangeMin = value;
                if (_pos < _rangeMin)
                {
                    _pos = _rangeMin;
                }
                Invalidate();
            }
        }


        /// <summary>
        /// Maximum cursor value
        /// </summary>
        public double RangeMax
        {
            get { return _rangeMax; }
            set
            {
                Debug.Assert(_rangeMin <= value);
                _rangeMax = value;
                if (_pos > _rangeMax)
                {
                    _pos = _rangeMax;
                }
                Invalidate();
            }
        }


        /// <summary>
        /// Cursor position in [RangeMin, RangeMax]
        /// </summary>
        public double Position
        {
            get { return _pos; }
            set
            {
                _pos = value;
                if (_pos > RangeMax)
                {
                    _pos = RangeMax;
                }
                if (_pos < RangeMin)
                {
                    _pos = RangeMin;
                }
                Invalidate();
            }
        }


        /// <summary>
        /// Valid cursor range (RangeMax-RangeMin)
        /// </summary>
        public double Range
        {
            get { return _rangeMax - _rangeMin; }
        }


        /// <summary>
        /// Cursor position in [0,1]
        /// </summary>
        public double RelativePosition
        {
            get
            {
                var d = Range;
                if (d < 1e-7) // prevent divide by zero
                {
                    return 0.0;
                }
                return (_pos - _rangeMin) / d;
            }
        }


        public delegate void RewindDelegate(object sender, RewindDelegateArgs args);
        public struct RewindDelegateArgs
        {
            public double OldPosition;
            public double NewPosition;
        }

        /// <summary>
        /// Invoked when the user manually rewinds the slider thumb.
        /// </summary>
        public event RewindDelegate Rewind;

        public virtual void OnRewind(RewindDelegateArgs args)
        {
            RewindDelegate handler = Rewind;
            if (handler != null) handler(this, args);
        }


        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            var rect = ClientRectangle;
            var newPos = ((e.X - rect.Left) * Range / (double)rect.Width) + _rangeMin;

            OnRewind(new RewindDelegateArgs { OldPosition = _pos, NewPosition = newPos });
            Position = newPos;
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var rect = ClientRectangle;
            _mouseRelativePos = (e.X - rect.Left) / (double)rect.Width;

            Invalidate();
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _mouseEntered = true;

            Invalidate();
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mouseEntered = false;

            Invalidate();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            var rect = ClientRectangle;
            graphics.FillRectangle(_lightGray, rect );

            if(!Enabled)
            {
                return;
            }

            var pos = RelativePosition;
            var xdraw = rect.Left + (int) (rect.Width*pos);
            graphics.DrawLine(_redPen, xdraw, 15, xdraw, rect.Bottom );

            var widthPerSecond = rect.Width / Range;
            
            //calc a stepsize that is a power of 10s
            double log = Math.Log10(Range);
            int roundedLog = (int) (Math.Floor(log));
            float stepsize = (float) (Math.Pow(10, roundedLog));

            for (float i = 0.0f; i < (float)Range; i += stepsize)
            {
                int xpos = (int)(i * widthPerSecond);
                graphics.DrawLine(_dimGrayPen, xpos, 55, xpos, rect.Bottom);
            }

            if (_mouseEntered)
            {
                graphics.DrawString((_mouseRelativePos * Range).ToString("0.000") + "s", _font, _blackPen.Brush, 5, 1);
                xdraw = rect.Left + (int)(rect.Width * _mouseRelativePos);
                graphics.DrawLine(_blackPen, xdraw, 40, xdraw, rect.Bottom);
            }
            graphics.DrawString(Position.ToString("0.000") + "s", _font, _redPen.Brush, rect.Width-70, 1);
        }
    }

    
}

/* vi: set shiftwidth=4 tabstop=4: */ 