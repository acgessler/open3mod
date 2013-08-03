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
using System.Text;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// A key-value pairing that represents some material property.
    /// </summary>
    public sealed class MaterialProperty : IMarshalable<MaterialProperty, AiMaterialProperty> {
        private String m_name;
        private PropertyType m_type;
        private byte[] m_rawValue;
        private TextureType m_texType;
        private int m_texIndex;
        private String m_fullyQualifiedName;
        private bool m_fullQualifiedNameNeedsUpdate = true;

        /// <summary>
        /// Gets or sets the property key name. E.g. $tex.file. This corresponds to the
        /// "AiMatKeys" base name constants.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
            set {
                m_name = value;
                m_fullQualifiedNameNeedsUpdate = true;
            }
        }

        /// <summary>
        /// Gets or sets the type of property.
        /// </summary>
        public PropertyType PropertyType {
            get {
                return m_type;
            }
            set {
                m_type = value;
            }
        }

        /// <summary>
        /// Gets the raw byte data count.
        /// </summary>
        public int ByteCount {
            get {
                return (m_rawValue == null) ? 0 : m_rawValue.Length;
            }
        }

        /// <summary>
        /// Checks if the property has data.
        /// </summary>
        public bool HasRawData {
            get {
                return m_rawValue != null;
            }
        }

        /// <summary>
        /// Gets the raw byte data. To modify/read this data, see the Get/SetXXXValue methods.
        /// </summary>
        public byte[] RawData {
            get {
                return m_rawValue;
            }
        }

        /// <summary>
        /// Gets or sets the texture type semantic, for non-texture properties this is always <see cref="Assimp.TextureType.None"/>.
        /// </summary>
        public TextureType TextureType {
            get {
                return m_texType;
            }
            set {
                m_texType = value;
                m_fullQualifiedNameNeedsUpdate = true;
            }
        }

        /// <summary>
        /// Gets or sets the texture index, for non-texture properties this is always zero.
        /// </summary>
        public int TextureIndex {
            get {
                return m_texIndex;
            }
            set {
                m_texIndex = value;
                m_fullQualifiedNameNeedsUpdate = true;
            }
        }

        /// <summary>
        /// Gets the property's fully qualified name. Format: "{base name},{texture type semantic},{texture index}". E.g. "$clr.diffuse,0,0". This
        /// is the key that is used to index the property in the material property map.
        /// </summary>
        public String FullyQualifiedName {
            get {
                if(m_fullQualifiedNameNeedsUpdate) {
                    m_fullyQualifiedName = Material.CreateFullyQualifiedName(m_name, m_texType, m_texIndex);
                    m_fullQualifiedNameNeedsUpdate = false;
                }

                return m_fullyQualifiedName;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class.
        /// </summary>
        public MaterialProperty() {
            m_name = String.Empty;
            m_type = PropertyType.Buffer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Constructs a buffer property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="buffer">Property value</param>
        public MaterialProperty(String name, byte[] buffer) {
            m_name = name;
            m_type = PropertyType.Buffer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = buffer;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Constructs a float property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(String name, float value) {
            m_name = name;
            m_type = PropertyType.Float;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetFloatValue(value);
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Constructs an integer property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(String name, int value) {
            m_name = name;
            m_type = PropertyType.Integer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetIntegerValue(value);
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Constructs a boolean property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(String name, bool value) {
            m_name = name;
            m_type = PropertyType.Integer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetBooleanValue(value);
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a string property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(String name, String value) {
            m_name = name;
            m_type = PropertyType.String;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetStringValue(value);
        }


        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a texture property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        /// <param name="texType">Texture type</param>
        /// <param name="textureIndex">Texture index</param>
        public MaterialProperty(String name, String value, TextureType texType, int textureIndex) {
            m_name = name;
            m_type = PropertyType.String;
            m_texIndex = textureIndex;
            m_texType = texType;
            m_rawValue = null;

            SetStringValue(value);
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a float array property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="values">Property values</param>
        public MaterialProperty(String name, float[] values) {
            m_name = name;
            m_type = PropertyType.Float;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetFloatArrayValue(values);
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a int array property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="values">Property values</param>
        public MaterialProperty(String name, int[] values) {
            m_name = name;
            m_type = PropertyType.Integer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetIntegerArrayValue(values);
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a Color3D property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(String name, Color3D value) {
            m_name = name;
            m_type = PropertyType.Float;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetColor3DValue(value);
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a Color4D property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(String name, Color4D value) {
            m_name = name;
            m_type = PropertyType.Float;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetColor4DValue(value);
        }

        /// <summary>
        /// Gets the property raw data as a float.
        /// </summary>
        /// <returns>Float</returns>
        public float GetFloatValue() {
            if(m_type == PropertyType.Float || m_type == PropertyType.Integer)
                return GetValueAs<float>();

            return 0;
        }

        /// <summary>
        /// Sets the property raw data with a float.
        /// </summary>
        /// <param name="value">Float.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetFloatValue(float value) {
            if(m_type != PropertyType.Float || m_type != PropertyType.Integer)
                return false;

            return SetValueAs<float>(value);
        }

        /// <summary>
        /// Gets the property raw data as an integer.
        /// </summary>
        /// <returns>Integer</returns>
        public int GetIntegerValue() {
            if(m_type == PropertyType.Float || m_type == PropertyType.Integer)
                return GetValueAs<int>();

            return 0;
        }

        /// <summary>
        /// Sets the property raw data as an integer.
        /// </summary>
        /// <param name="value">Integer</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetIntegerValue(int value) {
            if(m_type != PropertyType.Float || m_type != PropertyType.Integer)
                return false;

            return SetValueAs<int>(value);
        }

        /// <summary>
        /// Gets the property raw data as a string.
        /// </summary>
        /// <returns>String</returns>
        public String GetStringValue() {
            if(m_type != PropertyType.String)
                return null;

            return GetMaterialString(m_rawValue);
        }

        /// <summary>
        /// Sets the property raw data as string.
        /// </summary>
        /// <param name="value">String</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetStringValue(String value) {
            if(m_type != PropertyType.String)
                return false;

            m_rawValue = SetMaterialString(value, m_rawValue);
            return true;
        }

        /// <summary>
        /// Gets the property raw data as a float array.
        /// </summary>
        /// <param name="count">Number of elements to get</param>
        /// <returns>Float array</returns>
        public float[] GetFloatArrayValue(int count) {
            if(m_type == PropertyType.Float || m_type == PropertyType.Integer)
                return GetValueArrayAs<float>(count);
            
            return null;
        }

        /// <summary>
        /// Gets the property raw data as a float array.
        /// </summary>
        /// <returns>Float array</returns>
        public float[] GetFloatArrayValue() {
            if(m_type == PropertyType.Float || m_type == PropertyType.Integer) {
                int count = ByteCount / sizeof(float);
                return GetValueArrayAs<float>(count);
            }

            return null;
        }

        /// <summary>
        /// Sets the property raw data as a float array.
        /// </summary>
        /// <param name="values">Values to set</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool SetFloatArrayValue(float[] values) {
            if(m_type != PropertyType.Float || m_type != PropertyType.Integer)
                return false;

            return SetValueArrayAs<float>(values);
        }

        /// <summary>
        /// Gets the property raw data as an integer array.
        /// </summary>
        /// <param name="count">Number of elements to get</param>
        /// <returns>Integer array</returns>
        public int[] GetIntegerArrayValue(int count) {
            if(m_type == PropertyType.Float || m_type == PropertyType.Integer)
                return GetValueArrayAs<int>(count);

            return null;
        }

        /// <summary>
        /// Gets the property raw data as an integer array.
        /// </summary>
        /// <returns>Integer array</returns>
        public int[] GetIntegerArrayValue() {
            if(m_type == PropertyType.Float || m_type == PropertyType.Integer) {
                int count = ByteCount / sizeof(int);
                return GetValueArrayAs<int>(count);
            }

            return null;
        }

        /// <summary>
        /// Sets the property raw data as an integer array.
        /// </summary>
        /// <param name="values">Values to set</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool SetIntegerArrayValue(int[] values) {
            if(m_type != PropertyType.Float || m_type != PropertyType.Integer)
                return false;

            return SetValueArrayAs<int>(values);
        }

        /// <summary>
        /// Gets the property raw data as a boolean.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool GetBooleanValue() {
            return (GetIntegerValue() == 0) ? false : true;
        }

        /// <summary>
        /// Sets the property raw data as a boolean.
        /// </summary>
        /// <param name="value">Boolean value</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetBooleanValue(bool value) {
            return SetIntegerValue((value == false) ? 0 : 1);
        }

        /// <summary>
        /// Gets the property raw data as a Color3D.
        /// </summary>
        /// <returns>Color3D</returns>
        public Color3D GetColor3DValue() {
            if(m_type != PropertyType.Float)
                return new Color3D();

            return GetValueAs<Color3D>();
        }

        /// <summary>
        /// Sets the property raw data as a Color3D.
        /// </summary>
        /// <param name="value">Color3D</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetColor3DValue(Color3D value) {
            if(m_type != PropertyType.Float)
                return false;

            return SetValueAs<Color3D>(value);
        }

        /// <summary>
        /// Gets the property raw data as a Color4D.
        /// </summary>
        /// <returns>Color4D</returns>
        public Color4D GetColor4DValue() {
            if(m_type != PropertyType.Float || m_rawValue == null)
                return new Color4D();

            //We may have a Color that's RGB, so still read it and set alpha to 1.0
            unsafe {
                fixed(byte* ptr = m_rawValue) {

                    if(m_rawValue.Length >= MemoryHelper.SizeOf<Color4D>()) {
                        return MemoryHelper.Read<Color4D>(new IntPtr(ptr));
                    } else if(m_rawValue.Length >= MemoryHelper.SizeOf<Color3D>()) {
                        return new Color4D(MemoryHelper.Read<Color3D>(new IntPtr(ptr)), 1.0f);
                    }

                }
            }

            return new Color4D();
        }

        /// <summary>
        /// Sets the property raw data as a Color4D.
        /// </summary>
        /// <param name="value">Color4D</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetColor4DValue(Color4D value) {
            if(m_type != PropertyType.Float)
                return false;

            return SetValueAs<Color4D>(value);
        }

        private unsafe T[] GetValueArrayAs<T>(int count) where T : struct {
            int size = MemoryHelper.SizeOf<T>();

            if(m_rawValue != null && (m_rawValue.Length >= (size * count))) {
                T[] array = new T[count];
                fixed(byte* ptr = m_rawValue) {
                    MemoryHelper.Read<T>(new IntPtr(ptr), array, 0, count);
                }

                return array;
            }

            return null;
        }

        private unsafe T GetValueAs<T>() where T : struct {
            int size = MemoryHelper.SizeOf<T>();

            if(m_rawValue != null && m_rawValue.Length >= size) {
                fixed(byte* ptr = m_rawValue) {
                    return MemoryHelper.Read<T>(new IntPtr(ptr));
                }
            }

            return default(T);
        }

        private unsafe bool SetValueArrayAs<T>(T[] data) where T : struct {
            if(data == null || data.Length == 0)
                return false;

            int size = MemoryHelper.SizeOf<T>(data);

            //Resize byte array if necessary
            if(m_rawValue == null || m_rawValue.Length != size)
                m_rawValue = new byte[size];

            fixed(byte* ptr = m_rawValue) {
                MemoryHelper.Write<T>(new IntPtr(ptr), data, 0, data.Length);
            }

            return true;
        }

        private unsafe bool SetValueAs<T>(T value) where T : struct {
            int size = MemoryHelper.SizeOf<T>();

            //Resize byte array if necessary
            if(m_rawValue == null || m_rawValue.Length != size)
                m_rawValue = new byte[size];

            fixed(byte* ptr = m_rawValue) {
                MemoryHelper.Write<T>(new IntPtr(ptr), ref value);
            }

            return true;
        }

        private static unsafe String GetMaterialString(byte[] matPropData) {
            if(matPropData == null)
                return String.Empty;

            fixed(byte* ptr = &matPropData[0]) {
                //String is stored as 32 bit length prefix THEN followed by zero-terminated UTF8 data (basically need to reconstruct an AiString)
                AiString aiString;
                aiString.Length = new UIntPtr((uint) MemoryHelper.Read<int>(new IntPtr(ptr)));

                //Memcpy starting at dataPtr + sizeof(int) for length + 1 (to account for null terminator)
                MemoryHelper.CopyMemory(new IntPtr(aiString.Data), MemoryHelper.AddIntPtr(new IntPtr(ptr), sizeof(int)), (int) aiString.Length.ToUInt32() + 1);

                return aiString.GetString();
            }
        }

        private static unsafe byte[] SetMaterialString(String value, byte[] existing) {
            if(String.IsNullOrEmpty(value))
                return null;

            int stringSize = Encoding.UTF8.GetByteCount(value);

            if(stringSize < 0)
                return null;

            int size = stringSize + 1 + sizeof(int);
            byte[] data = existing;

            if(existing == null || existing.Length != size)
                data = new byte[size];

            fixed(byte* bytePtr = &data[0]) {
                MemoryHelper.Write<int>(new IntPtr(bytePtr), ref stringSize);
                byte[] utfBytes = Encoding.UTF8.GetBytes(value);
                MemoryHelper.Write<byte>(new IntPtr(bytePtr + sizeof(int)), utfBytes, 0, utfBytes.Length);
                //Last byte should be zero
            }

            return data;
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<MaterialProperty, AiMaterialProperty>.IsNativeBlittable {
            get { return true; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<MaterialProperty, AiMaterialProperty>.ToNative(IntPtr thisPtr, out AiMaterialProperty nativeValue) {
            nativeValue.Key = new AiString(m_name);
            nativeValue.Type = m_type;
            nativeValue.Index = (uint) m_texIndex;
            nativeValue.Semantic = m_texType;
            nativeValue.Data = IntPtr.Zero;
            nativeValue.DataLength = 0;

            if(m_rawValue != null) {
                nativeValue.DataLength = (uint) m_rawValue.Length;
                nativeValue.Data = MemoryHelper.ToNativeArray<byte>(m_rawValue);
            }
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<MaterialProperty, AiMaterialProperty>.FromNative(ref AiMaterialProperty nativeValue) {
            m_name = nativeValue.Key.GetString();
            m_type = nativeValue.Type;
            m_texIndex = (int) nativeValue.Index;
            m_texType = nativeValue.Semantic;
            m_rawValue = null;

            if(nativeValue.DataLength > 0 && nativeValue.Data != IntPtr.Zero)
                m_rawValue = MemoryHelper.FromNativeArray<byte>(nativeValue.Data, (int) nativeValue.DataLength);
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{MaterialProperty, AiMaterialProperty}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue == IntPtr.Zero)
                return;

            AiMaterialProperty aiMatProp = MemoryHelper.Read<AiMaterialProperty>(nativeValue);

            if(aiMatProp.DataLength > 0 && aiMatProp.Data != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiMatProp.Data);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
