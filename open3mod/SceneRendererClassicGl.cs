using System;
using System.Collections.Generic;
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
        private int _displayList;


        public SceneRendererClassicGl(Scene owner)
        {
            _owner = owner;
        }


        public void Render(UiState state, ICameraController cam)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);
            GL.FrontFace(FrontFaceDirection.Ccw);

            if (state.RenderWireframe)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }


            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 lookat = cam == null ? Matrix4.LookAt(0, 10, 5, 0, 0, 0, 0, 1, 0) : cam.GetView();

            GL.LoadMatrix(ref lookat);
            /*
            float tmp = _sceneMax.X - _sceneMin.X;
            tmp = Math.Max(_sceneMax.Y - _sceneMin.Y, tmp);
            tmp = Math.Max(_sceneMax.Z - _sceneMin.Z, tmp);
            tmp = 1.0f / tmp;
            GL.Scale(tmp * 2, tmp * 2, tmp * 2);

            GL.Translate(-_sceneCenter); */

            if (_displayList == 0)
            {
                _displayList = GL.GenLists(1);
                GL.NewList(_displayList, ListMode.Compile);
                RecursiveRender(_owner.Raw.RootNode, ref lookat);
                GL.EndList();
            }

            GL.CallList(_displayList);

            // always switch back to FILL
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
        }


        private void RecursiveRender(Node node, ref Matrix4 view)
        {
            Matrix4 m = Matrix4.Identity;
            RecursiveRender(node, ref m, ref view);
        }


        private void RecursiveRender(Node node, ref Matrix4 parentTransform, ref Matrix4 view)
        {
            Matrix4 m = AssimpToOpenTk.FromMatrix(node.Transform);
            m.Transpose();

            // keep using the opengl matrix stack to be compatible with displists
            GL.PushMatrix();
            GL.MultMatrix(ref m);

            if (node.HasMeshes)
            {
                foreach (int index in node.MeshIndices)
                {
                    Mesh mesh = _owner.Raw.Meshes[index];
                    _owner.MaterialMapper.ApplyMaterial(_owner.Raw.Materials[mesh.MaterialIndex]);

                    if (mesh.HasNormals)
                    {
                        GL.Enable(EnableCap.Lighting);
                    }
                    else
                    {
                        GL.Disable(EnableCap.Lighting);
                    }

                    bool hasColors = mesh.HasVertexColors(0);
                    if (hasColors)
                    {
                        GL.Enable(EnableCap.ColorMaterial);
                    }
                    else
                    {
                        GL.Disable(EnableCap.ColorMaterial);
                    }

                    bool hasTexCoords = mesh.HasTextureCoords(0);

                    foreach (Face face in mesh.Faces)
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
                        for (int i = 0; i < face.IndexCount; i++)
                        {
                            uint indice = face.Indices[i];
                            if (hasColors)
                            {
                                Color4 vertColor = AssimpToOpenTk.FromColor(mesh.GetVertexColors(0)[indice]);
                            }
                            if (mesh.HasNormals)
                            {
                                Vector3 normal = AssimpToOpenTk.FromVector(mesh.Normals[indice]);
                                GL.Normal3(normal);
                            }
                            if (hasTexCoords)
                            {
                                Vector3 uvw = AssimpToOpenTk.FromVector(mesh.GetTextureCoords(0)[indice]);
                                GL.TexCoord2(uvw.X, 1 - uvw.Y);
                            }
                            Vector3 pos = AssimpToOpenTk.FromVector(mesh.Vertices[indice]);
                            GL.Vertex3(pos);
                        }
                        GL.End();
                    }
                }
            }

            GL.PopMatrix();
            for (int i = 0; i < node.ChildCount; i++)
            {
                RecursiveRender(node.Children[i], ref parentTransform);
            }
        }
    }
}
