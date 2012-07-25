using System;
using System.Windows;

namespace MASGAU {
    public class Application : System.Windows.Application {
        string[] args = Environment.GetCommandLineArgs();
        bool all_users_mode = false;
        public Application() {
            Logger.Logger.AppName = "masgau";
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == "-allusers") {
                    all_users_mode = true;
                }
            }
            bool admin_status = SecurityHandler.amAdmin();

            if (all_users_mode) {
                if (!admin_status) {
                    SecurityHandler.elevation(Core.programs.main, null);
                    Environment.Exit(0);
                } else {
                }
            } else {
            }
            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

    }
}
