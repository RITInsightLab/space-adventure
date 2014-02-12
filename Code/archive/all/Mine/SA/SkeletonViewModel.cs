using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SpeechBasics.Event_Arguments;

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    class SkeletonViewModel
    {
        //checks gestures
        private GestureController gestures = new GestureController();

        //if a gesture is detected, true
        private bool gestureDetected;

        public event EventHandler<GestureEventArgs> GestureRecognized;

        //holds the data
        private Skeleton skeletonData;

        public SkeletonViewModel()
        {
            this.DefineGestures();
            this.gestures.GestureRecognized += new EventHandler<GestureEventArgs>(this.Gestures_GestureRecognized);
        }

        public bool GestureDetected
        {
            get
            {
                return gestureDetected;
            }
            set
            {
                if (this.gestureDetected != value)
                {
                    this.gestureDetected = value;
                    gestures.UpdateAllGestures(skeletonData);
                }
            }
        }

        public Skeleton SkeletonData
        {
            get
            {
                return this.skeletonData;
            }
            set
            {
                if (this.skeletonData != value)
                {
                    this.skeletonData = value;
                    this.gestures.UpdateAllGestures(this.skeletonData);
                }
            }
        }

        private void Gestures_GestureRecognized(object sender, GestureEventArgs e)
        {
            /*if(e.GestureType == GestureType.RightUp)
            {
                this.GestureRecognized(this, e);
                //Mouse down
            }
            else if(e.GestureType == GestureType.LeftUp)
            {
                this.GestureRecognized(this, e);
                //Mouse down
            }
            else if (e.GestureType == GestureType.RightSwipe)
            {
                this.GestureRecognized(this, e);
                //press correct key spin CCW
            }
            else if (e.GestureType == GestureType.LeftSwipe)
            {
                this.GestureRecognized(this, e);
                //press correct key spin clockwise
            }
            
            else if (e.GestureType == GestureType.SwipeDown)
            {
                this.GestureRecognized(this, e);
                //whatever goes here
            }*/

            if (this.GestureRecognized != null)
            {
                this.GestureRecognized(this, e);
            }
        }

        private void DefineGestures()
        {
            //swipe right
            IRelativeGestureSegment[] swipeRightSegments = new IRelativeGestureSegment[4];
            SwipeRightSegment1 swipeRightSegment1 = new SwipeRightSegment1();
            SwipeRightSegment2 swipeRightSegment2 = new SwipeRightSegment2();
            RightHandUp rightHandUp = new RightHandUp();
            swipeRightSegments[0] = rightHandUp;
            swipeRightSegments[1] = swipeRightSegment1;
            swipeRightSegments[2] = rightHandUp;
            swipeRightSegments[3] = swipeRightSegment2;
            this.gestures.AddGesture(GestureType.RightSwipe, swipeRightSegments);

            //swipe left
            IRelativeGestureSegment[] swipeLeftSegments = new IRelativeGestureSegment[4];
            SwipeLeftSegment1 swipeLeftSegment1 = new SwipeLeftSegment1();
            SwipeLeftSegment2 swipeLeftSegment2 = new SwipeLeftSegment2();
            LeftHandUp leftHandUp = new LeftHandUp();
            swipeLeftSegments[0] = leftHandUp;
            swipeLeftSegments[1] = swipeLeftSegment1;
            swipeLeftSegments[2] = leftHandUp;
            swipeLeftSegments[3] = swipeLeftSegment2;
            this.gestures.AddGesture(GestureType.LeftSwipe, swipeLeftSegments);

            //swipe down
            IRelativeGestureSegment[] swipeDownSegments = new IRelativeGestureSegment[1];
            SwipeDown swipeDown = new SwipeDown();
            swipeDownSegments[0] = swipeDown;
            this.gestures.AddGesture(GestureType.SwipeDown, swipeDownSegments);

            //Right up
            IRelativeGestureSegment[] rightUpSegments = new IRelativeGestureSegment[1];
            RightHandUp rightUp = new RightHandUp();
            rightUpSegments[0] = rightUp;
            this.gestures.AddGesture(GestureType.RightUp, rightUpSegments);


        }



    }
}
