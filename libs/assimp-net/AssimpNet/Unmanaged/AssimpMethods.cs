/*
* Copyright (c) 2012 Nicholas Woodfield
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
using System.IO;
using System.Runtime.InteropServices;

namespace Assimp.Unmanaged {
    /// <summary>
    /// Static class containing the P/Invoke methods exposing the Assimp C-API.
    /// </summary>
    public static class AssimpMethods {

        #region Native DLL Declarations

#if X64
        private const String AssimpDLL = "Assimp64.dll";
#else
        private const String AssimpDLL = "Assimp32.dll";
#endif

        #endregion

        #region Import Methods

        [DllImport(AssimpDLL, EntryPoint = "aiImportFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aiImportFile([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String file, uint flags);

        /// <summary>
        /// Imports a file.
        /// </summary>
        /// <param name="file">Valid filename</param>
        /// <param name="flags">Post process flags specifying what steps are to be run after the import.</param>
        /// <returns>Pointer to the unmanaged data structure.</returns>
        public static IntPtr ImportFile(String file, PostProcessSteps flags) {
            return aiImportFile(file, (uint) flags);
        }

        [DllImport(AssimpDLL, EntryPoint = "aiImportFileFromMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aiImportFileFromMemory(byte[] buffer, uint bufferLength, uint flags, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String formatHint);

        /// <summary>
        /// Imports a scene from a stream. This uses the "aiImportFileFromMemory" function. The stream can be from anyplace,
        /// not just a memory stream. It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream containing the scene data</param>
        /// <param name="flags">Post processing flags</param>
        /// <param name="formatHint">A hint to Assimp to decide which importer to use to process the data</param>
        /// <returns></returns>
        public static IntPtr ImportFileFromStream(Stream stream, PostProcessSteps flags, String formatHint) {
            byte[] buffer = MemoryHelper.ReadStreamFully(stream, 0);

            return aiImportFileFromMemory(buffer, (uint) buffer.Length, (uint) flags, formatHint);
        }

        /// <summary>
        /// Releases the unmanaged scene data structure.
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        [DllImport(AssimpDLL, EntryPoint = "aiReleaseImport", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseImport(IntPtr scene);

        [DllImport(AssimpDLL, EntryPoint = "aiApplyPostProcessing", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aiApplyPostProcessing(IntPtr scene, uint Flags);

        /// <summary>
        /// Applies a post-processing step on an already imported scene.
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        /// <param name="flags">Post processing steps to run.</param>
        /// <returns>Pointer to the unmanaged scene data structure.</returns>
        public static IntPtr ApplyPostProcessing(IntPtr scene, PostProcessSteps flags) {
            return aiApplyPostProcessing(scene, (uint) flags);
        }

        #endregion

        #region Export Methods

        [DllImport(AssimpDLL, EntryPoint = "aiGetExportFormatCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aiGetExportFormatCount();

        [DllImport(AssimpDLL, EntryPoint = "aiGetExportFormatDescription", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aiGetExportFormatDescription(IntPtr index);

        /// <summary>
        /// Gets all supported export formats.
        /// </summary>
        /// <returns>Array of supported export formats.</returns>
        public static ExportFormatDescription[] GetExportFormatDescriptions() {
            int count = aiGetExportFormatCount().ToInt32();

            if(count == 0)
                return new ExportFormatDescription[0];

            ExportFormatDescription[] descriptions = new ExportFormatDescription[count];

            for(int i = 0; i < count; i++) {
                IntPtr formatDescPtr = aiGetExportFormatDescription(new IntPtr(i));
                if(formatDescPtr != null) {
                    AiExportFormatDesc desc = MemoryHelper.MarshalStructure<AiExportFormatDesc>(formatDescPtr);
                    descriptions[i] = new ExportFormatDescription(ref desc);
                }
            }

            return descriptions;
        }

        [DllImport(AssimpDLL, EntryPoint = "aiExportSceneToBlob", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aiExportSceneToBlob(IntPtr scene, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String formatId, uint preProcessing);

        [DllImport(AssimpDLL, EntryPoint = "aiReleaseExportBlob", CallingConvention = CallingConvention.Cdecl)]
        private static extern void aiReleaseExportBlob(IntPtr blobData);

        /// <summary>
        /// Exports the given scene to a chosen file format. Returns the exported data as a binary blob which you can embed into another data structure or file.
        /// </summary>
        /// <param name="scene">Scene to export, it is the responsibility of the caller to free this when finished.</param>
        /// <param name="formatDescription">Export format description the scene is exported to.</param>
        /// <param name="preProcessing">Pre processing flags to operate on the scene during the export.</param>
        /// <returns>Exported binary blob, or null if there was an error.</returns>
        public static ExportDataBlob ExportSceneToBlob(IntPtr scene, ExportFormatDescription formatDescription, PostProcessSteps preProcessing) {
            if(formatDescription == null || scene == IntPtr.Zero)
                return null;

            IntPtr blobPtr = aiExportSceneToBlob(scene, formatDescription.FormatId, (uint) preProcessing);

            if(blobPtr == IntPtr.Zero)
                return null;

            AiExportDataBlob blob = MemoryHelper.MarshalStructure<AiExportDataBlob>(blobPtr);
            ExportDataBlob dataBlob = new ExportDataBlob(ref blob);
            aiReleaseExportBlob(blobPtr);

            return dataBlob;
        }

        [DllImport(AssimpDLL, EntryPoint = "aiExportScene", CallingConvention = CallingConvention.Cdecl)]
        private static extern ReturnCode aiExportScene(IntPtr scene, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String formatId, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String fileName, uint preProcessing);

        /// <summary>
        /// Exports the given scene to a chosen file format and writes the result file(s) to disk.
        /// </summary>
        /// <param name="scene">The scene to export, which needs to be freed by the caller. The scene is expected to conform to Assimp's Importer output format. In short,
        /// this means the model data should use a right handed coordinate system, face winding should be counter clockwise, and the UV coordinate origin assumed to be upper left. If the input is different, specify the pre processing flags appropiately.</param>
        /// <param name="formatDescription">Format description describing which format to export to.</param>
        /// <param name="fileName">Output filename to write to</param>
        /// <param name="preProcessing">Pre processing flags - accepts any post processing step flag. In reality only a small subset are actually supported, e.g. to ensure the input
        /// conforms to the standard Assimp output format. Some may be redundant, such as triangulation, which some exporters may have to enforce due to the export format.</param>
        /// <returns>Return code specifying if the operation was a success.</returns>
        public static ReturnCode ExportScene(IntPtr scene, ExportFormatDescription formatDescription, String fileName, PostProcessSteps preProcessing) {
            if(formatDescription == null || scene == IntPtr.Zero)
                return ReturnCode.Failure;

            return aiExportScene(scene, formatDescription.FormatId, fileName, (uint) preProcessing);
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Attaches a log stream callback to catch Assimp messages.
        /// </summary>
        /// <param name="stream">Logstream to attach</param>
        [DllImport(AssimpDLL, EntryPoint = "aiAttachLogStream", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AttachLogStream(ref AiLogStream stream);

        /// <summary>
        /// Enables verbose logging.
        /// </summary>
        /// <param name="enable">True if verbose logging is to be enabled or not.</param>
        [DllImport(AssimpDLL, EntryPoint = "aiEnableVerboseLogging", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableVerboseLogging([InAttribute()] [MarshalAs(UnmanagedType.Bool)] bool enable);

        /// <summary>
        /// Detaches a logstream callback.
        /// </summary>
        /// <param name="stream">Logstream to detach</param>
        /// <returns>A return code signifying if the function was successful or not.</returns>
        [DllImport(AssimpDLL, EntryPoint = "aiDetachLogStream", CallingConvention = CallingConvention.Cdecl)]
        public static extern ReturnCode DetachLogStream(ref AiLogStream stream);

        /// <summary>
        /// Detaches all logstream callbacks currently attached to Assimp.
        /// </summary>
        [DllImport(AssimpDLL, EntryPoint = "aiDetachAllLogStreams", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DetachAllLogStreams();

        #endregion

        #region Error and Info methods

        [DllImport(AssimpDLL, EntryPoint = "aiGetErrorString", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aiGetErrorString();

        /// <summary>
        /// Gets the last error logged in Assimp.
        /// </summary>
        /// <returns>The last error message logged.</returns>
        public static String GetErrorString() {
            return Marshal.PtrToStringAnsi(aiGetErrorString());
        }

        /// <summary>
        /// Checks whether the model format extension is supported by Assimp.
        /// </summary>
        /// <param name="extension">Model format extension, e.g. ".3ds"</param>
        /// <returns>True if the format is supported, false otherwise.</returns>
        [DllImport(AssimpDLL, EntryPoint = "aiIsExtensionSupported", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsExtensionSupported([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String extension);

        [DllImport(AssimpDLL, EntryPoint = "aiGetExtensionList", CallingConvention = CallingConvention.Cdecl)]
        private static extern void aiGetExtensionList(ref AiString extensionsOut);

        /// <summary>
        /// Gets all the model format extensions that are currently supported by Assimp.
        /// </summary>
        /// <returns>Array of supported format extensions</returns>
        public static String[] GetExtensionList() {
            AiString aiString = new AiString();
            aiGetExtensionList(ref aiString);
            return aiString.GetString().Split(new String[] { "*", ";*" }, StringSplitOptions.RemoveEmptyEntries);
        }

        [DllImport(AssimpDLL, EntryPoint = "aiGetMemoryRequirements", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetMemoryRequirements(IntPtr scene, ref AiMemoryInfo memoryInfo);

        /// <summary>
        /// Gets the memory requirements of the scene.
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        /// <returns>The memory information about the scene.</returns>
        public static AiMemoryInfo GetMemoryRequirements(IntPtr scene) {
            AiMemoryInfo info = new AiMemoryInfo();
            if(scene != IntPtr.Zero) {
                GetMemoryRequirements(scene, ref info);
            }
            return info;
        }

        #endregion

        #region Import Properties setters

        /// <summary>
        /// Create an empty property store. Property stores are used to collect import settings.
        /// </summary>
        /// <returns>Pointer to property store</returns>
        [DllImport(AssimpDLL, EntryPoint = "aiCreatePropertyStore", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreatePropertyStore();

        /// <summary>
        /// Deletes a property store.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        [DllImport(AssimpDLL, EntryPoint = "aiReleasePropertyStore", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleasePropertyStore(IntPtr propertyStore);

        /// <summary>
        /// Sets an integer property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        [DllImportAttribute(AssimpDLL, EntryPoint = "aiSetImportPropertyInteger", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetImportPropertyInteger(IntPtr propertyStore, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String name, int value);

        /// <summary>
        /// Sets a float property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        [DllImportAttribute(AssimpDLL, EntryPoint = "aiSetImportPropertyFloat", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetImportPropertyFloat(IntPtr propertyStore, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String name, float value);

        [DllImportAttribute(AssimpDLL, EntryPoint = "aiSetImportPropertyString", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetImportPropertyString(IntPtr propertyStore, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String name, ref AiString value);

        /// <summary>
        /// Sets a string property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public static void SetImportPropertyString(IntPtr propertyStore, String name, String value) {
            AiString str = new AiString();
            if(str.SetString(value))
                SetImportPropertyString(propertyStore, name, ref str);
        }

        #endregion

        #region Material getters

        
        [DllImportAttribute(AssimpDLL, EntryPoint = "aiGetMaterialProperty", CallingConvention = CallingConvention.Cdecl)]
        private static extern ReturnCode aiGetMaterialProperty(ref AiMaterial mat, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, out IntPtr propertyOut);

        /// <summary>
        /// Retrieves a material property with the specific key from the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the property from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The material property, if found.</returns>
        public static AiMaterialProperty GetMaterialProperty(ref AiMaterial mat, String key, TextureType texType, uint texIndex) {
            IntPtr ptr;
            ReturnCode code = aiGetMaterialProperty(ref mat, key, (uint)texType, texIndex, out ptr);
            AiMaterialProperty prop = new AiMaterialProperty();
            if(code == ReturnCode.Success && ptr != IntPtr.Zero) {
                prop = MemoryHelper.MarshalStructure<AiMaterialProperty>(ptr);
            }
            return prop;
        }

        [DllImportAttribute(AssimpDLL, EntryPoint="aiGetMaterialFloatArray", CallingConvention = CallingConvention.Cdecl)]
        private static extern ReturnCode aiGetMaterialFloatArray(ref AiMaterial mat, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr ptrOut, ref uint valueCount);

        /// <summary>
        /// Retrieves an array of float values with the specific key from the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <param name="floatCount">The maximum number of floats to read. This may not accurately describe the data returned, as it may not exist or be smaller. If this value is less than
        /// the available floats, then only the requested number is returned (e.g. 1 or 2 out of a 4 float array).</param>
        /// <returns>The float array, if it exists</returns>
        public static float[] GetMaterialFloatArray(ref AiMaterial mat, String key, TextureType texType, uint texIndex, uint floatCount) {
            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = Marshal.AllocHGlobal(IntPtr.Size);
                ReturnCode code = aiGetMaterialFloatArray(ref mat, key, (uint) texType, texIndex, ptr, ref floatCount);
                float[] array = null;
                if(code == ReturnCode.Success && floatCount > 0) {
                    array = new float[floatCount];
                    Marshal.Copy(ptr, array, 0, (int) floatCount);
                }
                return array;
            } finally {
                if(ptr != IntPtr.Zero) {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        [DllImportAttribute(AssimpDLL, EntryPoint="aiGetMaterialIntegerArray", CallingConvention = CallingConvention.Cdecl)]
        private static extern ReturnCode aiGetMaterialIntegerArray(ref AiMaterial mat, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr ptrOut, ref uint valueCount);

        /// <summary>
        /// Retrieves an array of integer values with the specific key from the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <param name="intCount">The maximum number of integers to read. This may not accurately describe the data returned, as it may not exist or be smaller. If this value is less than
        /// the available integers, then only the requested number is returned (e.g. 1 or 2 out of a 4 float array).</param>
        /// <returns>The integer array, if it exists</returns>
        public static int[] GetMaterialIntegerArray(ref AiMaterial mat, String key, TextureType texType, uint texIndex, uint intCount) {
            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = Marshal.AllocHGlobal(IntPtr.Size);
                ReturnCode code = aiGetMaterialIntegerArray(ref mat, key, (uint) texType, texIndex, ptr, ref intCount);
                int[] array = null;
                if(code == ReturnCode.Success && intCount > 0) {
                    array = new int[intCount];
                    Marshal.Copy(ptr, array, 0, (int) intCount);
                }
                return array;
            } finally {
                if(ptr != IntPtr.Zero) {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        [DllImport(AssimpDLL, EntryPoint = "aiGetMaterialColor", CallingConvention = CallingConvention.Cdecl)]
        private static extern ReturnCode aiGetMaterialColor(ref AiMaterial mat, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr colorOut);

        /// <summary>
        /// Retrieves a color value from the material property table.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The color if it exists. If not, the default Color4D value is returned.</returns>
        public static Color4D GetMaterialColor(ref AiMaterial mat, String key, TextureType texType, uint texIndex) {
            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Color4D)));
                aiGetMaterialColor(ref mat, key, (uint) texType, texIndex, ptr);
                Color4D color = new Color4D();
                if(ptr != IntPtr.Zero) {
                    color = MemoryHelper.MarshalStructure<Color4D>(ptr);
                }
                return color;
            } finally {
                if(ptr != IntPtr.Zero) {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        [DllImport(AssimpDLL, EntryPoint = "aiGetMaterialString", CallingConvention = CallingConvention.Cdecl)]
        private static extern ReturnCode aiGetMaterialString(ref AiMaterial mat, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, out AiString str);

        /// <summary>
        /// Retrieves a string from the material property table.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The string, if it exists. If not, an empty string is returned.</returns>
        public static String GetMaterialString(ref AiMaterial mat, String key, TextureType texType, uint texIndex) {
            AiString str;
            ReturnCode code = aiGetMaterialString(ref mat, key, (uint) texType, texIndex, out str);
            if(code == ReturnCode.Success) {
                return str.GetString();
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets the number of textures contained in the material for a particular texture type.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="type">Texture Type semantic</param>
        /// <returns>The number of textures for the type.</returns>
        [DllImport(AssimpDLL, EntryPoint = "aiGetMaterialTextureCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetMaterialTextureCount(ref AiMaterial mat, TextureType type);

        [DllImport(AssimpDLL, EntryPoint = "aiGetMaterialTexture", CallingConvention = CallingConvention.Cdecl)]
        private static extern ReturnCode aiGetMaterialTexture(ref AiMaterial mat, TextureType type, uint index, out AiString path, out TextureMapping mapping, out uint uvIndex, out float blendFactor, out TextureOperation textureOp, out TextureWrapMode wrapMode, out uint flags);

        /// <summary>
        /// Gets the texture filepath contained in the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="type">Texture type semantic</param>
        /// <param name="index">Texture index</param>
        /// <returns>The texture filepath, if it exists. If not an empty string is returned.</returns>
        public static String GetMaterialTextureFilePath(ref AiMaterial mat, TextureType type, uint index) {
            AiString str;
            TextureMapping mapping;
            uint uvIndex;
            float blendFactor;
            TextureOperation texOp;
            TextureWrapMode wrapMode;
            uint flags;
            ReturnCode code = aiGetMaterialTexture(ref mat, type, index, out str, out mapping, out uvIndex, out blendFactor, out texOp, out wrapMode, out flags);
            if(code == ReturnCode.Success) {
                return str.GetString();
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets all values pertaining to a particular texture from a material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="type">Texture type semantic</param>
        /// <param name="index">Texture index</param>
        /// <returns>Returns the texture slot struct containing all the information.</returns>
        public static TextureSlot GetMaterialTexture(ref AiMaterial mat, TextureType type, uint index) {
            AiString str;
            TextureMapping mapping;
            uint uvIndex;
            float blendFactor;
            TextureOperation texOp;
            TextureWrapMode wrapMode;
            uint flags;
            ReturnCode code = aiGetMaterialTexture(ref mat, type, index, out str, out mapping, out uvIndex, out blendFactor, out texOp, out wrapMode, out flags);
            return new TextureSlot(str.GetString(), type, index, mapping, uvIndex, blendFactor, texOp, wrapMode, flags);
        }
        
        #endregion

        #region Math methods

        /// <summary>
        /// Creates a quaternion from the 3x3 rotation matrix.
        /// </summary>
        /// <param name="quat">Quaternion struct to fill</param>
        /// <param name="mat">Rotation matrix</param>
        [DllImport(AssimpDLL, EntryPoint="aiCreateQuaternionFromMatrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateQuaternionFromMatrix(out Quaternion quat, ref Matrix3x3 mat);

        /// <summary>
        /// Decomposes a 4x4 matrix into its scaling, rotation, and translation parts.
        /// </summary>
        /// <param name="mat">4x4 Matrix to decompose</param>
        /// <param name="scaling">Scaling vector</param>
        /// <param name="rotation">Quaternion containing the rotation</param>
        /// <param name="position">Translation vector</param>
        [DllImport(AssimpDLL, EntryPoint="aiDecomposeMatrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecomposeMatrix(ref Matrix4x4 mat, out Vector3D scaling, out Quaternion rotation, out Vector3D position);

        /// <summary>
        /// Transposes the 4x4 matrix.
        /// </summary>
        /// <param name="mat">Matrix to transpose</param>
        [DllImportAttribute(AssimpDLL, EntryPoint="aiTransposeMatrix4", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TransposeMatrix4(ref Matrix4x4 mat);

        /// <summary>
        /// Transposes the 3x3 matrix.
        /// </summary>
        /// <param name="mat">Matrix to transpose</param>
        [DllImportAttribute(AssimpDLL, EntryPoint="aiTransposeMatrix3", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TransposeMatrix3(ref Matrix3x3 mat);

        /// <summary>
        /// Transforms the vector by the 3x3 rotation matrix.
        /// </summary>
        /// <param name="vec">Vector to transform</param>
        /// <param name="mat">Rotation matrix</param>
        [DllImportAttribute(AssimpDLL, EntryPoint="aiTransformVecByMatrix3", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TransformVecByMatrix3(ref Vector3D vec, ref Matrix3x3 mat);

        /// <summary>
        /// Transforms the vector by the 4x4 matrix.
        /// </summary>
        /// <param name="vec">Vector to transform</param>
        /// <param name="mat">Matrix transformation</param>
        [DllImportAttribute(AssimpDLL, EntryPoint="aiTransformVecByMatrix4", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TransformVecByMatrix4(ref Vector3D vec, ref Matrix4x4 mat);

        /// <summary>
        /// Multiplies two 4x4 matrices. The destination matrix receives the result.
        /// </summary>
        /// <param name="dst">First input matrix and is also the Matrix to receive the result</param>
        /// <param name="src">Second input matrix, to be multiplied with "dst".</param>
        [DllImportAttribute(AssimpDLL, EntryPoint="aiMultiplyMatrix4", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MultiplyMatrix4(ref Matrix4x4 dst, ref Matrix4x4 src);

        /// <summary>
        /// Multiplies two 3x3 matrices. The destination matrix receives the result.
        /// </summary>
        /// <param name="dst">First input matrix and is also the Matrix to receive the result</param>
        /// <param name="src">Second input matrix, to be multiplied with "dst".</param>
        [DllImportAttribute(AssimpDLL, EntryPoint="aiMultiplyMatrix3", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MultiplyMatrix3(ref Matrix3x3 dst, ref Matrix3x3 src);

        /// <summary>
        /// Creates a 3x3 identity matrix.
        /// </summary>
        /// <param name="mat">Matrix to hold the identity</param>
        [DllImportAttribute(AssimpDLL, EntryPoint="aiIdentityMatrix3", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IdentityMatrix3(out Matrix3x3 mat);

        /// <summary>
        /// Creates a 4x4 identity matrix.
        /// </summary>
        /// <param name="mat">Matrix to hold the identity</param>
        [DllImportAttribute(AssimpDLL, EntryPoint="aiIdentityMatrix4", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IdentityMatrix4(out Matrix4x4 mat);

        #endregion

        #region Version info

        [DllImport(AssimpDLL, EntryPoint="aiGetLegalString", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aiGetLegalString();

        /// <summary>
        /// Gets the Assimp legal info.
        /// </summary>
        /// <returns>String containing Assimp legal info.</returns>
        public static String GetLegalString() {
            return Marshal.PtrToStringAnsi(aiGetLegalString());
        }

        /// <summary>
        /// Gets the native Assimp DLL's minor version number.
        /// </summary>
        /// <returns></returns>
        [DllImport(AssimpDLL, EntryPoint="aiGetVersionMinor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetVersionMinor();

        /// <summary>
        /// Gets the native Assimp DLL's major version number.
        /// </summary>
        /// <returns>Assimp major version number</returns>
        [DllImport(AssimpDLL, EntryPoint="aiGetVersionMajor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetVersionMajor();

        /// <summary>
        /// Gets the native Assimp DLL's revision version number.
        /// </summary>
        /// <returns>Assimp revision version number</returns>
        [DllImport(AssimpDLL, EntryPoint="aiGetVersionRevision", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetVersionRevision();

        /// <summary>
        /// Gets the native Assimp DLL's current version number as "major.minor.revision" string. This is the
        /// version of Assimp that this wrapper is currently using.
        /// </summary>
        /// <returns></returns>
        public static String GetVersion() {
            uint major = GetVersionMajor();
            uint minor = GetVersionMinor();
            uint rev = GetVersionRevision();

            return String.Format("{0}.{1}.{2}", major.ToString(), minor.ToString(), rev.ToString());
        }

        [DllImport(AssimpDLL, EntryPoint="aiGetCompileFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint aiGetCompileFlags();

        /// <summary>
        /// Get the compilation flags that describe how the native Assimp DLL was compiled.
        /// </summary>
        /// <returns>Compilation flags</returns>
        public static CompileFlags GetCompileFlags() {
            return (CompileFlags) aiGetCompileFlags();
        }

        #endregion
    }
}
