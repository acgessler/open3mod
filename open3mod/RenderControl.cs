using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;

namespace open3mod
{
    /// <summary>
    /// Dummy derivative of GLControl to be able to specify constructor
    /// parameters while still being usable with the WinForms designer.
    /// 
    /// The RenderControl always requests 4 FSAA samples, a stencil buffer
    /// and a 24 bit depth buffer, which should be natively supported
    /// by most hardware in use today.
    /// 
    /// </summary>
    class RenderControl : GLControl
    {
        public RenderControl()
            : base(new GraphicsMode(new ColorFormat(32), 32, 0, 4))
        { }
    }
}
