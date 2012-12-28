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
        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="state">Global UI state, includes rendering parameters</param>
        /// <param name="cam">Camera controller to be used</param>
        /// <param name="visibleNodes">Set of nodes to render or null to render them all</param>
        /// <param name="visibleSetChanged">true if the visible is different to the last
        /// time this method was invoked.</param>
        void Render(UiState state, ICameraController cam, HashSet<Node> visibleNodes, 
            bool visibleSetChanged);
    }
}
