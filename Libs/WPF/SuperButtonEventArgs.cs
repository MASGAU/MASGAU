using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMJ.WPF {
    public class SuperButtonEventArgs: EventArgs {
        public object SelectedItem { get; protected set; }
        public SuperButtonEventArgs(object selected) {
            SelectedItem = selected;
        }
    }
}
