using System;
using System.Collections.Generic;
using System.IO;
using Communication.Translator;
using Email;
using MASGAU.Location.Holders;
using MVC;
using Translator;
using Config;
namespace MASGAU.Settings {

    public class Settings : ASettings {
        // This file contains methods for getting and manipulating all of MASGAU's settings
        // It acts as a worker layer between the program and the config file

        public Settings(ConfigMode mode)
            : base(new XmlSettingsFile("masgau", mode)) {
        }

        protected override SettingsCollection createSettings(SettingsCollection settings) {
            settings = base.createSettings(settings);

            Setting new_setting = new Setting("backup_path", null, "paths","backup");
            new_setting.addAdditionalNotification("IsBackupPathSet");
            new_setting.addAdditionalNotification("backup_path_not_set");
            settings.Add(new_setting);

            new_setting = new Setting("steam_override", null, "paths", "steam_override");
            new_setting.addAdditionalNotification("steam_path");
            settings.Add(new_setting);

            new_setting = new Setting("save_paths", null, "paths", "saves");
            settings.Add(new_setting);


            settings.Add(new Setting("ignore_date_check", false, "ignore_date_check"));
            settings.Add(new Setting("automatic_startup", false, "startup", "automatic"));
            settings.Add(new Setting("automatic_backup", false, "startup", "automatic_backup"));

            settings.Add(new Setting("monitored_games",null,"games","monitor"));

            //shared_settings.Add("versioning");
            //shared_settings.Add("versioning_frequency");
            //shared_settings.Add("versioning_unit");
            //shared_settings.Add("versioning_max");

            return settings;
        }


        protected override void processSettings() {
            if(mode == ConfigMode.PortableApps) {
                if (IsBackupPathSet &&
                    this.backup_path != adjustPortablePath(this.backup_path)) {
                    this.backup_path = adjustPortablePath(this.backup_path);
                }

                if (steam_override != null &&
                    this.steam_override != adjustPortablePath(this.steam_override)) {
                    this.steam_override = adjustPortablePath(this.steam_override);
                }

                //if (sync_path_set &&
                //    this.sync_path != adjustPortablePath(this.sync_path)) {
                //    this.sync_path = adjustPortablePath(this.sync_path);
                //}

                // This adjusts any alt. save locations that are set relative to the portable drive
                for (int i = 0; i < _save_paths.Count; i++) {
                    AltPathHolder path = _save_paths[i];
                    if (path.path != adjustPortablePath(path.path)) {
                        removeSavePath(path.path);
                        addSavePath(adjustPortablePath(path.path));
                    }
                }

                if (last_drive != current_drive)
                    updateLastDrive();

            }

            Games.Refresh();
        }




        #region Methods for the Backup Path
        public string backup_path {
            get {
                return getLast("backup_path");
            }
            set {
                set("backup_path", value);
            }

        }
        public Boolean backup_path_not_set {
            get {
                return !IsBackupPathSet;
            }
        }

        public bool IsBackupPathSet {
            get {
                if (backup_path != null)
                    return true;
                else
                    return false;
            }
        }
        public void clearBackupPath() {
            erase("backup_path");
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
                return getLast("steam_override");
            }
            set {
                set("steam_override",value);
                Core.locations.resetSteam();
            }
        }
        public void clearSteamPath() {
            erase("steam_override");
        }
        #endregion

        #region Methods to handle alternate paths
        private Model<AltPathHolder> _save_paths = new Model<AltPathHolder>();
        public Model<AltPathHolder> save_paths {
            get {
                return _save_paths;
            }
        }

        private void refreshSavePaths() {
            _save_paths.Clear();
            foreach (string alt_path in getSavePaths()) {
                AltPathHolder add_me = new AltPathHolder(alt_path);
                _save_paths.Add(add_me);
            }
        }

        public int save_path_count {
            get {
                return getSavePaths().Count;
            }
        }

        public List<string> getSavePaths() {
            List<string> locations = get("save_paths");
            return locations;
        }

        public bool addSavePath(string add_me) {
            bool result = this.addUnique("save_paths", add_me);
            if (result) {
                refreshSavePaths();
            }
            return result;
        }

        public bool removeSavePath(string remove_me) {
            List<string> locations = getSavePaths();
            if (locations.Contains(remove_me)) {
                locations.Remove(remove_me);
                if (locations.Count > 0) {
                    string locs = "";
                    foreach (string location in locations) {
                        locs += location + ";";
                    }
                    set("save_paths", locs.Trim(';'));
                } else {
                    erase("save_paths");
                }
                refreshSavePaths();
                return true;
            }
            return false;
        }
        #endregion

        #region Methods to handle the date check
        public bool ignore_date_check {
            get {
                return getLastBoolean("ignore_date_check");
            }
            set {
//                if (value)
  //                  versioning = false;
                set("ignore_date_check", value);
            }
        }
        #endregion

        #region Methods to handle the versioning settings
        //public List<VersioningUnit> versioning_units {
        //    get {
        //        List<VersioningUnit> return_me = new List<VersioningUnit>();
        //        foreach (VersioningUnit value in Enum.GetValues(typeof(VersioningUnit))) {
        //            return_me.Add(value);
        //        }
        //        return return_me;
        //    }
        //}
        //public bool versioning {
        //    get {
        //        return (getNodeAttribute("enabled", "versioning") == "True");
        //    }
        //    set {
        //        if (value) {
        //            ignore_date_check = false;
        //        }

