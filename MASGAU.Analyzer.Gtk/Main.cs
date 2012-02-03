using System;
using Gtk;

namespace MASGAU.Analyzer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			AnalyzerWindow win = new AnalyzerWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}

