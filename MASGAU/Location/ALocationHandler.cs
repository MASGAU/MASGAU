using System;
using System.Collections.Generic;
using System.IO;
using MASGAU.Location.Holders;
using MVC;
using GameSaveInfo;
namespace MASGAU.Location {
    public abstract class ALocationHandler : Model<StringID, UserData> {
        public HandlerType type {
            get;
            private set;
        }
        protected UserData global;

        // Checks if the particular path handler is ready
        public abstract bool ready { get; }


        public ALocationHandler(HandlerType type) {
            global = new UserData("global");
            this.type = type;
        }

        protected void setUserEv(string user, EnvironmentVariable ev, string folder) {
            UserData user_data;
            if (Contains(user))
                user_data = getUser(user);
            else {
                user_data = new UserData(user);
                this.AddWithSort(user_data);
            }
            user_data.setEvFolder(ev, folder);
        }

        protected UserData getUser(string get_me) {
            foreach (UserData user in this) {
                if (user.name == get_me)
                    return user;
            }
            return null;
        }

        protected bool Contains(string find_me) {
            return getUser(find_me) != null;
        }

        // Return location holders based on various kinds of input
        public virtual DetectedLocations getPaths(ALocation get_me) {
            Type check = get_me.GetType();
            if (check.Equals(typeof(ScummVM))) {
                return getPaths(get_me as ScummVM);
            } else if (check.IsSubclassOf(typeof(APlayStationID))) {
                return getPaths(get_me as APlayStationID);
            } else if (check.Equals(typeof(LocationPath)) || check.IsSubclassOf(typeof(LocationPath))) {
                return getPaths(get_me as LocationPath);
            } else if (check.Equals(typeof(LocationRegistry))) {
                return getPaths(get_me as LocationRegistry);
            } else if (check.Equals(typeof(LocationShortcut))) {
                return getPaths(get_me as LocationShortcut);
            } else {
                throw new NotSupportedException(get_me.GetType().ToString());
            }
//            return new DetectedLocations();
        }

        protected virtual DetectedLocations getPaths(LocationPath get_me) {
            DetectedLocations return_me = new DetectedLocations();
            if (!ready)
                return return_me;

            DetectedLocationPathHolder add_me;
            foreach (UserData user in this) {
                if (!user.hasFolderFor(get_me.EV))
                    continue;
                add_me = new DetectedLocationPathHolder(get_me);
                add_me.owner = user.name;
                add_me.AbsoluteRoot = user.getFolder(get_me.EV);
                if (add_me.exists) {
                    return_me.Add(add_me);
                }
            }
            if (global.hasFolderFor(get_me.EV)) {
                add_me = new DetectedLocationPathHolder(get_me);
                add_me.owner = null;
                add_me.AbsoluteRoot = global.getFolder(get_me.EV);
                if (add_me.exists)
                    return_me.Add(add_me);
            }
            return return_me;
        }

        protected virtual DetectedLocations getPaths(LocationRegistry get_me) {
            return new DetectedLocations();
        }

        protected virtual DetectedLocations getPaths(LocationShortcut get_me) {
            return new DetectedLocations();
        }

        protected virtual DetectedLocations getPaths(APlayStationID get_me) {
            return new DetectedLocations();
        }

        protected virtual DetectedLocations getPaths(ScummVM get_me) {
            return new DetectedLocations();
        }
        public List<string> getPaths(EnvironmentVariable for_me) {
            List<string> return_me = new List<string>();

            if (global.hasFolderFor(for_me)) {
                return_me.Add(global.getFolder(for_me));
            }
            foreach (UserData user in this) {
                if (user.hasFolderFor(for_me))
                    return_me.Add(user.getFolder(for_me));
            }

            return return_me;
        }

        // Return a list of users associated with the specified environment variable
        public List<string> getUsers(EnvironmentVariable for_me) {
            List<string> return_me = new List<string>();
            foreach (UserData user in this) {
                if (user.hasFolderFor(for_me))
                    return_me.Add(user.name);
            }
            return return_me;
        }

        // Gets the absolute root of the provided location
        public virtual string getAbsolutePath(LocationPath parse_me, string user) {
            string return_me = getAbsoluteRoot(parse_me, user);

            if (return_me != null && parse_me.Path != null && parse_me.Path != "")
                return_me = Path.Combine(return_me, parse_me.Path);

            return return_me;
        }


        public virtual string getAbsoluteRoot(LocationPath parse_me, string user) {
            if (user == null) {
                if (global.hasFolderFor(parse_me.EV)) {
                    return global.getFolder(parse_me.EV);
                }
            } else {
                foreach (UserData user_data in this) {
                    if (user_data.name == user && user_data.hasFolderFor(parse_me.EV)) {
                        return user_data.getFolder(parse_me.EV);
                    }
                }
            }
            return null;
        }


        // Returns a DetectedLocation that represents the chosen string
        public virtual List<DetectedLocationPathHolder> interpretPath(string interpret_me) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            LocationPath new_location;
            if (ready) {
                // this needs to be able to interpret user paths too!
                foreach (KeyValuePair<EnvironmentVariable, EvFolder> variable in global.folders) {
                    if (variable.Value != null && matches(variable.Value.getFolder(), interpret_me)) {
                        string path;
                        if (interpret_me.Length == variable.Value.getFolder().Length)
                            path = "";
                        else
                            path = interpret_me.Substring(variable.Value.getFolder().Length + 1);


                        new_location = new LocationPath(variable.Key,path);
                        return_me.AddRange(getPaths(new_location));
                    }
                }
                foreach (UserData user in this) {
                    foreach (KeyValuePair<EnvironmentVariable, EvFolder> variable in user.folders) {
                        if (variable.Value != null && matches(variable.Value.getFolder(), interpret_me)) {
                            string path;
                            if (interpret_me.Length == variable.Value.getFolder().Length)
                                path = "";
                            else
                                path = interpret_me.Substring(variable.Value.getFolder().Length + 1);
                            new_location = new LocationPath(variable.Key,path);
                            return_me.AddRange(getPaths(new_location));
                        }
                    }
                }
            }
            return return_me;

        }

        protected static bool matches(string root, string path) {
            string[] split = path.Split(Path.DirectorySeparatorChar);
            for (int i = 0; i < split.Length; i++) {
                string new_path = split[0] + Path.DirectorySeparatorChar;
                for (int j = 1; j <= i; j++) {
                    new_path = Path.Combine(new_path, split[j]);
                }
                if (new_path.ToLower().Equals(root.ToLower()))
                    return true;
            }
            return false;
        }

    }
}
