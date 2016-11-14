using System;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Leap;

using IUtilMusic.Keyboard;
using IUtilMusic.LeapMotion;

using WPFTaskbarNotifierLog;
using System.Windows.Threading;

namespace IUtilMusic
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        #region Consts
        private const string DISCONNECTED = "Disconnected";
        private const string CONNECTED = "Connected";
        private const string LM_STATUT = "Leap Motion Controller Statut";
        #endregion

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
        /// <summary>
        /// Timer for displaying the LM device not connected image when needed
        /// </summary>
        private DispatcherTimer _imageDeviceNotConnectedStayOpenTimer;
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
            _notifyIcon.Text = String.Format("{0}: {1}", LM_STATUT, DISCONNECTED);
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
            _notifyIcon.ContextMenuStrip.Items.Add(String.Format("{0}: {1}", LM_STATUT, DISCONNECTED));
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
            if (!_taskbarNotifier.IsActive) _taskbarNotifier.Show(); //If notifier hasn't been shown, init the display of it

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
            ShowBalloon(e.Message);
        }

        /// <summary>
        /// Display disconnect image to end-user depending of the connection's state of the Leap Motion device
        /// </summary>
        /// <param name="source">Instance of LeapMotionListener</param>
        /// <param name="e">Custom arg for Leap Motion Device</param>
        private void IsLeapMotionDeviceConnected(object sender, LeapMotionCustomEvents.LeapMotionDeviceConnectionArgs e)
        {
            if (e.IsDeviceConnected)
            {
                _leapMotionDeviceNotConnectedImage.Visibility = Visibility.Hidden;
                _notifyIcon.ContextMenuStrip.Items[0].Text = String.Format("{0}: {1}", LM_STATUT, CONNECTED);
                _notifyIcon.Text = String.Format("{0}: {1}", LM_STATUT, CONNECTED);
            }
            else
            {
                _leapMotionDeviceNotConnectedImage.Visibility = Visibility.Visible;
                _notifyIcon.ContextMenuStrip.Items[0].Text = String.Format("{0}: {1}", LM_STATUT, DISCONNECTED);
                _notifyIcon.Text = String.Format("{0}: {1}", LM_STATUT, DISCONNECTED);
            }
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
        /// Start dispatcher timer when image is displayed
        /// </summary>
        /// <param name="sender">Instance of the image's window</param>
        /// <param name="e">Dependency property changed args</param>
        private void LMDeviceNotConnectedImageWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                this._imageDeviceNotConnectedStayOpenTimer = new DispatcherTimer();
                this._imageDeviceNotConnectedStayOpenTimer.Interval = TimeSpan.FromSeconds(3);
                this._imageDeviceNotConnectedStayOpenTimer.Tick += stayOpenTimer_Elapsed;
                this._imageDeviceNotConnectedStayOpenTimer.Start();
            }
        }

        /// <summary>
        /// Event dispatched each time the dispatcher timer hit its interval
        /// Stop the timer and hide the image
        /// </summary>
        /// <param name="sender">Instance of disptacher timer</param>
        /// <param name="e">Args</param>
        private void stayOpenTimer_Elapsed(object sender, EventArgs e)
        {
            // Stop the timer because this should not be an ongoing event.
            this._imageDeviceNotConnectedStayOpenTimer.Stop();
            _leapMotionDeviceNotConnectedImage.Visibility = Visibility.Hidden;
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
            _imageDeviceNotConnectedStayOpenTimer = null;
            _leapMotionDeviceNotConnectedImage = new LMDeviceNotConnectedImage();
            _leapMotionDeviceNotConnectedImage.Closing += LMDeviceNotConnectedImageWindow_Closing;
            _leapMotionDeviceNotConnectedImage.IsVisibleChanged += LMDeviceNotConnectedImageWindow_IsVisibleChanged;
            _leapMotionDeviceNotConnectedImage.Show();


            _configWindow = new ConfigurationWindow();
            _configWindow.Closing += MainWindow_Closing;

            InitSysTrayIcon();
            _taskbarNotifier = new LogTaskbarNotifier();

            KeyboardListener keyListener = InitKeyboardListener();
            InitLeapMotionController(keyListener);
        }
        #endregion
        #endregion
    }
}
