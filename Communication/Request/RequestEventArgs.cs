using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication.Request {
    public class RequestEventArgs : RespondableEventArg {
        public List<string> options = null;
        public string default_option = null;
        public RequestType info_type;
        public RequestReply result;
        public RequestEventArgs(RequestType new_info_type, string new_title, string new_message, List<string> new_options, string new_default_option, RequestReply new_result)
            : base() {
            title = new_title;
            message = new_message;
            info_type = new_info_type;
            options = new_options;
            default_option = new_default_option;
            result = new_result;
        }
    }
}
