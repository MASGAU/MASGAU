using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace GameSaveInfo {
    public class PlayStation2ID : APlayStationID {
        public PlayStation2ID(XmlElement element) : base(element) { }
        public override string ToString() {
            return ExportPattern();
        }
    }
}
