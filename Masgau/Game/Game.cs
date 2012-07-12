using System;
using System.Xml;
using System.Collections.Generic;
using XmlData;
using MVC;
using Translator;
namespace MASGAU {
    public class Game : IXmlDataEntry {
        public string Name { get; protected set; }
        public string Title { get; protected set; }
        public string Comment { get; protected set; }
        public bool IsDeprecated { get; protected set; }
        public GameType Type { get; protected set; }

        public List<GameVersion> Versions = new List<GameVersion>();

        public void LoadData(XmlElement element) {
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
                        GameVersion version = new GameVersion(this, sub);
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
