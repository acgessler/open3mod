using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Abstract interface to support different scene rendering implementations.
    /// A ISceneRenderer is always bound to a single scene during its entire
    /// lifetime. They are expected to obey all view filters requested by
    /// a scene.
    /// </summary>
    public interface ISceneRenderer
    {
        void Render(UiState state, ICameraController cam, HashSet<Node> visibleNodes);
    }
}
