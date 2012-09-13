using System;
using System.Collections.Generic;
using MASGAU.Location;
using MVC.Communication;
using MVC.Translator;
using Translator;
using MASGAU.Backup;
namespace MASGAU.Main {
    public abstract class AMainProgramHandler : AProgramHandler {
        public bool disable_resize = false;

        public AMainProgramHandler()
            : base() {

            string mode;

            if (AllUsersMode)
                mode = Strings.GetSourceString("AllUsersMode");
            else
                mode = Strings.GetSourceString("SingleUserMode");

            ProgramTitle = Strings.GetLabelString("MASGAUWindowTitle", Common.VersionString, mode);
            Monitor = new MASGAU.Monitor.Monitor();
            Linker = this.CreateSymLinker();

        }


        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            base.doWork(sender, e);

            if (!ProgramReady)
                return;

            detectGames();

        }

        public void detectGames() {
            Games.detectGames();
            Archives.DetectBackups();

            Monitor.flushQueue();
            Updater = new Updater.Updater(Games.xml, Games.GameDataFolder);
            Monitor.start();
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
            ProgressHandler.max = Updater.Data.UpdateCount;

            try {
                while (Updater.Data.UpdateAvailable) {
                    ProgressHandler.value++;
                    TranslatingProgressHandler.setTranslatedMessage("UpdatingFile", Updater.Data.NextUpdateName);
                    Updater.Data.DownloadNextUpdate();
                }
            } catch (Exception e) {
                Logger.Logger.log(e);
            }
        }
        public void downloadProgramUpdate() {
            Updater.Program.DownloadNextUpdate();
        }

        #endregion
    }
}
