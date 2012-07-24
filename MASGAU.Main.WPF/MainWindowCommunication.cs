using System;
using System.Threading;
using MVC.Communication;
using Communication.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew : ICommunicationReceiver {


        #region RequestHandler stuff
        public override void requestInformation(RequestEventArgs e) {
            switch (e.info_type) {
                case RequestType.BackupFolder:
                    if (changeBackupPath()) {
                        e.result.cancelled = false;
                        e.response = ResponseType.OK;
                    } else {
                        e.result.cancelled = true;
                        e.response = ResponseType.Cancel;
                    }
                    return;
                case RequestType.SyncFolder:
                    if (changeSyncPath()) {
                        e.result.cancelled = false;
                        e.response = ResponseType.OK;
                    } else {
                        e.result.cancelled = true;
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
                progressLabel.Content = e.message;
                if (this.Visibility != System.Windows.Visibility.Visible)
                    sendBalloon(e.message);
            }

            WPFCommunicationHelpers.ApplyProgress(progress, e);
        }

        #endregion

    }
}
