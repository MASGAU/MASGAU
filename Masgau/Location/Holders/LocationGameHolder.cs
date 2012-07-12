using System;
using System.Xml;
using MVC;

namespace MASGAU.Location.Holders {
    public class LocationGameHolder : ALocationHolder {
        // Used when dealing with a game root

        public GameID game;

        public LocationGameHolder(XmlElement element)
            : base(element) {
            this.game = new GameID(element.GetAttribute("name"), element);
            foreach (XmlAttribute attrib in element.Attributes) {
                if (attributes.Contains(attrib.Name)||GameID.attributes.Contains(attrib.Name))
                    continue;
                switch (attrib.Name) {
                    case "name":
                        break;
                    default:
                        throw new NotSupportedException(attrib.Name);
                }
            }

        }

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationGameHolder location = (LocationGameHolder)comparable;
            return game.CompareTo(location.game);
        }

    }
}
