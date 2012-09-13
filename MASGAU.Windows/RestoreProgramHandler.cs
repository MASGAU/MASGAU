using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace MASGAU.Restore {
    public class RestoreProgramHandler: ARestoreProgramHandler  {

        public RestoreProgramHandler(Archive archive): base(archive) {}

        protected override Location.ALocationsHandler CreateLocationsHandler() {
            return new Location.LocationsHandler();
        }

        protected override AStartupHelper CreateStartupHelper(string program_name, string program_path) {
            return new StartupHelper(program_name, program_path);
        }

        protected override ASymLinker CreateSymLinker() {
            return new SymLinker();
        }
    }
}
