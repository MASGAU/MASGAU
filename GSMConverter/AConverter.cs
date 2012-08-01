using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using XmlData;
namespace GSMConverter {

    abstract class AConverter: XmlFile {
        protected MASGAU.GameXmlFile output;

        protected AConverter(string xml): base(xml) {
            FileInfo file = new FileInfo(Path.GetTempFileName());
            output = new MASGAU.GameXmlFile(file);
        }

        public XmlDocument export() {
            return output;
        }

        protected string generateName(string input) {
            return MASGAU.CustomGame.prepareGameName(input);
        }

        private void example(XmlElement entry) {
            foreach (XmlAttribute attr in entry.Attributes) {
                switch (attr.Name) {
                    default:
                        throw new NotSupportedException(attr.Name);
                }
            }
            foreach (XmlElement ele in entry.ChildNodes) {
                switch (ele.Name) {
                    default:
                        throw new NotSupportedException(ele.Name);
                }

            }
        }

    }
}
