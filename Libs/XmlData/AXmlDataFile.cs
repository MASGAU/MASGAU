using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
namespace XmlData {
    public abstract class AXmlDataFile<T> : XmlFile where T : AXmlDataEntry {

        public XmlElement RootNode;
        protected AXmlDataFile(FileInfo file, string root_element_name, bool create): base(file, create) {


            RootNode = LoadRootNode(root_element_name);

            entries.Clear();
            foreach (XmlElement element in RootNode.ChildNodes) {
//                try {
                    T entry = CreateDataEntry(element);
                    entries.Add(entry);
  //              } catch (Exception e) {
    //                continue;
      //          }
            }
        }

        public void removeEntry(T entry) {
            XmlElement ele = entry.exportXml();
            this.RootNode.RemoveChild(ele);
            this.entries.Remove(entry);
        }

        public List<T> entries = new List<T>();

        protected virtual XmlElement LoadRootNode(string name) {
            foreach (XmlNode node in this.ChildNodes) {

                if (node.Name == name) {
                    return (XmlElement)node;
                }
            }

            return (XmlElement)this.AppendChild(this.CreateElement(name));
//            throw new XmlException("Missing root: " + name);
        }

        protected abstract T CreateDataEntry(XmlElement element);

        public void Add(T entry) {
            XmlElement ele = entry.exportXml();
            if (ele != null) {
                RootNode.AppendChild(ele);
                entries.Add(entry);
            }
        }

    }
}
