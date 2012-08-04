using System;
using System.Collections.Generic;
using System.IO;
using MVC.Translator;
using Email;
using MASGAU.Location.Holders;
using MVC;
using Translator;
using Config;
namespace MASGAU.Settings {

    public class Settings : ASettings, IEmailSource {
        // This file contains methods for getting and manipulating all of MASGAU's settings
        // It acts as a worker layer between the program and the config file

        public Settings(ConfigMode mode)
            : base(new XmlSettingsFile("MASGAU", mode)) {
        }

        protected override SettingsCollection createSettings(SettingsCollection settings) {
            settings = base.createSettings(settings);

            Setting new_setting = new Setting("backup_path", null, "paths", "backup");
            new_setting.addAdditionalNotification("IsBackupPathSet");
            new_setting.addAdditionalNotification("backup_path_not_set");
            settings.Add(new_setting);

            new_setting = new Setting("sync_path", null, "paths", "sync");
            new_setting.addAdditionalNotification("any_sync_enabled");
            new_setting.addAdditionalNotification("sync_path_set");
            settings.Add(new_setting);


            new_setting = new Setting("steam_override", null, "paths", "steam_override");
            new_setting.addAdditionalNotification("steam_path");
            settings.Add(new_setting);

            new_setting = new Setting("save_paths", null, "paths", "saves");
            settings.Add(new_setting);


            settings.Add(new Setting("IgnoreDateCheck", false, "date_check","ignore"));

            settings.Add(new Setting("MonitoredGames",null,"games","monitor"));

            settings.Add(new Setting("VersioningEnabled", false, "versioning", "enabled"));
            settings.Add(new Setting("VersioningUnit", VersioningUnit.Hours, "versioning", "unit"));
            settings.Add(new Setting("VersioningFrequency", 5, "versioning", "frequency"));
            settings.Add(new Setting("VersioningMax", 100, "versioning", "max"));

            settings.Add(new Setting("SuppressSubmitRequests", false, "submit", "suprress"));

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

                if (sync_path_set &&
                    this.sync_path != adjustPortablePath(this.sync_path)) {
                    this.sync_path = adjustPortablePath(this.sync_path);
                }

                // This adjusts any alt. save locations that are set relative to the portable drive
                Model<AltPathHolder> tmp = new Model<AltPathHolder>();
                tmp.AddRange(save_paths);
                foreach(AltPathHolder path in tmp) {
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

        public override string EmailRecipient {
            get {
                return Core.submission_email;
            }
            set {
                base.EmailRecipient = value;
            }
        }

        public bool SuppressSubmitRequests {
            get {
                return getLastBoolean("SuppressSubmitRequests");
            }
            set {
                set("SuppressSubmitRequests", value);
            }
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
        public Model<AltPathHolder> save_paths {
            get {
                Model<AltPathHolder> _save_paths = new Model<AltPathHolder>();
                foreach (string alt_path in getSavePaths()) {
                    AltPathHolder add_me = new AltPathHolder(alt_path);
                    _save_paths.Add(add_me);
                }
                return _save_paths;
            }
        }

        public int save_path_count {
            get {
                return save_paths.Count;
            }
        }

        public List<string> getSavePaths() {
            List<string> locations = get("save_paths");
            return locations;
        }

        public bool addSavePath(string add_me) {
            bool result = this.addUnique("save_paths", add_me);
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
                return true;
            }
            return false;
        }
        #endregion

        #region Methods to handle the date check
        public bool IgnoreDateCheck {
            get {
                return getLastBoolean("IgnoreDateCheck");
            }
            set {
//                if (value)
  //                  versioning = false;
                set("IgnoreDateCheck", value);
            }
        }
        #endregion

        #region Methods to handle the versioning settings
        public int VersioningFrequency {
            get {
                return getLastInteger("VersioningFrequency");
            }
            set {
                set("VersioningFrequency", value);
            }
        }
        public int VersioningMax {
            get {
                return getLastInteger("VersioningMax");
            }
            set {
                set("VersioningMax", value);
            }
        }
        public VersioningUnit VersioningUnit {
            get {
                return (VersioningUnit)Enum.Parse(typeof(VersioningUnit), getLast("VersioningUnit"));
            }
            set {
                set("VersioningUnit", value);
            }
        }
        public bool VersioningEnabled {
            get {
                return getLastBoolean("VersioningEnabled");
            }
            set {
                set("VersioningEnabled", value);
            }
        }
        public long VersioningTicks {
            get {
                switch (VersioningUnit) {
                    case VersioningUnit.Seconds:
                        return VersioningFrequency * 10000000;
                    case VersioningUnit.Minutes:
                        return VersioningFrequency * 10000000 * 60;
                    case VersioningUnit.Hours:
                        return VersioningFrequency * 10000000 * 60 * 60;
                    case VersioningUnit.Days:
                        return VersioningFrequency * 10000000 * 60 * 60 * 24;
                    case VersioningUnit.Weeks:
                        return VersioningFrequency * 10000000 * 60 * 60 * 24 * 7;
                    case VersioningUnit.Months:
                        return VersioningFrequency * 10000000 * 60 * 60 * 24 * 7 * 4;
                    case VersioningUnit.Years:
                        return VersioningFrequency * 10000000 * 60 * 60 * 24 * 365;
                    case VersioningUnit.Decades:
                        return VersioningFrequency * 10000000 * 60 * 60 * 24 * 365 * 10;
                    case VersioningUnit.Centuries:
                        return VersioningFrequency * 10000000 * 60 * 60 * 24 * 365 * 100;
                    case VersioningUnit.Millenia:
                        return VersioningFrequency * 10000000 * 60 * 60 * 24 * 365 * 1000;
                    default:
                        return VersioningFrequency * 10000000 * 60 * 60;
                }
            }
        }
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
        public bool any_sync_enabled {
            get {
                return false;// getSpecificNode("game", false, "sync", true.ToString()) != null;
            }
        }
        public bool sync_path_set {
            get {
                return sync_path != null;
            }
        }
        public string sync_path {
            get {
                return getLast("sync_path");
            }
            set {
                set("sync_path", value);
            }
        }
        public void clearSyncPath() {
            erase("sync_path");
        }
        #endregion

        #region Enabling and disabling games
        public List<string> MonitoredGames {
            get {
                return this.get("MonitoredGames");
            }
        }

        public bool setGameMonitored(GameID set_me, bool to_me) {
            if(to_me) {
                return addUnique("MonitoredGames", set_me.ToString());
            } else {
                return this.remove("MonitoredGames", set_me.ToString());
            }
        }
        public bool isGameMonitored(GameID check_me) {
            return get("MonitoredGames").Contains(check_me.ToString());
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
