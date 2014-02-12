using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace Space_Adventure
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
