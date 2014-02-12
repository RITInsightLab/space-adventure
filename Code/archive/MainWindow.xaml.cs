//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Windows.Forms;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Threading;
    using KinectMouseController;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class MainWindow : Window
    {

        [DllImport("user32")]

        public static extern int SetCursorPos(int x, int y);

        private const int MOUSEEVENTF_MOVE = 0x0001;

        [DllImport("user32.dll",
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]

        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
     
        /// <summary>
        /// Active Kinect sensor.
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        // Travel Mode variable
        private bool travelMode = false;

        // HandUp Gesture
        private LeftHandUp leftHandUp;
        private RightHandUp rightHandUp;

        //Swipe Gestures
        private SwipeLeftSegment2 swipeLeftSeg2;
        private SwipeRightSegment2 swipeRightSeg2;

        //Forward Gestures
        private LeftHandForward leftHandForward;
        private RightHandForward rightHandForward;

        //Swipe Down Gesture
        private SwipeDown swipeDown;

        
        
        ///previousLeftForwardGestureResult 
        // HandUp GesturePartResult
        private GesturePartResult leftHandUpGestureResult = GesturePartResult.Fail;
        private GesturePartResult previousLeftHandUpGestureResult = GesturePartResult.Fail;
        private GesturePartResult rightHandUpGestureResult = GesturePartResult.Fail;

        // Swiping gestures
        private GesturePartResult swipeLeftSegment2Result = GesturePartResult.Fail;
        private GesturePartResult swipeRightSegment2Result = GesturePartResult.Fail;

        private GesturePartResult swipeDownResult = GesturePartResult.Fail;

        // Forward Gestures
        private GesturePartResult leftHandForwardGestureResult = GesturePartResult.Fail;
        private GesturePartResult previousLeftForwardGestureResult = GesturePartResult.Fail;
        private GesturePartResult rightHandForwardGestureResult = GesturePartResult.Fail;

        //all Skeletons array
        private Skeleton[] allSkeletons = new Skeleton[6];

        private Process myProcess;

        // Writer for log file
        StreamWriter writer;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            writer = new StreamWriter("log.txt");
            InitializeComponent();
            
        }
        
        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            
            return null;
        }


        /// <summary>
        /// Execute initialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            writer.WriteLine("--------------------------------");
            writer.WriteLine(DateTime.Now.ToString());
            
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            writer.WriteLine("Finding potential sensors...");
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                writer.WriteLine("\tSensor Found: " + potentialSensor.ToString());
                writer.WriteLine("\tSensor Status: " + potentialSensor.Status);

                if ((potentialSensor.Status).Equals(KinectStatus.Connected))
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                try
                {
                    writer.WriteLine("Starting sensor...");
                    // Start the sensor!
                    this.sensor.Start();

                    if (this.sensor.IsRunning)
                    {
                        writer.WriteLine("Sensor running successfully");
                    }
                    else
                    {
                        writer.WriteLine("WARNING: Sensor not running correctly!");
                    }

                    writer.WriteLine("Loading Hand Actions...");
                    leftHandUp = new LeftHandUp();
                    rightHandUp = new RightHandUp();
                    swipeLeftSeg2 = new SwipeLeftSegment2();
                    swipeRightSeg2 = new SwipeRightSegment2();
                    leftHandForward = new LeftHandForward();
                    rightHandForward = new RightHandForward();
                    writer.WriteLine("Hand action load complete");

                    swipeDown = new SwipeDown();
                    sensor.SkeletonStream.Enable();

                    if (sensor.SkeletonStream.IsEnabled)
                    {
                        writer.WriteLine("Sensor Skeleton Stream enabled");
                    }
                    else
                    {
                        writer.WriteLine("WARNING: Sensor Skeleton Stream not enabled correctly");
                    }



                    writer.WriteLine("Starting Celestia script at " + DateTime.Now.ToString() + "...");
                    //myProcess = Process.Start(@"C:\Users\Nocturnal\Dropbox\FrontiersII\CelX Script.celx");
                    //myProcess = Process.Start(@"C:\Users\Noonan11\Dropbox\FrontiersII\CelX Script.celx");
                    myProcess = Process.Start(@"C:\Users\Jake Noel-Storr\Dropbox\FrontiersII\CelX Script.celx");
                    writer.WriteLine("Celestia Process started");

                }
                catch (IOException exception)
                {
                    writer.WriteLine("ERROR: " + exception.ToString());
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                }
                finally
                {
                    writer.Flush();
                }
            }
            else   // Sensor is null
            {
                writer.WriteLine("ERROR line 200 MainWindow.xaml.cs::: Sensor is null");
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
                writer.Flush();
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);
                
                // Create a grammar from grammar definition XML file.
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    writer.WriteLine("\nCreating speech grammar...");
                    writer.Flush();
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);  // SYSTEM STALLS ON THIS LINE -ROSS
                    writer.WriteLine("Speech grammar created");
                    writer.Flush();
                }
                

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                writer.WriteLine("Speech recognizer created");
                writer.Flush();
            }
            else
            {
                writer.WriteLine("ERROR: Speech recognizer not enabled");
                this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }
            writer.Flush();
            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
           
        }

        public class Form1 : Form
        {
            [DllImport("user32.dll")]
            static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

            [Flags]
            public enum MouseEventFlags
            {
                LEFTDOWN = 0x00000002,
                LEFTUP = 0x00000004,
                MIDDLEDOWN = 0x00000020,
                MIDDLEUP = 0x00000040,
                MOVE = 0x00000001,
                ABSOLUTE = 0x00008000,
                RIGHTDOWN = 0x00000008,
                RIGHTUP = 0x00000010
            }

            public static void leftClick()
            {
                //int X = Cursor.Position.X;
                //int Y = Cursor.Position.Y;

                mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
                //mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            }
                public static void leftClickUp()
            {
                //int X = Cursor.Position.X;
                //int Y = Cursor.Position.Y;

                //mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
                mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            }

            public static void rightClick()
            {
                mouse_event((int)(MouseEventFlags.RIGHTDOWN), 0, 0, 0, 0);
            }

            public static void rightMouseUp()
            {
                mouse_event((int)(MouseEventFlags.RIGHTUP), 0, 0, 0, 0);
            }


            //...other code needed for the application
        }


        /// <summary>
        /// Execute uninitialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();

                this.sensor.Stop();
                this.sensor = null;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.0;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "TRAVELMODE":
                        travelMode = true;
                        SendKeys.SendWait("t");
                        break;

                    case "MERCURY":
                        if (travelMode)
                        {                          
                                SendKeys.SendWait("1");
                                travelMode = false;
                                break;                     
                        }
                       
                            break;

                    case "VENUS":
                        if (travelMode)
                        {
                            SendKeys.SendWait("2");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "EARTH":
                        if (travelMode)
                        {
                            SendKeys.SendWait("3");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "MARS":
                        if (travelMode)
                        {
                            SendKeys.SendWait("4");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "JUPITER":
                        if (travelMode)
                        {
                            SendKeys.SendWait("5");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "SATURN":
                        if (travelMode)
                        {
                            SendKeys.SendWait("6");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "URANUS":
                        if (travelMode)
                        {
                            SendKeys.SendWait("7");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "NEPTUNE":
                        if (travelMode)
                        {
                            SendKeys.SendWait("8");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "PLUTO":
                        if (travelMode)
                        {
                            SendKeys.SendWait("9");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "SUN":
                        if (travelMode)
                        {
                            SendKeys.SendWait("0");
                            travelMode = false;
                            break;
                        }
                        else
                            break;
                    case "MOON":
                        if (travelMode)
                        {
                            SendKeys.SendWait("l");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "TRANSIT":
                        if (travelMode)
                        {
                            SendKeys.SendWait("q");
                            travelMode = false;
                            break;
                        }
                        else
                            break;
                    case "CANCEL":
                        SendKeys.SendWait("c");
                        travelMode = false;
                        break;
                }

                if (!travelMode)
                {
                }
            }
        }

        private void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
            {
                return;
            }
            else
            {
                Console.WriteLine("Skeleton retrieval success");
            }

            previousLeftHandUpGestureResult = leftHandUpGestureResult;
            leftHandUpGestureResult = leftHandUp.CheckGesture(first);
            
            rightHandUpGestureResult = rightHandUp.CheckGesture(first);
            swipeLeftSegment2Result = swipeLeftSeg2.CheckGesture(first, swipeLeftSegment2Result);
            swipeRightSegment2Result = swipeRightSeg2.CheckGesture(first);
            rightHandForwardGestureResult = rightHandForward.CheckGesture(first);
            leftHandForwardGestureResult = leftHandForward.CheckGesture(first);
            swipeDownResult = swipeDown.CheckGesture(first, swipeDownResult);

            ControlMouse(first);

            // Check if left hand is up
            if (leftHandUpGestureResult == GesturePartResult.Succeed)
            {
                if (previousLeftHandUpGestureResult == GesturePartResult.Fail)
                {
                    Form1.rightClick();
                }
            }
            else
            {
                if (previousLeftHandUpGestureResult == GesturePartResult.Succeed)
                {
                    Form1.rightMouseUp();
                }
            }
            // Check if right hand is up
            if (rightHandUpGestureResult == GesturePartResult.Succeed)
            {
                
            }
            else
            {
            }

            // Check if both hands are up
            if (leftHandUpGestureResult == GesturePartResult.Succeed && rightHandUpGestureResult == GesturePartResult.Succeed)
            {
            }
            else
            {
            }

            // Swipe Left Segment
            if (swipeLeftSegment2Result == GesturePartResult.Pausing)
            {
            }
            else
            {
            }

            // Left hand up and swipe
            if ((swipeLeftSegment2Result == GesturePartResult.Pausing) && (leftHandUpGestureResult == GesturePartResult.Succeed))
            {
            }

            //Swipe down segment
            if (swipeDownResult == GesturePartResult.Succeed)
            {
            }
            else
            {
            }

            // Forward gestures
            if (rightHandForwardGestureResult == GesturePartResult.Succeed)
            {
            }
            else
            {
            }
            if (leftHandForwardGestureResult == GesturePartResult.Succeed)
            {
                
                if (previousLeftForwardGestureResult == GesturePartResult.Fail)
                {
                    //Form1.leftClick();
                }
            }
            else
            {
                if (previousLeftHandUpGestureResult == GesturePartResult.Succeed)
                {
                    //Form1.leftClickUp();
                }

            }
            
        }

        private Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get first skeleton tracked
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();
                return first;
            }
        }

        private void ControlMouse(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                Console.WriteLine("NO FUCKING SKELETON, SHIT >_<");
                return;
            }

            Joint handright = skeleton.Joints[JointType.HandRight];

            Microsoft.Kinect.SkeletonPoint vector = new Microsoft.Kinect.SkeletonPoint();
            vector.X = ScaleVector(3840, handright.Position.X);
            vector.Y = ScaleVector(1365, -handright.Position.Y);

            handright.Position = vector;

            int topofscreen;
            int leftofscreen;

            leftofscreen = Convert.ToInt32(handright.Position.X);
            topofscreen = Convert.ToInt32(handright.Position.Y);

            SetCursorPos(leftofscreen, topofscreen);
        }

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
  
        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech rejected: " + e.Result.ToString());  
        }
    }
}