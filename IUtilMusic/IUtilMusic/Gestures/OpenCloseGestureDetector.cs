using System;

using IUtilMusic.Keyboard;

namespace IUtilMusic.Gestures
{
    /// <summary>
    /// This class detect when hand is closed and then opened and execute some events when the hand is opened.
    /// </summary>
    /// <remarks>/!\ Only work for one hand /!\</remarks>
    public class OpenCloseGestureDetector : GestureDetectorAbstract
    {
        #region Consts
        /// <summary>
        /// The angle limit for the finger when the hand is closed
        /// </summary>
        /// <remarks>Angle unit is radian</remarks>
        private const double FINGER_ANGLE_LIMIT_FOR_CLOSED_HAND = 0.1;
        /// <summary>
        /// The angle limit for the finger when the hand is opened
        /// </summary>
        /// <remarks>Angle unit is radian</remarks>
        private const double FINGER_ANGLE_LIMIT_FOR_OPEN_HAND = 0.3;

        /// <summary>
        /// The angle limit for the roll's hand 
        /// </summary>
        /// <remarks>Angle unit is radian</remarks>
        private const double ROLL_HAND_ANGLE_LIMIT = 0.3;

        /// <summary>
        /// The min time between the open and close gestures
        /// </summary>
        private const long OPEN_CLOSE_GESTURE_DELAY = 1000;
        /// <summary>
        /// The min time between gesture
        /// </summary>
        private const long MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS = 1000; 
        #endregion

        #region Members
        /// <summary>
        /// Determine whether the hand was opened in the previous frame or not
        /// </summary>
        private bool _wasOpen;
        /// <summary>
        /// Determine whether the hand is currently opened or not
        /// </summary>
        private bool _isOpen;
        /// <summary>
        /// Determine whether the hand is currently closed or not
        /// </summary>
        private bool _isClose;

        /// <summary>
        /// Last hand closed gesture detected in millis
        /// </summary>
        private long _lastHandCloseDetectedInMillis;
        /// <summary>
        /// Last hand opened gesture detected in millis
        /// </summary>
        private long _lastHandOpenDetectedInMillis; 
        #endregion

        #region Constructors
        /// <summary>
        /// This class detect when hand is closed and then opened and execute some events when the hand is opened.
        /// init local variables
        /// </summary>
        /// <<param name="rightOrLeftHanded">Determine whether we are in Right-Handed mode or Left-Handed mode</param>
        public OpenCloseGestureDetector(Side rightOrLeftHanded)
            : base(rightOrLeftHanded)
        {
            _wasOpen = false;
            _isOpen = false;
            _isClose = false;

            _lastHandCloseDetectedInMillis = 0;
            _lastHandOpenDetectedInMillis = 0;
        } 
        #endregion

        #region Override Methods
        #region Protected
        /// <summary>
        /// Register data that corresponds to the gesture 
        /// </summary>
        protected override void RegisterGesture()
        {
            if (Math.Abs(this.SelectedHand.PalmNormal.Roll) <= ROLL_HAND_ANGLE_LIMIT)
            {
                //The angle is 0 radian for an open hand, and reaches pi radians when the pose is a tight fist.
                _isOpen = Math.Abs(this.SelectedHand.GrabAngle) <= FINGER_ANGLE_LIMIT_FOR_CLOSED_HAND;

                _isClose = Math.PI - FINGER_ANGLE_LIMIT_FOR_OPEN_HAND <= Math.Abs(this.SelectedHand.GrabAngle) &&
                           Math.Abs(this.SelectedHand.GrabAngle) <= Math.PI + FINGER_ANGLE_LIMIT_FOR_OPEN_HAND;

                if (_isClose) _lastHandCloseDetectedInMillis = Helpers.CurrentTimeMillis();
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Determine whether the gesture is valid or not
        /// </summary>
        /// <returns>True if the gesture is valid, otherwise false</returns>
        public override bool IsGestureValid()
        {
            return !_wasOpen && //Make sure that the hand wasn't already open to launch the gesture twice
                    _isOpen &&
                   Helpers.CurrentTimeMillis() - _lastHandCloseDetectedInMillis <= OPEN_CLOSE_GESTURE_DELAY &&  //Make sure the user do a closed/opened gesture
                   Helpers.CurrentTimeMillis() - _lastHandOpenDetectedInMillis > MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS; //To prevent multiple executed gestures
        }

        /// <summary>
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        /// <param name="keyboardListener">Keyboard's listener to perform the events</param>
        public override void ExecuteGesture(KeyboardListener keyboardListener)
        {
            //Register time the gesture was detected
            _lastHandOpenDetectedInMillis = Helpers.CurrentTimeMillis();

            keyboardListener.DoMediaPlayPause();
        }

        /// <summary>
        /// Clear tracked data if necessary
        /// </summary>
        public override void ClearFrames()
        {
            _wasOpen = _isOpen;
        }
        #endregion 
        #endregion
    }
}
