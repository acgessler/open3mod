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

using System.Diagnostics;

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
        /// Threshold below which alpha values are silently ignored and treated as
        /// erroneous input data. If this heuristic is wrong, the worst case is 
        /// that otherwise invisible geometry becomes visible, which is not that
        /// bad considering this is a 3D viewer.
        /// </summary>
        public const float AlphaSuppressionThreshold = 1e-5f;

        /// <summary>
        /// Check if a given assimp material requires alpha-blending for rendering
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public bool IsAlphaMaterial(Material material)
        {
            if (material.HasOpacity && IsTransparent(material.Opacity))
            {
                return true;
            }

            // Also treat material as (potentially) semi-transparent if the alpha
            // components of any of the diffuse, specular, ambient and emissive
            // colors are non-1. It is not very well-defined how assimp handles
            // these values so better count them into transparency as well.
            //
            // Ignore color values with alpha=0, however. These are most likely
            // not intended to be fully transparent.
            if (material.HasColorDiffuse && IsTransparent(material.ColorDiffuse.A))
            {
                return true;
            }

            if (material.HasColorSpecular && IsTransparent(material.ColorSpecular.A))
            {
                return true;
            }

            if (material.HasColorAmbient && IsTransparent(material.ColorAmbient.A))
            {
                return true;
            }

            if (material.HasColorEmissive && IsTransparent(material.ColorEmissive.A))
            {
                return true;
            }

            return false;
        }


        private static bool IsTransparent(float f)
        {
            return f < 1.0f && f > AlphaSuppressionThreshold;
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
        public void ApplyMaterial(Mesh mesh, Material mat, bool textured, bool shaded)
        {
            ApplyFixedFunctionMaterial(mesh, mat, textured, shaded);
        }


        public void ApplyGhostMaterial(Mesh mesh, Material material)
        {
            ApplyFixedFunctionGhostMaterial(mesh, material);
        }


        /// <summary>
        /// Uploads all the textures required for a given material to VRAM (i.e.
        /// create the corresponding Gl objects).
        /// </summary>
        /// <param name="material"></param>
        public void UploadTextures(Material material)
        {
            Debug.Assert(material != null);

            // note: keep this up to date with the code in ApplyFixedFunctionMaterial
            if (material.GetTextureCount(TextureType.Diffuse) > 0)
            {
                TextureSlot tex = material.GetTexture(TextureType.Diffuse, 0);
                var gtex = _scene.TextureSet.GetOriginalOrReplacement(tex.FilePath);

                if (gtex.State == Texture.TextureState.WinFormsImageCreated)
                {
                    gtex.Upload();
                }
            }
        }


        private void ApplyFixedFunctionMaterial(Mesh mesh, Material mat, bool textured, bool shaded)
        {
            if ((mesh == null || mesh.HasNormals) && shaded)
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

            // note: keep semantics of hasAlpha consistent with IsAlphaMaterial()
            var hasAlpha = false;

            // note: keep this up-to-date with the code in UploadTextures()
            if (textured && mat.GetTextureCount(TextureType.Diffuse) > 0)
            {
                TextureSlot tex = mat.GetTexture(TextureType.Diffuse, 0);
                var gtex = _scene.TextureSet.GetOriginalOrReplacement(tex.FilePath);

                hasAlpha = hasAlpha || gtex.HasAlpha();

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
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, color);

            color = new Color4(0, 0, 0, 1.0f);
            if (mat.HasColorSpecular)
            {
                color = AssimpToOpenTk.FromColor(mat.ColorSpecular);
                if (color.A < AlphaSuppressionThreshold) // s.a.
                {
                    color.A = 1.0f;
                }
            }
            color.A *= alpha;
            hasAlpha = hasAlpha || color.A < 1.0f;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, color);

            color = new Color4(.2f, .2f, .2f, 1.0f);
            if (mat.HasColorAmbient)
            {
                color = AssimpToOpenTk.FromColor(mat.ColorAmbient);
                if (color.A < AlphaSuppressionThreshold) // s.a.
                {
                    color.A = 1.0f;
                }
            }
            color.A *= alpha;
            hasAlpha = hasAlpha || color.A < 1.0f;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, color);

            color = new Color4(0, 0, 0, 1.0f);
            if (mat.HasColorEmissive)
            {
                color = AssimpToOpenTk.FromColor(mat.ColorEmissive);
                if (color.A < AlphaSuppressionThreshold) // s.a.
                {
                    color.A = 1.0f;
                }
            }
            color.A *= alpha;
            hasAlpha = hasAlpha || color.A < 1.0f;
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


        private void ApplyFixedFunctionGhostMaterial(Mesh mesh, Material mat)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.DepthMask(false);

            if (mesh == null || mesh.HasNormals)
            {
                GL.Enable(EnableCap.Lighting);
            }
            else
            {
                GL.Disable(EnableCap.Lighting);
            }

            GL.Disable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Texture2D);

            var color = new Color4(.6f, .6f, .9f, 0.15f);           
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, color);

            color = new Color4(1, 1, 1, 0.4f);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, color);

            color = new Color4(.2f, .2f, .2f, 0.1f);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, color);

            color = new Color4(0, 0, 0, 0.0f);       
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, color);

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 16.0f);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 