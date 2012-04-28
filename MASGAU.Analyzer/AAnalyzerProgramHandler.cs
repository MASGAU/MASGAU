using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Communication.Progress;
using Translations;

namespace MASGAU.Analyzer
{
    public abstract class AAnalyzerProgramHandler<L> : AProgramHandler<L> where L : ALocationsHandler
    {
        protected BackgroundWorker analyzer;
		public StringBuilder output {get; protected set;}
		
		private string _gameTitle = "";
		public string gameTitle {
			get {
				return _gameTitle;
			}
			set {
				_gameTitle = value;
				NotifyPropertyChanged("psScanEnabled");
				NotifyPropertyChanged("scanEnabled");
			}
		}
		
		
		private string _gamePath;
		public string gamePath {
			get {
				return _gamePath;
			}
			set {
				_gamePath = value;
				NotifyPropertyChanged("scanEnabled");
			}
		}
		
		private string _savePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		public string savePath {
			get {
				return _savePath;
			}
			set {
				_savePath = value;
				NotifyPropertyChanged("scanEnabled");
			}
		}
		
		public bool psScanEnabled {
			get {
				
				if(_gameTitle=="")
					return false;
				else {
				}
				return false;
			}
		}
		public bool scanEnabled {
			get {
				return _gameTitle!="";
			}
		}
		
		
		private void updateButton() {
	//		if(nameEntry.Text=="") {
			//	scanBtn.Sensitive = false;
			//	psScanBtn.Sensitive = false;
	//		} else {
	//			scanBtn.Sensitive = true;
	//			if(prefixEntry.Text==""||suffixEntry.Text==""||
	//			   prefixEntry.Text.Length!=4||suffixEntry.Text.Length!=5) {
	//				psScanBtn.Sensitive = false;
	//			} else { 
	//				psScanBtn.Sensitive = true;
	//			}
	//		}
		}
		
        public AAnalyzerProgramHandler(Interface iface):base(iface)  {
            _program_title.Append(" " + Strings.get("AnalyzerWindowTitle"));
			
			switch(iface) {
			case Interface.WPF:
				_gamePath = Environment.GetEnvironmentVariable("PROGRAMFILES");
				_savePath = Environment.GetEnvironmentVariable("USERPROFILE");
			break;
			case Interface.Gtk:
				_gamePath = "/";
				_savePath = "/";
				break;
			}
			//		psLocation.SelectFilename("/");

        }
		
		public void runAnalyzer(RunWorkerCompletedEventHandler when_done) {
            output = new StringBuilder();
            output.AppendLine("Game Name: " + _gameTitle);
			
			output.AppendLine( "Operating System: ");
			output.AppendLine( Environment.OSVersion.VersionString);
			
            if (_gamePath!=null){
                output.AppendLine();
    			output.AppendLine("Install Path: "); 
                outputDirectory(_gamePath);
            }
            output.AppendLine();
            output.AppendLine("Save Path: ");
            outputDirectory(_savePath);
            output.AppendLine();
			
			analyzer = new BackgroundWorker();
            //this.backgroundWorker1.WorkerReportsProgress = true;
            analyzer.WorkerSupportsCancellation = true;
			analyzer.DoWork += HandleAnalyzerDoWork;
			analyzer.RunWorkerCompleted += HandleAnalyzerRunWorkerCompleted;
			if(when_done!=null)
				analyzer.RunWorkerCompleted += when_done;
			ProgressHandler.progress_max = 3;
			analyzer.RunWorkerAsync();
		}
		
		public void cancelAnalyzer() {
			if(analyzer!=null)
				analyzer.CancelAsync();
		}
		#region BackgroundWorker Event Handlers
		void HandleAnalyzerRunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			
		}

