using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XmlData;
using GameSaveInfo;
namespace MASGAU {
    public class CustomGameXmlFile: GameXmlFile {
        public CustomGameXmlFile(FileInfo file)
            : base(file) {



        }

        protected override GameSaveInfo.Game CreateDataEntry(System.Xml.XmlElement element) {
            return new CustomGame(element);
        }


        public CustomGame createCustomGame(string title, DirectoryInfo location, string saves, string ignores) {
            CustomGame game = new CustomGame(title, location, saves, ignores, this);
            
            this.Add(game);
            return game;
        }

    }
}
