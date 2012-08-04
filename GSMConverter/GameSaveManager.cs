using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GameSaveInfo;
namespace GSMConverter {
    class GameSaveManager: AConverter {

        public GameSaveManager(string xml): base(xml) {
            foreach (XmlElement entry in this.DocumentElement.ChildNodes) {
                if (entry.Name != "entry")
                    throw new NotSupportedException(entry.Name);

                loadEntry(entry);
            }
        }

        private void loadEntry(XmlElement entry) {
            foreach (XmlAttribute attr in entry.Attributes) {
                switch (attr.Name) {
                    case "new":
                    case "id":
                        break;
                    default:
                        throw new NotSupportedException(attr.Name);
                }
            }
            string title = null, backupwarning, restorewarning;
            XmlElement dirs = null;
            XmlElement registry = null;
            foreach (XmlElement ele in entry.ChildNodes) {
                switch (ele.Name) {
                    case "title":
                        title = ele.InnerText;
                        break;
                    case "backupwarning":
                        backupwarning = ele.InnerText;
                        break;
                    case "restorewarning":
                        restorewarning = ele.InnerText;
                        break;
                    case "directories":
                        dirs = ele;
                        break;
                    case "registry":
                        registry = ele;
                        break;
                    default:
                        throw new NotSupportedException(ele.Name);
                }
            }
            string name = generateName(title);

            Game game = output.getGame(name);
            if (game == null) {
                game = new Game(name, title, null, false, GameType.game, output);
                output.Add(game);
            }

            if (game.Versions.Count == 0) {
                GameVersion ver = new GameVersion(game,"Windows",null);
                game.addVersion(ver);
            }
            GameVersion version = game.Versions[0];


            if(dirs!=null)
                loadDirectories(dirs,version);


            if (registry != null)
                loadRegistries(registry, version);

            version.addContributor("GameSaveManager");
            
        }


        private void loadDirectories(XmlElement dirs, GameVersion version) {
            foreach (XmlElement dir in dirs.ChildNodes) {
                switch (dir.Name) {
                    case "dir":
                        loadDirectory(dir, version);
                        break;
                    default:
                        throw new NotSupportedException(dir.Name);
                }
            }
        }

