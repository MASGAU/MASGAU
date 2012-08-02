using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace GameSaveInfo {
    public class Link {
        XmlElement xml = null;

        public bool Linked = false;

        private string path = null;
        public Link(GameVersion game, XmlElement ele) {
            if (ele.HasAttribute("path"))
                path = ele.Attributes["path"].Value;

            xml = ele;
        }


    }
}
