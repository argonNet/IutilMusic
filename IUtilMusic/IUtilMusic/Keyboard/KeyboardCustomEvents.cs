using System;
using System.Collections.Generic;

namespace IUtilMusic.Keyboard
{
    /// <summary>
    /// Class for all custom thing made for Keyboard's events
    /// </summary>
    public class KeyboardCustomEvent
    {
        /// <summary>
        /// Custom Event handler for the Keyboard
        /// </summary>
        /// <param name="source">Instance of KeyboardListener</param>
        /// <param name="e">Custom Arg for Keyboard</param>
        public delegate void KeyboardEventHandler(object source, KeyboardArgs e);

        /// <summary>
        /// Custom args for the Keyboard 
        /// </summary>
        /// <remarks>
        /// This is a class which describes the event to the class that recieves it.
        ///  An EventArgs class must always derive from System.EventArgs.
        ///  </remarks>
        public class KeyboardArgs : EventArgs
        {
            /// <summary>
            /// Informations of the key of the event
            /// </summary>
            public List<string> KeyInfo;
            public KeyboardArgs(int type, string text)
            {
                KeyInfo = new List<string>() { type.ToString(), text };
            }

        }
    }
}
