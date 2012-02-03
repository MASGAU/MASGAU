using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MASGAU.Location;

namespace MASGAU.Analyzer
{
    public abstract class AAnalyzerProgramHandler<L> : AProgramHandler<L> where L : ALocationsHandler
    {
        public AAnalyzerProgramHandler(RunWorkerCompletedEventHandler when_done, Interface iface):base(when_done, iface)  {
        }

        public static string last_save_path = null;


    }
}
