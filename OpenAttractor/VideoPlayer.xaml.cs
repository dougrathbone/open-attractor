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
using Microsoft.Surface.Presentation.Controls;

namespace OpenAttractor
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        public string Source { get { return (string)GetValue(_sourceProperty); } set { SetValue(_sourceProperty, value); } }
        public static readonly DependencyProperty _sourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(VideoPlayer), new FrameworkPropertyMetadata(String.Empty));

        public VideoPlayer()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(VideoPlayer_Loaded);
        }

        void VideoPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            videoPlayer.MediaEnded += delegate(object o, RoutedEventArgs args)
            {
                videoPlayer.Position = new TimeSpan(0, 0, 0, 0);
                videoPlayer.Play();
            };
            videoPlayer.Play();
            videoPlayer.Position = new TimeSpan(0, 0, 0, 1);
            videoPlayer.Stop();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Play();
            PlayButton.Visibility = Visibility.Hidden;
			Overlay.Visibility = Visibility.Hidden;

            PauseButton.Visibility = Visibility.Visible;
            RewindButton.Visibility = Visibility.Visible;
			PlayButtonSmall.Visibility = Visibility.Hidden;
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Position = new TimeSpan(0, 0, 0, 0);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Pause();

            ((SurfaceButton)sender).Visibility = Visibility.Collapsed;
            PlayButtonSmall.Visibility = Visibility.Visible;
			Overlay.Visibility = Visibility.Visible;
			PlayButton.Visibility = Visibility.Visible;
        }

        private void videoPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Play();
			((MediaElement)sender).Position = new TimeSpan(0, 0, 0, 1);
            ((MediaElement)sender).Pause();
        }

        private void PlayButtonSmall_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Play();

            ((SurfaceButton)sender).Visibility = Visibility.Collapsed;

            Overlay.Visibility = Visibility.Hidden;
			PlayButton.Visibility = Visibility.Hidden;
            PauseButton.Visibility = Visibility.Visible;
        }
    }
}
