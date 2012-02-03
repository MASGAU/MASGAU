using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASGAU.Game;

namespace MASGAU.Location.Holders {
    public class LocationGameHolder : ALocationHolder {
        // Used when dealing with a game root

        public GameID game;

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationGameHolder location = (LocationGameHolder)comparable;
            return game.CompareTo(location.game);
        }

    }
}
