using System;
using System.ComponentModel;

namespace MASGAU.Analyzer
{
	public class AnalyzerProgramHandler: AAnalyzerProgramHandler<Location.LocationsHandler>
	{
        public AnalyzerProgramHandler(RunWorkerCompletedEventHandler when_done):base(when_done, MASGAU.Interface.WPF)  {
        }
		

	}
}

