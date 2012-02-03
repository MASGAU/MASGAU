using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MASGAU
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
            for(int i = 0;i<args.Length;i++) {
                if(args[i]=="/allusers") {
                    if(!red_shirt.amAdmin()) {
                        red_shirt.elevation(null);
                        return;
                    }
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new masgauForm());
        }
    }
}