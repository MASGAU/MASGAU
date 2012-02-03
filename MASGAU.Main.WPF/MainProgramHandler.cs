using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MASGAU.Location;

namespace MASGAU.Main {
    class MainProgramHandler: AMainProgramHandler<LocationsHandler>  {
        public MainProgramHandler(RunWorkerCompletedEventHandler when_done)
            : base(when_done, Interface.WPF) {

        }
    }
}
