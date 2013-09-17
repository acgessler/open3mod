///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [MainWindow.cs]
// (c) 2012-2013, Open3Mod Contributors
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

#define USE_APP_IDLE

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

using OpenTK;

using Leap;

namespace open3mod
{
    public partial class MainWindow : Form
    {
        private readonly UiState _ui;
        private Renderer _renderer;
        private readonly FpsTracker _fps;
        private LogViewer _logViewer;

        private delegate void DelegateSelectTab(TabPage tab);
        private readonly DelegateSelectTab _delegateSelectTab;

        private delegate void DelegatePopulateInspector(Tab tab);
        private readonly DelegatePopulateInspector _delegatePopulateInspector;

        private readonly Controller _leapController;
        private readonly LeapListener _leapListener;

        private readonly bool _initialized;

#if !USE_APP_IDLE
        private System.Windows.Forms.Timer _timer;
#endif


        public delegate void TabAddRemoveHandler (Tab tab, bool add);
        public event TabAddRemoveHandler TabChanged;

        public GLControl GlControl
        {
            get { return glControl1; }
        }

        public UiState UiState
        {
            get { return _ui; }
        }

        public FpsTracker Fps
        {
            get { return _fps; }
        }

        public Renderer Renderer
        {
            get { return _renderer; }
        }


        public const int MaxRecentItems = 12;


        public MainWindow()
        {        
            // create delegate used for asynchronous calls 
            _delegateSelectTab = SelectTab;
            _delegatePopulateInspector = PopulateInspector;
         
            InitializeComponent();
            _captionStub = Text;

            AddEmptyTab();           
   
            // initialize UI state shelf with a default tab
            _ui = new UiState(new Tab(_emptyTab, null));
            _fps = new FpsTracker();

            // sync global UI with UIState
            framerateToolStripMenuItem.Checked = toolStripButtonShowFPS.Checked = _ui.ShowFps;
            lightingToolStripMenuItem.Checked = toolStripButtonShowShaded.Checked = _ui.RenderLit;
            texturedToolStripMenuItem.Checked = toolStripButtonShowTextures.Checked = _ui.RenderTextured;
            wireframeToolStripMenuItem.Checked = toolStripButtonWireframe.Checked = _ui.RenderWireframe;
            showNormalVectorsToolStripMenuItem.Checked = toolStripButtonShowNormals.Checked = _ui.ShowNormals;
            showBoundingBoxesToolStripMenuItem.Checked = toolStripButtonShowBB.Checked = _ui.ShowBBs;
            showAnimationSkeletonToolStripMenuItem.Checked = toolStripButtonShowSkeleton.Checked = _ui.ShowSkeleton;

            // manually register the MouseWheel handler
            glControl1.MouseWheel += OnMouseMove;

            // intercept all key events sent to children
            KeyPreview = true;
            _initialized = true;

            InitRecentList();

            //LeapMotion Support
            _leapListener = new LeapListener(this as MainWindow);
            _leapController = new Controller(_leapListener);
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }


        /// <summary>
        /// Add an "empty" tab if it doesn't exist yet
        /// </summary>
        private void AddEmptyTab()
        {
            // create default tab
            tabControl1.TabPages.Add("empty");
            _emptyTab = tabControl1.TabPages[tabControl1.TabPages.Count-1];
            PopulateUITab(_emptyTab);
            ActivateUiTab(_emptyTab);

            // happens when being called from ctor
            if (_ui != null)
            {
                _ui.AddTab(new Tab(_emptyTab, null));
            }
        }


        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            MaybeShowDonationDialog();
            MaybeShowTipOfTheDay();
        }


        private static void MaybeShowTipOfTheDay()
        {
            if (CoreSettings.CoreSettings.Default.ShowTipsOnStartup)
            {
                var tip = new TipOfTheDayDialog();
                tip.ShowDialog();
            }
        }


        /// <summary>
        /// Initial value for the donation countdown - every time the application is launched,
        /// a counter is decremented. Upon reaching 0, the user is asked to donate.
        /// </summary>
        private const int DonationCounterStart = 4;


