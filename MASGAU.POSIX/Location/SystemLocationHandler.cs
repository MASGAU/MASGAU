using System;
using GameSaveInfo;
namespace MASGAU.Location {
	public class SystemLocationHandler: ASystemLocationHandler {

		private bool initialized = false;
		public override bool ready {
			get {
				return initialized;
			}
		}
		public SystemLocationHandler () {
			global.addEvFolder(EnvironmentVariable.Root,"root","/");
			if(Core.StaticAllUsersMode) {
				throw new NotImplementedException();
			} else {
				this.addUserEv (System.Environment.UserName, EnvironmentVariable.Home,
				                "home",System.Environment.GetEnvironmentVariable("HOME"));
			}

			if (MASGAU.Core.OS== OperatingSystem.Windows) {
				// This means we're running the Mono code on Windows for some reason, most likely for testing purposes.
				// This block will set up a few environment variables to make Mono development on Windows possible.
				// This does NOT take the place of the Windows DLLs, as they make use of several deep Windows functions
				// that will never be available on Linux.

				this.addUserEv (System.Environment.UserName, EnvironmentVariable.AppData, "app_data", System.Environment.GetEnvironmentVariable ("AppData"));
								

			}

			initialized = true;
		}
	}
}

