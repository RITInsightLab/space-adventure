using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    class SwipeLeftSegment2 : IRelativeGestureSegment
    {
        private int framecount = 0;

        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GesturePartResult.Fail;
            }

            // right hand is raised but not swiping yet
            if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
            {                

                // right hand is to the right of the elbow
                if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    framecount = 0;
                    return GesturePartResult.Succeed;
                }

                return GesturePartResult.Pausing;
                
            }

            // hand dropped
            return GesturePartResult.Fail;
        }
    }
}
