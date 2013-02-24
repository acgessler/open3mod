///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [ICameraController.cs]
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

/* vi: set shiftwidth=4 tabstop=4: */ 