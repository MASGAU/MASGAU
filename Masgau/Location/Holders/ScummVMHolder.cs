using System;
using System.Xml;

namespace MASGAU.Location.Holders {
    public class ScummVMHolder: ALocationHolder {

        public String Name { get; protected set; }
        public ScummVMHolder(XmlElement element) {
            foreach (XmlAttribute attrib in element.Attributes) {
                switch (attrib.Name) {
                    case "name":
                        Name = attrib.Value;
                        break;
                    default:
                        throw new NotSupportedException(attrib.Name);
                }
            }
        }
    }
}
