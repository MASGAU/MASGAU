using System;

namespace MASGAU.Location
{
	public class LocationsHandler: ALocationsHandler
	{
		public LocationsHandler ()
		{
		}
		
		protected override ASystemLocationHandler setupSystemHandler ()
		{
			return new SystemLocationHandler();
		}
		protected override APlaystationLocationHandler setupPlaystationHandler ()
		{
			return new PlaystationLocationHandler();
		}
		protected override ASteamLocationHandler setupSteamHandler ()
		{
			return new SteamLocationHandler();
		}
	}
}

