using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.ComponentModel;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Archive;
using Communication;
using Communication.Request;
using Communication.Message;
using MASGAU.Registry;
using System.Collections.ObjectModel;
using System.Globalization;
using Collections;
using MVC;
using Translator;
using Communication.Translator;
namespace MASGAU.Game {
    public class GameHandler: AModelItem<GameID>
    {
        #region Parsers
        public static GamePlatform parseGamePlatform(string parse_me) {
            switch(parse_me.ToLower()) {
                case "flash":
                    return GamePlatform.Flash;
                case "steam":
                    return GamePlatform.Steam;
                case "windows":
                    return GamePlatform.Windows;
                case "dos":
                    return GamePlatform.DOS;
                case "scummvm":
                    return GamePlatform.ScummVM;
                case "psp":
                    return GamePlatform.PSP;
                case "ps1":
                    return GamePlatform.PS1;
                case "ps2":
                    return GamePlatform.PS2;
                case "ps3":
                    return GamePlatform.PS3;
                case "linux":
                    return GamePlatform.Linux;
                case "osx":
                    return GamePlatform.OSX;
                case "multiple":
                    return GamePlatform.Multiple;
                default:
                    throw new NotImplementedException("Specified Game Platform " + parse_me.ToString() + " Is Not Supported");
            }

        }
        private static ArchiveType parseArchiveType(string parse_me) {
            switch(parse_me.ToLower()) {
                case "statistics":
                    return ArchiveType.Statistics;
                case "characters":
                    return ArchiveType.Characters;
                case "settings":
                    return ArchiveType.Settings;
                case "mods":
                    return ArchiveType.Mods;
                case "replays":
                    return ArchiveType.Replays;
                default:
                    throw new NotImplementedException("Specified Archive Type " + parse_me.ToString() + " Is Not Supported");
            }
        }
 

        #endregion



        // These are publicly visible items
	    public string title{ get; set; }
        public string name {
            get {
                return id.name;
            }
        }
        public string platform{
            get {
                return id.platform.ToString();
            }
        }
        public bool backup_enabled {
            get {
                return Core.settings.isGameBackupEnabled(id);
            }
            set {
                Core.settings.setGameBackupEnabled(this.id,value);
                NotifyPropertyChanged("backup_enabled");
            }
        }
        public bool sync_enabled {
            get {
                return Core.settings.isGameSyncEnabled(id);
            }
            set {
                bool temp = Core.settings.isGameSyncEnabled(id);
                if(value!=temp) {
                    Core.settings.setGameSyncEnabled(this.id,value);
                    Core.rebuild_sync = true;
                    NotifyPropertyChanged("sync_enabled");
                }
            }
        }

        public string tooltip {
            get {
                StringBuilder tooltip = new StringBuilder();
                if(comment!=null) {
                    tooltip.AppendLine(comment);
                    tooltip.AppendLine();
                }
                tooltip.AppendLine("Detected Locations:");
                tooltip.Append(_detected_paths_string);
                return tooltip.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            }
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
                foreach(KeyValuePair<string, DetectedLocationPathHolder> add_me in detected_locations) {
                    return_me.Add(add_me.Key);
                }
                return return_me;
            }
        }

        public void clearSyncPath() {
            Core.settings.clearDefaultGamePath(this.id);
        }

        public bool sync_available {
            get {
                switch(id.platform) {
                    case GamePlatform.PS1:
                    case GamePlatform.PS2:
                    case GamePlatform.PS3:
                    case GamePlatform.PSP:
                        return false;
                    default:
                        return true;
                }
            }
        }

        public bool multiple_detected_paths { 
            get{
                return detected_locations.Count>1;
            }
        }

        public bool has_sync_location {
            get {
                return sync_location!=null;
            }
        }

        public string sync_location {
            get {
                if(multiple_detected_paths) {
                    string path = Core.settings.getDefaultGamePath(this.id);
                    if(path!=null&&this.detected_locations.ContainsKey(path))
                        return path;
                    else {
                        Core.settings.clearDefaultGamePath(this.id);
                        return null;
                    }
                } else {
                    foreach(KeyValuePair<string, DetectedLocationPathHolder> add_me in detected_locations) {
                        return add_me.Key;
                    }
                }
                return null;
            }
            set {
                Core.settings.setDefaultGamePath(this.id,value);
                NotifyPropertyChanged("sync_location");
            }
        }

        private IdentifierHolder identifier;

        public bool override_virtualstore = false, detection_required = false, detection_completed = false;
        
        // Thse are the location datas loaded from the xml profile
        private List<ALocationHolder> locations {
            get {
                return _locations;
            }
        }

