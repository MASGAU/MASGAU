using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication.Progress;
using Translations;
namespace Communication.Translator
{
    public class TranslatingProgressHandler: ProgressHandler
    {
        public static void setTranslatedMessage(string name, params string[] variables) {
            String line = Strings.getGeneralString(name, variables);
            ProgressHandler.message = line;

        }
    }
}
