using System;
using System.ComponentModel;
using MVC.Translator;
using MASGAU.Location;
using MASGAU.Monitor;
using Translator;
using MVC.Communication;

namespace MASGAU {
    public abstract class AProgramHandler: Core, INotifyPropertyChanged {

        // The title of the program's window
        public String program_title {
            get {
                return _program_title.ToString();
            }
        }

        protected string _program_title = "MASGAU";

        public AProgramHandler(ALocationsHandler locations)
            : base() {

                if (!Core.Ready)
                    return;

            if (Core.Mode>= Config.ConfigMode.Portable)
                _program_title = Strings.GetLabelString("PortableMode", _program_title);

            if (Core.locations == null) {
                Core.locations = locations;
            }

            worker.DoWork += new DoWorkEventHandler(doWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workCompleted);
        }

        protected virtual void workCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        protected virtual void doWork(object sender, DoWorkEventArgs e) {
            ProgressHandler.state = ProgressState.Indeterminate;
            if (!initialized) {
                TranslatingProgressHandler.setTranslatedMessage("LoadingSettings");

                if (!settings.IsReady) {
                    initialized = false;
                    TranslatingMessageHandler.SendError("CriticalSettingsFailure");
                    return;
                }

                Core.locations.setup();

                settings.PropertyChanged += new PropertyChangedEventHandler(settings_PropertyChanged);

                monitor = new Monitor.Monitor();

                TranslatingProgressHandler.setTranslatedMessage("ValidatingBackupPath");
                if (settings.IsBackupPathSet && (!PermissionsHelper.isReadable(settings.backup_path) || !PermissionsHelper.isWritable(settings.backup_path)))
                    settings.clearBackupPath();

                if (!locations.ready)
                    return;

                //task = new Task.TaskHandler();

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
