using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using XmlData;
namespace GameSaveInfo {
    public class GameVersion : AXmlDataSubEntry {
        public GameIdentifier ID { get; protected set; }

        public Game Game {
            get {
                return this.Parent as Game;
            }
        }
        public List<string> Contributors = new List<string>();

        public string Comment { get; protected set; }
        public string RestoreComment { get; protected set; }

        public bool IgnoreVirtualStore { get; protected set; }
        public bool IsDeprecated { get; protected set; }
        public bool DetectionRequired { get; protected set; }

        public List<Link> Links = new List<Link>();
        public List<Identifier> Identifiers = new List<Identifier>();
        public Dictionary<string, FileType> FileTypes = new Dictionary<string, FileType>();

        public override string ElementName {
            get { return "version"; }
        }

        protected string _title = null;
        public string Title {
            get {
                if (_title == null)
                    return Game.Title;
                return _title;
            }
        }

        public List<ALocation> AllLocations {
            get {
                if (Locations != null)
                    return Locations.AllLocations;
                else
                    return new List<ALocation>();
            }
        }
        public Locations Locations;
        public List<APlayStationID> PlayStationIDs = new List<APlayStationID>();
        public List<ScummVM> ScummVMs = new List<ScummVM>();

        public GameVersion(Game parent)
            : base(parent) {
                DetectionRequired = false;
        }

        public GameVersion(Game parent, XmlElement ele)
            : base(parent, ele) {
                DetectionRequired = false;
        }

        protected override void LoadData(XmlElement element) {
            this.ID = new GameIdentifier(Game.Name, element);

            foreach (XmlAttribute attrib in element.Attributes) {
                if (GameIdentifier.attributes.Contains(attrib.Name))
                    continue;

                switch (attrib.Name) {
                    case "virtualstore":
                        IgnoreVirtualStore = attrib.Value == "ignore";
                        break;
                    case "detect":
                        DetectionRequired = attrib.Value == "required";
                        break;
                    case "deprecated":
                        IsDeprecated = Boolean.Parse(attrib.Value);
                        break;
                    default:
                        throw new NotSupportedException(attrib.Name);
                }
            }
            Links.Clear();
            foreach (XmlElement sub in element.ChildNodes) {
                switch (sub.Name) {
                    case "title":
                        _title = sub.InnerText;
                        break;
                    case "locations":
                        this.Locations = new GameSaveInfo.Locations(sub);
                        break;
                    case "files":
                        FileType type = new FileType(sub);
                        FileTypes.Add(type.Type, type);
                        break;
                    case "ps_code":
                        PlayStationIDs.Add(APlayStationID.Create(this, sub));
                        break;
                    case "contributor":
                        Contributors.Add(sub.InnerText);
                        break;
                    case "comment":
                        Comment = sub.InnerText;
                        break;
                    case "restore_comment":
                        RestoreComment = sub.InnerText;
                        break;
                    case "identifier":
                        Identifiers.Add(new Identifier(sub));
                        break;
                    case "scummvm":
                        ScummVMs.Add(new ScummVM(sub));
                        break;
                    case "linkable":
                        Links.Add(new Link(this, sub));
                        break;
                    default:
                        throw new NotSupportedException(sub.Name);
                }
            }
        }

        protected override XmlElement WriteData(XmlElement element) {
            // This outputs little more than what's necessary to create a custom game entry
            // Once in the file, the xml wil not need to be re-generated, so it won't need to be outputted again
            // This way manual updates to the xml file won't be lost ;)

            ID.AddAttributes(element);

            if (element.HasAttribute("name"))
                element.RemoveAttribute("name");

            element.AppendChild(Locations.XML);

            foreach (FileType type in FileTypes.Values) {
                element.AppendChild(type.XML);
            }

            foreach (string con in Contributors) {
                element.AppendChild(Game.createElement("contributor", con));
            }

            if (Comment != null)
                element.AppendChild(Game.createElement("comment", Comment));
            if (RestoreComment != null)
                element.AppendChild(Game.createElement("restore_comment", RestoreComment));


            return element;
        }
    }
}
