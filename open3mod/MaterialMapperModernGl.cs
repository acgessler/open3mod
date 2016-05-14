///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [MaterialMapperModernGl.cs]
// (c) 2012-2015, Open3Mod Contributors
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
using System.Diagnostics;
using Assimp;
using System.Diagnostics;

namespace open3mod
{
    public sealed class MaterialMapperModernGl : MaterialMapper
    {

        internal MaterialMapperModernGl(Scene scene)
            : base(scene)
        { }

        private readonly ShaderGen _shaderGen = new ShaderGen();


#if DEBUG
        ~MaterialMapperModernGl()
        {
            Debug.Assert(false);
        }
#endif

        public override void Dispose()
        {
            _shaderGen.Dispose();
            GC.SuppressFinalize(this);
        }


        public override void ApplyMaterial(Mesh mesh, Material mat, bool textured, bool shaded)
        {
            ShaderGen.GenFlags flags = 0;
            if (textured)
            {
                flags |= ShaderGen.GenFlags.ColorMap;
            }
            if (shaded)
            {
                flags |= ShaderGen.GenFlags.Lighting;
            }
            Shader shader = _shaderGen.GenerateOrGetFromCache(flags);
            shader.BindIfNecessary();
        }


        //public override void UnapplyMaterial(Mesh, Material, Tex)


        public override void ApplyGhostMaterial(Mesh mesh, Material material, bool shaded)
        {
            
        }

        public override void BeginScene(Renderer renderer)
        {         
        }

        public override void EndScene(Renderer renderer)
        {
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 