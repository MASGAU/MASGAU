using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace GameSaveInfo {
    public class GameIdentifier: IComparable<GameIdentifier>, IEquatable<GameIdentifier> {
        public String Name { get; protected set; }
        public String OS { get; protected set; }
        public String Platform { get; protected set; }
        public String Region { get; protected set; }
        public String Media { get; protected set; }
        public String Release { get; protected set; }


        public static readonly List<string> attributes = new List<string> { "os", "platform", "region", "media", "release", "gsm_id" };

        public GameIdentifier(string name, string release) {
            this.Name = name;
            this.Release = release;
        }
        public GameIdentifier(string name, XmlElement element): this(element) {
            this.Name = name;
        }
        public GameIdentifier(XmlElement element) {
            foreach (XmlAttribute attrib in element.Attributes) {
                if (ALocation.attributes.Contains(attrib.Name))
                    continue;

                switch (attrib.Name) {
                    case "name":
                        Name = attrib.Value;
                        break;
                    case "os":
                        OS = attrib.Value;
                        break;
                    case "platform":
                        Platform = attrib.Value;
                        break;
                    case "media":
                        Media = attrib.Value;
                        break;
                    case "release":
                        Release = attrib.Value;
                        break;
                    case "region":
                        Region = attrib.Value;
                        break;
                    case "detect":
                    case "virtualstore":
                    case "gsm_id":
                        break;
                    default:
                        throw new NotSupportedException(attrib.Name);
                }
            }
        }

        public XmlElement AddAttributes(XmlElement element) {
            XmlDocument doc = element.OwnerDocument;
            XmlAttribute attribute;
            if (!element.HasAttribute("name")) {
                attribute = doc.CreateAttribute("name");
                attribute.Value = Name;
                element.SetAttributeNode(attribute);
            }
            if (OS != null) {
                attribute = doc.CreateAttribute("os");
                attribute.Value = OS;
                element.SetAttributeNode(attribute);
            }
            if (Platform != null) {
                attribute = doc.CreateAttribute("platform");
                attribute.Value = Platform;
                element.SetAttributeNode(attribute);
            }
            if (Region != null) {
                attribute = doc.CreateAttribute("region");
                attribute.Value = Region;
                element.SetAttributeNode(attribute);
            }
            if (Media != null) {
                attribute = doc.CreateAttribute("media");
                attribute.Value = Media;
                element.SetAttributeNode(attribute);
            }
            if (Release != null) {
                attribute = doc.CreateAttribute("release");
                attribute.Value = Release;
                element.SetAttributeNode(attribute);
            }

            return element;
        }

        public static int Compare(GameIdentifier a, GameIdentifier b) {
            int result = compare(a.Name, b.Name);

            if (result == 0)
                result = compare(a.Release, b.Release);
            if (result == 0)
                result = compare(a.OS, b.OS);
            if (result == 0)
                result = compare(a.Platform, b.Platform);
            if (result == 0)
                result = compare(a.Region, b.Region);
            if (result == 0)
                result = compare(a.Media, b.Media);
            return result;
        }

        public int CompareTo(GameIdentifier comparable) {
            return Compare(this, comparable);
        }

        public static bool Equals(GameIdentifier a, GameIdentifier b) {
            return Compare(a, b) == 0;
        }

        public Boolean Equals(GameIdentifier to_me) {
            return Equals(this, to_me as GameIdentifier);
        }

        public static String ToString(GameIdentifier id) {
            StringBuilder return_me = new StringBuilder(id.Name);

            if (id.Release != null)
                return_me.Append(" " + id.Release);
            if (id.OS != null)
                return_me.Append(" " + id.OS);
            if (id.Platform != null)
                return_me.Append(" " + id.Platform);
            if (id.Region != null)
                return_me.Append(" " + id.Region);
            if (id.Media != null)
                return_me.Append(" " + id.Media);

            return return_me.ToString();
        }

        public override int GetHashCode() {
            int re = Name.GetHashCode();
            if (OS != null)
                re += OS.GetHashCode();
            if (Platform != null)
                re += Platform.GetHashCode();
            if (Region != null)
                re += Region.GetHashCode();
            if (Media != null)
                re += Media.GetHashCode();
            if (Release != null)
                re += Release.GetHashCode();

            return re;
        }

        protected static int compare(IComparable a, IComparable b) {
            if (a == null) {
                if (b == null)
                    return 0;
                else
                    return -1;
            } else {
                return a.CompareTo(b);
            }
        }

    }
}
