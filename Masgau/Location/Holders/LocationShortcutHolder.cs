using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace MASGAU.Location.Holders {
    public class LocationShortcutHolder : ALocationHolder {
        
        // Used when dealing with a shortcut
        public EnvironmentVariable ev;
        public string path;

        public LocationShortcutHolder(XmlElement element): base(element)
        {
            this.ev = parseEnvironmentVariable(element.GetAttribute("environment_variable"));
            this.path = element.GetAttribute("path");

        }

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationShortcutHolder location = (LocationShortcutHolder)comparable;
            int result = ev.CompareTo(location.ev);
            if(result==0)
               result = path.CompareTo(location.path);
            return result;
        }
    }
}
