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
        public static string disclaimer = "Submitted reports are published to a public server. "
                                    + "No one involved with MASGAU can be held liable for any repurcusions from the publishing of "
                                    + "sensitive information through these reports, so please carefully "
                                    + "look over and edit the report for any information that you do not want to be made public. "
                                    + "By submitting a report you are agreeing that it is free for public dispersal.";

        public AAnalyzerProgramHandler(RunWorkerCompletedEventHandler when_done, Interface iface):base(when_done, iface)  {
        }

        public static string last_save_path = null;


    }
}
