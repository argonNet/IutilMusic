using System;
using System.Collections.Generic;

using Leap;
using Accord.Statistics.Models.Regression.Linear;

using IUtilMusic.Keyboard;

namespace IUtilMusic.Gestures
{
    /// <summary>
    /// This class detect swipe and execute some events based of the result of the swipte.
    /// </summary>
    /// <remarks>/!\ Only work for on hand /!\</remarks>
    public class SwipeGestureDetector : IGestureDetector
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

        #region Consts
        /// <summary>
        /// Max number of inputs saved for the gesture
        /// </summary>
        /// <remarks>This number also apply to outputs</remarks>
        private const int GESTURE_LENGTH = 15;
        /// <summary>
        /// The minimum velocity on the X axis to detect frame
        /// </summary>
        private const int MIN_GESTURE_VELOCITY_X_FRAME_DECTECTION = 500;
        /// <summary>
        /// The maximum velocity to reach to validate the gesture
        /// </summary>
        private const int MAX_GESTURE_VELOCITY_X_VALIDATION = 1200;
        /// <summary>
        /// The min time between gesture,
        /// </summary>
        private const long MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS = 500;
        /// <summary>
        /// The R coefficient of the the linear regression done on the gesture (we only take care of X and Y axis)
        /// </summary>
        private const double MIN_R = 0.5;
        /// <summary>
        /// The slope of the gesture with  the X axis
        /// </summary>
        private const double MIN_SLOPE = 0.5;
        #endregion

        #region Members
        /// <summary>
        /// Coefficient of determination, also known as  R-squared
        /// </summary>
        /// <remarks>The closer it is to 1, the more it is a simple line</remarks>
        private double _coefficientDetermination;
        /// <summary>
        /// Simple linear regression object
        /// </summary>
        private SimpleLinearRegression _regression;
        /// <summary>
        /// Ordinary Least Squares object used to calculate the regression
        /// </summary>
        private OrdinaryLeastSquares _ols;
        /// <summary>
        /// Array that contains inputs (X) to calculate the regression
        /// </summary>
        private List<double> _inputs;
        /// <summary>
        ///  Array that contains outputs (Y) to calculate the regression
        /// </summary>
        private List<double> _outputs;

        /// <summary>
        /// Current frame gesture count
        /// </summary>
        private int _frameGestureCount;

        /// <summary>
        /// Current max X velocity
        /// </summary>
        private double _xVelocityMax;
        /// <summary>
        /// Current min X velocity
        /// </summary>
        private double _xVelocityMin;
        /// <summary>
        /// Last gesture detected in millis
        /// </summary>
        private long _lastGestureDetectedInMillis;
        /// <summary>
        /// Current hand used for the gesture detection
        /// </summary>
        private Hand _selectedHand;
        /// <summary>
        /// Current direction of the gesture
        /// </summary>
        private Side _currentGestureDirection;
        #endregion

