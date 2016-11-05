using IUtilMusic.Keyboard;
using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IUtilMusic.Gestures
{
    public class OpenCloseGestureDetector : IGestureDetector
    {
        private const double FINGER_ANGLE_LIMIT_FOR_CLOSED_HAND = 0.1;
        private const double FINGER_ANGLE_LIMIT_FOR_OPEN_HAND = 0.3;

        private const long OPEN_CLOSE_GESTURE_DELAY = 1000;

        private const double ROLL_HAND_ANGLE_LIMIT = 0.3;

        /// <summary>
        /// The min time between gesture,
        /// </summary>
        private const long MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS = 1000;

        private bool _wasOpen;
        private bool _isOpen;

        private bool _isClose;
        private long _lastHandCloseStatus;

        /// <summary>
        /// Current hand used for the gesture detection
        /// </summary>
        private Hand _selectedHand;

        /// <summary>
        /// Last gesture detected in millis
        /// </summary>
        private long _lastGestureDetectedInMillis;

        public OpenCloseGestureDetector()
        {
            _wasOpen = false;
            _isOpen = false;

            _isClose = false;
            _lastHandCloseStatus = 0;

            _lastGestureDetectedInMillis = 0;
        }

        public void RegisterFrame(Frame frame)
        {
            _selectedHand = frame.Hands[0];

            if (frame.Hands.Count() > 1)
                foreach (Hand h in frame.Hands)
                    if (h.StabilizedPalmPosition.x > _selectedHand.StabilizedPalmPosition.x)
                        _selectedHand = h;

            if (Math.Abs(_selectedHand.PalmNormal.Roll) <= ROLL_HAND_ANGLE_LIMIT)
            {
                _isOpen = Math.Abs(_selectedHand.GrabAngle) <= FINGER_ANGLE_LIMIT_FOR_CLOSED_HAND;
                _isClose = Math.PI - FINGER_ANGLE_LIMIT_FOR_OPEN_HAND <= Math.Abs(_selectedHand.GrabAngle) &&
                           Math.Abs(_selectedHand.GrabAngle) <= Math.PI + FINGER_ANGLE_LIMIT_FOR_OPEN_HAND;
                if (_isClose) _lastHandCloseStatus = Helpers.CurrentTimeMillis();
            }
        }

        public bool IsGestureValid()
        {
            return !_wasOpen &&
                    _isOpen &&
                   Helpers.CurrentTimeMillis() - _lastHandCloseStatus <= OPEN_CLOSE_GESTURE_DELAY &&
                   Helpers.CurrentTimeMillis() - _lastGestureDetectedInMillis > MIN_DELAY_BETWEEN_GESTURE_IN_MILLIS;
        }

        public void ExecuteGesture(KeyboardListener keyboardListener)
        {
            //Register time the gesture was detected
            _lastGestureDetectedInMillis = Helpers.CurrentTimeMillis();

            keyboardListener.DoMediaPlayPause();
        }

        public void ClearFrames()
        {
            _wasOpen = _isOpen;
        }
    }
}
