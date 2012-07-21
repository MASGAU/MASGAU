using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Effects {
    public class FadeOutEffect: FadeEffect {
        public FadeOutEffect(double time)
            : base(1.0, 0.0, time) {
        }
        public override void Start(System.Windows.FrameworkElement target) {
            this.from = target.Opacity;
            base.Start(target);
        }

        protected override void OnCompleted(object sender, EventArgs e) {
            target.Visibility = System.Windows.Visibility.Collapsed;
            base.OnCompleted(sender, e);
        }
    }
}
