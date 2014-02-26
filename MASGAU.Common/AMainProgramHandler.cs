using System;
using System.IO;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using MASGAU.Location;
using MVC.Communication;
using MVC.Translator;
using Translator;
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

            _program_title = Strings.GetLabelString("MASGAUWindowTitle", Core.version.ToString(), mode);

        }
        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            base.doWork(sender, e);

            if (!initialized)
                return;

            //Games.loadXml();

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

        public Dictionary<string, int> contributions = new Dictionary<string,int>();

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
                    String file_path = Core.updater.Data.NextUpdatePath;
                    Core.updater.Data.DownloadNextUpdate();

                    FileInfo file = new FileInfo(file_path);
                    FileSecurity fSecurity = file.GetAccessControl();
                    // Aquires the Identity corresponding to 'Everyone' on the users computer
                    SecurityIdentifier everyoneIdentifier = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                    fSecurity.AddAccessRule(new FileSystemAccessRule(everyoneIdentifier,
                        FileSystemRights.FullControl,
                        InheritanceFlags.None, PropagationFlags.InheritOnly, AccessControlType.Allow));
                    file.SetAccessControl(fSecurity);

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