        private List<ALocationHolder> _locations = new List<ALocationHolder>();
        public List<LocationPathHolder>         location_paths {
            get {
                List<LocationPathHolder> return_me = new List<LocationPathHolder>();
                foreach(ALocationHolder location in locations) {
                    if(location.GetType()==typeof(LocationPathHolder))
                        return_me.Add(location as LocationPathHolder);
                }
                return return_me;
            }
        }


        public bool detected {
            get {
                return detected_locations!=null&&detected_locations.Count>0;
            }
        }

        // These are the save and ignore datas loaded from the xml profile
        private List<SaveHolder>   saves = new List<SaveHolder>();
        private List<IgnoreHolder> ignores =  new List<IgnoreHolder>();

        // These are the locations that have been found
        public Dictionary<string,DetectedLocationPathHolder> detected_locations;

        // This loads the names of the helpful people who contributed the paths to MASGAU
        public List<string> contributors;

        // This holds the name of the system the save is for


        private XmlElement xml;
	    public GameHandler(GameXMLHolder xml) {
            id = xml.id;
            this.xml = xml.xml;
        }
	    public GameHandler(GameID id): base(id) {
        }

        public string comment {
            get; protected set;
        }

        public string restore_comment {
            get; protected set;
        }

        public void detect() {
            contributors = new List<string>();
            identifier = new IdentifierHolder();

            foreach(XmlElement element in xml.ChildNodes) {
                ALocationHolder  location =  null;
                GenericHolder   holder =    null;
                PlayStationID   ps_id =     null;
                switch(element.Name) {
                    case "title":
                        title = element.InnerText;
                        break;
                    case "comment":
                        comment = element.InnerText;
                        break;
                    case "restore_comment":
                        restore_comment = element.InnerText;
                        break;
                    case "contributor":
                        contributors.Add(element.InnerText);
                        break;
					case "require_detection":
						detection_required = true;
						break;
                        // Location loaders
                    case "location_scummvm":
                        location = new ScummVMName(element);
                        break;
                    case "location_registry":
                        // Blanking out the new registry location
                        location = new LocationRegistryHolder(element);
                        break;
                    case "location_shortcut":
                        location = new LocationShortcutHolder(element);
                        break;
                    case "location_game":
                        location  = new LocationGameHolder(element);
                        break;
                    case "location_path":
                        location = new LocationPathHolder(element);
                        break;
                    case "save":
                        holder = new SaveHolder();
                        break;
                    case "ignore":
                        holder = new IgnoreHolder();
                        break;
                    case "identifier":
                        holder = new IdentifierHolder();
                        break;
                    case "ps_code":
                        switch(id.platform) {
                            case GamePlatform.PS1:
                                ps_id = new PlayStation1ID(element);
                                break;
                            case GamePlatform.PS2:
                                ps_id = new PlayStation2ID(element);
                                break;
                            case GamePlatform.PS3:
                                ps_id = new PlayStation3ID(element);
                                break;
                            case GamePlatform.PSP:
                                ps_id = new PlayStationPortableID(element);
                                break;
                            default:
                                throw new TranslateableException("ImproperPsCodeUse", this.id.ToString());
                        }
                        break;
                    case "virtualstore":
                        if(element.HasAttribute("override")&&element.GetAttribute("override")=="true")
                            override_virtualstore = true;
                        else
                            override_virtualstore = false;
                        break;
                }
                // Clever casting trickery to reduce code redundancy
                if(ps_id!=null) {
                    SaveHolder save = new SaveHolder();

                    if(ps_id.GetType()==typeof(PlayStationPortableID)||
                        ps_id.GetType()==typeof(PlayStation3ID)) {
                        save.path = ps_id.ToString();
                        save.name = null;
                    }
                    
                    if(ps_id.GetType()==typeof(PlayStation2ID)||ps_id.GetType()==typeof(PlayStation1ID)) {
                        save.path = null;
                        save.name = ps_id.ToString();
                        saves.Add(save);
                    }

                    if(ps_id.type!=null)
                        save.type = ps_id.type;

                    saves.Add(save);

                    locations.Add(ps_id);
                }
                if(holder!=null) {
                    if(element.HasAttribute("path"))
                        holder.path = element.GetAttribute("path");
                    else
                        holder.path = null;

                    if(element.HasAttribute("filename"))
                        holder.name = element.GetAttribute("filename");
                    else
                        holder.name = null;

                    if(element.HasAttribute("type"))
                        holder.type= element.GetAttribute("type");
                    else
                        holder.type = null;

                    if(element.HasAttribute("modified_after")) {
                        holder.modified_after = DateTime.Parse(element.GetAttribute("modified_after"));
                    }


                    if(holder.GetType()==typeof(SaveHolder))
                        saves.Add(holder as SaveHolder);

                    if(holder.GetType()==typeof(IgnoreHolder))
                        ignores.Add(holder as IgnoreHolder);

                    if(holder.GetType()==typeof(IdentifierHolder))
                        identifier = holder as IdentifierHolder;
                }
                if(location!=null) {
                    locations.Add(location);
                }

            }

            // Handy for stopping on a particular game ;)
            //if(title.StartsWith("American"))

            List<DetectedLocationPathHolder> interim = new List<DetectedLocationPathHolder>();
            GameHandler parent_game;

            string path = null;
            lock (locations)
            {

                foreach (ALocationHolder location in locations)
                {
                    // This skips if a location is marked as only being for a specific version of an OS
                    if (location.platform_version != Core.locations.platform_version && location.platform_version != PlatformVersion.All)
                        continue;

                    if (location.GetType() == typeof(LocationGameHolder))
                    {
                        // This checks all the locations that are based on other games
                        LocationGameHolder game = location as LocationGameHolder;
                        if (Core.games.all_games.containsId(game.game))
                        {
                            parent_game = Core.games.all_games.get(game.game);
                            // If the game hasn't been processed in the GamesHandler yetm it won't yield useful information, so we force it to process here
                            if (!parent_game.detection_completed)
                                parent_game.detect();
                            foreach (KeyValuePair<string, DetectedLocationPathHolder> check_me in parent_game.detected_locations)
                            {
                                path = location.modifyPath(check_me.Value.full_dir_path);
                                interim.AddRange(Core.locations.interpretPath(path));
                            }
                        }
                        else
                        {
                            TranslatingMessageHandler.SendError("ParentGameDoesntExist", game.game.ToString());
                        }
                    }
                    else
                    {
                        // This checks all the registry locations
                        // This checks all the shortcuts
                        // This parses each location supplied by the XML file
                        //if(title.StartsWith("Postal 2"))
                        //if(id.platform== GamePlatform.PS1)
                        interim.AddRange(Core.locations.getPaths(location));
                    }
                }
            }

		    detected_locations = new Dictionary<string,DetectedLocationPathHolder>();
            foreach (DetectedLocationPathHolder check_me in interim) {
                if(!detected_locations.ContainsKey(check_me.full_dir_path)) {
                    if(identifier.name!=null&&identifier.path!=null) {
                        foreach(DirectoryInfo directory in getPaths(check_me.full_dir_path,identifier.path)) {
                            if (directory.GetFiles(identifier.name).Length>0) {
                                detected_locations.Add(check_me.full_dir_path,check_me);
                                break;
                            }
                        }
                    } else if(identifier.path!=null) {
                        if(getPaths(check_me.full_dir_path,identifier.path).Count>0) {
                            detected_locations.Add(check_me.full_dir_path,check_me);
                        }
                    } else if(identifier.name!=null) {
                        if(new DirectoryInfo(check_me.full_dir_path).GetFiles(identifier.name).Length>0) {
                            detected_locations.Add(check_me.full_dir_path,check_me);
                        }
                    } else {
                        detected_locations.Add(check_me.full_dir_path,check_me);
                    }
                }
		    }
            detection_completed = true;
            _detected_paths_string = new StringBuilder(); ;
            bool first = true;
            foreach(KeyValuePair<string,DetectedLocationPathHolder> location in detected_locations) {
                _detected_paths_string.AppendLine(location.Key);
            }
        }

