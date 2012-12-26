using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace open3mod
{
    /// <summary>
    /// Utility to store global UI settings such as rendering options.
    /// A single instance of UiState is typically passed around to
    /// anybody who needs access to it.
    /// 
    /// UIState also maintains the central list of open tabs as well
    /// as the info which is active.
    /// </summary>
    public class UiState
    {
      
        public bool RenderWireframe;
        public bool RenderTextured = true;
        public bool RenderLit = true;

        public bool ShowFps = true;

        /// <summary>
        /// Current active tab. May never be null, there is always
        /// a selected tab, even if it is just the dummy tab.
        /// </summary>
        public Tab ActiveTab {
            get;
            private set;
        }

        /// <summary>
        /// Ordered list of tabs (order by creation)
        /// </summary>
        public readonly List<Tab> Tabs;



        /// <summary>
        /// Set a particular tab as selected. This sets the "ActiveTab"
        /// property to the tab object. This does *not* update the UI.
        /// </summary>
        /// <param name="id">Unique id of the tab to be selected</param>
        public void SelectTab(object id) 
        {
            foreach (Tab ts in Tabs)
            {
                if (ts.ID == id)
                {
                    ActiveTab = ts;
                    return;
                }
            }

            Debug.Assert(false, "tab with id not found: " + id.ToString());
        }


        public void SelectTab(Tab tab)
        {
            SelectTab(tab.ID);
        }


        /// <summary>
        /// Remove a particular tab. The tab need not be active (i.e. to
        /// remove a tab, one first needs to make sure another tab is
        /// selected. This also secures the invariant that there be 
        /// always at least one tab.
        /// </summary>
        /// <param name="id">Unique id of the tab to be removed</param>
        public void RemoveTab(object id)
        {
            foreach (Tab ts in Tabs)
            {
                if (ts.ID == id)
                {
                    Debug.Assert(ActiveTab != ts, "active tab cannot be removed: " + id.ToString());
                    Tabs.Remove(ts);
                    return;
                }
            }

            Debug.Assert(false, "tab with id not found: " + id.ToString());
        }


        public void RemoveTab(Tab tab)
        {
            RemoveTab(tab.ID);
        }


        /// <summary>
        /// Add a tab with a given ID. The selected tab is not changed by this
        /// operation.
        /// </summary>
        /// <param name="id">Tag object. The ID member of this tab must be
        /// unique for all tabs (during the entire lifetime of the tab).</param>
        public void AddTab(Tab tab)
        {
#if DEBUG
            Debug.Assert(!Tabs.Contains(tab), "tab exists already:" + tab.ID.ToString());
            foreach (Tab ts in Tabs)
            {
                Debug.Assert(ts.ID != tab.ID, "tab id exists already: " + tab.ID.ToString());
            }
#endif
            Tabs.Add(tab);
        }




        /// <summary>
        /// Font to be used for textual overlays in 3D view (size ~ 12px)
        /// </summary>
        public readonly Font DefaultFont12;


        /// <summary>
        /// Font to be used for textual overlays in 3D view (size ~ 16px)
        /// </summary>
        public readonly Font DefaultFont16;


        public UiState(Tab defaultTab)
        {
            DefaultFont12 = new Font(FontFamily.GenericSansSerif, 12);
            DefaultFont16 = new Font(FontFamily.GenericSansSerif, 16);

            Tabs = new List<Tab>();
            Tabs.Add(defaultTab);

            ActiveTab = defaultTab;
        }    
    }
}
