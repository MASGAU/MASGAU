using System.IO;
using System;

namespace MASGAU.LocationHandlers {
    public abstract class LocationHolder: ModelItem<NullID> {


        public abstract override int CompareTo(object comparable);


        // Used to add or remove path elements
        private string _append_path = null, _detract_path = null;
        public string append_path {
            set {
                _append_path = value;
            }
            get {
                return _append_path;
            }
        }
        public string detract_path {
            set {
                _detract_path = value;
            }
            get {
                return _detract_path;
            }
        }
        private bool _read_only = false;
        public bool read_only {
            set {
                _read_only = value;
            }
            get {
                return _read_only;
            }
        }

        // Used to filter user locations by windows versions and language
        public string language = null;
        // This receives a path and modifies it based on the object's append and detract settings
        public string modifyPath(string path) {
            path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (_detract_path!= null) {
                if(path.EndsWith(_detract_path))
                    path = path.Substring(0,path.Length-_detract_path.Length);
            }
            if (_append_path != null)
                path = Path.Combine(path,_append_path);
            return path.Trim(Path.DirectorySeparatorChar);
        }
        public bool override_virtual_store = false;
        public PlatformVersion platform_version = PlatformVersion.All;

        public LocationHolder() {}

        public LocationHolder(LocationHolder copy_me) {
            append_path = copy_me._append_path;
            detract_path = copy_me._detract_path;
            language = copy_me.language;
            platform_version = copy_me.platform_version;
        }


    }

    public class ManualLocationPathHolder: LocationPathHolder {
        string manual_path;
        public ManualLocationPathHolder(string manual_path) {
            this.manual_path = manual_path;
            this.override_virtual_store = true;
        }
        public override string ToString()
        {
            return manual_path;
        }
    }

    public class PlayStationID: LocationPathHolder{
        public string prefix, suffix;
    }

    public class LocationGameHolder: LocationHolder {
        // Used when dealing with a game root
        public GameID game;

        public override int CompareTo(object comparable)
        {
            LocationGameHolder location = (LocationGameHolder)comparable;
            return game.CompareTo(location.game);
        }

    }

    public class LocationPathHolder: LocationHolder {
        // Used when dealing with a path
        // ONLY holds the name of the environment variable or wahtever used to figure out the root
        public EnvironmentVariable rel_root;

        // Holds only the relative path from the root
        public string path;

        public LocationPathHolder() {}

        public override int CompareTo(object comparable)
        {
            LocationPathHolder location = (LocationPathHolder)comparable;
            int result = compare(this.rel_root,location.rel_root);
            if(result==0)
                result = compare(this.path,location.path);

            return result;
        }


        public override string ToString() {
            return Path.Combine(rel_root.ToString(),path);
        }

        public LocationPathHolder(LocationPathHolder copy_me): base(copy_me) {
            rel_root = copy_me.rel_root;
            path = copy_me.path;
        }

    }

    // This holds locations that have been found
    public class DetectedLocationPathHolder: LocationPathHolder {
        public DetectedLocationPathHolder(LocationPathHolder path) {
            this.append_path = path.append_path;
            this.detract_path = path.detract_path;
            this.IsEnabled = path.IsEnabled;
            this.IsExpanded = path.IsExpanded;
            this.IsSelected = path.IsSelected;
            this.language = path.language;
            this.platform_version = path.platform_version;
            this.path = path.path;
            this.read_only = path.read_only;
            this.rel_root = path.rel_root;
        }

        protected DetectedLocationPathHolder() {
        }

        // Holds the actual, interpreted root location
        public string abs_root;
        // Holds the associated user for this folder
        public string owner;
        // Gets the full absolute path of the folfer
        public string full_dir_path {
            get {
                if(abs_root!=null&&abs_root!="") {
                    if(path==null||path=="") {
                        return abs_root;
                    } else {
                        return Path.Combine(abs_root,path);
                    }
                } else {
                    return null;
                }
            }
        }

        public bool exists {
            get {
                return Directory.Exists(full_dir_path);
            }
        }

        public override string ToString() {
            string return_me;
            if(rel_root== EnvironmentVariable.AllUsersProfile||
                rel_root== EnvironmentVariable.AltSavePaths||
                rel_root== EnvironmentVariable.Drive||
                rel_root== EnvironmentVariable.InstallLocation||
                rel_root== EnvironmentVariable.ProgramFiles||
                rel_root== EnvironmentVariable.ProgramFilesX86||
                rel_root== EnvironmentVariable.Public||
                rel_root== EnvironmentVariable.SteamCommon||
                rel_root== EnvironmentVariable.SteamSourceMods)
                return_me = abs_root;
            else
                return_me = rel_root.ToString();

            if(path!=null)
                return_me = Path.Combine(return_me,path);

            return return_me;
        }
        
        public void delete() {
            try {
                DirectoryInfo info = new DirectoryInfo(full_dir_path);
                if(info.Exists) {
                    info.Attributes = FileAttributes.Normal;
                    info.Delete(true); 
                }
            } catch (Exception e) {
                throw new MException("Delete Error","Error while trying to delete this:\n" + full_dir_path + "\nYou probably don't have permission to do that.",e,false);
            }
        }
    }
    // This holds files that have been found
    public class DetectedFile: DetectedLocationPathHolder {
       // public DetectedFile(): base() {
       // }

        public DetectedFile(DetectedLocationPathHolder location, string type): base(location) {
            abs_root = location.full_dir_path;
            owner = location.owner;
            this.type = type;
        }

        // This is the name of the file
        public string name;

        public string type;

        public override string ToString()
        {
            return Path.Combine(base.ToString(),name);
        }

        // Gets the full path, including file name
        public string full_file_path {
            get {
                if(full_dir_path!=null) {
                    if(name!=null&&name!="") {
                        return Path.Combine(full_dir_path,name);
                    } else {
                        return full_dir_path;
                    }
                } else {
                    return null;
                }
            }
        }
        public new bool exists {
            get {
                return File.Exists(full_file_path);
            }
        }
        public new void delete() {
            try {
                Directory.Delete(full_file_path,true);
            } catch (Exception e) {
                throw new MException("Delete Error","Error while trying to delete this:\n" + full_file_path + "\nYou probably don't have permission to do that.",e,false);
            }
        }
    }
}