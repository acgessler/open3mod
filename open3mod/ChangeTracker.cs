using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms.VisualStyles;

namespace open3mod
{
    /// <summary>
    /// Keeps track of changes in scene items.
    /// 
    /// Quick answers to these questions:
    ///   - Did X change since I last checked? (using GetDeltaChangeToken() / GetDeltaChange()).
    ///     Used by rendering/animation to determine changes since the last update.
    ///   - Did X change with respect to the original file (using GetChangeCount()).
    ///     Used to with Undo/Redo to highlight items that have been changed.
    /// Note that the design is atypical, usually a listener approach is employed to inform
    /// components about changes to data. However since C# has no deterministic destruction, it would
    /// be hard to guarantee that listeners are always unregistered. This would introduce subtle bugs
    /// and resource leaks.
    /// 
    /// Scene owns a central instance of ChangeTracker.
    /// </summary>
    public class ChangeTracker
    {  
        private class TrackedObject
        {
            public int TicksOfLastChange { get; set; }
            public int CountChanges { get; set; }
        }

        public class DeltaChangeToken
        {
            public DeltaChangeToken(int ticks)
            {
                Ticks = ticks;
            }
            public int Ticks { get; private set; }
        }

        
        private int _currentTime = 0;
        private readonly Dictionary<object, TrackedObject> _trackedObjects = new Dictionary<object, TrackedObject>(); 

     
        /// <summary>
        /// Return a token representing the current state of affairs.
        /// 
        /// Use GetDeltaChange(token) to retrieve any changes that happened since this
        /// token has been generated.
        /// </summary>
        /// <returns></returns>
        public DeltaChangeToken GetDeltaChangeToken()
        {
            return new DeltaChangeToken(Tick());
        }

        /// <summary>
        /// Get all changes since the |token| has been generated.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public HashSet<object> GetDeltaChange(DeltaChangeToken token)
        {
            HashSet<object> result = new HashSet<object>();
            lock (_trackedObjects)
            {
                // TODO(acgessler): Remember change history sorted by time so we don't have to look at all objects.             
                foreach (var kv in _trackedObjects)
                {
                    if (kv.Value.TicksOfLastChange > token.Ticks)
                    {
                        result.Add(kv.Key);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get the number of forward (i.e. non-undo) changes record for |obj|.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>0 if the object has not been used with ChangeTracker before</returns>
        public int GetChangeCount(object obj)
        {
            lock (_trackedObjects)
            {
                return _trackedObjects[obj].CountChanges;
            }
        }

        /// <summary>
        /// Track a change to |obj|.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isUndo">If true, the change counter of the object is decremented,
        ///    otherwise incremented.</param>
        public void Change(object obj, bool isUndo = false)
        {
            lock (_trackedObjects)
            {
                if (!_trackedObjects.ContainsKey(obj))
                {
                    _trackedObjects[obj] = new TrackedObject();
                }
                var entry = _trackedObjects[obj];
                entry.TicksOfLastChange = Tick();
                entry.CountChanges += (isUndo ? -1 : 1);
            }
        }

        /// <summary>
        /// Increment the logical "tick" clock.
        /// </summary>
        /// <returns></returns>
        private int Tick()
        {
            return Interlocked.Increment(ref _currentTime);
        }
    }
}

