using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    class SwipeDown : IRelativeGestureSegment
    {

        private int framecount = 0;

        // start gesture boolean
        private bool startGestureCheck = false;

        public GesturePartResult CheckGesture(Skeleton skeleton, GesturePartResult previousResult)
        {
            if (skeleton == null)
            {
                return GesturePartResult.Fail;
            }

            // should we check for the gesutre yet??
            if (startGestureCheck)
            {
                //yes
                // right hand is below right elbow
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y)
                {
                    framecount = 0;
                    startGestureCheck = false;
                    return GesturePartResult.Succeed;
                }

                // hand is still above head
                return GesturePartResult.Fail;
            }
            else
            {
                //no
                // right hand is above right elbow
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderRight].Position.Y)
                {
                    startGestureCheck = true;                    
                }
                return GesturePartResult.Fail;
            }   
                
        }

        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            return GesturePartResult.Fail;
        }
    }
}
