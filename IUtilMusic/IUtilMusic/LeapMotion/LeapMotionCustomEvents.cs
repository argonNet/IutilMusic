using System;

namespace IUtilMusic.LeapMotion
{
    /// <summary>
    /// Class for all custom thing made for LeapMotion's events
    /// </summary>
    public class LeapMotionCustomEvents
    {
        /// <summary>
        /// Custom Event handler for the Leap Motion
        /// </summary>
        /// <param name="source">Instance of LeapMotionListener</param>
        /// <param name="e">Custom Arg for Leap Motion</param>
        public delegate void LeapMotionEventHandler(object source, LeapMotionArgs e);

        /// <summary>
        /// Custom Event handler for the Leap Motion Device
        /// </summary>
        /// <param name="source">Instance of LeapMotionListener</param>
        /// <param name="e">Custom arg for Leap Motion Device</param>
        public delegate void LeapMotionDeviceEventHandler(object source, LeapMotionDeviceConnectionArgs e);

        /// <summary>
        /// Custom args for the Leap Motion 
        /// </summary>
        /// <remarks>
        /// This is a class which describes the event to the class that recieves it.
        ///  An EventArgs class must always derive from System.EventArgs.
        ///  </remarks>
        public class LeapMotionArgs : EventArgs
        {

            public LeapMotionArgs(string message)
            {
                Message = message;
            }
            public string Message { get; private set; }
        }

        /// <summary>
        /// Custom args for the Leap Motion device
        /// </summary>
        /// <remarks>
        /// This is a class which describes the event to the class that recieves it.
        ///  An EventArgs class must always derive from System.EventArgs.
        ///  </remarks>
        public class LeapMotionDeviceConnectionArgs : EventArgs
        {
            public LeapMotionDeviceConnectionArgs(bool isDeviceConnected)
            {
                IsDeviceConnected = isDeviceConnected;
            }
            public bool IsDeviceConnected { get; private set; }
        }


    }
}
