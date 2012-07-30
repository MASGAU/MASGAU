
using System;
using System.Windows.Controls;
using MASGAU;
using MASGAU.WPF;
using MASGAU.Main;
namespace MASGAU {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            try {
                Application app = new MASGAU.Application();
                MainWindowNew win = new MainWindowNew();
                app.Run(win);
            } catch (Exception e) {
                Logger.Logger.log(e);
                System.Windows.MessageBox.Show("Error while trying to startup:\n"+ e.Message + @"Check the log in LocalAppData\MASGAU\logs\");
            }
        }
    }
}