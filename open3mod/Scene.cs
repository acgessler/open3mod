using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Represents a 3D scene/asset loaded through assimp.
    /// 
    /// Basically, this class contains the aiScene plus some auxiliary structures
    /// for drawing. Since assimp is the only source for model data and this is
    /// only a viewer, we ignore the recommendation of the assimp docs and use
    /// its data structures (almost) directly for rendering.
    /// </summary>
    public class Scene : IDisposable
    {
        private readonly Assimp.Scene _raw;
        private Vector3 _sceneCenter;
        private Vector3 _sceneMin;
        private Vector3 _sceneMax;
        private int _displayList;
        private LogStore _logStore;

        /// <summary>
        /// Obtain the "raw" scene data as imported by Assimp
        /// </summary>
        public Assimp.Scene Raw
        {
            get { return _raw; }
        }

        public Vector3 SceneCenter
        {
            get { return _sceneCenter; }
        }

        public LogStore LogStore
        {
            get { return _logStore; }
        }


        /// <summary>
        /// Construct a scene given a file name, throw if loading fails
        /// </summary>
        /// <param name="file">File name to be loaded</param>
        public Scene(string file)
        {
            _logStore = new LogStore();

            using (var imp = new AssimpImporter { VerboseLoggingEnabled = true })
            {
                imp.AttachLogStream((new LogPipe(_logStore)).GetStream());

                // Assimp configuration:

                //  - if no normals are present, generate them using a threshold
                //    angle of 66 degrees.
                imp.SetConfig(new NormalSmoothingAngleConfig(66.0f));


                //  - request lots of post processing steps, the details of which
                //    can be found in the TargetRealTimeMaximumQuality docs.
                _raw = imp.ImportFile(file, PostProcessPreset.TargetRealTimeMaximumQuality);
                if (_raw == null)
                {
                    throw new Exception("failed to read file: " + file);
                }
            }

            // compute a bounding box (AABB) for the scene we just loaded
            ComputeBoundingBox();
        } 
     


        public void Update(double delta)
        {
            
        }


        public void Render(UiState state)
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
            Matrix4 lookat = Matrix4.LookAt(0, 10, 5, 0, 0, 0, 0, 1, 0);
            GL.LoadMatrix(ref lookat);

            float tmp = _sceneMax.X - _sceneMin.X;
            tmp = Math.Max(_sceneMax.Y - _sceneMin.Y, tmp);
            tmp = Math.Max(_sceneMax.Z - _sceneMin.Z, tmp);
            tmp = 1.0f / tmp;
            GL.Scale(tmp * 2, tmp * 2, tmp * 2);

            GL.Translate(-_sceneCenter);

            if (_displayList == 0)
            {
                _displayList = GL.GenLists(1);
                GL.NewList(_displayList, ListMode.Compile);
                RecursiveRender(_raw.RootNode);
                GL.EndList();
            }

            GL.CallList(_displayList);

            // always switch back to FILL
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
        }


        private void RecursiveRender(Node node)
        {
            Matrix4 m = AssimpToOpenTk.FromMatrix(node.Transform);
            m.Transpose();
            GL.PushMatrix();
            GL.MultMatrix(ref m);

            if (node.HasMeshes)
            {
                foreach (int index in node.MeshIndices)
                {
                    Mesh mesh = _raw.Meshes[index];
                    ApplyMaterial(_raw.Materials[mesh.MaterialIndex]);

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

            for (int i = 0; i < node.ChildCount; i++)
            {
                RecursiveRender(node.Children[i]);
            }
        }


        private void ApplyMaterial(Material mat)
        {
 
            if (mat.GetTextureCount(TextureType.Diffuse) > 0)
            {
                TextureSlot tex = mat.GetTexture(TextureType.Diffuse, 0);
               // LoadTexture(tex.FilePath);
            }

            Color4 color = new Color4(.8f, .8f, .8f, 1.0f);
            if (mat.HasColorDiffuse)
            {
                // color = FromColor(mat.ColorDiffuse);
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




        private void ComputeBoundingBox()
        {
            _sceneMin = new Vector3(1e10f, 1e10f, 1e10f);
            _sceneMax = new Vector3(-1e10f, -1e10f, -1e10f);
            Matrix4 identity = Matrix4.Identity;

            ComputeBoundingBox(_raw.RootNode, ref _sceneMin, ref _sceneMax, ref identity);

            _sceneCenter.X = (_sceneMin.X + _sceneMax.X) / 2.0f;
            _sceneCenter.Y = (_sceneMin.Y + _sceneMax.Y) / 2.0f;
            _sceneCenter.Z = (_sceneMin.Z + _sceneMax.Z) / 2.0f;
        }


        private void ComputeBoundingBox(Node node, ref Vector3 min, ref Vector3 max, ref Matrix4 trafo)
        {
            Matrix4 prev = trafo;
            trafo = Matrix4.Mult(prev, AssimpToOpenTk.FromMatrix(node.Transform));

            if (node.HasMeshes)
            {
                foreach (int index in node.MeshIndices)
                {
                    Mesh mesh = _raw.Meshes[index];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        Vector3 tmp = AssimpToOpenTk.FromVector(mesh.Vertices[i]);
                        Vector3.Transform(ref tmp, ref trafo, out tmp);

                        min.X = Math.Min(min.X, tmp.X);
                        min.Y = Math.Min(min.Y, tmp.Y);
                        min.Z = Math.Min(min.Z, tmp.Z);

                        max.X = Math.Max(max.X, tmp.X);
                        max.Y = Math.Max(max.Y, tmp.Y);
                        max.Z = Math.Max(max.Z, tmp.Z);
                    }
                }
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ComputeBoundingBox(node.Children[i], ref min, ref max, ref trafo);
            }
            trafo = prev;
        }


        public void Dispose()
        {
            //GL.DeleteTexture(_texID);
        }
    }


    /// <summary>
    /// Utility class to generate an assimp logstream to capture the logging
    /// into a LogStore
    /// </summary>
    class LogPipe 
    {
        private readonly LogStore _logStore;

        public LogPipe(LogStore logStore) 
        {
            _logStore = logStore;
            
        }

        public LogStream GetStream()
        {
            return new LogStream(LogStreamCallback);
        }


        private void LogStreamCallback(string msg, IntPtr userdata)
        {
            // Unfortunately, assimp-net does not wrap assimp's native
            // logging interfaces so log streams (which receive
            // pre-formatted messages) are the only way to capture
            // the logging. This means we have to recover the original
            // information (such as log level) from the string contents.

            int start = msg.IndexOf(':');
            if(start == -1)
            {
                // this should not happen but nonetheless check for it
                //Debug.Assert(false);
                return;
            }

            var cat = LogStore.Category.Info;
            if (msg.StartsWith("Error, "))
            {
                cat = LogStore.Category.Error;        
            }
            else if (msg.StartsWith("Debug, "))
            {
                cat = LogStore.Category.Debug;
            }
            else if (msg.StartsWith("Warn, "))
            {
                cat = LogStore.Category.Warn;
            }
            else if (msg.StartsWith("Info, "))
            {
                cat = LogStore.Category.Info;
            }
            else
            {
                // this should not happen but nonetheless check for it
                //Debug.Assert(false);
                return;
            }

            _logStore.Add(cat, msg.Substring(start + 1), 0.0);
        }
    }
}
