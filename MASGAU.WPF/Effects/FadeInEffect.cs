using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Effects {
    public class FadeInEffect: FadeEffect {
        public FadeInEffect(double time)
            : base(0.0, 1.0, time) {
        }
        public override void Start(System.Windows.FrameworkElement target) {
            switch (target.Visibility) {
                case System.Windows.Visibility.Collapsed:
                case System.Windows.Visibility.Hidden:
                    target.Opacity = 0.0;
                    target.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    this.from = target.Opacity;
                    break;
            }
            base.Start(target);
        }
    }
}
