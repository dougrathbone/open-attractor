using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;

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

            Storyboard.SetTargetProperty(growAnimationWOut, new PropertyPath(FrameworkElement.WidthProperty));
            Storyboard.SetTargetProperty(growAnimationHOut, new PropertyPath(FrameworkElement.HeightProperty));

            e.BeginStoryboard(storyboard);
        }
    }
}
