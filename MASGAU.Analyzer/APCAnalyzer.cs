using System;
using System.ComponentModel;
using Communication;
namespace MASGAU.Analyzer {
    public class APCAnalyzer : AAnalyzer {
        protected string title, install_path, save_path;

        protected APCAnalyzer(string title, string install_path, string save_path, RunWorkerCompletedEventHandler when_done)
            : base(when_done) {
            this.title = title;
            this.install_path = install_path;
            this.save_path = save_path;
        }

        protected override void analyzerWork() {
            outputLine("Game Name: " + title);

            outputLine("Operating System: ");
            outputLine(Environment.OSVersion.VersionString);


            ProgressHandler.max += 4;
            outputLine();
            outputLine("Install Path: ");
            outputPath(install_path);
            outputLine();
            outputLine("Save Path: ");
            outputPath(save_path);
            outputLine();
            try {
                ProgressHandler.value++;
                outputLine(Environment.NewLine + "ScummVM Path Entries: ");
                scanForScumm(save_path);
            } catch (Exception ex) {
                outputLine("Error while attempting to cehck for ScummVM path entries:");
                recordException(ex);
            }
            try {
                ProgressHandler.value++;
                outputLine(Environment.NewLine + "Save Folder Dump: ");
                travelFolder(save_path);
            } catch (Exception ex) {
                outputLine("Error while attempting to search the save folder:");
                recordException(ex);
            }

            try {
                ProgressHandler.value++;
                outputLine(Environment.NewLine + "Install Folder Dump: ");
                travelFolder(install_path);
            } catch (Exception ex) {
                outputLine("Error while attempting to search the install folder:");
                recordException(ex);
            }

        }

        protected void scanForScumm(string save_path) {
            throw new NotImplementedException("Haven't configured for ScummVM yet");
        }

    }
}
