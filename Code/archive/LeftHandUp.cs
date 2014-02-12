using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;


namespace Microsoft.Samples.Kinect.SpeechBasics
{
    class LeftHandUp : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GesturePartResult.Fail;
            }
            // left hand is above elbow
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.Head].Position.Y)
            {
                return GesturePartResult.Succeed;
            }

            // hand dropped
            return GesturePartResult.Fail;
        }
    }
}
