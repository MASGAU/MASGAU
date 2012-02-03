using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace MASGAU.Game {
    public class GameXMLHolder {
        public readonly GameID id;
        public readonly XmlElement xml;
        public GameXMLHolder(GameID id, XmlElement xml) {
            this.id = id;
            this.xml = xml;
        }
    }
}