        private static void MaybeShowDonationDialog()
        {
            // first time: init donation countdown
            if (CoreSettings.CoreSettings.Default.DonationUseCountDown == 0)
            {
                CoreSettings.CoreSettings.Default.DonationUseCountDown = DonationCounterStart;
            }

            // -1 means the countdown is disabled, otherwise the dialog is shown when 0 is reached 
            if (CoreSettings.CoreSettings.Default.DonationUseCountDown != -1 &&
                --CoreSettings.CoreSettings.Default.DonationUseCountDown == 0)
            {
                CoreSettings.CoreSettings.Default.DonationUseCountDown = DonationCounterStart;
                var don = new DonationDialog();
                don.ShowDialog();
            }
        }


        private void PopulateUITab(TabPage ui)
        {
            var tui = new TabUiSkeleton();

            tui.Size = ui.ClientSize;
            tui.AutoSize = false;
            tui.Dock = DockStyle.Fill;
            ;
            ui.Controls.Add(tui);
        }


        private void ActivateUiTab(TabPage ui)
        {
            ((TabUiSkeleton)ui.Controls[0]).InjectGlControl(glControl1);
            if (_renderer != null)
            {
                _renderer.TextOverlay.Clear();
            }

            // add postfix to main window title
            if (UiState != null)
            {
                var tab = UiState.TabForId(ui);
                if (tab != null)
                {
                    if (!string.IsNullOrEmpty(tab.File))
                    {
                        Text = _captionStub + "  [" + tab.File + "]";
                    }
                    else
                    {
                        Text = _captionStub;
                    }
                }
            }
        }


        /// <summary>
        /// Open a new tab given a scene file to load. If the specified scene is
        /// already open in a tab, the existing
        /// tab is selected in the UI (if requested) and no tab is added.
        /// </summary>
        /// <param name="file">Source file</param>
        /// <param name="async">Specifies whether the data is loaded asynchr.</param>
        /// <param name="setActive">Specifies whether the newly added tab will
        /// be selected when the loading process is complete.</param>
        public void AddTab(string file, bool async = true, bool setActive = true)
        {
            AddRecentItem(file);

            // check whether the scene is already loaded
            for (int j = 0; j < tabControl1.TabPages.Count; ++j)
            {
                var tab = UiState.TabForId(tabControl1.TabPages[j]);
                Debug.Assert(tab != null);

                if(tab.File == file)
                {
                    // if so, activate its tab and return
                    if(setActive)
                    {
                        SelectTab(tabControl1.TabPages[j]);
                    }

                    return;
                }
            }
            
            var key = GenerateTabKey();
            tabControl1.TabPages.Add(key, GenerateTabCaption(file) + LoadingTitlePostfix);

            var ui = tabControl1.TabPages[key];
            ui.ToolTipText = file;
            tabControl1.ShowToolTips = true;

            PopulateUITab(ui);

            var t = new Tab(ui, file);
            UiState.AddTab(t);

            if (TabChanged != null)
            {
                TabChanged(t, true);
            }

            if (async)
            {
                var th = new Thread(() => OpenFile(t, setActive));
                th.Start();
            }
            else
            {
                OpenFile(t, setActive);
            }

            if (_emptyTab != null)
            {
                CloseTab(_emptyTab);
            }
        }


