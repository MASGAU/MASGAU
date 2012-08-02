using System;
using System.Xml;
using XmlData;
namespace GameSaveInfo {
    public class ScummVM: ALocation {

        public String Name { get; protected set; }

        public override string ElementName {
            get { return "scummvm"; }
        }

        public ScummVM(XmlElement element): base(element) {
        }

        protected override void LoadData(XmlElement element) {
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
        protected override void LoadMoreData(XmlElement element) {
            throw new NotImplementedException();
        }
        protected override XmlElement WriteData(XmlElement element) {
            addAtribute(element, "name", Name);
            return element;
        }
        protected override XmlElement WriteMoreData(XmlElement element) {
            throw new NotImplementedException();
        }

        public override int CompareTo(ALocation comparable) {
            ScummVM scumm = comparable as ScummVM;
            return this.Name.CompareTo(scumm.Name);
        }
    }
}
