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
        /// <summary>
        /// Source file name / path
        /// </summary>
        public string File { get; set; }

        private readonly Assimp.Scene _raw;
        private Vector3 _sceneCenter;
        private Vector3 _sceneMin;
        private Vector3 _sceneMax;       
        private readonly LogStore _logStore;

        private readonly TextureSet _textureSet;

        private readonly MaterialMapper _mapper;
        private readonly ISceneRenderer _renderer;

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

        public HashSet<Node> VisibleNodes
        {
            get { return _nodesToShow; }
        }

        private bool _nodesToShowChanged = true;
        private HashSet<Node> _nodesToShow;

        /// <summary>
        /// Construct a scene given a file name, throw if loading fails
        /// </summary>
        /// <param name="file">File name to be loaded</param>
        public Scene(string file)
        {
            File = file;
  
            _logStore = new LogStore();
            _mapper = new MaterialMapper();
            _renderer = new SceneRendererClassicGl(this);

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

            _textureSet = new TextureSet();
            LoadTextures();

            // compute a bounding box (AABB) for the scene we just loaded
            ComputeBoundingBox();
        }


        private void LoadTextures()
        {
            var materials = _raw.Materials;
            foreach(var mat in materials)
            {
                var textures = mat.GetAllTextures();
                foreach (var tex in textures)
                {
                    _textureSet.Add(tex.FilePath);               
                }
            }
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
     

        public void Update(double delta)
        {
            
        }


        public void Render(UiState state, ICameraController cam)
        {
            _renderer.Render(state, cam, _nodesToShow, _nodesToShowChanged);
            _nodesToShowChanged = false;
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
            _textureSet.Dispose();
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


        private void LogStreamCallback(string msg, IntPtr userdata)
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

            _logStore.Add(cat, msg.Substring(start + 1), millis);
        }
    }
}
