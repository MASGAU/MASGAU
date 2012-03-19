using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
namespace Translations {
    public class Strings {
        private static string language = "en";
        private static string region = "US";

        private static Dictionary<string, string> strings = new Dictionary<string, string>();

        private static XmlReaderSettings xml_settings;

        static Strings() {
            xml_settings = new XmlReaderSettings();
            xml_settings.ConformanceLevel = ConformanceLevel.Auto;
            xml_settings.IgnoreWhitespace = true;
            xml_settings.IgnoreComments = true;
            xml_settings.IgnoreProcessingInstructions = false;
            xml_settings.DtdProcessing = DtdProcessing.Parse;
            //xml_settings.ProhibitDtd = false;
            xml_settings.ValidationType = ValidationType.Schema;
            xml_settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
            xml_settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            xml_settings.ValidationEventHandler += new ValidationEventHandler(validationHandler);

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            RegionInfo currentRegion = new RegionInfo(currentCulture.LCID);

            language = currentCulture.TwoLetterISOLanguageName;
            region = currentRegion.TwoLetterISORegionName;

            loadRegion();
        }

        public static void overrideRegion(string new_language, string new_region) {
            language = new_language;
            region = new_region;
        }

        private static void loadRegion() {
            string file;
            if (File.Exists(Path.Combine("Strings", language + "-" + region + ".xml"))) {
                file = Path.Combine("Strings", language + "-" + region + ".xml");
            }
            else if (File.Exists(Path.Combine("Strings", language + ".xml"))) {
                file = Path.Combine("Strings", language + ".xml");
            }
            else if (File.Exists(@"Strings\en.xml")) {
                file = @"Strings\en.xml";
            }
            else {
                throw new Exception("Strings not found for " + language + "-" + region);
            }
            loadFile(file);
        }

        private static void loadFile(string file) {
            XmlDocument strings_xml = new XmlDocument();
            XmlReader parse_me = XmlReader.Create(file, xml_settings);

            try {
                strings_xml.Load(parse_me);
            }
            catch (XmlException ex) {
                IXmlLineInfo info = parse_me as IXmlLineInfo;
                throw new Exception("The file " + file + " has produced this error:" + Environment.NewLine + ex.Message + Environment.NewLine + "The error is on or near line " + info.LineNumber + ", possibly at column " + info.LinePosition + "." + Environment.NewLine + "Go fix it.");
            }
            finally {
                parse_me.Close();
            }


            XmlNode nodes = strings_xml.GetElementsByTagName("strings")[0];
            foreach (XmlNode node in nodes.ChildNodes) {
                if (node.Name == "string") {
                    strings.Add(node.Attributes["name"].InnerText, node.InnerText);
                }
            }
       

            
        }

        public static string get(string name)
        {
            StringBuilder return_me = null;

            if (name == null)
                return "NULL STRING";

            if (name == "")
                return "EMPTY STRING";

            if (strings.ContainsKey(name))
                return_me = new StringBuilder(strings[name]);

            if (strings.ContainsKey(name))
                return_me = new StringBuilder(strings[name]);

            if (return_me == null)
            {
                switch (name)
                {
                    case "-":
                    case ":":
                        return name;
                    default:
                        throw new Exception("Could not find string \"" + name + "\" in either the current language " + language + "-" + region + " or in the default string library");
                }

            }

            Regex r = new Regex(@"%[A-za-z]*%", RegexOptions.IgnoreCase);

            Match m = r.Match(return_me.ToString());
            int offset = 0;
            while (m.Success) {
                foreach (Group g in m.Groups)
                {
                    foreach (Capture c in g.Captures)
                    {
                        string key = c.Value.Trim('%');
                        string line = get(key);
                        return_me.Remove(c.Index + offset, c.Length);
                        return_me.Insert(c.Index + offset, line);
                        offset += line.Length - c.Length;
                    }
                }
                m = m.NextMatch();
            }

            return return_me.ToString();
        }

        // Event handler to take care of XML errors while reading game configs
        private static void validationHandler(object sender, ValidationEventArgs args) {
            throw new XmlException(args.Message);
        }
    }
}