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
    public sealed class Mesh : IMarshalable<Mesh, AiMesh> {
        private String m_name;
        private PrimitiveType m_primitiveType;
        private int m_materialIndex;
        private List<Vector3D> m_vertices;
        private List<Vector3D> m_normals;
        private List<Vector3D> m_tangents;
        private List<Vector3D> m_bitangents;
        private List<Face> m_faces;
        private List<Color4D>[] m_colors;
        private List<Vector3D>[] m_texCoords;
        private int[] m_texComponentCount;
        private List<Bone> m_bones;
        private List<MeshAnimationAttachment> m_meshAttachments;

        /// <summary>
        /// Gets or sets the mesh name. This tends to be used
        /// when formats name nodes and meshes independently,
        /// vertex animations refer to meshes by their names,
        /// or importers split meshes up, each mesh will reference
        /// the same (dummy) name.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
            set {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the primitive type. This may contain more than one
        /// type unless if <see cref="PostProcessSteps.SortByPrimitiveType"/>
        /// option is not set.
        /// </summary>
        public PrimitiveType PrimitiveType {
            get {
                return m_primitiveType;
            }
            set {
                m_primitiveType = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the material associated with this mesh.
        /// </summary>
        public int MaterialIndex {
            get {
                return m_materialIndex;
            }
            set {
                m_materialIndex = value;
            }
        }

        /// <summary>
        /// Gets the number of vertices in this mesh. This is the count that all
        /// per-vertex lists should be the size of.
        /// </summary>
        public int VertexCount {
            get {
                return m_vertices.Count;
            }
        }

        /// <summary>
        /// Gets if the mesh has a vertex array. This should always return
        /// true provided no special scene flags are set.
        /// </summary>
        public bool HasVertices {
            get {
                return m_vertices.Count > 0;
            }
        }

        /// <summary>
        /// Gets the vertex position list.
        /// </summary>
        public List<Vector3D> Vertices {
            get {
                return m_vertices; 
            }
        }

        /// <summary>
        /// Gets if the mesh as normals. If it does exist, the count should be the same as the vertex count.
        /// </summary>
        public bool HasNormals {
            get {
                return m_normals.Count > 0;
            }
        }

        /// <summary>
        /// Gets the vertex normal list.
        /// </summary>
        public List<Vector3D> Normals {
            get {
                return m_normals;
            }
        }

        /// <summary>
        /// Gets if the mesh has tangents and bitangents. It is not
        /// possible for one to be without the other. If it does exist, the count should be the same as the vertex count.
        /// </summary>
        public bool HasTangentBasis {
            get {
                return m_tangents.Count > 0 && m_bitangents.Count > 0;
            }
        }

        /// <summary>
        /// Gets the vertex tangent list.
        /// </summary>
        public List<Vector3D> Tangents {
            get {
                return m_tangents;
            }
        }

        /// <summary>
        /// Gets the vertex bitangent list.
        /// </summary>
        public List<Vector3D> BiTangents {
            get {
                return m_bitangents;
            }
        }

        /// <summary>
        /// Gets the number of faces contained in the mesh.
        /// </summary>
        public int FaceCount {
            get {
                return m_faces.Count;
            }
        }

        /// <summary>
        /// Gets if the mesh contains faces. If no special
        /// scene flags are set, this should always return true.
        /// </summary>
        public bool HasFaces {
            get {
                return m_faces.Count > 0;
            }
        }

        /// <summary>
        /// Gets the mesh's faces. Each face will contain indices
        /// to the vertices.
        /// </summary>
        public List<Face> Faces {
            get {
                return m_faces;
            }
        }

        /// <summary>
        /// Gets the number of valid vertex color channels contained in the
        /// mesh (list is not empty/not null). This can be a value between zero and the maximum vertex color count. Each individual channel
        /// should be the size of <see cref="VertexCount"/>.
        /// </summary>
        public int VertexColorChannelCount {
            get {
                int count = 0;
                for(int i = 0; i < m_colors.Length; i++) {
                    if(HasVertexColors(i))
                        count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the number of valid texture coordinate channels contained
        /// in the mesh (list is not empty/not null). This can be a value between zero and the maximum texture coordinate count.
        /// Each individual channel should be the size of <see cref="VertexCount"/>.
        /// </summary>
        public int TextureCoordinateChannelCount {
            get {
                int count = 0;
                for(int i = 0; i < m_texCoords.Length; i++) {
                    if(HasTextureCoords(i))
                        count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the array that contains each vertex color channels, by default all are lists of zero (but can be set to null). Each index
        /// in the array corresponds to the texture coordinate channel. The length of the array corresponds to Assimp's maximum vertex color channel limit.
        /// </summary>
        public List<Color4D>[] VertexColorChannels {
            get {
                return m_colors;
            }
        }

        /// <summary>
        /// Gets the array that contains each texture coordinate channel, by default all are lists of zero (but can be set to null). Each index
        /// in the array corresponds to the texture coordinate channel. The length of the array corresponds to Assimp's maximum UV channel limit.
        /// </summary>
        public List<Vector3D>[] TextureCoordinateChannels {
            get {
                return m_texCoords;
            }
        }

        /// <summary>
        /// Gets the array that contains the count of UV(W) components for each texture coordinate channel, usually 2 (UV) or 3 (UVW). A component
        /// value of zero means the texture coordinate channel does not exist. The channel index (index in the array) corresponds
        /// to the texture coordinate channel index.
        /// </summary>
        public int[] UVComponentCount {
            get {
                return m_texComponentCount;
            }
        }

        /// <summary>
        /// Gets the number of bones that influence this mesh.
        /// </summary>
        public int BoneCount {
            get {
                return m_bones.Count;
            }
        }

        /// <summary>
        /// Gets if this mesh has bones.
        /// </summary>
        public bool HasBones {
            get {
                return m_bones.Count > 0;
            }
        }

        /// <summary>
        /// Gets the bones that influence this mesh.
        /// </summary>
        public List<Bone> Bones {
            get {
                return m_bones;
            }
        }

        /// <summary>
        /// Gets the number of mesh animation attachments that influence this mesh.
        /// </summary>
        public int MeshAnimationAttachmentCount {
            get {
                return m_meshAttachments.Count;
            }
        }

        /// <summary>
        /// Gets if this mesh has mesh animation attachments.
        /// </summary>
        public bool HasMeshAnimationAttachments {
            get {
                return m_meshAttachments.Count > 0;
            }
        }

        /// <summary>
        /// Gets the mesh animation attachments that influence this mesh.
        /// </summary>
        public List<MeshAnimationAttachment> MeshAnimationAttachments {
            get {
                return m_meshAttachments;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        public Mesh() : this(String.Empty, PrimitiveType.Triangle) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="name">Name of the mesh.</param>
        public Mesh(String name) : this(name, PrimitiveType.Triangle) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="primType">Primitive types contained in the mesh.</param>
        public Mesh(PrimitiveType primType) : this(String.Empty, primType) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="primType">Primitive types contained in the mesh.</param>
        public Mesh(String name, PrimitiveType primType) {
            m_name = name;
            m_primitiveType = primType;
            m_materialIndex = 0;

            m_vertices = new List<Vector3D>();
            m_normals = new List<Vector3D>();
            m_tangents = new List<Vector3D>();
            m_bitangents = new List<Vector3D>();
            m_colors = new List<Color4D>[AiDefines.AI_MAX_NUMBER_OF_COLOR_SETS];

            for(int i = 0; i < m_colors.Length; i++) {
                m_colors[i] = new List<Color4D>();
            }

            m_texCoords = new List<Vector3D>[AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS];

            for(int i = 0; i < m_texCoords.Length; i++) {
                m_texCoords[i] = new List<Vector3D>();
            }

            m_texComponentCount = new int[AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS];
            m_bones = new List<Bone>();
            m_faces = new List<Face>();
            m_meshAttachments = new List<MeshAnimationAttachment>();
        }

        /// <summary>
        /// Checks if the mesh has vertex colors for the specified channel. This returns false if the list
        /// is null or empty. The channel, if it exists, should contain the same number of entries as <see cref="VertexCount"/>.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>True if vertex colors are present in the channel.</returns>
        public bool HasVertexColors(int channelIndex) {
            if(channelIndex >= m_colors.Length || channelIndex < 0)
                return false;

            List<Color4D> colors = m_colors[channelIndex];

            if(colors != null)
                return colors.Count > 0;

            return false;
        }

        /// <summary>
        /// Checks if the mesh has texture coordinates for the specified channel. This returns false if the list
        /// is null or empty. The channel, if it exists, should contain the same number of entries as <see cref="VertexCount"/>.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>True if texture coordinates are present in the channel.</returns>
        public bool HasTextureCoords(int channelIndex) {
            if(channelIndex >= m_texCoords.Length || channelIndex < 0)
                return false;

            List<Vector3D> texCoords = m_texCoords[channelIndex];

            if(texCoords != null)
                return texCoords.Count > 0;

            return false;
        }

        /// <summary>
        /// Convienence method for setting this meshe's face list from an index buffer.
        /// </summary>
        /// <param name="indices">Index buffer</param>
        /// <param name="indicesPerFace">Indices per face</param>
        /// <returns>True if the operation succeeded, false otherwise (e.g. not enough data)</returns>
        public bool SetIndices(int[] indices, int indicesPerFace) {
            if(indices == null || indices.Length == 0 || ((indices.Length % indicesPerFace) != 0))
                return false;

            m_faces.Clear();

            int numFaces = indices.Length / indicesPerFace;
            int index = 0;

            for(int i = 0; i < numFaces; i++) {
                Face face = new Face();
                for(int j = 0; j < indicesPerFace; j++) {
                    face.Indices.Add(indices[index]);
                    index++;
                }
                m_faces.Add(face);
            }

            return true;
        }

        /// <summary>
        /// Convienence method for accumulating all face indices into a single
        /// index array.
        /// </summary>
        /// <returns>int index array</returns>
        public int[] GetIndices() {
            if(HasFaces) {
                List<int> indices = new List<int>();
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
        /// Convienence method for accumulating all face indices into a single index
        /// array as unsigned integers (the default from Assimp, if you need them).
        /// </summary>
        /// <returns>uint index array</returns>
        [CLSCompliant(false)]
        public uint[] GetUnsignedIndices() {
            if(HasFaces) {
                List<uint> indices = new List<uint>();
                foreach(Face face in m_faces) {
                    if(face.IndexCount > 0 && face.Indices != null) {
                        foreach(uint index in face.Indices) {
                            indices.Add((uint) index);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Convienence method for accumulating all face indices into a single
        /// index array.
        /// </summary>
        /// <returns>short index array</returns>
        public short[] GetShortIndices() {
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

        private void ClearBuffers() {
            m_vertices.Clear();
            m_normals.Clear();
            m_tangents.Clear();
            m_bitangents.Clear();

            for(int i = 0; i < m_colors.Length; i++) {
                List<Color4D> colors = m_colors[i];

                if(colors == null)
                    m_colors[i] = new List<Color4D>();
                else
                    colors.Clear();
            }

            for(int i = 0; i < m_texCoords.Length; i++) {
                List<Vector3D> texCoords = m_texCoords[i];

                if(texCoords == null)
                    m_texCoords[i] = new List<Vector3D>();
                else
                    texCoords.Clear();
            }

            for(int i = 0; i < m_texComponentCount.Length; i++) {
                m_texComponentCount[i] = 0;
            }

            m_bones.Clear();
            m_faces.Clear();
            m_meshAttachments.Clear();
        }

        private Vector3D[] CopyTo(List<Vector3D> list, Vector3D[] copy) {
            list.CopyTo(copy);

            return copy;
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Mesh, AiMesh>.IsNativeBlittable {
            get { return false; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Mesh, AiMesh>.ToNative(IntPtr thisPtr, out AiMesh nativeValue) {
            nativeValue.Name = new AiString(m_name);
            nativeValue.Vertices = IntPtr.Zero;
            nativeValue.Normals = IntPtr.Zero;
            nativeValue.Tangents = IntPtr.Zero;
            nativeValue.BiTangents = IntPtr.Zero;
            nativeValue.AnimMeshes = IntPtr.Zero;
            nativeValue.Bones = IntPtr.Zero;
            nativeValue.Faces = IntPtr.Zero;
            nativeValue.Colors = new IntPtr[AiDefines.AI_MAX_NUMBER_OF_COLOR_SETS];
            nativeValue.TextureCoords = new IntPtr[AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS];
            nativeValue.NumUVComponents = new uint[AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS];
            nativeValue.PrimitiveTypes = m_primitiveType;
            nativeValue.MaterialIndex = (uint) m_materialIndex;
            nativeValue.NumVertices = (uint) VertexCount;
            nativeValue.NumBones = (uint) BoneCount;
            nativeValue.NumFaces = (uint) FaceCount;
            nativeValue.NumAnimMeshes = (uint) MeshAnimationAttachmentCount;

            if(nativeValue.NumVertices > 0) {

                //Since we can have so many buffers of Vector3D with same length, lets re-use a buffer
                Vector3D[] copy = new Vector3D[nativeValue.NumVertices];

                nativeValue.Vertices = MemoryHelper.ToNativeArray<Vector3D>(CopyTo(m_vertices, copy));

                if(HasNormals)
                    nativeValue.Normals = MemoryHelper.ToNativeArray<Vector3D>(CopyTo(m_normals, copy));

                if(HasTangentBasis) {
                    nativeValue.Tangents = MemoryHelper.ToNativeArray<Vector3D>(CopyTo(m_tangents, copy));
                    nativeValue.BiTangents = MemoryHelper.ToNativeArray<Vector3D>(CopyTo(m_bitangents, copy));
                }

                //Vertex Color channels
                for(int i = 0; i < m_colors.Length; i++) {
                    List<Color4D> list = m_colors[i];

                    if(list == null || list.Count == 0) {
                        nativeValue.Colors[i] = IntPtr.Zero;
                    } else {
                        nativeValue.Colors[i] = MemoryHelper.ToNativeArray<Color4D>(list.ToArray());
                    }
                }

                //Texture coordinate channels
                for(int i = 0; i < m_texCoords.Length; i++) {
                    List<Vector3D> list = m_texCoords[i];

                    if(list == null || list.Count == 0) {
                        nativeValue.TextureCoords[i] = IntPtr.Zero;
                    } else {
                        nativeValue.TextureCoords[i] = MemoryHelper.ToNativeArray<Vector3D>(CopyTo(list, copy));
                    }
                }

                //UV components for each tex coordinate channel
                for(int i = 0; i < m_texComponentCount.Length; i++) {
                    nativeValue.NumUVComponents[i] = (uint) m_texComponentCount[i];
                }
            }

            //Faces
            if(nativeValue.NumFaces > 0)
                nativeValue.Faces = MemoryHelper.ToNativeArray<Face, AiFace>(m_faces.ToArray());

            //Bones
            if(nativeValue.NumBones > 0)
                nativeValue.Bones = MemoryHelper.ToNativeArray<Bone, AiBone>(m_bones.ToArray(), true);

            //Attachment meshes
            if(nativeValue.NumAnimMeshes > 0)
                nativeValue.AnimMeshes = MemoryHelper.ToNativeArray<MeshAnimationAttachment, AiAnimMesh>(m_meshAttachments.ToArray());
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Mesh, AiMesh>.FromNative(ref AiMesh nativeValue) {
            ClearBuffers();

            int vertexCount = (int) nativeValue.NumVertices;
            m_name = nativeValue.Name.GetString();
            m_materialIndex = (int) nativeValue.MaterialIndex;

            //Load Per-vertex components
            if(vertexCount > 0) {

                //Positions
                if(nativeValue.Vertices != IntPtr.Zero)
                    m_vertices.AddRange(MemoryHelper.FromNativeArray<Vector3D>(nativeValue.Vertices, vertexCount));

                //Normals
                if(nativeValue.Normals != IntPtr.Zero)
                    m_normals.AddRange(MemoryHelper.FromNativeArray<Vector3D>(nativeValue.Normals, vertexCount));

                //Tangents
                if(nativeValue.Tangents != IntPtr.Zero)
                    m_tangents.AddRange(MemoryHelper.FromNativeArray<Vector3D>(nativeValue.Tangents, vertexCount));

                //BiTangents
                if(nativeValue.BiTangents != IntPtr.Zero)
                    m_bitangents.AddRange(MemoryHelper.FromNativeArray<Vector3D>(nativeValue.BiTangents, vertexCount));

                //Vertex Color channels
                IntPtr[] colors = nativeValue.Colors;

                if(colors != null) {
                    for(int i = 0; i < colors.Length; i++) {
                        IntPtr colorPtr = colors[i];

                        if(colorPtr != IntPtr.Zero)
                            m_colors[i].AddRange(MemoryHelper.FromNativeArray<Color4D>(colorPtr, vertexCount));
                    }
                }

                //Texture coordinate channels
                IntPtr[] texCoords = nativeValue.TextureCoords;

                if(texCoords != null) {
                    for(int i = 0; i < texCoords.Length; i++) {
                        IntPtr texCoordsPtr = texCoords[i];

                        if(texCoordsPtr != IntPtr.Zero)
                            m_texCoords[i].AddRange(MemoryHelper.FromNativeArray<Vector3D>(texCoordsPtr, vertexCount));
                    }
                }

                //UV components for each tex coordinate channel
                uint[] uvComps = nativeValue.NumUVComponents;

                if(uvComps != null) {
                    for(int i = 0; i < uvComps.Length; i++) {
                        m_texComponentCount[i] = (int) uvComps[i];
                    }
                }
            }

            //Faces
            if(nativeValue.NumFaces > 0 && nativeValue.Faces != IntPtr.Zero)
                m_faces.AddRange(MemoryHelper.FromNativeArray<Face, AiFace>(nativeValue.Faces, (int) nativeValue.NumFaces));

            //Bones
            if(nativeValue.NumBones > 0 && nativeValue.Bones != IntPtr.Zero)
                m_bones.AddRange(MemoryHelper.FromNativeArray<Bone, AiBone>(nativeValue.Bones, (int) nativeValue.NumBones, true));

            //Attachment meshes
            if(nativeValue.NumAnimMeshes > 0 && nativeValue.AnimMeshes != IntPtr.Zero)
                m_meshAttachments.AddRange(MemoryHelper.FromNativeArray<MeshAnimationAttachment, AiAnimMesh>(nativeValue.AnimMeshes, (int) nativeValue.NumAnimMeshes, true));
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{Mesh, AiMesh}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue == IntPtr.Zero)
                return;

            AiMesh aiMesh = MemoryHelper.MarshalStructure<AiMesh>(nativeValue);

            if(aiMesh.NumVertices > 0) {
                if(aiMesh.Vertices != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiMesh.Vertices);

                if(aiMesh.Normals != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiMesh.Normals);

                if(aiMesh.Tangents != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiMesh.Tangents);

                if(aiMesh.BiTangents != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiMesh.BiTangents);

                //Vertex Color channels
                IntPtr[] colors = aiMesh.Colors;

                if(colors != null) {
                    for(int i = 0; i < colors.Length; i++) {
                        IntPtr colorPtr = colors[i];

                        if(colorPtr != IntPtr.Zero)
                            MemoryHelper.FreeMemory(colorPtr);
                    }
                }

                //Texture coordinate channels
                IntPtr[] texCoords = aiMesh.TextureCoords;

                if(texCoords != null) {
                    for(int i = 0; i < texCoords.Length; i++) {
                        IntPtr texCoordsPtr = texCoords[i];

                        if(texCoordsPtr != IntPtr.Zero)
                            MemoryHelper.FreeMemory(texCoordsPtr);
                    }
                }
            }

            //Faces
            if(aiMesh.NumFaces > 0 && aiMesh.Faces != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiFace>(aiMesh.Faces, (int) aiMesh.NumFaces, Face.FreeNative);

            //Bones
            if(aiMesh.NumBones > 0 && aiMesh.Bones != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiBone>(aiMesh.Bones, (int) aiMesh.NumBones, Bone.FreeNative, true);

            //Attachment meshes
            if(aiMesh.NumAnimMeshes > 0 && aiMesh.AnimMeshes != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiAnimMesh>(aiMesh.AnimMeshes, (int) aiMesh.NumAnimMeshes, MeshAnimationAttachment.FreeNative, true);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
