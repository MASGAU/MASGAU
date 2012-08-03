using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using XmlData;
using GameSaveInfo;
namespace GSMConverter {

    abstract class AConverter: XmlFile {
        public  GameXmlFile output;

        protected AConverter(string xml): base(xml) {
            FileInfo file = new FileInfo(Path.GetTempFileName());
            output = new GameXmlFile(file);
        }

        public XmlDocument export() {
            return output;
        }

        protected string generateName(string input) {
            return Game.prepareGameName(input);
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
