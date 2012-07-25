using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XmlData;

namespace MASGAU {
    public class GameXmlFile: AXmlDataFile<Game> {

        public GameXmlFile(FileInfo file): base(file,"programs",true) {

        }

        protected override Game CreateDataEntry(System.Xml.XmlElement element) {
            return new Game(element);
        }

        public Game getGame(string name) {
            foreach (Game game in this.entries) {
                if (game.Name == name)
                    return game;
            }
            return null;
        }
    }
}
