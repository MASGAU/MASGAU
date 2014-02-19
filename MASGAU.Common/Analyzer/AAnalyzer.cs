using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using MASGAU.Location;
using MVC;
using MVC.Communication;
namespace MASGAU.Analyzer {
    public abstract class AAnalyzer : AWorker {
        public static string LastSavePath = null;
        public CustomGameEntry game { get; protected set; }

        public string report {
            get {
                string return_me;
                lock (_builder) {
                    return_me = _builder.ToString();
                }
                return return_me;
            }
        }

        private StringBuilder _builder = new StringBuilder();

        protected AAnalyzer(CustomGameEntry game, RunWorkerCompletedEventHandler when_done)
            : base(when_done) {
            this.game = game;
        }

        protected void outputLine() { outputLine(""); }
        protected void outputLine(string line) {
            lock (_builder) {
                _builder.AppendLine(line);
            }
            NotifyPropertyChanged("report");
        }
        protected void output(string line) {
            lock (_builder) {
                _builder.Append(line);
            }
            NotifyPropertyChanged("report");
        }

        public void analyze() {
            worker.RunWorkerAsync();
        }

        protected override void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            ProgressHandler.saveMessage();
            ProgressHandler.value = 0;
            ProgressHandler.max = 0;



            analyzerWork();
            ProgressHandler.value = 0;
            ProgressHandler.restoreMessage();
        }
        protected abstract void analyzerWork();

        protected void travelFolder(string look_here) {
            if (worker.CancellationPending)
                return;

            List<FileInfo> unsorted_files = folderTraveller(look_here);
            List<FileInfo> sorted_files = new List<FileInfo>();
            outputDriveInfo(look_here);
            foreach (FileInfo file in unsorted_files) {
                this.outputFile(file);
            }
        }
        protected List<FileInfo> folderTraveller(string look_here) {
            List<FileInfo> return_me = new List<FileInfo>();

            if (worker.CancellationPending)
                return return_me;
            try {
                foreach (FileInfo add_me in new DirectoryInfo(look_here).GetFiles()) {
                    return_me.Add(add_me);
                }

                foreach (DirectoryInfo now_here in new DirectoryInfo(look_here).GetDirectories()) {
                    return_me.AddRange(folderTraveller(now_here.FullName));
                }
            } catch (UnauthorizedAccessException e) {
                outputLine(e.Message);
            } catch (DirectoryNotFoundException e) {
                outputLine(e.Message);
            }

            return return_me;
        }

        protected void outputDriveInfo(string path) {
            if (worker.CancellationPending)
                return;
            outputLine(Environment.NewLine + "Drive Information: ");
            foreach (DriveInfo look_here in DriveInfo.GetDrives()) {
                if (look_here.Name == Path.GetPathRoot(path)) {
                    outputLine("Drive Name: " + look_here.Name);
                    outputLine("Drive Root: " + look_here.RootDirectory);
                    outputLine("Drive Format: " + look_here.DriveFormat);
                    outputLine("Drive Type: " + look_here.DriveType);
                    outputLine("Ready Status: " + look_here.IsReady);
                }
            }

        }

        #region outputting functions
        protected void recordException(Exception e) {
            outputLine(e.Message);
            outputLine(e.StackTrace);
            if (e.InnerException != null)
                recordException(e.InnerException);
        }

        protected bool outputPath(String path) {
            return outputFile(path) || outputDirectory(path);
        }

        private bool outputFile(FileInfo file) {
            if (!file.Exists)
                return false;

            string output_path = Path.Combine(censorDirectory(file.Directory), file.Name);

            outputLine(file.LastWriteTime + " - " + output_path + " - " + file.Length);
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

            string output_path = censorDirectory(dir);

            outputLine(output_path);

            return true;
        }
        protected string censorDirectory(DirectoryInfo dir) {
            DetectedLocations paths = Core.locations.interpretPath(dir.FullName.TrimEnd(Path.DirectorySeparatorChar)).DetectedOnly;
            if (paths.Count == 0)
                return dir.FullName;
            else
                return paths.getMostAccurateLocation().FullRelativeDirPath;
        }
        #endregion

    }
}
