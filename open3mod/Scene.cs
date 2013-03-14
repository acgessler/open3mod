///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [Scene.cs]
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Assimp;
using Assimp.Configs;
using OpenTK;


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
    public sealed class Scene : IDisposable
    {
        private readonly string _file;
        private readonly string _baseDir;
       
        private readonly Assimp.Scene _raw;
        private Vector3 _sceneCenter;
        private Vector3 _sceneMin;
        private Vector3 _sceneMax;       
        private readonly LogStore _logStore;

        private readonly TextureSet _textureSet;

        private readonly MaterialMapper _mapper;
        private readonly ISceneRenderer _renderer;


        /// <summary>
        /// Source file name with full path
        /// </summary>
        public string File
        {
            get { return _file; }
        }

        /// <summary>
        /// Folder in which the source file resides, the "working directory"
        /// for the scene if you want.
        /// </summary>
        public string BaseDir
        {
            get { return _baseDir; }
        }


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

        public MaterialMapper MaterialMapper
        {
            get { return _mapper; }
        }

        public SceneAnimator SceneAnimator
        {
            get { return _animator; }
        }

        public HashSet<Node> VisibleNodes
        {
            get { return _nodesToShow; }
        }

        public TextureSet TextureSet
        {
            get { return _textureSet; }
        }

        private bool _nodesToShowChanged = true;
        private HashSet<Node> _nodesToShow;
        private bool _texturesChanged = false;
        private SceneAnimator _animator;
        private double _accumulatedTimeDelta;


        /// <summary>
        /// Construct a scene given a file name, throw if loading fails
        /// </summary>
        /// <param name="file">File name to be loaded</param>
        public Scene(string file)
        {
            _file = file;
            _baseDir = Path.GetDirectoryName(file);
  
            _logStore = new LogStore();
             

            try
            {
                using (var imp = new AssimpImporter {VerboseLoggingEnabled = true})
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
            }
            catch(AssimpException ex)
            {
                throw new Exception("failed to read file: " + file + " (" + ex.Message + ")");
            }

            _mapper = new MaterialMapper(this); 
            _animator = new SceneAnimator(this);
            _textureSet = new TextureSet(BaseDir);
            LoadTextures();

            // compute a bounding box (AABB) for the scene we just loaded
            ComputeBoundingBox();

            _renderer = new SceneRendererClassicGl(this, _sceneMin, _sceneMax);
        }


        /// <summary>
        /// Set the nodes of the scene that are visible. Visibility is not
        /// automatically inherited by children, so all children need to
        /// be included in the filter list if they should be visible.
        /// </summary>
        /// <param name="filter">Node list or null to disable filtering</param>
        public void SetVisibleNodes(HashSet<Node> filter)
        {
            _nodesToShow = filter;
            _nodesToShowChanged = true;
        }


        /// <summary>
        /// Call once per frame to do non-rendering jobs such as updating 
        /// animations.
        /// </summary>
        /// <param name="delta">Real-world time delta in seconds</param>
        /// <param name="silent">True if the scene is in background and won't
        ///    be rendered this frame. It may then be possible to implement
        ///    a cheaper update.</param>
        public void Update(double delta, bool silent = false)
        {
            if(silent)
            {
                _accumulatedTimeDelta += delta;
                return;
            }
            _animator.Update(delta + _accumulatedTimeDelta);
            _accumulatedTimeDelta = 0.0;

            _renderer.Update(delta);
        }


        /// <summary>
        /// Call once per frame to render the scene to the current viewport.
        /// </summary>
        /// <param name="delta">Real-world time delta in seconds</param>
        public void Render(UiState state, ICameraController cam)
        {
            RenderFlags flags = 0;
          
            if (state.ShowNormals)
            {
                flags |= RenderFlags.ShowNormals;
            }
            if (state.ShowBBs)
            {
                flags |= RenderFlags.ShowBoundingBoxes;
            }
            if (state.ShowSkeleton)
            {
                flags |= RenderFlags.ShowSkeleton;
            }
            if (state.RenderLit)
            {
                flags |= RenderFlags.Shaded;
            }
            if (state.RenderTextured)
            {
                flags |= RenderFlags.Textured;
            }
            if (state.RenderWireframe)
            {
                flags |= RenderFlags.Wireframe;
            }

            _renderer.Render(cam, _nodesToShow, _nodesToShowChanged, _texturesChanged, flags);

            _texturesChanged = false;
            _nodesToShowChanged = false;
        }


        private void LoadTextures()
        {
            var materials = _raw.Materials;
            foreach (var mat in materials)
            {
                var textures = mat.GetAllTextures();
                foreach (var tex in textures)
                {
                    TextureSet.Add(tex.FilePath);
                }
            }

            TextureSet.AddCallback((name, tex) => _texturesChanged = true);
        }


        private void ComputeBoundingBox()
        {
            _sceneMin = new Vector3(1e10f, 1e10f, 1e10f);
            _sceneMax = new Vector3(-1e10f, -1e10f, -1e10f);
            Matrix4 identity = Matrix4.Identity;

            ComputeBoundingBox(_raw.RootNode, ref _sceneMin, ref _sceneMax, ref identity);
            _sceneCenter = (_sceneMin + _sceneMax) / 2.0f;
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
            TextureSet.Dispose();
        }
    }


    /// <summary>
    /// Utility class to generate an assimp logstream to capture the logging
    /// into a LogStore
    /// </summary>
    class LogPipe 
    {
        private readonly LogStore _logStore;
        private Stopwatch _timer;

        public LogPipe(LogStore logStore) 
        {
            _logStore = logStore;
            
        }

        public LogStream GetStream()
        {
            return new LogStream(LogStreamCallback);
        }


        private void LogStreamCallback(string msg, string userdata)
        {
            // Start timing with the very first logging messages. This
            // is relatively reliable because assimp writes a log header
            // as soon as it starts processing a file.
            if (_timer == null)
            {
                _timer = new Stopwatch();
                _timer.Start(); 
              
            }

            long millis = _timer.ElapsedMilliseconds;

            // Unfortunately, assimp-net does not wrap assimp's native
            // logging interfaces so log streams (which receive
            // pre-formatted messages) are the only way to capture
            // the logging. This means we have to recover the original
            // information (such as log level and the thread/job id) 
            // from the string contents.



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

            int startThread = msg.IndexOf('T');
            if (startThread == -1 || startThread >= start)
            {
                // this should not happen but nonetheless check for it
                //Debug.Assert(false);
                return;
            }

            int threadId = 0;
            int.TryParse(msg.Substring(startThread + 1, start - startThread - 1), out threadId);

            _logStore.Add(cat, msg.Substring(start + 1), millis, threadId);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 