///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [MaterialMapper.cs]
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
using Assimp.Configs;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    /// <summary>
    /// Map assimp materials to OpenGl materials and shaders. Each scene has its own
    /// MaterialMapper, which is accessible via the Scene.MaterialMapper property.
    /// </summary>
    public class MaterialMapper
    {
        private readonly Scene _scene;

        internal MaterialMapper(Scene scene)
        {
            _scene = scene;
        }


        /// <summary>
        /// Applies a material to the Gl state machine. Depending on the renderer,
        /// this either sets GLSL shaders (GL3) or it configures the fixed function pipeline
        /// (legacy/classic).
        /// </summary>
        /// <param name="mesh">Mesh to be drawn. This parameter may be left null
        ///    to use a material with geometry other than assimp meshes (i.e.
        ///    for the material preview tab). In this case, it is assumed that
        ///    the geometry to be used with the materials specifies normals,
        ///    one set of UV coordinates but no vertex colors.
        ///    TODO tangents, bitangents?
        /// </param>
        /// <param name="mat">Material to be applied, must be non-null</param>
        public void ApplyMaterial(Mesh mesh, Material mat)
        {
            ApplyFixedFunctionMaterial(mesh, mat);
        }



        private void ApplyFixedFunctionMaterial(Mesh mesh, Material mat)
        {
            if (mesh == null || mesh.HasNormals)
            {
                GL.Enable(EnableCap.Lighting);
            }
            else
            {
                GL.Disable(EnableCap.Lighting);
            }

            var hasColors = mesh == null || mesh.HasVertexColors(0);
            if (hasColors)
            {
                GL.Enable(EnableCap.ColorMaterial);
                GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
            }
            else
            {
                GL.Disable(EnableCap.ColorMaterial);
            }

            if (mat.GetTextureCount(TextureType.Diffuse) > 0)
            {
                TextureSlot tex = mat.GetTexture(TextureType.Diffuse, 0);
                var gtex = _scene.TextureSet.GetOriginalOrReplacement(tex.FilePath);

                if(gtex.GlTexture != 0)
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, gtex.GlTexture);
           
                    GL.Enable(EnableCap.Texture2D);
                }
                else
                {
                    GL.Disable(EnableCap.Texture2D);
                }
            }
            else
            {
                GL.Disable(EnableCap.Texture2D);
            }

            var color = new Color4(.8f, .8f, .8f, 1.0f);
            if (mat.HasColorDiffuse)
            {
                color = AssimpToOpenTk.FromColor(mat.ColorDiffuse);
            }
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, color);

            color = new Color4(0, 0, 0, 1.0f);
            if (mat.HasColorSpecular)
            {
                color = AssimpToOpenTk.FromColor(mat.ColorSpecular);
            }
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, color);

            color = new Color4(.2f, .2f, .2f, 1.0f);
            if (mat.HasColorAmbient)
            {
                color = AssimpToOpenTk.FromColor(mat.ColorAmbient);
            }
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, color);

            color = new Color4(0, 0, 0, 1.0f);
            if (mat.HasColorEmissive)
            {
                color = AssimpToOpenTk.FromColor(mat.ColorEmissive);
            }
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, color);

            float shininess = 1;
            float strength = 1;
            if (mat.HasShininess)
            {
                shininess = mat.Shininess;
            }
            if (mat.HasShininessStrength)
            {
                strength = mat.ShininessStrength;
            }

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, shininess * strength);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 