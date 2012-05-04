using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translations
{
    public class TranslateableException: Exception
    {
        string[] variables;
        public TranslateableException(string name, Exception inner, params string[] variables): 
            base(name,inner)
        {
            this.variables = variables;
        }
        public TranslateableException(string name, params string[] variables) :
            base(name)
        {
            this.variables = variables;
        }
    }
}