        private static List<DirectoryInfo> getPaths(string root, string path) {
            List<DirectoryInfo> return_me = new List<DirectoryInfo>();
            DirectoryInfo root_directory = new DirectoryInfo(root);

            string[] split = path.Split(Path.DirectorySeparatorChar);
            string forward_me = "";

            for(int i=1;i<split.Length;i++) {
                forward_me = Path.Combine(forward_me,split[i]);
            }
            try {
                DirectoryInfo[] directories = root_directory.GetDirectories(split[0]);
                if(split.Length==1) {
                    foreach(DirectoryInfo add_me in directories) {
                        return_me.Add(add_me);
                    }
                } else {
                    foreach(DirectoryInfo add_me in directories) {
                        return_me.AddRange(getPaths(add_me.FullName,forward_me));
                    }
                }
            } catch (Exception e) {
                TranslatingMessageHandler.SendError("ReadError",e, root);
            }
            return return_me;
        }

        private static List<string> gatherFiles(string root) {
            List<string> return_me = new List<string>();
            try {
                foreach(FileInfo file in new DirectoryInfo(root).GetFiles()) {
                    return_me.Add(file.FullName);
                }
                foreach(DirectoryInfo sub_folder in new DirectoryInfo(root).GetDirectories()) {
                    return_me.AddRange(gatherFiles(sub_folder.FullName));
                }
            } catch (UnauthorizedAccessException e) {
                throw e;
            } catch (Exception e) {
                TranslatingMessageHandler.SendError("FileGatherError", e, root);
            }
            return return_me;
        }

