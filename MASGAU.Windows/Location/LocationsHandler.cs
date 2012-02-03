using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASGAU.Communication.Progress;

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
    }
}
