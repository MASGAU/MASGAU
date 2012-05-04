using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using MASGAU.Archive;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Game;
using Communication;
using Communication.Progress;
using Communication.Translator;
using Translations;
namespace MASGAU.Restore
{
    public class ARestoreProgramHandler<L>: AProgramHandler<L> where L: ALocationsHandler
    {

        public static Boolean use_defaults = false;
        public static List<string> unsuccesfull_restores = new List<string>();
        public static Boolean overall_stop = false;

        public ObservableCollection<LocationPathHolder> path_candidates;
        public ObservableCollection<string> user_candidates; 

        public ArchiveHandler archive = null;
        public GameHandler game_data {
            get; protected set;
        }

        public ARestoreProgramHandler(Interface new_interface, ArchiveHandler archive): base(new_interface) {
            this._program_title.Append(Strings.getGeneralString("IsRestoring"));
            this.archive = archive;
        }

        public void addPathCandidate(LocationPathHolder location) {
            foreach(LocationPathHolder path in path_candidates) {
                if(location.GetType()==typeof(ManualLocationPathHolder)) {
                    if(path.GetType()==typeof(ManualLocationPathHolder)) {
                        path_candidates.Remove(path);
                        break;
                    }
                } else if(path.ToString().Equals(location.ToString())) {
                    if(location.GetType()==typeof(DetectedLocationPathHolder)) {
                        path_candidates.Remove(path);
                        path_candidates.Add(location);
                    }
                    return;
                }
            }
            path_candidates.Add(location);
        }

        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            base.doWork(sender, e);

            path_candidates = new ObservableCollection<LocationPathHolder>();
            user_candidates = new ObservableCollection<string>();

            ProgressHandler.state = ProgressState.Indeterminate;

            if(archive==null) {
                string[] args = Environment.GetCommandLineArgs();
                if(args.Length>0) {
                    foreach(string arg in args) {
                        if(!arg.StartsWith("-")&&(arg.EndsWith(Core.extension)||arg.EndsWith(Core.extension + "\""))) {
                            archive = new ArchiveHandler(new FileInfo(arg.Trim('\"')));
                        }
                    }
                }
            }

            if (archive == null)
                throw new TranslateableException("NoRestoreFileSelected");

            TranslatingProgressHandler.setTranslatedMessage("DetectingGameForRestoration");

            if (!File.Exists(archive.file_name))
                throw new TranslateableException("FileDoesntExist",archive.file_name);
                
            GameID selected_game = archive.id.game;
            string backup_owner = archive.id.owner;
            string archive_type = archive.id.type;

            if(!Core.games.all_games.containsId(selected_game))
                throw new TranslateableException("UnknownGame", selected_game.ToString());

            game_data = Core.games.all_games.get(selected_game);
            game_data.detect();
  
            // This adds hypothetical locations
			foreach(LocationPathHolder location in game_data.location_paths) {
                if(game_data.detection_required)
                    break;

                //if(location.path==null)
                    //continue;
                if(location.platform_version!=PlatformVersion.All&&location.platform_version!=Core.locations.platform_version)
                    continue;

				if(location.rel_root==EnvironmentVariable.InstallLocation||
                    location.rel_root==EnvironmentVariable.SteamCommon)
                    continue;

                // Adds user-friendly menu entries to the root selector
                switch(location.rel_root) {
                    case EnvironmentVariable.SteamSourceMods:
                        if(Core.locations.steam_detected)
                            addPathCandidate(location);
                        break;
                    case EnvironmentVariable.SteamUser:
                        if(Core.locations.steam_detected&&Core.locations.getUsers(EnvironmentVariable.SteamUser).Count>0)
                            addPathCandidate(location);
                        break;
                    case EnvironmentVariable.SteamUserData:
                        if(Core.locations.steam_detected&&Core.locations.getUsers(EnvironmentVariable.SteamUserData).Count>0)
                            addPathCandidate(location);
                        break;
                    default:
                        addPathCandidate(location);
                        break;
                }
			}
                        
            // This add already found locations
			foreach(KeyValuePair<string,DetectedLocationPathHolder> location in game_data.detected_locations) {
                location.Value.IsSelected = true;
                switch(location.Value.rel_root) {
                    case EnvironmentVariable.ProgramFiles:
                    case EnvironmentVariable.ProgramFilesX86:
                        // This adds a fake VirtualStore folder, just in case
                        if(Core.locations.uac_enabled) {
                            DetectedLocationPathHolder temp = new DetectedLocationPathHolder(location.Value);
                            temp.abs_root = null;
                            temp.owner = location.Value.owner;
                            temp.path = Path.Combine("VirtualStore", location.Value.full_dir_path.Substring(3));
                            temp.rel_root = EnvironmentVariable.LocalAppData;
                            addPathCandidate(temp);
                        }
                        addPathCandidate(location.Value);
                        break;
                    default:
                        addPathCandidate(location.Value);
                        break;
                }
			}


