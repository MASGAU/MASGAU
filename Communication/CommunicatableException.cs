using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Translations;
namespace Communication
{
    public class CommunicatableException: System.Exception
    {
        public string title;
        public bool submittable = false;
        public CommunicatableException(string name, System.Exception inner_exception, bool submittable, params string[] variables): 
            base(Strings.getTitleMessagePair(name,variables)[StringType.Message],inner_exception) {
                this.title = Strings.getTitleMessagePair(name,variables)[StringType.Title];
            this.submittable = submittable;
        }
 //       public CommunicatableException(string name, string extended_message, bool submittable, params string[] variables): 
   //         base(title, message + Environment.NewLine + Environment.NewLine + extended_message, submittable) {
     //           this.title = Strings.get(name + "Title", variables);
       //         this.submittable = submittable;
         //   }
        public CommunicatableException(string name, bool submittable, params string[] variables):
            base(Strings.getTitleMessagePair(name,variables)[StringType.Message])
        {
            this.title = Strings.getTitleMessagePair(name,variables)[StringType.Title];
            this.submittable = submittable;
        }
    }
}
