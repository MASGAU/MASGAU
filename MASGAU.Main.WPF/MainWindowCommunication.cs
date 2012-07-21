using System;
using System.Threading;
using Communication;
using Communication.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew : ICommunicationReceiver {


        #region MessageHandler stuff
        public void sendMessage(MessageEventArgs e) {
            bool response = false;
            switch (e.type) {
                case MessageTypes.Error:
                    response = displayError(e.title, e.message, e.exception);
                    break;
                case MessageTypes.Info:
                    response = displayInfo(e.title, e.message);
                    break;
                case MessageTypes.Warning:
                    response = displayWarning(e.title, e.message);
                    break;
            }
            e.response = ResponseType.OK;
        }
        #endregion

        #region RequestHandler stuff
        public virtual void requestInformation(RequestEventArgs e) {
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
            }

            WPFCommunicationHelpers.ApplyProgress(progress, e);
        }

        #endregion

    }
}
