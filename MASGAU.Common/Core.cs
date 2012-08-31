using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Threading;
using MASGAU.Location;
using MASGAU.Monitor;
//using MASGAU.Task;
using Updater;
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
        public static AppMode AppMode {
            get  {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 0) {
                    foreach (string arg in args) {
                        if (!arg.StartsWith("-") && (arg.EndsWith(Core.Extension) || arg.EndsWith(Core.Extension + "\""))) {
                            return MASGAU.AppMode.Restore;
                        }
                    }
                }
                return MASGAU.AppMode.Main;
            }
        }

        // These are values used througout the program
        public const string Extension = ".gb7";
        public const string seperator = "«";
        public const string owner_seperator = "@";
        public const string version = "1.0";
        public const string masgau_url = "http://masgau.org/";
        public const string gamesaveinfo_url = "http://gamesave.info/";
        public const string submission_email = "submissions@gamesave.info";

        public const bool Stable = true;

        public static Version ProgramVersion = new Version(1, 0, 0);


        // This stores the names of the various programs in masgau
        public static string LibsFolder {
            get {
                return Path.Combine(ExecutablePath, "libs");
            }
        }
        public static string ExecutableName { get; protected set; }
        public static string ExecutablePath {
            get {
                return Path.GetDirectoryName(ExecutableName);
            }
        }

        // This stores whether we're using wpf or gtk
//        public static Interface interface_library = Interface.WPF;

        // This stores what OS we're on
        //private static OperatingSystem os = OperatingSystem.Windows;

        // Shared super-objects
        public static ALocationsHandler locations;
        public static Settings.Settings settings;
        public static Updater.Updater updater;
        //public static TaskHandler task;
        public static Monitor.Monitor monitor;

        public static StartupHelper startup;

        // Indicates wether we're running in all users mode
        public static bool StaticAllUsersMode {
            get {
                // Checks if the command line indicates we should be running in all users mode
                string[] args = Environment.GetCommandLineArgs();
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-allusers":
                            return true;
                    }
                }
                return false;
            }
        }
        public bool AllUsersMode {
            get {
                return StaticAllUsersMode;
            }
        }
        public bool SingleUserMode {
            get {
                return !this.AllUsersMode;
            }
        }

        // Lots of parts of the program would like to know where the exe is

        public static bool initialized = false;

        public BackgroundWorker worker = new BackgroundWorker();

        public static Config.ConfigMode Mode {
            get {
                return settings.mode;
            }
        }

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



        private static System.Threading.Mutex mutex;
        public static bool MutexAlreadyTaken {
            get;
            protected set;
        }

        private const string CommunicationMutexName = "MASGAUCommunicate";
        private static Mutex CommunicationMutex;
        private static FileSystemWatcher CommunicationWatcher;

        private const string CommunicationFile = "comunicate.txt";
        private static string CommunicationFileFull {
            get {
                return Path.Combine(Core.settings.SourcePath, CommunicationFile);
            }
        }
        static void CommunicationWatcher_Changed(object sender, FileSystemEventArgs e) {
            if (File.Exists(CommunicationFileFull)) {
                Mutex mutex = new Mutex(false, CommunicationMutexName);
                try {
                    mutex.WaitOne(10000);
                    CommunicationWatcher.EnableRaisingEvents = false;
                    File.Delete(CommunicationFileFull);
                } catch (Exception ex) {
                    //handle exception
                } finally {
                    mutex.ReleaseMutex();
                    CommunicationWatcher.EnableRaisingEvents = true;
                }

                MVC.Communication.Interface.InterfaceHandler.showInterface();
            }
        }
        //static void SendArchivesToMainWindow() {
        //    Mutex mutex = new Mutex(false, CommunicationMutexName);
        //    try {
        //        mutex.WaitOne(10000);
        //        TextWriter writer = new StreamWriter(CommunicationFile);
        //        File.Create(CommunicationFile);

        //    } catch (Exception ex) {
        //        //handle exception
        //    } finally {
        //        mutex.ReleaseMutex();
        //    }
        //}

        static void OpenMainWindow() {
            Mutex mutex = new Mutex(false, CommunicationMutexName);
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
        public static bool Ready { get; protected set; }
        static Core()
        {
            Ready = false;
            mutex = new System.Threading.Mutex(false, "MASGAU");
            settings = new Settings.Settings();

            try {
                if (!mutex.WaitOne(1000)) {
//                    MVC.Translator.TranslatingMessageHandler.SendError("NoMultipleInstances");
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
            } catch (AbandonedMutexException ex) {
                Console.Out.Write("Boo-hoo, I'm an abandoned mutex. Suck it up.");
            } catch (Exception e) {
                throw new TranslateableException("NoMultipleInstances");

            }



            Assembly temp = Assembly.GetEntryAssembly();

            ExecutableName = temp.Location;


            CommunicationWatcher = new FileSystemWatcher();
            CommunicationWatcher.Path = Core.settings.SourcePath;
            CommunicationWatcher.Changed += new FileSystemEventHandler(CommunicationWatcher_Changed);
            CommunicationWatcher.Created += new FileSystemEventHandler(CommunicationWatcher_Changed);
            CommunicationWatcher.EnableRaisingEvents = true;


            email = new Email.EmailHandler(Core.settings.EmailSender, Core.submission_email);
            startup = new StartupHelper("MASGAU", Core.ExecutableName);
            Ready = true;

        }

        protected Core()
        {
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
