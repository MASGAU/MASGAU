using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location.Holders {
    public class LocationShortcutHolder : ALocationHolder {
        
        // Used when dealing with a shortcut
        
        public string shortcut;

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationShortcutHolder location = (LocationShortcutHolder)comparable;
            int result = shortcut.CompareTo(location.shortcut);
            return result;
        }
    }
}