        //        setNodeAttribute("enabled", value.ToString(), "versioning");
        //        NotifyPropertyChanged("versioning");
        //    }
        //}
        //public long versioning_frequency {
        //    get {
        //        string node = getNodeAttribute("frequency", "versioning");
        //        if (node != null)
        //            return Int64.Parse(node);
        //        else
        //            return 1;
        //    }
        //    set {
        //        setNodeAttribute("frequency", value.ToString(), "versioning");
        //        NotifyPropertyChanged("versioning_frequency");
        //    }
        //}
        //public VersioningUnit versioning_unit {
        //    get {
        //        if (getNodeAttribute("frequency_unit", "versioning") != null && getNodeAttribute("frequency_unit", "versioning") != "") {
        //            return parseVersioningUnit(getNodeAttribute("frequency_unit", "versioning"));
        //        } else
        //            return VersioningUnit.Hours;
        //    }
        //    set {
        //        setNodeAttribute("frequency_unit", value.ToString(), "versioning");
        //        NotifyPropertyChanged("versioning_unit");
        //    }
        //}
        //public static VersioningUnit parseVersioningUnit(string parse_me) {
        //    switch (parse_me) {
        //        case "Seconds":
        //            return VersioningUnit.Seconds;
        //        case "Minutes":
        //            return VersioningUnit.Minutes;
        //        case "Hours":
        //            return VersioningUnit.Hours;
        //        case "Days":
        //            return VersioningUnit.Days;
        //        case "Weeks":
        //            return VersioningUnit.Weeks;
        //        case "Months":
        //            return VersioningUnit.Months;
        //        case "Years":
        //            return VersioningUnit.Years;
        //        case "Decades":
        //            return VersioningUnit.Decades;
        //        case "Centuries":
        //            return VersioningUnit.Centuries;
        //        case "Millenia":
        //            return VersioningUnit.Millenia;
        //        default:
        //            return VersioningUnit.Hours;
        //    }
        //}

        //public long versioning_max {
        //    get {
        //        string node = getNodeAttribute("max", "versioning");
        //        if (node != null)
        //            return Int64.Parse(node);
        //        else
        //            return 1;
        //    }
        //    set {
        //        setNodeAttribute("max", value.ToString(), "versioning");
        //        NotifyPropertyChanged("versioning_max");
        //    }
        //}
        //public long versioning_ticks {
        //    get {
        //        switch (versioning_unit) {
        //            case VersioningUnit.Seconds:
        //                return versioning_frequency * 10000000;
        //            case VersioningUnit.Minutes:
        //                return versioning_frequency * 10000000 * 60;
        //            case VersioningUnit.Hours:
        //                return versioning_frequency * 10000000 * 60 * 60;
        //            case VersioningUnit.Days:
        //                return versioning_frequency * 10000000 * 60 * 60 * 24;
        //            case VersioningUnit.Weeks:
        //                return versioning_frequency * 10000000 * 60 * 60 * 24 * 7;
        //            case VersioningUnit.Months:
        //                return versioning_frequency * 10000000 * 60 * 60 * 24 * 7 * 4;
        //            case VersioningUnit.Years:
        //                return versioning_frequency * 10000000 * 60 * 60 * 24 * 365;
        //            case VersioningUnit.Decades:
        //                return versioning_frequency * 10000000 * 60 * 60 * 24 * 365 * 10;
        //            case VersioningUnit.Centuries:
        //                return versioning_frequency * 10000000 * 60 * 60 * 24 * 365 * 100;
        //            case VersioningUnit.Millenia:
        //                return versioning_frequency * 10000000 * 60 * 60 * 24 * 365 * 1000;
        //            default:
        //                return versioning_frequency * 10000000 * 60 * 60;
        //        }
        //    }
        //}
        #endregion

        #region Monitor settings
        public bool automatic_backup {
            get {
                // It's like this because I want the default setting to be "yes"
                return getLastBoolean("automatic_backup");
            }
            set {
                set("automatic_backup", value);
            }
        }
        #endregion

        #region The upcoming maybe sync support
        //public bool any_sync_enabled {
        //    get {
        //        return getSpecificNode("game", false, "sync", true.ToString()) != null;
        //    }
        //}
        //public bool sync_path_set {
        //    get {
        //        return sync_path != null;
        //    }
        //}
        //public string sync_path {
        //    get {
        //        return getNodeValue("sync_path");
        //    }
        //    set {
        //        setNodeValue(value, "sync_path");
        //        NotifyPropertyChanged("sync_path");
        //    }
        //}
        //public void clearSyncPath() {
        //    erase("sync_path");
        //}
        //public string getDefaultGamePath(GameID game) {
        //    return getSpecificNodeAttribute("game", "default_path", game.string_array);
        //}
        //public bool clearDefaultGamePath(GameID game) {
        //    return clearSpecificNodeAttribute("game", "default_path", game.string_array);
        //}
        //public bool setDefaultGamePath(GameID game, string path) {
        //    return setSpecificNodeAttrib("game", "default_path", path, game.string_array);
        //}
        #endregion

        #region Enabling and disabling games
        public bool setGameMonitored(GameID set_me, bool to_me) {
            if(to_me) {
                return addUnique("monitored_games",set_me.ToString());
            } else {
                return this.remove("monitored_games",set_me.ToString());
            }
        }
        public bool isGameMonitored(GameID check_me) {
            return get("monitored_games").Contains(check_me.ToString());
        }

        //public bool setGameSyncEnabled(GameID set_me, bool to_me) {
        //    return setSpecificNodeAttrib("game", "sync", to_me.ToString(), set_me.string_array);
        //}

        //public bool isGameSyncEnabled(GameID check_me) {
        //    return getSpecificNodeAttribute("game", "sync", check_me.string_array) == true.ToString();
        //}
        #endregion
    }
}
