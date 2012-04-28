using System;

namespace MASGAU.Console
{
	class MainClass: AConsole<Location.LocationsHandler>
	{
		public static void Main (string[] args)
		{
			new MainClass(args);
		}
		
		public MainClass(string[] args): base(args) {}
	}
}
