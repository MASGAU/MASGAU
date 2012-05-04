using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translations
{
    public class StringCollection: Dictionary<StringType,string>
    {
        string name;
        public StringCollection(string name)
        {
            this.name = name;
        }

        public StringCollection copyInto(StringCollection here) {
            foreach (StringType type in this.Keys)
            {
                if (here.ContainsKey(type))
                    here[type] = this[type];
                else
                    here.Add(type, this[type]);
            }
            return here;

        }

    }
}
