using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IUtilMusic.View
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {

        private RadioButton[] rbs;
        private int currentRbs = 0;

        public HelpWindow()
        {
            InitializeComponent();

            System.Windows.Threading.DispatcherTimer aTimer = new System.Windows.Threading.DispatcherTimer();
            aTimer.Tick += new EventHandler(OnTimedEvent);
            aTimer.Interval = TimeSpan.FromSeconds(2);
            aTimer.Start();
            //aTimer.SynchronizingObject = this;

            rbs = new RadioButton[] { rbSwipe, rbOpen, rbPinch };
        }

        private void setCurrentImage()
        {
            ImageOpen.Visibility = (rbOpen.IsChecked.HasValue && rbOpen.IsChecked.Value) ? Visibility.Visible : Visibility.Hidden;
            ImageSwipe.Visibility = (rbSwipe.IsChecked.HasValue && rbSwipe.IsChecked.Value) ? Visibility.Visible : Visibility.Hidden;
            ImageVolume.Visibility = (rbPinch.IsChecked.HasValue && rbPinch.IsChecked.Value) ? Visibility.Visible : Visibility.Hidden;
        }

        private void checkedChange(object sender, RoutedEventArgs e)
        {
            setCurrentImage();
        }

        private void OnTimedEvent(object Sender, EventArgs e)
        {
            rbs[currentRbs].IsChecked = true;

            if (rbs.Count() -1 == currentRbs)
                currentRbs = 0;
            else
                currentRbs++;

        }
    }
}
