using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using System.Security.Principal;
using System.Text;
    public class SecurityHandler
    {
        [DllImport("user32")]
        public static extern UInt32 SendMessage
        (IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);
        internal const int BCM_FIRST = 0x1600; //Normal button
        internal const int BCM_SETSHIELD = (BCM_FIRST + 0x000C); //Elevated button

        public static bool amAdmin() {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);

        }

        //public static void addShield(Button button) {
        //    button.FlatStyle = FlatStyle.System;
        //    SendMessage(button.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
        //}

        public static bool elevation(string app_name, string new_args) {
            ProcessStartInfo superMode = new ProcessStartInfo();
            superMode.UseShellExecute = true;
            superMode.WorkingDirectory = Environment.CurrentDirectory;

            if(app_name==null)
                superMode.FileName = System.AppDomain.CurrentDomain.FriendlyName;
            else
                superMode.FileName = app_name;


            string[] args = Environment.GetCommandLineArgs();


            StringBuilder arg_string = new StringBuilder();
            if (new_args != null)
                arg_string.Append(new_args);

            for(int j = 1;j<args.Length;j++) {
                if(args[j].Contains(" ")) {
                    arg_string.Append(" \"" + args[j] + "\"");
                } else {
                    arg_string.Append(" " + args[j]);
                }
            }

            superMode.Arguments = arg_string.ToString();

            //if(!amAdmin())
                superMode.Verb = "runas";

            try {
                Process p = Process.Start(superMode);
                p.WaitForExit();
                if(p.ExitCode!=0) {
                    return false;
                }
            }
            catch (Exception e){
                return false;
            }
            return true;
        }
    }
