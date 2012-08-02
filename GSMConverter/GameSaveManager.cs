using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MASGAU;
using MASGAU.Location.Holders;
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
            XmlElement registry;
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

            loadDirectories(dirs,game);
        }
        private void loadDirectories(XmlElement dirs, Game game) {
            foreach (XmlElement dir in dirs.ChildNodes) {
                switch (dir.Name) {
                    case "dir":
                        loadDirectory(dir, game);
                        break;
                    default:
                        throw new NotSupportedException(dir.Name);
                }
            }
        }
        private struct reg {
            MASGAU.Registry.RegRoot hive;
            string path;
            string value;
        }

        private void loadDirectory(XmlElement dir, Game game) {
            XmlElement path = null;
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
                    default:
                        throw new NotSupportedException(dir.Name);
                }
            }
            string specialpath = path.Attributes["specialpath"].Value;

            ALocation loc = null;
            switch (specialpath) {
                case "%APPDATA%":
                    loc = new LocationPathHolder(
                    break;
                default:
                    throw new NotSupportedException(specialpath);
            }


        }


    }
}
