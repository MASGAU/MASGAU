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
			
			initialized = true;
		}
	}
}

