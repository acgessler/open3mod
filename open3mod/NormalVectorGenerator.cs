///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [NormalVectorGenerator.cs]
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assimp;
using OpenTK;

namespace open3mod
{
    public class NormalVectorGenerator
    {
        private readonly Mesh _mesh;
        private readonly EditMesh _editMesh;
        private readonly Dictionary<EditFace, Vector3D> _faceNormals = new Dictionary<EditFace, Vector3D>(); 

        public NormalVectorGenerator(Mesh mesh)
        {
            Debug.Assert(!mesh.PrimitiveType.HasFlag(PrimitiveType.Polygon));
            _mesh = mesh;
            lock (_mesh)
            {
                _editMesh = new EditMesh(_mesh);
            }
        }

        /// <summary>
        /// Compute normal vectors with the given threshold for smoothing.
        /// 
        /// Updates the original |mesh|. This can be called multiple times.
        /// </summary>
        /// <param name="thresholdAngleInDegrees"></param>
        public void Compute(float thresholdAngleInDegrees)
        {
            CalculateFaceNormals();
            if (thresholdAngleInDegrees > 0.0f)
            {
                SmoothNormals(thresholdAngleInDegrees);
            }
            lock (_mesh)
            {
                _editMesh.ApplyToMesh(_mesh);
            }
        }

        private void CalculateFaceNormals()
        {
            _faceNormals.Clear();
            foreach (var face in _editMesh.Faces)
            {
                Vector3D faceNormal = new Vector3D();
                if (face.Vertices.Count ==  3)
                {
                    Vector3D v0 = face.Vertices[0].Position;
                    Vector3D v1 = face.Vertices[1].Position;
                    Vector3D v2 = face.Vertices[2].Position;
                    faceNormal = Vector3D.Cross(v1-v0,v2-v1);
                }
                if (faceNormal.LengthSquared() > 0.0f)
                {
                    faceNormal.Normalize();
                }
                foreach (var vert in face.Vertices)
                {
                    vert.Normal = faceNormal;
                }
                _faceNormals[face] = faceNormal;
            }
        }

        private void SmoothNormals(float thresholdAngleInDegrees)
        {
            float thresholdAngleInRadians = (float)(thresholdAngleInDegrees*Math.PI/180.0);
            float cosThresholdAngle = (float)Math.Cos(thresholdAngleInRadians);
            foreach (var vert in _editMesh.Vertices)
            {              
                var faceNormal = _faceNormals[vert.Face];
                vert.Normal = faceNormal;
                foreach (var adjacentVert in vert.AdjacentVertices)
                {
                    if (vert == adjacentVert)
                    {
                        continue;
                    }
                    var adjacentFace = adjacentVert.Face;
                    var adjacentFaceNormal = _faceNormals[adjacentFace];
                    if (Vector3D.Dot(faceNormal, adjacentFaceNormal) >= cosThresholdAngle)
                    {
                        vert.Normal += adjacentFaceNormal;
                    }
                }    
                if (vert.Normal.Value.LengthSquared() > 0.0f)
                {
                    var v = vert.Normal.Value;
                    v.Normalize();
                    vert.Normal = v;
                }
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 