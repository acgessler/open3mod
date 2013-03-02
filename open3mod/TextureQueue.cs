///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TextureQueue.cs]
// (c) 2012-2013, Alexander C. Gessler
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace open3mod
{
    /// <summary>
    /// Class that handles asynchronous texture loading. Users enqueue
    /// their requests using Enqueue(), which takes a delegate that it
    /// invokes as soon as loading completes. 
    /// 
    /// In the current implementation, textures are loaded in FIFO order 
    /// using a single background thread.
    /// </summary>
    public static class TextureQueue
    {
        public delegate void CompletionCallback(string file, Image image, TextureLoader.LoadResult result);

        private struct Task
        {
            public readonly string File;
            public readonly string BaseDir;
            public readonly CompletionCallback Callback;

            public Task(string file, string baseDir, CompletionCallback callback) : this()
            {
                File = file;
                BaseDir = baseDir;
                Callback = callback;
            }
        }

        private static Thread _thread;
        private static readonly Queue<Task> Queue = new Queue<Task>();
        private static readonly AutoResetEvent Event = new AutoResetEvent(false);

        private static volatile bool _break = false;

        public static void Enqueue(string file, string baseDir, CompletionCallback callback)
        {
            if (_thread == null)
            {
                StartThread();
            }

            lock (Queue) {
                Queue.Enqueue(new Task(file, baseDir, callback));
            }
            Event.Set();
        }

        public static void Terminate()
        {
            if (_thread == null || !_thread.IsAlive)
            {
                return;
            }

            _break = true;
            // note that completion callbacks may still be called
            // after this point. With the current implementation
            // of Texture it doesn't matter, though.
            Event.Set();
        }

        private static void StartThread()
        {
            _thread = new Thread(ThreadProc);
            _thread.Start();
        }

        private static void ThreadProc()
        {
            try
            {
                while (!_break)
                {
                
                    Task task;
                    try
                    {
                        lock (Queue) {
                            task = Queue.Dequeue();
                        }
                    }
                    catch(InvalidOperationException)
                    {
                        // empty queue, go sleep
                        Event.WaitOne();                      
                        continue;
                    }

                    // XXX support more file formats (such as dds, tga ..)
                    var loader = new TextureLoader(task.File, task.BaseDir);
                    task.Callback(task.File, loader.Image, loader.Result);
                }
            }
            catch(ThreadInterruptedException)
            {
                
            }
        }       
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 