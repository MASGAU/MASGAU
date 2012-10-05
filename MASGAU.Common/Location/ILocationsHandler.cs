using System.Collections.Generic;
using GameSaveInfo;
namespace MASGAU.Location {
    public interface ILocationsHandler {
        void resetSteam();

        DetectedLocations getPaths(ALocation get_me);

        List<string> getUsers(EnvironmentVariable for_me);

        string getAbsoluteRoot(LocationPath parse_me, string user);
        string getAbsolutePath(LocationPath parse_me, string user);

        DetectedLocations interpretPath(params string[] interpret_me);

    }
}
