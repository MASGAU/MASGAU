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
using Logger;
namespace Translations {
    public class Strings {
        private static string language = "en";
        private static string region = "US";

        private static string language_override = null;

        private static Dictionary<string, string> strings = new Dictionary<string, string>();

        private static XmlReaderSettings xml_settings;

        private static bool translation_mode = false;


        static Strings() {
            // Checks if the command line indicates we should be running in translation mode
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-language_file":
                        translation_mode = true;
                        if (args.Length > i + 1 && !args[i + 1].StartsWith("-"))
                        {
                            i++;
                            language_override = args[i];
                        }
                        else
                        {
                            Logger.Logger.log("No language file specified!");
                        }
                        break;
                }
            }


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
            if (language_override != null)
            {
                if (!translation_mode && File.Exists(Path.Combine("Strings", language_override + ".xml")))
                {
                    loadFile(Path.Combine("Strings", "en.xml"));
                }
            }


            // Load the English file first, so that if there are any strings missing from a translation,
            // at least the user will still see something they can punch in to babelfish
            // If we're in translate mode, this is skipped, so that untranslated strings will
            // Show up as the string name rather than the translated string itself
            if (!translation_mode && File.Exists(Path.Combine("Strings", "en.xml")))
            {
                loadFile(Path.Combine("Strings", "en.xml"));
            }
            else
            {
                Logger.Logger.log("ERROR: en.xml cannot be found! You probably need to re-install!");
            }

            // We start by checking for (and loading) a general string file for the current language
            if (File.Exists(Path.Combine("Strings", language + ".xml")))
            {
                loadFile(Path.Combine("Strings", language + ".xml"));

            }

            // We then load a region-specific string file, so that if several regions use the same translation
            // for a string, we just put them in the common language file, then put only the region-specific
            // strings in this file
            if (File.Exists(Path.Combine("Strings", language + "-" + region + ".xml"))) {
                loadFile(Path.Combine("Strings", language + "-" + region + ".xml"));
            }

        }

        private static void loadFile(string file) {
            XmlDocument strings_xml = new XmlDocument();
            XmlReader parse_me = XmlReader.Create(file, xml_settings);

            try {
                strings_xml.Load(parse_me);
            }
            catch (XmlException ex) {
                IXmlLineInfo info = parse_me as IXmlLineInfo;
                Logger.Logger.log("The file " + file + " has produced this error:" + Environment.NewLine + ex.Message + Environment.NewLine + "The error is on or near line " + info.LineNumber + ", possibly at column " + info.LinePosition + "." + Environment.NewLine + "Go fix it.");
                //throw new Exception("The file " + file + " has produced this error:" + Environment.NewLine + ex.Message + Environment.NewLine + "The error is on or near line " + info.LineNumber + ", possibly at column " + info.LinePosition + "." + Environment.NewLine + "Go fix it.");
            }
            finally {
                parse_me.Close();
            }


            XmlNode nodes = strings_xml.GetElementsByTagName("strings")[0];
            foreach (XmlNode node in nodes.ChildNodes) {
                if (node.Name == "string") {
                    // If the string is already present, then we assume that the new string supercedes the previous one
                    if (strings.ContainsKey(node.Attributes["name"].InnerText))
                    {
                        strings[node.Attributes["name"].InnerText] = node.InnerText;
                    }
                    else
                    {
                        strings.Add(node.Attributes["name"].InnerText, node.InnerText);
                    }
                }
            }
       

            
        }

        public static string get(string name)
        {
            StringBuilder return_me = null;

            if (name == null)
                return "";
            // So, all interface translation strings are going to start with a hash sign.
            // That way we can leave some interface elements alone
            if (name.StartsWith("#"))
            {
                name = name.TrimEnd('#');

                if (strings.ContainsKey(name))
                    return_me = new StringBuilder(strings[name]);

                if (return_me == null)
                {
                    // If running in translate mode, then we'll throw an exception when a string is missing.
                    Logger.Logger.log("STRING " + name + " NOT FOUND");
                    if (translation_mode)
                    {
                        // This behavior will probably not stick, as most of the time this code occurs during GUI drawing,
                        // So Windows wraps this exception in a WPF exception, which effectively hides this info
                        // from the average user. When breaking into debug in Visual Studio though, this allows us
                        // to see exactly which string is missing.
                        return "STRING " + name + " NOT FOUND";
                        //                            throw new Exception("Could not find string \"" + name + "\" in either the current language " + language + "-" + region + " or in the default string library");
                    }
                    else
                    {
                        // This will eventually become the only behavior when a string isn't found,
                        // so that the main interface will just display the name of a string
                        return name;
                    }

                }

                Regex r = new Regex(@"%[A-za-z]*%", RegexOptions.IgnoreCase);

                Match m = r.Match(return_me.ToString());
                int offset = 0;
                while (m.Success)
                {
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
            else
            {
                return name;
            }




        }

        // Event handler to take care of XML errors while reading game configs
        private static void validationHandler(object sender, ValidationEventArgs args) {
            throw new XmlException(args.Message);
        }
    }
}