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
using System.Collections.Generic;
using System.IO;
using Assimp.Configs;
using Assimp.Unmanaged;
using System.Runtime.InteropServices;

namespace Assimp {
    /// <summary>
    /// Assimp importer that will use Assimp to load a model into managed memory.
    /// </summary>
    public class AssimpImporter : IDisposable {
        private bool _isDisposed;
        private bool _verboseEnabled;
        private Dictionary<String, PropertyConfig> _configs;
        private List<LogStream> _logStreams;
        private Object sync = new Object();

        private float m_scale = 1.0f;
        private float m_xAxisRotation = 0.0f;
        private float m_yAxisRotation = 0.0f;
        private float m_zAxisRotation = 0.0f;
        private bool m_buildMatrix = false;
        private Matrix4x4 m_scaleRot = Matrix4x4.Identity;

        /// <summary>
        /// Gets if the importer has been disposed.
        /// </summary>
        public bool IsDisposed {
            get {
                return _isDisposed;
            }
        }

        /// <summary>
        /// Gets or sets the uniform scale for the model. This is multiplied
        /// with the existing root node's transform.
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
        /// with the existing root node's transform.
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
        /// with the existing root node's transform.
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
        /// with the existing root node's transform.
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
        /// Gets or sets if verbose logging should be enabled.
        /// </summary>
        public bool VerboseLoggingEnabled {
            get {
                return _verboseEnabled;
            }
            set {
                _verboseEnabled = value;
                AssimpMethods.EnableVerboseLogging(value);
            }
        }

        /// <summary>
        /// Gets the property configurations set to this importer.
        /// </summary>
        public Dictionary<String, PropertyConfig> PropertyConfigurations {
            get {
                return _configs;
            }
        }

        /// <summary>
        /// Gets the logstreams attached to this importer.
        /// </summary>
        public List<LogStream> LogStreams {
            get {
                return _logStreams;
            }
        }

