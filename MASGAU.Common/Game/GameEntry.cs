using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using GameSaveInfo;
using MASGAU.Backup;
using MASGAU.Location.Holders;
using MASGAU.Monitor;
using MVC;
using MVC.Communication;
using MVC.Translator;
using Translator;
namespace MASGAU {
    public class GameEntry : AModelItem<GameID> {
        public GameSaveInfo.Game Game {
            get {
                return version.Game;
            }
        }
        protected GameVersion version;

        public Locations Locations {
            get {
                return version.Locations;
            }
        }
        public bool DetectionRequired {
            get {
                return version.DetectionRequired;
            }
        }
        public string Comment {
            get {
                return version.Comment;
            }
        }
        public string RestoreComment {
            get {
                return version.RestoreComment;
            }
        }
        public string Name {
            get {
                return version.ID.Name;
            }
        }
        public string Title {
            get {
                return version.Title;
            }
        }

        public bool Linkable {
            get {
                return version.Links.Count > 0;
            }
        }
        public bool? IsLinked {
            get {
                if (version.Links.Count == 0)
                    return false;

                foreach (Link link in version.Links) {
                    if (!link.Linked)
                        return false;
                }
                return true;
            }
            set {
                foreach (Link link in version.Links) {
                    link.Linked = value == true;
                }
                NotifyPropertyChanged("LinkToolTip");
            }
        }
        public string LinkToolTip {
            get {
                switch (IsLinked) {
                    case true:
                        return Strings.GetToolTipString("IsLinked", DetectedLocations.Count.ToString());
                    case false:
                        return Strings.GetToolTipString("ClickToLink");
                    case null:
                        return Strings.GetToolTipString("PartiallyLinked", "3", DetectedLocations.Count.ToString());
                }
                return "";
            }
        }

        public string TitleAddendum {
            get {
                return id.Formatted;
            }
        }

        public override System.Drawing.Color BackgroundColor {
            get {
                return id.BackgroundColor;
            }
        }

        public override System.Drawing.Color SelectedColor {
            get {
                return id.SelectedColor;
            }
        }

        public string SourceFile {
            get {
                return this.version.SourceFile.File.Name;
            }
        }

        public override string ToolTip {
            get {
                StringBuilder tooltip = new StringBuilder();
                if (version.Comment != null) {
                    tooltip.AppendLine(version.Comment);
                    tooltip.AppendLine();
                }
                tooltip.AppendLine("Detected Locations:");
                tooltip.Append(_detected_paths_string);
                return tooltip.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            }
        }


        public List<ALocation> AllLocations {
            get {
                List<ALocation> locs = new List<ALocation>();
                locs.AddRange(version.AllLocations);
                locs.AddRange(version.PlayStationIDs);
                locs.AddRange(version.ScummVMs);
                return locs;
            }
        }



        // These are the locations that have been found
        public MASGAU.Location.DetectedLocations DetectedLocations;


        public bool IsDeprecated {
            get {
                return version.IsDeprecated;
            }
        }

        private const int MaxDetect = 300;
        //        private static int DetectCount = 0;
        Random rnd = new Random();

        private bool detect_override = false;

