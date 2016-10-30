using Leap;

using IUtilMusic.Keyboard;

namespace IUtilMusic.Gestures
{
   /// <summary>
   /// Interface that describe the way listener should be implemented
   /// </summary>
    public interface IGestureDetector
    {
        /// <summary>
        /// Register current frame of the controller as part of the gesture
        /// </summary>
        /// <param name="frame">Current frame</param>
        void RegisterFrame(Frame frame);
        /// <summary>
        /// Determine whether the gesture is valid or not
        /// </summary>
        /// <returns>True if the gesture is valid, otherwise false</returns>
        bool IsGestureValid();
        /// <summary>
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        /// <param name="keyboardListener">Keyboard's listener to perform the events</param>
        void ExecuteGesture(KeyboardListener keyboardListener);
        /// <summary>
        /// Clear tracked data if necessary
        /// </summary>
        void ClearFrames();

    }
}
