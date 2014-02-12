using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SpeechBasics.Event_Arguments;

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    class GestureController
    {

        //Gesture list
        private List<Gesture> gestures = new List<Gesture>();

        public GestureController()
        {
        }

        public event EventHandler<GestureEventArgs> GestureRecognized;

        //Updates Gestures
        public void UpdateAllGestures(Skeleton data)
        {
            foreach (Gesture gesture in this.gestures)
            {
                gesture.UpdateGesture(data);
            }
        }


        public void AddGesture(GestureType type, IRelativeGestureSegment[] gestureDefinition)
        {
            Gesture gesture = new Gesture(type, gestureDefinition);
            gesture.GestureRecognised += new EventHandler<GestureEventArgs>(this.Gesture_GestureRecognized);
            this.gestures.Add(gesture);
        }

        private void Gesture_GestureRecognized(object sender, GestureEventArgs e)
        {
            if (this.GestureRecognized != null)
            {
                this.GestureRecognized(this, e);
            }
            
            foreach (Gesture g in this.gestures)
            {
                g.Reset();
            }
             
        }




    }
}
