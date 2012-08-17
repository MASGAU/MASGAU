using System;
using System.Collections.Generic;
using System.IO;
using MVC.Translator;
using MASGAU.Location.Holders;
using GameSaveInfo;
namespace MASGAU.Location {
    public abstract class ALocationsHandler : ILocationsHandler {

        public ASteamLocationHandler steam { get; protected set; }
        public APlaystationLocationHandler ps { get; protected set; }

        public ALocationHandler getHandler(HandlerType type) {
            return handlers[type];
        }

        public string getFolder(EnvironmentVariable ev, string path) {
            LocationPath parse_me = new LocationPath(ev,path);
            foreach (string user in this.getUsers(ev)) {
                return this.getAbsoluteRoot(parse_me, user);
            }
            return this.getAbsoluteRoot(parse_me, null);
        }

        protected Dictionary<HandlerType, ALocationHandler> handlers;

        public List<EnvironmentVariable> these_always_require_user_selection =
            new List<EnvironmentVariable> {    EnvironmentVariable.SteamUser, 
                                                EnvironmentVariable.SteamUserData };

        protected ALocationsHandler() {
            Console.Out.Write("");
        }


        //private bool already_setup = false;
        public void setup() {
            handlers = new Dictionary<HandlerType, ALocationHandler>();

            TranslatingProgressHandler.setTranslatedMessage("DetectingSystemPaths");
            ASystemLocationHandler system_handler = this.setupSystemHandler();
            handlers.Add(HandlerType.System, system_handler);

            TranslatingProgressHandler.setTranslatedMessage("DetectingSteam");
            ASteamLocationHandler steam_handler = this.setupSteamHandler();
            handlers.Add(HandlerType.Steam, steam_handler);
            steam = steam_handler;

            TranslatingProgressHandler.setTranslatedMessage("DetectingPlayStation");
            APlaystationLocationHandler playstation_handler = this.setupPlaystationHandler();
            ps = playstation_handler;
            handlers.Add(HandlerType.PlayStation, playstation_handler);

            TranslatingProgressHandler.setTranslatedMessage("DetectingScummVM");
            AScummVMLocationHandler scummvm_handler = this.setupScummVMHandler();
            handlers.Add(HandlerType.ScummVM, scummvm_handler);
        }

        protected abstract APlaystationLocationHandler setupPlaystationHandler();
        protected abstract ASteamLocationHandler setupSteamHandler();
        protected abstract ASystemLocationHandler setupSystemHandler();
        protected abstract AScummVMLocationHandler setupScummVMHandler();

        protected void addNewHandler(ALocationHandler handler) {
            handlers.Add(handler.type, handler);
        }

        public void resetHandler(HandlerType type) {
            Type originalType = handlers[type].GetType();
            handlers.Remove(type);
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

        public string platform_version {
            get {
                return (handlers[HandlerType.System] as ASystemLocationHandler).platform_version;
            }
        }

        public bool uac_enabled {
            get {
                return (handlers[HandlerType.System] as ASystemLocationHandler).uac_enabled;
            }
        }

        public DetectedLocations getPaths(ALocation get_me) {
            DetectedLocations return_me = new DetectedLocations();
            foreach (KeyValuePair<HandlerType, ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.getPaths(get_me));
            }
            return return_me;
        }

        public List<string> getPaths(EnvironmentVariable ev) {
            List<string> return_me = new List<string>();
            foreach (KeyValuePair<HandlerType, ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.getPaths(ev));
            }
            return return_me;
        }



        public List<string> getUsers(EnvironmentVariable for_me) {
            List<string> return_me = new List<string>();
            foreach (KeyValuePair<HandlerType, ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.getUsers(for_me));
            }
            return return_me;
        }

        public string getAbsoluteRoot(LocationPath parse_me, string user) {
            foreach (KeyValuePair<HandlerType, ALocationHandler> handler in handlers) {
                string result = handler.Value.getAbsoluteRoot(parse_me, user);
                if (result != null)
                    return result;
            }
            return null;
        }

        public string getAbsolutePath(LocationPath parse_me, string user) {
            if (parse_me.EV == EnvironmentVariable.AltSavePaths) {
                DetectedLocations locs = interpretPath(parse_me.ToString());
                if (locs.Count > 0)
                    parse_me = locs.getMostAccurateLocation();
                else
                    return null;
            }

            foreach (KeyValuePair<HandlerType, ALocationHandler> handler in handlers) {
                string result = handler.Value.getAbsolutePath(parse_me, user);
                if (result != null)
                    return result;
            }
            return null;
        }

        public DetectedLocations interpretPath(string interpret_me) {
            interpret_me = interpret_me.TrimEnd(Path.DirectorySeparatorChar);
            DetectedLocations return_me = new DetectedLocations();
            interpret_me = correctPath(interpret_me);
            if (interpret_me == null) {
                return return_me;
            }

            foreach (KeyValuePair<HandlerType, ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.interpretPath(interpret_me));
            }

            return return_me;
        }

        protected static string correctPath(string correct_me) {
            string[] sections = correct_me.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            DirectoryInfo dir = new DirectoryInfo(sections[0] + Path.DirectorySeparatorChar);
            for (int i = 1; i < sections.Length; i++) {
                DirectoryInfo[] sub_dir = dir.GetDirectories(sections[i]);
                if (sub_dir.Length == 1) {
                    if (sub_dir[0].Exists) {
                        dir = sub_dir[0];
                        continue;
                    } else
                        return null;
                } else if (sub_dir.Length > 1) {
                    return null;
                } else {
                    return null;
                }
            }
            return dir.FullName;
        }

        public bool ready {
            get {
                return true;
            }
        }

    }
}
