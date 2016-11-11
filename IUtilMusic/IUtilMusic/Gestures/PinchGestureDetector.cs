using System;

using Leap;

using IUtilMusic.Keyboard;

namespace IUtilMusic.Gestures
{
    /// <summary>
    /// This class detects if the thumb and the index are pinching or not
    /// </summary>
    /// <remarks>/!\ Only work for one hand /!\</remarks>
    public class PinchGestureDetector : GestureDetectorAbstract
    {
        #region Consts
        /// <summary>
        /// Max distance between the index and the thumb (In millimeters)
        /// </summary>
        private const int MAX_INDEX_THUMB_TOUCH = 15;
        /// <summary>
        /// The angle limit for the finger when the hand is opened
        /// </summary>
        /// <remarks>Angle unit is radian</remarks>
        private const double FINGER_ANGLE_LIMIT_FOR_OPEN_HAND = 0.1; 
        #endregion

        #region Members
        /// <summary>
        /// Determine whether the index and the thumb are clipped or not
        /// </summary>
        private bool _isIndexAndThumbClipped;
        /// <summary>
        /// Position of the index's tip
        /// </summary>
        private Vector _indexTipPosition;
        /// <summary>
        /// Position of the thumb's tip
        /// </summary>
        private Vector _thumbTipPosition; 
        #endregion

        #region Constructors
        /// <summary>
        /// This class detects if the thumb and the index are pinching or not
        /// init local variables
        /// </summary>
        /// <param name="rightOrLeftHanded">Determine whether we are in Right-Handed mode or Left-Handed mode</param>
        public PinchGestureDetector(Side rightOrLeftHanded)
            : base(rightOrLeftHanded)
        {
            _isIndexAndThumbClipped = false;
            _isPinched = false;
        } 
        #endregion

        #region Public Properties
        private bool _isPinched;
        /// <summary>
        /// Determine whether the thumb and the index are pinching or not
        /// </summary>
        public bool IsPinched
        {
            get { return _isPinched; }
        } 
        #endregion

        #region Override Methods
        #region Protected
        /// <summary>
        /// Register data that corresponds to the gesture 
        /// </summary>
        protected override void RegisterGesture()
        {
            Finger index = this.SelectedHand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
            Finger thumb = this.SelectedHand.Fingers[(int)Finger.FingerType.TYPE_THUMB];

            _indexTipPosition = new Vector(index.TipPosition.x, index.TipPosition.y, index.TipPosition.z);
            _thumbTipPosition = new Vector(thumb.TipPosition.x, thumb.TipPosition.y, thumb.TipPosition.z);
        }
        #endregion

        #region Public
        /// <summary>
        /// Determine whether the gesture is valid or not
        /// </summary>
        /// <returns>True if the gesture is valid, otherwise false</returns>
        public override bool IsGestureValid()
        {
            bool previousIndexAndTumbClipped = _isIndexAndThumbClipped;
            _isIndexAndThumbClipped = _indexTipPosition.DistanceTo(_thumbTipPosition) <= MAX_INDEX_THUMB_TOUCH; //Add a margin because of some inprecision of the Leap Motion Controller....
            if (_isIndexAndThumbClipped) //Start clipping condition
            {
                return !previousIndexAndTumbClipped &&
                       this.SelectedHand.GrabAngle < Math.PI - FINGER_ANGLE_LIMIT_FOR_OPEN_HAND; //To prevent executing the pinch when having the hand closed
            }
            else //Stop clipping condition
            {
                return previousIndexAndTumbClipped;
            }
        }

        /// <summary>
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        /// <param name="keyboardListener">Keyboard's listener to perform the events</param>
        public override void ExecuteGesture(KeyboardListener keyboardListener)
        {
            _isPinched = _isIndexAndThumbClipped; //Store the pinch state to be available to other gestures
        }

        /// <summary>
        /// Clear tracked data if necessary
        /// </summary>
        public override void ClearFrames()
        {

        }
        #endregion 
        #endregion
    }
}
