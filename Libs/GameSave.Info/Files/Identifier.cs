using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace GameSaveInfo {
    public class Identifier: AFile {
        public Identifier(XmlElement element) : base(element) { }

        public override string ElementName {
            get { return "identifier"; }
        }

        protected override void LoadMoreData(XmlElement element) {
            
        }

        protected override XmlElement WriteMoreData(XmlElement element) {
            return element;
        }
    }
}
