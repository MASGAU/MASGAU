using System;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Registry;

namespace MASGAU.Analyzer
{
 
    /// <summary>
    /// Interaction logic for SearchingWindow.xaml
    /// </summary>
    public partial class SearchingWindow : AWindow
    {
		private string game_path, save_path, game_name;
		public StringBuilder output;

        private bool playstation_search;
        private bool cancelled = false;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;

        public SearchingWindow(string new_game_path, string new_save_path, string new_game_name, bool search_playstation, AWindow owner): base(owner)
        {
			InitializeComponent();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
			game_path = new_game_path;
			save_path = new_save_path;
            game_name = new_game_name;
            playstation_search = search_playstation;
            output = new StringBuilder();
        }

        #region The thigns that do the work

        private string prepareDirectory(DirectoryInfo dir) {
            List<DetectedLocationPathHolder> paths = Core.locations.interpretPath(dir.FullName.Trim(Path.DirectorySeparatorChar));
            if (paths.Count == 0)
                return dir.FullName;

            DetectedLocationPathHolder candidate = null;
            foreach (DetectedLocationPathHolder path in paths) {
                if (candidate == null || candidate.rel_root < path.rel_root)
                    candidate = path;
            }
            return candidate.full_relative_dir_path;
        }

        private bool outputFile(FileInfo file) {
            if (!file.Exists)
                return false;

            string output_path = Path.Combine(prepareDirectory(file.Directory),file.Name);

            output.AppendLine(file.LastWriteTime + " - " + output_path + " - " + file.Length);
            return true;
        }

        private bool outputFile(String path) {
            return this.outputFile(new FileInfo(path));
        }

        private bool outputDirectory(String dir_path) {
            return outputDirectory(new DirectoryInfo(dir_path));
        }
        private bool outputDirectory(DirectoryInfo dir) {
            if (!dir.Exists)
                return false;

            string output_path = prepareDirectory(dir);

            output.AppendLine(output_path);

            return true;
        }

        private bool outputFileSystemPath(String path) {
            if (!outputFile(path)) {
                if (!outputDirectory(path)) {
                    return false;
                }
            }
            return true;
        }

        private void searchRegistry() {
            
            if(backgroundWorker1.CancellationPending)
                return;
            
            backgroundWorker1.ReportProgress(1,"Scanning Local Machine Registry...");
            output.AppendLine( "Local Machine Registry Entries: ");
			RegistryKey look_here = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
			registryTraveller(look_here);

            if(backgroundWorker1.CancellationPending)
                return;
            output.AppendLine( "Current User Registry Entries: ");
            backgroundWorker1.ReportProgress(2,"Scanning Current User Registry...");
            look_here = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software");
			registryTraveller(look_here);
		}

		private void registryTraveller(RegistryKey look_here) {
            if(backgroundWorker1.CancellationPending)
                return;
			foreach(string check_me in look_here.GetValueNames()) {
		        RegistryKeyValue value = new RegistryKeyValue();
                try {
				value.key = look_here.Name;
				value.value = check_me;
				if(look_here.GetValue(check_me)!=null) {
					value.data = look_here.GetValue(check_me).ToString();
					if(value.data.Length>=game_path.Length&&game_path.ToLower()==value.data.Substring(0,game_path.Length).ToLower()) {
						output.AppendLine( Environment.NewLine + "Key:" + value.key);
                        output.AppendLine( "Value: " + value.value);
                        output.Append("Data: ");
                        outputFileSystemPath(value.data);
					}
				}
                } catch {}
			}

            try {
			    RegistryKey sub_key;
			    foreach(string check_me in look_here.GetSubKeyNames()) {
				    try {
					    sub_key = look_here.OpenSubKey(check_me);
					    if(sub_key!=null)
						    registryTraveller(sub_key);
				    } catch(System.Security.SecurityException) {}
			    }
            } catch {}
		}

		private void searchStartMenu() {
            if(backgroundWorker1.CancellationPending)
                return;
			output.AppendLine( Environment.NewLine + "Detected Start Menu Shortcuts: ");
            backgroundWorker1.ReportProgress(3,"Scanning Start Menu...");

			string start_menu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            if (start_menu != null && Directory.Exists(start_menu))
                startMenuTraveller(start_menu);
            else
                output.AppendLine("Folder for Start Menu (" + start_menu + ") not found");

			start_menu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
			if(start_menu!=null&&Directory.Exists(start_menu))
				startMenuTraveller(start_menu);
            else
                output.AppendLine("Folder for global Start Menu (" + start_menu + ") not found");
        }

		private void startMenuTraveller(string look_here) {
            if(backgroundWorker1.CancellationPending)
                return;

			IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut link;

            try
            {
                foreach (FileInfo shortcut in new DirectoryInfo(look_here).GetFiles("*.lnk"))
                {
                    try {
                        link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcut.FullName);
                        if (link.TargetPath.Length >= game_path.Length && game_path.ToLower() == link.TargetPath.Substring(0, game_path.Length).ToLower())
                        {
                            this.outputFile(shortcut.FullName);
                            this.outputFileSystemPath(link.TargetPath);
                        }
                    } catch (Exception e) {
                        this.writeLine(e.Message);
                    }
                }
            } catch (FileNotFoundException e) {
            } catch (UnauthorizedAccessException e) {
            } catch (Exception e) {
                writeLine(e.Message);
            }

            try {
                foreach (DirectoryInfo now_here in new DirectoryInfo(look_here).GetDirectories())
                {
                    try {
                        startMenuTraveller(now_here.FullName);
                    } catch (Exception e) {
                        this.writeLine(e.Message);
                    }
                }
            } catch(DirectoryNotFoundException) {
            } catch(UnauthorizedAccessException) {
            } catch(Exception e) {
                writeLine(e.Message);
            }
			
		}

