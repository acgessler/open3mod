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
    /// </summary>
    public class UiState
    {
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

        public bool RenderWireframe;
        public bool RenderTextured = true;
        public bool RenderLit = true;

        public bool ShowFps = true;

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
                            new Vector4(0.5f, 0.5f, 1.0f, 1.0f),
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
        public Scene ActiveScene = new Scene("../../../testdata/scenes/COLLADA.dae");


        /// <summary>
        /// Font to be used for textual overlays in 3D view (size ~ 12px)
        /// </summary>
        public readonly Font DefaultFont12;


        /// <summary>
        /// Font to be used for textual overlays in 3D view (size ~ 16px)
        /// </summary>
        public readonly Font DefaultFont16;


        public UiState()
        {
            DefaultFont12 = new Font(FontFamily.GenericSansSerif, 12);
            DefaultFont16 = new Font(FontFamily.GenericSansSerif, 16);

            ActiveViewMode = ViewMode.Single;
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
                        CameraImpls[(int)CamMode, (int)ActiveViewIndex] = null;
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
    }
}
