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
using System.Runtime.InteropServices;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Defines a custom IO handler that can be registered to an importer that will handle I/O for assimp. This includes searching/opening
    /// files to read during import, and creating/writing to files during export.
    /// </summary>
    public abstract class IOSystem : IDisposable {
        private AiFileOpenProc m_openProc;
        private AiFileCloseProc m_closeProc;
        private IntPtr m_fileIOPtr;
        private bool m_isDisposed;
        private Dictionary<IntPtr, IOStream> m_openedFiles;

        /// <summary>
        /// Gets whether or not this IOSystem has been disposed.
        /// </summary>
        public bool IsDisposed {
            get {
                return m_isDisposed;
            }
        }

        /// <summary>
        /// Gets the number of currently opened streams.
        /// </summary>
        public int OpenFileCount {
            get {
                return m_openedFiles.Count;
            }
        }

        internal IntPtr AiFileIO {
            get {
                return m_fileIOPtr;
            }
        }

        /// <summary>
        /// Constructs a new IOSystem.
        /// </summary>
        public IOSystem() {
            m_openProc = OnAiFileOpenProc;
            m_closeProc = OnAiFileCloseProc;

            AiFileIO fileIO;
            fileIO.OpenProc = Marshal.GetFunctionPointerForDelegate(m_openProc);
            fileIO.CloseProc = Marshal.GetFunctionPointerForDelegate(m_closeProc);
            fileIO.UserData = IntPtr.Zero;

            m_fileIOPtr = MemoryHelper.AllocateMemory(MemoryHelper.SizeOf<AiFileIO>());
            Marshal.StructureToPtr(fileIO, m_fileIOPtr, false);

            m_openedFiles = new Dictionary<IntPtr, IOStream>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="IOSystem"/> class.
        /// </summary>
        ~IOSystem() {
            Dispose(false);
        }

        /// <summary>
        /// Opens a stream to a file.
        /// </summary>
        /// <param name="pathToFile">Path to the file</param>
        /// <param name="fileMode">Desired file access mode</param>
        /// <returns>The IO stream</returns>
        public abstract IOStream OpenFile(String pathToFile, FileIOMode fileMode);

        /// <summary>
        /// Closes a stream that is owned by this IOSystem.
        /// </summary>
        /// <param name="stream">Stream to close</param>
        public virtual void CloseFile(IOStream stream) {
            if(stream == null)
                return;

            if(m_openedFiles.ContainsKey(stream.AiFile)) {
                m_openedFiles.Remove(stream.AiFile);

                if(!stream.IsDisposed)
                    stream.Close();
            }
        }

        /// <summary>
        /// Closes all outstanding streams owned by this IOSystem.
        /// </summary>
        public virtual void CloseAllFiles() {
            foreach(KeyValuePair<IntPtr, IOStream> kv in m_openedFiles) {
                if(!kv.Value.IsDisposed)
                    kv.Value.Close();
            }
            m_openedFiles.Clear();
        }

        /// <summary>
        /// Disposes of all resources held by this object.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; False to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if(!m_isDisposed) {
                if(m_fileIOPtr != IntPtr.Zero) {
                    MemoryHelper.FreeMemory(m_fileIOPtr);
                    m_fileIOPtr = IntPtr.Zero;
                }

                if(disposing) {
                    m_openProc = null;
                    m_closeProc = null;
                    CloseAllFiles();
                }
                m_isDisposed = true;
            }
        }

        private IntPtr OnAiFileOpenProc(IntPtr fileIO, String pathToFile, String mode) {
            if(m_fileIOPtr != fileIO)
                return IntPtr.Zero;

            FileIOMode fileMode = ConvertFileMode(mode);
            IOStream iostream = OpenFile(pathToFile, fileMode);
            IntPtr aiFilePtr = IntPtr.Zero;

            if(iostream != null) {
                if(iostream.IsValid) {
                    aiFilePtr = iostream.AiFile;
                    m_openedFiles.Add(aiFilePtr, iostream);
                } else {
                    iostream.Dispose();
                }
            }

            return aiFilePtr;
        }

        private void OnAiFileCloseProc(IntPtr fileIO, IntPtr file) {
            if(m_fileIOPtr != fileIO)
                return;

            IOStream iostream;
            if(m_openedFiles.TryGetValue(file, out iostream)) {
                CloseFile(iostream);
            }
        }

        private FileIOMode ConvertFileMode(String mode) {
            FileIOMode fileMode = FileIOMode.Read;

            switch(mode) {
                case "w":
                    fileMode = FileIOMode.Write;
                    break;
                case "wb":
                    fileMode = FileIOMode.WriteBinary;
                    break;
                case "wt":
                    fileMode = FileIOMode.WriteText;
                    break;
                case "r":
                    fileMode = FileIOMode.Read;
                    break;
                case "rb":
                    fileMode = FileIOMode.ReadBinary;
                    break;
                case "rt":
                    fileMode = FileIOMode.ReadText;
                    break;
            }

            return fileMode;
        }
    }
}
