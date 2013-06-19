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

            if (!String.IsNullOrEmpty(id.Release))
                return_me.Append(Core.seperator + id.Release);
			if (!String.IsNullOrEmpty(id.OS))
                return_me.Append(Core.seperator + id.OS);
            if (!String.IsNullOrEmpty(id.Platform))
                return_me.Append(Core.seperator + id.Platform);
            if (!String.IsNullOrEmpty(id.Region))
                return_me.Append(Core.seperator + id.Region);
            if (!String.IsNullOrEmpty(id.Media))
                return_me.Append(Core.seperator + id.Media);
			if(id.Revision != 0)
				return_me.Append(Core.seperator + "rev" + id.Revision);

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
		public static bool Equals(AComparable a, AComparable b, bool ignore_revision) {
			return Compare(a as GameID, b as GameID, ignore_revision) == 0;
		}

        public override Boolean Equals(AComparable to_me) {
            return Equals(this, to_me as GameID);
        }

		public Boolean Equals(AComparable to_me, bool ignore_revision) {
			return Equals(this, to_me as GameID,ignore_revision);
		}

        public static int Compare(GameID a, GameID b) {
            return a.game.CompareTo(b.game);
        }

		public static int Compare(GameID a, GameID b, bool ignore_revision) {
			return a.game.CompareTo(b.game, ignore_revision);
		}


        public override String ToString() {
            return ToString(game);
        }

    }
}
