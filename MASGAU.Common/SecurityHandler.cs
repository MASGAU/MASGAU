using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using MASGAU;
using MVC.Communication;
using MVC.Translator;
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

    public static ElevationResult elevation(string app_name, string new_args, bool wait_for_exit) {
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
        if (!Common.Settings.SuppressElevationWarnings) {
            ResponseType response = ResponseType.OK;
            if (Environment.OSVersion.Version < new Version(6, 0)) {
                response = TranslatingMessageHandler.SendWarning("ElevationXPWarning", true);
            }
            if (response >= ResponseType.Suppressed)
                Common.Settings.SuppressElevationWarnings = true;

        }

        return runExe(app_name, arg_string.ToString(), true, wait_for_exit);

    }

    public static ElevationResult runExe(string app_name, string arg_string, bool super, bool wait_for_exit) {

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
                    return ElevationResult.Failed;
                }
            }
        } catch (Win32Exception e) {
            switch (e.NativeErrorCode) {
                case 1223:
                    return ElevationResult.Cancelled;
                default:
                    return ElevationResult.Failed;
            }
        } catch (Exception) {
            return ElevationResult.Failed;
        }
        return ElevationResult.Success;
    }
}
