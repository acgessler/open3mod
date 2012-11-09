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
    }
}