        private void loadDirectory(XmlElement dir, GameVersion version) {
            XmlElement path = null, reg = null;
            string include = null, exclude = null;
            foreach (XmlElement ele in dir.ChildNodes) {
                switch (ele.Name) {
                    case "path":
                        path = ele;
                        break;
                    case "include":
                        include = ele.InnerText;
                        break;
                    case "exclude":
                        exclude = ele.InnerText;
                        break;
                    case "reg":
                        reg = ele;
                        break;
                    default:
                        throw new NotSupportedException(ele.Name);
                }
            }


            string specialpath = path.Attributes["specialpath"].Value;

            ALocation loc = null;
            EnvironmentVariable ev =  EnvironmentVariable.None;
            
            RegRoot reg_root = RegRoot.none;
            string reg_key = null;
            string reg_value = null;

            string rel_path = path.InnerText;
            bool linkable = false;
            switch (specialpath) {
                case "%REGISTRY%":
                    reg_root = getRegRoot(reg);
                    reg_key = getRegKey(reg);
                    reg_value = getRegValue(reg);
                    break;
                case "%APPDATA%":
                    ev = EnvironmentVariable.AppData;
                    linkable = true;
                    break;
                case "%DOCUMENTS%":
                    ev = EnvironmentVariable.UserDocuments;
                    linkable = true;
                    break;
                case "%APPDATA_COMMON%":
                    ev = EnvironmentVariable.CommonApplicationData;
                    linkable = true;
                    break;
                case "%APPDATA_LOCAL%":
                    ev = EnvironmentVariable.LocalAppData;
                    linkable = true;
                    break;
                case "%SAVED_GAMES%":
                    ev = EnvironmentVariable.SavedGames;
                    linkable = true;
                    break;
                case "%USER_PROFILE%":
                    ev = EnvironmentVariable.UserProfile;
                    linkable = true;
                    break;
                case "%SHARED_DOCUMENTS%":
                    ev = EnvironmentVariable.Public;
                    rel_path = System.IO.Path.Combine("Documents",rel_path);
                    linkable = true;
                    break;
                case "%STEAM_CLOUD%":
                    ev = EnvironmentVariable.SteamUserData;
                    break;
                case "%STEAM_CACHE%":
                    ev = EnvironmentVariable.SteamUser;
                    linkable = true;
                    break;
                case "%STEAM%":
                    if (rel_path.StartsWith("steamapps/common/")) {
                        ev = EnvironmentVariable.SteamCommon;
                        rel_path = rel_path.Substring(17).Trim(System.IO.Path.DirectorySeparatorChar);
                    } else if (rel_path.StartsWith("steamapps/sourcemods/")) {
                        ev = EnvironmentVariable.SteamSourceMods;
                        rel_path = rel_path.Substring(21).Trim(System.IO.Path.DirectorySeparatorChar);
                    } else {
                        throw new NotSupportedException(rel_path);
                    }
                    linkable = true;
                    break;
                case "%UPLAY%":
                    ev = EnvironmentVariable.UbisoftSaveStorage;
                    break;
                default:
                    throw new NotSupportedException(specialpath);
            }


            if(ev!= EnvironmentVariable.None) {
                loc = new LocationPath(version.Locations,ev, rel_path);
            } else {
                loc = new LocationRegistry(version.Locations, reg_root.ToString(), reg_key, reg_value);
                if (rel_path != null && rel_path != "")
                    loc.Append = rel_path;
            }

            version.addLocation(loc);

            FileType type = version.addFileType("Saves");

            foreach (string inc in include.Split('|')) {
                Include save;
                if (inc == "*.*"||inc=="*") {
                    save = type.addSave(null, null);
                } else {
                    save = type.addSave(null, inc);
                }
                foreach (string exc in exclude.Split('|')) {
                    if(exc!="")
                        save.addExclusion(null, exc);
                }

            }

            if (linkable) {
                version.addLink(null);
            }

        }

        private RegRoot getRegRoot(XmlElement reg) {
            foreach (XmlElement ele in reg) {
                switch (ele.Name) {
                    case "hive":
                        return LocationRegistry.parseRegRoot(ele.InnerText);
                    case "path":
                        break;
                    case "value":
                        break;
                    default:
                        throw new NotSupportedException(ele.Name);
                }
            }
            throw new KeyNotFoundException();
        }

        private string getRegKey(XmlElement reg) {
            foreach (XmlElement ele in reg) {
                switch (ele.Name) {
                    case "hive":
                        break;
                    case "path":
                        return ele.InnerText;
                    case "value":
                        break;
                    default:
                        throw new NotSupportedException(ele.Name);
                }
            }
            throw new KeyNotFoundException();
        }
        private string getRegValue(XmlElement reg) {
            foreach (XmlElement ele in reg) {
                switch (ele.Name) {
                    case "hive":
                        break;
                    case "path":
                        break;
                    case "value":
                        return ele.InnerText;
                    default:
                        throw new NotSupportedException(ele.Name);
                }
            }
            return null;
        }

        private void loadRegistries(XmlElement reg, GameVersion version) {
            foreach (XmlElement ele in reg.ChildNodes) {
                switch (ele.Name) {
                    case "reg":
                        loadRegistry(ele, version);
                        break;
                    default:
                        throw new NotSupportedException(ele.Name);
                }
            }
        }
        private void loadRegistry(XmlElement reg, GameVersion version) {
            RegRoot root = RegRoot.none;;
            string key = null, values = null;

            foreach (XmlElement ele in reg.ChildNodes) {
                switch (ele.Name) {
                    case "hive":
                        root = LocationRegistry.parseRegRoot(ele.InnerText);
                        break;
                    case "path":
                        key = ele.InnerText;
                        break;
                    case "values":
                        values = ele.InnerText;
                        break;
                    default:
                        throw new NotSupportedException(ele.Name);
                }
            }

            version.addRegEntry(root, key, values, "Saves");

        }


    }
}
