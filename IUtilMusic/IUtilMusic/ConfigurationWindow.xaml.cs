using System;
using System.Windows;
using System.Windows.Controls;

using IUtilMusic.Gestures;
using IUtilMusic.Persistence;

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
            cbNotifications.IsChecked = Config.getInstance().ShowNotification;
            cbStartupAtLogin.IsChecked = StartupShortcut.Exist();
        }
        #endregion

        #region Properties

        #endregion

        #region  Events Handlers

        private void cbNotifications_Click(object sender, RoutedEventArgs e)
        {
            Config.getInstance().ShowNotification = Convert.ToBoolean(((CheckBox)sender).IsChecked);
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
