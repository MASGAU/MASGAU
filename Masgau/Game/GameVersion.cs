using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading;
using MVC;
using MASGAU.Monitor;
using MASGAU.Location.Holders;
using MASGAU.Backup;
using MVC.Communication;
using MVC.Translator;
using XmlData;
using Translator;
namespace MASGAU {
    public struct Locations {
        public List<LocationPathHolder> Paths;
        public List<LocationRegistryHolder> Registries;
        public List<LocationShortcutHolder> Shortcuts;
        public List<LocationGameHolder> Games;
        public List<PlayStationID> PlayStationIDs;
        public List<ScummVMHolder> ScummVMs;

        public List<ALocationHolder> All {
            get {
                List<ALocationHolder> return_me = new List<ALocationHolder>();
                return_me.AddRange(Paths);
                return_me.AddRange(Registries);
                return_me.AddRange(Shortcuts);
                return_me.AddRange(Games);
                return_me.AddRange(PlayStationIDs);
                return_me.AddRange(ScummVMs);
                return return_me;
            }
        }
    }
    public class GameVersion : AModelItem<GameID> {
        public Game Game { get; protected set; }

        private List<GameLink> links = new List<GameLink>();
        public bool Linkable {
            get {
                if (Core.locations.platform_version == "WindowsXP")
                    return false;
                return links.Count > 0;
            }
        }
        public bool? IsLinked {
            get {
                if (links.Count == 0)
                    return false;

                foreach (GameLink link in links) {
                    if (!link.Linked)
                        return false;
                }
                return true;
            }
            set {
                foreach (GameLink link in links) {
                    link.Linked = value==true;
                }
                NotifyPropertyChanged("LinkToolTip");
            }
        }
        public string LinkToolTip {
            get {
                switch (IsLinked) {
                    case true:
                        return Strings.GetToolTipString("IsLinked",DetectedLocations.Count.ToString());
                    case false:
                        return Strings.GetToolTipString("ClickToLink");
                    case null:
                        return Strings.GetToolTipString("PartiallyLinked", "3", DetectedLocations.Count.ToString());
                }
                return "";
            }
        }

        protected string _title = null;
        public string Title {
            get {
                if (_title == null)
                    return Game.Title;
                return _title;
            }
        }
        public string TitleAddendum {
            get {
                return id.Formatted;
            }
        }

        public System.Drawing.Color BackgroundColor {
            get {
                return id.BackgroundColor;
            }
        }

        public System.Drawing.Color SelectedColor {
            get {
                return id.SelectedColor;
            }
        }


        public string ToolTip {
            get {
                StringBuilder tooltip = new StringBuilder();
                if (Comment != null) {
                    tooltip.AppendLine(Comment);
                    tooltip.AppendLine();
                }
                tooltip.AppendLine("Detected Locations:");
                tooltip.Append(_detected_paths_string);
                return tooltip.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            }
        }


        protected Dictionary<string,FileTypeHolder> FileTypes = new  Dictionary<string,FileTypeHolder>();

        public Locations Locations;

        public List<string> Contributors = new List<string>();
        public string Comment { get; protected set; }
        public string RestoreComment { get; protected set; }

        // These are the locations that have been found
        public Dictionary<string, DetectedLocationPathHolder> DetectedLocations;

        public bool IgnoreVirtualStore { get; protected set; }


        public bool IsDeprecated { get; protected set; }
        public bool DetectionRequired { get; protected set; }
        public bool DetectionAttempted { get; protected set; }
        public bool IsDetected {
            get {
                if ((Game!=null&&Game.IsDeprecated)||this.IsDeprecated)
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
                return id.OS==null||(!id.OS.StartsWith("PS")&&id.OS!="Android");
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

                Core.settings.setGameMonitored(this.id, value);
                NotifyPropertyChanged("MonitorEnabled");
                if(value)
                    startMonitoring();
                else
                    stopMonitoring();
            }
        }

        private List<IdentifierHolder> Identifiers = new List<IdentifierHolder>();

        private StringBuilder _detected_paths_string;
        public string detected_paths_string {
            get {
                return _detected_paths_string.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            }
        }

        public ObservableCollection<string> detected_location_list {
            get {
                ObservableCollection<string> return_me = new ObservableCollection<string>();
                foreach (KeyValuePair<string, DetectedLocationPathHolder> add_me in DetectedLocations) {
                    return_me.Add(add_me.Key);
                }
                return return_me;
            }
        }

