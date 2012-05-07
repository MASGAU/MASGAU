using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location {
    public class LocationsHandler: ALocationsHandler   {

        protected override APlaystationLocationHandler setupPlaystationHandler() {
            return new PlaystationLocationHandler();
        }

        protected override ASteamLocationHandler setupSteamHandler() {
            return new SteamLocationHandler();
        }

        protected override ASystemLocationHandler setupSystemHandler() {
            return new SystemLocationHandler();
        }

        protected override AScummVMLocationHandler setupScummVMHandler()
        {
            return new ScummVMLocationHandler();
        }
    }
}
