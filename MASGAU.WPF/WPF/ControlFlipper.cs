using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace MASGAU.WPF {
    public class ControlFlipper: List<FrameworkElement> {
        public ControlFlipper() {
        }

        public new void Add(FrameworkElement element) {
            element.Visibility = Visibility.Collapsed;
            base.Add(element);
        }

        public bool IsActiveControl(FrameworkElement element) {
            if (!this.Contains(element))
                throw new Exception("Framework element " + element.ToString() + " not found in control flipper");

            return element.Visibility == Visibility.Visible;
        }

        public void SwitchControl(FrameworkElement element) {
            if (!this.Contains(element))
                throw new Exception("Framework element " + element.ToString() + " not found in control flipper");
            foreach (FrameworkElement me in this) {
                if (me == element) {
                    me.Visibility = Visibility.Visible;
                } else {
                    me.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
