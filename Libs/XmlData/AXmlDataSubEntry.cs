using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace XmlData {
    public abstract class AXmlDataSubEntry: AXmlDataEntry {

        public override XmlFile SourceFile {
            get {
                if (Parent == null)
                    return null;
                return Parent.SourceFile;
            }
        }

        protected AXmlDataEntry Parent = null;

        protected AXmlDataSubEntry() : base() { }

        protected AXmlDataSubEntry(AXmlDataEntry parent): base(parent.Doc) {
            this.Parent = parent;
        }

        public AXmlDataSubEntry(AXmlDataEntry parent, XmlElement element)
            : base(element.OwnerDocument) {
                this.Parent = parent;
                xml = element;
                LoadData(element);
        }


    }
}