        public DetectedFiles getSaves() {
            DetectedFiles files = new DetectedFiles();
            DetectedFiles remove_us = new DetectedFiles();

            foreach(KeyValuePair<string,DetectedLocationPathHolder> location in detected_locations) {
                foreach (SaveHolder save in saves) {
                    files.AddRange(findTheseFiles(location.Value,save));
                }
                foreach(IgnoreHolder ignore in ignores) {
                    remove_us.AddRange(findTheseFiles(location.Value,ignore));
                }
            }

            files.Remove(remove_us);

            return files;
        }

        private static List<DetectedFile> findTheseFilesHelper(DetectedLocationPathHolder location, List<string> files, GenericHolder identifier) {
            List<DetectedFile> return_me = new List<DetectedFile>();
            DetectedFile add_me;
            foreach(string file_name in files) {
                FileInfo file = new FileInfo(file_name);
                if(!file.Exists)
                    continue;

                if(file.LastWriteTime<=identifier.modified_after)
                    continue;

                add_me = new DetectedFile(location, identifier.type);


                if(file.DirectoryName.Length==add_me.abs_root.Length)
                    add_me.path = "";
                else 
                    add_me.path = file.DirectoryName.Substring(add_me.abs_root.Trim(Path.DirectorySeparatorChar).Length+1);

                add_me.name = file.Name;


                return_me.Add(add_me);
            }
            return return_me;
        }

        private static List<DetectedFile> findTheseFiles(DetectedLocationPathHolder location, GenericHolder holder) {
		    List<DirectoryInfo> directories = new List<DirectoryInfo>();
            List<DetectedFile> return_me = new List<DetectedFile>();
            if(holder.name==null) {  
				if(holder.path==null) {
					if(Directory.Exists(location.full_dir_path)) {
                        return_me.AddRange(findTheseFilesHelper(location, gatherFiles(location.full_dir_path), holder));
					}
				} else {
					directories.AddRange(getPaths(location.full_dir_path,holder.path));
					foreach(DirectoryInfo directory in directories) {
                        return_me.AddRange(findTheseFilesHelper(location, gatherFiles(directory.FullName), holder));
					}
				}
            } else if(holder.path==null) {
                if(Directory.Exists(location.full_dir_path)) {
                    List<string> files = new List<string>();
                    foreach(FileInfo read_me in new DirectoryInfo(location.full_dir_path).GetFiles(holder.name)) {
                        files.Add(read_me.FullName);
                    }
                    return_me.AddRange(findTheseFilesHelper(location, files,holder));
                }
            } else {
				directories.AddRange(getPaths(location.full_dir_path,holder.path));
				foreach(DirectoryInfo directory in directories) {
                    List<string> files = new List<string>();
                    foreach(FileInfo read_me in directory.GetFiles(holder.name)) {
                        files.Add(read_me.FullName);
                    }
                    return_me.AddRange(findTheseFilesHelper(location, files, holder));
                }
            } 
            return return_me;
        }

        public List<DetectedFile> checkThis(string this_right_here) {
            List<DetectedFile> return_me = new List<DetectedFile>();
            string compare;

            foreach (DetectedFile check_me in this.getSaves().Flatten()) {
                compare = check_me.full_file_path;
                if (compare == this_right_here)
                    return_me.Add(check_me);
            }

            return return_me;
        }

        #region Purging Methods
        public bool purgeRoot() {
            List<string> options = new List<string>();
            options.Add("Purge All Detected Roots");
            foreach(KeyValuePair<string,DetectedLocationPathHolder> root in detected_locations) {
                if(!options.Contains((Path.Combine(root.Value.abs_root,root.Value.path))))
                    options.Add(Path.Combine(root.Value.abs_root,root.Value.path));
            }

            if(options.Count>2) {
                RequestReply info = TranslatingRequestHandler.Request(RequestType.Choice, "PurgeRootChoice", options[0], options);
                if(info.cancelled) {
                    return false;
                } 
                if(info.selected_index==0) {
                    foreach(KeyValuePair<string,DetectedLocationPathHolder> delete_me in detected_locations) {
                        delete_me.Value.delete();
                    }
                } else {
                    detected_locations[info.selected_option].delete();
                }
            } else {
                detected_locations[options[1]].delete();
            }
            return true;
        }
        #endregion

    }
}