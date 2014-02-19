using System;
using System.Collections.Generic;
using System.IO;
using GameSaveInfo;
using MASGAU.Location.Holders;
using MVC;
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

        protected void addUserEv(string user, EnvironmentVariable ev, string name, string folder) {
            UserData user_data;
            if (Contains(user))
                user_data = getUser(user);
            else {
                user_data = new UserData(user);
                this.AddWithSort(user_data);
            }
            user_data.addEvFolder(ev, name, folder);

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

            foreach (UserData user in this) {
                if (!user.hasFolderFor(get_me.EV))
                    continue;

                EvFolder evf = user.getFolder(get_me.EV);
                foreach (DetectedLocationPathHolder add_me in evf.createDetectedLocations(get_me, user.name)) {
                    if (add_me.Exists) {
                        return_me.Add(add_me);
                    }
                }
            }
            if (global.hasFolderFor(get_me.EV)) {
                foreach (DetectedLocationPathHolder add_me in global.getFolder(get_me.EV).createDetectedLocations(get_me, null)) {
                    if (add_me.Exists) {
                        return_me.Add(add_me);
                    }
                }
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
                return_me.AddRange(global.getFolder(for_me).Folders);
            }
            foreach (UserData user in this) {
                if (user.hasFolderFor(for_me))
                    return_me.AddRange(user.getFolder(for_me).Folders);
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
                    return global.getFolder(parse_me.EV).BaseFolder;
                }
            } else {
                foreach (UserData user_data in this) {
                    if (user_data.name == user && user_data.hasFolderFor(parse_me.EV)) {
                        return user_data.getFolder(parse_me.EV).BaseFolder;
                    }
                }
            }
            return null;
        }


        // Returns a DetectedLocation that represents the chosen string
        public virtual List<LocationPath> interpretPath(string interpret_me, bool must_exist) {
            List<LocationPath> return_me = new List<LocationPath>();
            LocationPath new_location;
            if (ready) {
                // this needs to be able to interpret user paths too!
                foreach (KeyValuePair<EnvironmentVariable, EvFolder> variable in global.folders) {
                    if (variable.Value != null && variable.Value.Matches(interpret_me)) {
                        string path;
                        foreach (string folder in variable.Value.Folders) {
							if (interpret_me.ToLower().StartsWith(folder.ToLower())) {
								if (interpret_me.Length == folder.Length) {
									path = "";
								} else {
									path = interpret_me.Substring(folder.Length + 1);
								}

								new_location = new LocationPath(variable.Key, path);
                                DetectedLocations detected = getPaths(new_location);
                                if (detected.Count > 0) {
                                    return_me.AddRange(detected);
                                } else if (!must_exist) {
                                    return_me.Add(new_location);
                                }
							}
						}

                    }
                }
                foreach (UserData user in this) {
                    foreach (KeyValuePair<EnvironmentVariable, EvFolder> variable in user.folders) {
                        if (variable.Value != null && variable.Value.Matches(interpret_me)) {
                            string path;
                            foreach (string folder in variable.Value.Folders) {
                                if (!interpret_me.StartsWith(folder))
                                    continue;

                                if (interpret_me.Length == folder.Length)
                                    path = "";
                                else
                                    path = interpret_me.Substring(folder.Length + 1);
                                new_location = new LocationPath(variable.Key, path);

                                
                                DetectedLocations detected = getPaths(new_location);
                                if (detected.Count > 0) {
                                    return_me.AddRange(detected);
                                } else if (!must_exist) {
                                    return_me.Add(new_location);
                                }
                            }
                        }
                    }
                }
            }
            return return_me;

        }
    }
}
