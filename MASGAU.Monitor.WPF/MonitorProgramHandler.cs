using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;

namespace MASGAU.Monitor {
    class MonitorProgramHandler: AMonitorProgramHandler<Location.LocationsHandler> {
        public MonitorProgramHandler(RunWorkerCompletedEventHandler when_done)
            : base(when_done, Interface.WPF) {
        }
    }
}
