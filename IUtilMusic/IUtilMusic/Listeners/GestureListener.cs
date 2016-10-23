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
        #region Consts
        /// <summary>
        /// Max number of inputs saved for the gesture
        /// </summary>
        /// <remarks>This number also apply to outputs</remarks>
        private const int MAX_INPUTS_LENGTH = 25;

        /// <summary>
        /// Precision of the coefficient of determination that is acceptable to execute events
        /// </summary>
        private const double COEFFICIENT_DETERMINATION_PRECISION = 0.8;
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

        #region Public
        /// <summary>
        /// Register current frame of the controller as part of the gesture
        /// </summary>
        /// <param name="frame">Current frame</param>
        public void RegisterFrame(Frame frame)
        {
            _inputs.Add(frame.Hands[0].Direction.x);
            _outputs.Add(frame.Hands[0].Direction.y);
            if (_inputs.Count() > MAX_INPUTS_LENGTH)
            {
                _inputs.RemoveAt(0);
                _outputs.RemoveAt(0);
                //CheckOutput();
            }
            //Use Ordinary Least Squares to learn the regression
            _regression = _ols.Learn(_inputs.ToArray(), _outputs.ToArray());

            //Gets the coefficient of determination, as known R-squared
            _coefficientDetermination = _regression.CoefficientOfDetermination(_inputs.ToArray(), _outputs.ToArray());
        }

        /// <summary>
        /// Execute specific event depending of the gesture that user made
        /// </summary>
        public void ExecuteEvent()
        {
            if (_inputs.Count() > MAX_INPUTS_LENGTH && _coefficientDetermination > COEFFICIENT_DETERMINATION_PRECISION)
            {
                KeyboardHandler.DoMediaPlayPause();
            }
        }
        #endregion 
        #endregion
        
    }
}
