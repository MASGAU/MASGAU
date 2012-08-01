using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MASGAU;
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
            XmlElement dirs;
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



        }


    }
}
