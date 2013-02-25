///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [Tab.cs]
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
    /// Represents a single tab in the UI. A tab always contains exactly one scene
    /// being rendered, a scene being loaded or no scene at all (the latter is the
    /// dummy tab that is initially open).
    /// 
    /// A scene is thus coupled to a tab and therefore owned by TabState. The 
    /// list of all tabs is maintained by UIState, which also knows which tab
    /// is active. 
    /// </summary>
    public sealed class Tab
    {
        /// <summary>
        /// Enum of all supported tab states.
        /// </summary>
        public enum TabState { 

            Empty = 0,
            Loading,
            Rendering
        }


        /// <summary>
        /// Enum of all supported camera modes.
        /// </summary>
        public enum CameraMode
        {
            Fps = 0,
            Orbit,
            _Max
        }

        /// <summary>
        /// Index all 3D views - there can be up to four 3D views at this time,
        /// but the rest of the codebase always works with _Max so it can be
        /// nicely adjusted simply by adding more indexes.
        /// </summary>
        public enum ViewIndex
        {
            Index0 = 0,
            Index1,
            Index2,
            Index3,
            _Max
        }

        /// <summary>
        /// Supported arrangements of 3D views. Right now only the number of
        /// 3d windows.
        /// </summary>
        public enum ViewMode
        {
            Single,
            Two,
            Four
        }


        /// <summary>
        /// Current state of the tab. The state flag is maintained internally
        /// and switched to "Rendering" as soon as a scene is set. The initial
        /// state can be set using the c'tor.
        /// </summary>
        public TabState State { get; private set; }
        

        public CameraMode CamMode = CameraMode.Orbit;
        public ViewIndex ActiveViewIndex = 0;

        private ViewMode _activeViewMode = ViewMode.Single;

        /// <summary>
        /// For each viewport the bottom-left and upper-right corners or null
        /// if the viewport is not currently active.
        /// </summary>
        public Vector4?[] ActiveViews = new Vector4?[(int) ViewIndex._Max];


        /// <summary>
        /// Current view mode
        /// </summary>
        public ViewMode ActiveViewMode
        {
            get { return _activeViewMode; }
            set
            { 
                // hardcoded table of viewport sizes. This is the only location
                // so changing these constants will suffice to adjust viewports
                _activeViewMode = value;
                switch(_activeViewMode)
                {
                    case ViewMode.Single:
                        ActiveViews = new Vector4?[]
                        {
                            new Vector4(0.0f, 0.0f, 1.0f, 1.0f), 
                            null,
                            null,
                            null
                        };
                        break;
                    case ViewMode.Two:
                        ActiveViews = new Vector4?[]
                        {
                            new Vector4(0.0f, 0.0f, 0.5f, 1.0f), 
                            null,
                            new Vector4(0.5f, 0.0f, 1.0f, 1.0f), 
                            null
                        };            
                        break;
                    case ViewMode.Four:
                        ActiveViews = new Vector4?[]
                        {
                            new Vector4(0.0f, 0.0f, 0.5f, 0.5f), 
                            new Vector4(0.5f, 0.0f, 1.0f, 0.5f),
                            new Vector4(0.0f, 0.5f, 0.5f, 1.0f),
                            new Vector4(0.5f, 0.5f, 1.0f, 1.0f)
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Debug.Assert(ActiveViews[0] != null);
                if (ActiveViews[(int)ActiveViewIndex] == null)
                {
                    ActiveViewIndex = ViewIndex.Index0;
                }
            }
        }


        /// <summary>
        /// Camera controllers maintain state even when they are not active, 
        /// therefore we need to keep a camera controller for every
        /// view index X camera mode.
        /// </summary>
        public ICameraController[,] CameraImpls = new ICameraController[(int)CameraMode._Max,(int)ViewIndex._Max];


        /// <summary>
        /// Obtain an instance of the current active camera controller (i.e.
        /// the controller for the current active view and current active camera
        /// mode. This may be a null.
        /// </summary>
        public ICameraController ActiveCameraController {
            get { return ActiveCameraControllerForView(ActiveViewIndex); }
        }

        /// <summary>
        /// Current active scene
        /// </summary>
        public Scene ActiveScene
        {
            get { return _activeScene; }
            set
            {
                // make sure the previous scene instance is properly disposed
                if (_activeScene != null)
                {
                    _activeScene.Dispose();
                }
                _activeScene = value;

                // switch state to "Rendering" if the new scene is non-null
                if (_activeScene == null)
                {
                    State = TabState.Empty;
                }
                else
                {
                    State = TabState.Rendering;
                }
            }
        }


        /// <summary>
        /// File name of the scene in the tab. This member is already set while
        /// the scene is loading and "ActiveScene" is null. This field is null
        /// if the tab is in state TabState.Empty.
        /// </summary>
        public string File { get; private set; }


        private Scene _activeScene;


        /// <summary>
        /// Unique ID of the tab. This is used to connect with the UI. The value 
        /// is set via the c'tor and never changes.
        /// </summary>
        public readonly object ID;


        /// <summary>
        /// Create an empty tab.
        /// <param name="id">Static id to associate with the tab, see "ID"</param>
        /// <param name="fileBeingLoaded">Specifies the file that is being loaded
        /// for this tab. If this is non-null, the state of the tab is set
        /// to TabState.Loading and the file name is stored in the File field.</param>
        /// </summary>
        public Tab(object id, string fileBeingLoaded)
        {
            ActiveViewMode = ViewMode.Single;
            State = fileBeingLoaded == null ? TabState.Empty : TabState.Loading;

            File = fileBeingLoaded;

            ID = id;
        }


        /// <summary>
        /// Get the ICameraController responsible for a particular view
        /// for the current active camera mode.
        /// </summary>
        /// <param name="targetView">View index</param>
        /// <returns>ICameraController or null if there is no implementation</returns>
        public ICameraController ActiveCameraControllerForView(ViewIndex targetView)
        {
            if (CameraImpls[(int)CamMode, (int)targetView] == null)
            {
                switch (CamMode)
                {
                    case CameraMode.Fps:
                        CameraImpls[(int)CamMode, (int)ActiveViewIndex] = new FpsCameraController();
                        break;
                    case CameraMode.Orbit:
                        CameraImpls[(int)CamMode, (int)ActiveViewIndex] = new OrbitCameraController();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return CameraImpls[(int)CamMode, (int)targetView];
        }


        public void Dispose()
        {
            if (ActiveScene != null)
            {
                ActiveScene.Dispose();
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 