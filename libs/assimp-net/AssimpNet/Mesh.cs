/*
* Copyright (c) 2012-2013 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// A mesh represents geometry with a single material.
    /// </summary>
    public sealed class Mesh {
        private String m_name;
        private PrimitiveType m_primitiveType;
        private int m_materialIndex;
        private int m_vertexCount;
        private Vector3D[] m_vertices;
        private Vector3D[] m_normals;
        private Vector3D[] m_tangents;
        private Vector3D[] m_bitangents;
        private Face[] m_faces;
        private List<Color4D[]> m_colors;
        private List<Vector3D[]> m_texCoords;
        private List<uint> m_texComponentNumber;
        private Bone[] m_bones;
        private MeshAnimationAttachment[] m_meshAttachments;

        /// <summary>
        /// Gets the mesh name. This tends to be used
        /// when formats name nodes and meshes independently,
        /// vertex animations refer to meshes by their names,
        /// or importers split meshes up, each mesh will reference
        /// the same (dummy) name.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the primitive type. This may contain more than one
        /// type unless if <see cref="PostProcessSteps.SortByPrimitiveType"/>
        /// option is not set.
        /// </summary>
        public PrimitiveType PrimitiveType {
            get {
                return m_primitiveType;
            }
        }

        /// <summary>
        /// Gets the index of the material associated with this mesh.
        /// </summary>
        public int MaterialIndex {
            get {
                return m_materialIndex;
            }
        }

        /// <summary>
        /// Gets the number of vertices in this mesh. This is also
        /// the size for all per-vertex data arays.
        /// </summary>
        public int VertexCount {
            get {
                return m_vertexCount;
            }
        }

        /// <summary>
        /// Gets if the mesh has a vertex array. This should always return
        /// true provided no special scene flags are set.
        /// </summary>
        public bool HasVertices {
            get {
                return m_vertices != null;
            }
        }

        /// <summary>
        /// Gets the vertex position array.
        /// </summary>
        public Vector3D[] Vertices {
            get {
                return m_vertices;
            }
        }

        /// <summary>
        /// Gets if the mesh as normals.
        /// </summary>
        public bool HasNormals {
            get {
                return m_normals != null;
            }
        }

        /// <summary>
        /// Gets the vertex normal array.
        /// </summary>
        public Vector3D[] Normals {
            get {
                return m_normals;
            }
        }

        /// <summary>
        /// Gets if the mesh has tangents and bitangents. It is not
        /// possible for one to be without the other.
        /// </summary>
        public bool HasTangentBasis {
            get {
                return VertexCount > 0 && m_tangents != null &&m_bitangents != null;
            }
        }

        /// <summary>
        /// Gets the vertex tangent array.
        /// </summary>
        public Vector3D[] Tangents {
            get {
                return m_tangents;
            }
        }

        /// <summary>
        /// Gets the vertex bitangent array.
        /// </summary>
        public Vector3D[] BiTangents {
            get {
                return m_bitangents;
            }
        }

        /// <summary>
        /// Gets the number of faces contained in the mesh.
        /// </summary>
        public int FaceCount {
            get {
                return (m_faces == null) ? 0 : m_faces.Length;
            }
        }

        /// <summary>
        /// Gets if the mesh contains faces. If no special
        /// scene flags are set, this should always return true.
        /// </summary>
        public bool HasFaces {
            get {
                return m_faces != null;
            }
        }

        /// <summary>
        /// Gets the mesh's faces. Each face will contain indices
        /// to the vertices.
        /// </summary>
        public Face[] Faces {
            get {
                return m_faces;
            }
        }

        /// <summary>
        /// Gets the number of valid vertex color channels contained in the
        /// mesh. This can be a value between zero and the maximum vertex color count.
        /// </summary>
        public int VertexColorChannelCount {
            get {
                return (m_colors == null) ? 0 : m_colors.Count;
            }
        }

        /// <summary>
        /// Gets the number of valid texture coordinate channels contained
        /// in the mesh. This can be a value between zero and the maximum texture coordinate count.
        /// </summary>
        public int TextureCoordsChannelCount {
            get {
                return (m_texCoords == null) ? 0 : m_texCoords.Count;
            }
        }

        /// <summary>
        /// Gets the number of bones that influence this mesh.
        /// </summary>
        public int BoneCount {
            get {
                return (m_bones == null) ? 0 : m_bones.Length;
            }
        }

        /// <summary>
        /// Gets if this mesh has bones.
        /// </summary>
        public bool HasBones {
            get {
                return m_bones != null;
            }
        }

        /// <summary>
        /// Gets the bones that influence this mesh.
        /// </summary>
        public Bone[] Bones {
            get {
                return m_bones;
            }
        }

        /// <summary>
        /// Constructs a new Mesh.
        /// </summary>
        /// <param name="mesh">Unmanaged AiMesh struct.</param>
        internal Mesh(ref AiMesh mesh) {
            m_name = mesh.Name.GetString();
            m_primitiveType = mesh.PrimitiveTypes;
            m_vertexCount = (int) mesh.NumVertices;
            m_materialIndex = (int) mesh.MaterialIndex;

            //Load per-vertex arrays
            if(mesh.NumVertices > 0) {
                if(mesh.Vertices != IntPtr.Zero) {
                    m_vertices = MemoryHelper.MarshalArray<Vector3D>(mesh.Vertices, m_vertexCount);
                }
                if(mesh.Normals != IntPtr.Zero) {
                    m_normals = MemoryHelper.MarshalArray<Vector3D>(mesh.Normals, m_vertexCount);
                }
                if(mesh.Tangents != IntPtr.Zero) {
                    m_tangents = MemoryHelper.MarshalArray<Vector3D>(mesh.Tangents, m_vertexCount);
                }
                if(mesh.BiTangents != IntPtr.Zero) {
                    m_bitangents = MemoryHelper.MarshalArray<Vector3D>(mesh.BiTangents, m_vertexCount);
                }
            }

            //Load faces
            if(mesh.NumFaces > 0 && mesh.Faces != IntPtr.Zero) {
                AiFace[] faces = MemoryHelper.MarshalArray<AiFace>(mesh.Faces, (int) mesh.NumFaces);
                m_faces = new Face[faces.Length];
                for(int i = 0; i < m_faces.Length; i++) {
                    m_faces[i] = new Face(ref faces[i]);
                }
            }

            //Load UVW components - this should match the texture coordinate channels
            uint[] components = mesh.NumUVComponents;
            if(components != null) {
                m_texComponentNumber = new List<uint>();
                foreach(uint num in components) {
                    if(num > 0) {
                        m_texComponentNumber.Add(num);
                    }
                }
            }

            //Load texture coordinate channels
            IntPtr[] texCoords = mesh.TextureCoords;
            if(texCoords != null) {
                m_texCoords = new List<Vector3D[]>();
                foreach(IntPtr texPtr in texCoords) {
                    if(texPtr != IntPtr.Zero) {
                        m_texCoords.Add(MemoryHelper.MarshalArray<Vector3D>(texPtr, m_vertexCount));
                    }
                }
            }

            //Load vertex color channels
            IntPtr[] vertexColors = mesh.Colors;
            if(vertexColors != null) {
                m_colors = new List<Color4D[]>();
                foreach(IntPtr colorPtr in vertexColors) {
                    if(colorPtr != IntPtr.Zero) {
                        m_colors.Add(MemoryHelper.MarshalArray<Color4D>(colorPtr, m_vertexCount));
                    }
                }
            }

            //Load bones
            if(mesh.NumBones > 0 && mesh.Bones != IntPtr.Zero) {
                AiBone[] bones = MemoryHelper.MarshalArray<AiBone>(mesh.Bones, (int) mesh.NumBones, true);
                m_bones = new Bone[bones.Length];
                for(int i = 0; i < m_bones.Length; i++) {
                    m_bones[i] = new Bone(ref bones[i]);
                }
            }

            //Load anim meshes (attachment meshes)
            if(mesh.NumAnimMeshes > 0 && mesh.AnimMeshes != IntPtr.Zero) {
                AiAnimMesh[] animMeshes = MemoryHelper.MarshalArray<AiAnimMesh>(mesh.AnimMeshes, (int) mesh.NumAnimMeshes, true);
                m_meshAttachments = new MeshAnimationAttachment[animMeshes.Length];
                for(int i = 0; i < m_meshAttachments.Length; i++) {
                    m_meshAttachments[i] = new MeshAnimationAttachment(ref animMeshes[i]);
                }
            }
        }

        /// <summary>
        /// Checks if the mesh has vertex colors for the specified channel. If
        /// this returns true, you can be confident that the channel contains
        /// the same number of vertex colors as there are vertices in this mesh.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>True if vertex colors are present in the channel.</returns>
        public bool HasVertexColors(int channelIndex) {
            if(m_colors != null) {
                if(channelIndex >= m_colors.Count || channelIndex < 0) {
                    return false;
                }
                return VertexCount > 0 && m_colors[channelIndex] != null;
            }
            return false;
        }

        /// <summary>
        /// Checks if the mesh has texture coordinates for the specified channel.
        /// If this returns true, you can be confident that the channel contains the same
        /// number of texture coordinates as there are vertices in this mesh.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>True if texture coordinates are present in the channel.</returns>
        public bool HasTextureCoords(int channelIndex) {
            if(m_texCoords != null) {
                if(channelIndex >= m_texCoords.Count || channelIndex < 0) {
                    return false;
                }
                return VertexCount > 0 && m_texCoords[channelIndex] != null;
            }
            return false;
        }

        /// <summary>
        /// Gets the array of vertex colors from the specified vertex color channel.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>The vertex color array, or null if it does not exist.</returns>
        public Color4D[] GetVertexColors(int channelIndex) {
            if(HasVertexColors(channelIndex)) {
                return m_colors[channelIndex];
            }
            return null;
        }

        /// <summary>
        /// Gets the array of texture coordinates from the specified texture coordinate
        /// channel.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>The texture coordinate array, or null if it does not exist.</returns>
        public Vector3D[] GetTextureCoords(int channelIndex) {
            if(HasTextureCoords(channelIndex)) {
                return m_texCoords[channelIndex];
            }
            return null;
        }

        /// <summary>
        /// Gets the number of UV(W) components for the texture coordinate channel, this
        /// usually either 2 (UV) or 3 (UVW). No components mean the texture coordinate channel
        /// does not exist. The channel index matches the texture coordinate channel index.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>The number of UV(W) components the texture coordinate channel contains</returns>
        public int GetUVComponentCount(int channelIndex) {
            if(HasTextureCoords(channelIndex)) {
                if(m_texComponentNumber != null) {
                    return (int)m_texComponentNumber[channelIndex];
                }
            }
            return 0;
        }

        /// <summary>
        /// Convienence method for accumulating all face indices into a single
        /// index array.
        /// </summary>
        /// <returns>uint index array</returns>
        public uint[] GetIndices() {
            if(HasFaces) {
                List<uint> indices = new List<uint>();
                foreach(Face face in m_faces) {
                    if(face.IndexCount > 0 && face.Indices != null) {
                        indices.AddRange(face.Indices);
                    }
                }
                return indices.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Convienence method for accumulating all face indices into a single
        /// index array.
        /// </summary>
        /// <returns>int index array</returns>
        public int[] GetIntIndices() {
            //We could use a dirty hack here to do a conversion...but may as well be
            //safe just in case
            if(HasFaces) {
                List<int> indices = new List<int>();
                foreach(Face face in m_faces) {
                    if(face.IndexCount > 0 && face.Indices != null) {
                        foreach(uint index in face.Indices) {
                            indices.Add((int) index);
                        }
                    }
                }
                return indices.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Convienence method for accumulating all face indices into a single
        /// index array.
        /// </summary>
        /// <returns>short index array</returns>
        public short[] GetShortIndices() {
            //We could use a dirty hack here to do a conversion...but may as well be
            //safe just in case
            if(HasFaces) {
                List<short> indices = new List<short>();
                foreach(Face face in m_faces) {
                    if(face.IndexCount > 0 && face.Indices != null) {
                        foreach(uint index in face.Indices) {
                            indices.Add((short) index);
                        }
                    }
                }
                return indices.ToArray();
            }
            return null;
        }
    }
}
