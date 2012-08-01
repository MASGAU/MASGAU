using System;
using System.Text;
using System.Collections.Generic;
using MVC;
using System.Xml;
namespace MASGAU {
    public class GameID : AIdentifier  {
        public String Name { get; protected set; }
        public String OS { get; protected set; }
        public String Platform { get; protected set; }
        public String Region { get; protected set; }
        public String Media { get; protected set; }
        public String Release { get; protected set; }

        public static readonly List<string> attributes = new List<string> { "os", "platform","region","media","release" };

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
        public GameID(string name, string release) {
            this.Name = name;
            this.Release = release;
        }
        public GameID(string name, XmlElement element): this(element) {
            this.Name = name;
        }
        public GameID(XmlElement element) {
            foreach (XmlAttribute attrib in element.Attributes) {
                if (MASGAU.Location.Holders.ALocationHolder.attributes.Contains(attrib.Name))
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

        public static int Compare(GameID a, GameID b) {
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

        public override int CompareTo(object comparable) {
            return Compare(this, comparable as GameID);
        }

        public static bool Equals(GameID a, GameID b) {
            return Compare(a, b) == 0;
        }

        public override Boolean Equals(AComparable to_me) {
            return Equals(this, to_me as GameID);
        }

        public static String ToString(GameID id) {
            StringBuilder return_me = new StringBuilder(id.Name);

            if (id.Release != null)
                return_me.Append(Core.seperator + id.Release);
            if (id.OS != null)
                return_me.Append(Core.seperator + id.OS);
            if (id.Platform != null)
                return_me.Append(Core.seperator + id.Platform);
            if (id.Region != null)
                return_me.Append(Core.seperator + id.Region);
            if (id.Media != null)
                return_me.Append(Core.seperator + id.Media);

            return return_me.ToString();
        }

        public System.Drawing.Color BackgroundColor {
            get {
                string hex = "11" + this.GetHashCode().ToString("X").Substring(0, 6);
                int value = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                return System.Drawing.Color.FromArgb(value);
            }
        }
        public System.Drawing.Color SelectedColor {
            get {
                string hex = "FF" + this.GetHashCode().ToString("X").Substring(0, 6);
                int value = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                return System.Drawing.Color.FromArgb(value);
            }
        }

        public String Formatted {
            get {
                StringBuilder return_me = new StringBuilder();

                if (OS != null)
                    return_me.Append(" " + OS);
                if (Platform != null)
                    return_me.Append(" " + Platform);
                if (Region != null)
                    return_me.Append(" " + Region);
                if (Media != null)
                    return_me.Append(" " + Media);

                return return_me.ToString();
            }
        }


        public override String ToString() {
            return ToString(this);
        }


        //public String[] string_array {
        //    get {
        //        if (platform != "Multiple") {
        //            if (region != null) {
        //                return new string[] { "Name", Name, "Platform", Platform.ToString(), "Region", Region };
        //            } else {
        //                return new string[] { "Name", Name, "Platform", Platform.ToString() };
        //            }
        //        } else {
        //            if (region != null) {
        //                return new string[] { "Name", Name, "Region", Region };
        //            } else {
        //                return new string[] { "Name", Name };
        //            }
        //        }
        //    }
        //}
    }
}
