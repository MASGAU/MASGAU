using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Translator;
namespace MASGAU.Main {
    class NotifierIcon  {
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


        public NotifierIcon(MainWindowNew parent) {
            this.parent = parent;
            icon = new NotifyIcon();
            icon.Icon = new System.Drawing.Icon("masgau.ico");
            exit.Text = Strings.GetLabelString("ExitMASGAU");
            exit.Click += new EventHandler(exit_Click);

            Menu.MenuItems.Add(exit);
            
            icon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            icon.Visible = true;
        }



        private string last_message = null;

        public void sendBalloon(string message) {
            if(message!=last_message)
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
                    if (Core.monitor.Status.Length > 63)
                        icon.Text = Core.monitor.Status.Substring(0, 63);
                    else
                        icon.Text = Core.monitor.Status;
                    break;
            }
        }
        void exit_Click(object sender, EventArgs e) {
            parent.Close();
        }
        void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            switch(e.Button) {
                case MouseButtons.Middle:
                case MouseButtons.Left:


                    //if (parent.IsVisible && !parent.Topmost) {
                      //  parent.Activate();
                    //} else {
                        parent.ShowInTaskbar = parent.toggleVisibility();
                    //}
                    parent.updateWindowState();
                    break;
            }
        }

    }
}
