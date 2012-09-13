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
                string drive = Path.GetPathRoot(path.full_dir_path);
                string new_path = Path.Combine(drive, path.Path);
                this.path = Core.locations.interpretPath(new_path).getMostAccurateLocation();
            }

            string[] folders = this.path.Path.Split(System.IO.Path.DirectorySeparatorChar);
            path.ReplacePath(folders[0]);


            outputLine("Operating System: ");
            outputLine(Environment.OSVersion.VersionString);


            ProgressHandler.max += 4;
            outputLine();
            outputLine("Path: ");
            outputPath(path.full_dir_path);
            outputLine();
            try {
                scanForScumm(path.full_dir_path);
            } catch (Exception ex) {
                outputLine("Error while attempting to cehck for ScummVM path entries:");
                recordException(ex);
            }
            try {
                TranslatingProgressHandler.setTranslatedMessage("DumpingFolder");
                ProgressHandler.value++;
                outputLine(Environment.NewLine + "Folder Dump: ");
                travelFolder(path.full_dir_path);
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
