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
    /// A single face in a mesh, referring to multiple vertices. This can be a triangle
    /// if the index count is equal to three, or a polygon if the count is greater than three.
    /// 
    /// Since multiple primitive types can be contained in a single mesh, this approach
    /// allows you to better examine how the mesh is constructed. If you use the <see cref="PostProcessSteps.SortByPrimitiveType"/>
    /// post process step flag during import, then each mesh will be homogenous where primitive type is concerned.
    /// </summary>
    public sealed class Face : IMarshalable<Face, AiFace> {
        private List<int> m_indices;

        /// <summary>
        /// Gets the number of indices defined in the face.
        /// </summary>
        public int IndexCount {
            get {
                return m_indices.Count;
            }
        }

        /// <summary>
        /// Gets if the face has faces (should always be true).
        /// </summary>
        public bool HasIndices {
            get {
                return m_indices.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets the indices that refer to positions of vertex data in the mesh's vertex 
        /// arrays.
        /// </summary>
        public List<int> Indices {
            get {
                return m_indices;
            }
        }

        /// <summary>
        /// Constructs a new Face.
        /// </summary>
        /// <param name="face">Unmanaged AiFace structure</param>
        internal Face(ref AiFace face) {
            m_indices = new List<int>();

            if(face.NumIndices > 0 && face.Indices != IntPtr.Zero)
                m_indices.AddRange(MemoryHelper.MarshalArray<int>(face.Indices, (int) face.NumIndices));
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Face"/> class.
        /// </summary>
        public Face() {
            m_indices = new List<int>();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Face"/> class.
        /// </summary>
        /// <param name="indices">Face indices</param>
        public Face(int[] indices) {
            m_indices = new List<int>();

            if(indices != null)
                m_indices.AddRange(indices);
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Face, AiFace>.IsNativeBlittable {
            get { return true; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Face, AiFace>.ToNative(IntPtr thisPtr, out AiFace nativeValue) {
            nativeValue.NumIndices = (uint) IndexCount;
            nativeValue.Indices = IntPtr.Zero;
            
            if(nativeValue.NumIndices > 0)
                nativeValue.Indices = MemoryHelper.ToNativeArray<int>(m_indices.ToArray());
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Face, AiFace>.FromNative(ref AiFace nativeValue) {
            m_indices.Clear();

            if(nativeValue.NumIndices > 0 && nativeValue.Indices != IntPtr.Zero)
                m_indices.AddRange(MemoryHelper.FromNativeArray<int>(nativeValue.Indices, (int) nativeValue.NumIndices));
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue == IntPtr.Zero)
                return;

            AiFace aiFace = MemoryHelper.Read<AiFace>(nativeValue);

            if(aiFace.NumIndices > 0 && aiFace.Indices != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiFace.Indices);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
