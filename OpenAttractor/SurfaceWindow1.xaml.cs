using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Controls;
using OpenAttractor.Properties;
using Timer = System.Timers.Timer;

namespace OpenAttractor
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private Timer clearScreenTimer;
        private DateTime timeLastTouched = DateTime.MinValue;

        public double TimeSinceLastTouch { get { return (double)GetValue(_timeSinceLastTouch); } set { SetValue(_timeSinceLastTouch, value); } }
        public static readonly DependencyProperty _timeSinceLastTouch = DependencyProperty.Register("TimeSinceLastTouch", typeof(double), typeof(SurfaceWindow1), new FrameworkPropertyMetadata((double)0));

        public string BackgroundImagePath { get { return (string)GetValue(_backgroundImagePath); } set { SetValue(_backgroundImagePath, value); } }
        public static readonly DependencyProperty _backgroundImagePath = DependencyProperty.Register("BackgroundImagePath", typeof(string), typeof(SurfaceWindow1), new FrameworkPropertyMetadata(AppSettings.BackgroundPath));


        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            Loaded += new RoutedEventHandler(SurfaceWindow1_Loaded);
            PreviewTouchDown += delegate(object sender, TouchEventArgs args) { FireClickEvent(); };
            PreviewMouseDown += delegate(object sender, MouseButtonEventArgs args) { FireClickEvent(); };
        }

        void FireClickEvent()
        {
            Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() =>
                            {
                                timeLastTouched = DateTime.Now;
                                throbObjects = false;
                            }));
        }

        void SurfaceWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            timeLastTouched = DateTime.Now;
            InitialiseTouchTimer();

            TimeSinceLastTouchLabel.Visibility = Visibility.Collapsed;

            InitialiseScatterItems();
        }

        private void InitialiseScatterItems()
        {

            ScatterContainer.Items.Clear();

            var images =
                Directory.GetFiles(ConfigurationManager.AppSettings["PhotoAssetsPath"], "*.*", SearchOption.AllDirectories)
                         .Where(s => s.EndsWith(".jpg")
                             || s.EndsWith(".jpeg")
                             || s.EndsWith(".png")
                             || s.EndsWith(".gif"));

            var videos =
                Directory.GetFiles(
                    ConfigurationManager.AppSettings[
                        "VideoAssetsPath"], "*.wmv");

            foreach (var imagePath in images)
            {
                var imageControl = new ImageViewer();
                var myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(imagePath);
                myBitmapImage.EndInit();
                imageControl.Source = myBitmapImage;
                var scatterView = new ScatterViewItem { Content = imageControl, MaxWidth = AppSettings.MaximumAssetWidth };

                ScatterContainer.Items.Add(scatterView);
            }

            foreach (var videoPath in videos)
            {
                var videoControl = new VideoPlayer { Source = videoPath };
                var scatterView = new ScatterViewItem { Content = videoControl, MaxWidth = AppSettings.MaximumAssetWidth };
                ScatterContainer.Items.Add(scatterView);
            }
        }

        void RunScaleAnimation(FrameworkElement e)
        {

            var storyboard = new Storyboard();
            var easeOut = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 };

            var startHeight = e.ActualHeight;
            var startWidth = e.ActualWidth;

            var growAnimationHOut = new DoubleAnimation(startHeight, startHeight * 1.05,
                                                        TimeSpan.FromMilliseconds(70)) { AutoReverse = true };

            var growAnimationWOut = new DoubleAnimation(startWidth, startWidth * 1.05,
                                                        TimeSpan.FromMilliseconds(70)) { AutoReverse = true };

            growAnimationHOut.EasingFunction = easeOut;
            growAnimationWOut.EasingFunction = easeOut;

            storyboard.Children.Add(growAnimationHOut);
            storyboard.Children.Add(growAnimationWOut);

            Storyboard.SetTargetProperty(growAnimationWOut, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(growAnimationHOut, new PropertyPath(HeightProperty));

            e.BeginStoryboard(storyboard);
        }

        private bool throbObjects = false;
        private void InitialiseTouchTimer()
        {
            clearScreenTimer = new Timer { Interval = 2000 };
            clearScreenTimer.Elapsed += clearScreenTimer_Elapsed;
            clearScreenTimer.Enabled = true;
        }

        void clearScreenTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(
                    () =>
                    {
                        TimeSinceLastTouch = ((TimeSpan)(DateTime.Now - timeLastTouched)).TotalSeconds;
                        if (TimeSinceLastTouch > AppSettings.ClearScreenTimerInterval)
                        {
                            timeLastTouched = DateTime.Now;
                            InitialiseScatterItems();
                        }
                        if (TimeSinceLastTouch > AppSettings.ThrobTimerInterval && !throbObjects)
                        {
                            throbObjects = true;
                            StartThrobStoryboards();
                        }
                    }));
        }

        private int nextObject = 0;
        private void StartThrobStoryboards()
        {
            
            RunScaleAnimation((FrameworkElement)ScatterContainer.Items[nextObject]);
            nextObject++;
            if (nextObject >= ScatterContainer.Items.Count) nextObject = 0;

            throbObjects = false;
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            throbObjects = false;
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