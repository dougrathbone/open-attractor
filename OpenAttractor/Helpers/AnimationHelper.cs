using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Surface.Presentation.Controls;

namespace OpenAttractor.Helpers
{
    public class AnimationHelper
    {
        public static void RunScaleAnimation(FrameworkElement e)
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

			//remove the events after completed to ensure that the ScatterViewItem is resizable again
			growAnimationWOut.Completed += delegate { e.BeginAnimation(FrameworkElement.WidthProperty, null); };
        	growAnimationHOut.Completed += delegate { e.BeginAnimation(FrameworkElement.HeightProperty, null); };

            Storyboard.SetTargetProperty(growAnimationWOut, new PropertyPath(FrameworkElement.WidthProperty));
            Storyboard.SetTargetProperty(growAnimationHOut, new PropertyPath(FrameworkElement.HeightProperty));

            e.BeginStoryboard(storyboard);
        }
    }
}
