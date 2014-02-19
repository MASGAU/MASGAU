using System;
using System.ComponentModel;
using System.IO;
using GameSaveInfo;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MVC.Communication;
using MVC.Translator;
namespace MASGAU.Analyzer {
    public class APCAnalyzer : AAnalyzer {
        protected DetectedLocationPathHolder path;
        protected APCAnalyzer(CustomGameEntry game, RunWorkerCompletedEventHandler when_done)
            : base(game, when_done) {
        }

        protected override void analyzerWork() {
            outputLine("Game Name: " + game.Title);

            this.path = game.DetectedLocations.getMostAccurateLocation();



            if (path.EV == EnvironmentVariable.VirtualStore) {
                string drive = Path.GetPathRoot(path.FullDirPath);
                string new_path = Path.Combine(drive, path.Path);
                this.path = Core.locations.interpretPath(new_path).DetectedOnly.getMostAccurateLocation();
            }

            string[] folders = this.path.Path.Split(System.IO.Path.DirectorySeparatorChar);


            string found_path = null;
            for (int i = folders.Length - 1; i >= 0; i--) {
                string temp_path = folders[0];
                for(int j = 1; j <= i; j++) {
                    temp_path = Path.Combine(temp_path,folders[j]);
                }

                DirectoryInfo dir = new DirectoryInfo(Path.Combine(path.AbsoluteRoot, temp_path));
                if (dir.GetFiles("*.exe").Length > 0) {
                    found_path = temp_path;
                    break;
                }

            }

            if (found_path != null) {
                path.ReplacePath(found_path);
            }


            outputLine("Operating System: ");
            outputLine(Environment.OSVersion.VersionString);


            ProgressHandler.max += 4;
            outputLine();
            outputLine("Path: ");
            outputPath(path.FullDirPath);
            outputLine();
            try {
                scanForScumm(path.FullDirPath);
            } catch (Exception ex) {
                outputLine("Error while attempting to cehck for ScummVM path entries:");
                recordException(ex);
            }
            try {
                TranslatingProgressHandler.setTranslatedMessage("DumpingFolder");
                ProgressHandler.value++;
                outputLine(Environment.NewLine + "Folder Dump: ");
                travelFolder(path.FullDirPath);
            } catch (Exception ex) {
                outputLine("Error while attempting to search the save folder:");
                recordException(ex);
            }

        }

        protected void scanForScumm(string save_path) {
            TranslatingProgressHandler.setTranslatedMessage("AnalyzingScummVM");
            ProgressHandler.value++;
            outputLine(Environment.NewLine + "ScummVM Path Entries: ");
            AScummVMLocationHandler scummvm = Core.locations.getHandler(HandlerType.ScummVM) as AScummVMLocationHandler;
            foreach (string user in scummvm.Locations.Keys) {
                foreach (string name in scummvm.Locations[user].Keys) {
                    string path = scummvm.Locations[user][name];
                    if (path.ToLower().Contains(save_path.ToLower())) {
                        outputLine("Entry Name: " + name);
                        outputLine("Entry Path: " + path);
                    }
                }
            }
        }

    }
}
