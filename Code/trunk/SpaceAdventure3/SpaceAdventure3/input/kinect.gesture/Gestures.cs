using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

/**
 * Gestures.cs 
 * This file contains many short classes, each one defining a different Kinect
 * gesture. The reason they're all in one file is for code readability and 
 * maintainence. 
 * Authors: Tyler Geery and Ross Kahn
 **/
namespace SpaceAdventure3.input.kinect.gesture
{
    
    #region Hands Out
    /**
     * Hands out
     * 
     * Arms are stretched out away from the user's body, making a 'T' shape
     **/
    public class HandsOut : BaseGesture
    {
        override public GestureResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GestureResult.Fail;
            }

            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderLeft].Position.X &&
                skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ElbowLeft].Position.X)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y &&
                    skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y &&
                    skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y &&
                    skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X &&
                    skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    return GestureResult.Success;
                }
            }

            // hands dropped
            return GestureResult.Fail;
        }
    }
    #endregion 

    
    #region Hands In
    /**
     * Hands In
     * 
     * Arms are stretched in front of the user's body, at chest level with
     * hands together
     **/
    public class HandsIn : BaseGesture
    {
        override public GestureResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GestureResult.Fail;
            }

            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ElbowLeft].Position.X)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y &&
                    skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y &&
                    skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y &&
                    skeleton.Joints[JointType.HandRight].Position.X < skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    return GestureResult.Success;
                }
            }

            // hands dropped
            return GestureResult.Fail;
        }
    }
    #endregion


    #region Left Hand Up
    /**
     * The user's left hand is above their head
     **/
    public class LeftHandUp : BaseGesture
    {
        override public GestureResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GestureResult.Fail;
            }
            // left hand is above head
            if ((skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.Head].Position.Y) &&
               (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y))
            {
                return GestureResult.Success;
            }
            else
            {
                // hand dropped
                return GestureResult.Fail;
            }

            
        }
    }
    #endregion

    #region Left Hand Down
    /** Left hand is below the head, to support releasing a click. Should only be valid if the 
     * previous action was a click
     **/
    public class LeftHandDown : BaseGesture
    {
        override public GestureResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GestureResult.Fail;
            }
            // left hand is below head and elbow
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y)
            {
                return GestureResult.Success;
            }
            else
            {
                // hand raised
                return GestureResult.Fail;
            }


        }
    }
    #endregion

    #region Neutral Hands
    // Both hands are below hip, to support releasing zoom. 
    public class NeutralHands : BaseGesture
    {
        override public GestureResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return GestureResult.Fail;
            }

            if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.HipLeft].Position.Y )
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.HipRight].Position.Y )
                {
                    return GestureResult.Success;
                }
            }

            return GestureResult.Fail;
        }
    }
    #endregion 
}
