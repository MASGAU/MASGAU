
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
         // d   try {

                Application app = new MASGAU.Application();


                Window win;

                switch (Core.AppMode) {
                    case AppMode.Main:
                        win = new MainWindowNew();
                        break;
                    case AppMode.Restore:
                        win = new MASGAU.Restore.RestoreWindow();
                        break;
                    default:
                        throw new NotSupportedException(Core.AppMode.ToString());
                }

                app.Run(win);

//            } catch (Exception e) {
  //              Logger.Logger.log(e);
    //            System.Windows.MessageBox.Show("Error while trying to startup:\n"+ e.Message + @"Check the log in LocalAppData\MASGAU\logs\");
      //      }
        }


    }
}