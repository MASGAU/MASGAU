using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
namespace XmlData {
    public abstract class AXmlDataFile<T> : XmlFile where T : AXmlDataEntry {
        protected AXmlDataFile(FileInfo file, bool create): base(file, create) {
            Entries.Clear();
            if (DocumentElement == null) {
                this.AppendChild(CreatRootNode());
            }


            foreach (XmlElement element in DocumentElement.ChildNodes) {
//                try {
                    T entry = CreateDataEntry(element);
                    Entries.Add(entry);
  //              } catch (Exception e) {
    //                continue;
      //          }
            }
        }

        public void removeEntry(T entry) {
            this.DocumentElement.RemoveChild(entry.XML);
            this.Entries.Remove(entry);
        }

        public List<T> Entries = new List<T>();
        public void sortEntries() {
            Entries.Sort();
        }
//        protected virtual XmlElement LoadRootNode(string name) {
//            foreach (XmlNode node in this.ChildNodes) {

//                if (node.Name == name) {
//                    return (XmlElement)node;
//                }
//            }

//            return (XmlElement)this.AppendChild(this.CreatRootNode(name));
////            throw new XmlException("Missing root: " + name);
//        }

        protected abstract XmlElement CreatRootNode();

        protected abstract T CreateDataEntry(XmlElement element);

        public void Add(T entry) {
            XmlElement ele = entry.XML;
            if (ele != null) {
                DocumentElement.AppendChild(ele);
                Entries.Add(entry);
            }
        }

    }
}
