using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace open3mod
{
    public delegate void UndoDelegate();
    public delegate void RedoDelegate();

    public class UndoStackEntry
    {
        public string Description;
        public UndoDelegate Undo;
        public RedoDelegate Redo;
    };

    public class UndoStack
    {
        private readonly List<UndoStackEntry> _stack = new List<UndoStackEntry>();
        private int _cursor;


        /// <summary>
        /// Create an entry on the undo stack with the given delegates to undo and redo the operation.
        /// 
        /// Calls the "redo" delegate once.
        /// </summary>
        /// <param name="description">UI text, keep brief.</param>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        public void PushAndDo(String description, RedoDelegate redo, UndoDelegate undo)
        {
            if (_cursor == _stack.Count)
            {
                _stack.Add(new UndoStackEntry());
            }
            var entry = _stack[_cursor];
            entry.Description = description;
            entry.Redo = redo;
            entry.Undo = undo;
            redo();

            ++_cursor;
        }

        public bool CanUndo()
        {
            return _cursor > 0;
        }

        public bool CanRedo()
        {
            return _cursor < _stack.Count;
        }


        public string GetUndoDescription()
        {
            Debug.Assert(CanUndo());
            return _stack[_cursor - 1].Description;
        }


        public string GetRedoDescription()
        {
            Debug.Assert(CanRedo());
            return _stack[_cursor].Description;
        }

        /// <summary>
        /// Undoes the last operation.
        /// </summary>
        public void Undo()
        {
            Debug.Assert(CanUndo());
            _stack[--_cursor].Undo();
        }

        /// <summary>
        /// Redoes a previously undone operation.
        /// </summary>
        public void Redo()
        {
            Debug.Assert(CanRedo());
            _stack[_cursor++].Redo();
        }
    }
}
