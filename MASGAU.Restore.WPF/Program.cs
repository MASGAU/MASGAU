
using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Text;  
using System.Windows; 
using MASGAU;
namespace MASGAU.Restore
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
                Application app = new Application();  
                RestoreWindow win = new RestoreWindow();  
                app.Run(win);  

            

        }
    }
}