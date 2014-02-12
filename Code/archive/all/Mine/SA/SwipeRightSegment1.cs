using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    class SwipeRightSegment1: IRelativeGestureSegment
    {

        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GesturePartResult.Fail;
            }

            //Hand up, but not in correct position
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {                             
                //Left hand outside left elbow
                if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X)
                {
                    return GesturePartResult.Succeed;
                }

                return GesturePartResult.Pausing;
            }
            
            return GesturePartResult.Fail;
            
        }

    }
}
