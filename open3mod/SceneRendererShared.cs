///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [SceneRendererShared.cs]
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace open3mod
{
    /// <summary>
    /// Shared utility class for SceneRendererClassicGl and SceneRendererModernGl
    /// </summary>
    public class SceneRendererShared
    {
        protected readonly Scene Owner;
        protected readonly Vector3 InitposeMin;
        protected readonly Vector3 InitposeMax;
        protected readonly CpuSkinningEvaluator Skinner;
        protected readonly bool[] IsAlphaMaterial;


        /// <summary>
        /// Constructs an instance given a scene with its bounds (AABB)
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initposeMin"></param>
        /// <param name="initposeMax"></param>
        protected SceneRendererShared(Scene owner, Vector3 initposeMin, Vector3 initposeMax)
        {
            Owner = owner;
            InitposeMin = initposeMin;
            InitposeMax = initposeMax;

            Debug.Assert(Owner.Raw != null);    
            Skinner = new CpuSkinningEvaluator(owner);

            IsAlphaMaterial = new bool[owner.Raw.MaterialCount];
            for (int i = 0; i < IsAlphaMaterial.Length; ++i)
            {
                IsAlphaMaterial[i] = Owner.MaterialMapper.IsAlphaMaterial(owner.Raw.Materials[i]);
            }
        }


        /// <summary>
        /// Make sure all textures required for the materials in the scene are uploaded to VRAM.
        /// </summary>
        public void UploadTextures()
        {
            if (Owner.Raw.Materials == null)
            {
                return;
            }
            var i = 0;
            foreach (var mat in Owner.Raw.Materials)
            {
                if (Owner.MaterialMapper.UploadTextures(mat))
                {
                    IsAlphaMaterial[i] = Owner.MaterialMapper.IsAlphaMaterial(mat);
                }
                ++i;
            }
        }
    }
}
