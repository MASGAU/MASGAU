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
            if(!red_shirt.amAdmin()) {
                red_shirt.elevation("/allusers");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new taskForm());
        }
    }
}