using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using MVC;
using MVC.Communication;
using MVC.Translator;
using MASGAU.Location;
using MASGAU.Location.Holders;
using Translator;
using GameSaveInfo;
namespace MASGAU.Restore {
    public class RestoreProgramHandler : AProgramHandler {
        public bool GameNotDetected {
            get {
                return game_data == null || !game_data.IsDetected;
            }
        }
        public static Boolean use_defaults = false;
        public static List<string> unsuccesfull_restores = new List<string>();
        public static Boolean overall_stop = false;

        public string RestoreComment {
            get {
                if (game_data == null)
                    return null;
                return game_data.RestoreComment;
            }
        }

        public SimpleModel<LocationPath> path_candidates;
        
        
        public SimpleModel<string> user_candidates;
        public Archive archive;

        protected MASGAU.GameEntry game_data {
            get;
            set;
        }

        public RestoreProgramHandler(Archive archive, ALocationsHandler loc)
            : base(loc) {
            this._program_title = Strings.GetLabelString("IsRestoring",
                this._program_title.ToString());
            this.archive = archive;
        }

        public void addPathCandidate(LocationPath location) {

            // Checks if the provided path is the original archive path
            if (location is DetectedLocationPathHolder) {
                DetectedLocationPathHolder loc = location as DetectedLocationPathHolder;
                if (archive.id.OriginalLocation!=null&&archive.id.OriginalLocation.ToLower() == loc.full_dir_path.ToLower()) {
                    loc.MatchesOriginalPath = true;
                }
            }

            // If path is a manuallly added path, we remove all other manual paths,
            // Then we don't compare further, since it doesn't matter
            if (location is ManualLocationPathHolder) {
                for (int i = 0; i < path_candidates.Count; i++) {
                    if (path_candidates[i] is ManualLocationPathHolder) {
                        path_candidates.RemoveAt(i);
                    }
                }
                path_candidates.Insert(0, location);
                return;
            }


            if (location is DetectedLocationPathHolder) {
                // If the new path is a real, existant path
                DetectedLocationPathHolder loc = location as DetectedLocationPathHolder;
                for (int i = 0; i < path_candidates.Count; i++) {
                    if (path_candidates[i] is DetectedLocationPathHolder) {
                        // If the present path is a real, detected path
                        DetectedLocationPathHolder path = path_candidates[i] as DetectedLocationPathHolder;
                        // We compare the absolute paths
                        if (path.full_dir_path.ToLower() == loc.full_dir_path.ToLower()) {
                            // If it's two real, detected paths that are the same location, we compare EVs
                            if (loc.EV > path.EV) {
                                // If the new EV is better, then we replace the old one
                                path_candidates.RemoveAt(i);
                                path_candidates.AddWithSort(loc);
                            }
                            return;
                        } else {
                            // If the paths don't match, then we compare Ev-relative paths
                            if (path.FullRelativeDirPath.ToLower() == loc.FullRelativeDirPath.ToLower()) {
                                //if they're the same, then we compare for matching
                                if(loc.MatchesOriginalPath) {
                                    path_candidates.RemoveAt(i);
                                    path_candidates.AddWithSort(loc);
                                }
                                return;
                            }
                        }
                    } else {
                        // If the existing path is an theoretical path path
                        LocationPath path = path_candidates[i] ;
                        if (loc.FullRelativeDirPath.ToLower() == path.FullRelativeDirPath.ToLower()) {
                            // If both paths are based on the same EV/path combo
                            // Then the real path supercedes the fake path
                            path_candidates.RemoveAt(i);
                            path_candidates.AddWithSort(loc);
                            return;
                        } else {
                            // Otherwise we don't do anything
                        }
                    }
                }                
            } else {
                switch(location.EV) {
                    case EnvironmentVariable.Drive:
                    case EnvironmentVariable.ProgramFiles:
                    case EnvironmentVariable.ProgramFilesX86:                       
                        return;
                }
                // If the new path is only a theoretical path
                for (int i = 0; i < path_candidates.Count; i++) {
                    LocationPath path = path_candidates[i];
                    if (path.FullRelativeDirPath.ToLower() == location.FullRelativeDirPath.ToLower()) {
                        // If the relative paths match we compare the EVs for the more accurate one
                        if (location.EV > path.EV) {
                            // If the new path has a more accurate EV, we replace the old one
                            path_candidates.RemoveAt(i);
                            path_candidates.AddWithSort(location);
                        }
                        return;
                    } else {
                        // If the relative paths don't match, then they have nothign to do with eachother
                        // so we'll just let the test continue
                    }
                }
            }
            path_candidates.AddWithSort(location);
            return;


        }