		protected virtual void HandleAnalyzerDoWork (object sender, DoWorkEventArgs e)
		{
			ProgressHandler.progress = 0;
            try {
				ProgressHandler.progress++;
				ProgressHandler.progress_message = Strings.get("DumpingSaveFolder");
                parseSaveFolder();
            }
            catch (Exception ex) {
                output.AppendLine("Error while attempting to search the save folder:");
                recordException(ex);
            }
			
            if (_gamePath!=null) {
                try {
					ProgressHandler.progress++;
					ProgressHandler.progress_message = Strings.get("DumpingInstallFolder");
                    parseInstallFolder();
                }
                catch (Exception ex) {
                    output.AppendLine("Error while attempting to search the install folder:");
                    recordException(ex);
                }
            }
		}
		#endregion
		
        #region The things that do the work
		protected void parseSaveFolder() {
            if (analyzer.CancellationPending)
                return;
			output.AppendLine( Environment.NewLine + "Root Drive Information: ");
            foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
                if(look_here.Name==Path.GetPathRoot(_savePath)) {
                    output.AppendLine( "Drive Name: " + look_here.Name);
                    output.AppendLine( "Drive Root: " + look_here.RootDirectory);
                    output.AppendLine( "Drive Format: " + look_here.DriveFormat);
                    output.AppendLine( "Drive Type: " + look_here.DriveType);
                    output.AppendLine( "Ready Status: " + look_here.IsReady);
                }
            }
            output.AppendLine( Environment.NewLine + "Save Folder Dump: ");
			travelSaveFolder(_savePath);
        }

        protected virtual void parseInstallFolder() {
            output.AppendLine(Environment.NewLine + "Install Folder Dump: ");
            travelSaveFolder(_savePath);
			
		}

		protected void travelSaveFolder(string look_here) {
            List<FileInfo> unsorted_files = saveFolderTraveller(look_here);
            List<FileInfo> sorted_files = new List<FileInfo>();

            foreach(FileInfo file in unsorted_files) {
                this.outputFile(file);
            }
        }

        protected List<FileInfo> saveFolderTraveller(string look_here) {
            List<FileInfo> return_me = new List<FileInfo>();

            if(analyzer.CancellationPending)
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
		
        public static string last_save_path = null;
		protected override void doWork (object sender, DoWorkEventArgs e)
		{
			ProgressHandler.progress_message = Strings.get("Loading") + " " + Strings.get("Settings") + "...";
			base.doWork (sender, e);
			ProgressHandler.progress_message = null;
		}
		
		#region Helpers
        protected void recordException(Exception e) {
            output.AppendLine(e.Message);
            output.AppendLine(e.StackTrace);
            if (e.InnerException != null)
                recordException(e.InnerException);
        }
        protected string prepareDirectory(DirectoryInfo dir) {
            List<DetectedLocationPathHolder> paths = Core.locations.interpretPath(dir.FullName.TrimEnd(Path.DirectorySeparatorChar));
            if (paths.Count == 0)
                return dir.FullName;

            DetectedLocationPathHolder candidate = null;
            foreach (DetectedLocationPathHolder path in paths) {
                if (candidate == null || candidate.rel_root < path.rel_root)
                    candidate = path;
            }
            return candidate.full_relative_dir_path;
        }

        protected bool outputFile(FileInfo file) {
            if (!file.Exists)
                return false;

            string output_path = Path.Combine(prepareDirectory(file.Directory),file.Name);

            output.AppendLine(file.LastWriteTime + " - " + output_path + " - " + file.Length);
            return true;
        }

        protected bool outputFile(String path) {
            return this.outputFile(new FileInfo(path));
        }

        protected bool outputDirectory(String dir_path) {
            return outputDirectory(new DirectoryInfo(dir_path));
        }
        protected bool outputDirectory(DirectoryInfo dir) {
            if (!dir.Exists)
                return false;

            string output_path = prepareDirectory(dir);

            output.AppendLine(output_path);

            return true;
        }

        protected bool outputFileSystemPath(String path) {
            if (!outputFile(path)) {
                if (!outputDirectory(path)) {
                    return false;
                }
            }
            return true;
        }
		#endregion
		
    }
}
