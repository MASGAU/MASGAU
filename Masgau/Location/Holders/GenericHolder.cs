using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location.Holders {
    // This lets me abstract out the loading logic for those below :D
    public class GenericHolder {
        public string path, name;
        public string type;
        public DateTime modified_after;
    }
    // This holds the save detection information loaded straight from the XML file
    public class SaveHolder : GenericHolder {
    }
    // This holds the ignore information loaded straight from the XML file
    public class IgnoreHolder : GenericHolder {
    }

    // This holds the information for an identifier
    public class IdentifierHolder : GenericHolder {
    }

}
