using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace MASGAU.Console
{
    class Console: AConsole<Location.LocationsHandler>
    {

        static void Main(string[] args)
        {
            new Console(args);
        }

        private Console(string[] args): base(args) {

        }

    }
}