        /// <summary>
        /// Initially build the Recent-Files menu
        /// </summary>
        private void InitRecentList()
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            var v = CoreSettings.CoreSettings.Default.RecentFiles;
            if (v == null)
            {
                v = CoreSettings.CoreSettings.Default.RecentFiles = new StringCollection();
                CoreSettings.CoreSettings.Default.Save();
            }
            foreach (var s in v)
            {
                var tool = recentToolStripMenuItem.DropDownItems.Add(Path.GetFileName(s));
                var path = s;
                tool.Click += (sender, args) => AddTab(path);
            }
        }


        /// <summary>
        /// Add a new item to the Recent-Files menu and save it persistently
        /// </summary>
        /// <param name="file"></param>
        private void AddRecentItem(string file)
        {
            var recent = CoreSettings.CoreSettings.Default.RecentFiles;

            bool removed = false;
            int i = 0;
            foreach (var s in recent)
            {
                if (s == file)
                {
                    recentToolStripMenuItem.DropDownItems.RemoveAt(i);
                    recent.Remove(s);
                    removed = true;
                    break;
                }
                ++i;
            }

            if (!removed && recent.Count == MaxRecentItems)
            {
                recent.RemoveAt(recent.Count - 1);
            }

            recent.Insert(0, file);
            CoreSettings.CoreSettings.Default.Save();

            recentToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(
                Path.GetFileName(file), 
                null, 
                (sender, args) => AddTab(file)));
        }


        /// <summary>
        /// Generate a suitable caption to display on a scene tab given a
        /// file name. If multiple contains files with the same names,
        /// their captions are disambiguated.
        /// </summary>
        /// <param name="file">Full path to scene file</param>
        /// <returns></returns>
        private string GenerateTabCaption(string file)
        {
            var name = Path.GetFileName(file);
            for (int j = 0; j < tabControl1.TabPages.Count; ++j )
            {
                if (name == tabControl1.TabPages[j].Text)
                {
                    string numberedName = null;
                    for (int i = 2; numberedName == null; ++i)
                    {
                        numberedName = name + " (" + i + ")";
                        for (int k = 0; k < tabControl1.TabPages.Count; ++k)
                        {
                            if (numberedName == tabControl1.TabPages[k].Text)
                            {
                                numberedName = null;
                                break;
                            }
                        }                    
                    }
                    return numberedName;
                }
            }
            return name;
        }


        /// <summary>
        /// Close a given tab in the UI
        /// </summary>
        /// <param name="tab"></param>
        private void CloseTab(TabPage tab)
        {
            Debug.Assert(tab != null);

            // If this is the last tab, we need to add an empty tab before we remove it
            if (tabControl1.TabCount == 1)
            {
                if (CoreSettings.CoreSettings.Default.ExitOnTabClosing)
                {
                    Application.Exit();
                    return;
                }
                else
                {
                    AddEmptyTab();
                }
            }

            if (tab == tabControl1.SelectedTab)
            {
                // need to select another tab first
                for (var i = 0; i < tabControl1.TabCount; ++i) 
                {
                    if (tabControl1.TabPages[i] == tab)
                    {
                        continue;
                    }
                    SelectTab(tabControl1.TabPages[i]);
                    break;
                }
            }            

            // free all internal data for this scene
            UiState.RemoveTab(tab);

            // and drop the UI tab
            tabControl1.TabPages.Remove(tab);

            if (TabChanged != null)
            {
                TabChanged((Tab)tab.Tag, false);
            }

            if(_emptyTab == tab)
            {
                _emptyTab = null;
            }
        }


        /// <summary>
        /// Select a given tab in the UI
        /// </summary>
        /// <param name="tab"></param>
        public void SelectTab(TabPage tab)
        {
            Debug.Assert(tab != null);
            tabControl1.SelectedTab = tab;

            var outer = UiState.TabForId(tab);
            Debug.Assert(outer != null);

            if (outer.ActiveScene != null)
            {
                toolStripStatistics.Text = outer.ActiveScene.StatsString;
            }
            else
            {
                toolStripStatistics.Text = "";
            }
            // update internal housekeeping
            UiState.SelectTab(tab);

            // update UI check boxes
            var vm = _ui.ActiveTab.ActiveViewMode;
            fullViewToolStripMenuItem.CheckState = toolStripButtonFullView.CheckState = 
                vm == Tab.ViewMode.Single 
                ? CheckState.Checked 
                : CheckState.Unchecked;
            twoViewsToolStripMenuItem.CheckState = toolStripButtonTwoViews.CheckState = 
                vm == Tab.ViewMode.Two 
                ? CheckState.Checked 
                : CheckState.Unchecked;
            fourViewsToolStripMenuItem.CheckState = toolStripButtonFourViews.CheckState = 
                vm == Tab.ViewMode.Four 
                ? CheckState.Checked 
                : CheckState.Unchecked;

            // some other UI housekeeping, this also injects the GL panel into the tab
            ActivateUiTab(tab);
        }


        private static int _tabCounter;
        private TabPage _tabContextMenuOwner;
        private TabPage _emptyTab;
        private SettingsDialog _settings;
        private Tab.ViewSeparator _dragSeparator = Tab.ViewSeparator._Max;
        private string _captionStub;
        private const string LoadingTitlePostfix = " (loading)";
        private const string FailedTitlePostfix = " (failed)";

        private string GenerateTabKey()
        {
            return (++_tabCounter).ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Opens a particular 3D model and assigns it to a particular tab.
        /// May be called on a non-GUI-thread.
        /// </summary>
        private void OpenFile(Tab tab, bool setActive)
        {
            try
            {
                tab.ActiveScene = new Scene(tab.File);

            }
            catch(Exception ex)
            {
                tab.SetFailed(ex.Message);
            }

            if (!setActive)
            {
                return;
            }

            var updateTitle = new MethodInvoker(() =>
            {
                var t = (TabPage)tab.Id;
                if (!t.Text.EndsWith(LoadingTitlePostfix))
                {
                    return;
                }
                t.Text = t.Text.Substring(0,t.Text.Length - LoadingTitlePostfix.Length);

                if (tab.State == Tab.TabState.Failed)
                {
                    t.Text = t.Text + FailedTitlePostfix;
                }
            });

            // must use BeginInvoke() here to make sure it gets executed
            // on the thread hosting the GUI message pump. An exception
            // are potential calls coming from our own c'tor: at this
            // time the window handle is not ready yet and BeginInvoke()
            // is thus not available.
            if (!_initialized)
            {
                SelectTab((TabPage)tab.Id);
                PopulateInspector(tab);
                updateTitle();
            }
            else
            {
                BeginInvoke(_delegateSelectTab, new[] { tab.Id });
                BeginInvoke(_delegatePopulateInspector, new object[] { tab });
                BeginInvoke(updateTitle);
            }
        }


        /// <summary>
        /// Populate the inspector view for a given tab. This can be called
        /// as soon as the scene to be displayed is loaded.
        /// </summary>
        /// <param name="tab"></param>
        private void PopulateInspector(Tab tab)
        {
            var ui = UiForTab(tab);
            Debug.Assert(ui != null);

            var inspector = ui.GetInspector();
            inspector.SetSceneSource(tab.ActiveScene);
        }


        public TabUiSkeleton UiForTab(Tab tab)
        {
            return ((TabUiSkeleton) ((TabPage) tab.Id).Controls[0]);
        }


        private void ToolsToolStripMenuItemClick(object sender, EventArgs e)
        {

        }


        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            var ab = new About();
            ab.ShowDialog();
        }


        private void OnGlLoad(object sender, EventArgs e)
        {
            if (_renderer != null)
            {
                _renderer.Dispose();
            }

            _renderer = new Renderer(this);           

#if USE_APP_IDLE
            // register Idle event so we get regular callbacks for drawing
            Application.Idle += ApplicationIdle;
#else
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 20;
            _timer.Tick += ApplicationIdle;
            _timer.Start();
#endif
        }


        private void OnGlResize(object sender, EventArgs e)
        {
            if (_renderer == null) // safeguard in case glControl's Load() wasn't fired yet
            {
                return;
            }

            _renderer.Resize();
        }


        private void GlPaint(object sender, PaintEventArgs e)
        {
            if (_renderer == null) // safeguard in case glControl's Load() wasn't fired yet
            {
                return;
            }

            FrameRender();
        }


        private void ApplicationIdle(object sender, EventArgs e)
        {           
            if(IsDisposed)
            {
                return;
            }
#if USE_APP_IDLE
            while (glControl1.IsIdle)
#endif
            {
                FrameUpdate();
                FrameRender();
            }
        }


        private void FrameUpdate()
        {          
            _fps.Update();
            var delta = _fps.LastFrameDelta;

            _renderer.Update(delta);
            foreach(var tab in UiState.Tabs)
            {
                if (tab.ActiveScene != null)
                {
                    tab.ActiveScene.Update(delta, tab != UiState.ActiveTab);
                }
            }

            ProcessKeys();
        }


        private void FrameRender()
        {
            _renderer.Draw(_ui.ActiveTab);
            glControl1.SwapBuffers();
        }


        private void ToggleFps(object sender, EventArgs e)
        {
            _ui.ShowFps = !_ui.ShowFps;
            framerateToolStripMenuItem.Checked = toolStripButtonShowFPS.Checked = _ui.ShowFps;
        }


        private void ToggleShading(object sender, EventArgs e)
        {
            _ui.RenderLit = !_ui.RenderLit;
            lightingToolStripMenuItem.Checked = toolStripButtonShowShaded.Checked = _ui.RenderLit;
        }


        private void ToggleTextures(object sender, EventArgs e)
        {
            _ui.RenderTextured = !_ui.RenderTextured;
            texturedToolStripMenuItem.Checked = toolStripButtonShowTextures.Checked = _ui.RenderTextured;
        }


        private void ToggleWireframe(object sender, EventArgs e)
        {
            _ui.RenderWireframe = !_ui.RenderWireframe;
            wireframeToolStripMenuItem.Checked = toolStripButtonWireframe.Checked = _ui.RenderWireframe;
        }


        private void ToggleShowBb(object sender, EventArgs e)
        {
            _ui.ShowBBs = !_ui.ShowBBs;
            showBoundingBoxesToolStripMenuItem.Checked = toolStripButtonShowBB.Checked = _ui.ShowBBs;
        }


        private void ToggleShowNormals(object sender, EventArgs e)
        {
            _ui.ShowNormals = !_ui.ShowNormals;
            showNormalVectorsToolStripMenuItem.Checked = toolStripButtonShowNormals.Checked = _ui.ShowNormals;
        }


        private void ToggleShowSkeleton(object sender, EventArgs e)
        {
            _ui.ShowSkeleton = !_ui.ShowSkeleton;
            showAnimationSkeletonToolStripMenuItem.Checked = toolStripButtonShowSkeleton.Checked = _ui.ShowSkeleton;
        }


        private void ToggleFullView(object sender, EventArgs e)
        {
            if (UiState.ActiveTab.ActiveViewMode == Tab.ViewMode.Single)
            {
                return;
            }
            UiState.ActiveTab.ActiveViewMode = Tab.ViewMode.Single;

            toolStripButtonFullView.CheckState = CheckState.Checked;
            toolStripButtonTwoViews.CheckState = CheckState.Unchecked;
            toolStripButtonFourViews.CheckState = CheckState.Unchecked;

            fullViewToolStripMenuItem.CheckState = CheckState.Checked;
            twoViewsToolStripMenuItem.CheckState = CheckState.Unchecked;
            fourViewsToolStripMenuItem.CheckState = CheckState.Unchecked;
        }


        private void ToggleTwoViews(object sender, EventArgs e)
        {
            if (UiState.ActiveTab.ActiveViewMode == Tab.ViewMode.Two)
            {
                return;
            }
            UiState.ActiveTab.ActiveViewMode = Tab.ViewMode.Two;

            toolStripButtonFullView.CheckState = CheckState.Unchecked;
            toolStripButtonTwoViews.CheckState = CheckState.Checked;
            toolStripButtonFourViews.CheckState = CheckState.Unchecked;

            fullViewToolStripMenuItem.CheckState = CheckState.Unchecked;
            twoViewsToolStripMenuItem.CheckState = CheckState.Checked;
            fourViewsToolStripMenuItem.CheckState = CheckState.Unchecked;
        }


        private void ToggleFourViews(object sender, EventArgs e)
        {
            if (UiState.ActiveTab.ActiveViewMode == Tab.ViewMode.Four)
            {
                return;
            }
            UiState.ActiveTab.ActiveViewMode = Tab.ViewMode.Four;

            toolStripButtonFullView.CheckState = CheckState.Unchecked;
            toolStripButtonTwoViews.CheckState = CheckState.Unchecked;
            toolStripButtonFourViews.CheckState = CheckState.Checked;

            fullViewToolStripMenuItem.CheckState = CheckState.Unchecked;
            twoViewsToolStripMenuItem.CheckState = CheckState.Unchecked;
            fourViewsToolStripMenuItem.CheckState = CheckState.Checked;
        }


        private void OnTabSelected(object sender, TabControlEventArgs e)
        {
            var tab = tabControl1.SelectedTab;

            SelectTab(tab);
        }


        private void OnShowTabContextMenu(object sender, MouseEventArgs e)
        {
            // http://social.msdn.microsoft.com/forums/en-US/winforms/thread/e09d081d-a7f5-479d-bd29-44b6d163ebc8
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    // get the tab's rectangle area and check if it contains the mouse cursor
                    Rectangle r = tabControl1.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        // hack: store the owning tab so the event handlers for
                        // the context menu know on whom they operate
                        _tabContextMenuOwner = tabControl1.TabPages[i];
                        tabContextMenuStrip.Show(tabControl1, e.Location);                    
                    }
                }
            }
        }


        private void OnCloseTabFromContextMenu(object sender, EventArgs e)
        {
            Debug.Assert(_tabContextMenuOwner != null);
            CloseTab(_tabContextMenuOwner);
        }


        private void OnCloseAllTabsButThisFromContextMenu(object sender, EventArgs e)
        {
            Debug.Assert(_tabContextMenuOwner != null);
            while (tabControl1.TabCount > 1)
            {
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    if (_tabContextMenuOwner != tabControl1.TabPages[i])
                    {
                        CloseTab(tabControl1.TabPages[i]);
                    }
                }
            }
        }


        private void OnCloseTab(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                CloseTab(tabControl1.SelectedTab);
            }
        }


        private void OnFileMenuOpen(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var names = openFileDialog.FileNames;
                var first = true;
                foreach(var name in names)
                {
                    AddTab(name,true, first);
                    first = false;
                }
            }
        }


        private void OnFileMenuCloseAll(object sender, EventArgs e)
        {
            while(tabControl1.TabPages.Count > 1)
            {
                CloseTab(tabControl1.TabPages[0]);
            }
            CloseTab(tabControl1.TabPages[0]);
        }


        private void OnFileMenuRecent(object sender, EventArgs e)
        {

        }


        private void OnFileMenuQuit(object sender, EventArgs e)
        {
            Close();
        }


        private void OnCloseForm(object sender, FormClosedEventArgs e)
        {           
            UiState.Dispose();
            _renderer.Dispose();
        }


        private void OnShowSettings(object sender, EventArgs e)
        {
            if (_settings == null || _settings.IsDisposed)
            {
                _settings = new SettingsDialog {Main = this};
            }

            if(!_settings.Visible)
            {
                _settings.Show();
            }
        }


        public void CloseSettingsDialog()
        {
            Debug.Assert(_settings != null);
            _settings.Close();
            _settings = null;
        }


        private void OnExport(object sender, EventArgs e)
        {
            var exp = new ExportDialog();
            exp.Show();
        }


        private void OnDrag(object sender, DragEventArgs e)
        {
            // code based on http://www.codeproject.com/Articles/3598/Drag-and-Drop
            try
            {
                var a = (Array)e.Data.GetData(DataFormats.FileDrop);

                if (a != null && a.GetLength(0) > 0)
                {
                    for (int i = 0, count = a.GetLength(0); i < count; ++i)
                    {
                        var s = a.GetValue(i).ToString();

                        // check if the dragged file is a folder. In this case,
                        // we load all applicable files in the folder.

                        // TODO this means, files with no proper file extension
                        // won't load this way.
                        try
                        {
                            FileAttributes attr = File.GetAttributes(s);
                            if (attr.HasFlag(FileAttributes.Directory))
                            {
                                string[] formats;
                                using (var tempImporter = new Assimp.AssimpContext())
                                {
                                    formats = tempImporter.GetSupportedImportFormats();
                                }

                                string[] files = Directory.GetFiles(s);
                                foreach (var file in files)
                                {
                                    var ext = Path.GetExtension(file);
                                    if (ext == null)
                                    {
                                        continue;
                                    }
                                    var lowerExt = ext.ToLower();
                                    if (formats.Any(format => lowerExt == format))
                                    {
                                        AddTab(file);
                                    }
                                }
                                continue;
                            }
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch (Exception)
                        // ReSharper restore EmptyGeneralCatchClause
                        {
                            // ignore this - AddTab() handles the failure
                        }

                        // Call OpenFile asynchronously.
                        // Explorer instance from which file is dropped is not responding
                        // all the time when DragDrop handler is active, so we need to return
                        // immediately (of particular importance if OpenFile shows MessageBox).
                        AddTab(s);
                    }

                    // in the case Explorer overlaps this form
                    Activate();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in DragDrop function: " + ex.Message);
            }
        }


        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // only accept files for drag and drop
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }


        private void OnLoad(object sender, EventArgs e)
        {
            if (CoreSettings.CoreSettings.Default.Maximized)
            {
                WindowState = FormWindowState.Maximized;
                Location = CoreSettings.CoreSettings.Default.Location;
                Size = CoreSettings.CoreSettings.Default.Size;
            }
            else
            {
                var size = CoreSettings.CoreSettings.Default.Size;
                if (size.Width != 0) // first-time run
                {
                    Location = CoreSettings.CoreSettings.Default.Location;
                    Size = size;
                }
            }
        }


        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                CoreSettings.CoreSettings.Default.Location = RestoreBounds.Location;
                CoreSettings.CoreSettings.Default.Size = RestoreBounds.Size;
                CoreSettings.CoreSettings.Default.Maximized = true;
            }
            else
            {
                CoreSettings.CoreSettings.Default.Location = Location;
                CoreSettings.CoreSettings.Default.Size = Size;
                CoreSettings.CoreSettings.Default.Maximized = false;
            }
            CoreSettings.CoreSettings.Default.Save();

            //Cleanup LeapMotion Controller
            _leapController.RemoveListener(_leapListener);
            _leapController.Dispose();
        }


        // note: the methods below are in MainWindow_Input.cs
        // Windows Forms Designer keeps re-generating them though.

        partial void OnKeyDown(object sender, KeyEventArgs e);
        partial void OnKeyUp(object sender, KeyEventArgs e);
        partial void OnMouseDown(object sender, MouseEventArgs e);
        partial void OnMouseEnter(object sender, EventArgs e);
        partial void OnMouseLeave(object sender, EventArgs e);
        partial void OnMouseMove(object sender, MouseEventArgs e);
        partial void OnMouseUp(object sender, MouseEventArgs e);
        partial void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e);


        private void OnTipOfTheDay(object sender, EventArgs e)
        {
            var tip = new TipOfTheDayDialog();
            tip.ShowDialog();
        }


        private void OnDonate(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelDonate.LinkVisited = false;
            var donate = new DonationDialog();
            donate.ShowDialog();
        }


        private void OnSetFileAssociations(object sender, EventArgs e)
        {
            using (var imp = new Assimp.AssimpContext())
            {
                var list = imp.GetSupportedImportFormats();

                // do not associate .xml - it is too generic 
                var filteredList = list.Where(s => s != ".xml");           

                var listString = string.Join(", ", filteredList);
                if(DialogResult.OK == MessageBox.Show(this, "The following file extensions will be associated with open3mod: " + listString,
                    "Set file associations",
                    MessageBoxButtons.OKCancel))
                {
                    if (!FileAssociations.SetAssociations(list))
                    {
                        MessageBox.Show(this, "Failed to set file extensions","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(this, "File extensions have been successfully associated", "open3mod", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 