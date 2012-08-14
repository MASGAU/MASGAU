using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using MVC.Communication;
using MVC.Translator;
using MASGAU.Location;
using MASGAU.Location.Holders;
using Translator;
using GameSaveInfo;
namespace MASGAU.Restore {
    public class RestoreProgramHandler : AProgramHandler {

        public static Boolean use_defaults = false;
        public static List<string> unsuccesfull_restores = new List<string>();
        public static Boolean overall_stop = false;


        public ObservableCollection<LocationPath> path_candidates;
        
        
        public ObservableCollection<string> user_candidates;
        public Archive archive;

        public MASGAU.GameEntry game_data {
            get;
            protected set;
        }

        public RestoreProgramHandler(Archive archive, ALocationsHandler loc)
            : base(loc, Program.Restore) {
            this._program_title = Strings.GetLabelString("IsRestoring",
                this._program_title.ToString());
            this.archive = archive;
        }

        public void addPathCandidate(LocationPath location) {
            foreach (LocationPath path in path_candidates) {
                if (location.GetType() == typeof(ManualLocationPathHolder)) {
                    if (path.GetType() == typeof(ManualLocationPathHolder)) {
                        path_candidates.Remove(path);
                        break;
                    }
                } else if (path.full_relative_dir_path.Equals(location.full_relative_dir_path)) {
                    if (location.GetType() == typeof(DetectedLocationPathHolder)) {
                        path_candidates.Remove(path);
                        path_candidates.Add(location);
                    }
                    return;
                }
            }
            path_candidates.Add(location);
        }

        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            base.doWork(sender, e);

            path_candidates = new ObservableCollection<LocationPath>();
            user_candidates = new ObservableCollection<string>();

