using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Communication
{
    public class CommunicatableException: System.Exception
    {
        public string title;

        public CommunicatableException(string title, string message, System.Exception inner_exception): 
            base(message,inner_exception) {
                this.title = title;
        }
 //       public CommunicatableException(string name, string extended_message, bool submittable, params string[] variables): 
   //         base(title, message + Environment.NewLine + Environment.NewLine + extended_message, submittable) {
     //           this.title = Strings.get(name + "Title", variables);
       //         this.submittable = submittable;
         //   }
        public CommunicatableException(string title, string message):
            base(message)
        {
            this.title = title;
        }
    }
}
