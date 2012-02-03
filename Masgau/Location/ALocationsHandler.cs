using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using MASGAU.Location.Holders;
using MASGAU.Communication.Progress;

namespace MASGAU.Location
{
    public abstract class ALocationsHandler: ILocationsHandler 
    {
        
        protected Dictionary<HandlerType,ALocationHandler> handlers;

        public List<EnvironmentVariable> these_always_require_user_selection = 
            new List<EnvironmentVariable> {    EnvironmentVariable.SteamUser, 
                                                EnvironmentVariable.SteamUserData };

        protected ALocationsHandler() {
            handlers = new Dictionary<HandlerType,ALocationHandler>();

            ProgressHandler.progress_message = "Detecting System Paths...";
            ASystemLocationHandler system_handler = this.setupSystemHandler();
            handlers.Add(HandlerType.System, system_handler);

            ProgressHandler.progress_message = "Checking For Steam...";
            ASteamLocationHandler steam_handler = this.setupSteamHandler();
            handlers.Add(HandlerType.Steam, steam_handler);

            ProgressHandler.progress_message = "Detecting PlayStation Paths...";
            APlaystationLocationHandler playstation_handler = this.setupPlaystationHandler();
            handlers.Add(HandlerType.PlayStation, playstation_handler);
        }

        protected abstract APlaystationLocationHandler setupPlaystationHandler();
        protected abstract ASteamLocationHandler setupSteamHandler();
        protected abstract ASystemLocationHandler setupSystemHandler();

        protected void addNewHandler(ALocationHandler handler) {
            handlers.Add(handler.type, handler);
        }


        public void resetHandler(HandlerType type) {
            Type originalType = handlers[type].GetType();
            handlers.Add(type, (ALocationHandler)Activator.CreateInstance(originalType));
        }

        public void resetSteam() {
            resetHandler(HandlerType.Steam);
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

        public List<DetectedLocationPathHolder> getPaths(ALocationHolder get_me) {
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
            interpret_me = correctPath(interpret_me);
            if(interpret_me==null) {
                return return_me;
            }
            foreach(KeyValuePair<HandlerType,ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.interpretPath(interpret_me));
            }
            return return_me;
        }

        protected static string correctPath(string correct_me) {
            string[] sections = correct_me.Split(Path.DirectorySeparatorChar);
            DirectoryInfo dir = new DirectoryInfo(sections[0] + Path.DirectorySeparatorChar);
            for(int i = 1;i<sections.Length;i++) {
                DirectoryInfo[] sub_dir = dir.GetDirectories(sections[i]);
                if(sub_dir.Length==1) {
                    if(sub_dir[0].Exists) {
                        dir = sub_dir[0];
                        continue;
                    } else 
                        return null;
                } else if (sub_dir.Length>1) {
                    return null;
                } else {
                    return null;
                }
            }
            return dir.FullName;
        }

        public bool ready  {
            get {
                return true;
            }
        }

   }
}