        protected GameVersion(Game parent) {
            this.Game = parent;
            Locations.Paths = new List<LocationPathHolder>();
            Locations.Registries = new List<LocationRegistryHolder>();
            Locations.Shortcuts = new List<LocationShortcutHolder>();
            Locations.Games = new List<LocationGameHolder>();
            Locations.PlayStationIDs = new List<PlayStationID>();
            Locations.ScummVMs = new List<ScummVMHolder>();
        }

        public XmlElement createXml() {
            // This outputs little more than what's necessary to create a custom game entry
            // Once in the file, the xml wil not need to be re-generated, so it won't need to be outputted again
            // This way manual updates to the xml file won't be lost ;)
            if (xml != null)
                return xml;


            xml = Game.createElement("version");

            id.AddAttributes(xml);
            if(xml.HasAttribute("name"))
                xml.RemoveAttribute("name");

            XmlElement locations = Game.createElement("locations");

            foreach(LocationPathHolder path in Locations.Paths) {
                XmlElement xp = path.createXml(Game);
                locations.AppendChild(xp);
            }
            xml.AppendChild(locations);

            foreach(FileTypeHolder type in FileTypes.Values) {
                XmlElement ft = type.createXml(Game);
                this.xml.AppendChild(ft);
            }

            foreach (string con in Contributors) {
                this.xml.AppendChild(Game.createElement("contributor", con));
            }

            if(Comment!=null)
                this.xml.AppendChild(Game.createElement("comment", Comment));
            if(RestoreComment!=null)
                this.xml.AppendChild(Game.createElement("restore_comment", RestoreComment));


            string output = this.xml.OuterXml;

            return xml;
        }

        protected XmlElement xml = null;
        public GameVersion(Game parent, XmlElement element): this(parent) {
            xml = element;
            DetectionAttempted = false;
            DetectionRequired = false;

            this.id = new GameID(parent.Name, element);

            foreach (XmlAttribute attrib in element.Attributes) {
                if (GameID.attributes.Contains(attrib.Name))
                    continue;
                switch (attrib.Name) {
                    case "virtualstore":
                        if (attrib.Value == "ignore")
                            IgnoreVirtualStore = true;
                        break;
                    case "detect":
                        if (attrib.Value == "required")
                            DetectionRequired = true;
                        break;
                    case "deprecated":
                        IsDeprecated = Boolean.Parse(attrib.Value);
                        break;
                    default:
                        throw new NotSupportedException(attrib.Name);
                }
            }
            links.Clear();
            foreach (XmlElement sub in element.ChildNodes) {
                switch (sub.Name) {
                    case "title":
                        _title = sub.InnerText;
                        break;
                    case "locations":
                        LoadLocations(sub);
                        break;
                    case "files":
                        FileTypeHolder type = new FileTypeHolder(sub);
                        FileTypes.Add(type.Type,type);
                        break;
                    case "ps_code":
                        LoadPlayStation(sub);
                        break;
                    case "contributor":
                        Contributors.Add(sub.InnerText);
                        break;
                    case "comment":
                        Comment = sub.InnerText;
                        break;
                    case "restore_comment":
                        RestoreComment = sub.InnerText;
                        break;
                    case "identifier":
                        Identifiers.Add(new IdentifierHolder(sub));
                        break;
                    case "scummvm":
                        LoadScummVM(sub);
                        break;
                    case "linkable":
                        GameLink link = new GameLink(this, sub);
                        links.Add(link);
                        break;
                    default:
                        throw new NotSupportedException(sub.Name);
                }
            }
        }

        private void LoadScummVM(XmlElement element) {
            Locations.ScummVMs.Add(new ScummVMHolder(element));
        }

        private void LoadPlayStation(XmlElement element) {
            PlayStationID id;
            switch (this.id.OS) {
                case "PS1":
                    id = new PlayStation1ID(element);
                    break;
                case "PS2":
                    id = new PlayStation2ID(element);
                    break;
                case "PS3":
                    id = new PlayStation3ID(element);
                    break;
                case "PSP":
                    id = new PlayStationPortableID(element);
                    break;
                default:
                    throw new NotSupportedException(this.id.OS);
            }

            SaveHolder save;

            if (id.GetType() == typeof(PlayStationPortableID) ||
                id.GetType() == typeof(PlayStation3ID)) {
                    save = new SaveHolder(id.ToString(), null,id.type);
            } else if (id.GetType() == typeof(PlayStation2ID) || id.GetType() == typeof(PlayStation1ID)) {
                save = new SaveHolder(null, id.ToString(), id.type);
            } else {
                throw new NotSupportedException(id.GetType().ToString());
            }

            if(!FileTypes.ContainsKey(id.type)) {
                FileTypes.Add(id.type, new FileTypeHolder(id.type));
            }
            FileTypes[id.type].Add(save);

            Locations.PlayStationIDs.Add(id);
        }

