using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using GameSaveInfo;
namespace MASGAU.Update {
    class DataUpdate: AUpdate {
        public string Name { get; protected set; }

        public DataUpdate(XmlElement xml): base(xml) {
            this.Name = xml.Attributes["name"].Value;

        }

        public override bool Update() {
//            GameXmlFile file = Games.xml.getFile(this.Name);
            return this.downloadHelper(Path.Combine(Games.GameDataFolder,this.Name));
        }

        public override int CompareTo(AUpdate update) {
            return this.Date.CompareTo(update.Date);
        }

        public override string getName() {
            return Name;
        }

        public override bool UpdateAvailable {
            get {
                GameXmlFile file = Games.xml.getFile(this.Name);
                if (file == null)
                    return true;

                return this.Date > file.date;                    
            }
        }

    }
}
