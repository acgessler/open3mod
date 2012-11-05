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
using System.Runtime.InteropServices;
using Assimp.Unmanaged;

namespace Assimp {

    /// <summary>
    /// Callback delegate for Assimp's LogStream.
    /// </summary>
    /// <param name="msg">Log message</param>
    /// <param name="userData">User data that is passed to the callback</param>
    public delegate void LogStreamCallback([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String msg, IntPtr userData);

    /// <summary>
    /// Represents a log stream, which receives all log messages and
    /// streams them somewhere.
    /// </summary>
    public sealed class LogStream {
        private AiLogStream _logStream;

        /// <summary>
        /// Callback that is called when a message is logged.
        /// </summary>
        public LogStreamCallback Callback {
            get {
                return _logStream.Callback;
            }
        }

        /// <summary>
        /// User data to be passed to the callback.
        /// </summary>
        public String UserData {
            get {
                return _logStream.UserData;
            }
        }

        /// <summary>
        /// Constructs a new LogStream.
        /// </summary>
        /// <param name="callback">Callback called when messages are logged.</param>
        public LogStream(LogStreamCallback callback) {
            _logStream = new AiLogStream(callback);
        }

        /// <summary>
        /// Constructs a new LogStream.
        /// </summary>
        /// <param name="callback">Callback called when messages are logged.</param>
        /// <param name="userData">User-supplied data</param>
        public LogStream(LogStreamCallback callback, String userData) {
            _logStream = new AiLogStream(callback, userData);
        }

        internal void Attach() {
            AssimpMethods.AttachLogStream(ref _logStream);
        }

        internal void Detach() {
            AssimpMethods.DetachLogStream(ref _logStream);
        }
    }
}
