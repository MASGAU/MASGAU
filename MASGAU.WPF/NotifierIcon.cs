using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Translator;
namespace MASGAU.Main {
    class NotifierIcon {
        NotifyIcon icon;

        MainWindowNew parent;

        public bool Visible {
            get {
                return icon.Visible;
            }
            set {
                icon.Visible = value;
            }
        }
        public void Dispose() {
            icon.Dispose();
        }

        public bool MenuEnabled {
            get {
                return icon.ContextMenu != null;
            }
            set {
                if (value)
                    icon.ContextMenu = Menu;
                else
                    icon.ContextMenu = null;
            }
        }
        private System.Windows.Forms.ContextMenu Menu = new ContextMenu();
        MenuItem exit = new MenuItem();

        Timer timer = new Timer();
        private List<Icon> icons = new List<Icon>();
        public NotifierIcon(MainWindowNew parent) {
            this.parent = parent;
            icon = new NotifyIcon();
            string[] names = Assembly.GetEntryAssembly().GetManifestResourceNames();

            //icons.Add(Properties.Resources._0001);
            //icons.Add(Properties.Resources._0002);
            //icons.Add(Properties.Resources._0003);
            icons.Add(Properties.Resources._0004);
            icons.Add(Properties.Resources._0005);
            icons.Add(Properties.Resources._0006);
            icons.Add(Properties.Resources._0007);
            icons.Add(Properties.Resources._0008);
            icons.Add(Properties.Resources._0009);
            icons.Add(Properties.Resources._0010);
            icons.Add(Properties.Resources._0011);
            icons.Add(Properties.Resources._0012);
            icons.Add(Properties.Resources._0013);
            icons.Add(Properties.Resources._0014);
            icons.Add(Properties.Resources._0015);
            icons.Add(Properties.Resources._0016);
            icons.Add(Properties.Resources._0017);
            icons.Add(Properties.Resources._0018);
            icons.Add(Properties.Resources._0019);
            icons.Add(Properties.Resources._0020);
            icons.Add(Properties.Resources._0021);
            icons.Add(Properties.Resources._0022);
            icons.Add(Properties.Resources._0023);
            icons.Add(Properties.Resources._0024);
            icons.Add(Properties.Resources._0025);
            icons.Add(Properties.Resources._0035);
            icons.Add(Properties.Resources._0036);
            icons.Add(Properties.Resources._0037);
            icons.Add(Properties.Resources._0038);
            icons.Add(Properties.Resources._0039);
            icons.Add(Properties.Resources._0040);
            icons.Add(Properties.Resources._0041);
            icons.Add(Properties.Resources._0042);
            icons.Add(Properties.Resources._0043);
            icons.Add(Properties.Resources._0044);
            icons.Add(Properties.Resources._0045);
            icons.Add(Properties.Resources._0046);
            icons.Add(Properties.Resources._0047);
            icons.Add(Properties.Resources._0048);
            icons.Add(Properties.Resources._0049);
            icons.Add(Properties.Resources._0050);
            icons.Add(Properties.Resources._0051);
            icons.Add(Properties.Resources._0052);
            icons.Add(Properties.Resources._0053);
            icons.Add(Properties.Resources._0054);
            icons.Add(Properties.Resources._0055);
            icons.Add(Properties.Resources._0056);
            icons.Add(Properties.Resources._0057);
            //icons.Add(Properties.Resources._0058);
            //icons.Add(Properties.Resources._0059);
            //icons.Add(Properties.Resources._0060);
            timer.Interval = 33;
            timer.Tick += new EventHandler(timer_Tick);

            icon.Icon = Properties.Resources._0001;

            exit.Text = Strings.GetLabelString("ExitMASGAU");
            exit.Click += new EventHandler(exit_Click);

            Menu.MenuItems.Add(exit);

            icon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            icon.Visible = true;
        }

        public bool Animated {
            set {
                if (value) {
                    timer.Start();
                } else {
                    timer.Stop();
                    icon.Icon = Properties.Resources._0001;
                }
            }
        }

        int frame = 0;
        void timer_Tick(object sender, EventArgs e) {
            if (frame >= icons.Count)
                frame = 0;
            icon.Icon = icons[frame];
            //            System.Console.Out.Write(frame.ToString() + ".");
            frame++;
        }




        private string last_message = null;

        public void sendBalloon(string message) {
            if (message != last_message)
                icon.ShowBalloonTip(5, "MASGAU", message, ToolTipIcon.Info);
            message = last_message;
        }


        public void monitor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (icon == null)
                return;
            switch (e.PropertyName) {
                case "Active":
                    //notifyIcon.Visible = Core.monitor.Active;
                    break;
                case "Status":
                    if (Common.Monitor.Status.Length > 63)
                        icon.Text = Common.Monitor.Status.Substring(0, 63);
                    else
                        icon.Text = Common.Monitor.Status;
                    break;
            }
        }
        void exit_Click(object sender, EventArgs e) {
            parent.Close();
        }
        public bool OpenCloseEnabled = true;

        void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            switch (e.Button) {
                case MouseButtons.Middle:
                case MouseButtons.Left:

                    if (OpenCloseEnabled)
                        parent.ShowInTaskbar = parent.toggleVisibility();

                    parent.updateWindowState();
                    break;
            }
        }

    }
}
