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
                //RecursiveRenderWithAlpha(Owner.Raw.RootNode, visibleMeshesByNode, flags, animated);
            }


            // always switch back to FILL
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
        }


        /// <summary>
        /// Recursive rendering function
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

            GL.PushMatrix();
            GL.MultMatrix(ref m);

            // the following permutations could be compacted into one big loop with lots of
            // condition magic, but at the cost of readability and also performance.
            // we therefore keep it redundant and stupid.
            if (node.HasMeshes)
            {
                if (visibleMeshesByNode == null)
                {
                    // everything is visible. alpha-blended materials are delayed for 2nd pass
                    foreach (var index in node.MeshIndices)
                    {
                        var mesh = Owner.Raw.Meshes[index];
                        if (IsAlphaMaterial[mesh.MaterialIndex])
                        {
                            needAlpha = true;
                            continue;
                        }

                        var skinning = DrawMesh(node, animated, false, index, mesh, flags);
                        if (flags.HasFlag(RenderFlags.ShowBoundingBoxes))
                        {
                            OverlayBoundingBox.DrawBoundingBox(node, index, mesh, skinning ? Skinner : null);
                        }
                    }
                }
                else
                {
                    List<Mesh> meshList;
                    if (visibleMeshesByNode.TryGetValue(node, out meshList))
                    {
                        // some meshes of this node are visible. alpha-blended materials are delayed for 2nd pass
                        foreach (var index in node.MeshIndices)
                        {
                            var mesh = Owner.Raw.Meshes[index];

                            if (IsAlphaMaterial[mesh.MaterialIndex] || (meshList != null && !meshList.Contains(mesh)))
                            {
                                needAlpha = true;
                                continue;
                            }

                            var skinning = DrawMesh(node, animated, false, index, mesh, flags);
                            if (flags.HasFlag(RenderFlags.ShowBoundingBoxes))
                            {
                                OverlayBoundingBox.DrawBoundingBox(node, index, mesh, skinning ? Skinner : null);
                            }
                        }
                    }
                    else
                    {
                        // node not visible, draw ghosts in 2nd pass
                        needAlpha = true;
                    }
                }
            }


            for (var i = 0; i < node.ChildCount; i++)
            {
                needAlpha = RecursiveRender(node.Children[i], visibleMeshesByNode, flags, animated) || needAlpha;
            }

            GL.PopMatrix();
            return needAlpha;
        }


        private bool DrawMesh(Node node, bool animated, bool p2, int index, Mesh mesh, RenderFlags flags)
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
