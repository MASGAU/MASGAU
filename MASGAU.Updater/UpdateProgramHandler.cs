using System.ComponentModel;

namespace MASGAU.Update {
    public class UpdateProgramHandler : BackgroundWorker {
        public UpdatesHandler updater = new UpdatesHandler();

        public UpdateProgramHandler() {
            if (!SecurityHandler.amAdmin()) {
                SecurityHandler.elevation(Core.programs.updater, null);
            }
            this.DoWork += new DoWorkEventHandler(UpdateProgramHandler_DoWork);
        }

        void UpdateProgramHandler_DoWork(object sender, DoWorkEventArgs e) {
            foreach (UpdateHandler update in updater) {
                if ((bool)update.update_me) {
                    update.update();
                }
            }
            //MessageHandler.SendInfo("Feeling Better","Update Finished");
        }

    }
}
