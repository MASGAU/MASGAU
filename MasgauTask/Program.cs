using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Masgau
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SecurityHandler red_shirt = new SecurityHandler();
            string[] args = Environment.GetCommandLineArgs();
            bool admin_status = red_shirt.amAdmin(), all_users_mode = false;;
            for(int i = 0;i<args.Length;i++) {
                if(args[i]=="/allusers") {
                    all_users_mode = true;
                }
            }

            if(all_users_mode) {
                if(!admin_status) {
                    red_shirt.elevation("/allusers");
                } else {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new taskForm());
                }
            } else {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new taskForm());
            }
        }
    }
}