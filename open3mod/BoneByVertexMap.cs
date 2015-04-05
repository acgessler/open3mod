///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [BoneByVertexMap.cs]
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

using System.Diagnostics;
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

        // Back ported from a .net 4.5 Tuple<int, float>
        public struct IndexWeightTuple
        {
            public IndexWeightTuple(int index, float weight)
            {
                Item1 = index;
                Item2 = weight;
            }

            public int Item1;
            public float Item2;
        }

        private readonly uint[] _countBones;
        private readonly IndexWeightTuple[] _bonesByVertex;
        private readonly uint[] _offsets;


        /// <summary>
        /// Array of per-vertex bone assignments. Use GetOffsetAndCountForVertex to 
        /// access the entries for a single vertex.
        /// </summary>
        public IndexWeightTuple[] BonesByVertex
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
                _bonesByVertex = new IndexWeightTuple[0];           
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

            _bonesByVertex = new IndexWeightTuple[countWeights];

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
                    BonesByVertex[_offsets[weight.VertexID]++] = new IndexWeightTuple(i, weight.Weight);
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

/* vi: set shiftwidth=4 tabstop=4: */ 