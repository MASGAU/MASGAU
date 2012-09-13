using System;
using System.Collections.Generic;
using System.IO;
using Collections;
using Config;
using GameSaveInfo;
using MASGAU.Location.Holders;
namespace MASGAU.Location {

    public abstract class AScummVMLocationHandler : ALocationHandler {
        public TwoKeyDictionary<string, string, string> Locations = null;
        private Dictionary<String, FileInfo> config_files;
        protected string install_path = null;


        public override bool ready {
            get { return config_files != null && config_files.Count > 0; }
        }



        public AScummVMLocationHandler()
            : base(HandlerType.ScummVM) {


        }

        protected abstract Dictionary<String, FileInfo> collectConfigFiles();
        protected abstract string findInstallPath();

        private void setup() {
            Locations = new TwoKeyDictionary<string, string, string>();
            config_files = collectConfigFiles();
            foreach (String user in config_files.Keys) {
                IniFileHandler ini = new IniFileHandler(config_files[user]);
                foreach (string section in ini.Keys) {
                    if (ini[section].ContainsKey("savepath")) {
                        Locations.Add(user, section, ini[section]["savepath"]);
                    }
                }
            }
            install_path = findInstallPath();
        }

        protected override DetectedLocations getPaths(ScummVM get_me) {
            if (get_me.Name == "scummvm")
                Console.Out.Write("");

            List<DetectedLocationPathHolder> paths = new List<DetectedLocationPathHolder>();
            if (Locations == null) {
                setup();
            }
            if (install_path != null) {
                paths.AddRange(loadLocations(install_path, get_me, null));
            }

            DetectedLocations return_me = new DetectedLocations();

            foreach (string user in Locations.Keys) {
                if (get_me.Name != "scummvm" && Locations[user].ContainsKey("scummvm")) {
                    foreach (DetectedLocationPathHolder path in loadLocations(Locations[user]["scummvm"], get_me, user)) {
                        DirectoryInfo info = new DirectoryInfo(path.full_dir_path);
                        if (info.GetFiles(get_me.Name + "*").Length > 0) {
                            return_me.Add(path);
                        }
                    }
                }


                if (Locations[user].ContainsKey(get_me.Name)) {
                    return_me.AddRange(loadLocations(Locations[user][get_me.Name], get_me, user));
                }
            }


            return return_me;
        }

        protected DetectedLocations loadLocations(String path, ScummVM scumm, string user) {
            DetectedLocations locs = Core.locations.interpretPath(path);
            return locs;
        }

        //protected DetectedLocations filterLocations(DetectedLocations locs, ScummVM scumm, string user) {
        //    List<string> keys = new List<string>(locs.Keys);

        //    foreach (string key in keys) {
        //        if (!filterLocation(locs[key], scumm, user)) {
        //            locs.Remove(key);
        //        }
        //    }
        //    return locs;
        //}

        protected bool filterLocation(DetectedLocationPathHolder loc, ScummVM scumm, string user) {
            DirectoryInfo dir = new DirectoryInfo(loc.full_dir_path);
            string pattern = scumm.Name + "*";
            if (dir.GetFiles(pattern).Length > 0) {
                loc.owner = user;
                return true;
            }
            return false;

        }

    }
}
