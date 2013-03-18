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
        private float _rangeMin;
        private float _rangeMax;
        private float _pos;


        public TimeSlideControl()
        {
            InitializeComponent();
        }


        public float RangeMin
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


        public float RangeMax
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


        public float Position
        {
            get { return _pos; }
            set
            {
                _pos = value;
                Invalidate();
            }
        }


        private void OnPaint(object sender, PaintEventArgs e)
        {
   
            var graphics = e.Graphics;
            graphics.DrawLine(new Pen(new SolidBrush(Color.Red),2), 10, 10, 100, 20 );
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 