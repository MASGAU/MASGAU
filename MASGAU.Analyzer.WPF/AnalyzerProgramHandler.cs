using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace MASGAU.Analyzer {
    class AnalyzerProgramHandler: AAnalyzerProgramHandler<Location.LocationsHandler> {
        public AnalyzerProgramHandler():base(MASGAU.Interface.WPF)  {
        }

    }
}
