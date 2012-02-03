using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Principal;
using System.Diagnostics;

    public class SecurityHandler
    {
        [DllImport("user32")]
        public static extern UInt32 SendMessage
        (IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);
        internal const int BCM_FIRST = 0x1600; //Normal button
        internal const int BCM_SETSHIELD = (BCM_FIRST + 0x000C); //Elevated button

        public SecurityHandler() {
        }

        public bool amAdmin() {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);

        }

        public void addShield(Button button) {
            button.FlatStyle = FlatStyle.System;
            SendMessage(button.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
        }

        public bool elevation(string new_args) {
            ProcessStartInfo superMode = new ProcessStartInfo();
            superMode.UseShellExecute = true;
            superMode.WorkingDirectory = Environment.CurrentDirectory;
            superMode.FileName = Application.ExecutablePath;

            string[] args = Environment.GetCommandLineArgs();


            string arg_string = "";
            if (new_args != null)
                arg_string = new_args;

            for(int j = 1;j<args.Length;j++) {
                if(args[j].Contains(" ")) {
                    arg_string += " \"" + args[j] + "\"";
                } else {
                    arg_string += " " + args[j];
                }
            }
            superMode.Arguments = arg_string;
            superMode.Verb = "runas";
            try {
                Process p = Process.Start(superMode);
            }
            catch{
                return false;
            }
            return true;
        }
    }
