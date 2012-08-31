using System;
using System.Collections.Generic;
using MVC.Translator;
using MASGAU.Location;
using Translator;
using MVC.Communication;
namespace MASGAU.Main {
    public class MainProgramHandler : AProgramHandler {
        public bool disable_resize = false;

        public MainProgramHandler(ALocationsHandler loc)
            : base(loc) {

            string mode;

            if (AllUsersMode)
                mode = Strings.GetSourceString("AllUsersMode");
            else
                mode = Strings.GetSourceString("SingleUserMode");

            _program_title = Strings.GetLabelString("MASGAUWindowTitle",Core.version.ToString(),mode);

        }
        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            base.doWork(sender, e);

            if (!initialized)
                return;

            Games.loadXml();

            if (!Core.initialized) {
                //this.Close();
                return;
            }
            detectGames();

        }

        public void detectGames() {
            Games.detectGames();
            Archives.DetectBackups();
            Monitor.Monitor.flushQueue();
            updater = new Updater.Updater(Games.xml, Games.GameDataFolder);
            Core.monitor.start();
        }

        #region Methods for preparing data about the games

        public Dictionary<string, int> contributions;

        private void addContribution(string contributor) {
            if (!contributions.ContainsKey(contributor))
                contributions.Add(contributor, 1);
            else
                contributions[contributor]++;
        }

        #endregion

        #region Methods for downloading updates 
        public void downloadDataUpdates() {
            ProgressHandler.value = 0;
            ProgressHandler.max = Core.updater.Data.UpdateCount;

            try {
                while (Core.updater.Data.UpdateAvailable) {
                    ProgressHandler.value++;
                    TranslatingProgressHandler.setTranslatedMessage("UpdatingFile", Core.updater.Data.NextUpdateName);
                    Core.updater.Data.DownloadNextUpdate();
                }
            } catch (Exception e) {
                Logger.Logger.log(e);
            }
        }
        public void downloadProgramUpdate() {
            Core.updater.Program.DownloadNextUpdate();
        }

        #endregion
    }
}
