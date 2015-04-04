using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using Assimp;
using Assimp.Unmanaged;
using OpenTK;
using KDTree;

namespace open3mod
{
    public class EditVertex
    {
        public EditFace Face { get; internal set; }

        /// <summary>
        /// Adjacent vertices have the same position, but possibly differ in other
        /// vertex components (i.e. normals).
        /// 
        /// This is an equivalence relation (thus reflexive!, i.e. A vertex
        /// is contained in its own adjacency list).
        /// </summary>
        public HashSet<EditVertex> AdjacentVertices { get; internal set; }

        /// <summary>
        /// Any channel is only copied to output meshes if the corresponding
        /// data field is present in *all* vertices in the mesh.
        /// </summary>
        public Vector3D Position { get; set; }
        public Vector3D? Normal { get; set; }
        public Vector3D? Tangent { get; set; }
        public Vector3D? Bitangent { get; set; }

        public static int MaxTexcoordChannels = AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS;
        public static int MaxColorChannels = AiDefines.AI_MAX_NUMBER_OF_COLOR_SETS;

        public Vector3D?[] TexCoord { get; set; }
        public Color4D?[] Color { get; set; }
        // TODO(acgessler): Bones.

        public EditVertex(Mesh mesh, int index)
        {
            TexCoord = new Vector3D?[MaxTexcoordChannels];
            Color = new Color4D?[MaxColorChannels];
            Position = mesh.Vertices[index];
            AdjacentVertices = new HashSet<EditVertex>();

            if (mesh.HasNormals)
            {
                Normal = mesh.Normals[index];
            }
            if (mesh.HasTangentBasis)
            {
                Tangent = mesh.Tangents[index];
                Bitangent = mesh.BiTangents[index];
            }
            for (int i = 0; i < Math.Min(MaxTexcoordChannels, mesh.TextureCoordinateChannelCount); ++i)
            {
                TexCoord[i] = mesh.TextureCoordinateChannels[i][index];
            }
            for (int i = 0; i < Math.Min(MaxColorChannels, mesh.VertexColorChannelCount); ++i)
            {
                Color[i] = mesh.VertexColorChannels[i][index];
            }
        }

        private const float MergeVectorEpsilon = 1e-5f;

        /// <summary>
        /// Check if a vertex can be merged with another vertex.
        /// 
        /// This is *not* Equals(). We want default Equals()/GetHashCode() behaviour
        /// to keep EditVertex instances distinct in dictionaries and hash sets.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CanBeMergedWith(EditVertex other)
        {
            return (Position - other.Position).LengthSquared() < MergeVectorEpsilon &&
                   AreNullableVectorsApproxEqual(Normal, other.Normal) &&
                   AreNullableVectorsApproxEqual(Tangent, other.Tangent) &&
                   AreNullableVectorsApproxEqual(Bitangent, other.Bitangent) &&
                   TexCoord.Zip(other.TexCoord, AreNullableVectorsApproxEqual).All(_ => _) &&
                   Color.Zip(other.Color, AreNullableColorsApproxEqual).All(_ => _);
        }

        private static bool AreNullableVectorsApproxEqual(Vector3D? a, Vector3D? b)
        {
            return a.HasValue == b.HasValue && (!a.HasValue ||
                  (a.Value - b.Value).LengthSquared() < MergeVectorEpsilon);
        }

        private static bool AreNullableColorsApproxEqual(Color4D? a, Color4D? b)
        {
            if (a.HasValue != b.HasValue)
            {
                return false;
            }
            if (!a.HasValue)
            {
                return true;
            }             
            var diff = a.Value - b.Value;
            return Math.Abs(diff.R) + Math.Abs(diff.G) + Math.Abs(diff.B) + Math.Abs(diff.A) < MergeVectorEpsilon;
        }
    }

    public class EditFace
    {
        public List<EditVertex> Vertices { set; get; }

        public EditFace()
        {
            Vertices = new List<EditVertex>();
        }

        public void AddVertex(EditVertex vertex)
        {
            Vertices.Add(vertex);
            vertex.Face = this;
        }
    }

    /// <summary>
    /// Editable mesh that stores rich adjacency information for vertices and faces.
    /// </summary>
    public class EditMesh
    {
        public List<EditFace> Faces { get; private set; }

        /// <summary>
        /// Non-unique vertices. Each face has its own copy of each vertex.
        /// Vertices are merged in ApplyToMesh().
        /// </summary>
        public List<EditVertex> Vertices { get; private set; }


        /// <summary>
        /// Construct an EditMesh from a given Mesh.
        /// 
        /// The EditMesh is independent and retains no reference to the original
        /// data, use ApplyToMesh() to propagate changes back.
        /// </summary>
        /// <param name="mesh"></param>
        public EditMesh(Mesh mesh)
        {
            Faces = new List<EditFace>(mesh.FaceCount);
            Vertices = new List<EditVertex>(mesh.FaceCount * 3);
            for (int i = 0; i < mesh.FaceCount; ++i)
            {
                var srcFace = mesh.Faces[i];
                var destFace = new EditFace();
                for (int j = 0; j < srcFace.IndexCount; ++j)
                {
                    var vert = new EditVertex(mesh, srcFace.Indices[j]);
                    Vertices.Add(vert);
                    destFace.Vertices.Add(vert);
                }
                Faces.Add(destFace);
            }
            ComputeAdjacentVertices();
        }


