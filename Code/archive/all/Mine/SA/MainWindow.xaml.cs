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
    using System.Threading;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using Microsoft.Samples.Kinect.SpeechBasics.Event_Arguments;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Resource key for medium-gray-colored brush.
        /// </summary>
        private const string MediumGreyBrushKey = "MediumGreyBrush";

        /// <summary>
        /// Map between each direction and the direction immediately to its right.
        /// </summary>
        private static readonly Dictionary<Direction, Direction> TurnRight = new Dictionary<Direction, Direction>
            {
                { Direction.Up, Direction.Right },
                { Direction.Right, Direction.Down },
                { Direction.Down, Direction.Left },
                { Direction.Left, Direction.Up }
            };

        /// <summary>
        /// Map between each direction and the direction immediately to its left.
        /// </summary>
        private static readonly Dictionary<Direction, Direction> TurnLeft = new Dictionary<Direction, Direction>
            {
                { Direction.Up, Direction.Left },
                { Direction.Right, Direction.Up },
                { Direction.Down, Direction.Right },
                { Direction.Left, Direction.Down }
            };

        /// <summary>
        /// Map between each direction and the displacement unit it represents.
        /// </summary>
        private static readonly Dictionary<Direction, Point> Displacements = new Dictionary<Direction, Point>
            {
                { Direction.Up, new Point { X = 0, Y = -1 } },
                { Direction.Right, new Point { X = 1, Y = 0 } },
                { Direction.Down, new Point { X = 0, Y = 1 } },
                { Direction.Left, new Point { X = -1, Y = 0 } }
            };

        /// <summary>
        /// Active Kinect sensor.
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        /// <summary>
        /// Current direction where turtle is facing.
        /// </summary>
        private Direction curDirection = Direction.Up;

        /// <summary>
        /// List of all UI span elements used to select recognized text.
        /// </summary>
        private List<Span> recognitionSpans;

        //START SPACE ADVENTURES SHIT

        //model created for gestures
        SkeletonViewModel viewModel;
        

        // Travel Mode variable
        private bool travelMode = false;

        // HandUp Gesture
        private LeftHandUp leftHandUp;
        private RightHandUp rightHandUp;

        //Swipe Gestures
        private SwipeLeftSegment1 swipeLeftSeg1;
        private SwipeLeftSegment2 swipeLeftSeg2;
        private SwipeRightSegment1 swipeRightSeg1;
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
        private GesturePartResult swipeLeftSegment1Result = GesturePartResult.Fail;
        private GesturePartResult swipeLeftSegment2Result = GesturePartResult.Fail;
        private GesturePartResult swipeRightSegment1Result = GesturePartResult.Fail;
        private GesturePartResult swipeRightSegment2Result = GesturePartResult.Fail;

        private GesturePartResult swipeDownResult = GesturePartResult.Fail;

        // Forward Gestures
        private GesturePartResult leftHandForwardGestureResult = GesturePartResult.Fail;
        private GesturePartResult rightHandForwardGestureResult = GesturePartResult.Fail;

        //all Skeletons array
        private Skeleton[] allSkeletons = new Skeleton[6];

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            
        }

        /// <summary>
        /// Enumeration of directions in which turtle may be facing.
        /// </summary>
        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
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
                    swipeLeftSeg1 = new SwipeLeftSegment1();
                    swipeLeftSeg2 = new SwipeLeftSegment2();
                    swipeRightSeg1 = new SwipeRightSegment1();
                    swipeRightSeg2 = new SwipeRightSegment2();
                    leftHandForward = new LeftHandForward();
                    rightHandForward = new RightHandForward();
                    swipeDown = new SwipeDown();
                    viewModel = new SkeletonViewModel();
                    viewModel.GestureRecognized += new EventHandler<GestureEventArgs>(this.GestureRecognized);
                    sensor.SkeletonStream.Enable();
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
                recognitionSpans = new List<Span> { mercurySpan, venusSpan, earthSpan, marsSpan, jupiterSpan, saturnSpan, uranusSpan, neptuneSpan, plutoSpan, sunSpan };

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                /****************************************************************
                * 
                * Use this code to create grammar programmatically rather than from
                * a grammar file.
                * 
                * var directions = new Choices();
                * directions.Add(new SemanticResultValue("forward", "FORWARD"));
                * directions.Add(new SemanticResultValue("forwards", "FORWARD"));
                * directions.Add(new SemanticResultValue("straight", "FORWARD"));
                * directions.Add(new SemanticResultValue("backward", "BACKWARD"));
                * directions.Add(new SemanticResultValue("backwards", "BACKWARD"));
                * directions.Add(new SemanticResultValue("back", "BACKWARD"));
                * directions.Add(new SemanticResultValue("turn left", "LEFT"));
                * directions.Add(new SemanticResultValue("turn right", "RIGHT"));
                *
                * var gb = new GrammarBuilder { Culture = ri.Culture };
                * gb.Append(directions);
                *
                * var g = new Grammar(gb);
                * 
                ****************************************************************/

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
        /// Remove any highlighting from recognition instructions.
        /// </summary>
        private void ClearRecognitionHighlights()
        {
            foreach (Span span in recognitionSpans)
            {
                span.Foreground = (Brush)this.Resources[MediumGreyBrushKey];
                span.FontWeight = FontWeights.Normal;
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

            // Number of degrees in a right angle.
            const int DegreesInRightAngle = 90;

            // Number of pixels turtle should move forwards or backwards each time.
            const int DisplacementAmount = 60;

            
            ClearRecognitionHighlights();

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "TRAVELMODE":
                        travelMode = true;
                        travelSpan.Foreground = Brushes.DarkGreen;
                        travelSpan.FontWeight = FontWeights.Bold;
                        break;

                    case "MERCURY":
                        if (travelMode)
                        {
                            mercurySpan.Foreground = Brushes.DeepSkyBlue;
                            mercurySpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "VENUS":
                        if (travelMode)
                        {
                            venusSpan.Foreground = Brushes.DeepSkyBlue;
                            venusSpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "EARTH":
                        if (travelMode)
                        {
                            earthSpan.Foreground = Brushes.DeepSkyBlue;
                            earthSpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "MARS":
                        if (travelMode)
                        {
                            marsSpan.Foreground = Brushes.DeepSkyBlue;
                            marsSpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "JUPITER":
                        if (travelMode)
                        {
                            jupiterSpan.Foreground = Brushes.DeepSkyBlue;
                            jupiterSpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "SATURN":
                        if (travelMode)
                        {
                            saturnSpan.Foreground = Brushes.DeepSkyBlue;
                            saturnSpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "URANUS":
                        if (travelMode)
                        {
                            uranusSpan.Foreground = Brushes.DeepSkyBlue;
                            uranusSpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "NEPTUNE":
                        if (travelMode)
                        {
                            neptuneSpan.Foreground = Brushes.DeepSkyBlue;
                            neptuneSpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "PLUTO":
                        if (travelMode)
                        {
                            plutoSpan.Foreground = Brushes.DeepSkyBlue;
                            plutoSpan.FontWeight = FontWeights.Bold;
                            travelMode = false;
                            break;
                        }
                        else
                            break;

                    case "SUN":
                        if (travelMode)
                        {
                            sunSpan.Foreground = Brushes.DeepSkyBlue;
                            sunSpan.FontWeight = FontWeights.Bold;
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
                    travelSpan.Foreground = Brushes.Red;
                    travelSpan.FontWeight = FontWeights.Bold;
                }
            }
        }
        /*****************************************************************************************
         * 
         * 
         * 
         * This is Where I start next time
         * 
         * 
         * 
         *****************************************************************************************/





        private void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            Skeleton first = GetFirstSkeleton(e);
            if (first != null)
            {
                Console.WriteLine("Hand" + first.Joints[JointType.HandRight].Position.Y);
                Console.WriteLine("Shoulder" + first.Joints[JointType.ShoulderRight].Position.Y);
                viewModel.SkeletonData = first;
                Thread.Sleep(50);
            }

            /*
            leftHandUpGestureResult = viewModel.SkeletonData = first;
            rightHandUpGestureResult = rightHandUp.CheckGesture(first);
            swipeLeftSegment1Result = swipeLeftSeg1.CheckGesture(first, swipeLeftSegment1Result);
            swipeLeftSegment2Result = swipeLeftSeg2.CheckGesture(first, swipeLeftSegment2Result);
            swipeRightSegment1Result = swipeRightSeg1.CheckGesture(first);
            swipeRightSegment2Result = swipeRightSeg2.CheckGesture(first);
            rightHandForwardGestureResult = rightHandForward.CheckGesture(first);
            leftHandForwardGestureResult = leftHandForward.CheckGesture(first);
            swipeDownResult = swipeDown.CheckGesture(first, swipeDownResult);
             
             */
            // Check if left hand is up
            if (leftHandUpGestureResult == GesturePartResult.Succeed)
            {
                leftHandUpSpan.Foreground = Brushes.Green;
                leftHandUpSpan.FontWeight = FontWeights.Bold;
            }
            else
            {
                leftHandUpSpan.Foreground = Brushes.Red;
                leftHandUpSpan.FontWeight = FontWeights.Bold;
            }
            // Check if right hand is up
            if (rightHandUpGestureResult == GesturePartResult.Succeed)
            {
                rightHandUpSpan.Foreground = Brushes.Green;
                rightHandUpSpan.FontWeight = FontWeights.Bold;
            }
            else
            {
                rightHandUpSpan.Foreground = Brushes.Red;
                rightHandUpSpan.FontWeight = FontWeights.Bold;
            }

            // Check if both hands are up
            if (leftHandUpGestureResult == GesturePartResult.Succeed && rightHandUpGestureResult == GesturePartResult.Succeed)
            {
                bothHandUpSpan.Foreground = Brushes.Green;
                bothHandUpSpan.FontWeight = FontWeights.Bold;
            }
            else
            {
                bothHandUpSpan.Foreground = Brushes.Red;
                bothHandUpSpan.FontWeight = FontWeights.Bold;
            }

            // Swipe Left Segment
            if (swipeLeftSegment2Result == GesturePartResult.Pausing)
            {
                swipedLeftSpan.Foreground = Brushes.Green;
                swipedLeftSpan.FontWeight = FontWeights.Bold;
            }
            else
            {
                swipedLeftSpan.Foreground = Brushes.Red;
                swipedLeftSpan.FontWeight = FontWeights.Bold;
            }

            // Swipe Right Segment
            if (swipeLeftSegment2Result == GesturePartResult.Pausing)
            {
                swipedRightSpan.Foreground = Brushes.Green;
                swipedRightSpan.FontWeight = FontWeights.Bold;
            }
            else
            {
                swipedRightSpan.Foreground = Brushes.Red;
                swipedRightSpan.FontWeight = FontWeights.Bold;
            }

            // Left hand up and swipe
            if (swipeLeftSegment2Result == GesturePartResult.Succeed)
            {
                swipedLeftSpan.Foreground = Brushes.Black;
                swipedLeftSpan.FontWeight = FontWeights.Bold;
                leftHandUpSpan.Foreground = Brushes.Black;
                leftHandUpSpan.FontWeight = FontWeights.Bold;
            }

            // Right hand up and swipe
            if (swipeRightSegment2Result == GesturePartResult.Succeed)
            {
                swipedRightSpan.Foreground = Brushes.Black;
                swipedRightSpan.FontWeight = FontWeights.Bold;
                rightHandUpSpan.Foreground = Brushes.Black;
                rightHandUpSpan.FontWeight = FontWeights.Bold;
            }

            //Swipe down segment
            if (swipeDownResult == GesturePartResult.Succeed)
            {
                swipedRightSpan.Foreground = Brushes.Green;
                swipedRightSpan.FontWeight = FontWeights.Bold;
            }
            else
            {
                swipedRightSpan.Foreground = Brushes.Red;
                swipedRightSpan.FontWeight = FontWeights.Bold;
            }

            // Forward gestures
            if (rightHandForwardGestureResult == GesturePartResult.Succeed)
            {
                rightHandForwardSpan.Foreground = Brushes.Green;
                rightHandForwardSpan.FontWeight = FontWeights.Bold;
            }
            else
            {
                rightHandForwardSpan.Foreground = Brushes.Red;
                rightHandForwardSpan.FontWeight = FontWeights.Bold;
            }
            if (leftHandForwardGestureResult == GesturePartResult.Succeed)
            {
                leftHandForwardSpan.Foreground = Brushes.Green;
                leftHandForwardSpan.FontWeight = FontWeights.Bold;
            }
            else
            {
                leftHandForwardSpan.Foreground = Brushes.Red;
                leftHandForwardSpan.FontWeight = FontWeights.Bold;
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
            ClearRecognitionHighlights();
        }

        
        private void GestureRecognized(object sender, GestureEventArgs e)
        {
            if (e.GestureType == GestureType.RightUp)
            {
                
                rightHandUpGestureResult = GesturePartResult.Succeed;
                //Mouse down
            }
            else if (e.GestureType == GestureType.LeftUp)
            {
                
                leftHandUpGestureResult = GesturePartResult.Succeed;
                //Mouse down
            }
            else if (e.GestureType == GestureType.RightSwipe)
            {
                
                swipeRightSegment2Result = GesturePartResult.Succeed;
                //press correct key spin CCW
            }
            else if (e.GestureType == GestureType.LeftSwipe)
            {
                
                swipeLeftSegment2Result = GesturePartResult.Succeed;
                //press correct key spin clockwise
            }

            else if (e.GestureType == GestureType.SwipeDown)
            {
                
                swipeDownResult = GesturePartResult.Succeed;
                //whatever goes here
            }

        }
    }
}