        private void LoadLocations(XmlElement element) {
            foreach (XmlElement sub in element.ChildNodes) {
                switch (sub.Name) {
                    case "path":
                        Locations.Paths.Add(new LocationPathHolder(sub));
                        break;
                    case "registry":
                        Locations.Registries.Add(new LocationRegistryHolder(sub));
                        break;
                    case "shortcut":
                        Locations.Shortcuts.Add(new LocationShortcutHolder(sub));
                        break;
                    case "parent":
                        Locations.Games.Add(new LocationGameHolder(sub));
                        break;
                    default:
                        throw new NotSupportedException(sub.Name);
                }
            }
        }

        public bool Detect() {
            List<DetectedLocationPathHolder> interim = new List<DetectedLocationPathHolder>();
            List<ALocationHolder> locations = Locations.All;

            foreach (ALocationHolder location in locations) {
                // This skips if a location is marked as only being for a specific version of an OS
                if (location.OnlyFor != Core.locations.platform_version && location.OnlyFor != null)
                    continue;

                if (location.GetType() == typeof(LocationGameHolder)) {
                    // This checks all the locations that are based on other games
                    LocationGameHolder game = location as LocationGameHolder;
                    if (Games.Contains(game.game)) {
                        GameVersion parent_game = Games.Get(game.game);
                        // If the game hasn't been processed in the GamesHandler yetm it won't yield useful information, so we force it to process here
                        if (!parent_game.DetectionAttempted)
                            parent_game.Detect();
                        foreach (KeyValuePair<string, DetectedLocationPathHolder> check_me in parent_game.DetectedLocations) {
                            string path = location.modifyPath(check_me.Value.full_dir_path);
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


            DetectedLocations = new Dictionary<string, DetectedLocationPathHolder>();
            foreach (DetectedLocationPathHolder check_me in interim) {
                if (!DetectedLocations.ContainsKey(check_me.full_dir_path)) {
                    if (Identifiers.Count == 0) {
                        DetectedLocations.Add(check_me.full_dir_path, check_me);
                        continue;
                    }
                    foreach (IdentifierHolder identifier in Identifiers) {
                        if (identifier.FindMatching(check_me).Count > 0) {
                            DetectedLocations.Add(check_me.full_dir_path, check_me);
                            break;
                        }
                    }
                }
            }
            _detected_paths_string = new StringBuilder();

            foreach (KeyValuePair<string, DetectedLocationPathHolder> location in DetectedLocations) {
                _detected_paths_string.AppendLine(location.Key);
            }
            NotifyPropertyChanged("IsDetected");
            NotifyPropertyChanged("IsMonitored");
            DetectionAttempted = true;

            return IsDetected;
        }


        public DetectedFiles Saves {
            get {
                DetectedFiles files = new DetectedFiles();
                foreach (KeyValuePair<string, DetectedLocationPathHolder> location in DetectedLocations) {
                    foreach (FileTypeHolder save in FileTypes.Values) {
                        files.AddRange(save.FindMatching(location.Value));
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


            TranslatingProgressHandler.setTranslatedMessage("BackupUpForMonitor", this.Title);

            ProgressHandler.suppress_communication = true;

            BackupProgramHandler backup = new BackupProgramHandler(this,null,null,Core.locations);
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
            while(monitors.Count>0) {
                MonitorPath  path = monitors.Pop();
                path.stop();
                path.Dispose();
            }
        }
        #endregion


        #region Purging Methods
        public bool purgeRoot() {
            List<string> options = new List<string>();
            options.Add("Purge All Detected Roots");
            foreach (KeyValuePair<string, DetectedLocationPathHolder> root in DetectedLocations) {
                if (!options.Contains((Path.Combine(root.Value.AbsoluteRoot, root.Value.Path))))
                    options.Add(Path.Combine(root.Value.AbsoluteRoot, root.Value.Path));
            }

            if (options.Count > 2) {
                RequestReply info = TranslatingRequestHandler.Request(RequestType.Choice, "PurgeRootChoice", options[0], options);
                if (info.Cancelled) {
                    return false;
                }
                if (info.SelectedIndex == 0) {
                    foreach (KeyValuePair<string, DetectedLocationPathHolder> delete_me in DetectedLocations) {
                        delete_me.Value.delete();
                    }
                } else {
                    DetectedLocations[info.SelectedOption].delete();
                }
            } else {
                DetectedLocations[options[1]].delete();
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
