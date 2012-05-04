using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translations.WPF
{
    public interface ITranslateableWindow
    {

        bool askQuestion(string title, string message);
        bool showError(string title, string message);
        bool showWarning(string title, string message);
        bool showInfo(string title, string message);
    }
}
