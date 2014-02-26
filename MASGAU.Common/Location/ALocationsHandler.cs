using System;
using System.Collections.Generic;
using System.IO;
using GameSaveInfo;
using MVC.Translator;
namespace MASGAU.Location {
    public abstract class ALocationsHandler : ILocationsHandler {

        public ASteamLocationHandler steam { get; protected set; }
        public APlaystationLocationHandler ps { get; protected set; }

        public ALocationHandler getHandler(HandlerType type) {
            return handlers[type];
        }

        public string getFolder(EnvironmentVariable ev, string user) {
            LocationPath parse_me = new LocationPath(ev, null);
            List<string> users = this.getUsers(ev);
            if (String.IsNullOrEmpty(user)) {
                foreach (string usr in users) {
                    return this.getAbsoluteRoot(parse_me, usr);
                }
                return this.getAbsoluteRoot(parse_me, null);
            }
            if (users.Contains(user)) {
                return this.getAbsoluteRoot(parse_me, user);
            }
            throw new Exception("User " + user + " does not have a folder for EV " + ev.ToString());

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
                if (!String.IsNullOrEmpty(result))
                    return result;
            }
            return null;
        }

        public string getAbsolutePath(LocationPath parse_me, string user) {
            if (parse_me.EV == EnvironmentVariable.AltSavePaths) {
                LocationsCollection locs = interpretPath(parse_me.ToString());
                if (locs.Count > 0)
                    parse_me = locs.getMostAccurateLocation();
                else
                    return null;
            }

            foreach (KeyValuePair<HandlerType, ALocationHandler> handler in handlers) {
                string result = handler.Value.getAbsolutePath(parse_me, user);
                if (!String.IsNullOrEmpty(result))
                    return result;
            }
            return null;
        }

        public LocationsCollection interpretPath(params string[] interpret_me) {
            return interpretPath(true, interpret_me);
        }

        public LocationsCollection interpretPath(bool must_exist, params string[] interpret_me) {
            if (interpret_me.Length == 0)
                throw new Exception("Not enough paths!");

            string path = interpret_me[0].TrimEnd(Path.DirectorySeparatorChar);
            for (int i = 1; i < interpret_me.Length; i++) {
                path = Path.Combine(path, interpret_me[i].TrimEnd(Path.DirectorySeparatorChar));
            }

            LocationsCollection return_me = new LocationsCollection();
            string tmp = correctPath(path, must_exist);
            
            if (String.IsNullOrEmpty(tmp)) {
                return return_me;
            } else {
                path = tmp ;
            }

            foreach (KeyValuePair<HandlerType, ALocationHandler> handler in handlers) {
                return_me.AddRange(handler.Value.interpretPath(path,must_exist));
            }

            return return_me;
        }

        protected static string correctPath(string correct_me, bool must_exist) {
            string[] sections = correct_me.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            DirectoryInfo dir = new DirectoryInfo(sections[0] + Path.DirectorySeparatorChar);
            for (int i = 1; i < sections.Length; i++) {
                if (!PermissionsHelper.isReadable(dir.FullName)) {
                    // If we can't access the subfolders, then we return the best path we can get
                    string asfar = dir.FullName;
                    while (i < sections.Length) {
                        asfar = Path.Combine(asfar, sections[i]);
                        i++;
                    }
                    return asfar;
                }
                DirectoryInfo[] sub_dir = dir.GetDirectories(sections[i]);
                if (sub_dir.Length == 1) {
                    if (sub_dir[0].Exists) {
                        dir = sub_dir[0];
                        continue;
                    } else {
                        break;
                    }
                } else if (sub_dir.Length > 1) {
                    break;
                } else {
                    break;
                }
            }
            if (dir.FullName.Trim('/').Trim('\\').Length == correct_me.Trim('/').Trim('\\').Length) {
                return dir.FullName;
            } else if (!must_exist) {
                // This correction process only corrects capitalization, so the length of the string should end up the same.
                // If it is not, then that means it was unable to find the path, and in most cases we return nothing.
                // If require_exists is set to false, then we take what was found and combine it with what wasn't found, 
                // to give the most accurate path possible.
                if (dir == null || dir.FullName.Length == 0) {
                    return correct_me;
                } else {
                    string tmp1 = dir.FullName.TrimEnd(Path.DirectorySeparatorChar);
                    string tmp2 = correct_me.Substring(dir.FullName.Length).Trim('/').Trim('\\');
                    string output = Path.Combine(tmp1, tmp2);
                    return output;
                }
            } else {
                return null;
            }
        }

        public bool ready {
            get {
                return true;
            }
        }


        #region Quick-browse button path sources

