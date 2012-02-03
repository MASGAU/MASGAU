using System;
using System.ComponentModel;
using MASGAU.Location;
using MASGAU.Communication.Progress;
using MASGAU.Config;
using MASGAU.Monitor;
using MASGAU.Game;
using MASGAU.Communication.Message;

namespace MASGAU {
    public abstract class AProgramHandler<L> : Core where L : ALocationsHandler {
        public AProgramHandler(RunWorkerCompletedEventHandler when_done, Interface new_interface)
            : base(new_interface) {
            this.DoWork += new DoWorkEventHandler(doWork);
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workCompleted);
            if (when_done != null) {
                this.RunWorkerCompleted += when_done;
            }
        }

        protected virtual void workCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        protected virtual void doWork(object sender, DoWorkEventArgs e) {
            ProgressHandler.progress_state = ProgressState.Indeterminate;
            if (!initialized) {
                ProgressHandler.progress_message = "Loading Settings...";

                settings = new SettingsHandler();

                if (!settings.IsReady) {
                    initialized = false;
                    MessageHandler.SendError("Show Stopper", "");
                    return;
                }

                settings.PropertyChanged += new PropertyChangedEventHandler(settings_PropertyChanged);

                monitor = new MonitorHandler();

                updater = new Update.UpdatesHandler();

                archives = new Archive.ArchivesHandler();

                ProgressHandler.progress_message = "Validating Backup Path...";
                if (settings.backup_path_set && (!PermissionsHelper.isReadable(settings.backup_path) || !PermissionsHelper.isWritable(settings.backup_path)))
                    settings.clearBackupPath();

                locations = (ALocationsHandler)Activator.CreateInstance(typeof(L));

                if (!locations.ready)
                    return;

                games = new GamesHandler();

                task = new Task.TaskHandler();

                initialized = true;
            }
            //ProgressHandler.progress_state = ProgressState.Normal;
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



    }
}
