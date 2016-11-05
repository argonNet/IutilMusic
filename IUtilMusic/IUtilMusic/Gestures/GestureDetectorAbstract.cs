using System.Linq;

using Leap;

using IUtilMusic.Keyboard;

namespace IUtilMusic.Gestures
{
    /// <summary>
    /// Base class for all the gesture detectors
    /// </summary>
    public abstract class GestureDetectorAbstract
    {
        #region Enum
        /// <summary>
        /// Side of the hand
        /// </summary>
        public enum Side
        {
            Left,
            Right
        }
        #endregion

        #region Members
        /// <summary>
        /// Determine whether we are in Right-Handed mode or Left-Handed mode
        /// </summary>
        private readonly Side _rightOrLeftHanded; 
        #endregion

        #region Constructors
        /// <summary>
        /// Base class for all the gesture detectors
        /// </summary>
        /// <param name="rightOrLeftHanded">Determine whether we are in Right-Handed mode or Left-Handed mode</param>
        public GestureDetectorAbstract(Side rightOrLeftHanded)
        {
            this._rightOrLeftHanded = rightOrLeftHanded;
        } 
        #endregion

        #region Protected Properties
        private Hand _selectedHand;
        /// <summary>
        /// Current hand used for the gesture detection
        /// </summary>
        protected Hand SelectedHand
        {
            get { return _selectedHand; }
        } 
        #endregion

        #region Methods
        #region Protected Abstract
        /// <summary>
        /// Register data that corresponds to the gesture 
        /// </summary>
        protected abstract void RegisterGesture();
        #endregion

        #region Public
        #region Abstract
        /// <summary>
        /// Determine whether the gesture is valid or not
        /// </summary>
        /// <returns>True if the gesture is valid, otherwise false</returns>
        public abstract bool IsGestureValid();
        /// <summary>
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        /// <param name="keyboardListener">Keyboard's listener to perform the events</param>
        public abstract void ExecuteGesture(KeyboardListener keyboardListener);
        /// <summary>
        /// Clear tracked data if necessary
        /// </summary>
        public abstract void ClearFrames();
        #endregion

        /// <summary>
        /// Register current frame of the controller as part of the gesture
        /// also select the hand that will make the gesture
        /// </summary>
        /// <param name="frame">Current frame</param>
        public void RegisterFrame(Frame frame)
        {
            _selectedHand = frame.Hands[0];

            if (frame.Hands.Count() > 1)
                foreach (Hand h in frame.Hands)
                {
                    if ((_rightOrLeftHanded == Side.Right && h.StabilizedPalmPosition.x > _selectedHand.StabilizedPalmPosition.x) ||
                        (_rightOrLeftHanded == Side.Left && h.StabilizedPalmPosition.x < _selectedHand.StabilizedPalmPosition.x))
                        _selectedHand = h;
                }

            RegisterGesture();
        }
        #endregion       
        #endregion
    }
}