		private void parseSaveFolder() {
            if(backgroundWorker1.CancellationPending)
                return;
			output.AppendLine( Environment.NewLine + "Root Drive Information: ");
            foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
                if(look_here.Name==Path.GetPathRoot(save_path)) {
                    output.AppendLine( "Drive Name: " + look_here.Name);
                    output.AppendLine( "Drive Root: " + look_here.RootDirectory);
                    output.AppendLine( "Drive Format: " + look_here.DriveFormat);
                    output.AppendLine( "Drive Type: " + look_here.DriveType);
                    output.AppendLine( "Ready Status: " + look_here.IsReady);
                }
            }
            output.AppendLine( Environment.NewLine + "Save Folder Dump: ");
            backgroundWorker1.ReportProgress(4,"Dumping Save Folder...");
			travelSaveFolder(save_path);
            backgroundWorker1.ReportProgress(5,"Dumping Install Folder...");
        }

        private void parseInstallFolder() {
            output.AppendLine( Environment.NewLine + "Install Folder Dump: ");
            travelSaveFolder(game_path);
            backgroundWorker1.ReportProgress(6,"Dumping VirtualStore Folders...");
			if(Core.locations.uac_enabled) {
                output.AppendLine( Environment.NewLine + "UAC Enabled" + Environment.NewLine);
				output.AppendLine( Environment.NewLine + "VirtualStore Folders: ");
				string virtual_path;

                foreach(string user in Core.locations.getUsers(EnvironmentVariable.LocalAppData)) {
                    LocationPathHolder parse_me = new LocationPathHolder();
                    parse_me.path = "VirtualStore";
                    parse_me.rel_root = EnvironmentVariable.LocalAppData;
                    virtual_path = Path.Combine(Core.locations.getAbsoluteRoot(parse_me,user),"VirtualStore");

                    backgroundWorker1.ReportProgress(6,"VirtualStore for user " + user + ": " + virtual_path);
				    virtual_path = Path.Combine(virtual_path,game_path.Substring(3));
				    if(Directory.Exists(virtual_path))
					    travelSaveFolder(virtual_path);

                }
			} else {
                output.AppendLine( Environment.NewLine + "UAC Disabled or not present" + Environment.NewLine);
            }
		}

		private void travelSaveFolder(string look_here) {
            List<FileInfo> unsorted_files = saveFolderTraveller(look_here);
            List<FileInfo> sorted_files = new List<FileInfo>();

            //foreach(FileInfo file in unsorted_files) {
            //    if(sorted_files.Count==0) {
            //        sorted_files.Add(file);
            //        continue;
            //    } else {
            //        foreach(FileInfo sorted_file in sorted_files) {
            //        }
            //    }
            //}

            foreach(FileInfo file in unsorted_files) {
                this.outputFile(file);
            }
        }

        private List<FileInfo> saveFolderTraveller(string look_here) {
            List<FileInfo> return_me = new List<FileInfo>();

            if(backgroundWorker1.CancellationPending)
                return return_me;
            try {
			    foreach(FileInfo add_me in new DirectoryInfo(look_here).GetFiles()) {
                    return_me.Add(add_me);
			    }

			    foreach(DirectoryInfo now_here in new DirectoryInfo(look_here).GetDirectories()) {
				    return_me.AddRange(saveFolderTraveller(now_here.FullName));
			    }
            } catch(UnauthorizedAccessException e) {
                output.AppendLine( e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                output.AppendLine( e.Message);
            }

            return return_me;
		}
        #endregion

        #region BackgroundWorker Event Handlers
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!playstation_search) {
                try {
                    searchRegistry();
                } catch (Exception ex) {
                    writeLine("Error while attempting to search the registry:");
                    recordException(ex);
                }

                try {
                    searchStartMenu();
                } catch (Exception ex) {
                    writeLine("Error while attempting to search the start menu:");
                    recordException(ex);
                }
            }

            try {
                parseSaveFolder();
            }
            catch (Exception ex) {
                writeLine("Error while attempting to search the save folder:");
                recordException(ex);
            }

            if (!playstation_search) {
                try {
                    parseInstallFolder();
                }
                catch (Exception ex) {
                    writeLine("Error while attempting to search the install folder:");
                    recordException(ex);
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
            TaskbarItemInfo.ProgressValue = e.ProgressPercentage/(double)6;
            progressBar1.Value = e.ProgressPercentage/(double)6*100;
            groupBox1.Header = (string)e.UserState;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            this.Closing -= new CancelEventHandler(Window_Closing);
            if(cancelled)
                this.DialogResult = false;
            else 
			    this.DialogResult = true;

        }
        #endregion

        private void writeLine(String line) {
            output.AppendLine( line);
        }

        private void recordException(Exception e) {
            writeLine(e.Message);
            writeLine(e.StackTrace);
            if (e.InnerException != null)
                recordException(e.InnerException);
        }


        #region Other Event Handlers
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            cancelled = true;
            cancelBtn.IsEnabled = false;
            backgroundWorker1.CancelAsync();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            cancelled = true;
            cancelBtn.IsEnabled = false;
            backgroundWorker1.CancelAsync();
            e.Cancel = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            output.AppendLine("Game Name: " + game_name);

			output.AppendLine( "Operating System: ");
			output.AppendLine( Environment.OSVersion.VersionString);

            if (!playstation_search){
                output.AppendLine();
    			output.AppendLine("Install Path: "); 
                outputDirectory(game_path);
            }
            output.AppendLine();
            output.AppendLine("Save Path: ");
            outputDirectory(save_path);
            output.AppendLine();
            backgroundWorker1.RunWorkerAsync();
        }
        #endregion
    }
}
