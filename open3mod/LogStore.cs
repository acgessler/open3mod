///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [LogStore.cs]
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

using System.Collections.Generic;

namespace open3mod
{
    /// <summary>
    /// Stores a list of logging messages obtained from assimp
    /// Log messages are intercepted in this fashion to enable arbitrary
    /// filtering in the UI.
    /// </summary>
    public class LogStore
    {

        /// <summary>
        /// Enumeration of logging levels. Category.System is for logging
        /// messsages produced by viewer code while all other categories
        /// are strictly taken from assimp.
        /// </summary>
        public enum Category
        {
            Info = 0,
            Warn,
            Error,
            Debug,

            System
        }

        /// <summary>
        /// Represents a single logging entry along with a timestamp
        /// </summary>
        public struct Entry
        {
            public int ThreadId;
            public Category Cat;
            public string Message;
            public long Time;
        }



        public List<Entry> Messages
        {
            get { return _messages; }
        }


        /// <summary>
        /// Construct a fresh store for log messages
        /// </summary>
        /// <param name="capacity">Number of entries to be expected</param>
        public LogStore(int capacity = 200)
        {
            _messages = new List<Entry>(capacity);
        }


        public void Add(Category cat, string message, long time, int tid)
        {
            _messages.Add(new Entry() {Cat = cat, Message = message, Time = time, ThreadId = tid});
        }


        public void Drop()
        {
            _messages.Clear();
        }



        private readonly List<Entry> _messages;
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 