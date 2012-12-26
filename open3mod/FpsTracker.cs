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
