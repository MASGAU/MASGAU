using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Communication {
    public class RespondableEventArg : EventArgs {
        public string title, message;
        public ResponseType response = ResponseType.None;
    }
}
