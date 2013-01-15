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
        public delegate void CompletionCallback(string file, Image image);

        private struct Task
        {
            public readonly string File;
            public readonly CompletionCallback Callback;

            public Task(string file, CompletionCallback callback) : this()
            {
                File = file;
                Callback = callback;
            }
        }

        private static Thread _thread;
        private static readonly Queue<Task> Queue = new Queue<Task>();
        private static readonly AutoResetEvent Event = new AutoResetEvent(false);


        public static void Enqueue(string file, CompletionCallback callback)
        {
            if (_thread == null)
            {
                StartThread();
            }

            lock (Queue) {
                Queue.Enqueue(new Task(file, callback));
            }
            Event.Set();
        }

        public static void Terminate()
        {
            if (_thread == null || !_thread.IsAlive)
            {
                return;
            }

            _thread.Interrupt();
            // note that completion callbacks may still be called
            // after this point. With the current implementation
            // of Texture it doesn't matter, though.
        }

        private static void StartThread()
        {
            _thread = new Thread(ThreadProc);
        }

        private static void ThreadProc()
        {
            try
            {
                while (true)
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
                    task.Callback(task.File, Image.FromFile(task.File));
                }
            }
            catch(ThreadInterruptedException)
            {
                
            }
        }       
    }
}
