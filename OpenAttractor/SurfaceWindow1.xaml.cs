using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Controls;

namespace OpenAttractor
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            Loaded += new RoutedEventHandler(SurfaceWindow1_Loaded);
        }

        void SurfaceWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            var images = Directory.GetFiles(ConfigurationManager.AppSettings["PhotoAssetsPath"], "*.jpg");
            var videos = Directory.GetFiles(ConfigurationManager.AppSettings["VideoAssetsPath"], "*.wmv");

            foreach (var imagePath in images)
            {
                var imageControl = new Image();
                var myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(imagePath);
                myBitmapImage.EndInit();
                imageControl.Source = myBitmapImage;
                var scatterView = new ScatterViewItem { Content = imageControl };
                ScatterContainer.Items.Add(scatterView);
            }

            foreach (var videoPath in videos)
            {
                var videoControl = new VideoPlayer { Source = videoPath };
                var scatterView = new ScatterViewItem { Content = videoControl };
                ScatterContainer.Items.Add(scatterView);
            }
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

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