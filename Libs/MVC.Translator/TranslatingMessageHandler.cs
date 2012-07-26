using System;
using System.Collections.Generic;
using Translator;
using MVC.Communication;
namespace MVC.Translator {
    public class TranslatingMessageHandler : MessageHandler {
        public new static ResponseType SendException(Exception e) {
            if (e.GetType() == typeof(TranslateableException)) {
                try {
                    TranslateableException ex = e as TranslateableException;
                    StringCollection strings = Strings.getStrings(e.Message);
                    return SendMessage(strings[StringType.Title].interpret(ex.variables),
                        strings[StringType.Message].interpret(ex.variables), MessageTypes.Error, e);
                } catch (KeyNotFoundException) {
                    return SendMessage("Translater error", "The string " + e.Message + " could not be found", MessageTypes.Error, e);
                }
            } else {
                return MessageHandler.SendException(e);
            }
        }


        public static ResponseType SendInfo(string name, params string[] variables) {
            StringCollection strings = Strings.getStrings(name);

            return MessageHandler.SendInfo(strings[StringType.Title].interpret(),
                strings[StringType.Message].interpret(variables));
        }
        public static ResponseType SendWarning(string name, params string[] variables) {
            return SendWarning(name, null, variables);
        }
        public static ResponseType SendWarning(string name, Exception e, params string[] variables) {
            StringCollection strings = Strings.getStrings(name);

            return MessageHandler.SendWarning(strings[StringType.Title].interpret(), strings[StringType.Message].interpret(variables), e);
        }
        public static ResponseType SendError(string name, params string[] variables) {
            return SendError(name, null, variables);
        }
        public static ResponseType SendError(string name, Exception e, params string[] variables) {
            StringCollection strings = Strings.getStrings(name);

            return MessageHandler.SendError(strings[StringType.Title].interpret(), strings[StringType.Message].interpret(variables), e);
        }
    }
}
