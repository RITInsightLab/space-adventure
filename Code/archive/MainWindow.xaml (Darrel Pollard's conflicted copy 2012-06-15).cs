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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class MainWindow : Window
    {
        
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

        // HandUp GesturePartResult
        private GesturePartResult leftHandUpGestureResult = GesturePartResult.Fail;
        private GesturePartResult rightHandUpGestureResult = GesturePartResult.Fail;

        // Swiping gestures
        private GesturePartResult swipeLeftSegment2Result = GesturePartResult.Fail;
        private GesturePartResult swipeRightSegment2Result = GesturePartResult.Fail;

        private GesturePartResult swipeDownResult = GesturePartResult.Fail;

        // Forward Gestures
        private GesturePartResult leftHandForwardGestureResult = GesturePartResult.Fail;
        private GesturePartResult rightHandForwardGestureResult = GesturePartResult.Fail;

        //all Skeletons array
        private Skeleton[] allSkeletons = new Skeleton[6];

        private Process myProcess;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
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
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                try
                {
                    // Start the sensor!
                    this.sensor.Start();
                    leftHandUp = new LeftHandUp();
                    rightHandUp = new RightHandUp();
                    swipeLeftSeg2 = new SwipeLeftSegment2();
                    swipeRightSeg2 = new SwipeRightSegment2();
                    leftHandForward = new LeftHandForward();
                    rightHandForward = new RightHandForward();
                    swipeDown = new SwipeDown();
                    sensor.SkeletonStream.Enable();
                    myProcess = Process.Start(@"C:\Users\Nocturnal\Dropbox\FrontiersII\CelX Script.celx");
                


                    
                     
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                }
            }
             
            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                // Create a grammar from grammar definition XML file.
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }            

            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);

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
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "TRAVELMODE":
                        travelMode = true;
                        break;

                    case "MERCURY":
                        if (travelMode)
                        {                          
                                SendKeys.SendWait("1");
                                travelMode = false;
                                break;                     
                        }
                        else
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
                            //SendKeys.Send("3");
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "MARS":
                        if (travelMode)
                        {
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "JUPITER":
                        if (travelMode)
                        {
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "SATURN":
                        if (travelMode)
                        {
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "URANUS":
                        if (travelMode)
                        {
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "NEPTUNE":
                        if (travelMode)
                        {
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "PLUTO":
                        if (travelMode)
                        {
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "SUN":
                        if (travelMode)
                        {
                            travelMode = false;
                            break;
                        }
                        else
                            break;
                    case "CANCEL":
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

            leftHandUpGestureResult = leftHandUp.CheckGesture(first);
            rightHandUpGestureResult = rightHandUp.CheckGesture(first);
            swipeLeftSegment2Result = swipeLeftSeg2.CheckGesture(first, swipeLeftSegment2Result);
            swipeRightSegment2Result = swipeRightSeg2.CheckGesture(first);
            rightHandForwardGestureResult = rightHandForward.CheckGesture(first);
            leftHandForwardGestureResult = leftHandForward.CheckGesture(first);
            swipeDownResult = swipeDown.CheckGesture(first, swipeDownResult);

            // Check if left hand is up
            if (leftHandUpGestureResult == GesturePartResult.Succeed)
            {
            }
            else
            {
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
            }
            else
            {
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

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
        }
    }
}