using System.Xml;
using MVC;
using System;
namespace MASGAU.Location.Holders {
    public class LocationShortcutHolder : ALocationHolder {

        // Used when dealing with a shortcut
        public EnvironmentVariable ev;
        public string path;

        public LocationShortcutHolder(XmlElement element)
            : base(element) {
            foreach (XmlAttribute attrib in element.Attributes) {
                if (attributes.Contains(attrib.Name))
                    continue;

                switch (attrib.Name) {
                    case "ev":
                        this.ev = parseEnvironmentVariable(attrib.Value);
                        break;
                    case "path":
                        this.path = attrib.Value;
                        break;
                    default:
                        throw new NotSupportedException(attrib.Name);
                }
            }
        }

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationShortcutHolder location = (LocationShortcutHolder)comparable;
            int result = ev.CompareTo(location.ev);
            if (result == 0)
                result = path.CompareTo(location.path);
            return result;
        }
    }
}
