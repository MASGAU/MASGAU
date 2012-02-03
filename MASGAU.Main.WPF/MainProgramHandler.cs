using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MASGAU.Location;

namespace MASGAU.Main {
    class MainProgramHandler: AMainProgramHandler<LocationsHandler>  {
        public MainProgramHandler()
            : base(Interface.WPF) {

        }
    }
}
