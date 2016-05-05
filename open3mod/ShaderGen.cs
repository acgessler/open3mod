///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [ShaderGen.cs]
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
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace open3mod
{
    public class ShaderGen : IDisposable
    {
        [Flags]
        public enum GenFlags
        {
            ColorMap = 0x1,
            VertexColor = 0x2,
            PhongSpecularShading = 0x4,
            Skinning = 0x8,
            Lighting = 0x10
        };

        private readonly Dictionary<GenFlags, Shader> shaders_ = new Dictionary<GenFlags, Shader>();

        public Shader GenerateOrGetFromCache(GenFlags flags)
        {
            if (!shaders_.ContainsKey(flags))
            {
                shaders_[flags] = Generate(flags);
            }
            return shaders_[flags];
        }

        public void Dispose()
        {
            foreach (var v in shaders_)
            {
                v.Value.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public Shader Generate( GenFlags flags ) 
        {
            string pp = "";

            if (flags.HasFlag(GenFlags.ColorMap))
            {
                pp += "#define HAS_COLOR_MAP\n";
            }

            if (flags.HasFlag(GenFlags.VertexColor))
            {
                pp += "#define HAS_VERTEX_COLOR\n";
            }

            if (flags.HasFlag(GenFlags.PhongSpecularShading))
            {
                pp += "#define HAS_PHONG_SPECULAR_SHADING\n";
            }

            if (flags.HasFlag(GenFlags.Skinning))
            {
                pp += "#define HAS_SKINNING\n";
            }

            if (flags.HasFlag(GenFlags.Lighting))
            {
                pp += "#define HAS_LIGHTING\n";
            }

            return Shader.FromResource("open3mod.Shader.UberVertexShader.glsl", "open3mod.Shader.UberFragmentShader.glsl", pp);
        }

    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 