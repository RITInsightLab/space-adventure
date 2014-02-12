using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.SpeechBasics.Event_Arguments
{
    class GestureEventArgs: EventArgs
    {
        public GestureEventArgs(GestureType type, int trackingID)
        {
            this.TrackingID = trackingID;
            this.GestureType = type;
        }

        public GestureType GestureType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tracking ID.
        /// </summary>
        /// <value>
        /// The tracking ID.
        /// </value>
        public int TrackingID
        {
            get;
            set;
        }

       

    }
}
