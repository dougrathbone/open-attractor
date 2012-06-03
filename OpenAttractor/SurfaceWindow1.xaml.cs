using System;
using System.Configuration;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Controls;

namespace OpenAttractor
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private Timer clearScreenTimer;
        private DateTime timeLastTouched = DateTime.MinValue;

        public double TimeSinceLastTouch { get { return (double) GetValue(_timeSinceLastTouch); } set { SetValue(_timeSinceLastTouch, value); } }
        public static readonly DependencyProperty _timeSinceLastTouch = DependencyProperty.Register("TimeSinceLastTouch", typeof(double), typeof(SurfaceWindow1), new FrameworkPropertyMetadata((double)0));

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            Loaded += new RoutedEventHandler(SurfaceWindow1_Loaded);
            TouchDown +=new EventHandler<System.Windows.Input.TouchEventArgs>(
                delegate(object o, TouchEventArgs args)
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() => timeLastTouched = DateTime.Now));
                    });
        }

        void SurfaceWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            timeLastTouched = DateTime.Now;
            InitialiseTouchTimer();

            if (AppSettings.DebugEnabled)
            {
                InitialiseDebugTimer();
                TimeSinceLastTouchLabel.Visibility = Visibility.Visible;
            }

            InitialiseScatterItems();
        }

        private void InitialiseScatterItems()
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                               {
                                   ScatterContainer.Items.Clear();

                                   var images =
                                       Directory.GetFiles(
                                           ConfigurationManager.AppSettings[
                                               "PhotoAssetsPath"], "*.jpg");
                                   var videos =
                                       Directory.GetFiles(
                                           ConfigurationManager.AppSettings[
                                               "VideoAssetsPath"], "*.wmv");

                                   foreach (var imagePath in images)
                                   {
                                       var imageControl = new Image();
                                       var myBitmapImage = new BitmapImage();
                                       myBitmapImage.BeginInit();
                                       myBitmapImage.UriSource = new Uri(imagePath);
                                       myBitmapImage.EndInit();
                                       imageControl.Source = myBitmapImage;
                                       var scatterView = new ScatterViewItem {Content = imageControl};
                                       ScatterContainer.Items.Add(scatterView);
                                   }

                                   foreach (var videoPath in videos)
                                   {
                                       var videoControl = new VideoPlayer {Source = videoPath};
                                       var scatterView = new ScatterViewItem {Content = videoControl};
                                       ScatterContainer.Items.Add(scatterView);
                                   }
                               }));
        }

        private Timer labelTimer;
        private void InitialiseDebugTimer()
        {
            labelTimer = new Timer { Interval = 1000, AutoReset = true };
            labelTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(
                        () =>
                        TimeSinceLastTouch =
                        ((TimeSpan)(DateTime.Now - timeLastTouched)).TotalSeconds));
            };
            labelTimer.Enabled = true;
        }
        private void InitialiseTouchTimer()
        {
            clearScreenTimer = new Timer {Interval = 2000};
            clearScreenTimer.Elapsed += new ElapsedEventHandler(clearScreenTimer_Elapsed);
            clearScreenTimer.Enabled = true;
        }

        void clearScreenTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (((TimeSpan)(DateTime.Now - timeLastTouched)).TotalSeconds > AppSettings.ClearScreenTimerInterval)
            {
                timeLastTouched = DateTime.Now;
                InitialiseScatterItems();
            }
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            labelTimer.Enabled = false;
            clearScreenTimer.Enabled = false;

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

    }
}