using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using Communication;
using Communication.Translator;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MVC;
using Translator;
namespace MASGAU {
    public class OldGame : AModelItem<GameID> {
        //public bool sync_enabled {
        //    get {
        //        return Core.settings.isGameSyncEnabled(id);
        //    }
        //    set {
        //        bool temp = Core.settings.isGameSyncEnabled(id);
        //        if (value != temp) {
        //            Core.settings.setGameSyncEnabled(this.id, value);
        //            Core.rebuild_sync = true;
        //            NotifyPropertyChanged("sync_enabled");
        //        }
        //    }
        //}



        //public void clearSyncPath() {
        //    Core.settings.clearDefaultGamePath(this.id);
        //}

        //public bool sync_available {
        //    get {
        //        switch (id.platform) {
        //            case GamePlatform.PS1:
        //            case GamePlatform.PS2:
        //            case GamePlatform.PS3:
        //            case GamePlatform.PSP:
        //                return false;
        //            default:
        //                return true;
        //        }
        //    }
        //}

        //public bool multiple_detected_paths {
        //    get {
        //        return detected_locations.Count > 1;
        //    }
        //}

        //public bool has_sync_location {
        //    get {
        //        return sync_location != null;
        //    }
        //}

        //public string sync_location {
        //    get {
        //        if (multiple_detected_paths) {
        //            string path = Core.settings.getDefaultGamePath(this.id);
        //            if (path != null && this.detected_locations.ContainsKey(path))
        //                return path;
        //            else {
        //                Core.settings.clearDefaultGamePath(this.id);
        //                return null;
        //            }
        //        } else {
        //            foreach (KeyValuePair<string, DetectedLocationPathHolder> add_me in detected_locations) {
        //                return add_me.Key;
        //            }
        //        }
        //        return null;
        //    }
        //    set {
        //        Core.settings.setDefaultGamePath(this.id, value);
        //        NotifyPropertyChanged("sync_location");
        //    }
        //}










    }
}