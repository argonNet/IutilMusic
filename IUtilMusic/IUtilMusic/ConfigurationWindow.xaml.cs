using System;
using System.Windows;
using System.Windows.Controls;

using IUtilMusic.Gestures;

namespace IUtilMusic
{
    /// <summary>
    /// Window with various configurations to customize the application
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        #region Constructors
        /// <summary>
        /// Window with various configurations to customize the application
        /// Init default value for config parameters
        /// </summary>
        public ConfigurationWindow()
        {
            InitializeComponent();
            _isLogNotificationsEnabled = true;
            _rightOrLeftHandedMode = GestureDetectorAbstract.Side.Right;
        }
        #endregion

        #region Properties
        private bool _isLogNotificationsEnabled;
        /// <summary>
        /// Determine whether the log notifications have to be shown or not
        /// </summary>
        public bool IsLogNotificationsEnabled
        {
            get { return _isLogNotificationsEnabled; }
        }

        private GestureDetectorAbstract.Side _rightOrLeftHandedMode;
        /// <summary>
        ///  Determine whether we are in Right-Handed mode or Left-Handed mode
        /// </summary>
        public GestureDetectorAbstract.Side RightOrLeftHandedMode
        {
            get { return _rightOrLeftHandedMode; }

        }
        #endregion

        #region  Events Handlers
        private void cbNotifications_Click(object sender, RoutedEventArgs e)
        {
            _isLogNotificationsEnabled = Convert.ToBoolean(((CheckBox)sender).IsChecked);
        }

        private void rbtnModeRightHanded_Checked(object sender, RoutedEventArgs e)
        {
            _rightOrLeftHandedMode = GestureDetectorAbstract.Side.Right;
            this.rbtnModeLeftHanded.IsChecked = false;
        }

        private void rbtnModeLeftHanded_Checked(object sender, RoutedEventArgs e)
        {
            _rightOrLeftHandedMode = GestureDetectorAbstract.Side.Left;
            this.rbtnModeRightHanded.IsChecked = false;
        }
        #endregion
    }
}
