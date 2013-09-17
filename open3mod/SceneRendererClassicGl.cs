///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [SceneRendererClassicGl.cs]
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    /// <summary>
    /// Render a Scene using old-school OpenGl, that is, display lists,
    /// matrix stacks and glVertexN-family calls.
    /// </summary>
    public sealed class SceneRendererClassicGl : SceneRendererShared, ISceneRenderer
    {        
        private int _displayList;
        private int _displayListAlpha;
        private RenderFlags _lastFlags;
      

        internal SceneRendererClassicGl(Scene owner, Vector3 initposeMin, Vector3 initposeMax)
            : base(owner, initposeMin, initposeMax)
        {
            
        }


        public void Dispose()
        {
            if (_displayList != 0)
            {
                GL.DeleteLists(_displayList, 1);
                _displayList = 0;
            }
            if (_displayListAlpha != 0)
            {
                GL.DeleteLists(_displayListAlpha, 1);
                _displayListAlpha = 0;
            }
            GC.SuppressFinalize(this);
        }


#if DEBUG
        ~SceneRendererClassicGl()
        {
            // OpenTk is unsafe from here, explicit Dispose() is required.
            Debug.Assert(false);
        }
#endif


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
            RenderFlags flags, 
            Renderer renderer)
        {            
            GL.Disable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);

            // set fixed-function lighting parameters
            GL.ShadeModel(ShadingModel.Smooth);
            GL.LightModel(LightModelParameter.LightModelAmbient, new[] { 0.3f, 0.3f, 0.3f, 1 });
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);

           
            if (flags.HasFlag(RenderFlags.Wireframe))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }

            var tmp = InitposeMax.X - InitposeMin.X;
            tmp = Math.Max(InitposeMax.Y - InitposeMin.Y, tmp);
            tmp = Math.Max(InitposeMax.Z - InitposeMin.Z, tmp);
            var scale = 2.0 / tmp;     

            if (cam != null)
            {
                cam.SetPivot((InitposeMax + InitposeMin) * 0.5f * (float)scale);
            }
            var view = cam == null ? Matrix4.LookAt(0, 10, 5, 0, 0, 0, 0, 1, 0) : cam.GetView();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref view);

            // light direction
            var dir = new Vector3(1, 1, 0);
            var mat = renderer.LightRotation;
            Vector3.TransformNormal(ref dir, ref mat, out dir);
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { dir.X, dir.Y, dir.Z, 0 });

            // light color
            var col = new Vector3(1, 1, 1);
            col *= (0.25f + 1.5f * GraphicsSettings.Default.OutputBrightness / 100.0f) * 1.5f;

            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { col.X, col.Y, col.Z, 1 });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { col.X, col.Y, col.Z, 1 });

            if (flags.HasFlag(RenderFlags.Shaded))
            {
                OverlayLightSource.DrawLightSource(dir);
            }

           
            GL.Scale(scale, scale, scale);


            // If textures changed, we may need to upload some of them to VRAM.
            // it is important this happens here and not accidentially while
            // compiling a displist.
            if (texturesChanged)
            {
                UploadTextures();
            }

            GL.PushMatrix();

            // Build and cache Gl displaylists and update only when the scene changes.
            // when the scene is being animated, this is bad because it changes every
            // frame anyway. In this case  we don't use a displist.
            var animated = Owner.SceneAnimator.IsAnimationActive;
            if (_displayList == 0 || visibleSetChanged || texturesChanged || flags != _lastFlags || animated)
            {
                _lastFlags = flags;

                // handle opaque geometry
                if (!animated)
                {
                    if (_displayList == 0)
                    {
                        _displayList = GL.GenLists(1);
                    }
                    
                    GL.NewList(_displayList, ListMode.Compile);
                }
                
                var needAlpha = RecursiveRender(Owner.Raw.RootNode, visibleMeshesByNode, flags, animated);

                if (flags.HasFlag(RenderFlags.ShowSkeleton) || flags.HasFlag(RenderFlags.ShowNormals))
                {
                    RecursiveRenderNoScale(Owner.Raw.RootNode, visibleMeshesByNode, flags, 1.0f / tmp, animated);
                }

                if (!animated)
                {
                    GL.EndList();
                }

                if (needAlpha)
                {
                    // handle semi-transparent geometry
                    if (!animated)
                    {
                        if (_displayListAlpha == 0)
                        {
                            _displayListAlpha = GL.GenLists(1);
                        }
                        GL.NewList(_displayListAlpha, ListMode.Compile);
                    }
                    RecursiveRenderWithAlpha(Owner.Raw.RootNode, visibleMeshesByNode, flags, animated);

                    if (!animated)
                    {
                        GL.EndList();
                    }
                }
                else if (_displayListAlpha != 0)
                {
                    GL.DeleteLists(_displayListAlpha, 1);
                    _displayListAlpha = 0;
                }
            }

            if (!animated)
            {
                GL.CallList(_displayList);
                if (_displayListAlpha != 0)
                {
                    GL.CallList(_displayListAlpha);
                }
            }

            GL.PopMatrix();

            // always switch back to FILL
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Enable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.DepthTest);

            // TEST CODE to visualize mid point (pivot) and origin
            GL.LoadMatrix(ref view);
            GL.Begin(BeginMode.Lines);

            GL.Vertex3((InitposeMin + InitposeMax) * 0.5f * (float)scale);
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0,0,0);
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3((InitposeMin + InitposeMax) * 0.5f * (float)scale);
            GL.Color3(0.0f, 1.0f, 0.0f);

            GL.Vertex3(10, 10, 10);
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.End();

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

            
            if (node.HasMeshes)
            {
                needAlpha = DrawOpaqueMeshes(node, visibleMeshesByNode, flags, animated);
            }

         
            for (var i = 0; i < node.ChildCount; i++)
            {
                needAlpha = RecursiveRender(node.Children[i], visibleMeshesByNode, flags, animated) || needAlpha;
            }

            GL.PopMatrix();
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

            GL.PushMatrix();
            GL.MultMatrix(ref m);

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

            GL.PopMatrix();
        }



        /// <summary>
        /// Draw a mesh using either its given material or a transparent "ghost" material.
        /// </summary>
        /// <param name="node">Current node</param>
        /// <param name="animated">Specifies whether animations should be played</param>
        /// <param name="showGhost">Indicates whether to substitute the mesh' material with a
        /// "ghost" surrogate material that allows looking through the geometry.</param>
        /// <param name="index">Mesh index in the scene</param>
        /// <param name="mesh">Mesh instance</param>
        /// <param name="flags"> </param>
        /// <returns></returns>
        protected override bool DrawMesh(Node node, bool animated, bool showGhost, int index, Mesh mesh, RenderFlags flags)
        {
            if (showGhost)
            {
                Owner.MaterialMapper.ApplyGhostMaterial(mesh, Owner.Raw.Materials[mesh.MaterialIndex], 
                    flags.HasFlag(RenderFlags.Shaded));
            }
            else
            {
                Owner.MaterialMapper.ApplyMaterial(mesh, Owner.Raw.Materials[mesh.MaterialIndex], 
                    flags.HasFlag(RenderFlags.Textured), 
                    flags.HasFlag(RenderFlags.Shaded));
            }

            if (GraphicsSettings.Default.BackFaceCulling)
            {
                GL.FrontFace(FrontFaceDirection.Ccw);
                GL.CullFace(CullFaceMode.Back);
                GL.Enable(EnableCap.CullFace);
            }
            else
            {
                GL.Disable(EnableCap.CullFace);
            }

            var hasColors = mesh.HasVertexColors(0);
            var hasTexCoords = mesh.HasTextureCoords(0);

            var skinning = mesh.HasBones && animated;

            foreach (var face in mesh.Faces)
            {
                BeginMode faceMode;
                switch (face.IndexCount)
                {
                    case 1:
                        faceMode = BeginMode.Points;
                        break;
                    case 2:
                        faceMode = BeginMode.Lines;
                        break;
                    case 3:
                        faceMode = BeginMode.Triangles;
                        break;
                    default:
                        faceMode = BeginMode.Polygon;
                        break;
                }

                GL.Begin(faceMode);
                for (var i = 0; i < face.IndexCount; i++)
                {
                    var indice = face.Indices[i];
                    if (hasColors)
                    {
                        var vertColor = AssimpToOpenTk.FromColor(mesh.VertexColorChannels[0][indice]);
                        GL.Color4(vertColor);
                    }
                    if (mesh.HasNormals)
                    {
                        Vector3 normal;
                        if (skinning)
                        {
                            Skinner.GetTransformedVertexNormal(node, index, (uint)indice, out normal);
                        }
                        else
                        {
                            normal = AssimpToOpenTk.FromVector(mesh.Normals[indice]);
                        }

                        GL.Normal3(normal);
                    }
                    if (hasTexCoords)
                    {
                        var uvw = AssimpToOpenTk.FromVector(mesh.TextureCoordinateChannels[0][indice]);
                        GL.TexCoord2(uvw.X, 1 - uvw.Y);
                    }

                    Vector3 pos;
                    if (skinning)
                    {
                        Skinner.GetTransformedVertexPosition(node, index, (uint)indice, out pos);
                    }
                    else
                    {
                        pos = AssimpToOpenTk.FromVector(mesh.Vertices[indice]);
                    }
                    GL.Vertex3(pos);
                }
                GL.End();
            }
            GL.Disable(EnableCap.CullFace);
            return skinning;
        }


        /// <summary>
        /// Recursive render function for drawing opaque geometry with no scaling 
        /// in the transformation chain. This is used for overlays, such as drawing
        /// normal vectors or the skeleton.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="visibleMeshesByNode"></param>
        /// <param name="flags"></param>
        /// <param name="invGlobalScale"></param>
        /// <param name="animated"></param>
        private void RecursiveRenderNoScale(Node node, Dictionary<Node, List<Mesh>> visibleMeshesByNode, RenderFlags flags, 
            float invGlobalScale,
            bool animated)
        {
            // TODO unify our use of OpenTK and Assimp matrices 
            Matrix4x4 m;
            Matrix4 mConv;
            if (animated)
            {
                Owner.SceneAnimator.GetLocalTransform(node, out mConv);
                OpenTkToAssimp.FromMatrix(ref mConv, out m);
            }
            else
            {
                m = node.Transform;
            }
            
            // get rid of the scaling part of the matrix
            // TODO this can be done faster and Decompose() doesn't handle
            // non positively semi-definite matrices correctly anyway.

            Vector3D scaling;
            Assimp.Quaternion rotation;
            Vector3D translation;
            m.Decompose(out scaling, out rotation, out translation);

            rotation.Normalize();

            m = new Matrix4x4(rotation.GetMatrix()) * Matrix4x4.FromTranslation(translation);
            mConv = AssimpToOpenTk.FromMatrix(ref m);
            mConv.Transpose();

            if (flags.HasFlag(RenderFlags.ShowSkeleton))
            {           
                var highlight = false;
                if (visibleMeshesByNode != null)
                {
                    List<Mesh> meshList;
                    if (visibleMeshesByNode.TryGetValue(node, out meshList) && meshList == null)
                    {
                        // If the user hovers over a node in the tab view, all of its descendants
                        // are added to the visible set as well. This is not the intended 
                        // behavior for skeleton joints, though! Here we only want to show the
                        // joint corresponding to the node being hovered over.

                        // Therefore, only highlight nodes whose parents either don't exist
                        // or are not in the visible set.
                        if (node.Parent == null || !visibleMeshesByNode.TryGetValue(node.Parent, out meshList) || meshList != null)
                        {
                            highlight = true;
                        }
                    }
                }
                OverlaySkeleton.DrawSkeletonBone(node, invGlobalScale,highlight);
            }

            GL.PushMatrix();
            GL.MultMatrix(ref mConv);

            if (flags.HasFlag(RenderFlags.ShowNormals))
            {
                List<Mesh> meshList = null;
                if (node.HasMeshes &&
                    (visibleMeshesByNode == null || visibleMeshesByNode.TryGetValue(node, out meshList)))
                {
                    foreach (var index in node.MeshIndices)
                    {
                        var mesh = Owner.Raw.Meshes[index];
                        if(meshList != null && !meshList.Contains(mesh))
                        {
                            continue;
                        }

                        OverlayNormals.DrawNormals(node, index, mesh, mesh.HasBones && animated ? Skinner : null, invGlobalScale);
                    }
                }
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                RecursiveRenderNoScale(node.Children[i], visibleMeshesByNode, flags, invGlobalScale, animated);
            }
            GL.PopMatrix();
        }       
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 