            if(path_candidates.Count==1) {
                multiple_paths = false;
                NotifyPropertyChanged("only_path");
            } else if(path_candidates.Count>1) {
                multiple_paths = true;
            } else {
                throw new CommunicatableException("Can't Restore If We Can't Find It","No restore paths detected for " + this.archive.id.ToString(),false);
            }
        }
        public bool _user_needed = false;
        public bool user_needed {
            get { return _user_needed; }
            set {
                _user_needed = value;
                NotifyPropertyChanged("user_needed");
            }
        }
        public bool _multiple_users = false;
        public bool single_user { get { return !_multiple_users; }}
        public bool multiple_users{
            get { return _multiple_users; }
            set {
                _multiple_users= value;
                NotifyPropertyChanged("multiple_users");
                NotifyPropertyChanged("single_user");
            }
        }
        public string only_user {
            get {
                if(user_candidates.Count>0) 
                    return user_candidates[0];
                else 
                    return "";
            }
        }

        public bool _multiple_paths = false;
        public bool single_path { get { return !_multiple_paths;}}
        public bool multiple_paths{
            get { return _multiple_paths; }
            set {
                _multiple_paths= value;
                NotifyPropertyChanged("multiple_paths");
                NotifyPropertyChanged("single_path");
            }
        }
        public LocationPathHolder only_path {
            get {
                if(path_candidates.Count>0) 
                    return path_candidates[0];
                else 
                    return null;
            }
        }
        // Make this a more robust suggestion engine
        public LocationPathHolder recommended_path {
            get {
                LocationPathHolder candidate = null;
                foreach(LocationPathHolder path in path_candidates) {
                    if(path.GetType()==typeof(ManualLocationPathHolder)) {
                        return path;
                    }
                    if(candidate!=null&&path.GetType()==typeof(DetectedLocationPathHolder)) {
                        candidate = path;
                    }
                }
                if(candidate!=null)
                    return candidate;
                else 
                    return path_candidates[0];
            }
        }
        public void populateUsers(LocationPathHolder location) {
            user_candidates.Clear();
            List<string> users = Core.locations.getUsers(location.rel_root);
            if(users.Count>0) {
                user_needed = true;
                if(users.Count>1) {
                    multiple_users = true;
                } else {
                    multiple_users = false;
                    NotifyPropertyChanged("only_user");
                }
            } else {
                switch(location.rel_root) {
                    case EnvironmentVariable.AllUsersProfile:
                    case EnvironmentVariable.AltSavePaths:
                    case EnvironmentVariable.Drive:
                    case EnvironmentVariable.InstallLocation:
                    case EnvironmentVariable.None:
                    case EnvironmentVariable.ProgramFiles:
                    case EnvironmentVariable.ProgramFilesX86:
                    case EnvironmentVariable.PS3Export:
                    case EnvironmentVariable.PS3Save:
                    case EnvironmentVariable.PSPSave:
                    case EnvironmentVariable.Public:
                    case EnvironmentVariable.SteamCommon:
                    case EnvironmentVariable.SteamSourceMods:
                        user_needed = false;
                        break;
                    default:
                        throw new CommunicatableException("User Error","The environment variable " + location.rel_root + " requires a user to restore to, but none was detected for it. Try creating a save with a game that you know uses the folder.",false);
                }

            }
            foreach(string user in users) {
                user_candidates.Add(user);
            }
        }

        public void cancel() {
            this.CancelAsync();
            if(restore_worker!=null)
                restore_worker.CancelAsync();
            archive.cancel_restore = true;
        }


        private List<String> file_list = null;

        public void specifyFileToRestore(string file_name) {
            if(file_list==null) {
                file_list = new List<string>();
            }
            file_list.Add(file_name);
        }



        public BackgroundWorker restore_worker;

        private string restore_path;
        public void restoreBackup(LocationPathHolder path, string user, RunWorkerCompletedEventHandler when_done) {
            string target;
            if(path.GetType()==typeof(ManualLocationPathHolder)) {
                target = path.ToString();
            } else {
                target = Core.locations.getAbsolutePath(path,user);
            }
            restoreBackup(target, when_done);
        }

        public void restoreBackup(string location, RunWorkerCompletedEventHandler when_done) {
            this.restore_path = location;
            restore_worker = new BackgroundWorker();
            restore_worker.DoWork += new DoWorkEventHandler(restore_worker_DoWork);
            restore_worker.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(when_done);
            restore_worker.RunWorkerAsync();
        }

        void restore_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ProgressHandler.state = ProgressState.Normal;
            archive.restore(new DirectoryInfo(restore_path),file_list);
        }
    }
}
