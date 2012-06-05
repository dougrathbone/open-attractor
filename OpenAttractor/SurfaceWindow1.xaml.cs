using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
using OpenAttractor.Helpers;
using OpenAttractor.Properties;
using Timer = System.Timers.Timer;

namespace OpenAttractor
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private Timer _clearScreenTimer;
        private DateTime _timeLastTouched = DateTime.MinValue;

        public double TimeSinceLastTouch { get { return (double)GetValue(_timeSinceLastTouch); } set { SetValue(_timeSinceLastTouch, value); } }
        public static readonly DependencyProperty _timeSinceLastTouch = DependencyProperty.Register("TimeSinceLastTouch", typeof(double), typeof(SurfaceWindow1), new FrameworkPropertyMetadata((double)0));

        public string BackgroundImagePath { get { return (string)GetValue(_backgroundImagePath); } set { SetValue(_backgroundImagePath, value); } }
        public static readonly DependencyProperty _backgroundImagePath = DependencyProperty.Register("BackgroundImagePath", typeof(string), typeof(SurfaceWindow1), new FrameworkPropertyMetadata(AppSettings.BackgroundPath));

        private List<VideoPlayer> _videoPlayers;

        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            Loaded += SurfaceWindow1_Loaded;
            PreviewTouchDown += delegate { FireClickEvent(); };
            PreviewMouseDown += delegate { FireClickEvent(); };
        }

        void FireClickEvent()
        {
            Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() =>
                            {
                                _timeLastTouched = DateTime.Now;
                                _throbObjects = false;
                            }));
        }

        void SurfaceWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            _timeLastTouched = DateTime.Now;
            _videoPlayers = new List<VideoPlayer>();

            InitialiseTouchTimer();

            TimeSinceLastTouchLabel.Visibility = Visibility.Collapsed;

            InitialiseScatterItems();
        }

        private void InitialiseScatterItems()
        {
            _videoPlayers.ForEach(x=>x.StopVideo());
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
                videoControl.OnVideoPlayerPlayed += videoControl_OnVideoPlayerPlayed;
                videoControl.OnVideoPlayerStopped += videoControl_VideoStopped;
                
                ScatterContainer.Items.Add(scatterView);
                _videoPlayers.Add(videoControl);
            }
        }

        void videoControl_OnVideoPlayerPlayed(object sender, EventArgs e)
        {
            var videoPlayersPlaying = _videoPlayers.Count(x => x.VideoIsPlaying);

            if (videoPlayersPlaying > AppSettings.MaximumVideosPlayingAtOnce)
            {
                var singleOrDefault = _videoPlayers.Where(x => x.VideoIsPlaying).OrderBy(x => x.PlayStarted).Take(1).SingleOrDefault();
                if (singleOrDefault != null)
                    singleOrDefault.StopVideo();
                Debug.WriteLine("Too many videos playing, stopping one");
            }
            Debug.WriteLine("Video player played");
            Debug.WriteLine("{0} videos currently playing", videoPlayersPlaying);
        }

        void videoControl_VideoStopped(object sender, EventArgs e)
        {
            Debug.WriteLine("Video player stopped");
            Debug.WriteLine("{0} videos currently playing", _videoPlayers.Count(x => x.VideoIsPlaying));
        }

        private bool _throbObjects = false;
        private void InitialiseTouchTimer()
        {
            _clearScreenTimer = new Timer { Interval = 2000 };
            _clearScreenTimer.Elapsed += clearScreenTimer_Elapsed;
            _clearScreenTimer.Enabled = true;
        }

        void clearScreenTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(
                    () =>
                    {
                        TimeSinceLastTouch = (DateTime.Now - _timeLastTouched).TotalSeconds;
                        if (TimeSinceLastTouch > AppSettings.ClearScreenTimerInterval)
                        {
                            _timeLastTouched = DateTime.Now;
                            InitialiseScatterItems();
                        }
                        if (TimeSinceLastTouch > AppSettings.ThrobTimerInterval && !_throbObjects)
                        {
                            _throbObjects = true;
                            StartThrobStoryboards();
                        }
                    }));
        }

        private int _nextObjectIndex = 0;
        private void StartThrobStoryboards()
        {
            
            AnimationHelper.RunScaleAnimation((FrameworkElement)ScatterContainer.Items[_nextObjectIndex]);
            _nextObjectIndex++;
            if (_nextObjectIndex >= ScatterContainer.Items.Count) _nextObjectIndex = 0;

            _throbObjects = false;
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _throbObjects = false;
            _clearScreenTimer.Enabled = false;

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