        protected void filterPathCandidates(LocationPath location) {
            //if(location.path==null)
            //continue;
            if (location.OnlyFor != null && location.OnlyFor != Core.locations.platform_version)
                return;

            if (location.EV == EnvironmentVariable.InstallLocation ||
                location.EV == EnvironmentVariable.SteamCommon)
                return;

            // Adds user-friendly menu entries to the root selector
            switch (location.EV) {
                case EnvironmentVariable.SteamSourceMods:
                    if (Core.locations.steam_detected)
                        addPathCandidate(location);
                    break;
                case EnvironmentVariable.SteamUser:
                case EnvironmentVariable.SteamUserData:
                    if (Core.locations.steam_detected && Core.locations.getUsers(location.EV).Count > 0)
                        addPathCandidate(location);
                    break;
                case EnvironmentVariable.UbisoftSaveStorage:
                case EnvironmentVariable.FlashShared:
                    IEnumerable<string> paths = Core.locations.getPaths(location.EV);
                    foreach (string path in paths) {
                        LocationPath loc = Core.locations.interpretPath(path).getMostAccurateLocation();
                        loc.AppendPath(location.Path);
                        addPathCandidate(loc);
                    }
                    break;
                default:
                    addPathCandidate(location);
                    break;
            }
        }
        private static Queue<string> _argchives;
        public static Queue<string> ArgArchives {
            get {
                if(_argchives!=null)
                    return _argchives;

               _argchives = new Queue<string>();
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 0) {
                    foreach (string arg in args) {
                        if (!arg.StartsWith("-") && (arg.EndsWith(Core.Extension) || arg.EndsWith(Core.Extension + "\""))) {
                            _argchives.Enqueue(arg.Trim('\"'));
                            break;
                        }
                    }
                }
                return _argchives;
            }
        }

        protected override void doWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            base.doWork(sender, e);

            path_candidates = new SimpleModel<LocationPath>();
            user_candidates = new SimpleModel<string>();

            ProgressHandler.state = ProgressState.Indeterminate;
            try {
                if (archive == null) {
                }
            } catch (Exception ex) {
                TranslatingMessageHandler.SendException(ex);
            }


            if (ArgArchives.Count > 0)
                archive = new Archive(new FileInfo(ArgArchives.Dequeue()));

            if (archive == null)
                throw new TranslateableException("NoRestoreFileSelected");

            TranslatingProgressHandler.setTranslatedMessage("DetectingGameForRestoration");

            if (!archive.Exists)
                throw new TranslateableException("FileNotFound", archive.ArchivePath);

            GameID selected_game = archive.id.Game;
            string backup_owner = archive.id.Owner;
            string archive_type = archive.id.Type;


            try {
                game_data = Games.detectGame(selected_game);
            } catch (Exception ex) {
                TranslatingMessageHandler.SendException(ex);
            }

            NotifyPropertyChanged("GameNotDetected");


            if (game_data != null) {
                // This adds hypothetical locations
                foreach (LocationPath location in game_data.Locations.Paths) {
                    if (game_data.DetectionRequired)
                        break;

                        filterPathCandidates(location);
                }


                // This add already found locations
                foreach (DetectedLocationPathHolder location in game_data.DetectedLocations) {
                    location.IsSelected = true;
                    switch (location.EV) {
                        case EnvironmentVariable.ProgramFiles:
                        case EnvironmentVariable.ProgramFilesX86:
                            // This adds a fake VirtualStore folder, just in case
                            if (Core.locations.uac_enabled) {
                                DetectedLocationPathHolder temp = new DetectedLocationPathHolder(location, Core.locations.getFolder(EnvironmentVariable.LocalAppData, location.owner), location.owner);
                                temp.ReplacePath(Path.Combine("VirtualStore", location.full_dir_path.Substring(3)));
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
            }

            if(archive.id.OriginalEV != EnvironmentVariable.None &&
                archive.id.OriginalRelativePath!=null) {
                    LocationPath path = new LocationPath(archive.id.OriginalEV, archive.id.OriginalRelativePath);
                    filterPathCandidates(path);
            }

            if (archive.id.OriginalLocation != null) {
                DetectedLocations locs = Core.locations.interpretPath(archive.id.OriginalLocation);
                DetectedLocationPathHolder loc = locs.getMostAccurateLocation();
                if (loc != null) {
                    addPathCandidate(loc);
                }
            }


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
                    if (path.GetType() == typeof(DetectedLocationPathHolder)) {
                        DetectedLocationPathHolder det_path = path as DetectedLocationPathHolder;
                        if(candidate == null || (det_path.MatchesOriginalPath && det_path.Exists)) {
                            if (det_path.MatchesOriginalPath&&det_path.Exists) {
                                return det_path;
                            }
                            candidate = det_path;
                        }
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
            if (users.Count > 0 && location.EV != EnvironmentVariable.FlashShared) {
                user_needed = true;
                if (users.Count > 1) {
                    multiple_users = true;
                } else {
                    multiple_users = false;
                    NotifyPropertyChanged("only_user");
                }
                foreach (string user in users) {
                    user_candidates.AddWithSort(user);
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
                    case EnvironmentVariable.CommonApplicationData:
                    case EnvironmentVariable.UbisoftSaveStorage:
                    case EnvironmentVariable.FlashShared:
                        user_needed = false;
                        break;
                    default:
                        throw new TranslateableException("NoUsersForEV", location.EV.ToString());
                }

            }
        }

        public void cancel() {
            this.CancelAsync();
            if (restore_worker != null)
                restore_worker.CancelAsync();
            if(archive!=null)
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
            if( path is DetectedLocationPathHolder) {
                DetectedLocationPathHolder loc = path as DetectedLocationPathHolder;
                target = loc.full_dir_path;
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