        public bool DetectionAttempted { get; protected set; }
        public bool IsDetected {
            get {
                if (detect_override)
                    return true;

                if (IsDeprecated)
                    return false;

                if (DetectedLocations != null) {
                    if (DetectedLocations.Count > 0) {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool CanBeMonitored {
            get {
                return id.OS == null || (!id.OS.StartsWith("PS") && id.OS != "Android");
            }
        }

        public bool IsMonitored {
            get {
                return MonitorEnabled && IsDetected && CanBeMonitored;
            }
            set {
                MonitorEnabled = value;
            }
        }

        public bool MonitorEnabled {
            get {
                return Core.settings.isGameMonitored(id);
            }
            set {
                if (!CanBeMonitored)
                    return;
                if (value) {
                    if (!CheckBackuptPathForMonitor()) {
                        return;
                    }
                }

                Core.settings.setGameMonitored(this.id, value);
                NotifyPropertyChanged("MonitorEnabled");
                if (value)
                    startMonitoring();
                else
                    stopMonitoring();
            }
        }


        public bool CheckBackuptPathForMonitor() {

            if (!Core.settings.IsBackupPathSet) {
                RequestReply reply = RequestHandler.Request(RequestType.BackupFolder, false);
                if (reply.Cancelled) {
                    TranslatingMessageHandler.SendWarning("MonitorNeedsBackupPath");
                    return false;
                    //throw new TranslateableException("BackupPathNotSet");
                }
            }
            return true;
        }

        private StringBuilder _detected_paths_string;
        public string detected_paths_string {
            get {
                return _detected_paths_string.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            }
        }

        public ObservableCollection<string> detected_location_list {
            get {
                ObservableCollection<string> return_me = new ObservableCollection<string>();
                foreach (DetectedLocationPathHolder add_me in DetectedLocations) {
                    return_me.Add(add_me.full_dir_path);
                }
                return return_me;
            }
        }

        public GameEntry(GameVersion version) {
            this.version = version;
            this.id = new GameID(version.ID);
            DetectionAttempted = false;
        }


        public bool Detect() {
            //            detect_override =  rnd.Next(0, 2) == 0;


            List<DetectedLocationPathHolder> interim = new List<DetectedLocationPathHolder>();
            List<ALocation> locations = AllLocations;

            if (this.id.OS != null && this.id.OS.StartsWith("PS"))
                System.Console.Out.Write("");

            foreach (ALocation location in locations) {
                // This skips if a location is marked as only being for a specific version of an OS
                if (location.OnlyFor != Core.locations.platform_version && location.OnlyFor != null)
                    continue;

                if (location.GetType() == typeof(LocationParent)) {
                    // This checks all the locations that are based on other games
                    LocationParent game = location as LocationParent;
                    if (Games.Contains(game.game)) {
                        GameEntry parent_game = Games.Get(game.game);
                        // If the game hasn't been processed in the GamesHandler yetm it won't yield useful information, so we force it to process here
                        if (!parent_game.DetectionAttempted)
                            parent_game.Detect();
                        foreach (DetectedLocationPathHolder check_me in parent_game.DetectedLocations) {
                            string path = location.modifyPath(check_me.full_dir_path);
                            interim.AddRange(Core.locations.interpretPath(path));
                        }
                    } else {
                        TranslatingMessageHandler.SendError("ParentGameDoesntExist", game.game.ToString());
                    }
                } else {
                    // This checks all the registry locations
                    // This checks all the shortcuts
                    // This parses each location supplied by the XML file
                    //if(title.StartsWith("Postal 2"))
                    //if(id.platform== GamePlatform.PS1)
                    interim.AddRange(Core.locations.getPaths(location));
                }
            }


            DetectedLocations = new Location.DetectedLocations();
            foreach (DetectedLocationPathHolder check_me in interim) {
                if (version.Identifiers.Count == 0) {
                    DetectedLocations.Add(check_me);
                    continue;
                }
                foreach (Identifier identifier in version.Identifiers) {
                    if (identifier.FindMatching(check_me.full_dir_path).Count > 0) {
                        DetectedLocations.Add(check_me);
                        break;
                    }
                }
            }
            _detected_paths_string = new StringBuilder();

            foreach (DetectedLocationPathHolder location in DetectedLocations) {
                _detected_paths_string.AppendLine(location.full_dir_path);
            }

            NotifyPropertyChanged("IsDetected");
            NotifyPropertyChanged("IsMonitored");
            DetectionAttempted = true;

            return IsDetected;
        }

        public DetectedFiles Saves {
            get {
                DetectedFiles files = new DetectedFiles();
                foreach (DetectedLocationPathHolder location in DetectedLocations) {
                    foreach (FileType type in version.FileTypes.Values) {
                        files.AddFiles(type, location);
                    }
                    foreach (APlayStationID id in version.PlayStationIDs) {
                        Include save = id.convertToInclude();
                        files.AddFiles(save, location);
                    }
                    foreach (ScummVM scumm in version.ScummVMs) {
                        Include save = new Include(scumm.Name + "*.cfg", null, "Settings");
                        files.AddFiles(save, location);
                        save = new Include(scumm.Name + "*.???", null, null);
                        files.AddFiles(save, location, new System.Text.RegularExpressions.Regex(".*[.][0-9]{3}$"));
                    }
                }
                return files;
            }
        }


        public IList<Archive> Archives {
            get {
                return MASGAU.Archives.GetArchives(this.id);
            }
        }


        #region monitoring methods
        private Stack<MonitorPath> monitors = new Stack<MonitorPath>();
        BackgroundWorker monitor_setup;
        public void startMonitoring() {
            MVC.Communication.Interface.InterfaceHandler.disableInterface();
            monitor_setup = new BackgroundWorker();
            monitor_setup.DoWork += new System.ComponentModel.DoWorkEventHandler(startMonitoring);
            monitor_setup.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(monitor_setup_RunWorkerCompleted);
            monitor_setup.RunWorkerAsync();
        }

        void monitor_setup_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            MVC.Communication.Interface.InterfaceHandler.enableInterface();
        }

        public void startMonitoring(object sender, System.ComponentModel.DoWorkEventArgs e) {
            if (monitors.Count > 0)
                stopMonitoring();

            if (!IsMonitored)
                return;


            if (!CheckBackuptPathForMonitor())
                return;

            TranslatingProgressHandler.setTranslatedMessage("BackupUpForMonitor", this.Title);

            ProgressHandler.suppress_communication = true;

            BackupProgramHandler backup = new BackupProgramHandler(this, null, null, Core.locations);
            backup.RunWorkerAsync();
            while (backup.IsBusy) {
                Thread.Sleep(100);
            }


            TranslatingProgressHandler.setTranslatedMessage("SettingUpMonitorFor", this.Title);

            ProgressHandler.suppress_communication = false;


            foreach (DetectedLocationPathHolder path in this.DetectedLocations.Values) {
                MonitorPath mon = new MonitorPath(this, path);
                monitors.Push(mon);
                mon.start();
            }

        }


        public void stopMonitoring() {
            while (monitors.Count > 0) {
                MonitorPath path = monitors.Pop();
                path.stop();
                path.Dispose();
            }
        }
        #endregion


        #region Purging Methods
        public bool purge(bool include_archives) {
            List<string> options = new List<string>();
            string all_option = Strings.GetLabelString("PurgeAllRoots");
            string question;
            if (include_archives)
                question = "PurgeGameAndArchivesQuestion";
            else
                question = "PurgeJustGameQuestion";

            options.Add(all_option);
            foreach (DetectedLocationPathHolder root in DetectedLocations) {
                string path;
                if (root.Path == null)
                    path = root.AbsoluteRoot;
                else
                    path = Path.Combine(root.AbsoluteRoot, root.Path);
                if (!options.Contains(path))
                    options.Add(path);
            }

            RequestReply info = TranslatingRequestHandler.Request(RequestType.Choice, question, options[0], options, this.Name);
            if (info.Cancelled) {
                return false;
            }
            if (info.SelectedIndex == 0) {
                foreach (DetectedLocationPathHolder delete_me in DetectedLocations) {
                    delete_me.delete();
                }
            } else {
                DetectedLocations[info.SelectedOption].delete();
            }
            return true;
        }
        #endregion


        public List<DetectedFile> GetSavesMatching(string this_right_here) {
            List<DetectedFile> return_me = new List<DetectedFile>();
            string compare;

            foreach (DetectedFile check_me in this.Saves.Flatten()) {
                compare = check_me.full_file_path;
                if (compare == this_right_here)
                    return_me.Add(check_me);
            }

            return return_me;
        }

    }
}