        #region Constructors
        /// <summary>
        /// Listener that creates and manages gestures
        /// init local variables
        /// </summary>
        public SwipeGestureDetector()
        {
            _regression = null;
            _ols = new OrdinaryLeastSquares();
            _inputs = new List<double>();
            _outputs = new List<double>();
            _frameGestureCount = 0;
            _xVelocityMax = 0;
            _lastGestureDetectedInMillis = 0;
            _xVelocityMin = Double.MaxValue;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Register current frame of the controller as part of the gesture
        /// TODO: Check to how to select hand.. The rightmost doesn't exist...
        /// </summary>
        /// <param name="frame">Current frame</param>
        public void RegisterFrame(Frame frame)
        {
            _selectedHand = frame.Hands[0];


            if (Math.Abs(_selectedHand.PalmVelocity.x) >= MIN_GESTURE_VELOCITY_X_FRAME_DECTECTION)
            {
                _frameGestureCount++;

                //Determine the direction of the initiated gesture
                _currentGestureDirection = _selectedHand.PalmVelocity.x > 0 ? Side.Right : Side.Left;

                _inputs.Add(_selectedHand.StabilizedPalmPosition.x);
                _outputs.Add(_selectedHand.StabilizedPalmPosition.y);

                //Use Ordinary Least Squares to learn the regression
                try
                {
                    _regression = _ols.Learn(_inputs.ToArray(), _outputs.ToArray());


                    //Gets the coefficient of determination, as known R-squared
                    _coefficientDetermination = _regression.CoefficientOfDetermination(_inputs.ToArray(), _outputs.ToArray());

                    //Checking max velocity on the gesture 
                    //Abs use for compatibility for both gesture (right and left)
                    _xVelocityMax = Math.Max(Math.Abs(_selectedHand.PalmVelocity.x), _xVelocityMax);
                    _xVelocityMin = Math.Min(Math.Abs(_selectedHand.PalmVelocity.x), _xVelocityMin);

                    //if (_frameGestureCount >= GESTURE_LENGTH && //We reach the end of the gesture
                    //    _xVelocityMax >= MAX_GESTURE_VELOCITY_X_VALIDATION && //The max velocity
                    //    Helpers.CurrentTimeMillis() - _lastGestureDetectedInMillis > MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS && //To prevent long gesture launch several Swipe
                    //    Math.Abs(_coefficientDetermination) >= MIN_R &&
                    //    -MIN_SLOPE <= _regression.Slope && _regression.Slope <= MIN_SLOPE)
                    //{
                    //    Console.WriteLine("R valide:{0}", _coefficientDetermination);

                    //    //Register time the gesture was detected
                    //    _lastGestureDetectedInMillis = Helpers.CurrentTimeMillis();

                    //    //Fire the event
                    //    if (_currentGestureDirection == Side.Right) KeyboardHandler.DoNextTrack();
                    //    else if (_currentGestureDirection == Side.Left) KeyboardHandler.DoPreviousTrack();

                    //}

                    ////If the gesture change the direction it started with, we clear it
                    //if ((_currentGestureDirection == Side.Right && _selectedHand.PalmVelocity.x < 0) ||
                    //    (_currentGestureDirection == Side.Left && _selectedHand.PalmVelocity.x > 0) ||
                    //    _frameGestureCount >= GESTURE_LENGTH) //The gesture ended we have to check several things
                    //{
                    //    _regression = null;
                    //    _frameGestureCount = 0;
                    //    _xVelocityMax = 0;
                    //    _xVelocityMin = Double.MaxValue;
                    //    _inputs.Clear();
                    //    _outputs.Clear();
                    //}
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                    _regression = null;
                    _frameGestureCount = 0;
                    _xVelocityMax = 0;
                    _xVelocityMin = Double.MaxValue;
                    _inputs.Clear();
                    _outputs.Clear();
                }


            }
        }

        /// <summary>
        /// Determine whether the gesture is valid or not
        /// </summary>
        /// <returns>True if the gesture is valid, otherwise false</returns>
        public bool IsGestureValid()
        {
            return (_frameGestureCount >= GESTURE_LENGTH && //We reach the end of the gesture
                       _xVelocityMax >= MAX_GESTURE_VELOCITY_X_VALIDATION && //The max velocity
                       Helpers.CurrentTimeMillis() - _lastGestureDetectedInMillis > MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS && //To prevent long gesture launch several Swipe
                       Math.Abs(_coefficientDetermination) >= MIN_R &&
                       -MIN_SLOPE <= _regression.Slope && _regression.Slope <= MIN_SLOPE);
        }
        /// <summary>
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        /// <param name="keyboardListener">Keyboard's listener to perform the events</param>
        public void ExecuteGesture(KeyboardListener keyboardListener)
        {
            Console.WriteLine("R valide:{0}", _coefficientDetermination);

            //Register time the gesture was detected
            _lastGestureDetectedInMillis = Helpers.CurrentTimeMillis();

            //Fire the event
            if (_currentGestureDirection == Side.Right) keyboardListener.DoNextTrack();
            else if (_currentGestureDirection == Side.Left) keyboardListener.DoPreviousTrack();
        }

        /// <summary>
        /// Clear tracked data if necessary
        /// </summary>
        public void ClearFrames()
        {
            //If the gesture change the direction it started with, we clear it
            if ((_currentGestureDirection == Side.Right && _selectedHand.PalmVelocity.x < 0) ||
                (_currentGestureDirection == Side.Left && _selectedHand.PalmVelocity.x > 0) ||
                _frameGestureCount >= GESTURE_LENGTH) //The gesture ended we have to check several things
            {
                _regression = null;
                _frameGestureCount = 0;
                _xVelocityMax = 0;
                _xVelocityMin = Double.MaxValue;
                _inputs.Clear();
                _outputs.Clear();
            }
        }
        #endregion
    }
}
