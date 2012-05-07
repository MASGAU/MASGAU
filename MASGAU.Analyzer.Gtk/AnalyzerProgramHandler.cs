using System;
using System.ComponentModel;

namespace MASGAU.Analyzer
{
	public class AnalyzerProgramHandler: AAnalyzerProgramHandler<Location.LocationsHandler>
	{
        public AnalyzerProgramHandler():base(MASGAU.Interface.Gtk)  {
        }
		

	}
}
