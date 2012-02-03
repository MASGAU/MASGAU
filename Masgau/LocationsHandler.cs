using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace MASGAU.LocationHandlers
{
    public class LocationsHandler: ILocationsHandler
    {
        public enum HandlerType {
            PlayStation,
            Steam,
            System
        }
        
        private Dictionary<HandlerType,ALocationHandler> handlers;

        public List<EnvironmentVariable> these_always_require_user_selection = 
            new List<EnvironmentVariable> {    EnvironmentVariable.SteamUser, 
                                                EnvironmentVariable.SteamUserData };


        public LocationsHandler() {
            handlers = new Dictionary<HandlerType,ALocationHandler>();

            ProgressHandler.progress_message = "Detecting System Paths...";
            handlers.Add(HandlerType.System,new SystemLocationHandler(Core.all_users_mode, Core.settings.alt_paths,this));

            ProgressHandler.progress_message = "Checking For Steam...";
            handlers.Add(HandlerType.Steam,new SteamLocationHandler(Core.settings.steam_override,this)); 

            ProgressHandler.progress_message = "Detecting PlayStation Paths...";
            handlers.Add(HandlerType.PlayStation,new PlaystationLocationHandler(this));
        
        }

        public void resetSteam() {
            handlers[HandlerType.Steam] = new SteamLocationHandler(steam_path,this);
        }
        public bool steam_detected {
            get {
                return (handlers[HandlerType.Steam] as ASteamLocationHandler).found;
            }
        }

        public string steam_path {
            get {
                return (handlers[HandlerType.Steam] as ASteamLocationHandler).steam_path;
            }
        }

        public PlatformVersion platform_version {
            get {
                return (handlers[HandlerType.System] as ASystemLocationHandler).platform_version;
            }
        }

        public bool uac_enabled {
            get {
                return (handlers[HandlerType.System] as ASystemLocationHandler).uac_enabled;
            }
        }

        public List<DetectedLocationPathHolder> getPaths(LocationHolder get_me) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            foreach(KeyValuePair<HandlerType,ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.getPaths(get_me));
            }
            return return_me;
        }
        public List<string> getUsers(EnvironmentVariable for_me) {
            List<string> return_me = new List<string>();
            foreach(KeyValuePair<HandlerType,ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.getUsers(for_me));
            }
            return return_me;
        }

        public string getAbsoluteRoot(LocationPathHolder parse_me, string user) {
            foreach(KeyValuePair<HandlerType,ALocationHandler>  handler in handlers) {
                string result = handler.Value.getAbsoluteRoot(parse_me, user);
                if(result!=null)
                    return result;
            }
            return null;
        }
        public string getAbsolutePath(LocationPathHolder parse_me, string user) 
        {
            foreach(KeyValuePair<HandlerType,ALocationHandler>  handler in handlers) {
                string result = handler.Value.getAbsolutePath(parse_me, user);
                if(result!=null)
                    return result;
            }
            return null;
        }

        public List<DetectedLocationPathHolder> interpretPath(string interpret_me) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            foreach(KeyValuePair<HandlerType,ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.interpretPath(interpret_me));
            }
            return return_me;
        }

        public bool ready  {
            get {
                return true;
            }
        }

   }
}
