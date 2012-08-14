using System.Collections.Generic;
using MVC.Translator;
using MASGAU.Location;
using Translator;
namespace MASGAU.Main {
    public class MainProgramHandler : AProgramHandler {
        public bool disable_resize = false;

        public MainProgramHandler(ALocationsHandler loc)
            : base(loc, MASGAU.Program.Main) {

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
            Monitor.Monitor.flushQueue();
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


    }
}
