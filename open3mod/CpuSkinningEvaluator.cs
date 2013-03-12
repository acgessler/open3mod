///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [CpuSkinningEvaluator.cs]
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
        private readonly CachedMeshData[] _cache;
        
        private sealed class CachedMeshData
        {
            private readonly Scene _scene;
            private readonly Mesh _source;
            private readonly Vector3[] _cachedPositions;
            private readonly Vector3[] _cachedNormals;
            private bool _dirty = true;
            private readonly BoneByVertexMap _boneMap;
            private Node _lastNode;

            private readonly int _sourceIndex;


            public CachedMeshData(Scene scene, Mesh source, int sourceIndex)
            {
                Debug.Assert(source.HasBones, "source mesh needs to have bones");

                _scene = scene;
                _source = source;
                _sourceIndex = sourceIndex;

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
                var boneMatrices = _scene.SceneAnimator.GetBoneMatricesForMesh(_lastNode, _sourceIndex);
                // update entire mesh
                for (uint i = 0; i < _cachedPositions.Length; ++i)
                {
                    var v = AssimpToOpenTk.FromVector(_source.Vertices[i]);
                    EvaluateBoneInfluences(ref v, i, boneMatrices, out _cachedPositions[i]);
                }
                for (uint i = 0; i < _cachedNormals.Length; ++i)
                {
                    var n = AssimpToOpenTk.FromVector(_source.Normals[i]);
                    EvaluateBoneInfluences(ref n, i, boneMatrices, out _cachedNormals[i], true);
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
        }


        /// <summary>
        /// Constructs a CpuSkinningEvaluator for a given scene.
        /// </summary>
        /// <param name="owner"></param>
        public CpuSkinningEvaluator(Scene owner)
        {
            _owner = owner;
            _cache = new CachedMeshData[owner.Raw.MeshCount];
            for (var i = 0; i < _cache.Length; ++i)
            {
                if (!owner.Raw.Meshes[i].HasBones)
                {
                    continue;
                }
                _cache[i] = new CachedMeshData(owner, owner.Raw.Meshes[i], i);
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
                if (v == null)
                {
                    continue;
                }
                v.Update();
            }
        }


        /// <summary>
        /// Get a transformed vertex position for a given mesh + vertex index. The
        /// results of this method are cached between calls in the same frame.
        /// </summary>
        /// <param name="node">Node that holds the mesh</param>
        /// <param name="meshIndex"></param>
        /// <param name="vertexIndex"></param>
        /// <param name="pos"></param>
        public void GetTransformedVertexPosition(Node node, int meshIndex, uint vertexIndex, out Vector3 pos)
        {
            _cache[meshIndex].GetTransformedVertexPosition(node, vertexIndex, out pos);
        }


        /// <summary>
        /// Get a transformed vertex normal for a given mesh + vertex index. The
        /// results of this method are cached between calls in the same frame.
        /// </summary>
        /// <param name="node">Node that holds the mesh</param>
        /// <param name="meshIndex"></param>
        /// <param name="vertexIndex"></param>
        /// <param name="nor"></param>
        public void GetTransformedVertexNormal(Node node, int meshIndex, uint vertexIndex, out Vector3 nor)
        {
            _cache[meshIndex].GetTransformedVertexNormal(node, vertexIndex, out nor);
        }    
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 