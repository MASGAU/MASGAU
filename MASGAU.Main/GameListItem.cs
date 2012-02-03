using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASGAU.Game;

namespace MASGAU.Main
{
    class GameListItem {
        public string name { get; set; }
        public string title { get; set; }
        public string platform { get; set; }
        public string tooltip;
        public bool disabled;
        public GameID id;
    }
}
