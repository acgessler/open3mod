using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace open3mod
{
    /// <summary>
    /// Base camera abstraction. 
    /// 
    /// A camera controller is assumed to be stateful, i.e. it maintains
    /// the current camera position and adjusts it to input.
    /// </summary>
    public interface ICameraController
    {

        Matrix4 GetView();

        /// <summary>
        /// Process mouse movement events
        /// </summary>
        /// <param name="x">X delta</param>
        /// <param name="y">Y delta</param>
        void MouseMove(int x, int y);

        /// <summary>
        /// Process scroll events
        /// </summary>
        /// <param name="z">Signed scroll delta (knocks * DELTA_.. constants
        ///   from WinFors)</param>
        void Scroll(int z);

        /// <summary>
        /// Process movement keys
        /// </summary>
        /// <param name="x">Signed X axis movement, normalized by time</param>
        /// <param name="y">Signed Y axis movement, normalized by time</param>
        /// <param name="z">Signed Z axis movement, normalized by time</param>
        void MovementKey(float x, float y, float z);
    }
}
