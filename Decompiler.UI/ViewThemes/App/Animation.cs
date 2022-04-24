using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Decompiler.UI.ViewResources.Helpers
{
    public class Animation
    {
        /// <summary>
        /// Plays a DoubleAnimation animation on the specified controls
        /// </summary>
        /// <param name="parentControl">The target controls parent</param>
        /// <param name="control">The name of the target</param>
        /// <param name="property">The property to change</param>
        /// <param name="value">The value to change the property to</param>
        /// <param name="timeSpan">The time it takes to change the property</param>
        public static void DoubleAnim(FrameworkElement parentControl, string control, DependencyProperty property, double value, int timeSpan = 1000)
        {
            DoubleAnimation anim = new();
            anim.To = value;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(timeSpan));

            Storyboard.SetTargetName(anim, control);
            Storyboard.SetTargetProperty(anim, new PropertyPath(property));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(anim);

            storyboard.Begin(parentControl);
        }

        /// <summary>
        /// Plays a Thickness animation on the specified controls
        /// </summary>
        /// <param name="parentControl">The target controls parent</param>
        /// <param name="control">The name of the target</param>
        /// <param name="property">The property to change</param>
        /// <param name="value">The value to change the property to</param>
        /// <param name="timeSpan">The time it takes to change the property</param>
        public static void ThicknessAnim(FrameworkElement parentControl, string control, DependencyProperty property, Thickness value, int timeSpan = 1000)
        {
            ThicknessAnimation anim = new();
            anim.To = value;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(timeSpan));

            Storyboard.SetTargetName(anim, control);
            Storyboard.SetTargetProperty(anim, new PropertyPath(property));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(anim);

            storyboard.Begin(parentControl);
        }
    }
}
