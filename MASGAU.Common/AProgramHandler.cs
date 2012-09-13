using System;
using System.ComponentModel;
using MASGAU.Location;
using MVC.Communication;
using MVC.Translator;
using Translator;

namespace MASGAU {
    public abstract class AProgramHandler : Common {

        // The title of the program's window
        public String ProgramTitle {
            get;
            protected set;
        }

        public AProgramHandler()
            : base() {
                ProgramTitle = "MASGAU";

            if (!CoreReady)
                return;

            if (Mode >= Config.ConfigMode.Portable)
                ProgramTitle = Strings.GetLabelString("PortableMode", ProgramTitle);



            worker.DoWork += new DoWorkEventHandler(doWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workCompleted);
        }

        protected virtual void workCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        protected virtual void doWork(object sender, DoWorkEventArgs e) {
            ProgressHandler.state = ProgressState.Indeterminate;
            if (!ProgramReady) {
                TranslatingProgressHandler.setTranslatedMessage("LoadingSettings");

                if (!Settings.IsReady) {
                    ProgramReady = false;
                    TranslatingMessageHandler.SendError("CriticalSettingsFailure");
                    return;
                }

                Locations.setup();

                Settings.PropertyChanged += new PropertyChangedEventHandler(settings_PropertyChanged);

                Monitor = new Monitor.Monitor();

                TranslatingProgressHandler.setTranslatedMessage("ValidatingBackupPath");
                if (Settings.IsBackupPathSet && (!PermissionsHelper.isReadable(Settings.backup_path) || !PermissionsHelper.isWritable(Settings.backup_path)))
                    Settings.clearBackupPath();

                if (!Locations.ready)
                    return;


                ProgramReady = true;
            }
        }

        void settings_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "alt_paths":
                    //redetect_games = true;
                    break;
                case "backup_path":
                    //redetect_archives = true;
                    break;
            }
        }

        #region Background worker stuff
        public BackgroundWorker worker = new BackgroundWorker();
        public bool IsBusy {
            get {
                return worker.IsBusy;
            }
        }
        public void CancelAsync() {
            worker.CancelAsync();
        }
        public void RunWorkerAsync() {
            worker.RunWorkerAsync();
        }
        public event RunWorkerCompletedEventHandler RunWorkerCompleted {
            add { worker.RunWorkerCompleted += value; }
            remove { worker.RunWorkerCompleted -= value; }
        }
        protected bool CancellationPending {
            get {
                return worker.CancellationPending;
            }
        }
        #endregion

    }
}
