using System.Collections.Generic;
using MASGAU.Location.Holders;
using MASGAU.Location;
namespace MASGAU.Location {
    public interface ILocationsHandler {
        void resetSteam();

        DetectedLocations getPaths(ALocationHolder get_me);

        List<string> getUsers(EnvironmentVariable for_me);

        string getAbsoluteRoot(LocationPathHolder parse_me, string user);
        string getAbsolutePath(LocationPathHolder parse_me, string user);

        DetectedLocations interpretPath(string interpret_me);

    }
}
