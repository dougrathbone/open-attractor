using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

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

		private void SurfaceButton_Click(object sender, RoutedEventArgs e)
		{
			videoPlayer.MediaEnded += delegate(object o, RoutedEventArgs args)
			                          	{
			                          		videoPlayer.Position = new TimeSpan(0,0,0,0);
											videoPlayer.Play();
			                          	};
			videoPlayer.Play();
			PlayButton.Visibility = Visibility.Hidden;
			Panel.SetZIndex(PlayButton,-1);

			PauseButton.Visibility = Visibility.Visible;
			RewindButton.Visibility = Visibility.Visible;
		}

		private void RewindButton_Click(object sender, RoutedEventArgs e)
		{
			videoPlayer.Position = new TimeSpan(0,0,0,0);
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			videoPlayer.Pause();

			((SurfaceButton)sender).Visibility = Visibility.Collapsed;
			PlayButtonSmall.Visibility = Visibility.Visible;
		}

		private void videoPlayer_Loaded(object sender, RoutedEventArgs e)
		{
			((MediaElement)sender).Play();
			((MediaElement)sender).Pause();
		}

		private void PlayButtonSmall_Click(object sender, RoutedEventArgs e)
		{
			videoPlayer.Play();

			((SurfaceButton)sender).Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Visible;
		}
    }
}