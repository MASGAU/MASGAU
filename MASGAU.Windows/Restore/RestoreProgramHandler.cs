using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MASGAU.Location;
using MASGAU.Archive;

namespace MASGAU.Restore {
    public class RestoreProgramHandler: ARestoreProgramHandler<LocationsHandler> {
        public RestoreProgramHandler(RunWorkerCompletedEventHandler when_done, ArchiveHandler archive): base(when_done, Interface.WPF, archive) {}
    }
}
