
using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.IO;
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


                Window win;

                bool restore_mode = false;

                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 0) {
                    foreach (string arg in args) {
                        if (!arg.StartsWith("-") && (arg.EndsWith(Core.Extension) || arg.EndsWith(Core.Extension + "\""))) {
                            restore_mode = true;
                            break;
                        }
                    }
                }

                if (restore_mode)
                    win = new MASGAU.Restore.RestoreWindow();
                else
                    win = new MainWindowNew();

                   app.Run(win);

            } catch (Exception e) {
                Logger.Logger.log(e);
                System.Windows.MessageBox.Show("Error while trying to startup:\n"+ e.Message + @"Check the log in LocalAppData\MASGAU\logs\");
            }
        }


    }
}