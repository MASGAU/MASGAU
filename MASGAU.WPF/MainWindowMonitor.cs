using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        protected void setupMonitorIcon() {
            Core.monitor.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(notifier.monitor_PropertyChanged);
        }





    }
}
