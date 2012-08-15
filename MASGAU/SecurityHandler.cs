using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using System.Security.Principal;
using System.Text;
using MVC.Translator;
using MVC.Communication;
using MASGAU;
public class SecurityHandler {
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

    public static bool elevation(string app_name, string new_args, bool wait_for_exit) {
        StringBuilder arg_string = new StringBuilder();
        if (new_args != null)
            arg_string.Append(new_args);
        string[] args = Environment.GetCommandLineArgs();

        for (int j = 1; j < args.Length; j++) {
            if (args[j].Contains(" ")) {
                arg_string.Append(" \"" + args[j] + "\"");
            } else {
                arg_string.Append(" " + args[j]);
            }
        }
        if (!Core.settings.SuppressElevationWarnings) {
            ResponseType response = ResponseType.OK;
            switch (MASGAU.Core.locations.platform_version) {
                case "WindowsXP":
                    response = TranslatingMessageHandler.SendWarning("ElevationXPWarning", true);
                    break;
                case "WindowsVista":
                    break;
                default:
                    throw new NotSupportedException(MASGAU.Core.locations.platform_version);
            }
            if (response >= ResponseType.Suppressed)
                Core.settings.SuppressElevationWarnings = true;

        }

        return runExe(app_name, arg_string.ToString(), true, wait_for_exit);

    }

    public static bool runExe(string app_name, string arg_string, bool super, bool wait_for_exit) {

        ProcessStartInfo superMode = new ProcessStartInfo();
        superMode.UseShellExecute = true;
        superMode.WorkingDirectory = Environment.CurrentDirectory;

        if (app_name == null)
            superMode.FileName = System.AppDomain.CurrentDomain.FriendlyName;
        else
            superMode.FileName = app_name;

        superMode.Arguments = arg_string;

        if (super)
            superMode.Verb = "runas";

        try {
            Process p = Process.Start(superMode);
            if (wait_for_exit) {
                p.WaitForExit();
                if (p.ExitCode != 0) {
                    return false;
                }
            }
        } catch (Exception) {
            return false;
        }
        return true;
    }
}
