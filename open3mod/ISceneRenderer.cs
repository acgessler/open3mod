using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open3mod
{
    /// <summary>
    /// Abstract interface to support different scene rendering implementations.
    /// A ISceneRenderer is always bound to a single scene during its entire
    /// lifetime.
    /// </summary>
    public interface ISceneRenderer
    {
        void Render(UiState state, ICameraController cam);
    }
}
