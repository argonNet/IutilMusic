using System;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Leap;

using IUtilMusic.Keyboard;
using IUtilMusic.LeapMotion;
using WPFTaskbarNotifierExample;

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
        private ExampleTaskbarNotifier _taskbarNotifier;
        #endregion

        #region Methods
        
        /// <summary>
        /// Create the Notify Icon and register its events
        /// </summary>
        private void InitSysTrayIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowConfigurationWindow();
            _notifyIcon.Icon = IUtilMusic.Properties.Resources.MyIcon;
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
                controller.Connect += leapMotionListener.OnServiceConnect;
                controller.Device += leapMotionListener.OnConnect;
                controller.FrameReady += leapMotionListener.OnFrame;
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
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        /// <summary>
        /// Close application and clear memory
        /// </summary>
        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }  
        #endregion

        /// <summary>
        /// Show a ballon notification
        /// </summary>
        /// <param name="title">Title of notification</param>
        /// <param name="body">Body of the notification</param>
        private void ShowBalloon(string message)
        {
            _taskbarNotifier.NotifyContent.Clear();
            //if (title != null)
            //{
            //    _notifyIcon.BalloonTipTitle = title;
            //}

            //if (body != null)
            //{
            //    _notifyIcon.BalloonTipText = body;
            //}

            //_notifyIcon.ShowBalloonTip(100);

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
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
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
        /// Show information message concerning keyboard to end-user
        /// TODO: Make something cleaner !
        /// </summary>
        /// <param name="source">Instance of KeyboardListener</param>
        /// <param name="e">Custom Arg for the keyboard</param>
        private void ShowKeyboardMessage(object sender, KeyboardCustomEvent.KeyboardArgs e)
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
                case 5:
                    gestureName = "open hand";
                    break;
                default:
                    throw new NotImplementedException("Unknown gesture.");
            }
            ShowBalloon(String.Format("Gesture {0} executed ({1})", gestureName, infosList[1]));
        }
        #endregion

        #region Override Protected
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new ConfigurationWindow();
            MainWindow.Closing += MainWindow_Closing;

            InitSysTrayIcon();
            _taskbarNotifier = new ExampleTaskbarNotifier();
            _taskbarNotifier.Show();
            KeyboardListener keyListener = InitKeyboardListener();
            InitLeapMotionController(keyListener);
        }
        #endregion
        #endregion
    }
}
