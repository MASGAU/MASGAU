using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MASGAU.Location.Holders
{
    public class ScummVMName: ALocationHolder
    {

        public String name { get; protected set; }
        public ScummVMName(XmlElement element) {
            name = element.GetAttribute("name");
        }
    }
}
