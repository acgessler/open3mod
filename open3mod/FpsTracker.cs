///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [FpsTracker.cs]
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open3mod
{
    /// <summary>
    /// Tracks performance and draws the nice fps counter in the rendering panel
    /// </summary>
    public class FpsTracker
    {
        private int _frameCnt = 1;
        private Stopwatch _sw;
        private double _lastFrameDelta;
        private double _lastFps;


        // limit maximum framerate to avoid taking too much CPU, overheating
        // laptop mainboards or accidentially causing nuclear detonations.
        public const int FRAMERATE_LIMIT = 100;


        public double LastFrameDelta
        {
            get { return _lastFrameDelta; }
        }

        public int FrameCnt
        {
            get { return _frameCnt; }
        }

        public double LastFps
        {
            get { return _lastFps; }
        }

        /// <summary>
        /// Called once per frame to update the internal frame statistics
        /// </summary>
        public void Update()
        {
            _frameCnt = FrameCnt + 1;
            if(_sw == null)
            {
                _sw = new Stopwatch();
                _lastFrameDelta = 0.0;
            }
            else
            {
                _lastFrameDelta = _sw.Elapsed.TotalMilliseconds / 1000.0;
                _sw.Reset();
            }

            _sw.Start();

            // prevent divide by zero
            if (_lastFrameDelta < 1e-8)
            {
                _lastFps = 0.0;
            }
            else
            {
                _lastFps = 1/_lastFrameDelta;
            }

            if (_lastFps > FRAMERATE_LIMIT)
            {
                System.Threading.Thread.Sleep(1 + (int)(1000.0 / FRAMERATE_LIMIT - _lastFrameDelta * 1000));
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 