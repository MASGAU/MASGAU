using System.Text;
using System.Xml;
namespace MASGAU.Location.Holders {
    public abstract class PlayStationID : LocationPathHolder {
        public string prefix, suffix, append = null, type = null;
        protected PlayStationID(XmlElement element) {
            this.prefix = element.GetAttribute("prefix");
            this.suffix = element.GetAttribute("suffix");

            if (element.HasAttribute("append"))
                this.append = element.GetAttribute("append");
            if (element.HasAttribute("type"))
                this.type = element.GetAttribute("type");
        }


        protected string SavePattern() {
            StringBuilder pattern = new StringBuilder();
            pattern.Append(prefix);
            pattern.Append(suffix);
            if (append != null) {
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
            if (append != null) {
                pattern.Append(append);
            }
            pattern.Append("*");
            return pattern.ToString();
        }
        public abstract override string ToString();
    }



    public class PlayStation1ID : PlayStationID {
        public PlayStation1ID(XmlElement element) : base(element) { }
        public override string ToString() {
            return ExportPattern();
        }
    }
    public class PlayStation2ID : PlayStationID {
        public PlayStation2ID(XmlElement element) : base(element) { }
        public override string ToString() {
            return ExportPattern();
        }
    }
    public class PlayStation3ID : PlayStationID {
        public PlayStation3ID(XmlElement element) : base(element) { }
        public override string ToString() {
            return SavePattern();
        }
    }
    public class PlayStationPortableID : PlayStationID {
        public PlayStationPortableID(XmlElement element) : base(element) { }
        public override string ToString() {
            return SavePattern();
        }
    }

}
