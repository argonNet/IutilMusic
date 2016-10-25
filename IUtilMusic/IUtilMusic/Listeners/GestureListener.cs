using System;
using System.Linq;
using System.Collections.Generic;

using Leap;
using Accord.Statistics.Models.Regression.Linear;

using IUtilMusic.Handlers;

namespace IUtilMusic.Listeners
{
    /// <summary>
    /// Listener that creates and manages gestures
    /// </summary>
    public class GestureListener
    {
        public enum Side
        {
            Left,
            Right
        }

        #region Consts
        /// <summary>
        /// Max number of inputs saved for the gesture
        /// </summary>
        /// <remarks>This number also apply to outputs</remarks>
        private const int GESTURE_LENGTH = 15;
        private const int MIN_GESTURE_VELOCITY_X_FRAME_DECTECTION = 500;
        private const int MAX_GESTURE_VELOCITY_X_VALIDATION = 1200;
        private const long MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS = 500;
        /// <summary>
        /// Precision of the coefficient of determination that is acceptable to execute events
        /// </summary>
        private const double MIN_R = 0.5;
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

        private int _frameGestureCount;

        private double _xVelocityMax;
        private double _xVelocityMin;
        private long _lastGestureDetectedInMillis;
        #endregion

        #region Constructors
        /// <summary>
        /// Listener that creates and manages gestures
        /// init local variables
        /// </summary>
        public GestureListener()
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

        #region Methods
        #region Private

        /// <summary>
        /// Check results of the calculated regression
        /// </summary>
        public void CheckOutput()
        {
            Console.WriteLine("Slope : {0}", _regression.Slope);
            Console.WriteLine("Error : {0}", _coefficientDetermination);
        }
        #endregion



        public static long CurrentTimeMillis()
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        #region Public
        /// <summary>
        /// Register current frame of the controller as part of the gesture
        /// </summary>
        /// <param name="frame">Current frame</param>
        public void RegisterFrame(Frame frame)
        {
            Side currentGestureDirection;
            Hand selectedHand = frame.Hands[0];

            if (Math.Abs(selectedHand.PalmVelocity.x) >= MIN_GESTURE_VELOCITY_X_FRAME_DECTECTION)
            {
                _frameGestureCount++;

                //Determine the direction of the initiated gesture
                currentGestureDirection = selectedHand.PalmVelocity.x > 0 ? Side.Right : Side.Left;

                _inputs.Add(selectedHand.StabilizedPalmPosition.x);
                _outputs.Add(selectedHand.StabilizedPalmPosition.y);

                //Use Ordinary Least Squares to learn the regression
                try
                {
                    _regression = _ols.Learn(_inputs.ToArray(), _outputs.ToArray());


                    //Gets the coefficient of determination, as known R-squared
                    _coefficientDetermination = _regression.CoefficientOfDetermination(_inputs.ToArray(), _outputs.ToArray());
                    Console.WriteLine(_coefficientDetermination);
                    //Checking max velocity on the gesture 
                    //Abs use for compatibility for both gesture (right and left)
                    _xVelocityMax = Math.Max(Math.Abs(selectedHand.PalmVelocity.x), _xVelocityMax);
                    _xVelocityMin = Math.Min(Math.Abs(selectedHand.PalmVelocity.x), _xVelocityMin);

                    if (_frameGestureCount >= GESTURE_LENGTH && //We reach the end of the gesture
                        _xVelocityMax >= MAX_GESTURE_VELOCITY_X_VALIDATION && //The max velocity
                        CurrentTimeMillis() - _lastGestureDetectedInMillis > MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS && //To prevent long gesture launch several Swipe
                        Math.Abs(_coefficientDetermination) >= MIN_R &&
                        -MIN_SLOPE <= _regression.Slope && _regression.Slope <= MIN_SLOPE)
                    {
                        Console.WriteLine("R valide:{0}",_coefficientDetermination);
                        
                        //Register time the gesture was detected
                        _lastGestureDetectedInMillis = CurrentTimeMillis();

                        //Fire the event
                        if (currentGestureDirection == Side.Right) KeyboardHandler.DoNextTrack();
                        else if (currentGestureDirection == Side.Left) KeyboardHandler.DoPreviousTrack();

                    }

                    //If the gesture change the direction it started with, we clear it
                    if ((currentGestureDirection == Side.Right && selectedHand.PalmVelocity.x < 0) ||
                        (currentGestureDirection == Side.Left && selectedHand.PalmVelocity.x > 0) ||
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
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        public void ExecuteEvent()
        {

            //KeyboardHandler.DoMediaPlayPause();
        }
        #endregion
        #endregion

    }
}
