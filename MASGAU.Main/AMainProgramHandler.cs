using System.Collections.Generic;
using Communication.Translator;
using MASGAU.Location;
using Translator;
namespace MASGAU.Main {
    public class AMainProgramHandler<L> : AProgramHandler<L> where L : ALocationsHandler {
        public bool disable_resize = false;

        public AMainProgramHandler(Interface new_iface)
            : base(new_iface) {

            string mode;

            if (all_users_mode)
                mode = Strings.GetSourceString("AllUsersMode");
            else
                mode = Strings.GetSourceString("SingleUserMode");

            _program_title = Strings.GetLabelString("MASGAUWindowTitle",Core.version.ToString(),mode);

        }
        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            base.doWork(sender, e);

            if (!initialized)
                return;

            //if (Core.settings.auto_update && !Core.settings.already_updated) {
            //    TranslatingProgressHandler.setTranslatedMessage("CheckingForUpdates");
            //    Core.updater.checkUpdates(false, true);
            //}
            if (Core.updater.redetect_required) {
                Games.loadXml();
            }
            if (Core.updater.shutdown_required) {
                //this.Close();
                return;
            }

            if (!Core.initialized) {
                //this.Close();
                return;
            }

            Games.detectGames();
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

        #region methods for dealing with custom game data

        public CustomGamesHandler custom_games = new CustomGamesHandler();


        #endregion

    }
}
