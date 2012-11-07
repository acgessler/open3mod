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

        public FpsTracker()
        {
            
        }

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
                _lastFrameDelta = _sw.Elapsed.TotalMilliseconds;
                _sw.Reset();
            }

            _sw.Start();
            _lastFps = 1/_lastFrameDelta;
        }
    }
}
