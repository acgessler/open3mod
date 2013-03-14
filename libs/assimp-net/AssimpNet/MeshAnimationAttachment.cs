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
    /// A mesh attachment store per-vertex animations for a particular frame. You may
    /// think of this as a 'patch' for the host mesh, since the mesh attachment replaces only certain
    /// vertex data streams at a particular time. Each mesh stores 'n' attached meshes. The actual
    /// relationship between the time line and mesh attachments is established by the mesh animation channel,
    /// which references singular mesh attachments by their ID and binds them to a time offset.
    /// </summary>
    public sealed class MeshAnimationAttachment {
        private int m_vertexCount;
        private Vector3D[] m_vertices;
        private Vector3D[] m_normals;
        private Vector3D[] m_tangents;
        private Vector3D[] m_bitangents;
        private List<Color4D[]> m_colors;
        private List<Vector3D[]> m_texCoords;

        /// <summary>
        /// Gets the number of vertices in this mesh. This is a replacement
        /// for the host mesh's vertex count. Likewise, a mesh attachment
        /// cannot add or remove per-vertex attributes, therefore the existance
        /// of vertex data will match the existance of data in the mesh.
        /// </summary>
        public int VertexCount {
            get {
                return m_vertexCount;
            }
        }

        /// <summary>
        /// Checks whether the attachment mesh overrides the vertex positions
        /// of its host mesh.
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
        /// Checks whether the attachment mesh overrides the vertex normals of
        /// its host mesh.
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
        /// Checks whether the attachment mesh overrides the vertex
        /// tangents and bitangents of its host mesh.
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
        /// Constructs a new MeshAttachment.
        /// </summary>
        /// <param name="animMesh">Unmanaged AiAnimMesh struct.</param>
        internal MeshAnimationAttachment(AiAnimMesh animMesh) {
            m_vertexCount = (int) animMesh.NumVertices;

            //Load per-vertex arrays
            if(animMesh.NumVertices > 0) {
                if(animMesh.Vertices != IntPtr.Zero) {
                    m_vertices = MemoryHelper.MarshalArray<Vector3D>(animMesh.Vertices, m_vertexCount);
                }
                if(animMesh.Normals != IntPtr.Zero) {
                    m_normals = MemoryHelper.MarshalArray<Vector3D>(animMesh.Normals, m_vertexCount);
                }
                if(animMesh.Tangents != IntPtr.Zero) {
                    m_tangents = MemoryHelper.MarshalArray<Vector3D>(animMesh.Tangents, m_vertexCount);
                }
                if(animMesh.BiTangents != IntPtr.Zero) {
                    m_bitangents = MemoryHelper.MarshalArray<Vector3D>(animMesh.BiTangents, m_vertexCount);
                }

                //Load texture coordinate channels
                IntPtr[] texCoords = animMesh.TextureCoords;
                if(texCoords != null) {
                    m_texCoords = new List<Vector3D[]>();
                    foreach(IntPtr texPtr in texCoords) {
                        if(texPtr != IntPtr.Zero) {
                            m_texCoords.Add(MemoryHelper.MarshalArray<Vector3D>(texPtr, m_vertexCount));
                        }
                    }
                }

                //Load vertex color channels
                IntPtr[] vertexColors = animMesh.Colors;
                if(vertexColors != null) {
                    m_colors = new List<Color4D[]>();
                    foreach(IntPtr colorPtr in vertexColors) {
                        if(colorPtr != IntPtr.Zero) {
                            m_colors.Add(MemoryHelper.MarshalArray<Color4D>(colorPtr, m_vertexCount));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the mesh attachment overrides a particular set of vertex colors on
        /// the host mesh. The index is between zero and the maximumb number of vertex color channels.
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
        /// Checks if the mesh attachment overrides a particular set of texture coordinates on
        /// the host mesh. The index is between zero and the maximum number of texture coordinate channels.
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
    }
}
