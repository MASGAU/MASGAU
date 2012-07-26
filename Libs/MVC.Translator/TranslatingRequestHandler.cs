using System.Collections.Generic;
using Translator;
using MVC.Communication;

namespace MVC.Translator {
    public class TranslatingRequestHandler : RequestHandler {

        public static RequestReply Request(RequestType type, string name, params string[] variables) {
            return TranslatingRequestHandler.Request(type, name,null, variables);
        }

        public static RequestReply Request(RequestType type, string name, bool suppressable, params string[] variables) {
            return TranslatingRequestHandler.Request(type, name, null, suppressable, variables);
        }



        public static RequestReply Request(RequestType type, string name, List<string> choices, params string[] variables) {
            return TranslatingRequestHandler.Request(type, name, null, choices, variables);
        }
        public static RequestReply Request(RequestType type, string name, List<string> choices, bool suppressable, params string[] variables) {
            return TranslatingRequestHandler.Request(type, name, null, choices, suppressable, variables);
        }

        public static RequestReply Request(RequestType type, string name, string default_choice, List<string> choices, params string[] variables) {
            return Request(type, name, default_choice, choices, false, variables);
        }

        public static RequestReply Request(RequestType type, string name, string default_choice, List<string> choices, bool suppressable, params string[] variables) {
            StringCollection col = Strings.getStrings(name);
            string title, message;
            if(col.ContainsKey(StringType.Title))
                title = col[StringType.Title].interpret(variables);
            else 
                title = name;

            if(col.ContainsKey(StringType.Message))
                message = col[StringType.Message].interpret(variables);
            else 
                message = name;


            return RequestHandler.Request(type, title, message, choices, default_choice, suppressable);

        }
    }
}
