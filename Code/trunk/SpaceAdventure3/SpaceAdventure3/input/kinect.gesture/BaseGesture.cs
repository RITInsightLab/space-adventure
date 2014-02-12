using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;

namespace SpaceAdventure3.input.kinect.gesture
{

    /**
     * SuperClass for all gestures
     * Author: Tyler Geery
     **/
	public abstract class BaseGesture
	{
        public const float PAN_PADDING_SCALE = 1.7f;

        abstract public GestureResult CheckGesture(Skeleton skeleton);
	}
}
