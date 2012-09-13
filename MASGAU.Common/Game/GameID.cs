using System;
using System.Text;
using System.Xml;
using GameSaveInfo;
using MVC;
namespace MASGAU {
    public class GameID : AIdentifier {
        public GameIdentifier game { get; protected set; }
        public string Name { get { return game.Name; } }
        public string OS {
            get {
                return game.OS;
            }
        }

        public GameID(XmlElement element) {
            game = new GameIdentifier(element);
        }

        public GameID(GameIdentifier id) {
            game = id;
        }

        public GameID(string name, string release) {
            game = new GameIdentifier(name, release);
        }

        public XmlElement AddAttributes(XmlElement element) {
            return game.AddAttributes(element);
        }

        private static String ToString(GameIdentifier id) {
            StringBuilder return_me = new StringBuilder(id.Name);

            if (id.Release != null)
                return_me.Append(Common.Seperator + id.Release);
            if (id.OS != null)
                return_me.Append(Common.Seperator + id.OS);
            if (id.Platform != null)
                return_me.Append(Common.Seperator + id.Platform);
            if (id.Region != null)
                return_me.Append(Common.Seperator + id.Region);
            if (id.Media != null)
                return_me.Append(Common.Seperator + id.Media);

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
                string hex = "55" + this.GetHashCode().ToString("X").Substring(0, 6);
                int value = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                return System.Drawing.Color.FromArgb(value);
            }
        }

        public override int GetHashCode() {
            return game.GetHashCode();
        }

        public String Formatted {
            get {
                StringBuilder return_me = new StringBuilder();

                if (game.OS != null)
                    return_me.Append(" " + game.OS);
                if (game.Platform != null)
                    return_me.Append(" " + game.Platform);
                if (game.Region != null)
                    return_me.Append(" " + game.Region);
                if (game.Media != null)
                    return_me.Append(" " + game.Media);

                return return_me.ToString();
            }
        }
        public override int CompareTo(object comparable) {
            GameID id = comparable as GameID;
            return game.CompareTo(id.game);
        }

        public static bool Equals(AComparable a, AComparable b) {
            return Compare(a as GameID, b as GameID) == 0;
        }

        public override Boolean Equals(AComparable to_me) {
            return Equals(this, to_me as GameID);
        }

        public static int Compare(GameID a, GameID b) {
            return a.game.CompareTo(b.game);

        }

        public override String ToString() {
            return ToString(game);
        }

    }
}
