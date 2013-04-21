///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [SceneRendererModernGl.cs]
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
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    /// <summary>
    /// "Modern" OpenGl-based renderer using VBOs and shaders.
    /// </summary>
    public class SceneRendererModernGl : SceneRendererShared, ISceneRenderer
    {
        private RenderMesh[] _meshes;

        internal SceneRendererModernGl(Scene owner, Vector3 initposeMin, Vector3 initposeMax) 
            : base(owner, initposeMin, initposeMax)
        {
            _meshes = new RenderMesh[owner.Raw.MeshCount];
        }


        /// <summary>
        /// <see cref="ISceneRenderer.Update"/>
        /// </summary>   
        public void Update(double delta)
        {
            Skinner.Update();
        }


        /// <summary>
        /// <see cref="ISceneRenderer.Render"/>
        /// </summary>   
        public void Render(ICameraController cam, Dictionary<Node, List<Mesh>> visibleMeshesByNode,
            bool visibleSetChanged,
            bool texturesChanged,
            RenderFlags flags)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);
            GL.FrontFace(FrontFaceDirection.Ccw);

            if (flags.HasFlag(RenderFlags.Wireframe))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }

            // If textures changed, we may need to upload some of them to VRAM.
            if (texturesChanged)
            {
                UploadTextures();
            }

            var tmp = 1.0f; // todo

            //
            var animated = Owner.SceneAnimator.IsAnimationActive;
            var needAlpha = RecursiveRender(Owner.Raw.RootNode, visibleMeshesByNode, flags, animated);
            if (flags.HasFlag(RenderFlags.ShowSkeleton) || flags.HasFlag(RenderFlags.ShowNormals))
            {
                //RecursiveRenderNoScale(Owner.Raw.RootNode, visibleMeshesByNode, flags, 1.0f / tmp, animated);
            }

            if (needAlpha)
            {
                // handle semi-transparent geometry              
                RecursiveRenderWithAlpha(Owner.Raw.RootNode, visibleMeshesByNode, flags, animated);
            }


            // always switch back to FILL
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
        }


        /// <summary>
        /// Recursive rendering function for opaque meshes that also checks whether there
        /// is any need for a second rendering pass to draw semi-transparent meshes.
        /// </summary>
        /// <param name="node">Current node</param>
        /// <param name="visibleMeshesByNode"> </param>
        /// <param name="flags">Rendering flags</param>
        /// <param name="animated">Play animation?</param>
        /// <returns>whether there is any need to do a second render pass with alpha blending enabled</returns>
        private bool RecursiveRender(Node node,
            Dictionary<Node, List<Mesh>> visibleMeshesByNode,
            RenderFlags flags, bool animated)
        {
            var needAlpha = false;

            Matrix4 m;
            if (animated)
            {
                Owner.SceneAnimator.GetLocalTransform(node, out m);
            }
            else
            {
                m = AssimpToOpenTk.FromMatrix(node.Transform);
            }
            // TODO for some reason, all OpenTk matrices need a ^T - we should clarify our conventions somewhere
            m.Transpose();

            // the following permutations could be compacted into one big loop with lots of
            // condition magic, but at the cost of readability and also performance.
            // we therefore keep it redundant and stupid.
            if (node.HasMeshes)
            {
                needAlpha = DrawOpaqueMeshes(node, visibleMeshesByNode, flags, animated);
            }


            for (var i = 0; i < node.ChildCount; i++)
            {
                needAlpha = RecursiveRender(node.Children[i], visibleMeshesByNode, flags, animated) || needAlpha;
            }
            return needAlpha;
        }


        /// <summary>
        /// Recursive rendering function for semi-transparent (i.e. alpha-blended) meshes.
        /// 
        /// Alpha blending is not globally on, meshes need to do that on their own. 
        /// 
        /// This render function is called _after_ solid geometry has been drawn, so the 
        /// relative order between transparent and opaque geometry is maintained. There
        /// is no further ordering within the alpha rendering pass.
        /// </summary>
        /// <param name="node">Current node</param>
        /// <param name="visibleNodes">Set of visible meshes</param>
        /// <param name="flags">Rendering flags</param>
        /// <param name="animated">Play animation?</param>
        private void RecursiveRenderWithAlpha(Node node, Dictionary<Node, List<Mesh>> visibleNodes,
            RenderFlags flags,
            bool animated)
        {
            Matrix4 m;
            if (animated)
            {
                Owner.SceneAnimator.GetLocalTransform(node, out m);
            }
            else
            {
                m = AssimpToOpenTk.FromMatrix(node.Transform);
            }
            // TODO for some reason, all OpenTk matrices need a ^T - clarify our conventions somewhere
            m.Transpose();

            // the following permutations could be compacted into one big loop with lots of
            // condition magic, but at the cost of readability and also performance.
            // we therefore keep it redundant and stupid.
            if (node.HasMeshes)
            {
                DrawAlphaMeshes(node, visibleNodes, flags, animated);
            }


            for (var i = 0; i < node.ChildCount; i++)
            {
                RecursiveRenderWithAlpha(node.Children[i], visibleNodes, flags, animated);
            }
        }


        protected override bool DrawMesh(Node node, bool animated, bool p2, int index, Mesh mesh, RenderFlags flags)
        {
            return true;
        }


        public void Dispose()
        {          
            GC.SuppressFinalize(this);
        }


#if DEBUG
        ~SceneRendererModernGl()
        {
            // OpenTk is unsafe from here, explicit Dispose() is required.
            Debug.Assert(false);
        }
#endif
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 