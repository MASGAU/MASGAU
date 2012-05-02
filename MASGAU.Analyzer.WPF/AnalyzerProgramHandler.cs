using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using MASGAU.Registry;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Communication.Progress;
using Translations;
namespace MASGAU.Analyzer {
    public class AnalyzerProgramHandler: AAnalyzerProgramHandler<Location.LocationsHandler> {
        public AnalyzerProgramHandler():base(MASGAU.Interface.WPF)  {
        }

        protected override void HandleAnalyzerDoWork(object sender, DoWorkEventArgs e) {
            ProgressHandler.progress_max = 6;
            if (gamePath != null)
            {
                try {
                    searchRegistry();
                }
                catch (Exception ex) {
                    output.AppendLine("Error while attempting to search the registry:");
                    recordException(ex);
                }

                try {
                    searchStartMenu();
                }
                catch (Exception ex) {
                    output.AppendLine("Error while attempting to search the start menu:");
                    recordException(ex);
                }
            }
            base.HandleAnalyzerDoWork(sender, e);
        }

        protected override void parseInstallFolder() {
            base.parseInstallFolder();

            
            ProgressHandler.progress++;
            ProgressHandler.progress_message = Strings.get("DumpingVirtualStoreFolder");
            if (Core.locations.uac_enabled) {
                output.AppendLine(Environment.NewLine + "UAC Enabled" + Environment.NewLine);
                output.AppendLine(Environment.NewLine + "VirtualStore Folders: ");
                string virtual_path;

                foreach (string user in Core.locations.getUsers(EnvironmentVariable.LocalAppData)) {
                    LocationPathHolder parse_me = new LocationPathHolder();
                    parse_me.path = "VirtualStore";
                    parse_me.rel_root = EnvironmentVariable.LocalAppData;
                    virtual_path = Path.Combine(Core.locations.getAbsoluteRoot(parse_me, user), "VirtualStore");

                    analyzer.ReportProgress(6, "VirtualStore for user " + user + ": " + virtual_path);
                    virtual_path = Path.Combine(virtual_path, gamePath.Substring(3));
                    if (Directory.Exists(virtual_path))
                        travelSaveFolder(virtual_path);

                }
            }
            else {
                output.AppendLine(Environment.NewLine + "UAC Disabled or not present" + Environment.NewLine);
            }
        }




        private void searchRegistry() {

            if (analyzer.CancellationPending)
                return;

            ProgressHandler.progress++;
            ProgressHandler.progress_message = Strings.get("ScanningLocalMachineRegistry");
            output.AppendLine("Local Machine Registry Entries: ");
            RegistryKey look_here = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
            registryTraveller(look_here);

            if (analyzer.CancellationPending)
                return;
            output.AppendLine("Current User Registry Entries: ");
            ProgressHandler.progress++;
            ProgressHandler.progress_message = Strings.get("ScanningCurrentUserRegistry");
            look_here = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software");
            registryTraveller(look_here);
        }
        private void registryTraveller(RegistryKey look_here) {
            if (analyzer.CancellationPending)
                return;
            foreach (string check_me in look_here.GetValueNames()) {
                RegistryKeyValue value = new RegistryKeyValue();
                try {
                    value.key = look_here.Name;
                    value.value = check_me;
                    if (look_here.GetValue(check_me) != null) {
                        value.data = look_here.GetValue(check_me).ToString();
                        if (value.data.Length >= gamePath.Length && gamePath.ToLower() == value.data.Substring(0, gamePath.Length).ToLower())
                        {
                            output.AppendLine(Environment.NewLine + "Key:" + value.key);
                            output.AppendLine("Value: " + value.value);
                            output.Append("Data: ");
                            outputFileSystemPath(value.data);
                        }
                    }
                }
                catch { }
            }

            try {
                RegistryKey sub_key;
                foreach (string check_me in look_here.GetSubKeyNames()) {
                    try {
                        sub_key = look_here.OpenSubKey(check_me);
                        if (sub_key != null)
                            registryTraveller(sub_key);
                    }
                    catch (System.Security.SecurityException) { }
                }
            }
            catch { }
        }

        private void searchStartMenu() {

            if (analyzer.CancellationPending)
                return;
            ProgressHandler.progress++;
            ProgressHandler.progress_message = Strings.get("ScanningStartMenu");
            output.AppendLine(Environment.NewLine + "Detected Start Menu Shortcuts: ");

            string start_menu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            if (start_menu != null && Directory.Exists(start_menu))
                startMenuTraveller(start_menu);
            else
                output.AppendLine("Folder for Start Menu (" + start_menu + ") not found");

            start_menu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            if (start_menu != null && Directory.Exists(start_menu))
                startMenuTraveller(start_menu);
            else
                output.AppendLine("Folder for global Start Menu (" + start_menu + ") not found");
        }

        private void startMenuTraveller(string look_here) {
            if (analyzer.CancellationPending)
                return;

            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut link;

            try {
                foreach (FileInfo shortcut in new DirectoryInfo(look_here).GetFiles("*.lnk")) {
                    try {
                        link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcut.FullName);
                        if (link.TargetPath.Length >= gamePath.Length && gamePath.ToLower() == link.TargetPath.Substring(0, gamePath.Length).ToLower())
                        {
                            this.outputFile(shortcut.FullName);
                            this.outputFileSystemPath(link.TargetPath);
                        }
                    }
                    catch (Exception e) {
                        output.AppendLine(e.Message);
                    }
                }
            }
            catch (FileNotFoundException e) {
            }
            catch (UnauthorizedAccessException e) {
            }
            catch (Exception e) {
                output.AppendLine(e.Message);
            }

            try {
                foreach (DirectoryInfo now_here in new DirectoryInfo(look_here).GetDirectories()) {
                    try {
                        startMenuTraveller(now_here.FullName);
                    }
                    catch (Exception e) {
                        this.output.AppendLine(e.Message);
                    }
                }
            }
            catch (DirectoryNotFoundException) {
            }
            catch (UnauthorizedAccessException) {
            }
            catch (Exception e) {
                output.AppendLine(e.Message);
            }

        }

    }
}
