using System;
using System.Collections.Generic;
using System.ComponentModel;
using Communication.Translator;
using MVC.Communication;

namespace MASGAU.Analyzer {
    public class PSAnalyzer : AAnalyzer {

        public PSAnalyzer(RunWorkerCompletedEventHandler when_done)
            : base(when_done) {

        }

        protected override void analyzerWork() {
            ProgressHandler.max = 4;
            outputPsFolder(Location.EnvironmentVariable.PS3Save, "PS3 Save");
            outputPsFolder(Location.EnvironmentVariable.PS3Export, "PS3 Export");
            outputPsFolder(Location.EnvironmentVariable.PSPSave, "PSP Save");
        }

        private void outputPsFolder(Location.EnvironmentVariable ev, string name) {
            List<string> paths = Common.Locations.getPaths(ev);
            TranslatingProgressHandler.setTranslatedMessage("AnalyzingPlayStationSaves");
            ProgressHandler.value++;
            outputLine(Environment.NewLine + "Dumping Detected " + name + " Folders: ");
            foreach (string path in paths) {
                try {
                    travelFolder(path);
                } catch (Exception ex) {
                    outputLine("Error while attempting to search the " + name + " folder:");
                    recordException(ex);
                }
            }

        }

    }
}
