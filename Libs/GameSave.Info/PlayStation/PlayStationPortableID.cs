using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace GameSaveInfo {
    public class PlayStationPortableID : APlayStationID {
        public PlayStationPortableID(XmlElement element) : base(element) { }
        public override string ToString() {
            return SavePattern();
        }
    }
}
