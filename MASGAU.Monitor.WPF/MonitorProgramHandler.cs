using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;

namespace MASGAU.Monitor {
    class MonitorProgramHandler: AMonitorProgramHandler<Location.LocationsHandler> {
        public MonitorProgramHandler()
            : base(Interface.WPF) {
        }
    }
}
