using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication.Message;
using Translations;
namespace Communication.Translator
{
    public class TranslatingMessageHandler: MessageHandler
    {
        public static ResponseType SendWarning(string name, params string[] variables)
        {
            return SendWarning(name, null, variables);
        }
        public static ResponseType SendWarning(string name, Exception e, params string[] variables)
        {
            StringCollection strings = Strings.get(name, variables);

            return MessageHandler.SendWarning(strings[StringType.Title], strings[StringType.Message], e);
        }
        public static ResponseType SendError(string name, params string[] variables)
        {
            return SendError(name, null, variables);
        }
        public static ResponseType SendError(string name, Exception e, params string[] variables)
        {
            StringCollection strings = Strings.get(name, variables);

            return MessageHandler.SendError(strings[StringType.Title], strings[StringType.Message], e);
        }
    }
}
