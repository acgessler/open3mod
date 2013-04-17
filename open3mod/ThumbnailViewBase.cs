using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        protected readonly List<TThumbnailType> Entries;
        private TThumbnailType _selectedEntry;


        protected ThumbnailViewBase(FlowLayoutPanel flow)
        {
            Flow = flow;
            Flow.AutoScroll = true;
            Entries = new List<TThumbnailType>();
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
        protected TThumbnailType AddEntry(TThumbnailType control)
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
            return control;
        }
    }
}
