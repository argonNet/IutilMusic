using System;
using System.Collections.Generic;

using Leap;
using Accord.Statistics.Models.Regression.Linear;

using IUtilMusic.Keyboard;

namespace IUtilMusic.Gestures
{
    /// <summary>
    /// This class detect swipe and execute some events based of the result of the swipe.
    /// </summary>
    /// <remarks>/!\ Only work for one hand /!\</remarks>
    public class SwipeGestureDetector : GestureDetectorAbstract
    {
        #region Consts
        /// <summary>
        /// Max length of the gesture (in millimeter)
        /// </summary>
        private const int GESTURE_LENGTH = 100;
        /// <summary>
        /// Max numbers of frame for the gesture
        /// </summary>
        private const int FRAME_MAX_GESTURE_LENGTH = 50;
        /// <summary>
        /// The minimum velocity on the X axis to detect frame
        /// </summary>
        private const int MIN_GESTURE_VELOCITY_X_FRAME_DETECTION = 750;
        /// <summary>
        /// The maximum velocity to reach to validate the gesture
        /// </summary>
        private const int MAX_GESTURE_VELOCITY_X_VALIDATION = 1200;
        /// <summary>
        /// The min time between gesture,
        /// </summary>
        private const long MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS = 1000;
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
        /// Current direction of the gesture
        /// </summary>
        private Side _currentGestureDirection;
        /// <summary>
        /// First 3D point of the gesture (Starting point)
        /// </summary>
        private Vector _gestureFirstPoint;
        /// <summary>
        /// Distance runned through by the gesture
        /// </summary>
        private double _distance;
        #endregion

        #region Constructors
        /// <summary>
        /// This class detect swipe and execute some events based of the result of the swipe.
        /// init local variables
        /// </summary>
        /// <<param name="rightOrLeftHanded">Determine whether we are in Right-Handed mode or Left-Handed mode</param>
        public SwipeGestureDetector(Side rightOrLeftHanded)
            : base(rightOrLeftHanded)
        {
            _regression = null;
            _ols = new OrdinaryLeastSquares();
            _inputs = new List<double>();
            _outputs = new List<double>();
            _frameGestureCount = 0;
            _xVelocityMax = 0;
            _lastGestureDetectedInMillis = 0;
            _xVelocityMin = Double.MaxValue;
            _distance = 0;
        }
        #endregion

        #region Override Methods
        #region Protected
        /// <summary>
        /// Register data that corresponds to the gesture 
        /// </summary>
        protected override void RegisterGesture()
        {
            //We only detectr frame with a minimum of velocity in the palm gesture
            if (Math.Abs(this.SelectedHand.PalmVelocity.x) >= MIN_GESTURE_VELOCITY_X_FRAME_DETECTION)
            {
                
                //We keep the departure point for the gesture
                if (_frameGestureCount == 0) _gestureFirstPoint = this.SelectedHand.StabilizedPalmPosition;

                _frameGestureCount++;

                //Determine the direction of the initiated gesture
                _currentGestureDirection = this.SelectedHand.PalmVelocity.x > 0 ? Side.Right : Side.Left;

                _inputs.Add(this.SelectedHand.StabilizedPalmPosition.x);
                _outputs.Add(this.SelectedHand.StabilizedPalmPosition.y);

                //Use Ordinary Least Squares to learn the regression
                try
                {
                    _regression = _ols.Learn(_inputs.ToArray(), _outputs.ToArray());


                    //Gets the coefficient of determination, as known R-squared
                    _coefficientDetermination = _regression.CoefficientOfDetermination(_inputs.ToArray(), _outputs.ToArray());

                    //Checking max velocity on the gesture 
                    //Abs use for compatibility for both gesture (right and left)
                    _xVelocityMax = Math.Max(Math.Abs(this.SelectedHand.PalmVelocity.x), _xVelocityMax);
                    _xVelocityMin = Math.Min(Math.Abs(this.SelectedHand.PalmVelocity.x), _xVelocityMin);

                    //Calc the distance from the first point of the gesture
                    //Do the hypothenus of the triangle given from the delta between the first point and the current point
                    _distance = Math.Sqrt(Math.Pow(this.SelectedHand.StabilizedPalmPosition.x - _gestureFirstPoint.x,2) + 
                                          Math.Pow(this.SelectedHand.StabilizedPalmPosition.y - _gestureFirstPoint.y,2));

                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                    _regression = null;
                    _frameGestureCount = 0;
                    _xVelocityMax = 0;
                    _xVelocityMin = Double.MaxValue;
                    _distance = 0;
                    _inputs.Clear();
                    _outputs.Clear();
                }


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
            return (_distance >= GESTURE_LENGTH && //We reach the end of the gesture
                       _xVelocityMax >= MAX_GESTURE_VELOCITY_X_VALIDATION && //The max velocity
                       Helpers.CurrentTimeMillis() - _lastGestureDetectedInMillis > MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS && //To prevent long gesture launch several Swipe
                       Math.Abs(_coefficientDetermination) >= MIN_R &&
                       -MIN_SLOPE <= _regression.Slope && _regression.Slope <= MIN_SLOPE);
        }
        /// <summary>
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        /// <param name="keyboardListener">Keyboard's listener to perform the events</param>
        public override void ExecuteGesture(KeyboardListener keyboardListener)
        {
            //Register time the gesture was detected
            _lastGestureDetectedInMillis = Helpers.CurrentTimeMillis();

            //Fire the event
            if (_currentGestureDirection == Side.Right) keyboardListener.DoNextTrack();
            else if (_currentGestureDirection == Side.Left) keyboardListener.DoPreviousTrack();
        }

        /// <summary>
        /// Clear tracked data if necessary
        /// </summary>
        public override void ClearFrames()
        {
            //If the gesture change the direction it started with, we clear it
            if ((_currentGestureDirection == Side.Right && this.SelectedHand.PalmVelocity.x < 0) ||
                (_currentGestureDirection == Side.Left && this.SelectedHand.PalmVelocity.x > 0) ||
                _frameGestureCount >= FRAME_MAX_GESTURE_LENGTH ||
                _distance >= GESTURE_LENGTH) //The gesture ended we have to check several things
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
        #endregion
    }
}
