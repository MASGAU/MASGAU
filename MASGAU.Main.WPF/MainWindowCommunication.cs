using System;
using System.Threading;
using MVC.Communication;
using MVC.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew : ICommunicationReceiver {


        #region RequestHandler stuff
        public override void requestInformation(RequestEventArgs e) {
            switch (e.info_type) {
                case RequestType.BackupFolder:
                    if (changeBackupPath()) {
                        e.result.Cancelled = false;
                        e.response = ResponseType.OK;
                    } else {
                        e.result.Cancelled = true;
                        e.response = ResponseType.Cancel;
                    }
                    return;
                case RequestType.SyncFolder:
                    if (changeSyncPath()) {
                        e.result.Cancelled = false;
                        e.response = ResponseType.OK;
                    } else {
                        e.result.Cancelled = true;
                        e.response = ResponseType.Cancel;
                    }
                    return;

                default:
                    base.requestInformation(e);
                    break;
            }
        }


        #endregion


        #region Progress stuff
        public override void updateProgress(ProgressUpdatedEventArgs e) {
            if (e.message != null) {
                if (this.Visibility != System.Windows.Visibility.Visible)
                    notifier.sendBalloon(e.message);
            }

            this.applyProgress(progress, e);

//            WPFCommunicationHelpers.ApplyProgress(progress, e);
        }

        #endregion

    }
}
