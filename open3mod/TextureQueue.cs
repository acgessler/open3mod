///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TextureQueue.cs]
// (c) 2012-2013, Open3Mod Contributors
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
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Collections;

namespace open3mod
{
    /// <summary>
    /// Class that handles asynchronous texture loading. Users enqueue
    /// their requests using Enqueue(), which takes a delegate that it
    /// invokes as soon as loading completes.
    /// In the current implementation, textures are loaded in FIFO order
    /// using a single background thread.
    /// </summary>
    public static class TextureQueue
    {
        public delegate void CompletionCallback(string file, Image image, string actualLocation, TextureLoader.LoadResult result);

        private abstract class Task
        {
            protected readonly CompletionCallback Callback;

            public abstract void Load();

            protected Task(CompletionCallback callback)
            {
                Callback = callback;
            }
        }


        private class TextureFromFileTask : Task
        {
            private readonly string _file;
            private readonly string _baseDir;

            public TextureFromFileTask (string file, string baseDir, CompletionCallback callback) 
                : base(callback)
            {
                _file = file;
                _baseDir = baseDir;
            }


            public override void Load()
            {
                var loader = new TextureLoader(_file, _baseDir);
                Callback(_file, loader.Image, loader.ActualLocation, loader.Result);
            }
        }


        private class TextureFromMemoryTask : Task
        {
            private readonly Assimp.EmbeddedTexture _dataSource;
            private readonly string _refName;

            public TextureFromMemoryTask(Assimp.EmbeddedTexture dataSource, string refName, CompletionCallback callback)
                : base(callback)
            {
                _dataSource = dataSource;
                _refName = refName;
            }

            public override void Load()
            {
                var loader = new EmbeddedTextureLoader(_dataSource);
                Callback(_refName, loader.Image, "", loader.Result);
            }
        }


        private static Thread _thread;
        // Back ported from a .net 4.5 BlockingCollection
        private static readonly Queue<Task> Queue = new Queue<Task>();

        /// <summary>
        /// Enqueues an from-file texture loading job to the queue.
        /// </summary>
        /// <param name="file">Path / file name of the texture to be loaded</param>
        /// <param name="baseDir">Base folder for the current scene. This is used
        ///    to locate the file in case it is not found at the exact path given.</param>
        /// <param name="callback">Callback to be invoked when loading is complete</param>
        public static void Enqueue(string file, string baseDir, CompletionCallback callback)
        {
            Debug.Assert(file != null);
            Debug.Assert(baseDir != null);
            Debug.Assert(callback != null);

            if (_thread == null)
            {
                StartThread();
            }

            lock(Queue)
            {
                Queue.Enqueue(new TextureFromFileTask(file, baseDir, callback));
                Monitor.Pulse(Queue);
            }
        }


        /// <summary>
        /// Enqueues an in-memory (embedded) texture loading job to the queue.
        /// </summary>
        /// <param name="dataSource">Assimp texture to read from</param>
        /// <param name="refName">Name to report to the callback</param>
        /// <param name="callback">Callback to be invoked when loading is complete</param>
        public static void Enqueue(Assimp.EmbeddedTexture dataSource, string refName, CompletionCallback callback)
        {
            Debug.Assert(dataSource != null);
            Debug.Assert(refName != null);
            Debug.Assert(callback != null);

            if (_thread == null)
            {
                StartThread();
            }

            lock (Queue)
            {
                Queue.Enqueue(new TextureFromMemoryTask(dataSource, refName, callback));
                Monitor.Pulse(Queue);
            }
        }


        public static void Terminate()
        {
            if (_thread == null || !_thread.IsAlive)
            {
                return;
            }

            lock (Queue)
            {
                while (Queue.Count != 0)
                {
                    Monitor.Wait(Queue);    
                }
            }
        }


        private static void StartThread()
        {
            Debug.Assert(_thread == null);
            _thread = new Thread(ThreadProc);
            _thread.Start();
        }


        private static void ThreadProc()
        {
            try
            {
                while (true)
                {
                    lock (Queue)
                    {
                        while (Queue.Count > 0)
                        {
                            var t = Queue.Dequeue();
                            t.Load();
                        }
                        Monitor.Wait(Queue);
                    }
                }
            }
            catch(ThreadInterruptedException)
            {
                
            }
            catch (InvalidOperationException)
            {

            }
        }       
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 