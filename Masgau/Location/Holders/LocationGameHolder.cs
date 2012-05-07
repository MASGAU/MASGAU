using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASGAU.Game;
using System.Xml;
using MVC;

namespace MASGAU.Location.Holders {
    public class LocationGameHolder : ALocationHolder {
        // Used when dealing with a game root

        public GameID game;

        public LocationGameHolder(XmlElement element): base(element)
        {
            String new_game_name = element.GetAttribute("name");
            GamePlatform new_game_platform;
            String new_game_region;
            if (element.HasAttribute("platform"))
                new_game_platform = GameHandler.parseGamePlatform(element.GetAttribute("platform"));
            else
                new_game_platform = GamePlatform.Multiple;
            if (element.HasAttribute("region"))
                new_game_region = element.GetAttribute("region");
            else
                new_game_region = null;
            this.game = new GameID(new_game_name, new_game_platform, new_game_region);

        }

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationGameHolder location = (LocationGameHolder)comparable;
            return game.CompareTo(location.game);
        }

    }
}
