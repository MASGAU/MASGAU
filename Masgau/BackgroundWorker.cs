using System.ComponentModel;
using Communication.Translator;
using MVC;
namespace MASGAU {
    public class BackgroundWorker : System.ComponentModel.BackgroundWorker, INotifyPropertyChanged, ICancellable {

        public BackgroundWorker()
            : base() {
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(forwardExceptions);
        }

        protected void forwardExceptions(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                TranslatingMessageHandler.SendException(e.Error);
            }
        }



        public event RunWorkerCompletedEventHandler Completed {
            add { this.RunWorkerCompleted += value; }
            remove { this.RunWorkerCompleted -= value; }
        }


        public void Cancel() {
                this.CancelAsync();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string prop) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
