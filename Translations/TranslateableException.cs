using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translations
{
    public class TranslateableException: Exception
    {
        public TranslateableException(string name, Exception inner): 
            base(name,inner)
        {

        }
        public TranslateableException(string name) :
            base(name)
        { }
    }
}
