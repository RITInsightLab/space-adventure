using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using SpaceAdventure3.input.kinect.gesture;
using SpaceAdventure3.command;

using KinectMouseController;

using Microsoft.Kinect;


namespace SpaceAdventure3
{

    /**
     * This class takes in all Kinect motion and gesture input. It is responsible
     * for anything that happens in the program as a result of the user's physical movement.
     * 
     * Author: Ross Kahn
     **/
    public class KinectManager 
    {
        private static bool connected;      // Is the Kinect connected?
        private KinectSensor sensor;        // Kinect Sensor object

        // TODO: Clean up so these aren't needed
        private bool PRESS = true;          // Class constants
        private bool RELEASE = false;

        private bool clicking = false;      // Is the user currently 'clicking'?

        // Gestures and their associated system actions
        private Dictionary<BaseGesture, Constants.ACTION> gestureBindings;

        // The action, if any, that the user is doing right now
        private Constants.ACTION currentAction = Constants.ACTION.NEUTRAL;

        // Public getters
        public static bool isConnected() { return connected; }
        public KinectSensor getSensor() { return sensor; }

        #region Kinect Initialization

        /**
         * Default Constructor. Not connected by default (needs to be launched)
         **/
        public KinectManager()
        {
            connected = false;

            // Each defined gesture needs an associated ACTION
            gestureBindings = new Dictionary<BaseGesture, Constants.ACTION>();
            gestureBindings.Add(new HandsIn(), Constants.ACTION.ZOOMIN);
            gestureBindings.Add(new HandsOut(), Constants.ACTION.ZOOMOUT);
            gestureBindings.Add(new NeutralHands(), Constants.ACTION.NEUTRAL);
            gestureBindings.Add(new LeftHandUp(), Constants.ACTION.CLICK);
            gestureBindings.Add(new LeftHandDown(), Constants.ACTION.RELEASE_CLICK);

        }

        /**
         * Determine if the Kinect is hooked up to the system. If so, start the sensor
         **/
        public bool Initialize()
        {

            // Find the first available connected Kinect
            foreach (KinectSensor kinect in KinectSensor.KinectSensors)
            {
                if (kinect.Status == KinectStatus.Connected)
                {
                    sensor = kinect;
                    break;
                }
            }

            // No Kinect was found
            if (sensor == null)
            {
                Console.WriteLine("Unable to detect an available Kinect Sensor.\nPlease make sure you have a Kinect sensor plugged in.");
                return false;
            }

            // Event handler for status changed (like the kinect being disconnected)
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;

            // Start the sensor and configure it
            try
            {
                this.sensor.Start();
                this.SetupSensor();

            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
                connected = false;
                return false;
            }

            // Success
            Console.WriteLine("Kinect Sensor Connected");
            connected = true;
            return true;

        }

        /**
         * Enabled the skeleton stream. The depth and color streams are not
         * needed in this application. Add an event listener for when a skeleton
         * is ready to be analyzed
         **/
        private void SetupSensor()
        {
            var parameters = new TransformSmoothParameters
            {
                Correction = 0.5f,
                Prediction = 0.5f,
                Smoothing = 0.05f,
                JitterRadius = 0.8f,
                MaxDeviationRadius = 0.2f
            };

            sensor.SkeletonStream.Enable(parameters);
            sensor.DepthStream.Enable();
            sensor.SkeletonFrameReady += this.Sensor_SkeletonFrameReady;

        }

        #endregion      


        #region Mouse Control

        /**
         * Helper function for determining the appropriate mouse position
         * Author: Ryan Noonan 
         **/
        private float ScaleVector(int length, float position)
        {
            float value = (((((float)length) / 1f) / 2f) * position) + (length / 2);
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }

        /**
         * Convert the 'right hand joint' position into an appropriate (x,y) position
         * on the screen
         * Author: Ross Kahn (heavily taken from Internet)
         **/
        private void controlMouse(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return;
            }

            Joint handright = skeleton.Joints[JointType.HandRight];

            Microsoft.Kinect.SkeletonPoint vector = new Microsoft.Kinect.SkeletonPoint();
            vector.X = ScaleVector(SystemInformation.PrimaryMonitorSize.Width, handright.Position.X);
            vector.Y = ScaleVector(SystemInformation.PrimaryMonitorSize.Height, -handright.Position.Y);

            handright.Position = vector;

            int topofscreen;
            int leftofscreen;

            leftofscreen = Convert.ToInt32(handright.Position.X);
            topofscreen = Convert.ToInt32(handright.Position.Y);

            // Uses an external library to move the mouse
            KinectMouseController.KinectMouseMethods.SendMouseInput(leftofscreen, topofscreen,
                SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height,
                false);
        }
        #endregion

