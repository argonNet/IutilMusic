using System;

using IUtilMusic.Keyboard;

namespace IUtilMusic.Gestures
{
    /// <summary>
    /// This class detect when hand is moving up or down execute some events depending of the direction
    /// </summary>
    /// <remarks>/!\ Only work for one hand /!\</remarks>
    public class UpDownGestureDetector : GestureDetectorAbstract
    {
        #region Consts
        /// <summary>
        /// Min distance of the gesture to activate the detection
        /// </summary>
        private const double MIN_DISTANCE_TO_RAISE_DETECTION = 3;
        #endregion

        #region Members
        /// <summary>
        /// The lastest Y position of the hand
        /// </summary>
        private double _lastFrameYPosition;
        /// <summary>
        /// Current vertical movement of the hand
        /// </summary>
        private double _currentProgression;
        /// <summary>
        /// Vertical shifting done in current frame
        /// </summary>
        private double _shifting;
        /// <summary>
        /// Pinch gesture state
        /// </summary>
        private PinchGestureDetector _pinchGesture;
        #endregion

        #region Constructors
        /// <summary>
        /// This class detect when hand is moving up or down execute some events depending of the direction
        /// init local variables
        /// </summary>
        /// <param name="rightOrLeftHanded">Determine whether we are in Right-Handed mode or Left-Handed mode</param>
        /// <param name="pinchGesture">Pinch gesture state</param>
        public UpDownGestureDetector(Side rightOrLeftHanded, PinchGestureDetector pinchGesture)
            : base(rightOrLeftHanded)
        {
            _lastFrameYPosition = 0;
            _currentProgression = 0;
            _shifting = 0;
            _pinchGesture = pinchGesture;
        }
        #endregion

        #region Override Methods
        #region Protected
        /// <summary>
        /// Register data that corresponds to the gesture 
        /// </summary>
        protected override void RegisterGesture()
        {
            _shifting = this.SelectedHand.PalmPosition.y - _lastFrameYPosition;
        }
        #endregion

        #region Public
        /// <summary>
        /// Determine whether the gesture is valid or not
        /// </summary>
        /// <returns>True if the gesture is valid, otherwise false</returns>
        public override bool IsGestureValid()
        {
            return Math.Abs(_currentProgression) >= MIN_DISTANCE_TO_RAISE_DETECTION && _pinchGesture.IsPinched;
        }


        /// <summary>
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        /// <param name="keyboardListener">Keyboard's listener to perform the events</param>
        public override void ExecuteGesture(KeyboardListener keyboardListener)
        {
            if (this.SelectedHand.PalmVelocity.y > 0) keyboardListener.DoVolumeUp();
            else keyboardListener.DoVolumeDown();

            _currentProgression = 0;
        }

        /// <summary>
        /// Clear tracked data if necessary
        /// </summary>
        public override void ClearFrames()
        {
            _currentProgression += _shifting; //Add the shifting made in this frame to the main movement
            _lastFrameYPosition = this.SelectedHand.PalmPosition.y;
        }
        #endregion
        #endregion
    }
}
