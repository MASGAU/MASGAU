using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using MVC;
using MASGAU.Location;
using Translator;
namespace MASGAU {
    public abstract class Common: ANotifyingObject {
        // Common constants for use throughout the program
        public const string Extension = ".gb7";
        public const string Seperator = "«";
        public const string OwnerSeperator = "@";
        public const string MasgauUrl = "http://masgau.org/";
        public const string GameSaveInfoUrl = "http://gamesave.info/";
        public const string SubmissionEmail = "submissions@gamesave.info";

        // Universal objects for all platforms that are awesome
        public static Settings.Settings Settings;
        public static Updater.Updater Updater;
        public static Monitor.Monitor Monitor;

        // Platform-dependent abstract objects
        public static ALocationsHandler Locations;
        public static AStartupHelper Startup;
        public static ASymLinker Linker;

        // Stores the current program version
        public static Version Version = new Version(1, 1, 0);
        public static string VersionString {
            get {
                return Version.ToString();
            }
        }

        #region Easily usable program locations
        public static string ExecutablePath {
            get {
                Assembly temp = Assembly.GetEntryAssembly();
                return temp.Location;
            }
        }
        //public static string ExecutableName {
        //    get {
        //        return Path.GetFileName(ExecutablePath);
        //    }
        //}
        public static string ExecutableFolder {
            get {
                return Path.GetDirectoryName(ExecutablePath);
            }
        }
        #endregion

        // Indicates if we're in portable mode or not, which is determined byt he settings library
        public static Config.ConfigMode Mode {
            get {
                return Settings.mode;
            }
        }


        // Determines wether the program was started in regular mode or in restore mode based on the command line args
        public static AppMode AppMode {
            get {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 0) {
                    foreach (string arg in args) {
                        if (!arg.StartsWith("-") && (arg.EndsWith(Extension) || arg.EndsWith(Extension + "\""))) {
                            return MASGAU.AppMode.Restore;
                        }
                    }
                }
                return MASGAU.AppMode.Main;
            }
        }


        public static bool AllUsersMode {
            get {
                // Checks if the command line indicates we should be running in all users mode
                string[] args = Environment.GetCommandLineArgs();
                for (int i = 0; i < args.Length; i++) {
                    switch (args[i]) {
                        case "-allusers":
                            return true;
                    }
                }
                return false;
            }
        }
        public bool IsAllUsersMode {
            get {
                return Common.AllUsersMode;
            }
        }
        public bool IsSingleUserMode {
            get {
                return !Common.AllUsersMode;
            }
        }

//        public static bool IsReady { get; protected set; }
        public static bool CoreReady { get; protected set; }
        public static bool ProgramReady { get; protected set; }

        protected Common() {
            CoreReady = false;
            ProgramReady = false;

            Settings = new Settings.Settings();


            if(Locations==null)
                Locations = CreateLocationsHandler();


            Startup = CreateStartupHelper("MASGAU", Common.ExecutablePath);
            Linker = CreateSymLinker();

            CommunicationWatcher = new FileSystemWatcher();
            CommunicationWatcher.Path = Settings.SourcePath;
            CommunicationWatcher.Changed += new FileSystemEventHandler(CommunicationWatcher_Changed);
            CommunicationWatcher.Created += new FileSystemEventHandler(CommunicationWatcher_Changed);
            CommunicationWatcher.EnableRaisingEvents = true;


            CoreReady = true;
        }

        static Common() {
            Mutex = new System.Threading.Mutex(false, "MASGAU");

            try {
                if (!Mutex.WaitOne(1000)) {
                    MutexAlreadyTaken = true;
                    switch (AppMode) {
                        case MASGAU.AppMode.Restore:
                            //SendArchivesToMainWindow();
                            break;
                        case MASGAU.AppMode.Main:
                            OpenMainWindow();
                            System.Windows.Forms.Application.Exit();
                            return;
                        default:
                            throw new NotImplementedException(AppMode.ToString());
                    }
                }
            } catch (System.Threading.AbandonedMutexException) {
                Console.Out.Write("Boo-hoo, I'm an abandoned mutex. Suck it up.");
            } catch (Exception) {
                throw new TranslateableException("NoMultipleInstances");

            }

        
        }

        // Abstract functions to allow platform-specific object initialization
        protected abstract ALocationsHandler CreateLocationsHandler();
        protected abstract AStartupHelper CreateStartupHelper(string program_name, string program_path);
        protected abstract ASymLinker CreateSymLinker();

        // Simple function for removing numbers from strings
        public static string makeNumbersOnly(string remove) {
            if (remove.Length > 18)
                remove = remove.Substring(0, 18);
            for (int i = 0; i < remove.Length; i++) {
                try {
                    Int64.Parse(remove.Substring(i, 1));
                } catch {
                    remove = remove.Remove(i, 1);
                    i--;
                }
            }
            return remove;
        }

        #region Mutex stuff
        private static System.Threading.Mutex Mutex;
        public static bool MutexAlreadyTaken {
            get;
            protected set;
        }

        #endregion

        #region Intra-window communication stuff
        private const string CommunicationMutexName = "MASGAUCommunicate";
        private static FileSystemWatcher CommunicationWatcher;
        private const string CommunicationFile = "comunicate.txt";
        private static string CommunicationFileFull {
            get {
                return Path.Combine(Settings.SourcePath, CommunicationFile);
            }
        }

        static void CommunicationWatcher_Changed(object sender, FileSystemEventArgs e) {
            if (File.Exists(CommunicationFileFull)) {
                System.Threading.Mutex mutex = new System.Threading.Mutex(false, CommunicationMutexName);
                try {
                    mutex.WaitOne(10000);
                    CommunicationWatcher.EnableRaisingEvents = false;
                    File.Delete(CommunicationFileFull);
                } catch (Exception) {
                    //handle exception
                } finally {
                    mutex.ReleaseMutex();
                    CommunicationWatcher.EnableRaisingEvents = true;
                }

                MVC.Communication.Interface.InterfaceHandler.showInterface();
            }
        }

        static void OpenMainWindow() {
            System.Threading.Mutex mutex = new System.Threading.Mutex(false, CommunicationMutexName);
            try {
                mutex.WaitOne(10000);

                FileInfo file = new FileInfo(CommunicationFileFull);
                if (file.Exists)
                    file.Delete();
                file.Create();
            } catch (Exception ex) {
                Logger.Logger.log(ex);
            } finally {
                mutex.ReleaseMutex();
            }
        }
        #endregion


        #region Opening Paths
        public static void openPath(string path) {
            System.Diagnostics.Process.Start(path);
        }
        public static void openBackupPath() {
            openPath(Common.Settings.backup_path);
        }
        public static void openSteamPath() {
            openPath(Common.Settings.steam_path);
        }
        //public static void openSyncPath() {
        //    openPath(Common.Settings.sync_path);
        //}
        #endregion

    }
}
