using System;
using System.IO;
using System.Reflection;
using System.Xml;
using MASGAU.Location;
using MASGAU.Archive;
using MASGAU.Config;
using MASGAU.Monitor;
using MASGAU.Update;
using MASGAU.Game;
using MASGAU.Task;
using System.Xml.Schema;

namespace MASGAU
{
    // This basically sets up all the static classes that are used all over MASGAU
    // I'd like to consolidate as much function into this class as possible,
    // but I don't want it to unnecessarily become a convoluted highway of 
    // method forwarding.

    public abstract class Core : BackgroundWorker {
        // This allows us to lock the config file across platforms
        public static System.Threading.Mutex mutex = new System.Threading.Mutex(false, "MASGAU");

        // These are values used througout the program
        public const string extension = ".gb7";
        public const string seperator = "«";
        public const string owner_seperator = "@";
        public const string version = "0.9.2";
        public const string site_url = "http://masgau.org/";

        // Portable-related settings
        public static bool portable_mode {get; protected set;}
        public static string config_location {get; protected set;}

        public static UpdateVersion program_version = new UpdateVersion(0, 9, 2);
        public static UpdateVersion update_compatibility = new UpdateVersion(1, 1, 0);

        // This stores the names of the various programs in masgau
        public static ProgramNames programs = new ProgramNames();

        // This stores wether we're using wpf or gtk
        public static Interface interface_library = Interface.WPF;

        // This stores what OS we're on
        private static OperatingSystem os = OperatingSystem.Windows;

        // Shared super-objects
        public static GamesHandler games;
        public static ALocationsHandler locations;
        public static SettingsHandler settings;
        public static ArchivesHandler archives;
        public static UpdatesHandler updater;
        public static TaskHandler task;
        public static MonitorHandler monitor;

        // Indicates wether we're running in all users mode
        public static bool all_users_mode = false;

        // Lots of parts of the program would like to know where the exe is
        public static string app_path;

        public static bool initialized = false;

        #region Redetection indicators
        // Indicates wether the games need to be re-detected
        private static bool _redetect_games = false;
        public static bool redetect_games {
            get {
                return _redetect_games;
            }
            set {
                _redetect_games = value;
            }
        }

        // Indicates wether the archvies need to be re-detected
        private static bool _redetect_archives = true;
        public static bool redetect_archives {
            get {
                return _redetect_archives;
            }
            set {
                _redetect_archives = value;
            }
        }

        // This indicates wether the sync folder needs re-populated
        public static bool rebuild_sync = false;

        #endregion


        static Core() {
            portable_mode = false;
            config_location = null;

            // Checks if the command line indicates we should be running in all users mode
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) {
                switch(args[i]) {
                    case "-allusers":
                        all_users_mode = true;
                        break;
                    case "-portable":
                        portable_mode = true;
                        break;
                    case "-config":
                        i++;
                        if(args.Length>i)
                            config_location = args[i];
                        break;
                }
            }

            Assembly temp = Assembly.GetExecutingAssembly();

            app_path = Path.GetDirectoryName(temp.Location);

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

            settings = new SettingsHandler();
        }

        protected Core(Interface new_interface) {
            prepareProgramNames(new_interface);
            interface_library = new_interface;
        }

        private static void prepareProgramNames(Interface iface) {
            string bin_root = app_path;
            programs.analyzer = Path.Combine(bin_root, "MASGAU.Analyzer." + iface + ".exe");
            programs.main = Path.Combine(bin_root, "MASGAU.Main." + iface + ".exe");
            programs.monitor = Path.Combine(bin_root, "MASGAU.Monitor." + iface + ".exe");
            programs.updater = Path.Combine(bin_root, "MASGAU.Updater." + iface + ".exe");
            programs.restore = Path.Combine(bin_root, "MASGAU.Restore." + iface + ".exe");
            programs.backup = Path.Combine(bin_root, "MASGAU.Console." + os + ".exe");
        }

        public static string makeNumbersOnly(string remove) {
            if (remove.Length > 18)
                remove = remove.Substring(0, 18);
            for (int i = 0; i < remove.Length; i++) {
                try {
                    Int64.Parse(remove.Substring(i, 1));
                }
                catch {
                    remove = remove.Remove(i, 1);
                    i--;
                }
            }
            return remove;
        }
        #region Opening Paths
        public static void openPath(string path) {
            System.Diagnostics.Process.Start(path);
        }
        public static void openBackupPath() {
            openPath(Core.settings.backup_path);
        }
        public static void openSteamPath() {
            openPath(Core.settings.steam_path);
        }
        public static void openSyncPath() {
            openPath(Core.settings.sync_path);
        }
        #endregion

        // Event handler to take care of XML errors while reading game configs
        private static void validationHandler(object sender, ValidationEventArgs args){
            throw new XmlException(args.Message);
        }

        private static XmlReaderSettings xml_settings = new XmlReaderSettings();
        public static XmlDocument readXmlFile(string name) {
            XmlDocument game_config = new XmlDocument();
            XmlReader parse_me = XmlReader.Create(name, xml_settings);
            try {
                game_config.Load(parse_me);
                return game_config;
            } catch (XmlException ex) {
                IXmlLineInfo info = parse_me as IXmlLineInfo;
                throw new XmlException("The file " + name + " has produced this error:" + Environment.NewLine + ex.Message + Environment.NewLine + "The error is on or near line " + info.LineNumber + ", possibly at column " + info.LinePosition + "." + Environment.NewLine + "Go fix it.");
            } finally {
                parse_me.Close();
            }
        }

    }
}
