using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace XmlData {
    public abstract class AXmlDataEntry {
        protected XmlDocument doc = null;
        protected XmlElement xml = null;
        public AXmlDataEntry(XmlDocument document) {
            this.doc = document;
        }

        public AXmlDataEntry(XmlElement element): this(element.OwnerDocument) {
            xml = element;
            LoadData(element);
        }

        public XmlElement addAtribute(XmlElement ele, string name, string contents) {
            XmlAttribute att= this.doc.CreateAttribute(name);
            att.Value = contents;
            ele.Attributes.Append(att);
            return ele;
        }

        public XmlElement createElement(string name, string contents) {
            XmlElement ele = createElement(name);
            ele.InnerText = contents;
            return ele;
        }
        public XmlElement createElement(string name) {
            return this.doc.CreateElement(name);
        }

        protected abstract void LoadData(XmlElement element);
        public abstract XmlElement exportXml();
    }
}
