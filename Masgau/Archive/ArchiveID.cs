using System;
using System.Text;
using System.Xml;
using MVC;
namespace MASGAU {
    public class ArchiveID : AIdentifier {
        public readonly String Owner;
        public readonly GameID Game;
        public readonly String Type;
        public readonly QuickHash OriginalPathHash = null;
        // Pre-0.10 MASGAU didn't embed a path hash


        public ArchiveID(XmlElement root) {
            foreach (XmlElement element in root.ChildNodes) {
                switch (element.Name) {
                    case "game":
                        Game = new GameID(element);
                        foreach (XmlAttribute attr in element.Attributes) {
                            if (GameID.attributes.Contains(attr.Name) || attr.Name == "name")
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
                    case "original_path_hash":
                        OriginalPathHash = new QuickHash(element.InnerText);
                        break;
                    case "owner":
                        if (!element.HasAttribute("name"))
                            throw new Exception("NAME MISSING FROM ARCHIVES");

                        Owner = element.GetAttribute("name");
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

            if (OriginalPathHash != null) {
                node = doc.CreateElement("original_path_hash");
                node.InnerText = OriginalPathHash.ToString();
                doc.DocumentElement.InsertAfter(node, doc.DocumentElement.LastChild);
            }

            return doc;
        }

        public ArchiveID(GameID game, String owner, String type, QuickHash original_path_hash) {
            this.Game = game;
            this.Owner = owner;
            this.Type = type;
            this.OriginalPathHash = original_path_hash;
        }

        public override int GetHashCode() {
            int value = Owner.GetHashCode() + Game.GetHashCode() + Type.GetHashCode();

            if (OriginalPathHash != null)
                value += OriginalPathHash.GetHashCode();
            return value;
        }

        public override Boolean Equals(AComparable obj) {
            ArchiveID id = obj as ArchiveID;

            // Basically we're going to ignore old archives for backup purposes
            if (this.OriginalPathHash == null) {
                return false;
            }

            return this.Owner == id.Owner &&
                this.Game == id.Game &&
                this.Type == id.Type &&
                this.OriginalPathHash == id.OriginalPathHash;
        }

        public override int CompareTo(object obj) {
            ArchiveID id = obj as ArchiveID;
            int result = compare(Game, id.Game);
            if (result == 0)
                result = compare(Owner, id.Owner);
            if (result == 0)
                result = compare(Type, id.Type);
            if (result == 0)
                result = compare(OriginalPathHash, id.OriginalPathHash);

            return result;
        }

        public override String ToString() {
            StringBuilder return_me = new StringBuilder(Game.ToString());
            if (Owner != null)
                return_me.Append(Core.seperator + Owner);

            if (Type != null)
                return_me.Append(Core.seperator + Type);

            if (OriginalPathHash != null)
                return_me.Append(Core.seperator + OriginalPathHash);

            return return_me.ToString();
        }

    }
}