        /// <summary>
        /// Apply the EditMesh to a given Mesh.
        /// 
        /// Mesh name and material index are unchanged.
        /// </summary>
        /// <param name="mesh"></param>
        public void ApplyToMesh(Mesh mesh)
        {
            // Find subsets of vertices that are identical in all vertex components
            // and can be merged. To do this fast, we exploit the adjacency list in
            // the vertices. Each merge-able set must be a subset of already adjacent
            // vertices.
            Dictionary<EditVertex, int> indices = new Dictionary<EditVertex, int>();
            int cursor = 0;
            foreach (EditVertex vert in Vertices)
            {
                if (indices.ContainsKey(vert))
                {
                    continue;
                }
                foreach (EditVertex adjacentVert in vert.AdjacentVertices)
                {
                    if (adjacentVert == vert || vert.CanBeMergedWith(adjacentVert))
                    {
                        indices[adjacentVert] = cursor;
                    }
                }
                ++cursor;
            }

            // Allocate output channels only if all input vertices set a component.
            bool hasNormals = Vertices.All(vert => vert.Normal.HasValue);
            bool hasTangentSpace = Vertices.All(vert => vert.Tangent.HasValue && vert.Bitangent.HasValue);
            bool[] hasTexCoords = Enumerable.Range(0, EditVertex.MaxTexcoordChannels).
                Select(i => Vertices.All(vert => vert.TexCoord[i].HasValue)).ToArray();
            bool[] hasColors = Enumerable.Range(0, EditVertex.MaxColorChannels).
                Select(i => Vertices.All(vert => vert.Color[i].HasValue)).ToArray();

            mesh.Vertices.Clear();
            mesh.Normals.Clear();
            mesh.Tangents.Clear();
            mesh.BiTangents.Clear();

            mesh.Vertices.Capacity = indices.Count;         
            if (hasNormals)
            {
                mesh.Normals.Capacity = indices.Count;
            }
            if (hasTangentSpace)
            {
                mesh.Tangents.Capacity = indices.Count;
                mesh.BiTangents.Capacity = indices.Count;
            }
            for (var i = 0; i < EditVertex.MaxTexcoordChannels; ++i)
            {
                mesh.TextureCoordinateChannels[i].Clear();
                mesh.TextureCoordinateChannels[i].Capacity = indices.Count;
            }
            for (var i = 0; i < EditVertex.MaxColorChannels; ++i)
            {
                mesh.VertexColorChannels[i].Clear();
                mesh.VertexColorChannels[i].Capacity = indices.Count;
            }
           
            // Store unique indices.
            var reverseIndices = indices.ToDictionary(x => x.Value, x => x.Key);
            for (var i = 0; i < indices.Count; ++i)
            {
                var srcVert = reverseIndices[i];
                mesh.Vertices.Add(srcVert.Position);
                if (hasNormals)
                {

                    mesh.Normals.Add(srcVert.Normal.Value);
                }
                if (hasTangentSpace)
                {
                    mesh.Tangents.Add(srcVert.Tangent.Value);
                    mesh.BiTangents.Add(srcVert.Bitangent.Value);
                }
                for (var c = 0; c < EditVertex.MaxTexcoordChannels; ++c)
                {
                    if (hasTexCoords[c])
                    {
                        mesh.TextureCoordinateChannels[c].Add(srcVert.TexCoord[c].Value);
                    }
                }
                for (var c = 0; c < EditVertex.MaxColorChannels; ++c)
                {
                    if (hasColors[c])
                    {
                        mesh.VertexColorChannels[c].Add(srcVert.Color[c].Value);
                    }
                }              
            }

            mesh.Faces.Clear();
            mesh.Faces.Capacity = Faces.Count;
            foreach (var srcFace in Faces)
            {
                var face = new Face();
                foreach (var srcVert in srcFace.Vertices)
                {
                    face.Indices.Add(indices[srcVert]);
                }
                mesh.Faces.Add(face);
            }
        }


        private void ComputeAdjacentVertices()
        {
            // Put all vertices into a KDTree.
            KDTree<EditVertex> tree = new KDTree<EditVertex>(3);
            foreach (var face in Faces)
            {
                foreach (var vert in face.Vertices)
                {
                    var pos = vert.Position;
                    tree.AddPoint(new double[] {pos.X, pos.Y, pos.Z}, vert);
                }
            }
            // For each vertices, retrieve adjacent/identical vertices by looking
            // up neighbors within a very small (epsilon'ish) radius.
            foreach (var face in Faces)
            {
                foreach (var vert in face.Vertices)
                {
                    var pos = vert.Position;
                    var neighbors = tree.NearestNeighbors(new double[] {pos.X, pos.Y, pos.Z},
                        new SquareEuclideanDistanceFunction(),
                        int.MaxValue, 1e-5f);
                    foreach (var neighbor in neighbors)
                    {
                        vert.AdjacentVertices.Add(neighbor);
                    }
                }
            }
            // Vertices being adjacent should be a transitive relation, yet this is
            // not guaranteed if implemented through a search radius.
            HashSet<EditVertex> seen = new HashSet<EditVertex>();
            foreach (var face in Faces)
            {
                foreach (var vert in face.Vertices)
                {
                    if (seen.Contains(vert))
                    {
                        continue;
                    }
                    foreach (var adjacentVert in vert.AdjacentVertices)
                    {
                        vert.AdjacentVertices.UnionWith(vert.AdjacentVertices);                
                    }
                    foreach (var adjacentVert in vert.AdjacentVertices)
                    {
                        adjacentVert.AdjacentVertices = vert.AdjacentVertices;
                        seen.Add(adjacentVert);
                    }
                }
            }
        }
    }
}
