///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [ISceneRenderer.cs]
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace open3mod
{
    [Flags]
    public enum RenderFlags
    {
        Wireframe = 0x1,
        Shaded = 0x2,
        ShowBoundingBoxes = 0x4,
        ShowNormals = 0x8,
        ShowSkeleton = 0x10,
        Textured = 0x20,
        ShowGhosts = 0x40, // show ghost (i.e. semi-transparent) shapes for filtered meshes
    }


    /// <summary>
    /// Abstract interface to support different scene rendering implementations.
    /// An ISceneRenderer is always bound to a single scene during its entire
    /// lifetime. Renderers are expected to handle all view filters requested by
    /// a scene.
    /// </summary>
    public interface ISceneRenderer : IDisposable
    {
        /// <summary>
        /// Called once per frame to update per-frame data.
        /// </summary>
        /// <param name="delta"></param>
        void Update(double delta);

        /// <summary>
        /// Draws the scene
        /// </summary>
        /// <param name="cam">Camera controller to be used</param>
        /// <param name="visibleMeshesByNode">Set of meshes to be drawn for specific nodes.
        ///   If a node has a null entry, all of its meshes should be drawn. Otherwise
        ///   only the meshes in the list. If this parameter is null, all nodes with all
        ///   meshes should be drawn.</param>
        /// <param name="visibleSetChanged">true if the visible set is different to the last
        ///    time this method was invoked (this refers to changes in either the visibleNodes
        ///    and the visibleMeshesByNode parameters.</param>
        /// <param name="texturesChanged">true if one or more textures were changed since
        /// the last time this method was invoked. This happens when textures are being
        /// replaced by the user, or during asynchronous loading.</param>
        /// <param name="flags">Selected set of rendering overlays</param>
        void Render(ICameraController cam, 
            Dictionary<Node, List<Mesh>> visibleMeshesByNode,
            bool visibleSetChanged, bool texturesChanged, 
            RenderFlags flags);
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 