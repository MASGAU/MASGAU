using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location.Holders {
    public abstract class PlayStationID : LocationPathHolder {
        public string prefix, suffix, append = null, type = null;
        protected string SavePattern()
        {
            StringBuilder pattern = new StringBuilder();
            pattern.Append(prefix);
            pattern.Append(suffix);
            if(append!=null) {
                pattern.Append(append);
            }
            pattern.Append("*");
            return pattern.ToString();
        }
        protected string ExportPattern() {
            StringBuilder pattern = new StringBuilder();
            pattern.Append("BA");
            pattern.Append(prefix);
            pattern.Append("?");
            pattern.Append(suffix);
            if(append!=null) {
                pattern.Append(append);
            }
            pattern.Append("*");
            return pattern.ToString();
        }
        public abstract override string ToString();
    }
    public class PlayStation1ID : PlayStationID {
        public override string ToString()
        {
            return ExportPattern();
        }
    }
    public class PlayStation2ID : PlayStationID {
        public override string ToString()
        {
            return ExportPattern();
        }
    }
    public class PlayStation3ID : PlayStationID {
        public override string ToString()
        {
            return SavePattern();
        }
    }
    public class PlayStationPortableID : PlayStationID {
        public override string ToString()
        {
            return SavePattern();
        }
    }

}
