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
using System.Reflection;
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
        private String m_libraryPath = "";

        private bool m_enableVerboseLogging = false;

        /// <summary>
        /// Event that is triggered when the assimp library is loaded.
        /// </summary>
        public event EventHandler LibraryLoaded;

        /// <summary>
        /// Event that is triggered when the assimp library is -about to be freed-.
        /// </summary>
        public event EventHandler LibraryFreed;

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
                return m_impl.DefaultLibraryPath32Bit;
            }
        }

        /// <summary>
        /// Gets the default path used for loading the 64-bit version of the unmanaged Assimp DLL.
        /// </summary>
        public String DefaultLibraryPath64Bit {
            get {
                return m_impl.DefaultLibraryPath64Bit;
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
        public bool IsLibraryLoaded {
            get {
                return m_impl != null && m_impl.LibraryLoaded;
            }
        }

        /// <summary>
        /// Gets if the Assimp unmanaged library supports multithreading. If it was compiled for single threading only,
        /// then it will not utilize multiple threads during import.
        /// </summary>
        public bool IsMultithreadingSupported {
            get {
                return !((GetCompileFlags() & CompileFlags.SingleThreaded) == CompileFlags.SingleThreaded);
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
            if(IsLibraryLoaded)
                throw new AssimpException("Assimp library already loaded.");

            AssimpLibraryImplementation impl = AssimpDefaultLibraryPath.CreateRuntimeImplementation();
            String libPath = (IntPtr.Size == 8) ? impl.DefaultLibraryPath64Bit : impl.DefaultLibraryPath32Bit;

            impl.LoadAssimpLibrary(libPath);

            m_impl = impl;
            m_libraryPath = libPath;

            OnLibraryLoaded();
        }

        /// <summary>
        /// Loads the Assimp unmanaged library using the DLL path specified in LibraryPath. If the library is already loaded, or there was an error
        /// in loading the library, an AssimpException is thrown.
        /// </summary>
        /// <param name="libPath">Path to the Assimp DLL.</param>
        /// <exception cref="Assimp.AssimpException">Thrown if the library is already loaded or if there was an error in loading the library.</exception>
        public void LoadLibrary(String libPath) {
            if(IsLibraryLoaded)
                throw new AssimpException("Assimp library already loaded.");

            AssimpLibraryImplementation impl = AssimpDefaultLibraryPath.CreateRuntimeImplementation();
            impl.LoadAssimpLibrary(libPath);

            m_impl = impl;
            m_libraryPath = libPath;

            OnLibraryLoaded();
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
                OnLibraryFreed();

                m_impl.FreeAssimpLibrary();
                m_impl = null;
                m_libraryPath = String.Empty;
            }
        }

        private void LoadIfNotLoaded() {
            if(!IsLibraryLoaded)
                LoadLibrary();
        }

        private void OnLibraryLoaded() {
            EventHandler temp = LibraryLoaded;

            if(temp != null)
                temp(this, EventArgs.Empty);
        }

        private void OnLibraryFreed() {
            EventHandler temp = LibraryFreed;

            if(temp != null)
                temp(this, EventArgs.Empty);
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

            AssimpDelegates.aiImportFileExWithProperties func = m_impl.GetFunction<AssimpDelegates.aiImportFileExWithProperties>(AssimpFunctionNames.aiImportFileExWithProperties);

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

            AssimpDelegates.aiImportFileFromMemoryWithProperties func = m_impl.GetFunction<AssimpDelegates.aiImportFileFromMemoryWithProperties>(AssimpFunctionNames.aiImportFileFromMemoryWithProperties);

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

            AssimpDelegates.aiReleaseImport func = m_impl.GetFunction<AssimpDelegates.aiReleaseImport>(AssimpFunctionNames.aiReleaseImport);

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

            AssimpDelegates.aiApplyPostProcessing func = m_impl.GetFunction<AssimpDelegates.aiApplyPostProcessing>(AssimpFunctionNames.aiApplyPostProcessing);

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

            int count = m_impl.GetFunction<AssimpDelegates.aiGetExportFormatCount>(AssimpFunctionNames.aiGetExportFormatCount)().ToInt32();

            if(count == 0)
                return new ExportFormatDescription[0];

            ExportFormatDescription[] descriptions = new ExportFormatDescription[count];

            AssimpDelegates.aiGetExportFormatDescription func = m_impl.GetFunction<AssimpDelegates.aiGetExportFormatDescription>(AssimpFunctionNames.aiGetExportFormatDescription);

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

            AssimpDelegates.aiExportSceneToBlob exportBlobFunc = m_impl.GetFunction<AssimpDelegates.aiExportSceneToBlob>(AssimpFunctionNames.aiExportSceneToBlob);
            AssimpDelegates.aiReleaseExportBlob releaseExportBlobFunc = m_impl.GetFunction<AssimpDelegates.aiReleaseExportBlob>(AssimpFunctionNames.aiReleaseExportBlob);

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

            AssimpDelegates.aiExportSceneEx exportFunc = m_impl.GetFunction<AssimpDelegates.aiExportSceneEx>(AssimpFunctionNames.aiExportSceneEx);

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

            AssimpDelegates.aiCopyScene func = m_impl.GetFunction<AssimpDelegates.aiCopyScene>(AssimpFunctionNames.aiCopyScene);

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

            AssimpDelegates.aiAttachLogStream func = m_impl.GetFunction<AssimpDelegates.aiAttachLogStream>(AssimpFunctionNames.aiAttachLogStream);

            func(logStreamPtr);
        }

        /// <summary>
        /// Enables verbose logging.
        /// </summary>
        /// <param name="enable">True if verbose logging is to be enabled or not.</param>
        public void EnableVerboseLogging(bool enable) {
            LoadIfNotLoaded();

            AssimpDelegates.aiEnableVerboseLogging func = m_impl.GetFunction<AssimpDelegates.aiEnableVerboseLogging>(AssimpFunctionNames.aiEnableVerboseLogging);

            func(enable);

            m_enableVerboseLogging = enable;
        }

        /// <summary>
        /// Gets if verbose logging is enabled.
        /// </summary>
        /// <returns>True if verbose logging is enabled, false otherwise.</returns>
        public bool GetVerboseLoggingEnabled() {
            return m_enableVerboseLogging;
        }

        /// <summary>
        /// Detaches a logstream callback.
        /// </summary>
        /// <param name="logStreamPtr">Pointer to an instance of AiLogStream.</param>
        /// <returns>A return code signifying if the function was successful or not.</returns>
        public ReturnCode DetachLogStream(IntPtr logStreamPtr) {
            LoadIfNotLoaded();

            AssimpDelegates.aiDetachLogStream func = m_impl.GetFunction<AssimpDelegates.aiDetachLogStream>(AssimpFunctionNames.aiDetachLogStream);

            return func(logStreamPtr);
        }

        /// <summary>
        /// Detaches all logstream callbacks currently attached to Assimp.
        /// </summary>
        public void DetachAllLogStreams() {
            LoadIfNotLoaded();

            AssimpDelegates.aiDetachAllLogStreams func = m_impl.GetFunction<AssimpDelegates.aiDetachAllLogStreams>(AssimpFunctionNames.aiDetachAllLogStreams);

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

            AssimpDelegates.aiCreatePropertyStore func = m_impl.GetFunction<AssimpDelegates.aiCreatePropertyStore>(AssimpFunctionNames.aiCreatePropertyStore);

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

            AssimpDelegates.aiReleasePropertyStore func = m_impl.GetFunction<AssimpDelegates.aiReleasePropertyStore>(AssimpFunctionNames.aiReleasePropertyStore);

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

            AssimpDelegates.aiSetImportPropertyInteger func = m_impl.GetFunction<AssimpDelegates.aiSetImportPropertyInteger>(AssimpFunctionNames.aiSetImportPropertyInteger);

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

            AssimpDelegates.aiSetImportPropertyFloat func = m_impl.GetFunction<AssimpDelegates.aiSetImportPropertyFloat>(AssimpFunctionNames.aiSetImportPropertyFloat);

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

            AssimpDelegates.aiSetImportPropertyString func = m_impl.GetFunction<AssimpDelegates.aiSetImportPropertyString>(AssimpFunctionNames.aiSetImportPropertyString);

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

            AssimpDelegates.aiGetMaterialColor func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialColor>(AssimpFunctionNames.aiGetMaterialColor);

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

            AssimpDelegates.aiGetMaterialFloatArray func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialFloatArray>(AssimpFunctionNames.aiGetMaterialFloatArray);

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

            AssimpDelegates.aiGetMaterialIntegerArray func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialIntegerArray>(AssimpFunctionNames.aiGetMaterialIntegerArray);

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

            AssimpDelegates.aiGetMaterialProperty func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialProperty>(AssimpFunctionNames.aiGetMaterialProperty);

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

            AssimpDelegates.aiGetMaterialString func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialString>(AssimpFunctionNames.aiGetMaterialString);

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

            AssimpDelegates.aiGetMaterialTextureCount func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialTextureCount>(AssimpFunctionNames.aiGetMaterialTextureCount);

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

            AssimpDelegates.aiGetMaterialTexture func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialTexture>(AssimpFunctionNames.aiGetMaterialTexture);

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

            AssimpDelegates.aiGetMaterialTexture func = m_impl.GetFunction<AssimpDelegates.aiGetMaterialTexture>(AssimpFunctionNames.aiGetMaterialTexture);

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

            AssimpDelegates.aiGetErrorString func = m_impl.GetFunction<AssimpDelegates.aiGetErrorString>(AssimpFunctionNames.aiGetErrorString);

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

            AssimpDelegates.aiIsExtensionSupported func = m_impl.GetFunction<AssimpDelegates.aiIsExtensionSupported>(AssimpFunctionNames.aiIsExtensionSupported);

            return func(extension);
        }

        /// <summary>
        /// Gets all the model format extensions that are currently supported by Assimp.
        /// </summary>
        /// <returns>Array of supported format extensions</returns>
        public String[] GetExtensionList() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetExtensionList func = m_impl.GetFunction<AssimpDelegates.aiGetExtensionList>(AssimpFunctionNames.aiGetExtensionList);

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

            AssimpDelegates.aiGetMemoryRequirements func = m_impl.GetFunction<AssimpDelegates.aiGetMemoryRequirements>(AssimpFunctionNames.aiGetMemoryRequirements);

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

            AssimpDelegates.aiCreateQuaternionFromMatrix func = m_impl.GetFunction<AssimpDelegates.aiCreateQuaternionFromMatrix>(AssimpFunctionNames.aiCreateQuaternionFromMatrix);

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

            AssimpDelegates.aiDecomposeMatrix func = m_impl.GetFunction<AssimpDelegates.aiDecomposeMatrix>(AssimpFunctionNames.aiDecomposeMatrix);

            func(ref mat, out scaling, out rotation, out position);
        }

        /// <summary>
        /// Transposes the 4x4 matrix.
        /// </summary>
        /// <param name="mat">Matrix to transpose</param>
        public void TransposeMatrix4(ref Matrix4x4 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiTransposeMatrix4 func = m_impl.GetFunction<AssimpDelegates.aiTransposeMatrix4>(AssimpFunctionNames.aiTransposeMatrix4);

            func(ref mat);
        }

        /// <summary>
        /// Transposes the 3x3 matrix.
        /// </summary>
        /// <param name="mat">Matrix to transpose</param>
        public void TransposeMatrix3(ref Matrix3x3 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiTransposeMatrix3 func = m_impl.GetFunction<AssimpDelegates.aiTransposeMatrix3>(AssimpFunctionNames.aiTransposeMatrix3);

            func(ref mat);
        }

        /// <summary>
        /// Transforms the vector by the 3x3 rotation matrix.
        /// </summary>
        /// <param name="vec">Vector to transform</param>
        /// <param name="mat">Rotation matrix</param>
        public void TransformVecByMatrix3(ref Vector3D vec, ref Matrix3x3 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiTransformVecByMatrix3 func = m_impl.GetFunction<AssimpDelegates.aiTransformVecByMatrix3>(AssimpFunctionNames.aiTransformVecByMatrix3);

            func(ref vec, ref mat);
        }

        /// <summary>
        /// Transforms the vector by the 4x4 matrix.
        /// </summary>
        /// <param name="vec">Vector to transform</param>
        /// <param name="mat">Matrix transformation</param>
        public void TransformVecByMatrix4(ref Vector3D vec, ref Matrix4x4 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiTransformVecByMatrix4 func = m_impl.GetFunction<AssimpDelegates.aiTransformVecByMatrix4>(AssimpFunctionNames.aiTransformVecByMatrix4);

            func(ref vec, ref mat);
        }

        /// <summary>
        /// Multiplies two 4x4 matrices. The destination matrix receives the result.
        /// </summary>
        /// <param name="dst">First input matrix and is also the Matrix to receive the result</param>
        /// <param name="src">Second input matrix, to be multiplied with "dst".</param>
        public void MultiplyMatrix4(ref Matrix4x4 dst, ref Matrix4x4 src) {
            LoadIfNotLoaded();

            AssimpDelegates.aiMultiplyMatrix4 func = m_impl.GetFunction<AssimpDelegates.aiMultiplyMatrix4>(AssimpFunctionNames.aiMultiplyMatrix4);

            func(ref dst, ref src);
        }

        /// <summary>
        /// Multiplies two 3x3 matrices. The destination matrix receives the result.
        /// </summary>
        /// <param name="dst">First input matrix and is also the Matrix to receive the result</param>
        /// <param name="src">Second input matrix, to be multiplied with "dst".</param>
        public void MultiplyMatrix3(ref Matrix3x3 dst, ref Matrix3x3 src) {
            LoadIfNotLoaded();

            AssimpDelegates.aiMultiplyMatrix3 func = m_impl.GetFunction<AssimpDelegates.aiMultiplyMatrix3>(AssimpFunctionNames.aiMultiplyMatrix3);

            func(ref dst, ref src);
        }

        /// <summary>
        /// Creates a 3x3 identity matrix.
        /// </summary>
        /// <param name="mat">Matrix to hold the identity</param>
        public void IdentityMatrix3(out Matrix3x3 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiIdentityMatrix3 func = m_impl.GetFunction<AssimpDelegates.aiIdentityMatrix3>(AssimpFunctionNames.aiIdentityMatrix3);

            func(out mat);
        }

        /// <summary>
        /// Creates a 4x4 identity matrix.
        /// </summary>
        /// <param name="mat">Matrix to hold the identity</param>
        public void IdentityMatrix4(out Matrix4x4 mat) {
            LoadIfNotLoaded();

            AssimpDelegates.aiIdentityMatrix4 func = m_impl.GetFunction<AssimpDelegates.aiIdentityMatrix4>(AssimpFunctionNames.aiIdentityMatrix4);

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

            AssimpDelegates.aiGetLegalString func = m_impl.GetFunction<AssimpDelegates.aiGetLegalString>(AssimpFunctionNames.aiGetLegalString);

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

            AssimpDelegates.aiGetVersionMinor func = m_impl.GetFunction<AssimpDelegates.aiGetVersionMinor>(AssimpFunctionNames.aiGetVersionMinor);

            return func();
        }

        /// <summary>
        /// Gets the native Assimp DLL's major version number.
        /// </summary>
        /// <returns>Assimp major version number</returns>
        public uint GetVersionMajor() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetVersionMajor func = m_impl.GetFunction<AssimpDelegates.aiGetVersionMajor>(AssimpFunctionNames.aiGetVersionMajor);

            return func();
        }

        /// <summary>
        /// Gets the native Assimp DLL's revision version number.
        /// </summary>
        /// <returns>Assimp revision version number</returns>
        public uint GetVersionRevision() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetVersionRevision func = m_impl.GetFunction<AssimpDelegates.aiGetVersionRevision>(AssimpFunctionNames.aiGetVersionRevision);

            return func();
        }

        /// <summary>
        /// Gets the native Assimp DLL's current version number as "major.minor.revision" string. This is the
        /// version of Assimp that this wrapper is currently using.
        /// </summary>
        /// <returns>Unmanaged DLL version</returns>
        public String GetVersion() {
            uint major = GetVersionMajor();
            uint minor = GetVersionMinor();
            uint rev = GetVersionRevision();

            return String.Format("{0}.{1}.{2}", major.ToString(), minor.ToString(), rev.ToString());
        }

        /// <summary>
        /// Gets the native Assimp DLL's current version number as a .NET version object.
        /// </summary>
        /// <returns>Unmanaged DLL version</returns>
        public Version GetVersionAsVersion() {
            return new Version((int) GetVersionMajor(), (int) GetVersionMinor(), 0, (int) GetVersionRevision());
        }

        /// <summary>
        /// Get the compilation flags that describe how the native Assimp DLL was compiled.
        /// </summary>
        /// <returns>Compilation flags</returns>
        public CompileFlags GetCompileFlags() {
            LoadIfNotLoaded();

            AssimpDelegates.aiGetCompileFlags func = m_impl.GetFunction<AssimpDelegates.aiGetCompileFlags>(AssimpFunctionNames.aiGetCompileFlags);

            return (CompileFlags) func();
        }

        #endregion
    }

    #region Assimp Functions

    /// <summary>
    /// A special attribute for assimp function delegates that specifies the actual unmanaged C-function name. Generally the delegates
    /// are named the same way as the actual function name, but this is for future proofing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate)]
    internal class AssimpFunctionNameAttribute : Attribute {
        private String m_unmanagedFunctionName;

        public String UnmanagedFunctionName {
            get {
                return m_unmanagedFunctionName;
            }
        }

        public AssimpFunctionNameAttribute(String unmanagedFunctionName) {
            m_unmanagedFunctionName = unmanagedFunctionName;
        }
    }

    /// <summary>
    /// Defines all the unmanaged assimp C-function names.
    /// </summary>
    internal static class AssimpFunctionNames {

        #region Import Function Names

        public const String aiImportFile = "aiImportFile";
        public const String aiImportFileEx = "aiImportFileEx";
        public const String aiImportFileExWithProperties = "aiImportFileExWithProperties";
        public const String aiImportFileFromMemory = "aiImportFileFromMemory";
        public const String aiImportFileFromMemoryWithProperties = "aiImportFileFromMemoryWithProperties";
        public const String aiReleaseImport = "aiReleaseImport";
        public const String aiApplyPostProcessing = "aiApplyPostProcessing";

        #endregion

        #region Export Function Names

        public const String aiGetExportFormatCount = "aiGetExportFormatCount";
        public const String aiGetExportFormatDescription = "aiGetExportFormatDescription";
        public const String aiExportSceneToBlob = "aiExportSceneToBlob";
        public const String aiReleaseExportBlob = "aiReleaseExportBlob";
        public const String aiExportScene = "aiExportScene";
        public const String aiExportSceneEx = "aiExportSceneEx";
        public const String aiCopyScene = "aiCopyScene";

        #endregion

        #region Logging Function Names

        public const String aiAttachLogStream = "aiAttachLogStream";
        public const String aiEnableVerboseLogging = "aiEnableVerboseLogging";
        public const String aiDetachLogStream = "aiDetachLogStream";
        public const String aiDetachAllLogStreams = "aiDetachAllLogStreams";

        #endregion

        #region Import Properties Function Names

        public const String aiCreatePropertyStore = "aiCreatePropertyStore";
        public const String aiReleasePropertyStore = "aiReleasePropertyStore";
        public const String aiSetImportPropertyInteger = "aiSetImportPropertyInteger";
        public const String aiSetImportPropertyFloat = "aiSetImportPropertyFloat";
        public const String aiSetImportPropertyString = "aiSetImportPropertyString";

        #endregion

        #region Material Getters Function Names

        public const String aiGetMaterialColor = "aiGetMaterialColor";
        public const String aiGetMaterialFloatArray = "aiGetMaterialFloatArray";
        public const String aiGetMaterialIntegerArray = "aiGetMaterialIntegerArray";
        public const String aiGetMaterialProperty = "aiGetMaterialProperty";
        public const String aiGetMaterialString = "aiGetMaterialString";
        public const String aiGetMaterialTextureCount = "aiGetMaterialTextureCount";
        public const String aiGetMaterialTexture = "aiGetMaterialTexture";

        #endregion

        #region Error and Info Function Names

        public const String aiGetErrorString = "aiGetErrorString";
        public const String aiIsExtensionSupported = "aiIsExtensionSupported";
        public const String aiGetExtensionList = "aiGetExtensionList";
        public const String aiGetMemoryRequirements = "aiGetMemoryRequirements";

        #endregion

        #region Math Function Names

        public const String aiCreateQuaternionFromMatrix = "aiCreateQuaternionFromMatrix";
        public const String aiDecomposeMatrix = "aiDecomposeMatrix";
        public const String aiTransposeMatrix4 = "aiTransposeMatrix4";
        public const String aiTransposeMatrix3 = "aiTransposeMatrix3";
        public const String aiTransformVecByMatrix3 = "aiTransformVecByMatrix3";
        public const String aiTransformVecByMatrix4 = "aiTransformVecByMatrix4";
        public const String aiMultiplyMatrix4 = "aiMultiplyMatrix4";
        public const String aiMultiplyMatrix3 = "aiMultiplyMatrix3";
        public const String aiIdentityMatrix3 = "aiIdentityMatrix3";
        public const String aiIdentityMatrix4 = "aiIdentityMatrix4";

        #endregion

        #region Version Info Function Names

        public const String aiGetLegalString = "aiGetLegalString";
        public const String aiGetVersionMinor = "aiGetVersionMinor";
        public const String aiGetVersionMajor = "aiGetVersionMajor";
        public const String aiGetVersionRevision = "aiGetVersionRevision";
        public const String aiGetCompileFlags = "aiGetCompileFlags";

        #endregion
    }

    /// <summary>
    /// Defines all of the delegates that represent the unmanaged assimp functions.
    /// </summary>
    internal static class AssimpDelegates {

        #region Import Delegates
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiImportFile)]
        public delegate IntPtr aiImportFile([In, MarshalAs(UnmanagedType.LPStr)] String file, uint flags);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiImportFileEx)]
        public delegate IntPtr aiImportFileEx([In, MarshalAs(UnmanagedType.LPStr)] String file, uint flags, IntPtr fileIO);
  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiImportFileExWithProperties)]
        public delegate IntPtr aiImportFileExWithProperties([In, MarshalAs(UnmanagedType.LPStr)] String file, uint flag, IntPtr fileIO, IntPtr propStore);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiImportFileFromMemory)]
        public delegate IntPtr aiImportFileFromMemory(byte[] buffer, uint bufferLength, uint flags, [In, MarshalAs(UnmanagedType.LPStr)] String formatHint);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiImportFileFromMemoryWithProperties)]
        public delegate IntPtr aiImportFileFromMemoryWithProperties(byte[] buffer, uint bufferLength, uint flags, [In, MarshalAs(UnmanagedType.LPStr)] String formatHint, IntPtr propStore);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiReleaseImport)]
        public delegate void aiReleaseImport(IntPtr scene);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiApplyPostProcessing)]
        public delegate IntPtr aiApplyPostProcessing(IntPtr scene, uint Flags);

        #endregion

        #region Export Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetExportFormatCount)]
        public delegate IntPtr aiGetExportFormatCount();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetExportFormatDescription)]
        public delegate IntPtr aiGetExportFormatDescription(IntPtr index);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiExportSceneToBlob)]
        public delegate IntPtr aiExportSceneToBlob(IntPtr scene, [In, MarshalAs(UnmanagedType.LPStr)] String formatId, uint preProcessing);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiReleaseExportBlob)]
        public delegate void aiReleaseExportBlob(IntPtr blobData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiExportScene)]
        public delegate ReturnCode aiExportScene(IntPtr scene, [In, MarshalAs(UnmanagedType.LPStr)] String formatId, [In, MarshalAs(UnmanagedType.LPStr)] String fileName, uint preProcessing);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiExportSceneEx)]
        public delegate ReturnCode aiExportSceneEx(IntPtr scene, [In, MarshalAs(UnmanagedType.LPStr)] String formatId, [In, MarshalAs(UnmanagedType.LPStr)] String fileName, IntPtr fileIO, uint preProcessing);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiCopyScene)]
        public delegate void aiCopyScene(IntPtr sceneIn, out IntPtr sceneOut);

        #endregion

        #region Logging Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiAttachLogStream)]
        public delegate void aiAttachLogStream(IntPtr logStreamPtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiEnableVerboseLogging)]
        public delegate void aiEnableVerboseLogging([In, MarshalAs(UnmanagedType.Bool)] bool enable);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiDetachLogStream)]
        public delegate ReturnCode aiDetachLogStream(IntPtr logStreamPtr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiDetachAllLogStreams)]
        public delegate void aiDetachAllLogStreams();

        #endregion

        #region Property Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiCreatePropertyStore)]
        public delegate IntPtr aiCreatePropertyStore();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiReleasePropertyStore)]
        public delegate void aiReleasePropertyStore(IntPtr propertyStore);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiSetImportPropertyInteger)]
        public delegate void aiSetImportPropertyInteger(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPStr)] String name, int value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiSetImportPropertyFloat)]
        public delegate void aiSetImportPropertyFloat(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPStr)] String name, float value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiSetImportPropertyString)]
        public delegate void aiSetImportPropertyString(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPStr)] String name, ref AiString value);

        #endregion

        #region Material Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetMaterialColor)]
        public delegate ReturnCode aiGetMaterialColor(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr colorOut);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetMaterialFloatArray)]
        public delegate ReturnCode aiGetMaterialFloatArray(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr ptrOut, ref uint valueCount);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetMaterialIntegerArray)]
        public delegate ReturnCode aiGetMaterialIntegerArray(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, IntPtr ptrOut, ref uint valueCount);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetMaterialProperty)]
        public delegate ReturnCode aiGetMaterialProperty(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, out IntPtr propertyOut);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetMaterialString)]
        public delegate ReturnCode aiGetMaterialString(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPStr)] String key, uint texType, uint texIndex, out AiString str);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetMaterialTexture)]
        public delegate ReturnCode aiGetMaterialTexture(ref AiMaterial mat, TextureType type, uint index, out AiString path, out TextureMapping mapping, out uint uvIndex, out float blendFactor, out TextureOperation textureOp, [In, Out] TextureWrapMode[] wrapModes, out uint flags);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetMaterialTextureCount)]
        public delegate uint aiGetMaterialTextureCount(ref AiMaterial mat, TextureType type);

        #endregion

        #region Math Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiCreateQuaternionFromMatrix)]
        public delegate void aiCreateQuaternionFromMatrix(out Quaternion quat, ref Matrix3x3 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiDecomposeMatrix)]
        public delegate void aiDecomposeMatrix(ref Matrix4x4 mat, out Vector3D scaling, out Quaternion rotation, out Vector3D position);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiTransposeMatrix4)]
        public delegate void aiTransposeMatrix4(ref Matrix4x4 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiTransposeMatrix3)]
        public delegate void aiTransposeMatrix3(ref Matrix3x3 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiTransformVecByMatrix3)]
        public delegate void aiTransformVecByMatrix3(ref Vector3D vec, ref Matrix3x3 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiTransformVecByMatrix4)]
        public delegate void aiTransformVecByMatrix4(ref Vector3D vec, ref Matrix4x4 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiMultiplyMatrix4)]
        public delegate void aiMultiplyMatrix4(ref Matrix4x4 dst, ref Matrix4x4 src);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiMultiplyMatrix3)]
        public delegate void aiMultiplyMatrix3(ref Matrix3x3 dst, ref Matrix3x3 src);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiIdentityMatrix3)]
        public delegate void aiIdentityMatrix3(out Matrix3x3 mat);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiIdentityMatrix4)]
        public delegate void aiIdentityMatrix4(out Matrix4x4 mat);

        #endregion

        #region Error and Info Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetErrorString)]
        public delegate IntPtr aiGetErrorString();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetExtensionList)]
        public delegate void aiGetExtensionList(ref AiString extensionsOut);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetMemoryRequirements)]
        public delegate void aiGetMemoryRequirements(IntPtr scene, ref AiMemoryInfo memoryInfo);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiIsExtensionSupported)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool aiIsExtensionSupported([In, MarshalAs(UnmanagedType.LPStr)] String extension);

        #endregion

        #region Version Info Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetLegalString)]
        public delegate IntPtr aiGetLegalString();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetVersionMinor)]
        public delegate uint aiGetVersionMinor();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetVersionMajor)]
        public delegate uint aiGetVersionMajor();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetVersionRevision)]
        public delegate uint aiGetVersionRevision();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), AssimpFunctionName(AssimpFunctionNames.aiGetCompileFlags)]
        public delegate uint aiGetCompileFlags();

        #endregion
    }

    #endregion

    #region Implementation

    /// <summary>
    /// Specifies default paths for the unmanaged library for all platforms and handles the choosing of the correct implementation platform based on
    /// the .NET runtime.
    /// </summary>
    internal static class AssimpDefaultLibraryPath {
        public const String DefaultWindows32BitPath = "Assimp32.dll";
        public const String DefaultWindows64BitPath = "Assimp64.dll";

        public const String DefaultLinux32BitPath = "Assimp32.so";
        public const String DefaultLinux64BitPath = "Assimp64.so";

        public static AssimpLibraryImplementation CreateRuntimeImplementation() {
            if(IsLinux()) {
                return new AssimpLibraryLinuxImplementation();
            } else {
                return new AssimpLibraryWindowsImplementation();
            }
        }

        private static bool IsLinux() {
            int platform = (int) Environment.OSVersion.Platform;
            return (platform == 4) || (platform == 6) || (platform == 128);
        }
    }

    /// <summary>
    /// Base class for library implementations, handles loading of each unmanaged function delegate. Implementations handle the environment specific
    /// tasks of loading the native library and getting function proc addresses.
    /// </summary>
    internal abstract class AssimpLibraryImplementation : IDisposable {
        private Dictionary<String, Delegate> m_nameToUnmanagedFunction;
        private IntPtr m_libraryHandle;
        private bool m_isDisposed;

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

        public abstract String DefaultLibraryPath32Bit {
            get;
        }

        public abstract String DefaultLibraryPath64Bit {
            get;
        }

        public AssimpLibraryImplementation() {
            m_libraryHandle = IntPtr.Zero;
            m_nameToUnmanagedFunction = new Dictionary<String, Delegate>();
            m_isDisposed = false;
        }

        ~AssimpLibraryImplementation() {
            Dispose(false);
        }

        public void LoadAssimpLibrary(String path) {
            FreeAssimpLibrary(true);
            m_libraryHandle = NativeLoadLibrary(path);

            //When we load the library, we preload all the functions so we don't have to synchronize our GetFunction method
            PreloadFunctions();
        }

        public void FreeAssimpLibrary() {
            FreeAssimpLibrary(true);
        }

        private void FreeAssimpLibrary(bool clearFunctions) {
            if(m_libraryHandle != IntPtr.Zero) {
                NativeFreeLibrary(m_libraryHandle);
                m_libraryHandle = IntPtr.Zero;

                if(clearFunctions)
                    m_nameToUnmanagedFunction.Clear();
            }
        }

        public T GetFunction<T>(String functionName) where T : class {
            return GetFunction(functionName, typeof(T)) as T;
        }

        public Object GetFunction(String functionName, Type type) {
            if(!LibraryLoaded || type == null || String.IsNullOrEmpty(functionName))
                return null;

            if(IsDisposed)
                throw new ObjectDisposedException("AssimpLibrary has been disposed");

            IntPtr procAddr = NativeGetProcAddress(m_libraryHandle, functionName);

            if(procAddr == IntPtr.Zero)
                return null;

            Delegate function;

            if(!m_nameToUnmanagedFunction.TryGetValue(functionName, out function)) {
                function = Marshal.GetDelegateForFunctionPointer(procAddr, type);
                m_nameToUnmanagedFunction.Add(functionName, function);
            }

            return (Object) function;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract IntPtr NativeLoadLibrary(String path);
        protected abstract void NativeFreeLibrary(IntPtr handle);
        protected abstract IntPtr NativeGetProcAddress(IntPtr handle, String functionName);

        private void PreloadFunctions() {
            Type[] funcDelegateTypes = typeof(AssimpDelegates).GetNestedTypes();

            foreach(Type funcType in funcDelegateTypes) {
                AssimpFunctionNameAttribute assimpAttr = GetAssimpAttribute(funcType);

                if(assimpAttr == null)
                    continue;

                GetFunction(assimpAttr.UnmanagedFunctionName, funcType);
            }
        }

        private void Dispose(bool disposing) {
            if(!m_isDisposed)
                FreeAssimpLibrary(disposing);

            m_isDisposed = true;
        }

        private AssimpFunctionNameAttribute GetAssimpAttribute(Type type) {
            object[] attributes = type.GetCustomAttributes(typeof(AssimpFunctionNameAttribute), false);
            foreach(object attr in attributes) {
                if(attr is AssimpFunctionNameAttribute)
                    return attr as AssimpFunctionNameAttribute;
            }

            return null;
        }
    }

    #region Windows Implementation

    /// <summary>
    /// Windows implementation for loading the unmanaged assimp library.
    /// </summary>
    internal sealed class AssimpLibraryWindowsImplementation : AssimpLibraryImplementation {

        public override string DefaultLibraryPath32Bit {
            get {
                return AssimpDefaultLibraryPath.DefaultWindows32BitPath;
            }
        }

        public override string DefaultLibraryPath64Bit {
            get {
                return AssimpDefaultLibraryPath.DefaultWindows64BitPath;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
        private static extern IntPtr LoadLibrary(String fileName);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, String procName);

        protected override IntPtr NativeLoadLibrary(string path) {
            IntPtr libraryHandle = LoadLibrary(path);

            if(libraryHandle == IntPtr.Zero) {
                int hr = Marshal.GetHRForLastWin32Error();
                Exception innerException = Marshal.GetExceptionForHR(hr);
                if(innerException != null)
                    throw new AssimpException("Error loading unmanaged library from path: " + path + ", see inner exception for details.\n" + innerException.Message, innerException);
                else
                    throw new AssimpException("Error loading unmanaged library from path: " + path);
            }

            return libraryHandle;
        }

        protected override void NativeFreeLibrary(IntPtr handle) {
            FreeLibrary(handle);
        }

        protected override IntPtr NativeGetProcAddress(IntPtr handle, String functionName) {
            return GetProcAddress(handle, functionName);
        }
    }

    #endregion

    #region Linux Implementation

    /// <summary>
    /// Linux implementation for loading the unmanaged assimp library.
    /// </summary>
    internal sealed class AssimpLibraryLinuxImplementation : AssimpLibraryImplementation {

        public override string DefaultLibraryPath32Bit {
            get {
                return AssimpDefaultLibraryPath.DefaultLinux32BitPath;
            }
        }

        public override string DefaultLibraryPath64Bit {
            get {
                return AssimpDefaultLibraryPath.DefaultLinux64BitPath;
            }
        }

        [DllImport("libdl.so")]
        private static extern IntPtr dlopen(String fileName, int flags);

        [DllImport("libdl.so")]
        private static extern IntPtr dlsym(IntPtr handle, String functionName);

        [DllImport("libdl.so")]
        private static extern int dlclose(IntPtr handle);

        [DllImport("libdl.so")]
        private static extern IntPtr dlerror();

        private const int RTLD_NOW = 2;

        protected override IntPtr NativeLoadLibrary(String path) {
            IntPtr libraryHandle = dlopen(path, RTLD_NOW);

            if(libraryHandle == IntPtr.Zero) {
                IntPtr errPtr = dlerror();
                String msg = Marshal.PtrToStringAnsi(errPtr);
                if(!String.IsNullOrEmpty(msg))
                    throw new AssimpException("Error loading unmanaged library from path: " + path + ", error detail:\n" + msg);
                else
                    throw new AssimpException("Error loading unmanaged library from path: " + path);
            }

            return libraryHandle;
        }

        protected override void NativeFreeLibrary(IntPtr handle) {
            dlclose(handle);
        }

        protected override IntPtr NativeGetProcAddress(IntPtr handle, string functionName) {
            return dlsym(handle, functionName);
        }
    }

    #endregion

    #endregion
}
