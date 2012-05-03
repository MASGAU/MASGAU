using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication.Message {
    public class MessageEventArgs : RespondableEventArg {
        public MessageTypes type;
        public bool acknowledged = false;
        public Exception exception = null;
    }
}
