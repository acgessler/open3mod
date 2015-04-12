///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [CpuSkinningEvaluator.cs]
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

using System.Collections.Generic;
using System.Diagnostics;
using Assimp;
using OpenTK;

namespace open3mod
{
    /// <summary>
    /// Evaluates bone deformations on the CPU as opposed to a shader-based implementation.
    /// 
    /// This gets used in the legacy renderer (SceneRendererClassicGl).
    /// </summary>
    public class CpuSkinningEvaluator
    {
        private readonly Scene _owner;
        private readonly Dictionary<Mesh, CachedMeshData> _cache;
        
        private sealed class CachedMeshData
        {
            private readonly Scene _scene;
            private readonly Mesh _source;
            private readonly Vector3[] _cachedPositions;
            private readonly Vector3[] _cachedNormals;
            private bool _dirty = true;
            private readonly BoneByVertexMap _boneMap;
            private Node _lastNode;


            public CachedMeshData(Scene scene, Mesh source)
            {
                Debug.Assert(source.HasBones, "source mesh needs to have bones");

                _scene = scene;
                _source = source;

                _cachedPositions = new Vector3[source.VertexCount];
                _cachedNormals = new Vector3[source.VertexCount];
            
                _boneMap = new BoneByVertexMap(source);
            }


            public void Update()
            {
                _dirty = true;
            }


            public void GetTransformedVertexPosition(Node node, uint vertexIndex, out Vector3 pos)
            {
                // note: the scenario in which a skinned mesh is referenced by multiple nodes
                // is not currently implemented properly. It works, but it prevents caching.
                if (node != _lastNode)
                {
                    _lastNode = node;
                    _dirty = true;
                }
                if(_dirty)
                {
                   Cache();        
                }
                Debug.Assert(!_dirty);

                Debug.Assert(vertexIndex < _cachedPositions.Length);
                pos = _cachedPositions[vertexIndex];
            }


            public void GetTransformedVertexNormal(Node node, uint vertexIndex, out Vector3 nor)
            {
                // note: the scenario in which a skinned mesh is referenced by multiple nodes
                // is not currently implemented properly. It works, but it prevents caching.
                if (node != _lastNode)
                {
                    _lastNode = node;
                    _dirty = true;
                }
                if (_dirty)
                {
                    Cache();
                }
                Debug.Assert(!_dirty);

                Debug.Assert(vertexIndex < _cachedNormals.Length);
                nor = _cachedNormals[vertexIndex];
            }


            /// <summary>
            /// Internal method to (re-)cache all transformed vertex positions and normals
            /// </summary>
            private void Cache()
            {
                var boneMatrices = _scene.SceneAnimator.GetBoneMatricesForMesh(_lastNode, _source);
                // update entire mesh
                for (int i = 0; i < _cachedPositions.Length; ++i)
                {
                    var v = AssimpToOpenTk.FromVector(_source.Vertices[i]);
                    EvaluateBoneInfluences(ref v, (uint)i, boneMatrices, out _cachedPositions[i]);
                }
                for (int i = 0; i < _cachedNormals.Length; ++i)
                {
                    var n = AssimpToOpenTk.FromVector(_source.Normals[i]);
                    EvaluateBoneInfluences(ref n, (uint)i, boneMatrices, out _cachedNormals[i], true);
                }

                _dirty = false;
            }


