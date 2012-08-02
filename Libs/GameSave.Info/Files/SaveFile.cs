using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace GameSaveInfo {
    public class SaveFile: ExceptFile {
        public List<ExceptFile> Excepts = new List<ExceptFile>();

        public SaveFile(string name, string path, string type, XmlDocument doc)
            : base(name, path, type, doc) {
        }

        public SaveFile(XmlElement element, string type)
            : base(element, type) {
        }

        protected override void LoadMoreData(XmlElement element) {
            foreach (XmlElement child in element.ChildNodes) {
                switch (child.Name) {
                    case "except":
                        ExceptFile except = new ExceptFile(child, Type);
                        Excepts.Add(except);
                        break;
                    default:
                        throw new NotSupportedException(child.Name);
                }
            }
        }

        protected override XmlElement WriteMoreData(XmlElement element) {
            foreach (ExceptFile ex in Excepts) {
                element.AppendChild(ex.XML);
            }
            return element;
        }
    }
}
