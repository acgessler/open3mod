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
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Represents an embedded texture. Some file formats directly embed texture assets.
    /// Embedded textures may be uncompressed, where the data is given in an uncompressed format.
    /// Or it may be compressed in a format like png or jpg. In the latter case, the raw
    /// file bytes are given so the application must utilize an image decoder (e.g. DevIL) to
    /// get access to the actual color data. This object represents both types, so some properties may or may not be valid depending
    /// if it is compressed or not.
    /// </summary>
    public sealed class EmbeddedTexture : IMarshalable<EmbeddedTexture, AiTexture> {
        private bool m_isCompressed;

        //Uncompressed textures only
        private int m_width;
        private int m_height;
        private Texel[] m_nonCompressedData;

        //Compressed textures only
        private byte[] m_compressedData;
        private String m_compressedFormatHint;

        /// <summary>
        /// Gets if the texture is compressed or not.
        /// </summary>
        public bool IsCompressed {
            get {
                return m_isCompressed;
            }
        }

        /// <summary>
        /// Gets the width of the texture in pixels. Only valid for non-compressed textures.
        /// </summary>
        public int Width {
            get {
                return m_width;
            }
        }

        /// <summary>
        /// Gets the height of the texture in pixels. Only valid for non-compressed textures.
        /// </summary>
        public int Height {
            get {
                return m_height;
            }
        }

        /// <summary>
        /// Gets if the texture has non-compressed texel data. Only valid for non-compressed textures.
        /// </summary>
        public bool HasNonCompressedData {
            get {
                return m_nonCompressedData != null || m_nonCompressedData.Length != 0;
            }
        }

        /// <summary>
        /// Gets the size of the non-compressed texel data. Only valid for non-compressed textures.
        /// </summary>
        public int NonCompressedDataSize {
            get {
                return (m_nonCompressedData == null) ? 0 : m_nonCompressedData.Length;
            }
        }

        /// <summary>
        /// Gets the non-compressed texel data, the array is of size Width * Height. Only valid for non-compressed textures.
        /// </summary>
        public Texel[] NonCompressedData {
            get {
                return m_nonCompressedData;
            }
        }

        /// <summary>
        /// Gets if the embedded texture has compressed data. Only valid for compressed textures.
        /// </summary>
        public bool HasCompressedData {
            get {
                return m_compressedData != null || m_compressedData.Length != 0;
            }
        }

        /// <summary>
        /// Gets the size of the compressed data. Only valid for compressed textures.
        /// </summary>
        public int CompressedDataSize {
            get {
                return (m_compressedData == null) ? 0 : m_compressedData.Length;
            }
        }

        /// <summary>
        /// Gets the raw byte data representing the compressed texture. Only valid for compressed textures.
        /// </summary>
        public byte[] CompressedData {
            get {
                return m_compressedData;
            }
        }

        /// <summary>
        /// Gets the format hint to determine the type of compressed data. This hint
        /// is a three-character lower-case hint like "dds", "jpg", "png".
        /// </summary>
        public String CompressedFormatHint {
            get {
                return m_compressedFormatHint;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="EmbeddedTexture"/> class. Should use only if
        /// reading from a native value.
        /// </summary>
        public EmbeddedTexture() {
            m_isCompressed = false;
        }


        /// <summary>
        /// Constructs a new instance of the <see cref="EmbeddedTexture"/> class. This creates a compressed
        /// embedded texture.
        /// </summary>
        /// <param name="compressedFormatHint">The 3 character format hint.</param>
        /// <param name="compressedData">The compressed data.</param>
        public EmbeddedTexture(String compressedFormatHint, byte[] compressedData) {
            m_compressedFormatHint = compressedFormatHint;
            m_compressedData = compressedData;

            m_isCompressed = true;
            m_width = 0;
            m_height = 0;
            m_nonCompressedData = null;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="EmbeddedTexture"/> class. This creates an uncompressed
        /// embedded texture.
        /// </summary>
        /// <param name="width">Width of the texture</param>
        /// <param name="height">Height of the texture</param>
        /// <param name="uncompressedData">Color data</param>
        /// <exception cref="ArgumentException">Thrown if the data size does not match width * height.</exception>
        public EmbeddedTexture(int width, int height, Texel[] uncompressedData) {
            m_width = width;
            m_height = height;
            m_nonCompressedData = uncompressedData;

            if((m_width * m_height) == NonCompressedDataSize)
                throw new ArgumentException("Texel data size does not match width * height.");

            m_isCompressed = false;
            m_compressedFormatHint = null;
            m_compressedData = null;
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<EmbeddedTexture, AiTexture>.IsNativeBlittable {
            get { return true; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<EmbeddedTexture, AiTexture>.ToNative(IntPtr thisPtr, out AiTexture nativeValue) {
            if(IsCompressed) {
                nativeValue.Width = (uint) CompressedDataSize;
                nativeValue.Height = 0;
                nativeValue.Data = IntPtr.Zero;
                
                if(CompressedDataSize > 0)
                    nativeValue.Data = MemoryHelper.ToNativeArray<byte>(m_compressedData);

                nativeValue.SetFormatHint(m_compressedFormatHint);
            } else {
                nativeValue.Width = (uint) m_width;
                nativeValue.Height = (uint) m_height;
                nativeValue.Data = IntPtr.Zero;

                if(NonCompressedDataSize > 0)
                    nativeValue.Data = MemoryHelper.ToNativeArray<Texel>(m_nonCompressedData);

                nativeValue.SetFormatHint(null);
            }
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<EmbeddedTexture, AiTexture>.FromNative(ref AiTexture nativeValue) {
            m_isCompressed = nativeValue.Height == 0;

            if(IsCompressed) {
                m_width = 0;
                m_height = 0;
                m_nonCompressedData = null;
                m_compressedData = null;

                if(nativeValue.Width > 0 && nativeValue.Data != IntPtr.Zero)
                    m_compressedData = MemoryHelper.FromNativeArray<byte>(nativeValue.Data, (int) nativeValue.Width);

                m_compressedFormatHint = nativeValue.GetFormatHint();
            } else {
                m_compressedData = null;
                m_compressedFormatHint = null;
                m_nonCompressedData = null;

                m_width = (int) nativeValue.Width;
                m_height = (int) nativeValue.Height;

                int size = m_width * m_height;

                if(size > 0 && nativeValue.Data != IntPtr.Zero)
                    m_nonCompressedData = MemoryHelper.FromNativeArray<Texel>(nativeValue.Data, size);
            }
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{EmbeddedTexture, AiTexture}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue == IntPtr.Zero)
                return;

            AiTexture aiTexture = MemoryHelper.Read<AiTexture>(nativeValue);

            if(aiTexture.Width > 0 && aiTexture.Data != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiTexture.Data);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
