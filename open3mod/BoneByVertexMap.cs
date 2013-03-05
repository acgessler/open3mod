using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Caches vertex->bone (1:n) assignments for a mesh. This information is derived from
    /// the bone->vertex (1:n) assignments loaded by assimp.
    /// 
    /// The class can be used with meshes that have no bones at all.
    /// </summary>
    public class BoneByVertexMap
    {
        private readonly Mesh _mesh;

        private readonly uint[] _countBones;
        private readonly Tuple<int, float>[] _bonesByVertex;
        private readonly uint[] _offsets;


        /// <summary>
        /// Array of per-vertex bone assignments. Use GetOffsetAndCountForVertex to 
        /// access the entries for a single vertex.
        /// </summary>
        public Tuple<int, float>[] BonesByVertex
        {
            get { return _bonesByVertex; }
        }


        /// <summary>
        /// Get the number of bone influences for a vertex and the offset in the 
        /// BonesByVertex array where they are stored.
        /// </summary>
        /// <param name="vertex">Vertex index</param>
        /// <param name="offset">Receives the index of the first bone influence for the vertex</param>
        /// <param name="count">Receives the number of bone influenves for the vertex</param>
        public void GetOffsetAndCountForVertex(uint vertex, out uint offset, out uint count)
        {
            offset = _offsets[vertex];
            count = _countBones[vertex];
        }


        internal BoneByVertexMap(Mesh mesh)
        {
            Debug.Assert(mesh != null);
            _mesh = mesh;

            _offsets = new uint[mesh.VertexCount];
            _countBones = new uint[mesh.VertexCount];

            if (_mesh.BoneCount == 0)
            {
                _bonesByVertex = new Tuple<int, float>[0];           
                return;
            }

            // get per-vertex bone influence counts
            var countWeights = 0;
            for (var i = 0; i < mesh.BoneCount; ++i)
            {
                var bone = mesh.Bones[i];
                countWeights += bone.VertexWeightCount;
                for (int j = 0; j < bone.VertexWeightCount; ++j)
                {
                    var weight = bone.VertexWeights[j];
                    ++_countBones[weight.VertexID];
                }
            }

            _bonesByVertex = new Tuple<int, float>[countWeights];

            // generate offset table
            uint sum = 0;
            for (var i = 0; i < _mesh.VertexCount; ++i)
            {
                _offsets[i] = sum;
                sum += _countBones[i];
            }

            // populate vertex-to-bone table, using the offset table
            // to keep track of how many bones have already been
            // written for a vertex.
            for (var i = 0; i < mesh.BoneCount; ++i)
            {
                var bone = mesh.Bones[i];
                countWeights += bone.VertexWeightCount;
                for (int j = 0; j < bone.VertexWeightCount; ++j)
                {
                    var weight = bone.VertexWeights[j];
                    BonesByVertex[_offsets[weight.VertexID]++] = new Tuple<int, float>(i, weight.Weight);
                }
            }

            // undo previous changes to the offset table 
            for (var i = 0; i < _mesh.VertexCount; ++i)
            {
                _offsets[i] -= _countBones[i];
            }

            Debug.Assert(_offsets[0] == 0);
        }
    }
}
