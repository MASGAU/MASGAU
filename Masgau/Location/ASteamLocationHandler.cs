using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location
{
    public abstract class ASteamLocationHandler: ALocationHandler
    {
        // The paths
        protected string     path, userdata_path, steam_apps_path;

        public ASteamLocationHandler(): base(HandlerType.Steam) {
            resetSteamPath();
        }

        protected abstract void resetSteamPath();

        public bool found {
            get {
                if(steam_path!=null)
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
                if(path==null) {
                    return false;
                } else {
                    return true;
                }
            }
        }

    }
}
