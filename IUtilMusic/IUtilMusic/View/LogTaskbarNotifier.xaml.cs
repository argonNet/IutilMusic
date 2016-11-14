using System;
using System.Collections.ObjectModel;

using WPFTaskbarNotifier;

//Widely inspired by Buzz  Weetman's own WPF Task Notifier:
//http://www.codeproject.com/Articles/22876/WPF-Taskbar-Notifier-A-WPF-Taskbar-Notification-Wi
namespace WPFTaskbarNotifierLog
{
    /// <summary>
    /// This is just a mock object to hold something of interest. 
    /// </summary>
    public class NotifyObject
    {
        public NotifyObject(string message)
        {
            this.message = message;
        }


        private string message;
        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }
    }

    /// <summary>
    /// This is a TaskbarNotifier that contains a list of NotifyObjects to be displayed.
    /// </summary>
    public partial class LogTaskbarNotifier : TaskbarNotifier
    {
        public LogTaskbarNotifier()
        {
            InitializeComponent();
        }

        private ObservableCollection<NotifyObject> notifyContent;
        /// <summary>
        /// A collection of NotifyObjects that the main window can add to.
        /// </summary>
        public ObservableCollection<NotifyObject> NotifyContent
        {
            get
            {
                if (this.notifyContent == null)
                {
                    // Not yet created.
                    // Create it.
                    this.NotifyContent = new ObservableCollection<NotifyObject>();
                }

                return this.notifyContent;
            }
            set
            {
                this.notifyContent = value;
            }
        }


        private void HideButton_Click(object sender, EventArgs e)
        {
            this.ForceHidden();
        }
    }
}