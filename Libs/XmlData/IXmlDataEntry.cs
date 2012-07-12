using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace XmlData {
    public interface IXmlDataEntry {
        void LoadData(XmlElement element);
    }
}
