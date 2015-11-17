///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [ThumbnailViewBase.cs]
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
    /// <summary>
    /// Defines base behavior for the thumbnail panels used in the materials and textures tabs
    /// </summary>
    /// <typeparam name="TThumbnailType">(User) control type that pertains to a single thumbnail</typeparam>
    public abstract class ThumbnailViewBase<TThumbnailType> where TThumbnailType : ThumbnailControlBase<TThumbnailType>
    {
        protected readonly FlowLayoutPanel Flow;
        // Not ordered - Flow is authoritative of order.
        protected readonly List<TThumbnailType> Entries;
        private TThumbnailType _selectedEntry;

        private ToolTip _toolTip;

        protected ThumbnailViewBase(FlowLayoutPanel flow)
        {
            Flow = flow;
            Flow.AutoScroll = false;
            Entries = new List<TThumbnailType>();

            _toolTip = new ToolTip();
        }


        public bool Empty
        {
            get { return Entries.Count == 0; }
        }


        /// <summary>
        /// Gets the selected thumbnail entry, or null if there is no selection.
        /// </summary>
        public TThumbnailType SelectedEntry
        {
            get { return _selectedEntry; }
        }


        /// <summary>
        /// Remove the given thumbnail entry from the view.
        /// </summary>
        /// <param name="thumb"></param>
        /// <returns>Previous index of the entry for re-insertion/undo purposes.</returns>
        public int RemoveEntry(TThumbnailType thumb)
        {
            var index = Flow.Controls.GetChildIndex(thumb);
            Entries.Remove(thumb);
            Flow.Controls.Remove(thumb);
            return index;
        }


        /// <summary>
        /// Makes a given entry in the thumbnail view the currently selected one
        /// </summary>
        /// <param name="thumb">Entry to select, must be contained in the thumbnail view</param>
        public void SelectEntry(TThumbnailType thumb)
        {
            Debug.Assert(Entries.Contains(thumb));

            if (thumb == SelectedEntry)
            {
                return;
            }

            thumb.IsSelected = true;

            if (_selectedEntry != null)
            {
                _selectedEntry.IsSelected = false;
            }
            _selectedEntry = thumb;
        }


        /// <summary>
        /// Ensure a specific thumbnail entry is visible (i.e. scroll so it is).
        /// </summary>
        /// <param name="thumb">Entry to scroll to, must be contained in the thumbnail view</param>
        public void EnsureVisible(TThumbnailType thumb)
        {
            Debug.Assert(Entries.Contains(thumb));
            Flow.ScrollControlIntoView(thumb);
        }


        /// <summary>
        /// Adds a given entry to the thumbnail view.
        /// </summary>
        /// <param name="control">Entry to be added, it may not be contained in the
        /// thumbnail view yet</param>
        /// <param name="index">Index at which to add the entry, -1 to add to end.</param>
        public TThumbnailType AddEntry(TThumbnailType control, int index = -1)
        {
            Debug.Assert(!Entries.Contains(control));

            control.Click += (sender, args) =>
            {
                var v = sender as TThumbnailType;
                if (v != null)
                {
                    SelectEntry(v);
                }
            };

            Entries.Add(control);
            Flow.Controls.Add(control);
            if (index != -1)
            {
                Flow.Controls.SetChildIndex(control, index);
            }

            control.OnSetTooltips(_toolTip);
            return control;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 