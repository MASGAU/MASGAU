using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Reflection;
namespace MASGAU.Effects {
    public class FadeEffect : AEffect {

        protected double from;
        protected double to;
        protected double speed;

        public FadeEffect(double from, double to, double speed) {
            this.from = from;
            this.to = to;
            this.speed = speed;
        }

        protected override Storyboard CreateStoryboard(FrameworkElement target) {
            Storyboard result = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.SpeedRatio = speed;

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));

            result.Children.Add(animation);

            return result;
        }
    }
}
