using System;
using MVC.Communication;

using Translator;
namespace MVC.Translator {
    public class TranslatingProgressHandler : ProgressHandler {
        public static void setTranslatedMessage(string name, params string[] variables) {
            String line = Strings.getString(StringType.Message, name, variables);
            if (line == name) {
                throw new StringNotFoundException(name, StringType.Message);
            }
            ProgressHandler.message = line;

        }
    }
}