        /// <summary>
        /// Constructs a new AssimpImporter.
        /// </summary>
        public AssimpImporter() {
            _configs = new Dictionary<String, PropertyConfig>();
            _logStreams = new List<LogStream>();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="AssimpImporter"/> is reclaimed by garbage collection.
        /// </summary>
        ~AssimpImporter() {
            Dispose(false);
        }

        /// <summary>
        /// Importers a model from the stream without running any post-process steps. The importer sets configurations
        /// and loads the model into managed memory, releasing the unmanaged memory used by Assimp. It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="formatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <returns>The imported scene</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if attempting to import a model if the importer has been disposed of</exception>
        public Scene ImportFileFromStream(Stream stream, String formatHint) {
            return ImportFileFromStream(stream, PostProcessSteps.None, formatHint);
        }

        /// <summary>
        /// Importers a model from the stream. The importer sets configurations
        /// and loads the model into managed memory, releasing the unmanaged memory used by Assimp. It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="postProcessFlags">Post processing flags, if any</param>
        /// <param name="formatHint">Format extension to serve as a hint to Assimp to choose which importer to use</param>
        /// <returns>The imported scene</returns>
        /// <exception cref="AssimpException">Thrown if the stream is not valid (null or write-only) or if the format hint is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if attempting to import a model if the importer has been disposed of</exception>
        public Scene ImportFileFromStream(Stream stream, PostProcessSteps postProcessFlags, String formatHint) {
            lock(sync) {
                if(_isDisposed) {
                    throw new ObjectDisposedException("Importer has been disposed.");
                }

                if(stream == null || stream.CanRead != true) {
                    throw new AssimpException("stream", "Can't read from the stream it's null or write-only");
                }

                if(String.IsNullOrEmpty(formatHint)) {
                    throw new AssimpException("formatHint", "Format hint is null or empty");
                }

                IntPtr ptr = IntPtr.Zero;
                try {

                    AttachLogs();
                    CreateConfigs();

                    ptr = AssimpMethods.ImportFileFromStream(stream, PostProcessSteps.None, formatHint);

                    if(ptr != IntPtr.Zero) {
                        TransformScene(ptr);

                        ptr = AssimpMethods.ApplyPostProcessing(ptr, postProcessFlags);
                    } 

                    if(ptr == IntPtr.Zero) {
                        throw new AssimpException("Error importing file: " + AssimpMethods.GetErrorString());
                    }

                    AiScene scene = MemoryHelper.MarshalStructure<AiScene>(ptr);
                    if((scene.Flags & SceneFlags.Incomplete) == SceneFlags.Incomplete) {
                        throw new AssimpException("Error importing file: Imported scene is incomplete. " + AssimpMethods.GetErrorString());
                    }

                    return new Scene(scene);
                } finally {

                    ReleaseConfigs();
                    DetatachLogs();

                    if(ptr != IntPtr.Zero) {
                        AssimpMethods.ReleaseImport(ptr);
                    }
                }
            }
        }

        /// <summary>
        /// Importers a model from the specified file without running any post-process steps. The importer sets configurations
        /// and loads the model into managed memory, releasing the unmanaged memory used by Assimp.
        /// </summary>
        /// <param name="file">Full path to the file</param>
        /// <returns>The imported scene</returns>
        /// <exception cref="AssimpException">Thrown if the file is valid or there was a general error in importing the model.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if attempting to import a model if the importer has been disposed of</exception>
        public Scene ImportFile(String file) {
            return ImportFile(file, PostProcessSteps.None);
        }

        /// <summary>
        /// Importers a model from the specified file. The importer sets configurations
        /// and loads the model into managed memory, releasing the unmanaged memory used by Assimp.
        /// </summary>
        /// <param name="file">Full path to the file</param>
        /// <param name="postProcessFlags">Post processing flags, if any</param>
        /// <returns>The imported scene</returns>
        /// <exception cref="AssimpException">Thrown if the file is valid or there was a general error in importing the model.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if attempting to import a model if the importer has been disposed of</exception>
        public Scene ImportFile(String file, PostProcessSteps postProcessFlags) {
            lock(sync) {
                if(_isDisposed) {
                    throw new ObjectDisposedException("Importer has been disposed.");
                }
                if(String.IsNullOrEmpty(file) || !File.Exists(file)) {
                    throw new AssimpException("file", "Filename is null or not valid.");
                }

                IntPtr ptr = IntPtr.Zero;
                try {

                    AttachLogs();
                    CreateConfigs();

                    ptr = AssimpMethods.ImportFile(file, PostProcessSteps.None);

                    if(ptr != IntPtr.Zero) {
                        TransformScene(ptr);

                        ptr = AssimpMethods.ApplyPostProcessing(ptr, postProcessFlags);
                    } 

                    if(ptr == IntPtr.Zero) {
                        throw new AssimpException("Error importing file: " + AssimpMethods.GetErrorString());
                    }

                    AiScene scene = MemoryHelper.MarshalStructure<AiScene>(ptr);
                    if((scene.Flags & SceneFlags.Incomplete) == SceneFlags.Incomplete) {
                        throw new AssimpException("Error importing file: Imported scene is incomplete. " + AssimpMethods.GetErrorString());
                    }

                    return new Scene(scene);
                } finally {

                    ReleaseConfigs();
                    DetatachLogs();

                    if(ptr != IntPtr.Zero) {
                        AssimpMethods.ReleaseImport(ptr);
                    }
                }
            }
        }

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

        private unsafe bool TransformScene(IntPtr scene) {
            BuildMatrix();

            try {
                if(!m_scaleRot.IsIdentity) {
                    IntPtr rootNode = Marshal.ReadIntPtr(MemoryHelper.AddIntPtr(scene, sizeof(uint))); //Skip over sceneflags

                    IntPtr matrixPtr = MemoryHelper.AddIntPtr(rootNode, Marshal.SizeOf(typeof(AiString))); //Skip over AiString

                    Matrix4x4 matrix = MemoryHelper.MarshalStructure<Matrix4x4>(matrixPtr); //Get the root transform

                    matrix = matrix * m_scaleRot; //Transform

                    //Save back to unmanaged mem
                    int index = 0;
                    for(int i = 1; i <= 4; i++) {
                        for(int j = 1; j <= 4; j++) {
                            float value = matrix[i, j];
                            byte[] bytes = BitConverter.GetBytes(value);
                            foreach(byte b in bytes) {
                                Marshal.WriteByte(matrixPtr, index, b);
                                index++;
                            }
                        }
                    }
                    return true;
                }
            } catch(Exception) {

            }
            return false;
        }

        /// <summary>
        /// Attaches a logging stream to the importer.
        /// </summary>
        /// <param name="logstream"></param>
        public void AttachLogStream(LogStream logstream) {
            if(logstream == null || _logStreams.Contains(logstream)) {
                return;
            }
            _logStreams.Add(logstream);
        }

        /// <summary>
        /// Detaches a logging stream from the importer.
        /// </summary>
        /// <param name="logStream"></param>
        public void DetachLogStream(LogStream logStream) {
            if(logStream == null) {
                return;
            }
            _logStreams.Remove(logStream);
        }

        /// <summary>
        /// Detaches all logging streams that are currently attached to the importer.
        /// </summary>
        public void DetachLogStreams() {
            foreach(LogStream stream in _logStreams) {
                stream.Detach();
            }
        }

        /// <summary>
        /// Gets the model formats that are supported by Assimp. Each
        /// format should follow this example: ".3ds"
        /// </summary>
        /// <returns>The format extensions that are supported</returns>
        public String[] GetSupportedFormats() {
            return AssimpMethods.GetExtensionList();
        }

        /// <summary>
        /// Checks of the format extension is supported. Example: ".3ds"
        /// </summary>
        /// <param name="formatExtension">Format extension</param>
        /// <returns></returns>
        public bool IsFormatSupported(String formatExtension) {
            return AssimpMethods.IsExtensionSupported(formatExtension);
        }

        /// <summary>
        /// Sets a configuration property to the importer.
        /// </summary>
        /// <param name="config">Config to set</param>
        public void SetConfig(PropertyConfig config) {
            if(config == null) {
                return;
            }
            String name = config.Name;
            if(!_configs.ContainsKey(name)) {
                _configs[name] = config;
            } else {
                _configs.Add(name, config);
            }
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
            if(!_configs.TryGetValue(configName, out oldConfig)) {
                _configs.Remove(configName);
            }
        }

        /// <summary>
        /// Removes all configuration properties from the importer.
        /// </summary>
        public void RemoveConfigs() {
            _configs.Clear();
        }

        /// <summary>
        /// Checks if the importer has a config set by the specified name.
        /// </summary>
        /// <param name="configName">Name of the config property</param>
        /// <returns>True if the config is present, false otherwise</returns>
        public bool ContainsConfig(String configName) {
            if(String.IsNullOrEmpty(configName)) {
                return false;
            }
            return _configs.ContainsKey(configName);
        }

        //Creates all property stores and sets their values
        private void CreateConfigs() {
            foreach(KeyValuePair<String, PropertyConfig> config in _configs) {
                config.Value.CreatePropertyStore();
            }
        }

        //Destroys all property stores
        private void ReleaseConfigs() {
            foreach(KeyValuePair<String, PropertyConfig> config in _configs) {
                config.Value.ReleasePropertyStore();
            }
        }

        //Attachs all logstreams to Assimp
        private void AttachLogs() {
            foreach(LogStream log in _logStreams) {
                log.Attach();
            }
        }

        //Detatches all logstreams from Assimp
        private void DetatachLogs() {
            foreach(LogStream log in _logStreams) {
                log.Detach();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing) {

            if(!_isDisposed) {
                if(disposing) {
                    //Dispose of managed resources
                }
                _isDisposed = true;
            }
        }
    }
}
