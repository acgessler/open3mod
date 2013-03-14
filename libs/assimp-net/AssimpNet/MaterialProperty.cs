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
using System.Runtime.InteropServices;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// A key-value pairing that represents some material property.
    /// </summary>
    public sealed class MaterialProperty {
        private String m_name;
        private PropertyType m_type;
        private byte[] m_value;
        private TextureType m_texType;
        private int m_texIndex;
        private String m_stringValue;
        private String m_fullyQualifiedName;

        /// <summary>
        /// Gets the property key name. E.g. $tex.file. This corresponds to the
        /// "AiMatKeys" base name constants.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the type of property.
        /// </summary>
        public PropertyType PropertyType {
            get {
                return m_type;
            }
        }

        /// <summary>
        /// Gets the raw byte data count.
        /// </summary>
        public int ByteCount {
            get {
                return (m_value == null) ? 0 : m_value.Length;
            }
        }

        /// <summary>
        /// Checks if the property has data.
        /// </summary>
        public bool HasRawData {
            get {
                return m_value != null;
            }
        }

        /// <summary>
        /// Gets the raw byte data.
        /// </summary>
        public byte[] RawData {
            get {
                return m_value;
            }
        }

        /// <summary>
        /// Gets the texture type semantic, for non-texture properties this is always <see cref="Assimp.TextureType.None"/>.
        /// </summary>
        public TextureType TextureType {
            get {
                return m_texType;
            }
        }

        /// <summary>
        /// Gets the texture index, for non-texture properties this is always zero.
        /// </summary>
        public int TextureIndex {
            get {
                return m_texIndex;
            }
        }

        /// <summary>
        /// Gets the property's fully qualified name. Format: "{base name},{texture type semantic},{texture index}". E.g. "$clr.diffuse,0,0". This
        /// is the key that is used to index the property in the material property map.
        /// </summary>
        public String FullyQualifiedName {
            get {
                return m_fullyQualifiedName;
            }
        }

        /// <summary>
        /// Constructs a new MaterialProperty.
        /// </summary>
        /// <param name="property">Umananaged AiMaterialProperty struct</param>
        internal MaterialProperty(AiMaterialProperty property) {
            m_name = property.Key.GetString();
            m_type = property.Type;
            m_texIndex = (int) property.Index;
            m_texType = property.Semantic;
            
            if(property.DataLength > 0 && property.Data != IntPtr.Zero) {
                if (m_type == Assimp.PropertyType.String) {
                    m_stringValue = Marshal.PtrToStringAnsi(property.Data, (int)property.DataLength);
                } else {
                    m_value = MemoryHelper.MarshalArray<byte>(property.Data, (int)property.DataLength);
                }
            }

            m_fullyQualifiedName = String.Format("{0},{1},{2}", m_name, ((uint)m_texType).ToString(), m_texIndex.ToString());
        }

        /// <summary>
        /// Returns the property raw data as a float.
        /// </summary>
        /// <returns>Float</returns>
        public float AsFloat() {
            if(m_type == PropertyType.Float) {
                return BitConverter.ToSingle(m_value, 0);
            }
            return 0;
        }

        /// <summary>
        /// Returns the property raw data as an integer.
        /// </summary>
        /// <returns>Integer</returns>
        public int AsInteger() {
            if(m_type == PropertyType.Integer) {
                return BitConverter.ToInt32(m_value, 0);
            }
            return 0;
        }

        /// <summary>
        /// Returns the property raw data as a string.
        /// </summary>
        /// <returns>String</returns>
        public String AsString() {
            if(m_type == PropertyType.String) {
                return m_stringValue;
            }
            return null;
        }

        /// <summary>
        /// Returns the property raw data as a float array.
        /// </summary>
        /// <returns>Float array</returns>
        public float[] AsFloatArray() {
            if(m_type == Assimp.PropertyType.Float) {
                return ValueAsArray<float>();
            }
            return null;
        }

        /// <summary>
        /// Returns the property raw data as an integer array.
        /// </summary>
        /// <returns>Integer array</returns>
        public int[] AsIntegerArray() {
            if(m_type == Assimp.PropertyType.Integer) {
                return ValueAsArray<int>();
            }
            return null;
        }

        /// <summary>
        /// Returns the property raw data as a boolean.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool AsBoolean() {
            return (AsInteger() == 0) ? false : true;
        }

        /// <summary>
        /// Returns the property raw data as a Color3D.
        /// </summary>
        /// <returns>Color3D</returns>
        public Color3D AsColor3D() {
            if(m_type == Assimp.PropertyType.Float) {
                return ValueAs<Color3D>();
            }
            return new Color3D();
        }

        /// <summary>
        /// Returns the property raw data as a Color4D.
        /// </summary>
        /// <returns>Color4D</returns>
        public Color4D AsColor4D() {
            if(m_type == Assimp.PropertyType.Float) {
                return ValueAs<Color4D>();
            }
            return new Color4D();
        }

        /// <summary>
        /// Returns the property raw data as a ShadingMode enum value.
        /// </summary>
        /// <returns>Shading mode</returns>
        public ShadingMode AsShadingMode() {
            return (ShadingMode) AsInteger();
        }

        /// <summary>
        /// Returns the property raw data as a BlendMode enum value.
        /// </summary>
        /// <returns>Blend mode</returns>
        public BlendMode AsBlendMode() {
            return (BlendMode) AsInteger();
        }

        private unsafe T ValueAs<T>() where T : struct {
            T value = default(T);
            if(HasRawData) {
                try {
                    fixed(byte* ptr = m_value) {
                        value = MemoryHelper.MarshalStructure<T>(new IntPtr(ptr));
                    }
                } catch(Exception) {

                }
            }
            return value;
        }

        //Probably shouldn't use for anything other than float/int types.
        private unsafe T[] ValueAsArray<T>() where T : struct {
            T[] value = null;
            if(HasRawData) {
                try {
                    int size = Marshal.SizeOf(typeof(T));
                    fixed(byte* ptr = m_value) {
                        value = MemoryHelper.MarshalArray<T>(new IntPtr(ptr), ByteCount / size);
                    }
                } catch(Exception) {

                }
            }
            return value;
        }
    }
}
