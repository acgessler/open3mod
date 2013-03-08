///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [SceneRendererClassicGl.cs]
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
    public class SceneRendererClassicGl : ISceneRenderer
    {
        private readonly Scene _owner;
        private readonly Vector3 _initposeMin;
        private readonly Vector3 _initposeMax;
        private int _displayList;
        private RenderFlags _lastFlags;


        private readonly CpuSkinningEvaluator _skinner;

        
        internal SceneRendererClassicGl(Scene owner, Vector3 initposeMin, Vector3 initposeMax)
        {
            _owner = owner;
            _initposeMin = initposeMin;
            _initposeMax = initposeMax;

            Debug.Assert(_owner.Raw != null);    
            _skinner = new CpuSkinningEvaluator(owner);
        }


        public void Update(double delta)
        {
            _skinner.Update();
        }


        public void Render(ICameraController cam, HashSet<Node> visibleNodes, bool visibleSetChanged, bool texturesChanged,
            RenderFlags flags)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);
            GL.FrontFace(FrontFaceDirection.Ccw);

            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Light0);

            if (flags.HasFlag(RenderFlags.Wireframe))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }

            GL.MatrixMode(MatrixMode.Modelview);
            var lookat = cam == null ? Matrix4.LookAt(0, 10, 5, 0, 0, 0, 0, 1, 0) : cam.GetView();

            GL.LoadMatrix(ref lookat);

            var tmp = _initposeMax.X - _initposeMin.X;
            tmp = Math.Max(_initposeMax.Y - _initposeMin.Y, tmp);
            tmp = Math.Max(_initposeMax.Z - _initposeMin.Z, tmp);
            tmp = 2.0f / tmp;
            GL.Scale(tmp,tmp,tmp);

            GL.Translate(-(_initposeMin + _initposeMax) * 0.5f);

            // build and cache opengl displaylist and update only when the scene changes.
            // when the scene is being animated, this is bad because it changes every
            // frame anyway. In this case  we don't use a displist.
            var animated = _owner.SceneAnimator.IsAnimationActive;
            if (_displayList == 0 || visibleSetChanged || texturesChanged || flags != _lastFlags || animated)
            {
                _lastFlags = flags;

                if (!animated)
                {
                    if (_displayList == 0)
                    {
                        _displayList = GL.GenLists(1);
                    }
                    GL.NewList(_displayList, ListMode.Compile);
                }

                RecursiveRender(_owner.Raw.RootNode, visibleNodes, flags, animated);

                if (flags.HasFlag(RenderFlags.ShowSkeleton) || flags.HasFlag(RenderFlags.ShowNormals))
                {
                    RecursiveRenderNoScale(_owner.Raw.RootNode, visibleNodes, flags, 1.0f / tmp, animated);
                }

                GL.EndList();
            }

            if (!animated)
            {
                GL.CallList(_displayList);
            }

            // always switch back to FILL
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
        }


        private void RecursiveRender(Node node, HashSet<Node> visibleNodes, RenderFlags flags, bool animated)
        {
            Matrix4 m;
            if (animated)
            {
                _owner.SceneAnimator.GetLocalTransform(node, out m);
            }
            else
            {
                m = AssimpToOpenTk.FromMatrix(node.Transform);
            }
            // TODO for some reason, all OpenTk matrices need a ^T - clarify our conventions somewhere
            m.Transpose();
     
            GL.PushMatrix();        
            GL.MultMatrix(ref m);

            

            if (node.HasMeshes && (visibleNodes == null || visibleNodes.Contains(node)))
            {
                foreach (var index in node.MeshIndices)
                {
                    var mesh = _owner.Raw.Meshes[index];
                    _owner.MaterialMapper.ApplyMaterial(mesh,_owner.Raw.Materials[mesh.MaterialIndex]);
               
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
                                var vertColor = AssimpToOpenTk.FromColor(mesh.GetVertexColors(0)[indice]);
                                if (flags.HasFlag(RenderFlags.Shaded))
                                {
                                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, vertColor);
                                }
                                else
                                {
                                    GL.Color4(vertColor);
                                }
                            }
                            if (mesh.HasNormals)
                            {
                                Vector3 normal;
                                if (skinning)
                                {
                                    _skinner.GetTransformedVertexNormal(node, index, indice, out normal);
                                }
                                else
                                {
                                    normal = AssimpToOpenTk.FromVector(mesh.Normals[indice]);
                                }
                               
                                GL.Normal3(normal);
                            }
                            if (hasTexCoords)
                            {
                                var uvw = AssimpToOpenTk.FromVector(mesh.GetTextureCoords(0)[indice]);
                                GL.TexCoord2(uvw.X, 1 - uvw.Y);
                            }
 
                            Vector3 pos;
                            if (skinning)
                            {
                                _skinner.GetTransformedVertexPosition(node, index, indice, out pos);
                            }
                            else
                            {
                                pos = AssimpToOpenTk.FromVector(mesh.Vertices[indice]);
                            }
                            GL.Vertex3(pos);
                        }
                        GL.End();
                    }
                   
                    if (flags.HasFlag(RenderFlags.ShowBoundingBoxes))
                    {
                        DrawBoundingBox(node, index, mesh, skinning);
                    }
                }
            }
                      
            for (var i = 0; i < node.ChildCount; i++)
            {              
                RecursiveRender(node.Children[i], visibleNodes, flags, animated);
            }

            GL.PopMatrix();
        }


        private void RecursiveRenderNoScale(Node node, HashSet<Node> visibleNodes, RenderFlags flags, 
            float invGlobalScale, 
            bool animated)
        {
            // TODO unify our use of OpenTK and Assimp matrices 
            Matrix4x4 m;
            Matrix4 mConv;
            if (animated)
            {
                _owner.SceneAnimator.GetLocalTransform(node, out mConv);
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
                DrawSkeletonBone(node, invGlobalScale);
            }

            GL.PushMatrix();
            GL.MultMatrix(ref mConv);

            if (node.HasMeshes && (visibleNodes == null || visibleNodes.Contains(node)))
            {
                foreach (var index in node.MeshIndices)
                {
                    var mesh = _owner.Raw.Meshes[index];           
                    if (flags.HasFlag(RenderFlags.ShowNormals))
                    {
                        DrawNormals(node, index, mesh, mesh.HasBones && animated, invGlobalScale);
                    }
                }
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                RecursiveRenderNoScale(node.Children[i], visibleNodes, flags, invGlobalScale, animated);
            }

            GL.PopMatrix();
        }


        private void DrawSkeletonBone(Node node, float invGlobalScale)
        {
            var target = new Vector3(node.Transform.A4, node.Transform.B4, node.Transform.C4);
            if (target.LengthSquared > 1e-6f)
            {
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.Texture2D);
                GL.Enable(EnableCap.ColorMaterial);
                GL.Disable(EnableCap.DepthTest);

                GL.Color4(new Color4(0.0f, 0.5f, 1.0f, 1.0f));

                var right = new Vector3(1, 0, 0);
                var targetNorm = target;
                targetNorm.Normalize();

                Vector3 up;
                Vector3.Cross(ref targetNorm, ref right, out up);
                Vector3.Cross(ref up, ref targetNorm, out right);

                const float jointWidth = 0.03f;

                up *= invGlobalScale;
                right *= invGlobalScale;

                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(-jointWidth*up + -jointWidth*right);
                GL.Vertex3(-jointWidth*up + jointWidth*right);
                GL.Vertex3(jointWidth*up + jointWidth*right);
                GL.Vertex3(jointWidth*up + -jointWidth*right);
                GL.End();

                GL.Begin(BeginMode.Lines);
                GL.Vertex3(-jointWidth*up + -jointWidth*right);
                GL.Vertex3(target);
                GL.Vertex3(-jointWidth*up + jointWidth*right);
                GL.Vertex3(target);
                GL.Vertex3(jointWidth*up + jointWidth*right);
                GL.Vertex3(target);
                GL.Vertex3(jointWidth*up + -jointWidth*right);
                GL.Vertex3(target);

                GL.Color4(new Color4(1.0f, 1.0f, 0.0f, 1.0f));
                GL.Vertex3(Vector3.Zero);
                GL.Vertex3(target);
                GL.End();

                GL.Disable(EnableCap.ColorMaterial);
                GL.Enable(EnableCap.DepthTest);
            }           
        }


        private void DrawBoundingBox(Node node, int meshIndex, Mesh mesh, bool skinning)
        {           
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Color4(new Color4(1.0f, 0.0f, 0.0f, 1.0f));

            var min = new Vector3(1e10f, 1e10f, 1e10f);
            var max = new Vector3(-1e10f, -1e10f, -1e10f);
            for (uint i = 0; i < mesh.VertexCount; ++i)
            {              
                Vector3 tmp;
                if (skinning)
                {
                    _skinner.GetTransformedVertexPosition(node, meshIndex, i, out tmp);
                }
                else
                {
                    tmp = AssimpToOpenTk.FromVector(mesh.Vertices[i]);
                }          

                min.X = Math.Min(min.X, tmp.X);
                min.Y = Math.Min(min.Y, tmp.Y);
                min.Z = Math.Min(min.Z, tmp.Z);

                max.X = Math.Max(max.X, tmp.X);
                max.Y = Math.Max(max.Y, tmp.Y);
                max.Z = Math.Max(max.Z, tmp.Z);               
            }

            GL.Begin(BeginMode.LineLoop);
            GL.Vertex3(min);
            GL.Vertex3(new Vector3(min.X, max.Y, min.Z));
            GL.Vertex3(new Vector3(min.X, max.Y, max.Z));
            GL.Vertex3(new Vector3(min.X, min.Y, max.Z));
            GL.End();

            GL.Begin(BeginMode.LineLoop);
            GL.Vertex3(new Vector3(max.X, min.Y, min.Z));
            GL.Vertex3(new Vector3(max.X, max.Y, min.Z));
            GL.Vertex3(new Vector3(max.X, max.Y, max.Z));
            GL.Vertex3(new Vector3(max.X, min.Y, max.Z));
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(min);
            GL.Vertex3(new Vector3(max.X, min.Y, min.Z));

            GL.Vertex3(new Vector3(min.X, max.Y, min.Z));
            GL.Vertex3(new Vector3(max.X, max.Y, min.Z));

            GL.Vertex3(new Vector3(min.X, max.Y, max.Z));
            GL.Vertex3(new Vector3(max.X, max.Y, max.Z));

            GL.Vertex3(new Vector3(min.X, min.Y, max.Z));
            GL.Vertex3(new Vector3(max.X, min.Y, max.Z));
            GL.End();

            GL.Disable(EnableCap.ColorMaterial);
        }


        private void DrawNormals(Node node, int meshIndex, Mesh mesh, bool skinning, float invGlobalScale)
        {
            if(!mesh.HasNormals)
            {
                return;
            }
            // scale by scene size because the scene will be resized to fit
            // the unit box but the normals should have a fixed length
            var scale = invGlobalScale * 0.03f;

            GL.Begin(BeginMode.Lines);

            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Color4(new Color4(0.0f, 1.0f, 0.0f, 1.0f));

            for (uint i = 0; i < mesh.VertexCount; ++i)
            {
                Vector3 v;          
                if(skinning)
                {
                    _skinner.GetTransformedVertexPosition(node, meshIndex, i, out v);
                }
                else
                {
                    v = AssimpToOpenTk.FromVector(mesh.Vertices[i]);
                }

                Vector3 n;
                if (skinning)
                {
                    _skinner.GetTransformedVertexNormal(node, meshIndex, i, out n);
                }
                else
                {
                    n = AssimpToOpenTk.FromVector(mesh.Normals[i]);
                }
         
                GL.Vertex3(v);
                GL.Vertex3(v+n*scale);
            }
            GL.End();

            GL.Disable(EnableCap.ColorMaterial);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 