using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.IO;
using MASGAU.LocationHandlers;
using System.ComponentModel;
namespace MASGAU
{
    public enum VersioningUnit {
        Seconds,
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years,
        Decades,
        Centuries,
        Millenia
    }
    public class SettingsHandler: ConfigFileHandler
    {
        // This file contains methods for getting and manipulating all of MASGAU's settings
        // It acts as a worker layer between the program and the config file

        public SettingsHandler(): base(null,"config.xml",Core.mutex) {
            // Checks if there is a config folder in the same folder as the program
            // This is for the portable version
            if (File.Exists(Path.Combine(Core.app_path,file_name))){
                file_path = Core.app_path;
            } else {
                // Checks what OS is being used, as each one will store the config file somewhere different
                // Checks what mode MASGAU is running in so the appropriate config file can be loaded
            	if(Core.all_users_mode) {
                	file_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"masgau");
            	} else {
                	file_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"masgau");
            	}
            }

           if(file_path!=null) {
                // Load the settings from the XML file
                loadSettings();
            } else {
                throw new MException("Config File Error","Could not determine an acceptable path for the config file.",false);
            }


            shared_settings.Add("email");
            shared_settings.Add("backup_path");
            shared_settings.Add("backup_path_set");
            shared_settings.Add("steam_path");
            shared_settings.Add("ignore_date_check");
            shared_settings.Add("versioning");
            shared_settings.Add("versioning_frequency");
            shared_settings.Add("versioning_unit");
            shared_settings.Add("versioning_max");
            shared_settings.Add("auto_update");
            //shared_settings.Add("sync_enabled");
            //shared_settings.Add("sync_path");
            //shared_settings.Add("monitor_startup_backup");
        }

        protected override void loadSettings() {
            // Load the settings from the XML file
            base.loadSettings();
            if(getAltPaths().Count!=alt_paths.Count) {
                try {
                    alt_paths.Clear();
                    loadAltPaths();
                    alt_paths.refresh();
                    NotifyPropertyChanged("alt_paths");
                } catch (Exception e) {
                    MessageHandler.SendError("Error","Error while auto-refreshing alt paths",e);
                }
            }

            if(Core.games!=null)
                Core.games.refresh();
        }

        protected override void config_watcher_Changed(object sender, FileSystemEventArgs e)
        {
            base.config_watcher_Changed(sender,e);
        }


        public bool IsReady {
            get {
                return ready;
            }
        }

        public string email {
            get {
                return getNodeAttribute("address","email");
            }
            set {
                if(value!=null&&value.Contains("@")) {
                    int loc = value.IndexOf('@');
                    if(value.Substring(loc+1).Contains("."))
                        setNodeAttribute("address",value,"email");
                }
                NotifyPropertyChanged("email");
            }
        }


        #region Methods for getting and setting the saved window size
        public int window_width {
            get {
                string value = getNodeAttribute("width","window");
                if(value!=null) {
                    try{
                        return Int32.Parse(value);
                    } catch {
                        return 0;
                    }
                } else {
                    return 0;
                }
            }
            set {
                setNodeAttribute("width",value.ToString(),"window");
            }
        }
        public int window_height {
            get {
                string value = getNodeAttribute("height","window");
                if(value!=null) {
                    try{
                        return Int32.Parse(value);
                    } catch {
                        return 0;
                    }
                } else {
                    return 0;
                }
            }
            set {
                setNodeAttribute("height",value.ToString(),"window");
            }
        }
        #endregion

        #region Methods for the Backup Path
        private void notifyBackupPathChange()
        {
            NotifyPropertyChanged("backup_path");
            NotifyPropertyChanged("backup_path_set");
            NotifyPropertyChanged("backup_path_not_set");
        }

        public string backup_path {
            get {
                return getNodeValue("backup_path");
            }
            set {
                if(getNodeValue("backup_path")!=value) {
                    setNodeValue(value,"backup_path");
                    notifyBackupPathChange();
                }
            }

        }
        public Boolean backup_path_not_set
        {
            get
            {
                return !backup_path_set;
            }
        }

        public bool backup_path_set
        {
            get {
                if(backup_path!=null)
                    return true;
                else
                    return false;
            }
        }
        public bool clearBackupPath() {
            bool return_me = clearNode("backup_path");
            notifyBackupPathChange();
            return return_me;
        }
        #endregion

        #region Methods for Steam
        // Thing to make overriding Steam easier
        public string steam_path {
            get {
                return Core.locations.steam_path;
            }
            set {
                steam_override = value;
            }
        } 
        public string steam_override {
            get {
                return getNodeValue("steam_override");
            }
            set {
                setNodeValue(value,"steam_override");
                Core.locations.resetSteam();
                NotifyPropertyChanged("steam_path");
            }
        } 
        public void clearSteamPath() {
            clearNode("steam_override");
            Core.locations.resetSteam();
            NotifyPropertyChanged("steam_path");
        }
        #endregion

        #region Methods to handle alternate paths
        public Model<AltPathHolder> alt_paths = new Model<AltPathHolder>();
        private void loadAltPaths() {
            foreach(string alt_path in getAltPaths()) {
                AltPathHolder add_me = new AltPathHolder(alt_path);
                alt_paths.Add(add_me);
            }
        }

        public int alt_path_count {
            get {
                return getAltPaths().Count;
            }
        }

        public List<string> getAltPaths() {
            return getNodeGroupValues("path","alt_paths");
        }
        public bool addAltPath(string add_me) {
            AltPathHolder new_path = new AltPathHolder(add_me);
            alt_paths.Add(new_path);
            return saveAltPaths();
        }
        public bool removeAltPath(string remove_me) {
            foreach(AltPathHolder item in alt_paths) {
                if(item.path==remove_me) {
                    alt_paths.Remove(item);
                    return saveAltPaths();
                }
            }
            return false;
        }
        private bool saveAltPaths() {
            List<string> list = new List<string>();
            foreach(AltPathHolder item in alt_paths)
                list.Add(item.path);
            return setNodeGroupValues("path",list,"alt_paths");
        }
        #endregion

        #region Methods to handle the date check
        public bool ignore_date_check {
            get {
                return (getNodeAttribute("ignore","date_check")=="True");
            }
            set {
                if(value)
                    versioning = false;

                setNodeAttribute("ignore",value.ToString(),"date_check");
                NotifyPropertyChanged("ignore_date_check");
            }
        }
        #endregion

        #region Methods to handle the versioning settings
        public List<VersioningUnit> versioning_units {
            get {
                List<VersioningUnit> return_me = new List<VersioningUnit>();
                foreach(VersioningUnit value in Enum.GetValues(typeof(VersioningUnit))) {
                    return_me.Add(value);
                }
                return return_me;
            }
        }
        public bool versioning {
            get {
                return (getNodeAttribute("enabled","versioning")=="True");
            }
            set {
                if(value) {
                    ignore_date_check = false;
                }

                setNodeAttribute("enabled",value.ToString(),"versioning");
                NotifyPropertyChanged("versioning");
            }
        }
        public long versioning_frequency {
            get {
                try {
                    return Int64.Parse(getNodeAttribute("frequency","versioning"));
                } catch{
                    return 1;
                }
            }
            set {
                setNodeAttribute("frequency",value.ToString(),"versioning");
                NotifyPropertyChanged("versioning_frequency");
            }
        }
        public VersioningUnit versioning_unit {
            get {
                if(getNodeAttribute("frequency_unit","versioning")!=null&&getNodeAttribute("frequency_unit","versioning")!="") {
                    return parseVersioningUnit(getNodeAttribute("frequency_unit","versioning"));
                } else
                    return VersioningUnit.Hours;
            }
            set {
                setNodeAttribute("frequency_unit",value.ToString(),"versioning");
                NotifyPropertyChanged("versioning_unit");
            }
        }
        public static VersioningUnit parseVersioningUnit(string parse_me) {
            switch(parse_me) {
                case "Seconds":
                    return VersioningUnit.Seconds;
                case "Minutes":
                    return VersioningUnit.Minutes;
                case "Hours":
                    return VersioningUnit.Hours;
                case "Days":
                    return VersioningUnit.Days;
                case "Weeks":
                    return VersioningUnit.Weeks;
                case "Months":
                    return VersioningUnit.Months;
                case "Years":
                    return VersioningUnit.Years;
                case "Decades":
                    return VersioningUnit.Decades;
                case "Centuries":
                    return VersioningUnit.Centuries;
                case "Millenia":
                    return VersioningUnit.Millenia;
                default:
                    return VersioningUnit.Hours;
            }
        }

        public long versioning_max {
            get {
                try {
                    return Int64.Parse(getNodeAttribute("max","versioning"));
                } catch{
                    return 1;
                }
            }
            set {
                setNodeAttribute("max",value.ToString(),"versioning");
                NotifyPropertyChanged("versioning_max");
            }
        }
        public long versioning_ticks {
            get{
                switch(versioning_unit) {
                    case VersioningUnit.Seconds:
                        return versioning_frequency*10000000;
                    case VersioningUnit.Minutes:
                        return versioning_frequency*10000000*60;
                    case VersioningUnit.Hours:
                        return versioning_frequency*10000000*60*60;
                    case VersioningUnit.Days:
                        return versioning_frequency*10000000*60*60*24;
                    case VersioningUnit.Weeks:
                        return versioning_frequency*10000000*60*60*24*7;
                    case VersioningUnit.Months:
                        return versioning_frequency*10000000*60*60*24*7*4;
                    case VersioningUnit.Years:
                        return versioning_frequency*10000000*60*60*24*365;
                    case VersioningUnit.Decades:
                        return versioning_frequency*10000000*60*60*24*365*10;
                    case VersioningUnit.Centuries:
                        return versioning_frequency*10000000*60*60*24*365*100;
                    case VersioningUnit.Millenia:
                        return versioning_frequency*10000000*60*60*24*365*1000;
                    default:
                        return versioning_frequency*10000000*60*60;
                }
            }
        }
        #endregion

        #region Monitor settings
        public bool monitor_startup_backup {
            get {
                // It's like this because I want the default setting to be "yes"
                return !(getNodeAttribute("startup_backup","monitor")=="False");
            }
            set {
                setNodeAttribute("startup_backup",value.ToString(),"monitor");
                NotifyPropertyChanged("monitor_startup_backup");
            }
        }
        #endregion

        #region Update settings
        public bool already_updated = false;
        public bool auto_update {
            get {
                if(getNodeAttribute("enabled","auto_update")!=null)
                    return (getNodeAttribute("enabled","auto_update").ToLower().Equals("true"));
                else
                    return false;
            }
            set {
                setNodeAttribute("enabled",value.ToString(),"auto_update");
                NotifyPropertyChanged("auto_update");
            }
        }
        #endregion

        #region The upcoming maybe sync support
        public bool any_sync_enabled {
            get {
                return getSpecificNode("game", false, "sync", true.ToString())!=null;
            }
        }
        public bool sync_path_set {
            get {
                return sync_path!=null;
            }
        }
        public string sync_path {
            get {
                return getNodeValue("sync_path");
            }
            set {
                setNodeValue(value,"sync_path");
                NotifyPropertyChanged("sync_path");
            }
        }
        public bool clearSyncPath() {
            return clearNode("sync_path");
        }
        public string getDefaultGamePath(GameID game) {
            return getSpecificNodeAttribute("game", "default_path", game.string_array);
        }
        public bool clearDefaultGamePath(GameID game) {
            return clearSpecificNodeAttribute("game", "default_path", game.string_array);
        }
        public bool setDefaultGamePath(GameID game, string path) {
            return setSpecificNodeAttrib("game", "default_path",path,game.string_array);
        }
        #endregion

        #region Enabling and disabling games
        public bool setGameBackupEnabled(GameID set_me, bool to_me) {
            return setSpecificNodeAttrib("game", "backup",to_me.ToString(),set_me.string_array);
        }

        public bool isGameBackupEnabled(GameID check_me) {
            return getSpecificNodeAttribute("game", "backup", check_me.string_array)!=false.ToString();
        }
        public bool setGameSyncEnabled(GameID set_me, bool to_me) {
            return setSpecificNodeAttrib("game", "sync",to_me.ToString(),set_me.string_array);
        }

        public bool isGameSyncEnabled(GameID check_me) {
            return getSpecificNodeAttribute("game", "sync", check_me.string_array)==true.ToString();
        }
        #endregion
    }
}
