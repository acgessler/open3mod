///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [UIState.cs]
// (c) 2012-2013, Alexander C. Gessler
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
    public sealed class UiState : IDisposable
    {
      
        public bool RenderWireframe;
        public bool RenderTextured = true;
        public bool RenderLit = true;

        public bool ShowFps = true;

        public bool ShowBBs = false;
        public bool ShowNormals = false;
        public bool ShowSkeleton = false;

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
        /// Enumerate all tabs that contain valid scenes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tabs> TabsWithActiveScenes()
        {
            foreach (var tab in _main.UiState.Tabs)
            {
                if (tab.ActiveScene != null)
                {
                    yield return tab;
                }
            }
        }


        /// <summary>
        /// Enumerate all scenes that are active in some tab.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Scene> ActiveScenes()
        {
            foreach (var tab in TabsWithActiveScenes())
            {
                yield return tab.ActiveScene;
            }
        }  


        /// <summary>
        /// Get the Tab instance with a particular id 
        /// </summary>
        /// <param name="id">unique tab ID</param>
        /// <returns>null if there is no tab with this id</returns>
        public Tab TabForId(object id)
        {
            return Tabs.FirstOrDefault(ts => ts.Id == id);
        }


        /// <summary>
        /// Set a particular tab as selected. This sets the "ActiveTab"
        /// property to the tab object. This does *not* update the UI.
        /// </summary>
        /// <param name="id">Unique id of the tab to be selected</param>
        public void SelectTab(object id) 
        {
            foreach (Tab ts in Tabs)
            {
                if (ts.Id == id)
                {
                    ActiveTab = ts;
                    return;
                }
            }

            Debug.Assert(false, "tab with id not found: " + id.ToString());
        }


        public void SelectTab(Tab tab)
        {
            SelectTab(tab.Id);
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
                if (ts.Id == id)
                {
                    Debug.Assert(ActiveTab != ts, "active tab cannot be removed: " + id.ToString());
                    Tabs.Remove(ts);

                    // strictly necessary to Dispose() because this has OpenTk resources
                    ts.Dispose();
                    return;
                }
            }

            Debug.Assert(false, "tab with id not found: " + id.ToString());
        }


        public void RemoveTab(Tab tab)
        {
            RemoveTab(tab.Id);
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
            Debug.Assert(!Tabs.Contains(tab), "tab exists already:" + tab.Id.ToString());
            foreach (Tab ts in Tabs)
            {
                Debug.Assert(ts.Id != tab.Id, "tab id exists already: " + tab.Id.ToString());
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
            DefaultFont12 = new Font("Segoe UI", 12);
            DefaultFont16 = new Font("Segoe UI", 18);

            Tabs = new List<Tab> {defaultTab};

            ActiveTab = defaultTab;
        }


        public void Dispose()
        {
            foreach (var tab in Tabs)
            {
                tab.Dispose();
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 