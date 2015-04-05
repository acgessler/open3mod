///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [LogPipe.cs]
// (c) 2012-2015, Open3Mod Contributors
//
// Licensed under the terms and conditions of the 3-clause BSD license. See
// the LICENSE file in the root folder of the repository for the details.
//
// HIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Utility class to generate an assimp logstream to capture the logging into a LogStore
    /// </summary>
    public class LogPipe : IDisposable
    {
        private readonly LogStore _logStore;
        private Stopwatch _timer;
        private LogStream _stream;

        public LogPipe(LogStore logStore)
        {
            _logStore = logStore;
            _stream = new LogStream(LogStreamCallback);
            _stream.Attach();

        }

        public LogStream GetStream()
        {
            return _stream;
        }


        public void Dispose()
        {
            _stream.Detach();
            _stream.Dispose();

            GC.SuppressFinalize(this);
        }


        private void LogStreamCallback(string msg, string userdata)
        {
            // Start timing with the very first logging messages. This
            // is relatively reliable because assimp writes a log header
            // as soon as it starts processing a file.
            if (_timer == null)
            {
                _timer = new Stopwatch();
                _timer.Start();

            }

            long millis = _timer.ElapsedMilliseconds;

            // Unfortunately, assimp-net does not wrap assimp's native
            // logging interfaces so log streams (which receive
            // pre-formatted messages) are the only way to capture
            // the logging. This means we have to recover the original
            // information (such as log level and the thread/job id) 
            // from the string contents.



            int start = msg.IndexOf(':');
            if (start == -1)
            {
                // this should not happen but nonetheless check for it
                //Debug.Assert(false);
                return;
            }

            var cat = LogStore.Category.Info;
            if (msg.StartsWith("Error, "))
            {
                cat = LogStore.Category.Error;
            }
            else if (msg.StartsWith("Debug, "))
            {
                cat = LogStore.Category.Debug;
            }
            else if (msg.StartsWith("Warn, "))
            {
                cat = LogStore.Category.Warn;
            }
            else if (msg.StartsWith("Info, "))
            {
                cat = LogStore.Category.Info;
            }
            else
            {
                // this should not happen but nonetheless check for it
                //Debug.Assert(false);
                return;
            }

            int startThread = msg.IndexOf('T');
            if (startThread == -1 || startThread >= start)
            {
                // this should not happen but nonetheless check for it
                //Debug.Assert(false);
                return;
            }

            int threadId = 0;
            int.TryParse(msg.Substring(startThread + 1, start - startThread - 1), out threadId);

            _logStore.Add(cat, msg.Substring(start + 1), millis, threadId);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 