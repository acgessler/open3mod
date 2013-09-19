///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [MaterialMapperClassicGl.cs]
// (c) 2012-2013, Open3Mod Contributors
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

using System.Diagnostics;

namespace open3mod
{
    public sealed class MaterialMapperClassicGl : MaterialMapper
    {

        internal MaterialMapperClassicGl(Scene scene)
            : base(scene)
        { }



        public override void Dispose()
        {
            // no dispose semantics in this implementation
        }



        public override void ApplyMaterial(Mesh mesh, Material mat, bool textured, bool shaded)
        {
            ApplyFixedFunctionMaterial(mesh, mat, textured, shaded);
        }


        public override void ApplyGhostMaterial(Mesh mesh, Material material, bool shaded)
        {
            ApplyFixedFunctionGhostMaterial(mesh, material, shaded);
        }


        private void ApplyFixedFunctionMaterial(Mesh mesh, Material mat, bool textured, bool shaded)
        {
            shaded = shaded && (mesh == null || mesh.HasNormals);
            if (shaded)
            {
                GL.Enable(EnableCap.Lighting);
            }
            else
            {
                GL.Disable(EnableCap.Lighting);
            }

            var hasColors = mesh != null && mesh.HasVertexColors(0);
            if (hasColors)
            {
                GL.Enable(EnableCap.ColorMaterial);
                GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
            }
            else
            {
                GL.Disable(EnableCap.ColorMaterial);
            }

            // note: keep semantics of hasAlpha consistent with IsAlphaMaterial()
            var hasAlpha = false;
            var hasTexture = false;

            // note: keep this up-to-date with the code in UploadTextures()
            if (textured && mat.GetMaterialTextureCount(TextureType.Diffuse) > 0)
            {
                hasTexture = true;

                TextureSlot tex;
                mat.GetMaterialTexture(TextureType.Diffuse, 0, out tex);
                var gtex = _scene.TextureSet.GetOriginalOrReplacement(tex.FilePath);

                hasAlpha = hasAlpha || gtex.HasAlpha == Texture.AlphaState.HasAlpha;

                if(gtex.State == Texture.TextureState.GlTextureCreated)
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    gtex.BindGlTexture();
   
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

            GL.Enable(EnableCap.Normalize);

            var alpha = 1.0f;
            if (mat.HasOpacity)
            {
                alpha = mat.Opacity;
                if (alpha < AlphaSuppressionThreshold) // suppress zero opacity, this is likely wrong input data
                {
                    alpha = 1.0f;
                }
            }

            var color = new Color4(.8f, .8f, .8f, 1.0f);
            if (mat.HasColorDiffuse)
            {
                color = AssimpToOpenTk.FromColor(mat.ColorDiffuse);
                if (color.A < AlphaSuppressionThreshold) // s.a.
                {
                    color.A = 1.0f;
                }
            }
            color.A *= alpha;
            hasAlpha = hasAlpha || color.A < 1.0f;

            if (shaded)
            {
                // if the material has a texture but the diffuse color texture is all black,
                // then heuristically assume that this is an import/export flaw and substitute
                // white.
                if (hasTexture && color.R < 1e-3f && color.G < 1e-3f && color.B < 1e-3f)
                {
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Color4.White);
                }
                else
                {
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, color);
                }

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
                // todo: I don't even remember how shininess strength was supposed to be handled in assimp
                if (mat.HasShininessStrength)
                {
                    strength = mat.ShininessStrength;
                }

                var exp = shininess*strength;
                if (exp >= 128.0f) // 128 is the maximum exponent as per the Gl spec
                {
                    exp = 128.0f;
                }

                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, exp);
            }
            else if (!hasColors)
            {
                GL.Color3(color.R, color.G, color.B);
            }

            if (hasAlpha)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.DepthMask(false);
            }
            else
            {
                GL.Disable(EnableCap.Blend);
                GL.DepthMask(true);
            }
        }


        private void ApplyFixedFunctionGhostMaterial(Mesh mesh, Material mat, bool shaded)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.DepthMask(false);

            var color = new Color4(.6f, .6f, .9f, 0.15f);           

            shaded = shaded && (mesh == null || mesh.HasNormals);
            if (shaded)
            {
                GL.Enable(EnableCap.Lighting);

                
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, color);

                color = new Color4(1, 1, 1, 0.4f);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, color);

                color = new Color4(.2f, .2f, .2f, 0.1f);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, color);

                color = new Color4(0, 0, 0, 0.0f);       
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, color);

                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 16.0f);
            }
            else
            {
                GL.Disable(EnableCap.Lighting);
                GL.Color3(color.R, color.G, color.B);
            }

            GL.Disable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Texture2D);
        }
    }
}
