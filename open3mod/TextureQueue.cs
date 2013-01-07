using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open3mod
{
    /// <summary>
    /// Class that handles asynchronous texture loading. Users enqueue
    /// their requests using Enqueue(), which takes a delegate that is
    /// invokes as soon as loading completes. 
    /// 
    /// In the current implementation, textures are loaded in FIFO order 
    /// using a single background thread.
    /// </summary>
    public static class TextureQueue
    {
        public delegate void CompletionCallback(string file, Image image);


        public static void Enqueue(string file, CompletionCallback callback)
        {
            
        }
    }
}
