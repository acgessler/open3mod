using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public Category Cat;
            public string Message;
            public double Time;
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


        public void Add(Category cat, string message, double time)
        {
            _messages.Add(new Entry() {Cat = cat, Message = message, Time = time});
        }



        private readonly List<Entry> _messages;
    }
}

