using System;
using System.IO;
using System.Text;
using System.Xml;
using GameSaveInfo;
using MASGAU.Location.Holders;
using MVC;
namespace MASGAU {
    public class ArchiveID : AIdentifier {
        public readonly String Owner;
        public readonly GameID Game;
        public readonly String Type;
        public QuickHash OriginalLocationhHash {
            get {
                if (OriginalLocation == null)
                    return null;
                return new QuickHash(OriginalLocation);
            }
        }
        public EnvironmentVariable OriginalEV { get; protected set; }
        public readonly String OriginalLocation;
        public readonly String OriginalRelativePath;
        public String OriginalDrive {
            get {
                if (OriginalLocation != null)
                    return Path.GetPathRoot(OriginalLocation);
                return null;
            }
        }
        // Pre-0.10 MASGAU didn't embed a path hash


        public ArchiveID(XmlElement root) {
            foreach (XmlElement element in root.ChildNodes) {
                switch (element.Name) {
                    case "game":
                        Game = new GameID(element);
                        foreach (XmlAttribute attr in element.Attributes) {
                            if (GameIdentifier.attributes.Contains(attr.Name) || attr.Name == "name")
                                continue;

                            throw new NotSupportedException(attr.Name);
                        }
                        break;
                    case "archive":
                        foreach (XmlAttribute attr in element.Attributes) {
                            switch (attr.Name) {
                                case "type":
                                    Type = attr.Value;
                                    break;
                                default:
                                    throw new NotSupportedException(attr.Name);
                            }
                        }
                        break;
                    case "original_location":
                        OriginalLocation = element.InnerText;
                        break;
                    case "original_relative_path":
                        OriginalRelativePath = element.InnerText;
                        break;
                    case "original_ev":
                        OriginalEV = ALocation.parseEnvironmentVariable(element.InnerText);
                        break;
                    case "owner":
                        if (!element.HasAttribute("name"))
                            throw new Exception("NAME MISSING FROM ARCHIVES");

                        Owner = element.GetAttribute("name");
                        break;
                    case "original_location_hash":
                    case "original_drive":
                        break;
                    default:
                        throw new NotSupportedException(element.Name);
                }
            }

        }

        public XmlDocument AddElements(XmlDocument doc) {
            XmlElement node;
            XmlAttribute attribute;

            node = doc.CreateElement("game");
            node = Game.AddAttributes(node);

            doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);

            if (Owner != null) {
                node = doc.CreateElement("owner");
                attribute = doc.CreateAttribute("name");
                attribute.Value = Owner;
                node.SetAttributeNode(attribute);
                doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);
            }

            if (Type != null) {
                node = doc.CreateElement("archive");
                attribute = doc.CreateAttribute("type");
                attribute.Value = Type.ToString();
                node.SetAttributeNode(attribute);
                doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);
            }

            if (OriginalLocationhHash != null) {
                node = doc.CreateElement("original_location_hash");
                node.InnerText = OriginalLocationhHash.ToString();
                doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);
            }
            if (OriginalLocation != null) {
                node = doc.CreateElement("original_location");
                node.InnerText = OriginalLocation.ToString();
                doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);
            }
            if (OriginalDrive != null) {
                node = doc.CreateElement("original_drive");
                node.InnerText = OriginalDrive.ToString();
                doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);
            }
            if (OriginalRelativePath != null) {
                node = doc.CreateElement("original_relative_path");
                node.InnerText = OriginalRelativePath.ToString();
                doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);
            }

            node = doc.CreateElement("original_ev");
            node.InnerText = OriginalEV.ToString();
            doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);

            return doc;
        }

        public ArchiveID(GameID game, DetectedFile first_file) {
            this.Game = game;
            this.Owner = first_file.owner;
            this.Type = first_file.Type;
            DetectedLocationPathHolder loc = first_file.OriginalLocation;


            OriginalLocation = loc.full_dir_path;
            OriginalEV = loc.EV;
            OriginalRelativePath = loc.Path;
        }

        public override int GetHashCode() {
            int value = Owner.GetHashCode() + Game.GetHashCode() + Type.GetHashCode();

            if (OriginalLocationhHash != null)
                value += OriginalLocationhHash.GetHashCode();
            return value;
        }

        public override Boolean Equals(AComparable obj) {
            ArchiveID id = obj as ArchiveID;

            // Basically we're going to ignore old archives for backup purposes
            if (this.OriginalLocationhHash == null) {
                return false;
            }

            return this.Owner == id.Owner &&
                this.Game == id.Game &&
                this.Type == id.Type &&
                this.OriginalLocationhHash == id.OriginalLocationhHash;
        }

        public override int CompareTo(object obj) {
            ArchiveID id = obj as ArchiveID;
            int result = compare(Game, id.Game);
            if (result == 0)
                result = compare(Owner, id.Owner);
            if (result == 0)
                result = compare(Type, id.Type);
            if (result == 0)
                result = compare(OriginalLocationhHash, id.OriginalLocationhHash);

            return result;
        }

        public override String ToString() {
            StringBuilder return_me = new StringBuilder(Game.ToString());
            if (Owner != null)
                return_me.Append(Common.Seperator + Owner);

            if (Type != null)
                return_me.Append(Common.Seperator + Type);

            if (OriginalLocationhHash != null)
                return_me.Append(Common.Seperator + OriginalLocationhHash);

            return return_me.ToString();
        }

    }
}
