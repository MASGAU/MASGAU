using System;
using VDF;
namespace MASGAU.Location {
    public abstract class ASteamLocationHandler : ALocationHandler {
        // The paths
        public string path { get; protected set; }
        public string userdata_path { get; protected set; }
        public string steam_apps_path { get; protected set; }

        protected SteamConfigFile config_file;

        public ASteamLocationHandler()
            : base(HandlerType.Steam) {
            resetSteamPath();
        }

        protected abstract void resetSteamPath();

        public bool found {
            get {
                if (!String.IsNullOrEmpty(steam_path))
                    return true;
                else
                    return false;
            }
        }

        public string steam_path {
            get {
                return path;
            }
        }

        public override bool ready {
            get {
                if (String.IsNullOrEmpty(path)) {
                    return false;
                } else {
                    return true;
                }
            }
        }

    }
}
