using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace GameSaveInfo {
    public class ExceptFile: AFile {
        public string Type { get; protected set; }

        public override string ElementName {
            get { return "except"; }
        }

        public ExceptFile(string name, string path, string type, XmlDocument doc): base(name,path, doc) {
            this.Type = type;
        }

        public ExceptFile(XmlElement element, string type)
            : base(element) {
                this.Type = type;
        }

        protected override void LoadMoreData(XmlElement element) {
            
        }

        protected override XmlElement WriteMoreData(XmlElement element) {
            return element;
        }
    }
}
