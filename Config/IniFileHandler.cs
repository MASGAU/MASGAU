using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Collections;
namespace Config
{
    class IniFileHandler:  TwoKeyDictionary<String,String,String> 
    {

        public String getValue(String section, String value) {
            try {
                return this.Get(section, value);
            } catch (KeyNotFoundException ex) {
                throw new Exception("Could not find value in INI file",ex);
            }

        }

        private Regex header_regex = new Regex("^\\[.*\\]");
        private Regex value_regex = new Regex(".*=.*");

        public IniFileHandler(FileInfo file) {
            using (StreamReader sr = new StreamReader(file.FullName))
            {
                String line;
                String section = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (header_regex.IsMatch(line))
                    {
                        char[] brackets = { '[', ']' };
                        section = line.Trim(brackets);
                    }
                    if (value_regex.IsMatch(line))
                    {
                        char[] equals = { '=' };
                        string[] pair = line.Split(equals, 2);
                        this.Add(section, pair[0], pair[1]);
                    }

                }
            }
        }
    }
}
