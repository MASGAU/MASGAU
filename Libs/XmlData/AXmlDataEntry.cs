using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace XmlData {
    public abstract class AXmlDataEntry {
        public AXmlDataEntry(XmlElement element) {
            LoadData(element);
            foreach(XmlAttribute attribute in element.Attributes) {

            }
            foreach (XmlElement sub in element.ChildNodes) {

            }
        }

        protected abstract void LoadData(XmlElement element);


    }
}
