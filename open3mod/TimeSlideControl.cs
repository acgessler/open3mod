///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TimeSlideControl.cs]
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public TimeSlideControl()
        {
            InitializeComponent();

            _font = new Font(FontFamily.GenericMonospace,9);
        }


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


        public double Position
        {
            get { return _pos; }
            set
            {
                _pos = value;
                Invalidate();
            }
        }


        public double Range
        {
            get { return _rangeMax - _rangeMin; }
        }


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


        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
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
            graphics.FillRectangle(new SolidBrush(Color.LightGray), rect );

            if(!Enabled)
            {
                return;
            }
            var pos = RelativePosition;
            var xdraw = rect.Left + (int) (rect.Width*pos);
            graphics.DrawLine(new Pen(new SolidBrush(Color.Red),1), xdraw, 15, xdraw, rect.Bottom );

            if (_mouseEntered)
            {
                graphics.DrawString((_mouseRelativePos*Range).ToString("0.000") + "s", _font, new SolidBrush(Color.DimGray), 5,1);
            }
            graphics.DrawString(RelativePosition.ToString("0.000") + "s", _font, new SolidBrush(Color.Red), rect.Width-70, 1);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 