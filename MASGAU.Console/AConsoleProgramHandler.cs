﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Communication.Progress;

namespace MASGAU.Console
{
    class AConsoleProgramHandler<L>: AProgramHandler<L> where L: Location.ALocationsHandler
    {
        public AConsoleProgramHandler(): base(Interface.Console) {}

        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            base.doWork(sender, e);

            if(!initialized)
                return;

            //if(Core.settings.auto_update&&!Core.settings.already_updated) {
                //ProgressHandler.progress_message = "Checking For Updates...";
                //Core.updater.checkUpdates(false,true);
            //} 
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

        }


        public void detectGames() {
            Core.games.detectGames();
        }
    }
}
