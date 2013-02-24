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

/* vi: set shiftwidth=4 tabstop=4: */ 