            ProgressHandler.state = ProgressState.Indeterminate;
            try {
                if (archive == null) {
                    string[] args = Environment.GetCommandLineArgs();
                    if (args.Length > 0) {
                        foreach (string arg in args) {
                            if (!arg.StartsWith("-") && (arg.EndsWith(Core.Extension) || arg.EndsWith(Core.Extension + "\""))) {
                                archive = new Archive(new FileInfo(arg.Trim('\"')));
                                break;
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                TranslatingMessageHandler.SendException(ex);
            }

            if (archive == null)
                throw new TranslateableException("NoRestoreFileSelected");

            TranslatingProgressHandler.setTranslatedMessage("DetectingGameForRestoration");

            if (!archive.Exists)
                throw new TranslateableException("FileNotFound", archive.ArchivePath);

            GameID selected_game = archive.id.Game;
            string backup_owner = archive.id.Owner;
            string archive_type = archive.id.Type;

            game_data = Games.detectGame(selected_game);

            // This adds hypothetical locations
            foreach (LocationPath location in game_data.Locations.Paths) {
                if (game_data.DetectionRequired)
                    break;

                //if(location.path==null)
                //continue;
                if (location.OnlyFor != null && location.OnlyFor != Core.locations.platform_version)
                    continue;

                if (location.EV == EnvironmentVariable.InstallLocation ||
                    location.EV == EnvironmentVariable.SteamCommon)
                    continue;

                // Adds user-friendly menu entries to the root selector
                switch (location.EV) {
                    case EnvironmentVariable.SteamSourceMods:
                        if (Core.locations.steam_detected)
                            addPathCandidate(location);
                        break;
                    case EnvironmentVariable.SteamUser:
                        if (Core.locations.steam_detected && Core.locations.getUsers(EnvironmentVariable.SteamUser).Count > 0)
                            addPathCandidate(location);
                        break;
                    case EnvironmentVariable.SteamUserData:
                        if (Core.locations.steam_detected && Core.locations.getUsers(EnvironmentVariable.SteamUserData).Count > 0)
                            addPathCandidate(location);
                        break;
                    default:
                        addPathCandidate(location);
                        break;
                }
            }


            // This add already found locations
            foreach (DetectedLocationPathHolder location in game_data.DetectedLocations) {
                location.IsSelected = true;
                switch (location.EV) {
                    case EnvironmentVariable.ProgramFiles:
                    case EnvironmentVariable.ProgramFilesX86:
                        // This adds a fake VirtualStore folder, just in case
                        if (Core.locations.uac_enabled) {
                            DetectedLocationPathHolder temp = new DetectedLocationPathHolder(location);
                            temp.AbsoluteRoot = null;
                            temp.owner = location.owner;
                            temp.Path = Path.Combine("VirtualStore", location.full_dir_path.Substring(3));
                            temp.EV = EnvironmentVariable.LocalAppData;
                            addPathCandidate(temp);
                        }
                        addPathCandidate(location);
                        break;
                    default:
                        addPathCandidate(location);
                        break;
                }
            }

            //if (archive.id.Game.OS!=null&&archive.id.Game.OS.StartsWith("PS")) {

            //    foreach (string drive in Core.locations.ps.GetDriveCandidates()) {
            //        DetectedLocationPathHolder loc = new DetectedLocationPathHolder(EnvironmentVariable.Drive, drive, null);
            //            addPathCandidate(loc);
            //    }
            //}


            if (path_candidates.Count == 1) {
                multiple_paths = false;
                NotifyPropertyChanged("only_path");
            } else if (path_candidates.Count > 1) {
                multiple_paths = true;
            } else {
                throw new TranslateableException("NoRestorePathsDetected", this.archive.id.ToString());
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
        public bool single_user { get { return !_multiple_users; } }
        public bool multiple_users {
            get { return _multiple_users; }
            set {
                _multiple_users = value;
                NotifyPropertyChanged("multiple_users");
                NotifyPropertyChanged("single_user");
            }
        }
        public string only_user {
            get {
                if (user_candidates.Count > 0)
                    return user_candidates[0];
                else
                    return "";
            }
        }

        public bool _multiple_paths = false;
        public bool single_path { get { return !_multiple_paths; } }
        public bool multiple_paths {
            get { return _multiple_paths; }
            set {
                _multiple_paths = value;
                NotifyPropertyChanged("multiple_paths");
                NotifyPropertyChanged("single_path");
            }
        }
        public LocationPath only_path {
            get {
                if (path_candidates.Count > 0)
                    return path_candidates[0];
                else
                    return null;
            }
        }
        // Make this a more robust suggestion engine
        public LocationPath recommended_path {
            get {
                LocationPath candidate = null;
                foreach (LocationPath path in path_candidates) {
                    if (path.GetType() == typeof(ManualLocationPathHolder)) {
                        return path;
                    }
                }
                foreach (LocationPath path in path_candidates) {
                    if (candidate == null && path.GetType() == typeof(DetectedLocationPathHolder)) {
                        DetectedLocationPathHolder det_path = path as DetectedLocationPathHolder;
                        if (det_path.RootHash == archive.id.OriginalPathHash) {
                            return det_path;
                        }
                        candidate = det_path;
                    }
                }
                if (candidate != null)
                    return candidate;
                else
                    return path_candidates[0];
            }
        }
        public void populateUsers(LocationPath location) {
            user_candidates.Clear();
            List<string> users = Core.locations.getUsers(location.EV);
            if (users.Count > 0) {
                user_needed = true;
                if (users.Count > 1) {
                    multiple_users = true;
                } else {
                    multiple_users = false;
                    NotifyPropertyChanged("only_user");
                }
            } else {
                switch (location.EV) {
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
                        throw new TranslateableException("NoUsersForEV", location.EV.ToString());
                }

            }
            foreach (string user in users) {
                user_candidates.Add(user);
            }
        }

        public void cancel() {
            this.CancelAsync();
            if (restore_worker != null)
                restore_worker.CancelAsync();
            archive.cancel_restore = true;
        }


        private List<String> file_list = null;

        public void specifyFileToRestore(string file_name) {
            if (file_list == null) {
                file_list = new List<string>();
            }
            file_list.Add(file_name);
        }



        public BackgroundWorker restore_worker;

        private string restore_path;
        public void restoreBackup(LocationPath path, string user, RunWorkerCompletedEventHandler when_done) {
            string target;
            if (path.GetType() == typeof(ManualLocationPathHolder)) {
                target = path.ToString();
            } else {
                target = Core.locations.getAbsolutePath(path, user);
            }
            restoreBackup(target, when_done);
        }

        public void restoreBackup(string location, RunWorkerCompletedEventHandler when_done) {
            this.restore_path = location;
            restore_worker = new BackgroundWorker();
            restore_worker.DoWork += new DoWorkEventHandler(restore_worker_DoWork);
            restore_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(when_done);
            restore_worker.RunWorkerAsync();
        }

        void restore_worker_DoWork(object sender, DoWorkEventArgs e) {
            ProgressHandler.state = ProgressState.Normal;
            archive.restore(new DirectoryInfo(restore_path), file_list);
        }
    }
}
