using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU
{
    public class MException: System.Exception
    {
        public string title;
        public bool submittable = false;
        public MException(string title, string message, System.Exception inner_exception, bool submittable): base(message,inner_exception) {
            this.title = title;
            this.submittable = submittable;
        }
        public MException(string title, string message, string extended_message, bool submittable): this(title, message + Environment.NewLine + Environment.NewLine + extended_message, submittable) {

        }
        public MException(string title, string message, bool submittable): base(message) {
            this.title = title;
            this.submittable = submittable;
        }
    }
}