        public List<QuickBrowsePath> QuickBrowsePaths {
            get {
                List<QuickBrowsePath> return_me = new List<QuickBrowsePath>();
                QuickBrowsePath new_path;

                Array values = Enum.GetValues(typeof(QuickBrowsePathEnum));
                foreach (QuickBrowsePathEnum val in values) {
                    if ((val == QuickBrowsePathEnum.Steamapps || val == QuickBrowsePathEnum.SteamUserData) && !Core.locations.steam_detected)
                        continue;
                    else if (val == QuickBrowsePathEnum.ProgramFilesX86 && String.IsNullOrEmpty(Core.locations.getFolder(EnvironmentVariable.ProgramFilesX86, null)))
                        continue;
                    else if ((val == QuickBrowsePathEnum.PublicUser || val == QuickBrowsePathEnum.VirtualStore || val == QuickBrowsePathEnum.SavedGames) &&
                        Core.locations.platform_version == "WindowsXP")
                        continue;

                    new_path = new QuickBrowsePath();
                    new_path.Name = Translator.Strings.GetLabelString(val.ToString());
                    new_path.EV = val;

                    switch (val) {
                        case QuickBrowsePathEnum.MyComputer:
                            new_path.Path = "/";
                            break;
                        case QuickBrowsePathEnum.AllUsers:
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.AllUsersProfile, null);
                            break;
                        case QuickBrowsePathEnum.LocalAppData:
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.LocalAppData, null);
                            break;
                        case QuickBrowsePathEnum.MyDocuments:
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.UserDocuments, null);
                            break;
                        case QuickBrowsePathEnum.ProgramFiles:
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.ProgramFiles, null);
                            break;
                        case QuickBrowsePathEnum.ProgramFilesX86:
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.ProgramFilesX86, null);
                            break;
                        case QuickBrowsePathEnum.PublicUser:
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.Public, null);
                            break;
                        case QuickBrowsePathEnum.RoamingAppData:
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.AppData, null);
                            break;
                        case QuickBrowsePathEnum.SavedGames:
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.SavedGames, null);
                            break;
                        case QuickBrowsePathEnum.Steamapps:
                            new_path.Path = Path.Combine(Core.settings.steam_path, "steamapps");
                            break;
                        case QuickBrowsePathEnum.SteamUserData:
                            new_path.Path = Path.Combine(Core.settings.steam_path, "userdata");
                            break;
                        case QuickBrowsePathEnum.VirtualStore:
                            //Path.Combine(Core.locations.getFolder(EnvironmentVariable.LocalAppData, null), "VirtualStore");
                            new_path.Path = Core.locations.getFolder(EnvironmentVariable.VirtualStore, null);
                            break;
                        default:
                            throw new NotSupportedException(val.ToString() + " not supported");
                    }

                    if (string.IsNullOrEmpty(new_path.Path)) {
                        continue;
                    }
                    
                    return_me.Add(new_path);
                }
                                
                // Adds alternative save paths
                foreach (Location.Holders.AltPathHolder path in Core.settings.save_paths) {
                    new_path = new QuickBrowsePath();
                    new_path.Name = abbreviateString(path.path,50);
                    new_path.Path = path.path;
                    return_me.Add(new_path);
                }


                // Adds alternative steam library paths
                foreach (string path in Core.locations.getPaths(EnvironmentVariable.SteamCommon)) {
                    if (!path.ToLower().StartsWith(Path.Combine(Core.settings.steam_path, "steamapps").ToLower())) {
                        new_path = new QuickBrowsePath();
                        new_path.Name = abbreviateString(path,50);
                        new_path.Path = path;
                        return_me.Add(new_path);
                    }
                }

                return return_me;
            }
        }

        private static string abbreviateString(string input, int max_length) {
            if (input.Length <= max_length) {
                return input;
            }
            int diff = input.Length - max_length;
            double half_max_length = (max_length - 3) / 2;

            string pre = input.Substring(0, Convert.ToInt32(Math.Floor(half_max_length)));
            string post = input.Substring(input.Length - Convert.ToInt32(Math.Ceiling(half_max_length)));
            return String.Concat(pre, "...", post);
        }
        #endregion

    }

    public enum QuickBrowsePathEnum {
        AllUsers,
        LocalAppData,
        MyComputer,
        MyDocuments,
        ProgramFiles,
        ProgramFilesX86,
        RoamingAppData,
        SavedGames,
        Steamapps,
        SteamUserData,
        VirtualStore,
        PublicUser
    }
    public class QuickBrowsePath {
        public string Name;
        public string Path;
        public QuickBrowsePathEnum EV;
        public QuickBrowsePath() {
            // TODO: Make this default multi-platform
            EV = QuickBrowsePathEnum.MyComputer;
        }

    }
}
