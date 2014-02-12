using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    class RightHandUp : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GesturePartResult.Fail;
            }
            // right hand is above elbow
            if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.Head].Position.Y)
            {
                return GesturePartResult.Succeed;
            }

            // hand dropped
            return GesturePartResult.Fail;
        }
    }
}
