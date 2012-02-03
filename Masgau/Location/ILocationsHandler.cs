using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using MASGAU.Location.Holders;

namespace MASGAU.Location
{
    public interface ILocationsHandler
    {
        void resetSteam();

        List<DetectedLocationPathHolder> getPaths(ALocationHolder get_me);

        List<string> getUsers(EnvironmentVariable for_me);

        string getAbsoluteRoot(LocationPathHolder parse_me, string user);
        string getAbsolutePath(LocationPathHolder parse_me, string user);

        List<DetectedLocationPathHolder> interpretPath(string interpret_me);

   }
}
