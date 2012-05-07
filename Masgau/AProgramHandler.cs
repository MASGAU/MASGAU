using System;
using System.ComponentModel;
using System.Text;
using MASGAU.Location;
using Communication.Progress;
using MASGAU.Config;
using MASGAU.Monitor;
using MASGAU.Game;
using Communication;
using Communication.Message;
using Communication.Translator;
using Translator;
namespace MASGAU {
    public abstract class AProgramHandler<L> : Core, INotifyPropertyChanged where L : ALocationsHandler {

        // The title of the program's window
        public String program_title {
            get {
                return _program_title.ToString();
            }
        }

        protected string _program_title = "MASGAU";
        
        public AProgramHandler(Interface new_interface)
            : base(new_interface) {
            if(Core.portable_mode)
                _program_title = Strings.getGeneralString("MASGAUPortable");


            this.DoWork += new DoWorkEventHandler(doWork);
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workCompleted);
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

                settings.PropertyChanged += new PropertyChangedEventHandler(settings_PropertyChanged);

                monitor = new MonitorHandler();

                updater = new Update.UpdatesHandler();

                archives = new Archive.ArchivesHandler();

                TranslatingProgressHandler.setTranslatedMessage("ValidatingBackupPath");
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
