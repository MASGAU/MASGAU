using System;

namespace MASGAU.Location {
	public class LocationsHandler: ALocationsHandler {
		public LocationsHandler () {
		}

		protected override APlaystationLocationHandler setupPlaystationHandler() {
			//return new PlaystationLocationHandler();
			return null;
		}
		
		protected override ASteamLocationHandler setupSteamHandler() {
			//return new SteamLocationHandler();
			return null;
		}
		
		protected override ASystemLocationHandler setupSystemHandler() {
			return new SystemLocationHandler();
		}
		
		protected override AScummVMLocationHandler setupScummVMHandler() {
			//return new ScummVMLocationHandler();
			return null;
		}
	}
}

