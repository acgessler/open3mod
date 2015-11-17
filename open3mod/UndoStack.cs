///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [UndoStack.cs]
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace open3mod
{
    public delegate void UndoDelegate();
    public delegate void RedoDelegate();
    public delegate void UpdateDelegate();

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

        /// <summary>
        /// Same as PushAndDo(desc, redo, undo) but also calls the specified UpdateDelegate after
        /// both the undo and redo operation.
        /// 
        /// This is to avoid code duplication if some UI elements needs to be updated in either case.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <param name="update"></param>
        public void PushAndDo(String description, RedoDelegate redo, UndoDelegate undo, UpdateDelegate update)
        {
            PushAndDo(description,
                () =>
                {
                    redo();
                    update();
                },
                () =>
                {
                    undo();
                    update();
                });
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
            var item = _stack[--_cursor];
            try
            {
                item.Undo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Failed to undo operation [{0}], error: [{1}]", item.Description, ex));
            }
        }

        /// <summary>
        /// Redoes a previously undone operation.
        /// </summary>
        public void Redo()
        {
            Debug.Assert(CanRedo());
            var item = _stack[_cursor++];
            try
            {
                item.Redo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Failed to perform operation [{0}], error: [{1}]", item.Description, ex));
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 