using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.IO;
using MASGAU.Communication.Message;

namespace MASGAU.Update
{
    public class UpdateProgramHandler: BackgroundWorker
    {
        public UpdatesHandler updater = new UpdatesHandler();

        public UpdateProgramHandler() {
            if(!SecurityHandler.amAdmin()) {
                SecurityHandler.elevation(Core.programs.updater,null);
            }
            this.DoWork += new DoWorkEventHandler(UpdateProgramHandler_DoWork);
        }

        void UpdateProgramHandler_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach(UpdateHandler update in updater) {
                if((bool)update.update_me) {
                    update.update();
                } 
            } 
            //MessageHandler.SendInfo("Feeling Better","Update Finished");
        }

    }
}
