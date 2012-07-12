using System;
using System.Threading;
using Communication;
using Communication.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew : ICommunicationReceiver {
        #region Communication handler stuff
        private SynchronizationContext _context;
        public SynchronizationContext context {
            get {
                return _context;
            }
        }
        protected bool _available = true;
        public bool available {
            get {
                return _available;
            }
        }


        #endregion

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
                //case RequestType.SyncFolder:
                //    if (changeSyncPath()) {
                //        e.result.cancelled = false;
                //        e.response = ResponseType.OK;
                //    } else {
                //        e.result.cancelled = true;
                //        e.response = ResponseType.Cancel;
                //    }
                //    return;
                case RequestType.Question:
                    if (displayQuestion(e.title, e.message)) {
                        e.result.selected_option = "Yes";
                        e.result.selected_index = 1;
                        e.response = ResponseType.OK;
                    } else {
                        e.response = ResponseType.Cancel;
                    }
                    return;
                case RequestType.Choice:
                    ChoiceWindow choice = new ChoiceWindow(e.title, e.message, e.options, e.default_option, this);
                    if ((bool)choice.ShowDialog()) {
                        choice.Close();
                        e.result.selected_index = choice.selected_index;
                        e.result.selected_option = choice.selected_item;
                        e.response = ResponseType.OK;
                    } else {
                        e.response = ResponseType.Cancel;
                    }
                    return;
                default:
                    throw new NotImplementedException("The specified request type " + e.info_type.ToString() + " is not supported in this GUI toolkit.");
            }
        }


        #endregion

        #region MessageBox showing things
        public bool displayQuestion(string title, string message) {
            Communication.WPF.MessageBox box = new Communication.WPF.MessageBox(title, message, RequestType.Question, this, Core.email);
            return (bool)box.ShowDialog();
        }
        public bool displayError(string title, string message) {
            return displayError(title, message, null);
        }
        public bool displayError(string title, string message, Exception e) {
            return displayMessage(title, message, MessageTypes.Error, e);
        }
        public bool displayWarning(string title, string message) {
            return displayMessage(title, message, MessageTypes.Warning, null);
        }
        public bool displayInfo(string title, string message) {
            return displayMessage(title, message, MessageTypes.Info, null);
        }
        private bool displayMessage(string title, string message, MessageTypes type, Exception e) {
            Communication.WPF.MessageBox box = new Communication.WPF.MessageBox(title, message, e, type, this, Core.email);
            return (bool)box.ShowDialog();
        }
        #endregion

        #region Progress stuff
        public virtual void updateProgress(ProgressUpdatedEventArgs e) {
            if (e.message != null) {
                progressLabel.Content = e.message;
            }

            WPFCommunicationHelpers.ApplyProgress(progress, e);
        }

        #endregion

    }
}
