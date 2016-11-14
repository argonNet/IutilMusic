using System;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Leap;

using IUtilMusic.Keyboard;
using IUtilMusic.LeapMotion;

using WPFTaskbarNotifierLog;

namespace IUtilMusic
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        #region Members
        /// <summary>
        /// Instance of the System Tray icon of the application
        /// </summary>
        private NotifyIcon _notifyIcon;
        /// <summary>
        /// Determine if application is closed or not
        /// </summary>
        private bool _isExit;
        /// <summary>
        /// Taskbar notifier used for log information of Leap Motion and Keyboard
        /// </summary>
        private LogTaskbarNotifier _taskbarNotifier;
        /// <summary>
        /// Instance of the configuration window
        /// </summary>
        private ConfigurationWindow _configWindow;
        /// <summary>
        /// Instance of the Leap Motion Controller disconnect's image window
        /// </summary>
        private LMDeviceNotConnectedImage _leapMotionDeviceNotConnectedImage;
        #endregion

        #region Methods
        /// <summary>
        /// Create the Notify Icon and register its events
        /// </summary>
        private void InitSysTrayIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowConfigurationWindow();
            _notifyIcon.Icon = IUtilMusic.Properties.Resources.AppIcon;
            _notifyIcon.Visible = true;
            InitContextMenu();
        }
        /// <summary>
        /// Create the keyboard listener and register its events
        /// </summary>
        /// <returns></returns>
        private KeyboardListener InitKeyboardListener()
        {
            KeyboardListener keyListener = new KeyboardListener();
            keyListener.OnKeyDownInformation += new KeyboardCustomEvent.KeyboardEventHandler(ShowKeyboardMessage);
            return keyListener;
        }

        /// <summary>
        /// Create the Leap Motion's controller and register its events
        /// </summary>
        private void InitLeapMotionController(KeyboardListener keyListener)
        {
            using (Controller controller = new Controller())
            {
                LeapMotionListener leapMotionListener = new LeapMotionListener(keyListener);
                leapMotionListener.OnShowInformations += new LeapMotionCustomEvents.LeapMotionEventHandler(ShowLeapMotionMessage);
                leapMotionListener.OnDeviceConnectionStateChanged += new LeapMotionCustomEvents.LeapMotionDeviceEventHandler(IsLeapMotionDeviceConnected);
                controller.Connect += leapMotionListener.OnServiceConnect;               
                controller.Disconnect += leapMotionListener.OnServiceDisconnect;
                controller.Device += leapMotionListener.OnConnect;
                controller.DeviceLost += leapMotionListener.OnDisconnect;
                controller.DeviceFailure += leapMotionListener.OnDeviceFailure;
                controller.FrameReady += leapMotionListener.OnFrame;
                controller.LogMessage += leapMotionListener.OnLogMessage;
  
            }
        }

        #region ContextMenu
        /// <summary>
        /// Create the context menu of the System Tray Icon
        /// </summary>
        private void InitContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
              new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Configuration...").Click += (s, e) => ShowConfigurationWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();

        }

        /// <summary>
        /// Display the configuration's form
        /// </summary>
        private void ShowConfigurationWindow()
        {
            if (_configWindow.IsVisible)
            {
                if (_configWindow.WindowState == WindowState.Minimized)
                {
                    _configWindow.WindowState = WindowState.Normal;
                }
                _configWindow.Activate();
            }
            else
            {
                _configWindow.Show();
            }
        }

        /// <summary>
        /// Close application and clear memory
        /// </summary>
        private void ExitApplication()
        {
            _isExit = true;
            _configWindow.Close();
            if (_notifyIcon != null)
            {
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }

            //Save the config
            IUtilMusic.Persistence.Config.getInstance().Dispose();

            //Stop the application
            Environment.Exit(0);
        }  
        #endregion

        /// <summary>
        /// Show a ballon notification
        /// </summary>
        /// <param name="title">Message to display on the notification</param>
        private void ShowBalloon(string message)
        {

                _taskbarNotifier.NotifyContent.Clear();

                _taskbarNotifier.NotifyContent.Add(new NotifyObject(message));
                // Tell the TaskbarNotifier to open.
                _taskbarNotifier.Notify();
        }
        #endregion

        #region Events
        #region Private
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                _configWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }

        /// <summary>
        /// Show information message concerning leap motion to end-user
        /// </summary>
        /// <param name="source">Instance of LeapMotionListener</param>
        /// <param name="e">Custom Arg for Leap Motion</param>
        private void ShowLeapMotionMessage(object sender, LeapMotionCustomEvents.LeapMotionArgs e)
        {
            ShowBalloon( e.Message);
        }

        /// <summary>
        /// Display disconnect image to end-user depending of the connection's state of the Leap Motion device
        /// </summary>
        /// <param name="source">Instance of LeapMotionListener</param>
        /// <param name="e">Custom arg for Leap Motion Device</param>
        private void IsLeapMotionDeviceConnected (object sender, LeapMotionCustomEvents.LeapMotionDeviceConnectionArgs e)
        {
            if (e.IsDeviceConnected) _leapMotionDeviceNotConnectedImage.Visibility = Visibility.Hidden;
            else _leapMotionDeviceNotConnectedImage.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Show information message concerning keyboard to end-user
        /// TODO: Make something cleaner !
        /// </summary>
        /// <param name="source">Instance of KeyboardListener</param>
        /// <param name="e">Custom Arg for the keyboard</param>
        private void ShowKeyboardMessage(object sender, KeyboardCustomEvent.KeyboardArgs e)
        {
            if (IUtilMusic.Persistence.Config.getInstance().ShowNotification)
            {
                List<string> infosList = e.KeyInfo;
                string gestureName;
                switch (Convert.ToInt32(infosList[0]))
                {
                    case 1:
                        gestureName = "right swipe";
                        break;
                    case 2:
                        gestureName = "left swipe";
                        break;
                    case 3:
                        gestureName = "up hand";
                        break;
                    case 4:
                        gestureName = "down hand";
                        break;
                    case 5:
                        gestureName = "open hand";
                        break;
                    default:
                        throw new NotImplementedException("Unknown gesture.");
                }
                ShowBalloon(String.Format("Gesture {0} executed ({1})", gestureName, infosList[1]));
            }
        }

        /// <summary>
        /// Prevent users to close the window having the disconnect image !
        /// </summary>
        /// <param name="sender">Instance of the image's window</param>
        /// <param name="e">Cancel args</param>
        private void LMDeviceNotConnectedImageWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion

        #region Override Protected
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _leapMotionDeviceNotConnectedImage = new LMDeviceNotConnectedImage();
            _leapMotionDeviceNotConnectedImage.Closing += LMDeviceNotConnectedImageWindow_Closing;
            _leapMotionDeviceNotConnectedImage.Show();
            _configWindow = new ConfigurationWindow();
            _configWindow.Closing += MainWindow_Closing;
           
            InitSysTrayIcon();
            _taskbarNotifier = new LogTaskbarNotifier();
            _taskbarNotifier.Show();
            KeyboardListener keyListener = InitKeyboardListener();
            InitLeapMotionController(keyListener);
        }
        #endregion
        #endregion
    }
}
