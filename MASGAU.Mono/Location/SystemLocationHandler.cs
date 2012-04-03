using System;
namespace MASGAU.Location
{
	public class SystemLocationHandler: ASystemLocationHandler
	{
		private bool _ready = false;
		public override bool ready {
			get {
				return _ready;
			}
		}

		public SystemLocationHandler (): base()
		{
			
		}
		
	}
}

