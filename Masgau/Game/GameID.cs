using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASGAU;

namespace MASGAU.Game {
    public class GameID : AIdentifier {
        public readonly String name;
        public readonly GamePlatform platform;
        public readonly String country;
        public readonly bool deprecated;

        public GameID(String name, GamePlatform platform, String country, bool deprecated): this(name,platform,country) {
            this.deprecated = deprecated;
        }
        public GameID(String name, GamePlatform platform, String country) {
            this.name = name;
            this.platform = platform;
            this.country = country;
        }

        public static int Compare(GameID a, GameID b) {
            int result = compare(a.name,b.name);
            if (result == 0)
                result = compare(a.platform,b.platform);
            if (result == 0)
                result = compare(a.country,b.country);

            return result;
        }

        public override int CompareTo(object comparable) {
            return Compare(this,comparable as GameID);
        }

        public static bool Equals(GameID a, GameID b) {
            if (a.name == b.name &&
                a.platform == b.platform &&
                a.country == b.country)
                return true;
            else
                return false;
        }

        public override Boolean Equals(AIdentifier to_me) {
            return Equals(this,to_me as GameID);
        }

        public static String ToString(GameID id) {
            StringBuilder return_me = new StringBuilder(id.name);
            if (id.platform != GamePlatform.Multiple) {

                return_me.Append("«" + id.platform.ToString());
            }
            if (id.country != null) {
                return_me.Append("«" + id.country);
            }
            return return_me.ToString();
        }

        public override String ToString() {
            return ToString(this);
        }


        public String[] string_array {
            get {
                if (platform != GamePlatform.Multiple) {
                    if (country != null) {
                        return new string[] { "name", name, "platform", platform.ToString(), "country", country };
                    }
                    else {
                        return new string[] { "name", name, "platform", platform.ToString() };
                    }
                }
                else {
                    if (country != null) {
                        return new string[] { "name", name, "country", country };
                    }
                    else {
                        return new string[] { "name", name };
                    }
                }
            }
        }
    }
}
