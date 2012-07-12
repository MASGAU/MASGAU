
using System;
namespace MASGAU.Restore {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application app = new Application();
            RestoreWindow win = new RestoreWindow();
            app.Run(win);



        }
    }
}