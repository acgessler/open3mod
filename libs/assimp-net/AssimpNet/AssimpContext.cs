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
using Assimp.Configs;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Represents an Assimp Import/Export context that load or save models using the unmanaged library. Additionally, conversion
    /// functionality is offered to bypass loading model data into managed memory.
    /// </summary>
    public class AssimpContext : IDisposable {
        private bool m_isDisposed;
        private Dictionary<String, PropertyConfig> m_configs;
        private IOSystem m_ioSystem;

        private ExportFormatDescription[] m_exportFormats;
        private String[] m_importFormats;

        private float m_scale = 1.0f;
        private float m_xAxisRotation = 0.0f;
        private float m_yAxisRotation = 0.0f;
        private float m_zAxisRotation = 0.0f;
        private bool m_buildMatrix = false;
        private Matrix4x4 m_scaleRot = Matrix4x4.Identity;

        private IntPtr m_propStore = IntPtr.Zero;

        /// <summary>
        /// Gets if the context has been disposed.
        /// </summary>
        public bool IsDisposed {
            get {
                return m_isDisposed;
            }
        }

        /// <summary>
        /// Gets or sets the uniform scale for the model. This is multiplied
        /// with the existing root node's transform. This is only used during import.
        /// </summary>
        public float Scale {
            get {
                return m_scale;
            }
            set {
               if(m_scale != value) {
                    m_scale = value;
                    m_buildMatrix = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the model's rotation about the X-Axis, in degrees. This is multiplied
        /// with the existing root node's transform. This is only used during import.
        /// </summary>
        public float XAxisRotation {
            get {
                return m_xAxisRotation;
            }
            set {
                if(m_xAxisRotation != value) {
                    m_xAxisRotation = value;
                    m_buildMatrix = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the model's rotation abut the Y-Axis, in degrees. This is multiplied
        /// with the existing root node's transform. This is only used during import.
        /// </summary>
        public float YAxisRotation {
            get {
                return m_yAxisRotation;
            }
            set {
                if(m_yAxisRotation != value) {
                    m_yAxisRotation = value;
                    m_buildMatrix = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the model's rotation about the Z-Axis, in degrees. This is multiplied
        /// with the existing root node's transform. This is only used during import.
        /// </summary>
        public float ZAxisRotation {
            get {
                return m_zAxisRotation;
            }
            set {
                if(m_zAxisRotation != value) {
                    m_zAxisRotation = value;
                    m_buildMatrix = true;
                }
            }
        }

        /// <summary>
        /// Gets whether this context is using a user-defined IO system for file handling.
        /// </summary>
        public bool UsingCustomIOSystem {
            get {
                return m_ioSystem != null && !m_ioSystem.IsDisposed;
            }
        }

        /// <summary>
        /// Gets the property configurations set to this context. This is only used during import.
        /// </summary>
        public Dictionary<String, PropertyConfig> PropertyConfigurations {
            get {
                return m_configs;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="AssimpContext"/> class.
        /// </summary>
        public AssimpContext() {
            m_configs = new Dictionary<String, PropertyConfig>();
        }

        #region Import

        #region ImportFileFromStream

        /// <summary>
        /// Imports a model from the stream without running any post-process steps. The importer sets configurations
        /// and loads the model into managed memory, releasing the unmanaged memory used by Assimp. It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="formatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <returns>The imported scene</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public Scene ImportFileFromStream(Stream stream, String formatHint) {
            return ImportFileFromStream(stream, PostProcessSteps.None, formatHint);
        }

        /// <summary>
        /// Imports a model from the stream. The importer sets configurations
        /// and loads the model into managed memory, releasing the unmanaged memory used by Assimp. It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="postProcessFlags">Post processing flags, if any</param>
        /// <param name="formatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <returns>The imported scene</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public Scene ImportFileFromStream(Stream stream, PostProcessSteps postProcessFlags, String formatHint) {
            CheckDisposed();

            if(stream == null || stream.CanRead != true)
                throw new AssimpException("stream", "Can't read from the stream it's null or write-only");

            if(String.IsNullOrEmpty(formatHint))
                throw new AssimpException("formatHint", "Format hint is null or empty");

            IntPtr ptr = IntPtr.Zero;
            PrepareImport();

            try {
                ptr = AssimpLibrary.Instance.ImportFileFromStream(stream, PostProcessSteps.None, formatHint, m_propStore);

                if(ptr == IntPtr.Zero)
                    throw new AssimpException("Error importing file: " + AssimpLibrary.Instance.GetErrorString());

                TransformScene(ptr);

                if(postProcessFlags != PostProcessSteps.None)
                    ptr = AssimpLibrary.Instance.ApplyPostProcessing(ptr, postProcessFlags);

                return Scene.FromUnmanagedScene(ptr);
            } finally {
                CleanupImport();

                if(ptr != IntPtr.Zero) {
                    AssimpLibrary.Instance.ReleaseImport(ptr);
                }
            }
        }

        #endregion

        #region ImportFile

        /// <summary>
        /// Imports a model from the specified file without running any post-process steps. The importer sets configurations
        /// and loads the model into managed memory, releasing the unmanaged memory used by Assimp.
        /// </summary>
        /// <param name="file">Full path to the file</param>
        /// <returns>The imported scene</returns>
        /// <exception cref="AssimpException">Thrown if there was a general error in importing the model.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file could not be located.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public Scene ImportFile(String file) {
            return ImportFile(file, PostProcessSteps.None);
        }

        /// <summary>
        /// Imports a model from the specified file. The importer sets configurations
        /// and loads the model into managed memory, releasing the unmanaged memory used by Assimp.
        /// </summary>
        /// <param name="file">Full path to the file</param>
        /// <param name="postProcessFlags">Post processing flags, if any</param>
        /// <returns>The imported scene</returns>
        /// <exception cref="AssimpException">Thrown if there was a general error in importing the model.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file could not be located.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public Scene ImportFile(String file, PostProcessSteps postProcessFlags) {
            CheckDisposed();

            IntPtr ptr = IntPtr.Zero;
            IntPtr fileIO = IntPtr.Zero;

            //Only do file checks if not using a custom IOSystem
            if(UsingCustomIOSystem) {
                fileIO = m_ioSystem.AiFileIO;
            } else if(String.IsNullOrEmpty(file) || !File.Exists(file)) {
                throw new FileNotFoundException("Filename was null or could not be found", file);
            }

            PrepareImport();

            try {
                ptr = AssimpLibrary.Instance.ImportFile(file, PostProcessSteps.None, fileIO, m_propStore);

                if(ptr == IntPtr.Zero)
                    throw new AssimpException("Error importing file: " + AssimpLibrary.Instance.GetErrorString());

                TransformScene(ptr);

                if(postProcessFlags != PostProcessSteps.None)
                    ptr = AssimpLibrary.Instance.ApplyPostProcessing(ptr, postProcessFlags);

                return Scene.FromUnmanagedScene(ptr);
            } finally {
                CleanupImport();

                if(ptr != IntPtr.Zero) {
                    AssimpLibrary.Instance.ReleaseImport(ptr);
                }
            }
        }

        #endregion

        #endregion

        #region Export

        #region ExportFile

        /// <summary>
        /// Exports a scene to the specified format and writes it to a file.
        /// </summary>
        /// <param name="scene">Scene containing the model to export.</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="exportFormatId">FormatID representing the format to export to.</param>
        /// <returns>True if the scene was exported successfully, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the scene is null.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public bool ExportFile(Scene scene, String fileName, String exportFormatId) {
            return ExportFile(scene, fileName, exportFormatId, PostProcessSteps.None);
        }

        /// <summary>
        /// Exports a scene to the specified format and writes it to a file.
        /// </summary>
        /// <param name="scene">Scene containing the model to export.</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="exportFormatId">FormatID representing the format to export to.</param>
        /// <param name="preProcessing">Preprocessing flags to apply to the model before it is exported.</param>
        /// <returns>True if the scene was exported successfully, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the scene is null.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public bool ExportFile(Scene scene, String fileName, String exportFormatId, PostProcessSteps preProcessing) {
            CheckDisposed();

            IntPtr fileIO = IntPtr.Zero;
            IntPtr scenePtr = IntPtr.Zero;

            if(scene == null)
                throw new ArgumentNullException("scene", "Scene must exist.");

            try {
                scenePtr = Scene.ToUnmanagedScene(scene);

                ReturnCode status = AssimpLibrary.Instance.ExportScene(scenePtr, exportFormatId, fileName, fileIO, preProcessing);

                return status == ReturnCode.Success;
            } finally {
                if(scenePtr != IntPtr.Zero)
                    Scene.FreeUnmanagedScene(scenePtr);
            }
        }

        #endregion

        #region ExportToBlob

        /// <summary>
        /// Exports a scene to the specified format and writes it to a data blob.
        /// </summary>
        /// <param name="scene">Scene containing the model to export.</param>
        /// <param name="exportFormatId">FormatID representing the format to export to.</param>
        /// <returns>The resulting data blob, or null if the export failed.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the scene is null.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public ExportDataBlob ExportToBlob(Scene scene, String exportFormatId) {
            return ExportToBlob(scene, exportFormatId, PostProcessSteps.None);
        }

        /// <summary>
        /// Exports a scene to the specified format and writes it to a data blob.
        /// </summary>
        /// <param name="scene">Scene containing the model to export.</param>
        /// <param name="exportFormatId">FormatID representing the format to export to.</param>
        /// <param name="preProcessing">Preprocessing flags to apply to the model before it is exported.</param>
        /// <returns>The resulting data blob, or null if the export failed.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the scene is null.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public ExportDataBlob ExportToBlob(Scene scene, String exportFormatId, PostProcessSteps preProcessing) {
            CheckDisposed();

            IntPtr fileIO = IntPtr.Zero;
            IntPtr scenePtr = IntPtr.Zero;

            if(scene == null)
                throw new ArgumentNullException("scene", "Scene must exist.");

            try {
                scenePtr = Scene.ToUnmanagedScene(scene);

                return AssimpLibrary.Instance.ExportSceneToBlob(scenePtr, exportFormatId, preProcessing);
            } finally {
                if(scenePtr != IntPtr.Zero)
                    Scene.FreeUnmanagedScene(scenePtr);
            }
        }

        #endregion

        #endregion

        #region ConvertFromFile

        #region File to File

        /// <summary>
        /// Converts the model contained in the file to the specified format and save it to a file.
        /// </summary>
        /// <param name="inputFilename">Input file name to import</param>
        /// <param name="outputFilename">Output file name to export to</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <returns>True if the conversion was successful or not, false otherwise.</returns>
        /// <exception cref="AssimpException">Thrown if there was a general error in importing the model.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file could not be located.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public bool ConvertFromFileToFile(String inputFilename, String outputFilename, String exportFormatId) {
            return ConvertFromFileToFile(inputFilename, PostProcessSteps.None, outputFilename, exportFormatId, PostProcessSteps.None);
        }

        /// <summary>
        /// Converts the model contained in the file to the specified format and save it to a file.
        /// </summary>
        /// <param name="inputFilename">Input file name to import</param>
        /// <param name="outputFilename">Output file name to export to</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <param name="exportProcessSteps">Pre processing steps used for the export</param>
        /// <returns>True if the conversion was successful or not, false otherwise.</returns>
        /// <exception cref="AssimpException">Thrown if there was a general error in importing the model.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file could not be located.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public bool ConvertFromFileToFile(String inputFilename, String outputFilename, String exportFormatId, PostProcessSteps exportProcessSteps) {
            return ConvertFromFileToFile(inputFilename, PostProcessSteps.None, outputFilename, exportFormatId, exportProcessSteps);
        }

        /// <summary>
        /// Converts the model contained in the file to the specified format and save it to a file.
        /// </summary>
        /// <param name="inputFilename">Input file name to import</param>
        /// <param name="importProcessSteps">Post processing steps used for the import</param>
        /// <param name="outputFilename">Output file name to export to</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <param name="exportProcessSteps">Pre processing steps used for the export</param>
        /// <returns>True if the conversion was successful or not, false otherwise.</returns>
        /// <exception cref="AssimpException">Thrown if there was a general error in importing the model.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file could not be located.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public bool ConvertFromFileToFile(String inputFilename, PostProcessSteps importProcessSteps, String outputFilename, String exportFormatId, PostProcessSteps exportProcessSteps) {
            CheckDisposed();

            IntPtr ptr = IntPtr.Zero;
            IntPtr fileIO = IntPtr.Zero;

            //Only do file checks if not using a custom IOSystem
            if(UsingCustomIOSystem) {
                fileIO = m_ioSystem.AiFileIO;
            } else if(String.IsNullOrEmpty(inputFilename) || !File.Exists(inputFilename)) {
                throw new FileNotFoundException("Filename was null or could not be found", inputFilename);
            }

            PrepareImport();

            try {
                ptr = AssimpLibrary.Instance.ImportFile(inputFilename, PostProcessSteps.None, fileIO, m_propStore);

                if(ptr == IntPtr.Zero)
                    throw new AssimpException("Error importing file: " + AssimpLibrary.Instance.GetErrorString());

                TransformScene(ptr);

                if(importProcessSteps != PostProcessSteps.None)
                    ptr = AssimpLibrary.Instance.ApplyPostProcessing(ptr, importProcessSteps);

                ReturnCode status = AssimpLibrary.Instance.ExportScene(ptr, exportFormatId, outputFilename, fileIO, exportProcessSteps);

                return status == ReturnCode.Success;
            } finally {
                CleanupImport();

                if(ptr != IntPtr.Zero)
                    AssimpLibrary.Instance.ReleaseImport(ptr);
            }
        }

        #endregion

        #region File to Blob

        /// <summary>
        /// Converts the model contained in the file to the specified format and save it to a data blob.
        /// </summary>
        /// <param name="inputFilename">Input file name to import</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <returns>Data blob containing the exported scene in a binary form</returns>
        /// <exception cref="AssimpException">Thrown if there was a general error in importing the model.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file could not be located.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public ExportDataBlob ConvertFromFileToBlob(String inputFilename, String exportFormatId) {
            return ConvertFromFileToBlob(inputFilename, PostProcessSteps.None, exportFormatId, PostProcessSteps.None);
        }

        /// <summary>
        /// Converts the model contained in the file to the specified format and save it to a data blob.
        /// </summary>
        /// <param name="inputFilename">Input file name to import</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <param name="exportProcessSteps">Pre processing steps used for the export</param>
        /// <returns>Data blob containing the exported scene in a binary form</returns>
        /// <exception cref="AssimpException">Thrown if there was a general error in importing the model.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file could not be located.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public ExportDataBlob ConvertFromFileToBlob(String inputFilename, String exportFormatId, PostProcessSteps exportProcessSteps) {
            return ConvertFromFileToBlob(inputFilename, PostProcessSteps.None, exportFormatId, exportProcessSteps);
        }

        /// <summary>
        /// Converts the model contained in the file to the specified format and save it to a data blob.
        /// </summary>
        /// <param name="inputFilename">Input file name to import</param>
        /// <param name="importProcessSteps">Post processing steps used for the import</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <param name="exportProcessSteps">Pre processing steps used for the export</param>
        /// <returns>Data blob containing the exported scene in a binary form</returns>
        /// <exception cref="AssimpException">Thrown if there was a general error in importing the model.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file could not be located.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public ExportDataBlob ConvertFromFileToBlob(String inputFilename, PostProcessSteps importProcessSteps, String exportFormatId, PostProcessSteps exportProcessSteps) {
            CheckDisposed();

            IntPtr ptr = IntPtr.Zero;
            IntPtr fileIO = IntPtr.Zero;

            //Only do file checks if not using a custom IOSystem
            if(UsingCustomIOSystem) {
                fileIO = m_ioSystem.AiFileIO;
            } else if(String.IsNullOrEmpty(inputFilename) || !File.Exists(inputFilename)) {
                throw new FileNotFoundException("Filename was null or could not be found", inputFilename);
            }

            PrepareImport();

            try {
                ptr = AssimpLibrary.Instance.ImportFile(inputFilename, PostProcessSteps.None, fileIO, m_propStore);

                if(ptr == IntPtr.Zero)
                    throw new AssimpException("Error importing file: " + AssimpLibrary.Instance.GetErrorString());

                TransformScene(ptr);

                if(importProcessSteps != PostProcessSteps.None)
                    ptr = AssimpLibrary.Instance.ApplyPostProcessing(ptr, importProcessSteps);

                return AssimpLibrary.Instance.ExportSceneToBlob(ptr, exportFormatId, exportProcessSteps);
            } finally {
                CleanupImport();

                if(ptr != IntPtr.Zero)
                    AssimpLibrary.Instance.ReleaseImport(ptr);
            }
        }

        #endregion

        #endregion

        #region ConvertFromStream

        #region Stream to File

        /// <summary>
        /// Converts the model contained in the stream to the specified format and save it to a file.
        /// </summary>
        /// <param name="inputStream">Stream to read from</param>
        /// <param name="importFormatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <param name="outputFilename">Output file name to export to</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <returns>True if the conversion was successful or not, false otherwise.</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public bool ConvertFromStreamToFile(Stream inputStream, String importFormatHint, String outputFilename, String exportFormatId) {
            return ConvertFromStreamToFile(inputStream, importFormatHint, PostProcessSteps.None, outputFilename, exportFormatId, PostProcessSteps.None);
        }

        /// <summary>
        /// Converts the model contained in the stream to the specified format and save it to a file.
        /// </summary>
        /// <param name="inputStream">Stream to read from</param>
        /// <param name="importFormatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <param name="outputFilename">Output file name to export to</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <param name="exportProcessSteps">Pre processing steps used for the export</param>
        /// <returns>True if the conversion was successful or not, false otherwise.</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public bool ConvertFromStreamToFile(Stream inputStream, String importFormatHint, String outputFilename, String exportFormatId, PostProcessSteps exportProcessSteps) {
            return ConvertFromStreamToFile(inputStream, importFormatHint, PostProcessSteps.None, outputFilename, exportFormatId, exportProcessSteps);
        }

        /// <summary>
        /// Converts the model contained in the stream to the specified format and save it to a file.
        /// </summary>
        /// <param name="inputStream">Stream to read from</param>
        /// <param name="importFormatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <param name="importProcessSteps">Post processing steps used for import</param>
        /// <param name="outputFilename">Output file name to export to</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <param name="exportProcessSteps">Pre processing steps used for the export</param>
        /// <returns>True if the conversion was successful or not, false otherwise.</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public bool ConvertFromStreamToFile(Stream inputStream, String importFormatHint, PostProcessSteps importProcessSteps, String outputFilename, String exportFormatId, PostProcessSteps exportProcessSteps) {
            CheckDisposed();

            if(inputStream == null || inputStream.CanRead != true)
                throw new AssimpException("stream", "Can't read from the stream it's null or write-only");

            if(String.IsNullOrEmpty(importFormatHint))
                throw new AssimpException("formatHint", "Format hint is null or empty");

            IntPtr ptr = IntPtr.Zero;
            PrepareImport();

            try {
                ptr = AssimpLibrary.Instance.ImportFileFromStream(inputStream, importProcessSteps, importFormatHint, m_propStore);

                if(ptr == IntPtr.Zero)
                    throw new AssimpException("Error importing file: " + AssimpLibrary.Instance.GetErrorString());

                TransformScene(ptr);

                if(importProcessSteps != PostProcessSteps.None)
                    ptr = AssimpLibrary.Instance.ApplyPostProcessing(ptr, importProcessSteps);

                ReturnCode status = AssimpLibrary.Instance.ExportScene(ptr, exportFormatId, outputFilename, exportProcessSteps);

                return status == ReturnCode.Success;
            } finally {
                CleanupImport();

                if(ptr != IntPtr.Zero)
                    AssimpLibrary.Instance.ReleaseImport(ptr);
            }
        }

        #endregion

        #region Stream to Blob

        /// <summary>
        /// Converts the model contained in the stream to the specified format and save it to a data blob.
        /// </summary>
        /// <param name="inputStream">Stream to read from</param>
        /// <param name="importFormatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <returns>Data blob containing the exported scene in a binary form</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public ExportDataBlob ConvertFromStreamToBlob(Stream inputStream, String importFormatHint, String exportFormatId) {
            return ConvertFromStreamToBlob(inputStream, importFormatHint, PostProcessSteps.None, exportFormatId, PostProcessSteps.None);
        }

        /// <summary>
        /// Converts the model contained in the stream to the specified format and save it to a data blob.
        /// </summary>
        /// <param name="inputStream">Stream to read from</param>
        /// <param name="importFormatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <param name="exportProcessSteps">Pre processing steps used for the export</param>
        /// <returns>Data blob containing the exported scene in a binary form</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public ExportDataBlob ConvertFromStreamToBlob(Stream inputStream, String importFormatHint, String exportFormatId, PostProcessSteps exportProcessSteps) {
            return ConvertFromStreamToBlob(inputStream, importFormatHint, PostProcessSteps.None, exportFormatId, exportProcessSteps);
        }

        /// <summary>
        /// Converts the model contained in the stream to the specified format and save it to a data blob.
        /// </summary>
        /// <param name="inputStream">Stream to read from</param>
        /// <param name="importFormatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <param name="importProcessSteps">Post processing steps used for import</param>
        /// <param name="exportFormatId">Format id that specifies what format to export to</param>
        /// <param name="exportProcessSteps">Pre processing steps used for the export</param>
        /// <returns>Data blob containing the exported scene in a binary form</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the context has already been disposed of.</exception>
        public ExportDataBlob ConvertFromStreamToBlob(Stream inputStream, String importFormatHint, PostProcessSteps importProcessSteps, String exportFormatId, PostProcessSteps exportProcessSteps) {
            CheckDisposed();

            if(inputStream == null || inputStream.CanRead != true)
                throw new AssimpException("stream", "Can't read from the stream it's null or write-only");

            if(String.IsNullOrEmpty(importFormatHint))
                throw new AssimpException("formatHint", "Format hint is null or empty");

            IntPtr ptr = IntPtr.Zero;
            PrepareImport();

            try {
                ptr = AssimpLibrary.Instance.ImportFileFromStream(inputStream, importProcessSteps, importFormatHint, m_propStore);

                if(ptr == IntPtr.Zero)
                    throw new AssimpException("Error importing file: " + AssimpLibrary.Instance.GetErrorString());

                TransformScene(ptr);

                if(importProcessSteps != PostProcessSteps.None)
                    ptr = AssimpLibrary.Instance.ApplyPostProcessing(ptr, importProcessSteps);

                return AssimpLibrary.Instance.ExportSceneToBlob(ptr, exportFormatId, exportProcessSteps);
            } finally {
                CleanupImport();

                if(ptr != IntPtr.Zero)
                    AssimpLibrary.Instance.ReleaseImport(ptr);
            }
        }

        #endregion

        #endregion

        #region IOSystem

        /// <summary>
        /// Sets a custom file system implementation that is used by this importer. If it is null, then the default assimp file system
        /// is used instead.
        /// </summary>
        /// <param name="ioSystem">Custom file system implementation</param>
        public void SetIOSystem(IOSystem ioSystem) {
            if(ioSystem == null || ioSystem.IsDisposed)
                ioSystem = null;

            m_ioSystem = ioSystem;
        }

        /// <summary>
        /// Removes the currently set custom file system implementation from the importer.
        /// </summary>
        public void RemoveIOSystem() {
            m_ioSystem = null;
        }

        #endregion

        #region Format support

        /// <summary>
        /// Gets the model formats that are supported for export by Assimp.
        /// </summary>
        /// <returns>Export formats supported</returns>
        public ExportFormatDescription[] GetSupportedExportFormats() {
            if(m_exportFormats == null)
                m_exportFormats = AssimpLibrary.Instance.GetExportFormatDescriptions();

            return (ExportFormatDescription[]) m_exportFormats.Clone();
        }

        /// <summary>
        /// Gets the model formats that are supported for import by Assimp.
        /// </summary>
        /// <returns>Import formats supported</returns>
        public String[] GetSupportedImportFormats() {
            if(m_importFormats == null)
                m_importFormats = AssimpLibrary.Instance.GetExtensionList();

            return (String[]) m_importFormats.Clone();
        }

        /// <summary>
        /// Checks if the format extension (e.g. ".dae" or ".obj") is supported for import.
        /// </summary>
        /// <param name="format">Model format</param>
        /// <returns>True if the format is supported, false otherwise</returns>
        public bool IsImportFormatSupported(String format) {
            return AssimpLibrary.Instance.IsExtensionSupported(format);
        }

        /// <summary>
        /// Checks if the format extension (e.g. ".dae" or ".obj") is supported for export.
        /// </summary>
        /// <param name="format">Model format</param>
        /// <returns>True if the format is supported, false otherwise</returns>
        public bool IsExportFormatSupported(String format) {
            if(String.IsNullOrEmpty(format))
                return false;

            ExportFormatDescription[] exportFormats = GetSupportedExportFormats();

            if(format.StartsWith(".") && format.Length >= 2)
                format = format.Substring(1);

            foreach(ExportFormatDescription desc in exportFormats) {
                if(String.Equals(desc.FileExtension, format))
                    return true;
            }

            return false;
        }

        #endregion

        #region Configs

        /// <summary>
        /// Sets a configuration property to the context. This is only used during import.
        /// </summary>
        /// <param name="config">Config to set</param>
        public void SetConfig(PropertyConfig config) {
            if(config == null) {
                return;
            }
            String name = config.Name;
            m_configs[config.Name] = config;
        }

        /// <summary>
        /// Removes a set configuration property by name.
        /// </summary>
        /// <param name="configName">Name of the config property</param>
        public void RemoveConfig(String configName) {
            if(String.IsNullOrEmpty(configName)) {
                return;
            }
            PropertyConfig oldConfig;
            if(m_configs.TryGetValue(configName, out oldConfig)) {
                m_configs.Remove(configName);
            }
        }

        /// <summary>
        /// Removes all configuration properties from the context.
        /// </summary>
        public void RemoveConfigs() {
            m_configs.Clear();
        }

        /// <summary>
        /// Checks if the context has a config set by the specified name.
        /// </summary>
        /// <param name="configName">Name of the config property</param>
        /// <returns>True if the config is present, false otherwise</returns>
        public bool ContainsConfig(String configName) {
            if(String.IsNullOrEmpty(configName)) {
                return false;
            }
            return m_configs.ContainsKey(configName);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Disposes of resources held by the context. These include IO systems still attached.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; False to release only unmanaged resources.</param>
        protected void Dispose(bool disposing) {
            if(!m_isDisposed) {
                if(disposing) {
                    if(UsingCustomIOSystem)
                        m_ioSystem.Dispose();
                }
                m_isDisposed = true;
            }
        }

        #endregion

        #region Private methods

        private void CheckDisposed() {
            if(m_isDisposed)
                throw new ObjectDisposedException("Assimp Context has been disposed.");
        }

        //Build import transformation matrix
        private void BuildMatrix() {

            if(m_buildMatrix) {
                Matrix4x4 scale = Matrix4x4.FromScaling(new Vector3D(m_scale, m_scale, m_scale));
                Matrix4x4 xRot = Matrix4x4.FromRotationX(m_xAxisRotation * (float) (180.0d / Math.PI));
                Matrix4x4 yRot = Matrix4x4.FromRotationY(m_yAxisRotation * (float) (180.0d / Math.PI));
                Matrix4x4 zRot = Matrix4x4.FromRotationZ(m_zAxisRotation * (float) (180.0d / Math.PI));
                m_scaleRot = scale * ((xRot * yRot) * zRot);
            }

            m_buildMatrix = false;
        }

        //Transforms the root node of the scene and writes it back to the native structure
        private bool TransformScene(IntPtr scene) {
            BuildMatrix();

            try {
                if(!m_scaleRot.IsIdentity) {
                    AiScene aiScene = MemoryHelper.MarshalStructure<AiScene>(scene);
                    if(aiScene.RootNode == IntPtr.Zero)
                        return false;

                    IntPtr matrixPtr = MemoryHelper.AddIntPtr(aiScene.RootNode, MemoryHelper.SizeOf<AiString>()); //Skip over Node Name

                    Matrix4x4 matrix = MemoryHelper.Read<Matrix4x4>(matrixPtr); //Get the root transform
                    matrix = matrix * m_scaleRot; //Transform

                    //Write back to unmanaged mem
                    MemoryHelper.Write<Matrix4x4>(matrixPtr, ref matrix);

                    return true;
                }
            } catch(Exception) {

            }

            return false;
        }

        //Creates all property stores and sets their values
        private void CreateConfigs() {
            m_propStore = AssimpLibrary.Instance.CreatePropertyStore();

            foreach(KeyValuePair<String, PropertyConfig> config in m_configs) {
                config.Value.ApplyValue(m_propStore);
            }
        }

        //Destroys all property stores
        private void ReleaseConfigs() {
            if(m_propStore != IntPtr.Zero)
                AssimpLibrary.Instance.ReleasePropertyStore(m_propStore);
        }

        //Does all the necessary prep work before we import
        private void PrepareImport() {
            CreateConfigs();
        }

        //Does all the necessary cleanup work after we import
        private void CleanupImport() {
            ReleaseConfigs();

            //Noticed that sometimes Assimp doesn't call Close() callbacks always, so ensure we clean up those up here
            if(UsingCustomIOSystem) {
                m_ioSystem.CloseAllFiles();
            }
        }

        #endregion

    }
}
