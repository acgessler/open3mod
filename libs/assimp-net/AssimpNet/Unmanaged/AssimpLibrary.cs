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
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Assimp.Unmanaged {
    /// <summary>
    /// Singleton that governs access to the unmanaged Assimp library functions.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class AssimpLibrary {
        private static AssimpLibrary s_instance;
        private AssimpLibraryImplementation m_impl;
        private const String m_default32BitPath = "Assimp32.dll";
        private const String m_default64BitPath = "Assimp64.dll";
        private String m_libraryPath = "";

        /// <summary>
        /// Gets the AssimpLibrary instance.
        /// </summary>
        public static AssimpLibrary Instance {
            get {
                if(s_instance == null)
                    s_instance = new AssimpLibrary();
                return s_instance;
            }
        }

        /// <summary>
        /// Gets the default path used for loading the 32-bit version of the unmanaged Assimp DLL.
        /// </summary>
        public String DefaultLibraryPath32Bit {
            get {
                return m_default32BitPath;
            }
        }

        /// <summary>
        /// Gets the default path used for loading the 64-bit version of the unmanaged Assimp DLL.
        /// </summary>
        public String DefaultLibraryPath64Bit {
            get {
                return m_default64BitPath;
            }
        }

        /// <summary>
        /// Gets the current path to the unmanaged Assimp DLL that is loaded.
        /// </summary>
        public String LibraryPath {
            get {
                return m_libraryPath;
            }
        }

        /// <summary>
        /// Gets if the Assimp unmanaged library has been loaded or not.
        /// </summary>
        public bool LibraryLoaded {
            get {
                return m_impl != null && m_impl.LibraryLoaded;
            }
        }

        /// <summary>
        /// Private Constructor
        /// </summary>
        private AssimpLibrary() { }

        /// <summary>
        /// Loads the library using the default 32-bit or 64-bit DLL path, depending on the OS AssimpNet is running on.
        /// </summary>
        /// <exception cref="Assimp.AssimpException">Thrown if the library is already loaded or if there was an error in loading the library.</exception>
        public void LoadLibrary() {
            LoadLibrary(m_default32BitPath, m_default64BitPath);
        }

        /// <summary>
        /// Loads the Assimp unmanaged library using the DLL path specified in LibraryPath. If the library is already loaded, or there was an error
        /// in loading the library, an AssimpException is thrown.
        /// </summary>
        /// <param name="libPath">Path to the Assimp DLL.</param>
        /// <exception cref="Assimp.AssimpException">Thrown if the library is already loaded or if there was an error in loading the library.</exception>
        public void LoadLibrary(String libPath) {
            if(LibraryLoaded)
                throw new AssimpException("Assimp library already loaded.");

            AssimpLibraryImplementation impl = new AssimpLibraryImplementation();
            impl.LoadLibrary(libPath);

            m_impl = impl;
            m_libraryPath = libPath;
        }

        /// <summary>
        /// Convienence method that gives the AssimpLibrary paths to both a 32-bit and 64-bit path, letting the runtime choose between the two depending on the
        /// bitness of the process.
        /// </summary>
        /// <param name="lib32Path">Path to the 32 bit Assimp DLL</param>
        /// <param name="lib64Path">Path to the 64 bit Assimp DLL</param>
        public void LoadLibrary(String lib32Path, String lib64Path) {
            String libPath = (IntPtr.Size == 8) ? lib64Path : lib32Path;
            LoadLibrary(libPath);
        }

        /// <summary>
        /// Frees the Assimp unmanaged library.
        /// </summary>
        public void FreeLibrary() {
            if(m_impl != null) {
                m_impl.FreeLibrary();
                m_impl = null;
                m_libraryPath = String.Empty;
            }
        }

        private void LoadIfNotLoaded() {
            if(!LibraryLoaded)
                LoadLibrary();
        }

        #region Import Methods

        /// <summary>
        /// Imports a file.
        /// </summary>
        /// <param name="file">Valid filename</param>
        /// <param name="flags">Post process flags specifying what steps are to be run after the import.</param>
        /// <param name="propStore">Property store containing config name-values, may be null.</param>
        /// <returns>Pointer to the unmanaged data structure.</returns>
        public IntPtr ImportFile(String file, PostProcessSteps flags, IntPtr propStore) {
            return ImportFile(file, flags, IntPtr.Zero, propStore);
        }

        /// <summary>
        /// Imports a file.
        /// </summary>
        /// <param name="file">Valid filename</param>
        /// <param name="flags">Post process flags specifying what steps are to be run after the import.</param>
        /// <param name="fileIO">Pointer to an instance of AiFileIO, a custom file IO system used to open the model and 
        /// any associated file the loader needs to open, passing NULL uses the default implementation.</param>
        /// <param name="propStore">Property store containing config name-values, may be null.</param>
        /// <returns>Pointer to the unmanaged data structure.</returns>
        public IntPtr ImportFile(String file, PostProcessSteps flags, IntPtr fileIO, IntPtr propStore) {
            LoadIfNotLoaded();

            AssimpDelegates.aiImportFileExWithProperties func = m_impl.GetFunction<AssimpDelegates.aiImportFileExWithProperties>("aiImportFileExWithProperties");

            return func(file, (uint) flags, fileIO, propStore);
        }

        /// <summary>
        /// Imports a scene from a stream. This uses the "aiImportFileFromMemory" function. The stream can be from anyplace,
        /// not just a memory stream. It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream containing the scene data</param>
        /// <param name="flags">Post processing flags</param>
        /// <param name="formatHint">A hint to Assimp to decide which importer to use to process the data</param>
        /// <param name="propStore">Property store containing the config name-values, may be null.</param>
        /// <returns></returns>
        public IntPtr ImportFileFromStream(Stream stream, PostProcessSteps flags, String formatHint, IntPtr propStore) {
            LoadIfNotLoaded();

            AssimpDelegates.aiImportFileFromMemoryWithProperties func = m_impl.GetFunction<AssimpDelegates.aiImportFileFromMemoryWithProperties>("aiImportFileFromMemoryWithProperties");

            byte[] buffer = MemoryHelper.ReadStreamFully(stream, 0);

            return func(buffer, (uint) buffer.Length, (uint) flags, formatHint, propStore);
        }

        /// <summary>
        /// Releases the unmanaged scene data structure. This should NOT be used for unmanaged scenes that were marshaled
        /// from the managed scene structure - only for scenes whose memory was allocated by the native library!
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        public void ReleaseImport(IntPtr scene) {
            LoadIfNotLoaded();

            if(scene == IntPtr.Zero)
                return;

            AssimpDelegates.aiReleaseImport func = m_impl.GetFunction<AssimpDelegates.aiReleaseImport>("aiReleaseImport");

            func(scene);
        }

        /// <summary>
        /// Applies a post-processing step on an already imported scene.
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        /// <param name="flags">Post processing steps to run.</param>
        /// <returns>Pointer to the unmanaged scene data structure.</returns>
        public IntPtr ApplyPostProcessing(IntPtr scene, PostProcessSteps flags) {
            LoadIfNotLoaded();

            if(scene == IntPtr.Zero)
                return IntPtr.Zero;

            AssimpDelegates.aiApplyPostProcessing func = m_impl.GetFunction<AssimpDelegates.aiApplyPostProcessing>("aiApplyPostProcessing");

            return func(scene, (uint) flags);
        }

        #endregion

        #region Export Methods

        /// <summary>
        /// Gets all supported export formats.
        /// </summary>
        /// <returns>Array of supported export formats.</returns>
        public ExportFormatDescription[] GetExportFormatDescriptions() {
            LoadIfNotLoaded();

            int count = m_impl.GetFunction<AssimpDelegates.aiGetExportFormatCount>("aiGetExportFormatCount")().ToInt32();

            if(count == 0)
                return new ExportFormatDescription[0];

            ExportFormatDescription[] descriptions = new ExportFormatDescription[count];

            AssimpDelegates.aiGetExportFormatDescription func = m_impl.GetFunction<AssimpDelegates.aiGetExportFormatDescription>("aiGetExportFormatDescription");

            for(int i = 0; i < count; i++) {
                IntPtr formatDescPtr = func(new IntPtr(i));
                if(formatDescPtr != null) {
                    AiExportFormatDesc desc = MemoryHelper.MarshalStructure<AiExportFormatDesc>(formatDescPtr);
                    descriptions[i] = new ExportFormatDescription(ref desc);
                }
            }

            return descriptions;
        }


        /// <summary>
        /// Exports the given scene to a chosen file format. Returns the exported data as a binary blob which you can embed into another data structure or file.
        /// </summary>
        /// <param name="scene">Scene to export, it is the responsibility of the caller to free this when finished.</param>
        /// <param name="formatId">Format id describing which format to export to.</param>
        /// <param name="preProcessing">Pre processing flags to operate on the scene during the export.</param>
        /// <returns>Exported binary blob, or null if there was an error.</returns>
        public ExportDataBlob ExportSceneToBlob(IntPtr scene, String formatId, PostProcessSteps preProcessing) {
            LoadIfNotLoaded();

            if(String.IsNullOrEmpty(formatId) || scene == IntPtr.Zero)
                return null;

            AssimpDelegates.aiExportSceneToBlob exportBlobFunc = m_impl.GetFunction<AssimpDelegates.aiExportSceneToBlob>("aiExportSceneToBlob");
            AssimpDelegates.aiReleaseExportBlob releaseExportBlobFunc = m_impl.GetFunction<AssimpDelegates.aiReleaseExportBlob>("aiReleaseExportBlob");

            IntPtr blobPtr = exportBlobFunc(scene, formatId, (uint) preProcessing);

            if(blobPtr == IntPtr.Zero)
                return null;

            AiExportDataBlob blob = MemoryHelper.MarshalStructure<AiExportDataBlob>(blobPtr);
            ExportDataBlob dataBlob = new ExportDataBlob(ref blob);
            releaseExportBlobFunc(blobPtr);

            return dataBlob;
        }

        /// <summary>
        /// Exports the given scene to a chosen file format and writes the result file(s) to disk.
        /// </summary>
        /// <param name="scene">The scene to export, which needs to be freed by the caller. The scene is expected to conform to Assimp's Importer output format. In short,
        /// this means the model data should use a right handed coordinate system, face winding should be counter clockwise, and the UV coordinate origin assumed to be upper left. If the input is different, specify the pre processing flags appropiately.</param>
        /// <param name="formatId">Format id describing which format to export to.</param>
        /// <param name="fileName">Output filename to write to</param>
        /// <param name="preProcessing">Pre processing flags - accepts any post processing step flag. In reality only a small subset are actually supported, e.g. to ensure the input
        /// conforms to the standard Assimp output format. Some may be redundant, such as triangulation, which some exporters may have to enforce due to the export format.</param>
        /// <returns>Return code specifying if the operation was a success.</returns>
        public ReturnCode ExportScene(IntPtr scene, String formatId, String fileName, PostProcessSteps preProcessing) {
            return ExportScene(scene, formatId, fileName, IntPtr.Zero, preProcessing);
        }

        /// <summary>
        /// Exports the given scene to a chosen file format and writes the result file(s) to disk.
        /// </summary>
        /// <param name="scene">The scene to export, which needs to be freed by the caller. The scene is expected to conform to Assimp's Importer output format. In short,
        /// this means the model data should use a right handed coordinate system, face winding should be counter clockwise, and the UV coordinate origin assumed to be upper left. If the input is different, specify the pre processing flags appropiately.</param>
        /// <param name="formatId">Format id describing which format to export to.</param>
        /// <param name="fileName">Output filename to write to</param>
        /// <param name="fileIO">Pointer to an instance of AiFileIO, a custom file IO system used to open the model and 
        /// any associated file the loader needs to open, passing NULL uses the default implementation.</param>
        /// <param name="preProcessing">Pre processing flags - accepts any post processing step flag. In reality only a small subset are actually supported, e.g. to ensure the input
        /// conforms to the standard Assimp output format. Some may be redundant, such as triangulation, which some exporters may have to enforce due to the export format.</param>
        /// <returns>Return code specifying if the operation was a success.</returns>
        public ReturnCode ExportScene(IntPtr scene, String formatId, String fileName, IntPtr fileIO, PostProcessSteps preProcessing) {
            LoadIfNotLoaded();

            if(String.IsNullOrEmpty(formatId) || scene == IntPtr.Zero)
                return ReturnCode.Failure;

            AssimpDelegates.aiExportSceneEx exportFunc = m_impl.GetFunction<AssimpDelegates.aiExportSceneEx>("aiExportSceneEx");

            return exportFunc(scene, formatId, fileName, fileIO, (uint) preProcessing);
        }

        /// <summary>
        /// Creates a modifyable copy of a scene, useful for copying the scene that was imported so its topology can be modified
        /// and the scene be exported.
        /// </summary>
        /// <param name="sceneToCopy">Valid scene to be copied</param>
        /// <returns>Modifyable copy of the scene</returns>
        public IntPtr CopyScene(IntPtr sceneToCopy) {
            if(sceneToCopy == IntPtr.Zero)
                return IntPtr.Zero;

            IntPtr copiedScene;

            AssimpDelegates.aiCopyScene func = m_impl.GetFunction<AssimpDelegates.aiCopyScene>("aiCopyScene");

            func(sceneToCopy, out copiedScene);

            return copiedScene;
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Attaches a log stream callback to catch Assimp messages.
        /// </summary>
        /// <param name="logStreamPtr">Pointer to an instance of AiLogStream.</param>
        public void AttachLogStream(IntPtr logStreamPtr) {
            LoadIfNotLoaded();

            AssimpDelegates.aiAttachLogStream func = m_impl.GetFunction<AssimpDelegates.aiAttachLogStream>("aiAttachLogStream");

            func(logStreamPtr);
        }

        /// <summary>
        /// Enables verbose logging.
        /// </summary>
        /// <param name="enable">True if verbose logging is to be enabled or not.</param>
        public void EnableVerboseLogging(bool enable) {
            LoadIfNotLoaded();

            AssimpDelegates.aiEnableVerboseLogging func = m_impl.GetFunction<AssimpDelegates.aiEnableVerboseLogging>("aiEnableVerboseLogging");

            func(enable);
        }

        /// <summary>
        /// Detaches a logstream callback.
        /// </summary>
        /// <param name="logStreamPtr">Pointer to an instance of AiLogStream.</param>
        /// <returns>A return code signifying if the function was successful or not.</returns>
        public ReturnCode DetachLogStream(IntPtr logStreamPtr) {
            LoadIfNotLoaded();

            AssimpDelegates.aiDetachLogStream func = m_impl.GetFunction<AssimpDelegates.aiDetachLogStream>("aiDetachLogStream");

            return func(logStreamPtr);
        }

        /// <summary>
        /// Detaches all logstream callbacks currently attached to Assimp.
        /// </summary>
        public void DetachAllLogStreams() {
            LoadIfNotLoaded();

            AssimpDelegates.aiDetachAllLogStreams func = m_impl.GetFunction<AssimpDelegates.aiDetachAllLogStreams>("aiDetachAllLogStreams");

            func();
        }

        #endregion

        #region Import Properties Setters

        /// <summary>
        /// Create an empty property store. Property stores are used to collect import settings.
        /// </summary>
        /// <returns>Pointer to property store</returns>
        public IntPtr CreatePropertyStore() {
            LoadIfNotLoaded();

            AssimpDelegates.aiCreatePropertyStore func = m_impl.GetFunction<AssimpDelegates.aiCreatePropertyStore>("aiCreatePropertyStore");

            return func();
        }

        /// <summary>
        /// Deletes a property store.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        public void ReleasePropertyStore(IntPtr propertyStore) {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero)
                return;

            AssimpDelegates.aiReleasePropertyStore func = m_impl.GetFunction<AssimpDelegates.aiReleasePropertyStore>("aiReleasePropertyStore");

            func(propertyStore);
        }

        /// <summary>
        /// Sets an integer property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public void SetImportPropertyInteger(IntPtr propertyStore, String name, int value) {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero || String.IsNullOrEmpty(name))
                return;

            AssimpDelegates.aiSetImportPropertyInteger func = m_impl.GetFunction<AssimpDelegates.aiSetImportPropertyInteger>("aiSetImportPropertyInteger");

            func(propertyStore, name, value);
        }

        /// <summary>
        /// Sets a float property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public void SetImportPropertyFloat(IntPtr propertyStore, String name, float value) {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero || String.IsNullOrEmpty(name))
                return;

            AssimpDelegates.aiSetImportPropertyFloat func = m_impl.GetFunction<AssimpDelegates.aiSetImportPropertyFloat>("aiSetImportPropertyFloat");

            func(propertyStore, name, value);
        }

        /// <summary>
        /// Sets a string property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public void SetImportPropertyString(IntPtr propertyStore, String name, String value) {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero || String.IsNullOrEmpty(name))
                return;

            AssimpDelegates.aiSetImportPropertyString func = m_impl.GetFunction<AssimpDelegates.aiSetImportPropertyString>("aiSetImportPropertyString");

            AiString str = new AiString();
            if(str.SetString(value))
                func(propertyStore, name, ref str);
        }

        #endregion

        #region Material Getters

        /// <summary>
        /// Retrieves a color value from the material property table.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The color if it exists. If not, the default Color4D value is returned.</returns>
        public Color4D GetMaterialColor(ref AiMaterial mat, String key, TextureType texType, uint texIndex) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMaterialColor func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialColor>("aiGetMaterialColor");

            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = MemoryHelper.AllocateMemory(MemoryHelper.SizeOf<Color4D>());
                ReturnCode code = func(ref mat, key, (uint) texType, texIndex, ptr);
                Color4D color = new Color4D();
                if(code == ReturnCode.Success && ptr != IntPtr.Zero) {
                    color = MemoryHelper.MarshalStructure<Color4D>(ptr);
                }
                return color;
            } finally {
                if(ptr != IntPtr.Zero) {
                    MemoryHelper.FreeMemory(ptr);
                }
            }
        }

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
        public float[] GetMaterialFloatArray(ref AiMaterial mat, String key, TextureType texType, uint texIndex, uint floatCount) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMaterialFloatArray func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialFloatArray>("aiGetMaterialFloatArray");

            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = MemoryHelper.AllocateMemory(IntPtr.Size);
                ReturnCode code = func(ref mat, key, (uint) texType, texIndex, ptr, ref floatCount);
                float[] array = null;
                if(code == ReturnCode.Success && floatCount > 0) {
                    array = new float[floatCount];
                    MemoryHelper.Read<float>(ptr, array, 0, (int) floatCount);
                }
                return array;
            } finally {
                if(ptr != IntPtr.Zero) {
                    MemoryHelper.FreeMemory(ptr);
                }
            }
        }

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
        public int[] GetMaterialIntegerArray(ref AiMaterial mat, String key, TextureType texType, uint texIndex, uint intCount) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMaterialIntegerArray func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialIntegerArray>("aiGetMaterialIntegerArray");

            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = MemoryHelper.AllocateMemory(IntPtr.Size);
                ReturnCode code = func(ref mat, key, (uint) texType, texIndex, ptr, ref intCount);
                int[] array = null;
                if(code == ReturnCode.Success && intCount > 0) {
                    array = new int[intCount];
                    MemoryHelper.Read<int>(ptr, array, 0, (int) intCount);
                }
                return array;
            } finally {
                if(ptr != IntPtr.Zero) {
                    MemoryHelper.FreeMemory(ptr);
                }
            }
        }

        /// <summary>
        /// Retrieves a material property with the specific key from the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the property from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The material property, if found.</returns>
        public AiMaterialProperty GetMaterialProperty(ref AiMaterial mat, String key, TextureType texType, uint texIndex) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMaterialProperty func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialProperty>("aiGetMaterialProperty");

            IntPtr ptr;
            ReturnCode code = func(ref mat, key, (uint) texType, texIndex, out ptr);
            AiMaterialProperty prop = new AiMaterialProperty();
            if(code == ReturnCode.Success && ptr != IntPtr.Zero) {
                prop = MemoryHelper.MarshalStructure<AiMaterialProperty>(ptr);
            }
            return prop;
        }

        /// <summary>
        /// Retrieves a string from the material property table.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The string, if it exists. If not, an empty string is returned.</returns>
        public String GetMaterialString(ref AiMaterial mat, String key, TextureType texType, uint texIndex) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMaterialString func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialString>("aiGetMaterialString");

            AiString str;
            ReturnCode code = func(ref mat, key, (uint) texType, texIndex, out str);
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
        public uint GetMaterialTextureCount(ref AiMaterial mat, TextureType type) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMaterialTextureCount func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialTextureCount>("aiGetMaterialTextureCount");

            return func(ref mat, type);
        }

        /// <summary>
        /// Gets the texture filepath contained in the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="type">Texture type semantic</param>
        /// <param name="index">Texture index</param>
        /// <returns>The texture filepath, if it exists. If not an empty string is returned.</returns>
        public String GetMaterialTextureFilePath(ref AiMaterial mat, TextureType type, uint index) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMaterialTexture func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialTexture>("aiGetMaterialTexture");

            AiString str;
            TextureMapping mapping;
            uint uvIndex;
            float blendFactor;
            TextureOperation texOp;
            TextureWrapMode[] wrapModes = new TextureWrapMode[2];
            uint flags;

            ReturnCode code = func(ref mat, type, index, out str, out mapping, out uvIndex, out blendFactor, out texOp, wrapModes, out flags);
            
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
        public TextureSlot GetMaterialTexture(ref AiMaterial mat, TextureType type, uint index) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMaterialTexture func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialTexture>("aiGetMaterialTexture");

            AiString str;
            TextureMapping mapping;
            uint uvIndex;
            float blendFactor;
            TextureOperation texOp;
            TextureWrapMode[] wrapModes = new TextureWrapMode[2];
            uint flags;

            ReturnCode code = func(ref mat, type, index, out str, out mapping, out uvIndex, out blendFactor, out texOp, wrapModes, out flags);
            
            return new TextureSlot(str.GetString(), type, (int) index, mapping, (int) uvIndex, blendFactor, texOp, wrapModes[0], wrapModes[1], (int) flags);
        }

        #endregion

        #region Error and Info Methods

        /// <summary>
        /// Gets the last error logged in Assimp.
        /// </summary>
        /// <returns>The last error message logged.</returns>
        public String GetErrorString() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetErrorString func = m_impl.GetFunction<AssimpDelegates.aiGetErrorString>("aiGetErrorString");

            IntPtr ptr = func();

            if(ptr == IntPtr.Zero)
                return String.Empty;

            return Marshal.PtrToStringAnsi(ptr);
        }

                /// <summary>
        /// Checks whether the model format extension is supported by Assimp.
        /// </summary>
        /// <param name="extension">Model format extension, e.g. ".3ds"</param>
        /// <returns>True if the format is supported, false otherwise.</returns>
        public bool IsExtensionSupported(String extension) {
            LoadIfNotLoaded();

            AssimpDelegates.aiIsExtensionSupported func = m_impl.GetFunction<AssimpDelegates.aiIsExtensionSupported>("aiIsExtensionSupported");

            return func(extension);
        }

        /// <summary>
        /// Gets all the model format extensions that are currently supported by Assimp.
        /// </summary>
        /// <returns>Array of supported format extensions</returns>
        public String[] GetExtensionList() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetExtensionList func = m_impl.GetFunction<AssimpDelegates.aiGetExtensionList>("aiGetExtensionList");

            AiString aiString = new AiString();
            func(ref aiString);
            return aiString.GetString().Split(new String[] { "*", ";*" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the memory requirements of the scene.
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        /// <returns>The memory information about the scene.</returns>
        public AiMemoryInfo GetMemoryRequirements(IntPtr scene) {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetMemoryRequirements func = m_impl.GetFunction <AssimpDelegates.aiGetMemoryRequirements>("aiGetMemoryRequirements");

            AiMemoryInfo info = new AiMemoryInfo();
            if(scene != IntPtr.Zero) {
                func(scene, ref info);
            }

            return info;
        }

        #endregion

        #region Math Methods

        /// <summary>
        /// Creates a quaternion from the 3x3 rotation matrix.
        /// </summary>
        /// <param name="quat">Quaternion struct to fill</param>
        /// <param name="mat">Rotation matrix</param>
        public void CreateQuaternionFromMatrix(out Quaternion quat, ref Matrix3x3 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiCreateQuaternionFromMatrix func = m_impl.GetFunction<AssimpDelegates.aiCreateQuaternionFromMatrix>("aiCreateQuaternionFromMatrix");

            func(out quat, ref mat);
        }

        /// <summary>
        /// Decomposes a 4x4 matrix into its scaling, rotation, and translation parts.
        /// </summary>
        /// <param name="mat">4x4 Matrix to decompose</param>
        /// <param name="scaling">Scaling vector</param>
        /// <param name="rotation">Quaternion containing the rotation</param>
        /// <param name="position">Translation vector</param>
        public void DecomposeMatrix(ref Matrix4x4 mat, out Vector3D scaling, out Quaternion rotation, out Vector3D position) {
            LoadIfNotLoaded();

            AssimpDelegates.aiDecomposeMatrix func = m_impl.GetFunction<AssimpDelegates.aiDecomposeMatrix>("aiDecomposeMatrix");

            func(ref mat, out scaling, out rotation, out position);
        }

        /// <summary>
        /// Transposes the 4x4 matrix.
        /// </summary>
        /// <param name="mat">Matrix to transpose</param>
        public void TransposeMatrix4(ref Matrix4x4 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiTransposeMatrix4 func = m_impl.GetFunction<AssimpDelegates.aiTransposeMatrix4>("aiTransposeMatrix4");

            func(ref mat);
        }

        /// <summary>
        /// Transposes the 3x3 matrix.
        /// </summary>
        /// <param name="mat">Matrix to transpose</param>
        public void TransposeMatrix3(ref Matrix3x3 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiTransposeMatrix3 func = m_impl.GetFunction<AssimpDelegates.aiTransposeMatrix3>("aiTransposeMatrix3");

            func(ref mat);
        }

        /// <summary>
        /// Transforms the vector by the 3x3 rotation matrix.
        /// </summary>
        /// <param name="vec">Vector to transform</param>
        /// <param name="mat">Rotation matrix</param>
        public void TransformVecByMatrix3(ref Vector3D vec, ref Matrix3x3 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiTransformVecByMatrix3 func = m_impl.GetFunction<AssimpDelegates.aiTransformVecByMatrix3>("aiTransformVecByMatrix3");

            func(ref vec, ref mat);
        }

        /// <summary>
        /// Transforms the vector by the 4x4 matrix.
        /// </summary>
        /// <param name="vec">Vector to transform</param>
        /// <param name="mat">Matrix transformation</param>
        public void TransformVecByMatrix4(ref Vector3D vec, ref Matrix4x4 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiTransformVecByMatrix4 func = m_impl.GetFunction<AssimpDelegates.aiTransformVecByMatrix4>("aiTransformVecByMatrix4");

            func(ref vec, ref mat);
        }

        /// <summary>
        /// Multiplies two 4x4 matrices. The destination matrix receives the result.
        /// </summary>
        /// <param name="dst">First input matrix and is also the Matrix to receive the result</param>
        /// <param name="src">Second input matrix, to be multiplied with "dst".</param>
        public void MultiplyMatrix4(ref Matrix4x4 dst, ref Matrix4x4 src) {
            LoadIfNotLoaded();

            AssimpDelegates.aiMultiplyMatrix4 func = m_impl.GetFunction<AssimpDelegates.aiMultiplyMatrix4>("aiMultiplyMatrix4");

            func(ref dst, ref src);
        }

        /// <summary>
        /// Multiplies two 3x3 matrices. The destination matrix receives the result.
        /// </summary>
        /// <param name="dst">First input matrix and is also the Matrix to receive the result</param>
        /// <param name="src">Second input matrix, to be multiplied with "dst".</param>
        public void MultiplyMatrix3(ref Matrix3x3 dst, ref Matrix3x3 src) {
            LoadIfNotLoaded();

            AssimpDelegates.aiMultiplyMatrix3 func = m_impl.GetFunction<AssimpDelegates.aiMultiplyMatrix3>("aiMultiplyMatrix3");

            func(ref dst, ref src);
        }

        /// <summary>
        /// Creates a 3x3 identity matrix.
        /// </summary>
        /// <param name="mat">Matrix to hold the identity</param>
        public void IdentityMatrix3(out Matrix3x3 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiIdentityMatrix3 func = m_impl.GetFunction<AssimpDelegates.aiIdentityMatrix3>("aiIdentityMatrix3");

            func(out mat);
        }

        /// <summary>
        /// Creates a 4x4 identity matrix.
        /// </summary>
        /// <param name="mat">Matrix to hold the identity</param>
        public void IdentityMatrix4(out Matrix4x4 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiIdentityMatrix4 func = m_impl.GetFunction<AssimpDelegates.aiIdentityMatrix4>("aiIdentityMatrix4");

            func(out mat);
        }

        #endregion

        #region Version Info

        /// <summary>
        /// Gets the Assimp legal info.
        /// </summary>
        /// <returns>String containing Assimp legal info.</returns>
        public String GetLegalString() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetLegalString func = m_impl.GetFunction<AssimpDelegates.aiGetLegalString>("aiGetLegalString");

            IntPtr ptr = func();

            if(ptr == IntPtr.Zero)
                return String.Empty;

            return Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Gets the native Assimp DLL's minor version number.
        /// </summary>
        /// <returns></returns>
        public uint GetVersionMinor() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetVersionMinor func = m_impl.GetFunction<AssimpDelegates.aiGetVersionMinor>("aiGetVersionMinor");

            return func();
        }

        /// <summary>
        /// Gets the native Assimp DLL's major version number.
        /// </summary>
        /// <returns>Assimp major version number</returns>
        public uint GetVersionMajor() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetVersionMajor func = m_impl.GetFunction<AssimpDelegates.aiGetVersionMajor>("aiGetVersionMajor");

            return func();
        }

        /// <summary>
        /// Gets the native Assimp DLL's revision version number.
        /// </summary>
        /// <returns>Assimp revision version number</returns>
        public uint GetVersionRevision() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetVersionRevision func = m_impl.GetFunction<AssimpDelegates.aiGetVersionRevision>("aiGetVersionRevision");

            return func();
        }

        /// <summary>
        /// Gets the native Assimp DLL's current version number as "major.minor.revision" string. This is the
        /// version of Assimp that this wrapper is currently using.
        /// </summary>
        /// <returns></returns>
        public String GetVersion() {
            uint major = GetVersionMajor();
            uint minor = GetVersionMinor();
            uint rev = GetVersionRevision();

            return String.Format("{0}.{1}.{2}", major.ToString(), minor.ToString(), rev.ToString());
        }

        /// <summary>
        /// Get the compilation flags that describe how the native Assimp DLL was compiled.
        /// </summary>
        /// <returns>Compilation flags</returns>
        public CompileFlags GetCompileFlags() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetCompileFlags func = m_impl.GetFunction<AssimpDelegates.aiGetCompileFlags>("aiGetCompileFlags");

            return (CompileFlags) func();
        }

        #endregion
    }

    internal static class AssimpDelegates {

        #region Import Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiImportFile([In, MarshalAs(UnmanagedType.LPStr)] String file, uint flags);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiImportFileEx([In, MarshalAs(UnmanagedType.LPStr)] String file, uint flags, IntPtr fileIO);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiImportFileExWithProperties([In, MarshalAs(UnmanagedType.LPStr)] String file, uint flag, IntPtr fileIO, IntPtr propStore);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiImportFileFromMemory(byte[] buffer, uint bufferLength, uint flags, [In, MarshalAs(UnmanagedType.LPStr)] String formatHint);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiImportFileFromMemoryWithProperties(byte[] buffer, uint bufferLength, uint flags, [In, MarshalAs(UnmanagedType.LPStr)] String formatHint, IntPtr propStore);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiReleaseImport(IntPtr scene);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiApplyPostProcessing(IntPtr scene, uint Flags);

        #endregion

        #region Export Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiGetExportFormatCount();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiGetExportFormatDescription(IntPtr index);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiExportSceneToBlob(IntPtr scene, [In, MarshalAs(UnmanagedType.LPStr)] String formatId, uint preProcessing);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiReleaseExportBlob(IntPtr blobData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiExportScene(IntPtr scene, [In, MarshalAs(UnmanagedType.LPStr)] String formatId, [In, MarshalAs(UnmanagedType.LPStr)] String fileName, uint preProcessing);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiExportSceneEx(IntPtr scene, [In, MarshalAs(UnmanagedType.LPStr)] String formatId, [In, MarshalAs(UnmanagedType.LPStr)] String fileName, IntPtr fileIO, uint preProcessing);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiCopyScene(IntPtr sceneIn, out IntPtr sceneOut);

        #endregion

        #region Logging Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiAttachLogStream(IntPtr logStreamPtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiEnableVerboseLogging([In, MarshalAs(UnmanagedType.Bool)] bool enable);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiDetachLogStream(IntPtr logStreamPtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiDetachAllLogStreams();

        #endregion

        #region Property Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiCreatePropertyStore();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiReleasePropertyStore(IntPtr propertyStore);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiSetImportPropertyInteger(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPStr)] String name, int value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiSetImportPropertyFloat(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPStr)] String name, float value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiSetImportPropertyString(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPStr)] String name, ref AiString value);

        #endregion

        #region Material Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiGetMaterialColor(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr colorOut);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiGetMaterialFloatArray(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr ptrOut, ref uint valueCount);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiGetMaterialIntegerArray(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr ptrOut, ref uint valueCount);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiGetMaterialProperty(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, out IntPtr propertyOut);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiGetMaterialString(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, out AiString str);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ReturnCode aiGetMaterialTexture(ref AiMaterial mat, TextureType type, uint index, out AiString path, out TextureMapping mapping, out uint uvIndex, out float blendFactor, out TextureOperation textureOp, [In, Out] TextureWrapMode[] wrapModes, out uint flags);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint aiGetMaterialTextureCount(ref AiMaterial mat, TextureType type);

        #endregion

        #region Math Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiCreateQuaternionFromMatrix(out Quaternion quat, ref Matrix3x3 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiDecomposeMatrix(ref Matrix4x4 mat, out Vector3D scaling, out Quaternion rotation, out Vector3D position);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiTransposeMatrix4(ref Matrix4x4 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiTransposeMatrix3(ref Matrix3x3 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiTransformVecByMatrix3(ref Vector3D vec, ref Matrix3x3 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiTransformVecByMatrix4(ref Vector3D vec, ref Matrix4x4 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiMultiplyMatrix4(ref Matrix4x4 dst, ref Matrix4x4 src);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiMultiplyMatrix3(ref Matrix3x3 dst, ref Matrix3x3 src);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiIdentityMatrix3(out Matrix3x3 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiIdentityMatrix4(out Matrix4x4 mat);

        #endregion

        #region Error and Info Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiGetErrorString();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiGetExtensionList(ref AiString extensionsOut);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void aiGetMemoryRequirements(IntPtr scene, ref AiMemoryInfo memoryInfo);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool aiIsExtensionSupported([In, MarshalAs(UnmanagedType.LPStr)] String extension);

        #endregion

        #region Version Info Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr aiGetLegalString();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint aiGetVersionMinor();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint aiGetVersionMajor();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint aiGetVersionRevision();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint aiGetCompileFlags();

        #endregion
    }

    internal sealed class AssimpLibraryImplementation : IDisposable {
        private Dictionary<String, Delegate> m_nameToUnmanagedFunction;
        private IntPtr m_libraryHandle;
        private bool m_isDisposed;

        public AssimpLibraryImplementation() {
            m_libraryHandle = IntPtr.Zero;
            m_nameToUnmanagedFunction = new Dictionary<String, Delegate>();
            m_isDisposed = false;
        }

        ~AssimpLibraryImplementation() {
            Dispose(false);
        }

        public bool LibraryLoaded {
            get {
                return m_libraryHandle != IntPtr.Zero;
            }
        }

        public bool IsDisposed {
            get {
                return m_isDisposed;
            }
        }

        public void LoadLibrary(String path) {
            FreeLibrary(true);
            m_libraryHandle = NativeMethods.LoadLibrary(path);

            if(m_libraryHandle == IntPtr.Zero) {
                int hr = Marshal.GetHRForLastWin32Error();
                Exception innerException = Marshal.GetExceptionForHR(hr);
                if(innerException != null)
                    throw new AssimpException("Error loading unmanaged library from path: " + path + ", see inner exception for details.\n" + innerException.Message, innerException);
                else
                    throw new AssimpException("Error loading unmanaged library from path: " + path);
            }
        }

        public void FreeLibrary() {
            FreeLibrary(true);
        }

        private void FreeLibrary(bool clearFunctions) {
            if(m_libraryHandle != IntPtr.Zero) {
                NativeMethods.FreeLibrary(m_libraryHandle);
                m_libraryHandle = IntPtr.Zero;

                if(clearFunctions)
                    m_nameToUnmanagedFunction.Clear();
            }
        }

        public T GetFunction<T>(String functionName) where T : class {
            if(String.IsNullOrEmpty(functionName) || !LibraryLoaded)
                return null;

            if(IsDisposed)
                throw new ObjectDisposedException("AssimpLibrary has been disposed");

            IntPtr procAddr = NativeMethods.GetProcAddress(m_libraryHandle, functionName);

            if(procAddr == IntPtr.Zero)
                return null;

            Delegate function;

            if(!m_nameToUnmanagedFunction.TryGetValue(functionName, out function)) {
                function = Marshal.GetDelegateForFunctionPointer(procAddr, typeof(T));
                m_nameToUnmanagedFunction.Add(functionName, function);
            }

            Object o = function;

            return o as T;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if(!m_isDisposed) {
                FreeLibrary(disposing);
            }

            m_isDisposed = true;
        }
    }

    internal static class NativeMethods {

        [DllImport("kernel32", CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
        public static extern IntPtr LoadLibrary(String fileName);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, String procName);
    }
}
