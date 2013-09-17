///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [Tab.cs]
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
            Rendering,
            Failed
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
        /// Enumerates all the separator bars that can be dragged in order to
        /// resize viewports.
        /// </summary>
        public enum ViewSeparator
        {
            Horizontal = 0,
            Vertical,
            _Max,
            Both
        }

        /// <summary>
        /// Supported arrangements of 3D views. Right now only the number of
        /// 3d windows.
        /// </summary>
        public enum ViewMode
        {
            // values pertain to CoreSettings:DefaultViewMode!
            Single = 0,
            Two = 1,
            Four = 2
        }


        /// <summary>
        /// Current state of the tab. The state flag is maintained internally
        /// and switched to "Rendering" as soon as a scene is set. The initial
        /// state can be set using the constructor.
        /// </summary>
        public TabState State { get; private set; }


        /// <summary>
        /// Index of the currently active viewport
        /// </summary>
        public ViewIndex ActiveViewIndex = 0;


        /// <summary>
        /// Array of viewport objects. Entries are null until a viewport index is
        /// at least used once with the tab. After a viewport has been enabled once,
        /// the corresponding Viewport instance is retained so a viewport
        /// keeps its state when the user hides it and shows it again. 
        /// 
        /// Which viewport setup is currently active in the GUI is specified by the 
        /// ActiveViewMode property.
        /// </summary>
        public Viewport[] ActiveViews
        {
            get
            {
                if (_dirtySplit)
                {
                    ValidateViewportBounds();
                }
                return _activeViews;
            }
            set{
                _activeViews = value;
            }
        }

        private Viewport[] _activeViews = new Viewport[(int)ViewIndex._Max];

        /// <summary>
        /// Current view mode
        /// </summary>
        public ViewMode ActiveViewMode
        {
            get { return _activeViewMode; }
            set
            { 
                // hardcoded table of viewport sizes. This is the only location
                // so changing these constants is sufficient to adjust viewport defaults
                _activeViewMode = value;
                CoreSettings.CoreSettings.Default.DefaultViewMode = (int) value;
                switch(_activeViewMode)
                {
                    case ViewMode.Single:
                        ActiveViews = new []
                        {
                            new Viewport(new Vector4(0.0f, 0.0f, 1.0f, 1.0f), CameraMode.Orbit), 
                            null,
                            null,
                            null
                        };
                        break;
                    case ViewMode.Two:
                        ActiveViews = new []
                        {
                            new Viewport(new Vector4(0.0f, 0.0f, 1.0f, 0.5f), CameraMode.Orbit), 
                            null,
                            new Viewport(new Vector4(0.0f, 0.5f, 1.0f, 1.0f), CameraMode.X), 
                            null
                        };            
                        break;
                    case ViewMode.Four:
                        ActiveViews = new []
                        {
                            new Viewport(new Vector4(0.0f, 0.0f, 0.5f, 0.5f), CameraMode.Orbit), 
                            new Viewport(new Vector4(0.5f, 0.0f, 1.0f, 0.5f), CameraMode.Z),
                            new Viewport(new Vector4(0.0f, 0.5f, 0.5f, 1.0f), CameraMode.X),
                            new Viewport(new Vector4(0.5f, 0.5f, 1.0f, 1.0f), CameraMode.Y)
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
                Debug.Assert(State != TabState.Failed, "cannot recover from TabState.Failed");

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


        /// <summary>
        /// If the tab is in a failed state this contains the error message
        /// that describes the failure. Otherwise, this is an empty string.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }
       


        /// <summary>
        /// Unique ID of the tab. This is used to connect with the UI. The value 
        /// is set via the constructor and never changes.
        /// </summary>
        public readonly object Id;



        private Scene _activeScene;
        private string _errorMessage;
        private ViewMode _activeViewMode = ViewMode.Four;

        /// <summary>
        /// Position of the horizontal and vertical splits 
        /// in [MinimumViewportSplit,1-MinimumViewportSplit]
        /// </summary>
        private float _verticalSplitPos = 0.1f;
        private float _horizontalSplitPos = 0.1f;

        /// <summary>
        /// dirty flag for the recalculation of viewport bounds
        /// </summary>
        private bool _dirtySplit = true;


        /// <summary>
        /// Create an empty tab.
        /// <param name="id">Static id to associate with the tab, see "ID"</param>
        /// <param name="fileBeingLoaded">Specifies the file that is being loaded
        /// for this tab. If this is non-null, the state of the tab is set
        /// to TabState.Loading and the file name is stored in the File field.</param>
        /// </summary>
        public Tab(object id, string fileBeingLoaded)
        {
            var vm = CoreSettings.CoreSettings.Default.DefaultViewMode;
            if(vm <= 2 && vm >= 0)
            {
                ActiveViewMode = (ViewMode) vm;
            }
            else
            {
                ActiveViewMode = ViewMode.Four;
                CoreSettings.CoreSettings.Default.DefaultViewMode = (int) ViewMode.Four;
            }
            
            State = fileBeingLoaded == null ? TabState.Empty : TabState.Loading;
            File = fileBeingLoaded;
            Id = id;
        }


        /// <summary>
        /// Gets the ICameraController responsible for a particular view
        /// for the current active camera mode.
        /// </summary>
        /// <param name="targetView">View index</param>
        /// <returns>ICameraController or null if there is no implementation</returns>
        public ICameraController ActiveCameraControllerForView(ViewIndex targetView)
        {
            return ActiveViews[(int)targetView] == null ? null : ActiveViews[(int) targetView].ActiveCameraControllerForView();
        }


        public void Dispose()
        {
            if (ActiveScene != null)
            {
                ActiveScene.Dispose();
                ActiveScene = null;
            }

            GC.SuppressFinalize(this);
        }

#if DEBUG
        ~Tab()
        {
            // OpenTk is unsafe from here, explicit Dispose() is required.
            Debug.Assert(false);
        }
#endif

        /// <summary>
        /// Sets the tab to a permanent "failed to load" state. In this
        /// state, the tab keeps displaying an error message but nothing
        /// else. 
        /// </summary>
        /// <param name="message"></param>
        public void SetFailed(string message)
        {
            State = TabState.Failed;
            _activeScene = null;
            _errorMessage = message;
        }


        /// <summary>
        /// Changes the camera mode in the currently active view.
        /// </summary>
        /// <param name="cameraMode">New camera mode</param>
        public void ChangeActiveCameraMode(CameraMode cameraMode)
        {
            ChangeCameraModeForView(ActiveViewIndex, cameraMode);
        }


        /// <summary>
        /// Changes the camera mode for a view.
        /// </summary>
        /// <param name="viewIndex">index of the view.</param>
        /// <param name="cameraMode">New camera mode.</param>
        public void ChangeCameraModeForView(ViewIndex viewIndex, CameraMode cameraMode)
        {
            Debug.Assert(ActiveViews[(int)viewIndex] != null);
            var view = ActiveViews[(int) viewIndex];

            view.ChangeCameraModeForView(cameraMode);           
        }


        /// <summary>
        /// Converts a (mouse) hit position to a viewport index - in other words,
        /// it calculates the index of the viewport that is hit by a click
        /// on a given mouse position.
        /// </summary>
        /// <param name="x">Hit position x, in normalized [0,1] range</param>
        /// <param name="y">Hit position y, in normalized [0,1] range</param>
        /// <returns>Tab.ViewIndex._Max if the hit coordinate doesn't hit a
        /// viewport. If not, the ViewIndex of the tab that was hit.</returns>
        public ViewIndex GetViewportIndexHit(float x, float y)
        {
            var index = ViewIndex.Index0;
            foreach (var viewport in ActiveViews)
            {
                if (viewport == null)
                {
                    ++index;
                    continue;
                }

                var view = viewport.Bounds;

                if (x >= view.X && x <= view.Z &&
                    y >= view.Y && y <= view.W)
                {
                    break;
                }
                ++index;
            }
            return index;
        }


        /// <summary>
        /// Converts a mouse position to a viewport separator. It therefore
        /// checks whether the mouse is in a region where dragging viewport
        /// borders is possible.
        /// </summary>
        /// <param name="x">Mouse x, in relative coordinates</param>
        /// <param name="y">Mouse y, in relative coordinates</param>
        /// <returns>Tab.ViewSeparator._Max if the mouse coordinate doesn't hit a
        /// viewport separator. If not, the separator that was hit.</returns>
        public ViewSeparator GetViewportSeparatorHit(float x, float y)
        {
            if (_activeViewMode == ViewMode.Single)
            {
                return ViewSeparator._Max;
            }
            var vp = ActiveViews[0];
            Debug.Assert(vp != null);

            const float threshold = 0.01f;

            if (Math.Abs(x - vp.Bounds.Z) < threshold && _activeViewMode != ViewMode.Two)
            {
                if (Math.Abs(y - vp.Bounds.W) < threshold)
                {
                    return ViewSeparator.Both;
                }
                return ViewSeparator.Vertical;
            }
            if (Math.Abs(y - vp.Bounds.W) < threshold)
            {
                return ViewSeparator.Horizontal;
            }
            return ViewSeparator._Max;
        }

        private const float MinimumViewportSplit = 0.1f;


        /// <summary>
        /// Sets a new position for the horizontal split between viewports.
        /// 
        /// This is only possible (and otherwise ignored) if all the four viewports are enabled.
        /// </summary>
        /// <param name="f">New splitter bar position, in [0,1]. Positions outside
        ///   [MinimumViewportSplit,1-MinimumViewportSplit] are clamped.</param>
        public void SetViewportSplitH(float f)
        {
            if (ActiveViewMode != ViewMode.Four)
            {
                return;
            }

            if (f < MinimumViewportSplit)
            {
                f = MinimumViewportSplit;
            }
            else if (f > 1.0f-MinimumViewportSplit)
            {
                f = 1.0f-MinimumViewportSplit;
            }

            _horizontalSplitPos = f;
            _dirtySplit = true;
        }


        /// <summary>
        /// Sets a new position for the vertical split between viewports.
        /// 
        /// This is only possible (and otherwise ignored) if at least two viewports are enabled.
        /// </summary>
        /// <param name="f">New splitter bar position, in [0,1]. Positions outside
        ///   [MinimumViewportSplit,1-MinimumViewportSplit] are clamped.</param>
        public void SetViewportSplitV(float f)
        {
            if (ActiveViewMode != ViewMode.Two && ActiveViewMode != ViewMode.Four)
            {
                return;
            }
            if (f < MinimumViewportSplit)
            {
                f = MinimumViewportSplit;
            }
            else if (f > 1.0f - MinimumViewportSplit)
            {
                f = 1.0f - MinimumViewportSplit;
            }

            _verticalSplitPos = f;
            _dirtySplit = true;
        }


        /// <summary>
        /// Ensure every viewport bounds do not overlap the splitter in both directions
        /// </summary>
        private void ValidateViewportBounds()
        {
            Debug.Assert(_dirtySplit);
            foreach (var viewport in _activeViews.Where(viewport => viewport != null))
            {
                var b = viewport.Bounds;

                //set vertical split
                if (Math.Abs(b.Y - _verticalSplitPos) > 0.0f && b.Y >= MinimumViewportSplit * 0.99999f)
                {
                    b.Y = _verticalSplitPos;
                }
                else if (b.W <= 1.0f - MinimumViewportSplit * 0.99999f)
                {
                    b.W = _verticalSplitPos;
                }

                //set horizontal split
                if (Math.Abs(b.X - _horizontalSplitPos) > 0.0f && b.X >= MinimumViewportSplit * 0.99999f)
                {
                    b.X = _horizontalSplitPos;
                }
                else if (b.Z <= 1.0f - MinimumViewportSplit * 0.99999f)
                {
                    b.Z = _horizontalSplitPos;
                }
                viewport.Bounds = b;
            }
            _dirtySplit = false;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 