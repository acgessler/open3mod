///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [MaterialMapper.cs]
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

namespace open3mod
{
    /// <summary>
    /// Map assimp materials to OpenGl materials and shaders. Each scene has its own
    /// MaterialMapper, which is accessible via the Scene.MaterialMapper property.
    /// 
    /// The class interface is implemented for every type of renderer.
    /// </summary>
    public abstract class MaterialMapper : IDisposable
    {
        protected readonly Scene _scene;

        protected MaterialMapper(Scene scene)
        {
            _scene = scene;
        }


        public abstract void Dispose();


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


            if (material.GetMaterialTextureCount(TextureType.Diffuse) > 0)
            {
                TextureSlot tex;
                material.GetMaterialTexture(TextureType.Diffuse, 0, out tex);
                var gtex = _scene.TextureSet.GetOriginalOrReplacement(tex.FilePath);

                if(gtex.HasAlpha == Texture.AlphaState.HasAlpha)
                {
                    return true;
                }
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
        public abstract void ApplyMaterial(Mesh mesh, Material mat, bool textured, bool shaded);
        public abstract void ApplyGhostMaterial(Mesh mesh, Material material, bool shaded);

        public abstract void BeginScene(Renderer renderer);
        public abstract void EndScene(Renderer renderer);
      

        /// <summary>
        /// Uploads all the textures required for a given material to VRAM (i.e.
        /// create the corresponding Gl objects). Textures that have been 
        /// uploaded before are not attempted again.
        /// </summary>
        /// <param name="material"></param>
        /// <returns>Whether there were any new texture uploads</returns>
        public bool UploadTextures(Material material)
        {
            Debug.Assert(material != null);
            var any = false;

            // note: keep this up to date with the code in ApplyFixedFunctionMaterial
            if (material.GetMaterialTextureCount(TextureType.Diffuse) > 0)
            {
                TextureSlot tex;
                material.GetMaterialTexture(TextureType.Diffuse, 0, out tex);

                var gtex = _scene.TextureSet.GetOriginalOrReplacement(tex.FilePath);

                if (gtex.State == Texture.TextureState.WinFormsImageCreated)
                {
                    gtex.Upload();
                    any = true;
                }
                else if (gtex.ReconfigureUploadedTextureRequested)
                {
                    gtex.ReconfigureUploadedTexture();
                }
            }
            return any;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 