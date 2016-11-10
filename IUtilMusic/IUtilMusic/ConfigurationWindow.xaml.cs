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
            //_rightOrLeftHandedMode = GestureDetectorAbstract.Side.Right;
            cbStartupAtLogin.IsChecked = StartupShortcut.Exist();
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
        
        #endregion

        #region  Events Handlers

        private void cbNotifications_Click(object sender, RoutedEventArgs e)
        {
            _isLogNotificationsEnabled = Convert.ToBoolean(((CheckBox)sender).IsChecked);
        }

        private void cbStartupAtLogin_Click(object sender, RoutedEventArgs e)
        {
            if (cbStartupAtLogin.IsChecked.HasValue && cbStartupAtLogin.IsChecked.Value)
            {
                if(!StartupShortcut.Exist()) StartupShortcut.Add();
            }
            else
            {
                if (StartupShortcut.Exist()) StartupShortcut.Remove();
            }
        }

        #endregion
    }
}
