using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
namespace XmlData {
    public abstract class AXmlDataFile<T> : XmlFile where T : IXmlDataEntry {

        public XmlElement RootNode;
        protected AXmlDataFile(FileInfo file, string root_element_name): base(file) {
            RootNode = LoadRootNode(root_element_name);

            entries.Clear();
            foreach (XmlElement element in RootNode.ChildNodes) {
//                try {
                    T entry = CreateDataEntry();
                    entry.LoadData(element);
                    entries.Add(entry);
  //              } catch (Exception e) {
    //                continue;
      //          }
            }
        }

        public List<T> entries = new List<T>();

        protected virtual XmlElement LoadRootNode(string name) {
            foreach (XmlNode node in this.ChildNodes) {

                if (node.Name == name) {
                    return (XmlElement)node;
                }
            }
            throw new XmlException("Missing root: " + name);
        }

        protected abstract T CreateDataEntry();


    }
}
