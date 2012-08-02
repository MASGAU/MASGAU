using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using XmlData;
namespace GameSaveInfo {
    public class Locations: AXmlDataEntry {
        public List<LocationPath> Paths = new List<LocationPath>();
        public List<LocationRegistry> Registries = new List<LocationRegistry>();
        public List<LocationShortcut> Shortcuts = new List<LocationShortcut>();
        public List<LocationParent> Parents = new List<LocationParent>();
        public List<ALocation> AllLocations {
            get {
                List<ALocation> return_me = new List<ALocation>();
                return_me.AddRange(Paths);
                return_me.AddRange(Registries);
                return_me.AddRange(Shortcuts);
                return_me.AddRange(Parents);
                return return_me;
            }
        }


        public override string ElementName {
            get { return "locations"; }
        }

        public Locations(XmlElement xml): base(xml) {

        }

        protected override void LoadData(XmlElement element) {
            foreach (XmlElement sub in element.ChildNodes) {
                switch (sub.Name) {
                    case "path":
                        Paths.Add(new LocationPath(sub));
                        break;
                    case "registry":
                        Registries.Add(new LocationRegistry(sub));
                        break;
                    case "shortcut":
                        Shortcuts.Add(new LocationShortcut(sub));
                        break;
                    case "parent":
                        Parents.Add(new LocationParent(sub));
                        break;
                    default:
                        throw new NotSupportedException(sub.Name);
                }
            }
        }

        protected override XmlElement WriteData(XmlElement element) {
            foreach (ALocation path in AllLocations) {
                XmlElement xp = path.XML;
                element.AppendChild(xp);
            }
            return element;
        }
    }
}
