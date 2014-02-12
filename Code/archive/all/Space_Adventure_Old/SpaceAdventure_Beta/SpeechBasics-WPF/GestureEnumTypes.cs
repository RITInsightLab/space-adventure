// -----------------------------------------------------------------------
// <copyright file="GestureEnumTypes.cs" company="Microsoft Limited">
//  Copyright (c) Microsoft Limited, Microsoft Consulting Services, UK. All rights reserved.
// All rights reserved.
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>Different enums for the gesture service</summary>
//-----------------------------------------------------------------------
namespace Microsoft.Samples.Kinect.SpeechBasics
{
    /// <summary>
    /// the gesture part result
    /// </summary>
    public enum GesturePartResult 
    {
        /// <summary>
        /// Gesture part fail
        /// </summary>
        Fail,

        /// <summary>
        /// Gesture part suceed
        /// </summary>
        Succeed,

        /// <summary>
        /// Gesture part result undetermined
        /// </summary>
        Pausing 
    }

    /// <summary>
    /// The gesture type
    /// </summary>
    public enum GestureType 
    {
        /// <summary>
        /// Right Hand Up
        /// </summary>
        RightUp,

        /// <summary>
        /// Left Hand Up
        /// </summary>
        LeftUp,

        /// <summary>
        /// Swipes Down
        /// </summary>
        SwipeDown,

        /// <summary>
        /// Swiped left
        /// </summary>
        LeftSwipe,

        /// <summary>
        /// swiped right
        /// </summary>
        RightSwipe 
    }
}
