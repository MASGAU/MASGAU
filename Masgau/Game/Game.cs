using System;
using System.Xml;
using System.Collections.Generic;
using XmlData;
using MVC;
using Translator;
namespace MASGAU {
    public class Game : AXmlDataEntry {
        public string Name { get; protected set; }
        public string Title { get; protected set; }
        public string Comment { get; protected set; }
        public bool IsDeprecated { get; protected set; }
        public GameType Type { get; protected set; }

        public List<GameVersion> Versions = new List<GameVersion>();

        protected Game(XmlDocument doc): base(doc) { }

        public Game(XmlElement element)
            : base(element) {
        }

        protected virtual GameVersion createVersion(Game parent, XmlElement element) {
            return new GameVersion(parent, element);
        }

        public override XmlElement exportXml() {
            // This outputs little more than what's necessary to create a custom game entry
            // Once in the file, the xml wil not need to be re-generated, so it won't need to be outputted again
            // This way manual updates to the xml file won't be lost ;)
            if (this.xml != null)
                return this.xml;

            this.xml = createElement(Type.ToString());
            addAtribute(this.xml,"name",Name);
            if(IsDeprecated)
                addAtribute(this.xml,"deprecated","true");

            this.xml.AppendChild(createElement("title",Title));




            if(Comment!=null)
                this.xml.AppendChild(createElement("comment", Comment));

            foreach (GameVersion ver in Versions) {
                XmlElement ele = ver.createXml();
                if (ele != null)
                    this.xml.AppendChild(ele);
            }

            string output = this.xml.OuterXml;
            return this.xml;
        }

        protected override void LoadData(XmlElement element) {
            switch (element.Name) {
                case "system":
                    Type = GameType.system;
                    break;
                case "game":
                    Type = GameType.game;
                    break;
                case "mod":
                    Type = GameType.mod;
                    break;
                case "expansion":
                    Type = GameType.expansion;
                    break;
                default:
                    throw new NotSupportedException(element.Name);
            }

            foreach (XmlAttribute attrib in element.Attributes) {
                switch (attrib.Name) {
                    case "name":
                        Name = attrib.Value;
                        break;
                    case "deprecated":
                        IsDeprecated = Boolean.Parse(attrib.Value);
                        break;
                    case "follows":
                    case "for":
                    case "submitted":
                        break;
                    default:
                        throw new NotSupportedException(attrib.Name);
                }
            }

            foreach (XmlElement sub in element.ChildNodes) {
                switch (sub.Name) {
                    case "title":
                        Title = sub.InnerText;
                        break;
                    case "version":
                        GameVersion version = createVersion(this, sub);
                        Versions.Add(version);
                        break;
                    case "comment":
                        Comment = sub.InnerText;
                        break;
                    default:
                        throw new NotSupportedException(sub.Name);
                }
            }

        }
    }
}
