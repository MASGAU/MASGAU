using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location.Holders {
    public class LocationShortcutHolder : ALocationHolder {
        
        // Used when dealing with a shortcut
        public EnvironmentVariable ev;
        public string path;

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationShortcutHolder location = (LocationShortcutHolder)comparable;
            int result = ev.CompareTo(location.ev);
            if(result==0)
               result = path.CompareTo(location.path);
            return result;
        }
    }
}
