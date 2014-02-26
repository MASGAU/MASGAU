using System;
using System.ComponentModel;
using System.IO;
using GameSaveInfo;
using MASGAU.Registry;
using Microsoft.Win32;
using MVC.Communication;
using MVC.Translator;
namespace MASGAU.Analyzer {
    public class PCAnalyzer : APCAnalyzer {
        public PCAnalyzer(GameEntry game, RunWorkerCompletedEventHandler when_done)
            : base(game, when_done) { }


        protected override void analyzerWork() {
            ProgressHandler.max += 3;
            base.analyzerWork();

            if (Core.locations.uac_enabled) {
                TranslatingProgressHandler.setTranslatedMessage("DumpingVirtualStore");
                outputLine(Environment.NewLine + "UAC Enabled" + Environment.NewLine);
                outputLine(Environment.NewLine + "VirtualStore Folders: ");
                string virtual_path;

                foreach (string user in Core.locations.getUsers(EnvironmentVariable.LocalAppData)) {
                    LocationPath parse_me = new LocationPath(EnvironmentVariable.LocalAppData, "VirtualStore");
                    virtual_path = Path.Combine(Core.locations.getAbsoluteRoot(parse_me, user), "VirtualStore");

                    virtual_path = Path.Combine(virtual_path, path.FullDirPath.Substring(3));
                    if (Directory.Exists(virtual_path))
                        travelFolder(virtual_path);

                }
            } else {
                outputLine(Environment.NewLine + "UAC Disabled or not present" + Environment.NewLine);
            }

            try {
                TranslatingProgressHandler.setTranslatedMessage("AnalyzingRegistry");
                ProgressHandler.value++;
                searchRegistry();
            } catch (Exception ex) {
                outputLine("Error while attempting to search the registry:");
                recordException(ex);
            }

            try {
                TranslatingProgressHandler.setTranslatedMessage("AnalyzingStartMenu");
                ProgressHandler.value++;
                searchStartMenu();
            } catch (Exception ex) {
                outputLine("Error while attempting to search the start menu:");
                recordException(ex);
            }
        }


        private void searchRegistry() {

            if (worker.CancellationPending)
                return;

            TranslatingProgressHandler.setTranslatedMessage("AnalyzingLocalMachineRegistry");
            outputLine("Local Machine Registry Entries: ");
            RegistryKey look_here = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
            registryTraveller(look_here);

            if (worker.CancellationPending)
                return;

            TranslatingProgressHandler.setTranslatedMessage("AnalyzingCurrentUserRegistry");
            outputLine("Current User Registry Entries: ");
            ProgressHandler.value++;

            look_here = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software");
            registryTraveller(look_here);
        }
        private void registryTraveller(RegistryKey look_here) {
            if (worker.CancellationPending)
                return;
            foreach (string check_me in look_here.GetValueNames()) {
                RegistryKeyValue value = new RegistryKeyValue();
                try {
                    value.key = look_here.Name;
                    value.value = check_me;
                    if (look_here.GetValue(check_me) != null) {
                        value.data = look_here.GetValue(check_me).ToString();
                        if (value.data.Length >= path.FullDirPath.Length && path.FullDirPath.ToLower() == value.data.Substring(0, path.FullDirPath.Length).ToLower()) {
                            outputLine(Environment.NewLine + "Key:" + value.key);
                            outputLine("Value: " + value.value);
                            output("Data: ");
                            outputPath(value.data);
                        }
                    }
                } catch { }
            }

            try {
                RegistryKey sub_key;
                foreach (string check_me in look_here.GetSubKeyNames()) {
                    try {
                        sub_key = look_here.OpenSubKey(check_me);
                        if (sub_key != null)
                            registryTraveller(sub_key);
                    } catch (System.Security.SecurityException) { }
                }
            } catch { }
        }

        private void searchStartMenu() {

            if (worker.CancellationPending)
                return;

            outputLine(Environment.NewLine + "Detected Start Menu Shortcuts: ");

            string start_menu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            if (!String.IsNullOrEmpty(start_menu) && Directory.Exists(start_menu))
                startMenuTraveller(start_menu);
            else
                outputLine("Folder for Start Menu (" + start_menu + ") not found");

            start_menu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            if (!String.IsNullOrEmpty(start_menu) && Directory.Exists(start_menu))
                startMenuTraveller(start_menu);
            else
                outputLine("Folder for global Start Menu (" + start_menu + ") not found");
        }

        private void startMenuTraveller(string look_here) {
            if (worker.CancellationPending)
                return;

            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut link;

            try {
                foreach (FileInfo shortcut in new DirectoryInfo(look_here).GetFiles("*.lnk")) {
                    try {
                        link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcut.FullName);
                        if (link.TargetPath.Length >= path.FullDirPath.Length && path.FullDirPath.ToLower() == link.TargetPath.Substring(0, path.FullDirPath.Length).ToLower()) {
                            this.outputPath(shortcut.FullName);
                            this.outputPath(link.TargetPath);
                        }
                    } catch (Exception e) {
                        outputLine(e.Message);
                    }
                }
            } catch (FileNotFoundException) {
            } catch (UnauthorizedAccessException) {
            } catch (Exception e) {
                outputLine(e.Message);
            }

            try {
                foreach (DirectoryInfo now_here in new DirectoryInfo(look_here).GetDirectories()) {
                    try {
                        startMenuTraveller(now_here.FullName);
                    } catch (Exception e) {
                        this.outputLine(e.Message);
                    }
                }
            } catch (DirectoryNotFoundException) {
            } catch (UnauthorizedAccessException) {
            } catch (Exception e) {
                outputLine(e.Message);
            }

        }
    }
}
