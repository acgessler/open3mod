using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace open3mod
{
    // The portions of the MainWindow code that deal with handling input from keyboard
    // and mouse and dispatching it to camera controllers, implement viewport dragging etc.
    public partial class MainWindow
    {
        private void ProcessKeys()
        {
            var cam = UiState.ActiveTab.ActiveCameraController;
            if (cam == null)
            {
                return;
            }

            var dt = (float)_fps.LastFrameDelta;
            float x = 0.0f, y = 0.0f, z = 0.0f;

            var changed = false;

            if (_forwardPressed)
            {
                changed = true;
                z -= dt;
            }
            if (_backPressed)
            {
                changed = true;
                z += dt;
            }

            if (_rightPressed)
            {
                changed = true;
                x += dt;
            }
            if (_leftPressed)
            {
                changed = true;
                x -= dt;
            }

            if (_upPressed)
            {
                changed = true;
                y += dt;
            }
            if (_downPressed)
            {
                changed = true;
                y -= dt;
            }

            if (!changed)
            {
                return;
            }

            cam.MovementKey(x, y, z);
        }


        private void UpdateActiveViewIfNeeded(MouseEventArgs e)
        {
            // check which viewport has been hit and activate it
            _ui.ActiveTab.ActiveViewIndex = MousePosToViewportIndex(e.X, e.Y);
        }


        /// <summary>
        /// Converts a mouse position to a viewport index - in other words,
        /// it calculates the index of the viewport that is hit by a click
        /// on a given mouse position.
        /// </summary>
        /// <param name="x">Mouse x, in client (pixel) coordinates</param>
        /// <param name="y">Mouse y, in client (pixel) coordinates</param>
        /// <returns>Tab.ViewIndex._Max if the mouse coordinate doesn't hit a
        /// viewport. If not, the ViewIndex of the tab that was hit.</returns>
        private Tab.ViewIndex MousePosToViewportIndex(int x, int y)
        {
            var xf = x / (float)glControl1.ClientSize.Width;
            var yf = 1.0f - y / (float)glControl1.ClientSize.Height;

            return _ui.ActiveTab.GetViewportIndexHit(xf, yf);
        }



        private void SetViewportSplitH(float f)
        {
            _ui.ActiveTab.SetViewportSplitH(f);
        }


        private void SetViewportSplitV(float f)
        {
            _ui.ActiveTab.SetViewportSplitV(f);
        }


        /// <summary>
        /// Converts a mouse position to a viewport separator. It therefore
        /// checks whether the mouse is in a region where dragging viewport
        /// borders is possible.
        /// </summary>
        /// <param name="x">Mouse x, in client (pixel) coordinates</param>
        /// <param name="y">Mouse y, in client (pixel) coordinates</param>
        /// <returns>Tab.ViewSeparator._Max if the mouse coordinate doesn't hit a
        /// viewport separator. If not, the separator that was hit.</returns>
        private Tab.ViewSeparator MousePosToViewportSeparator(int x, int y)
        {
            var xf = x / (float)glControl1.ClientSize.Width;
            var yf = 1.0f - y / (float)glControl1.ClientSize.Height;

            return _ui.ActiveTab.GetViewportSeparatorHit(xf, yf);
        }


        private void OnShowLogViewer(object sender, EventArgs e)
        {
            if (_logViewer == null)
            {
                _logViewer = new LogViewer(this);
                _logViewer.Closed += (o, args) =>
                {
                    _logViewer = null;
                };
                _logViewer.Show();
            }
        }


        partial void OnMouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;

            _previousMousePosX = e.X;
            _previousMousePosY = e.Y;

            var sep = MousePosToViewportSeparator(e.X, e.Y);
            if (sep != Tab.ViewSeparator._Max)
            {
                // start dragging viewport separators
                _dragSeparator = sep;
                SetViewportDragCursor(sep);
            }

            // hack: the renderer handles the input for the HUD, so forward the event
            var index = MousePosToViewportIndex(e.X, e.Y);
            if (index == Tab.ViewIndex._Max)
            {
                return;
            }

            if (sep == Tab.ViewSeparator._Max)
            {
                var view = UiState.ActiveTab.ActiveViews[(int)index];
                Debug.Assert(view != null);
                _renderer.OnMouseClick(e, view.Bounds, index);
            }

            UpdateActiveViewIfNeeded(e);
        }


        partial void OnMouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
            if (!IsDraggingViewportSeparator)
            {
                return;
            }
            _dragSeparator = Tab.ViewSeparator._Max;
            Cursor = Cursors.Default;
        }


        public bool IsDraggingViewportSeparator
        {
            get { return _dragSeparator != Tab.ViewSeparator._Max; }
        }


        partial void OnMouseMove(object sender, MouseEventArgs e)
        {
            var sep = _dragSeparator != Tab.ViewSeparator._Max ? _dragSeparator : MousePosToViewportSeparator(e.X, e.Y);
            if (sep != Tab.ViewSeparator._Max)
            {
                // show resize cursor
                SetViewportDragCursor(sep);

                // and adjust viewport separators
                if (IsDraggingViewportSeparator)
                {
                    if (sep == Tab.ViewSeparator.Horizontal)
                    {
                        SetViewportSplitV(1.0f - e.Y / (float)glControl1.ClientSize.Height);
                    }
                    else if (sep == Tab.ViewSeparator.Vertical)
                    {
                        SetViewportSplitH(e.X / (float)glControl1.ClientSize.Width);
                    }
                    else if (sep == Tab.ViewSeparator.Both)
                    {
                        SetViewportSplitV(1.0f - e.Y / (float)glControl1.ClientSize.Height);
                        SetViewportSplitH(e.X / (float)glControl1.ClientSize.Width);
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                return;
            }

            Cursor = Cursors.Default;
            if (e.Delta != 0 && UiState.ActiveTab.ActiveCameraController != null)
            {
                UiState.ActiveTab.ActiveCameraController.Scroll(e.Delta);
            }


            // hack: the renderer handles the input for the HUD, so forward the event
            var index = MousePosToViewportIndex(e.X, e.Y);
            if (index == Tab.ViewIndex._Max)
            {
                return;
            }
            var view = UiState.ActiveTab.ActiveViews[(int)index];
            Debug.Assert(view != null);
            _renderer.OnMouseMove(e, view.Bounds, index);

            if (!_mouseDown)
            {
                return;
            }

            if (UiState.ActiveTab.ActiveCameraController != null)
            {
                UiState.ActiveTab.ActiveCameraController.MouseMove(e.X - _previousMousePosX, e.Y - _previousMousePosY);
            }
            _previousMousePosX = e.X;
            _previousMousePosY = e.Y;
        }


        private void SetViewportDragCursor(Tab.ViewSeparator sep)
        {
            switch (sep)
            {
                case Tab.ViewSeparator.Horizontal:
                    Cursor = Cursors.HSplit;
                    break;
                case Tab.ViewSeparator.Vertical:
                    Cursor = Cursors.VSplit;
                    break;
                case Tab.ViewSeparator.Both:
                    Cursor = Cursors.SizeAll;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        partial void OnMouseLeave(object sender, EventArgs e)
        {
            if (_mouseDown)
            {
                Capture = true;
            }

            Cursor = Cursors.Default;
        }


        partial void OnMouseEnter(object sender, EventArgs e)
        {
            Capture = false;
        }


        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }


        partial void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }


        partial void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.W:
                case Keys.Up:
                    _forwardPressed = true;
                    break;

                case Keys.A:
                case Keys.Left:
                    _leftPressed = true;
                    break;

                case Keys.S:
                case Keys.Down:
                    _backPressed = true;
                    break;

                case Keys.D:
                case Keys.Right:
                    _rightPressed = true;
                    break;

                case Keys.PageUp:
                    _upPressed = true;
                    break;

                case Keys.PageDown:
                    _downPressed = true;
                    break;
            }
        }


        partial void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.KeyData)
            {
                case Keys.W:
                case Keys.Up:
                    _forwardPressed = false;
                    break;

                case Keys.A:
                case Keys.Left:
                    _leftPressed = false;
                    break;

                case Keys.S:
                case Keys.Down:
                    _backPressed = false;
                    break;

                case Keys.D:
                case Keys.Right:
                    _rightPressed = false;
                    break;

                case Keys.PageUp:
                    _upPressed = false;
                    break;

                case Keys.PageDown:
                    _downPressed = false;
                    break;
            }
        }
    }
}
