using System.Collections.Generic;
using MASGAU.Location.Holders;
using MASGAU.Location;
using GameSaveInfo;
namespace MASGAU.Location {
    public interface ILocationsHandler {
        void resetSteam();

        DetectedLocations getPaths(ALocation get_me);

        List<string> getUsers(EnvironmentVariable for_me);

        string getAbsoluteRoot(LocationPath parse_me, string user);
        string getAbsolutePath(LocationPath parse_me, string user);

        DetectedLocations interpretPath(string interpret_me);

    }
}
