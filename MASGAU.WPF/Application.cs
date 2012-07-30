using System;
using System.Windows;

namespace MASGAU {
    public class Application : System.Windows.Application {
        string[] args = Environment.GetCommandLineArgs();
        bool all_users_mode = false;
        public Application() {
            Logger.Logger.AppName = "MASGAU";
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
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
            Uri uri = new Uri("PresentationFramework.Aero;V3.0.0.0;31bf3856ad364e35;component\\themes/aero.normalcolor.xaml", UriKind.Relative);

            Resources.MergedDictionaries.Add(Application.LoadComponent(uri) as ResourceDictionary);

            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            Logger.Logger.log("Error on program startup");
            Logger.Logger.log(e.Exception);
            MessageBox.Show(e.Exception.StackTrace);
        }

        protected override void OnStartup(StartupEventArgs e) {
            // hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            base.OnStartup(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            // put your tracing or logging code here (I put a message box as an example)
            Logger.Logger.log("Error on program startup");
            Logger.Logger.log(e.ExceptionObject as Exception);
            MessageBox.Show((e.ExceptionObject as Exception).StackTrace);
        }
    }
}
