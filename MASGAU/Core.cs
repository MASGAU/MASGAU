using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using MASGAU.Location;
using MASGAU.Monitor;
//using MASGAU.Task;
using MASGAU.Update;
using Translator;
using MVC;
using System.ComponentModel;
namespace MASGAU
{
    // This basically sets up all the static classes that are used all over MASGAU
    // I'd like to consolidate as much function into this class as possible,
    // but I don't want it to unnecessarily become a convoluted highway of 
    // method forwarding.

    public abstract class Core : ANotifyingObject
    {
        // This allows us to lock the config file across platforms
        public static System.Threading.Mutex mutex = new System.Threading.Mutex(false, "MASGAU");

        // These are values used througout the program
        public const string extension = ".gb7";
        public const string seperator = "«";
        public const string owner_seperator = "@";
        public const string version = "1.0";
        public const string masgau_url = "http://masgau.org/";
        public const string gamesaveinfo_url = "http://gamesave.info/";
        public const string submission_email = "submissions@gamesave.info";



        // Portable-related settings
        public static bool portable_mode { get; protected set; }
        public static string config_location { get; protected set; }

        public const bool Stable = true;

        public static Version program_version = new Version(0, 9, 9);


        // This stores the names of the various programs in masgau
        public static ProgramNames programs = new ProgramNames();

        // This stores whether we're using wpf or gtk
//        public static Interface interface_library = Interface.WPF;

        // This stores what OS we're on
        //private static OperatingSystem os = OperatingSystem.Windows;

        // Shared super-objects
        public static ALocationsHandler locations;
        public static Settings.Settings settings;
        public static Updater updater;
        //public static TaskHandler task;
        public static Monitor.Monitor monitor;

        public static StartupHelper startup;

        // Indicates wether we're running in all users mode
        public static bool all_users_mode = false;

        // Lots of parts of the program would like to know where the exe is
        public static string app_path;

        public static bool initialized = false;

        public BackgroundWorker worker = new BackgroundWorker();


        public bool IsBusy {
            get {
                return worker.IsBusy;
            }
        }
        public void CancelAsync() {
            worker.CancelAsync();
        }
        public void RunWorkerAsync() {
            worker.RunWorkerAsync();
        }
        public event RunWorkerCompletedEventHandler RunWorkerCompleted {
            add { worker.RunWorkerCompleted += value; }
            remove { worker.RunWorkerCompleted -= value; }
        }
        protected bool CancellationPending {
            get {
                return worker.CancellationPending;
            }
        }

        // This indicates wether the sync folder needs re-populated
        public static bool rebuild_sync = false;
        public static bool redetect_games = false;
        public static Email.EmailHandler email { get; protected set; }
        private static bool mutex_acquired = false;
        static Core()
        {
            if (!mutex_acquired && !mutex.WaitOne(100))
            {
                throw new TranslateableException("NoMultipleInstances");
            }


            portable_mode = false;
            config_location = null;

            // Checks if the command line indicates we should be running in all users mode
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-allusers":
                        all_users_mode = true;
                        break;
                    case "-portable":
                        portable_mode = true;
                        break;
                    case "-config":
                        i++;
                        if (args.Length > i)
                            config_location = args[i];
                        break;
                }
            }

            Assembly temp = Assembly.GetExecutingAssembly();

            app_path = Path.GetDirectoryName(temp.Location);


            global::Config.ConfigMode mode;
            if (portable_mode)
            {
                mode = global::Config.ConfigMode.PortableApps;
            }
            else
            {
                if (all_users_mode)
                {
                    mode = global::Config.ConfigMode.SingleUser;
                }
                else
                {
                    mode = global::Config.ConfigMode.SingleUser;
                }
            }
            settings = new Settings.Settings(mode);
            prepareProgramNames();

            email = new Email.EmailHandler(Core.settings.EmailSender, Core.submission_email);
            startup = new StartupHelper("MASGAU", Core.programs.main);
        }

        protected Core()
        {
        }

        private static void prepareProgramNames()
        {
            string bin_root = app_path;
            programs.main = Path.Combine(bin_root, "MASGAU.exe");
            programs.updater = Path.Combine(bin_root, "MASGAU.Updater.exe");
            programs.restore = Path.Combine(bin_root, "MASGAU.Restore.exe");
        }

        public static string makeNumbersOnly(string remove)
        {
            if (remove.Length > 18)
                remove = remove.Substring(0, 18);
            for (int i = 0; i < remove.Length; i++)
            {
                try
                {
                    Int64.Parse(remove.Substring(i, 1));
                }
                catch
                {
                    remove = remove.Remove(i, 1);
                    i--;
                }
            }
            return remove;
        }
        #region Opening Paths
        public static void openPath(string path)
        {
            System.Diagnostics.Process.Start(path);
        }
        public static void openBackupPath()
        {
            openPath(Core.settings.backup_path);
        }
        public static void openSteamPath()
        {
            openPath(Core.settings.steam_path);
        }
        //public static void openSyncPath() {
        //    openPath(Core.settings.sync_path);
        //}
        #endregion


    }
}
