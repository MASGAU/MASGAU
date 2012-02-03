using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace MASGAU.Analyzer {
    class AnalyzerProgramHandler: AAnalyzerProgramHandler<Location.LocationsHandler> {
        public AnalyzerProgramHandler(RunWorkerCompletedEventHandler when_done):base(when_done, MASGAU.Interface.WPF)  {
        }

    }
}
