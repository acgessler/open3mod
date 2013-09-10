using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Leap;

namespace open3mod
{
    class LeapListener : Listener
    {
        private Object thisLock = new Object();

        /// <summary>
        /// Identification of the parameters that should be smoothed out
        /// </summary>
        private enum SmoothedValues
        {
            Pitch = 0,
            X = 1,

            _Max = 2,
        };

        /// <summary>
        /// List holding the data to smooth the values,
        /// one list for each parameter in SmoothedValues (ENUM)
        /// </summary>
        private List<float>[] _valueHistory = new List<float>[(int) SmoothedValues._Max];

        /// <summary>
        /// How many values should be averaged
        /// </summary>
        int smoothingWindowSize = 20;

        /// <summary>
        /// reference to the MainWindow
        /// </summary>
        private MainWindow _mainWindow;

        public LeapListener(MainWindow mainwindow)
        {
            _mainWindow = mainwindow;
        }

        private void SafeWriteLine(String line)
        {
            lock (thisLock)
            {
                Console.WriteLine(line);
            }
        }

        public override void OnInit(Controller controller)
        {
            SafeWriteLine("Initialized");
        }

        public override void OnConnect(Controller controller)
        {
            SafeWriteLine("Connected");
            //controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
            //controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
            //controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            //controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
        }

        public override void OnDisconnect(Controller controller)
        {
            //Note: not dispatched when running in a debugger.
            SafeWriteLine("Disconnected");
        }

        public override void OnExit(Controller controller)
        {
            SafeWriteLine("Exited");
        }

        public override void OnFrame(Controller controller)
        {
            // Get the most recent frame and report some basic information
            Frame frame = controller.Frame();

            //SafeWriteLine("Frame id: " + frame.Id
            //            + ", timestamp: " + frame.Timestamp
            //            + ", hands: " + frame.Hands.Count
            //            + ", fingers: " + frame.Fingers.Count
            //            + ", tools: " + frame.Tools.Count
            //            + ", gestures: " + frame.Gestures().Count);

            if (!frame.Hands.IsEmpty)
            {
                // Get the first hand
                Hand hand = frame.Hands[0];
                Frame oldFrame = controller.Frame(10);
                Vector translation = hand.Translation(oldFrame);

                // Get the hand's normal vector and direction
                Vector normal = hand.PalmNormal;
                Vector direction = hand.Direction;

                _mainWindow.UiState.ActiveTab.ActiveCameraController.LeapInput( GetSmoothedValue(translation.x,SmoothedValues.X), translation.y, translation.z, GetSmoothedValue(direction.Pitch,SmoothedValues.Pitch), normal.Roll, direction.Yaw);

                //// Check if the hand has any fingers
                //FingerList fingers = hand.Fingers;
                //if (!fingers.Empty)
                //{
                //    // Calculate the hand's average finger tip position
                //    Vector avgPos = Vector.Zero;
                //    foreach (Finger finger in fingers)
                //    {
                //        avgPos += finger.TipPosition;
                //    }
                //    avgPos /= fingers.Count;
                //    SafeWriteLine("Hand has " + fingers.Count
                //                + " fingers, average finger tip position: " + avgPos);
                //}

                //// Get the hand's sphere radius and palm position
                //SafeWriteLine("Hand sphere radius: " + hand.SphereRadius.ToString("n2")
                //            + " mm, palm position: " + hand.PalmPosition);

                //// Get the hand's normal vector and direction
                //Vector normal = hand.PalmNormal;
                //Vector direction = hand.Direction;

                //// Calculate the hand's pitch, roll, and yaw angles
                //SafeWriteLine("Hand pitch: " + direction.Pitch * 180.0f / (float)Math.PI + " degrees, "
                //            + "roll: " + normal.Roll * 180.0f / (float)Math.PI + " degrees, "
                //            + "yaw: " + direction.Yaw * 180.0f / (float)Math.PI + " degrees");
            }

            //// Get gestures
            //GestureList gestures = frame.Gestures();
            //for (int i = 0; i < gestures.Count; i++)
            //{
            //    Gesture gesture = gestures[i];

            //    switch (gesture.Type)
            //    {
            //        case Gesture.GestureType.TYPECIRCLE:
            //            CircleGesture circle = new CircleGesture(gesture);

            //            // Calculate clock direction using the angle between circle normal and pointable
            //            String clockwiseness;
            //            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
            //            {
            //                //Clockwise if angle is less than 90 degrees
            //                clockwiseness = "clockwise";
            //            }
            //            else
            //            {
            //                clockwiseness = "counterclockwise";
            //            }

            //            float sweptAngle = 0;

            //            // Calculate angle swept since last frame
            //            if (circle.State != Gesture.GestureState.STATESTART)
            //            {
            //                CircleGesture previousUpdate = new CircleGesture(controller.Frame(1).Gesture(circle.Id));
            //                sweptAngle = (circle.Progress - previousUpdate.Progress) * 360;
            //            }

            //            SafeWriteLine("Circle id: " + circle.Id
            //                           + ", " + circle.State
            //                           + ", progress: " + circle.Progress
            //                           + ", radius: " + circle.Radius
            //                           + ", angle: " + sweptAngle
            //                           + ", " + clockwiseness);
            //            break;
            //        case Gesture.GestureType.TYPESWIPE:
            //            SwipeGesture swipe = new SwipeGesture(gesture);
            //            SafeWriteLine("Swipe id: " + swipe.Id
            //                           + ", " + swipe.State
            //                           + ", position: " + swipe.Position
            //                           + ", direction: " + swipe.Direction
            //                           + ", speed: " + swipe.Speed);
            //            break;
            //        case Gesture.GestureType.TYPEKEYTAP:
            //            KeyTapGesture keytap = new KeyTapGesture(gesture);
            //            SafeWriteLine("Tap id: " + keytap.Id
            //                           + ", " + keytap.State
            //                           + ", position: " + keytap.Position
            //                           + ", direction: " + keytap.Direction);
            //            break;
            //        case Gesture.GestureType.TYPESCREENTAP:
            //            ScreenTapGesture screentap = new ScreenTapGesture(gesture);
            //            SafeWriteLine("Tap id: " + screentap.Id
            //                           + ", " + screentap.State
            //                           + ", position: " + screentap.Position
            //                           + ", direction: " + screentap.Direction);
            //            break;
            //        default:
            //            SafeWriteLine("Unknown gesture type.");
            //            break;
            //    }
            //}

            if (!frame.Hands.IsEmpty || !frame.Gestures().IsEmpty)
            {
                SafeWriteLine("");
            }
        }

        private float GetSmoothedValue(float newvalue, SmoothedValues id)
        {
            List<float> list;
            if (_valueHistory[(int)id] == null)
            {
                //initialize it
                list = _valueHistory[(int)id] = new List<float>();
            }
            else
            {
                list = _valueHistory[(int)id];
            }

            if(list.Count >= smoothingWindowSize)
            {
                list.RemoveAt(0);
            }
            list.Add(newvalue);
            float average = list.Average();
            return average;
        }
    }
}