            /// <summary>
            /// Evaluate all bone influences on a single vertex
            /// </summary>
            /// <param name="pos">Untransformed vertex position</param>
            /// <param name="vertexIndex">Index of the vertex in the mesh</param>
            /// <param name="boneMatrices">Bone matrices evaluated for the current mesh, node</param>
            /// <param name="transformedPosOut">Receives the transformed vertex</param>
            /// <param name="isDirectionVector">Specifies whether the input parameter is a direction vector
            ///    instead of a 3D position. Vector3.TransformNormal() is used in this case.</param>
            private void EvaluateBoneInfluences(ref Vector3 pos, uint vertexIndex, Matrix4[] boneMatrices,
                out Vector3 transformedPosOut,
                bool isDirectionVector = false)
            {
                uint offset;
                uint count;
                _boneMap.GetOffsetAndCountForVertex(vertexIndex, out offset, out count);

                var transformedPos = Vector3.Zero;

                var bones = _boneMap.BonesByVertex;
                for (var k = 0; k < count; ++k, ++offset)
                {
                    var boneWeightTuple = bones[offset];
                    Debug.Assert(boneWeightTuple.Item1 < boneMatrices.Length);

                    Vector3 tmp;
                    if (isDirectionVector)
                    {
                        Vector3.TransformNormal(ref pos, ref boneMatrices[boneWeightTuple.Item1], out tmp);
                    }
                    else
                    {
                        Vector3.Transform(ref pos, ref boneMatrices[boneWeightTuple.Item1], out tmp);
                    }
                    transformedPos += tmp * boneWeightTuple.Item2;
                }

                transformedPosOut = transformedPos;
            }

            /// <summary>
            /// Checks if the cache entry is compatible with the given mesh.
            /// 
            /// If the size of the mesh data changes, the cache no longer works for it
            /// and should be re-generated.
            /// 
            /// TODO(acgessler): Devise a clean way of notifying dependents of scene changes.
            /// </summary>
            /// <param name="mesh"></param>
            /// <returns></returns>
            public bool CompatibleWith(Mesh mesh)
            {
                return _cachedPositions.Length == mesh.Vertices.Count;
            }
        }


        /// <summary>
        /// Constructs a CpuSkinningEvaluator for a given scene.
        /// </summary>
        /// <param name="owner"></param>
        public CpuSkinningEvaluator(Scene owner)
        {
            _owner = owner;
            _cache = new Dictionary<Mesh, CachedMeshData>();
            for (var i = 0; i < owner.Raw.Meshes.Count; ++i)
            {
                var mesh = owner.Raw.Meshes[i];
                if (!mesh.HasBones)
                {
                    continue;
                }
                _cache[mesh] = new CachedMeshData(owner, mesh);
            }
        }


        /// <summary>
        /// Informs the skinner that a new frame has begun, which means that
        /// any cached data may have become obsolete.
        /// </summary>
        public void Update()
        {
            foreach(var v in _cache)
            {
                v.Value.Update();
            }
        }


        /// <summary>
        /// Get the cache entry corresponding to a mesh.
        /// 
        /// Creates an entry if it does not exist yet.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        private CachedMeshData GetEntry(Mesh mesh)
        {
            CachedMeshData entry;
            if (_cache.TryGetValue(mesh, out entry) && entry.CompatibleWith(mesh))
            {
                return entry;
            }
            return _cache[mesh] = new CachedMeshData(_owner, mesh);
        }


        /// <summary>
        /// Get a transformed vertex position for a given mesh + vertex index. The
        /// results of this method are cached between calls in the same frame.
        /// </summary>
        /// <param name="node">Node that holds the mesh</param>
        /// <param name="meshIndex"></param>
        /// <param name="vertexIndex"></param>
        /// <param name="pos"></param>
        public void GetTransformedVertexPosition(Node node, Mesh mesh, uint vertexIndex, out Vector3 pos)
        {
            GetEntry(mesh).GetTransformedVertexPosition(node, vertexIndex, out pos);
        }

     
        /// <summary>
        /// Get a transformed vertex normal for a given mesh + vertex index. The
        /// results of this method are cached between calls in the same frame.
        /// </summary>
        /// <param name="node">Node that holds the mesh</param>
        /// <param name="meshIndex"></param>
        /// <param name="vertexIndex"></param>
        /// <param name="nor"></param>
        public void GetTransformedVertexNormal(Node node, Mesh mesh, uint vertexIndex, out Vector3 nor)
        {
            GetEntry(mesh).GetTransformedVertexNormal(node, vertexIndex, out nor);
        }    
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 