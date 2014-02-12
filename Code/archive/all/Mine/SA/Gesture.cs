using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SpeechBasics.Event_Arguments;


namespace Microsoft.Samples.Kinect.SpeechBasics
{
    class Gesture
    {
        private IRelativeGestureSegment[] gestureParts;

        private int currentGesturePart = 0;

        private int pausedFrameCount = 10;

        private int frameCount = 0;

        private bool paused = false;

        private GestureType type;

        private GesturePartResult result;

        public Gesture(GestureType type, IRelativeGestureSegment[] gestureParts)
        {
            this.gestureParts = gestureParts;
            this.type = type;
        }

        public event EventHandler<GestureEventArgs> GestureRecognised;

        public void UpdateGesture(Skeleton data)
        {
            //if you reach max pause time unpause
            if (this.paused)
            {
                if (this.frameCount == this.pausedFrameCount)
                {
                    this.paused = false;
                }

                this.frameCount++;
            }
            

            result = this.gestureParts[this.currentGesturePart].CheckGesture(data);
            if (result == GesturePartResult.Succeed)
            {
                //if theres another part, reset framecount, and pause for next input
                if (this.currentGesturePart + 1 < this.gestureParts.Length)
                {
                    this.currentGesturePart++;
                    this.frameCount = 0;
                    this.pausedFrameCount = 10;
                    this.paused = true;
                }
                else
                {
                    if (this.GestureRecognised != null)
                    {
                        this.GestureRecognised(this, new GestureEventArgs(this.type, data.TrackingId));
                        this.Reset();
                    }
                }
            }
            else if(result == GesturePartResult.Fail || this.frameCount== 50)
            {
                this.currentGesturePart = 0;
                this.frameCount = 0;
                this.pausedFrameCount = 5;
                this.paused = true;
            }
            else
            {
                this.frameCount++;
                this.pausedFrameCount = 5;
                this.paused = true;
            }

        }

        public void Reset()
        {
            this.currentGesturePart = 0;
            this.frameCount = 0;
            this.pausedFrameCount = 5;
            this.paused = true;
        }





    }
}
