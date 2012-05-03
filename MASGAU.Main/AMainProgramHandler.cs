using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using MASGAU.Location;
using MASGAU.Communication.Progress;
using MASGAU.Game;
using System.ComponentModel;

namespace MASGAU.Main
{
    public class AMainProgramHandler<L> : AProgramHandler<L> where L : ALocationsHandler
    {
        public bool         disable_resize = false;
       
        public AMainProgramHandler(Interface new_iface): base(new_iface) {
            _program_title.Append(" v." + Core.version);

            if (all_users_mode)
                _program_title.Append(" - All Users Mode");
            else
                _program_title.Append(" - Single User Mode");

        }
        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            base.doWork(sender, e);

            if(!initialized)
                return;

            if(Core.settings.auto_update&&!Core.settings.already_updated) {
                ProgressHandler.message = "Checking For Updates...";
                Core.updater.checkUpdates(false,true);
            } 
            if(Core.updater.redetect_required) {
                Core.games.loadXml();
            }
            if(Core.updater.shutdown_required) {
                //this.Close();
                return;
            }

            if(!Core.initialized){
                //this.Close();
                return;
            }

            Core.games.detectGames();

            string temp = ProgressHandler.message;
            ProgressHandler.message = temp;
        }


        #region Methods for preparing data about the games

        public Dictionary<string,int> contributions;

        private void addContribution(string contributor)
        {
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