        #region Skeletal Tracking

        /**
         * Finds the first skeleton data available and extracts the information
         * Author: Ryan Noonan
         **/
        private Skeleton GetFirstSkeleton(SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }

                Skeleton[] allSkeletons = new Skeleton[6];
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get first skeleton tracked
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();
                return first;
            }
        }

        /**
         * When a skeleton frame is ready, extract the skeleton information. Cycle
         * through the list of known gestures, and when a successful gesture is found,
         * do the ACTION that is associated with it (in gestureBindings)
         **/
        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Extract skeleton information
            Skeleton skeleton = GetFirstSkeleton(e);

            // Update the mouse position
            controlMouse(skeleton);

            foreach (BaseGesture gesture in gestureBindings.Keys)
            {
                // Find the first motion that is successfuly recgonized
                if (gesture.CheckGesture(skeleton) == GestureResult.Success)
                {

                    // Don't repeat actions
                    if (gestureBindings[gesture] == currentAction) { continue; }

                    // Special case. Only release click if clicking is the currently active action
                    if (gestureBindings[gesture] == Constants.ACTION.RELEASE_CLICK)
                    {
                        if (!(currentAction == Constants.ACTION.CLICK)) { continue; }
                    }

                    // Do the action associated with this gesture
                    doAction(gestureBindings[gesture]);

                    // Update the 'current' action
                    currentAction = gestureBindings[gesture];
                    break;
                }
            }
        }

        /**
         * Event handler for when the Kinect's status is changed (e.g. disconnected)
         **/
        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine("Sensor Status Changed: " + e.Status);
        }

        #endregion

        #region Action Support

        private void doAction(Constants.ACTION newAction)
        {
            Console.WriteLine("Action: " + newAction);
            switch (newAction)
            {
                case Constants.ACTION.CLICK:    // Stop zooming (if applicable) and start holding down a click
                    releaseActions();
                    clicking = true;
                    KeyboardMouseManager.simulateMouseAction(PRESS);
                    break;

                case Constants.ACTION.NEUTRAL:  // Stop any currently active action
                    releaseActions();
                    break;

                case Constants.ACTION.RELEASE_CLICK:    // Stop the clicking action, if it's active
                    releaseActions();
                    break;

                case Constants.ACTION.ZOOMIN:   // Stop any other action and start zooming in
                    releaseActions();
                    simulateKeyAction(Constants.ZOOMIN_KEY);
                    break;

                case Constants.ACTION.ZOOMOUT:  // Stop any other action and start zooming out
                    releaseActions();
                    simulateKeyAction(Constants.ZOOMOUT_KEY);
                    break;

                default:
                    Console.WriteLine("No behavior specified for " + newAction);
                    break;
            }
           Console.WriteLine();
        }

        /**
         * Stops all actions. This is idempotent, so there is no effect if 
         * an inactive action is 'released'. 
         **/
        private void releaseActions()
        {
            // Only release a click if it's been clicking. Prevents the context menu from popping up
            if (clicking)
            {
                clicking = false;
                simulateMouseAction(RELEASE);                       // Simulate a mouse up
            }

            // Pauses all zooming
            Command pauseCommand = new ZoomCommand(Constants.ZOOM_ACTION.PAUSE);
            main.doCommand(pauseCommand);
        }

        // Simulates the right mouse button being held down (if mouseDown == true)
        // Simulates the right mouse button being released (if mouseDown == false)
        private void simulateMouseAction(bool mouseDown)
        {
            KeyboardMouseManager.simulateMouseAction(mouseDown);
        }

        /**
         * Translates a gesture into a key press. 
         **/
        private void simulateKeyAction(Keys toSimulate)
        {

            // Zoom in if the "to simulate" key was a zoom in
            Command zoomCommand;
            if (toSimulate == Constants.ZOOMIN_KEY)
            {
                zoomCommand = new ZoomCommand(Constants.ZOOM_ACTION.ZOOMIN);
                Console.WriteLine("(KinectManager): Zoom In");
            }

            // Zoom out if the "to simulate" key was a zoom out
            else if (toSimulate == Constants.ZOOMOUT_KEY)
            {
                zoomCommand = new ZoomCommand(Constants.ZOOM_ACTION.ZOOMOUT);
                Console.WriteLine("(KinectManager): Zoom Out");
            }
            else
            {
                Console.WriteLine("(KinectManager) simulateKeyAction called with unspecified key: " + toSimulate);
                return;
            }

            // Execute command
            main.doCommand(zoomCommand);
            
        }

        #endregion

    }
}
