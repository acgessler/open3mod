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

namespace Assimp {
    /// <summary>
    /// Simple implementation of an IOSystem that searches for files on the disk. This implementation
    /// can be given a number of search directories that it will attempt to locate the file in first, before
    /// using the file path given by Assimp. That way, you can load models that have files distributed in a number of other
    /// directories besides the root model's.
    /// </summary>
    public class FileIOSystem : IOSystem {
        private List<DirectoryInfo> m_searchDirectories;

        /// <summary>
        /// Constructs a new FileIOSystem that does not have any search directories.
        /// </summary>
        public FileIOSystem() : this(null) { }

        /// <summary>
        /// Constructs a new FileIOSystem that uses the specified search directories.
        /// </summary>
        /// <param name="searchPaths">Search directories to search for files in</param>
        public FileIOSystem(params String[] searchPaths) {
            m_searchDirectories = new List<DirectoryInfo>();

            SetSearchDirectories(searchPaths);
        }

        /// <summary>
        /// Sets the search directories the FileIOSystem will use when searching for files.
        /// </summary>
        /// <param name="searchPaths">Directory paths</param>
        public void SetSearchDirectories(params String[] searchPaths) {
            m_searchDirectories.Clear();

            if(searchPaths != null && searchPaths.Length != 0) {
                foreach(String path in searchPaths) {
                    if(!String.IsNullOrEmpty(path) && Directory.Exists(path))
                        m_searchDirectories.Add(new DirectoryInfo(path));
                }
            }
        }

        /// <summary>
        /// Gets the search directories the FileIOSystem is using.
        /// </summary>
        /// <returns>Directory paths</returns>
        public String[] GetSearchDirectories() {
            List<String> searchPaths = new List<String>();

            foreach(DirectoryInfo dir in m_searchDirectories) {
                searchPaths.Add(dir.FullName);
            }

            return searchPaths.ToArray();
        }

        /// <summary>
        /// Opens a stream to a file.
        /// </summary>
        /// <param name="pathToFile">Path to the file</param>
        /// <param name="fileMode">Desired file access mode</param>
        /// <returns>The IO stream</returns>
        public override IOStream OpenFile(String pathToFile, FileIOMode fileMode) {
            return new FileIOStream(this, pathToFile, fileMode);
        }

        /// <summary>
        /// Finds the first file that matches the file name (name + extension) in the search paths.
        /// </summary>
        /// <param name="fileName">File name (+ extension) to search for</param>
        /// <param name="pathToFile">Found file path</param>
        /// <returns>True if the file was found, false otherwise</returns>
        public bool FindFile(String fileName, out String pathToFile)
        {
            pathToFile = null;

            if(String.IsNullOrEmpty(fileName) || m_searchDirectories.Count == 0)
                return false;

            foreach(DirectoryInfo dir in m_searchDirectories)
            {
                String fullPath = Path.Combine(dir.FullName, fileName);
                if(File.Exists(fullPath)) {
                    pathToFile = fullPath;
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Wraps a FileStream.
    /// </summary>
    internal class FileIOStream : IOStream {
        private FileIOSystem m_parent;
        private FileStream m_fileStream;

        public override bool IsValid {
            get {
                return m_fileStream != null;
            }
        }

        public FileIOStream(FileIOSystem parent, String pathToFile, FileIOMode fileMode)
            : base(pathToFile, fileMode) {
            m_parent = parent;

            switch(fileMode) {
                case FileIOMode.Read:
                case FileIOMode.ReadBinary:
                case FileIOMode.ReadText:
                    OpenRead(pathToFile, fileMode);
                    break;
                case FileIOMode.Write:
                case FileIOMode.WriteBinary:
                case FileIOMode.WriteText:
                    OpenWrite(pathToFile, fileMode);
                    break;
            }
        }

        public override long Write(byte[] dataToWrite, long count) {
            if(dataToWrite == null)
                throw new ArgumentOutOfRangeException("dataToWrite", "Data to write cannot be null.");

            if(count < 0 || dataToWrite.Length < count)
                throw new ArgumentOutOfRangeException("count", "Number of bytes to write is greater than data size.");

            if(m_fileStream == null || !m_fileStream.CanWrite)
                throw new IOException("Stream is not writable.");

            m_fileStream.Write(dataToWrite, (int) m_fileStream.Position, (int) count);

            return count;
        }

        public override long Read(byte[] dataRead, long count) {
            if(dataRead == null)
                throw new ArgumentOutOfRangeException("dataRead", "Array to store data in cannot be null.");

            if(count < 0 || dataRead.Length < count)
                throw new ArgumentOutOfRangeException("count", "Number of bytes to read is greater than data store size.");

            if(m_fileStream == null || !m_fileStream.CanRead)
                throw new IOException("Stream is not readable.");

            m_fileStream.Read(dataRead, (int) m_fileStream.Position, (int) count);

            return count;
        }

        public override ReturnCode Seek(long offset, Origin seekOrigin) {
            if(m_fileStream == null || !m_fileStream.CanSeek)
                throw new IOException("Stream does not support seeking.");

            SeekOrigin orig = SeekOrigin.Begin;
            switch(seekOrigin)
            {
                case Origin.Set:
                    orig = SeekOrigin.Begin;
                    break;
                case Origin.Current:
                    orig = SeekOrigin.Current;
                    break;
                case Origin.End:
                    orig = SeekOrigin.End;
                    break;
            }

            m_fileStream.Seek(offset, orig);

            return ReturnCode.Success;
            
        }

        public override long GetPosition() {
            if(m_fileStream == null)
                return -1;

            return m_fileStream.Position;
        }

        public override long GetFileSize() {
            if(m_fileStream == null)
                return 0;

            return m_fileStream.Length;
        }

        public override void Flush() {
            if(m_fileStream == null)
                return;

            m_fileStream.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed && disposing)
            {
                if(m_fileStream != null)
                    m_fileStream.Close();

                m_fileStream = null;

                base.Dispose(disposing);
            }
        }

        private void OpenRead(String pathToFile, FileIOMode fileMode) {
            String fileName = Path.GetFileName(pathToFile);

            String foundPath;
            if(m_parent.FindFile(fileName, out foundPath))
                pathToFile = foundPath;

            if(File.Exists(pathToFile))
                m_fileStream = File.OpenRead(pathToFile);
        }

        private void OpenWrite(String pathToFile, FileIOMode fileMode) {
            m_fileStream = File.OpenWrite(pathToFile);
        }
    }
}
