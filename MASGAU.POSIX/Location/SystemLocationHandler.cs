using System;

namespace MASGAU.Location {
	public class SystemLocationHandler: ASystemLocationHandler {
		private bool initialized = false;
		public override bool ready {
			get {
				return initialized;
			}
		}
		public SystemLocationHandler () {
			initialized = true;
		}
	}
}

