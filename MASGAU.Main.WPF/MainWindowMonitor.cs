using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        protected void setupMonitorIcon() {
            Core.monitor.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(monitor_PropertyChanged);
        }

        void exit_Click(object sender, EventArgs e) {
            this.Close();
        }
        void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {                
                this.ShowInTaskbar = toggleVisibility();
                updateWindowState();
            }
        }


        protected void sendBalloon(string message) {
            notifyIcon.ShowBalloonTip(5,"MASGAU",message, ToolTipIcon.Info);
        }

        void monitor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (notifyIcon == null)
                return;
            switch (e.PropertyName) {
                case "Active":
                    //notifyIcon.Visible = Core.monitor.Active;
                    break;
                case "Status":
                    notifyIcon.Text = Core.monitor.Status;
                    break;
            }
        }

    }
}
