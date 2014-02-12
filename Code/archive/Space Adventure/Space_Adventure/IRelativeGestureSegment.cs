using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    // Defines a single gesture segment using relative positioning of joints
    public interface IRelativeGestureSegment
    {
        // Checks the gesture
        GesturePartResult CheckGesture(Skeleton skeleton);
        
    }
}
