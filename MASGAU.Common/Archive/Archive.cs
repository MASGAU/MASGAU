using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Xml;
using MASGAU.Location.Holders;
using MASGAU.Restore;
using MVC;
using MVC.Communication;
using MVC.Translator;
using Translator;
using XmlData;
namespace MASGAU {
    public class Archive : AModelItem<ArchiveID> {
        #region Archive identification stuff
        public string Title {
            get {
                GameEntry game = Games.Get(this.id.Game);
                if (game == null) {
                    return id.Game.Formatted;
                } else {
                    return game.Title;
                }
            }
        }


        public override System.Drawing.Color BackgroundColor {
            get {
                return id.Game.BackgroundColor;
            }
        }
        public override System.Drawing.Color SelectedColor {
            get {
                return id.Game.SelectedColor;
            }
        }

        public string Owner {
            get {
                return id.Owner;
            }
        }
        public string Type {
            get {
                return id.Type;
            }
        }
        public override string ToolTip {
            get {
                return ArchiveFile.Name;
            }
        }

        public string ArchivePath {
            get {
                return ArchiveFile.FullName;
            }
        }

        public FileInfo ArchiveFile { get; protected set; }



        public string TempFileName {
            get {
                // This ~ tells dropbox to ignore the file
                String name = Path.Combine(ArchiveFile.DirectoryName, "~" + Path.GetFileNameWithoutExtension(ArchiveFile.Name) + ".tmp");
                return name;
            }
        }
        public DateTime LastModified {
            get {
                if (ArchiveFile == null)
                    return new DateTime(1955, 11, 5);
                ArchiveFile.Refresh();
                return ArchiveFile.LastAccessTime;
            }
        }


        public string game {
            get {
                return id.ToString();
            }
        }
        #endregion

        public bool Exists {
            get {
                ArchiveFile.Refresh();
                return ArchiveFile.Exists;
            }
        }

        public void Delete() {
            if (this.Exists) {
                this.ArchiveFile.Delete();
                Archives.Remove(this);
            }
        }

        #region static stuff
        private static string ZipExecutableName {
            get {
                return Path.GetFileName(ZipExecutable);
            }
        }
        private static string ZipExecutable {
            get {
                return Path.Combine(Core.ExecutablePath, "7z.exe");
            }
        }
        private static bool ZipExecutablePresent {
            get {
                return File.Exists(ZipExecutable);
            }
        }

        private static bool ready;
        private static Process zipper;

        public static DirectoryInfo MasterTemp {
            get {
                return new DirectoryInfo(Path.Combine(Path.GetTempPath(), "masgau"));
            }
        }


        private string _tempfldr = null;
        private string TempFolder {
            get {
                if (_tempfldr == null) {
                    QuickHash hash = new QuickHash(this.ArchiveFile.FullName);
                    _tempfldr = Path.Combine(MasterTemp.FullName, hash.ToString());
                }
                return _tempfldr;
            }
        }


        private void purgeTemp() {
            try {
                if (Directory.Exists(TempFolder)) {
                    Directory.Delete(TempFolder, true);
                    Thread.Sleep(10);
                }
            } catch (Exception e) {
                throw new TranslateableException("FolderPrepError", e, TempFolder);
            }
        }
        private void prepTemp() {
            try {
                purgeTemp();
                Directory.CreateDirectory(TempFolder);
            } catch (TranslateableException e) {
                throw e;
            } catch (Exception e) {
                throw new TranslateableException("FolderPrepError", e, TempFolder);
            }
        }
        #endregion

        #region Constructor
        public Archive(string folder, ArchiveID new_id) :
            this(new FileInfo(Path.Combine(folder, new_id.ToString()) + Core.Extension), new_id) {
        }

        public Archive(FileInfo archive, ArchiveID new_id) :
            this(archive) {

            this.id = new_id;
        }

        public Archive(FileInfo archive)
            : base() {
            this.ArchiveFile = archive;


            zipper = new Process();

            if (ZipExecutablePresent) {
                zipper.StartInfo.FileName = ZipExecutableName;
                ready = true;
            } else {
                ready = false;
                throw new TranslateableException("FileNotFoundCritical", ZipExecutable);
            }

            zipper.StartInfo.UseShellExecute = false;
            zipper.StartInfo.RedirectStandardOutput = true;
            zipper.StartInfo.RedirectStandardError = true;
            zipper.StartInfo.CreateNoWindow = true;
            zipper.StartInfo.WorkingDirectory = Core.ExecutablePath;

            if (!ready)
                throw new TranslateableException("FileNotFoundCritical", ZipExecutable);


            // If the archive is new, then the following won't apply
            if (!Exists)
                return;

            // Attempts to extract only the masgau.xml identifying file from the game
            List<string> xml_file = new List<string>();
            xml_file.Add("masgau.xml");
            extract(xml_file, false);

            if (File.Exists(Path.Combine(TempFolder, "masgau.xml"))) {
                XmlFile file = new XmlFile(new FileInfo(Path.Combine(TempFolder, "masgau.xml")), false, "masgau_archive");
                XmlElement root = file.DocumentElement;
                id = new ArchiveID(root);

                File.Delete(Path.Combine(TempFolder, "masgau.xml"));
                purgeTemp();

            } else {
                throw new TranslateableException("ArchiveMissingData", ArchiveFile.FullName);
                // This is a fallback for really old ASGAU archives - One day I will be able to just delete this.
                // I'm going to delete this actually
                // Goodbye, legacy!!!!
                //hold_me = file_name.Replace(".gb7", "").Split('@');
                //hold_me = hold_me[0].Split('«');
                //id.name = hold_me[0].Trim(); ;
                //id.platform = GamePlatform.Windows;
                //if(hold_me.Length>1) {
                //    owner = hold_me[1].Trim();
                //} else {
                //    owner = null;
                //}
            }
        }
        #endregion

        #region Backup Stuff
        // This helps determin wether this is the first time the archive has been backed up to
        // it's so we can bypass the date checks as they will not be accurate anymore
        public void backup(ICollection<DetectedFile> files, bool disable_versioning, bool disable_date_check) {
            prepTemp();


            if (Exists) {
                if (!canWrite(ArchiveFile))
                    throw new TranslateableException("WriteDenied", ArchiveFile.FullName);
            } else {
                if (!canWrite(ArchiveFile.Directory))
                    throw new TranslateableException("WriteDenied", ArchiveFile.DirectoryName);

                // If this is the first time writing to the archive, we create the identifying XML
                XmlDocument write_me = new XmlDocument();
                if (File.Exists(Path.Combine(TempFolder, "masgau.xml")))
                    File.Delete(Path.Combine(TempFolder, "masgau.xml"));

                XmlTextWriter write_here = new XmlTextWriter(Path.Combine(TempFolder, "masgau.xml"), System.Text.Encoding.UTF8);
                write_here.Formatting = Formatting.Indented;
                write_here.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                write_here.WriteStartElement("masgau_archive");
                write_here.Close();
                write_me.Load(Path.Combine(TempFolder, "masgau.xml"));
                //XmlNode root = write_me.DocumentElement;

                id.AddElements(write_me);

                FileStream this_file = new FileStream(Path.Combine(TempFolder, "masgau.xml"), FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                write_me.Save(this_file);
                write_here.Close();
                this_file.Close();
            }
            bool files_added = false;
            foreach (DetectedFile file in files) {
                // Copies the particular file to a relative path inside the temp folder
                FileInfo source;
                FileInfo destination;
                if (file.Path == "" || file.Path == null) {
                    source = new FileInfo(Path.Combine(file.AbsoluteRoot, file.Name));
                    destination = new FileInfo(Path.Combine(TempFolder, file.Name));
                } else {
                    source = new FileInfo(Path.Combine(file.AbsoluteRoot, file.Path, file.Name));
                    destination = new FileInfo(Path.Combine(TempFolder, file.Path, file.Name));
                }

                if (!source.Exists)
                    continue;

                DateTime file_write_time = source.LastWriteTime;
                int time_comparison = LastModified.CompareTo(source.LastWriteTime);

                if (Core.settings.IgnoreDateCheck || disable_date_check || time_comparison <= 0) {
                    try {
                        if (!destination.Directory.Exists)
                            destination.Directory.Create();
                    } catch (Exception e) {
                        TranslatingMessageHandler.SendError("CreateError", e, destination.DirectoryName);
                        continue;
                    }

                    if (source.Exists) {
                        try {
                            source.CopyTo(destination.FullName, true);
                        } catch (Exception e) {
                            TranslatingMessageHandler.SendError("CopyError", e, source.FullName, destination.FullName);
                            continue;
                        }
                    } else {
                        TranslatingMessageHandler.SendError("FileToCopyNotFound", source.FullName);
                        continue;
                    }

                    if (destination.Exists) {
                        try {
                            if ((destination.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                File.SetAttributes(destination.FullName, FileAttributes.Normal);
                        } catch (Exception e) {
                            TranslatingMessageHandler.SendError("PermissionChangeError", e, destination.FullName);
                            continue;
                        }
                    } else {
                        TranslatingMessageHandler.SendError("FileCopiedNotFound", source.FullName, destination.FullName);
                        continue;
                    }
                    files_added = true;
                }
                //file_date = new FileInfo(file_name).LastWriteTime;
            }
            if (files_added)
                add(TempFolder, disable_versioning);

            purgeTemp();
        }

        #endregion

        #region Restore stuff
        public bool cancel_restore = false;
        public RestoreResult restore(DirectoryInfo destination, List<string> only_these) {
            cancel_restore = false;
            TranslatingProgressHandler.setTranslatedMessage("CheckingDestination");
            // The files get extracted to the temp folder, so this sets up our ability to read them
            DirectoryInfo from_here = new DirectoryInfo(TempFolder);

            // If it's a destination that doesn't yet exist
            if (!destination.Exists) {
                try {
                    destination.Create();
                    // This sets the permissions on the folder to be for everyone
                    DirectorySecurity everyone = destination.GetAccessControl();
                    string everyones_name = @"Everyone";
                    everyone.AddAccessRule(new FileSystemAccessRule(everyones_name, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    destination.SetAccessControl(everyone);
                } catch {
                    // This is if the folder creation fails
                    // It means we have to run as admin, but we check here if we already are
                    if (!SecurityHandler.amAdmin()) {
                        // If we aren't we elevate ourselves
                        RestoreResult res = restoreElevation(destination.FullName);
                        if (res != RestoreResult.Success) {
                            return res;
                        }
                    } else {
                        throw new TranslateableException("UnableToCreateOutputFolder", destination.FullName);
                    }
                }
            }
            if (cancel_restore)
                return RestoreResult.Cancel;

            TranslatingProgressHandler.setTranslatedMessage("ExtractingArchive");
            ProgressHandler.state = ProgressState.Indeterminate;
            if (only_these == null) {
                ProgressHandler.max = file_count;
                extract(true);
            } else {
                ProgressHandler.max = only_these.Count;
                extract(only_these, true);
            }
            ProgressHandler.value = 0;
            ProgressHandler.state = ProgressState.Normal;

            // Clean up the masgau archive ID
            if (File.Exists(Path.Combine(TempFolder, "masgau.xml")))
                File.Delete(Path.Combine(TempFolder, "masgau.xml"));

            if (cancel_restore)
                return RestoreResult.Cancel;

            TranslatingProgressHandler.setTranslatedMessage("CopyingFilesToDestination");
            if (!canWrite(destination)) {
                Directory.Delete(TempFolder, true);

                RestoreResult res = restoreElevation(destination.FullName);
                if (res != RestoreResult.Success) {
                    return res;
                }
                cancel_restore = true;
            }
            if (cancel_restore)
                return RestoreResult.Cancel;

            copyFolders(from_here, destination, true);

            if (cancel_restore)
                return RestoreResult.Cancel;

            purgeTemp();

            ProgressHandler.state = ProgressState.None;

            return RestoreResult.Success;
        }
        private RestoreResult restoreElevation(string destination) {
            try {
                //   if (SecurityHandler.amAdmin()) {
                //     TranslatingMessageHandler.SendError("UnableToCreateOutputFolderAdmin", destination);
                //   return false;
                //}

                MVC.Communication.Interface.InterfaceHandler.disableInterface();
                if (!TranslatingRequestHandler.Request(RequestType.Question, "UnableToCreateOutputFolderRequest", destination).Cancelled) {
                    try {
                        MVC.Communication.Interface.InterfaceHandler.disableInterface();
                        ElevationResult res = SecurityHandler.elevation(Core.ExecutableName, "\"" + ArchiveFile.FullName + "\"", true);
                        switch (res) {
                            case ElevationResult.Success:
                                return RestoreResult.Success;
                            case ElevationResult.Cancelled:
                                return RestoreResult.ElevationDenied;
                            case ElevationResult.Failed:
                                return RestoreResult.ElevationFailed;
                            default:
                                throw new NotSupportedException(res.ToString());
                        }
                    } catch (Exception e) {
                        throw new TranslateableException("RestoreProgramNotFound", e, Core.ExecutableName);
                    }
                } else {
                    return RestoreResult.ElevationDenied;
                }
            } finally {
                //MVC.Communication.Interface.InterfaceHandler.showInterface();
                MVC.Communication.Interface.InterfaceHandler.enableInterface();
                MVC.Communication.Interface.InterfaceHandler.closeInterface();
            }
        }


        #endregion

        #region 7-zip stuff
        // Compression-related options
        protected static string compress_mode = "a";
        protected static string file_type = "7z";
        protected static bool solid_mode = false;
        protected static bool multi_threading = true;
        protected static int compress_level = 9;

        private static string compress_switches {
            get {
                // Sets the mode and file type
                StringBuilder return_me = new StringBuilder(compress_mode + " -t" + file_type);
                // Solid mode is on by default in 7zip
                if (!solid_mode)
                    return_me.Append(" -ms=off");

                //Compression level
                return_me.Append(" -mx=" + compress_level.ToString());

                if (multi_threading)
                    return_me.Append(" -mmt=on");
                else
                    return_me.Append(" -mmt=off");
                return return_me.ToString();
            }
        }
        private void run7z(bool send_progress) {
            if (this.Exists) {
                string status = ProgressHandler.message;
                int wait_max = 30;
                for (int count = 0; !canRead(ArchiveFile.FullName); count++) {
                    TranslatingProgressHandler.setTranslatedMessage("ArchiveInUseWaiting", status.TrimEnd('.'), (count + 1).ToString(), wait_max.ToString());
                    Thread.Sleep(1000);
                    if (count == wait_max - 1)
                        throw new TranslateableException("FileInUseTimeout", ArchiveFile.FullName, wait_max.ToString());
                }
            }
            try {
                zipper.Start();
                string output = zipper.StandardOutput.ReadLine();
                while (output != null) {
                    if (send_progress) {
                        if (output.StartsWith("Extracting") || output.StartsWith("Compressing"))
                            ProgressHandler.value++;
                    }
                    output = zipper.StandardOutput.ReadLine();
                }
            } catch (Exception e) {
                throw new TranslateableException("ExeRunError", e, ZipExecutable, zipper.StartInfo.Arguments, zipper.StartInfo.WorkingDirectory);
            } finally {
                try {
                    zipper.Close();
                } catch { }
            }
        }

        private void add(string file, bool disable_versioning) {
            FileInfo TempFile = new FileInfo(TempFileName);
            if (TempFile.Exists)
                TempFile.Delete();


            if (this.Exists) {
                ArchiveFile.CopyTo(TempFileName);
                File.SetAttributes(TempFileName, FileAttributes.Hidden);
            }

            zipper.StartInfo.Arguments = compress_switches + " \"" + TempFileName + "\" \"" + file + "\\*\"";
            run7z(false);

            File.SetAttributes(TempFileName, FileAttributes.Hidden);

            // This here's the versioning stuff. Since it's here, it's universal.
            // This handles versioning copies
            if (!disable_versioning && Core.settings.VersioningEnabled) {
                if (Core.settings.VersioningMax != 0) {
                    DateTime right_now = DateTime.Now;
                    FileInfo original_file = new FileInfo(file);
                    if (original_file.Exists) {
                        if (right_now.Ticks - original_file.CreationTime.Ticks > Core.settings.VersioningTicks) {
                            string new_path = Path.Combine(original_file.DirectoryName, Path.GetFileNameWithoutExtension(original_file.FullName) + "@" + right_now.ToString().Replace('/', '-').Replace(':', '-') + Path.GetExtension(original_file.FullName));
                            try {
                                File.Move(file, new_path);
                                //File.SetCreationTime(temp_file_name,right_now);
                            } catch (Exception ex) {
                                throw new TranslateableException("RevisionCopyError", ex, original_file.FullName);
                            }
                        } else {
                            // This is if it hasn't been long enough for a new file
                        }
                    } else {
                        // This shouldn't really be an error, as it just means that it's a new archive
                        //MessageHandler.SendError("Where'd That Come From?","The file " + file_name + " can't be found.");
                    }
                } else {
                    //this is if the Settings for versioning are fucked up
                }

                FileInfo[] count_us = new DirectoryInfo(Path.GetDirectoryName(file)).GetFiles(
                    Path.GetFileNameWithoutExtension(file) + "@*");


                if (count_us.Length > Core.settings.VersioningMax) {
                    Array.Sort(count_us, new MASGAU.Comparers.FileInfoComparer(true));
                    for (long i = Core.settings.VersioningMax; i < count_us.Length; i++) {
                        try {
                            (count_us[i] as FileInfo).Delete();
                        } catch (Exception ex) {
                            TranslatingMessageHandler.SendError("DeleteError", ex, (count_us[i] as FileInfo).Name);
                        }
                    }
                }
            } else {
                // This is if versioning is disabled, or something

            }

            if (Exists)
                ArchiveFile.Delete();

            TempFile.MoveTo(ArchiveFile.FullName);
            File.SetAttributes(ArchiveFile.FullName, FileAttributes.Normal);
        }


        private string list_switches = "l";
        public int file_count {
            get {
                List<string> lines = getFileListing();

                string[] hold_this = lines[lines.Count - 1].Substring(53).Split(' ');

                return Int32.Parse(hold_this[0]);
            }
        }
        public List<string> file_list {
            get {
                List<string> return_me = new List<string>();

                List<string> lines = getFileListing();
                bool file_list_started = false;
                foreach (string line in lines) {
                    if (line.StartsWith("-----")) {
                        file_list_started = true;
                        continue;
                    } else if (line.StartsWith("     ")) {
                        file_list_started = false;
                    }
                    if (file_list_started && line.Substring(20, 1) != "D") {
                        string add_me = line.Substring(53);
                        if (add_me != "masgau.xml")
                            return_me.Add(add_me);
                    }
                }

                return return_me;
            }
        }

        private List<string> getFileListing() {
            List<String> return_me = new List<string>();
            zipper.StartInfo.Arguments = list_switches + " \"" + ArchiveFile.FullName + "\"";
            zipper.Start();

            string output = zipper.StandardOutput.ReadLine();
            while (output != null) {
                return_me.Add(output);
                output = zipper.StandardOutput.ReadLine();
            }

            zipper.Close();

            return return_me;
        }

        private const string extract_switches = "x";
        private void extract(bool send_progress) {
            prepTemp();
            zipper.StartInfo.Arguments = extract_switches + " \"" + ArchiveFile.FullName + "\" -o\"" + TempFolder + "\"";
            run7z(send_progress);
        }

        private void extract(List<string> files, bool send_progress) {
            prepTemp();
            if (send_progress) {
                ProgressHandler.max = files.Count;
                ProgressHandler.value = 0;
            }
            foreach (string file in files) {
                zipper.StartInfo.Arguments = extract_switches + " \"" + ArchiveFile.FullName + "\" \"" + file + "\" -o\"" + TempFolder + "\"";
                run7z(false);
                if (send_progress)
                    ProgressHandler.value++;
            }
        }
        #endregion

        #region Helper methods
        // Returns a list of the files in the archive


        // Static method for copying folders
        public bool copyFolders(DirectoryInfo from_here, DirectoryInfo to_here, bool send_progress) {
            DirectoryInfo now_here;
            if (!to_here.Exists) {
                try {
                    Directory.CreateDirectory(to_here.FullName);
                } catch (Exception e) {
                    throw new TranslateableException("CreateError", e, to_here.FullName);
                }
            }
            foreach (FileInfo copy_me in from_here.GetFiles()) {
                if (cancel_restore)
                    break;
                if (send_progress)
                    ProgressHandler.value++;
                try {
                    File.Copy(copy_me.FullName, Path.Combine(to_here.FullName, copy_me.Name), true);
                    if ((File.GetAttributes(Path.Combine(to_here.FullName, copy_me.Name)) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        File.SetAttributes(Path.Combine(to_here.FullName, copy_me.Name), FileAttributes.Normal);
                } catch (Exception e) {
                    throw new TranslateableException("CopyError", e, copy_me.FullName, to_here.FullName);
                }
            }
            foreach (DirectoryInfo check_me in from_here.GetDirectories()) {
                if (cancel_restore)
                    break;
                now_here = new DirectoryInfo(Path.Combine(to_here.FullName, check_me.Name));
                if (!copyFolders(check_me, now_here, send_progress))
                    return false;
            }
            return true;
        }


        // Static methods for determining access
        public static bool canRead(string file) {
            return canRead(new FileInfo(file));
        }
        public static bool canRead(FileInfo file) {
            try {
                FileStream close_me = file.Open(FileMode.Open, FileAccess.Read);
                close_me.Close();
                return true;
            } catch (Exception) {
                return false;
            }
        }
        public static bool canWrite(DirectoryInfo path) {
            if (!path.Exists) {
                try {
                    //createPath(path); WTF was I thinking? This shall remain here out of shame.
                    path.Create();
                } catch (Exception) {
                    return false;
                }
            }

            path.Refresh();

            if (path.Exists) {
                string file_name = "~" + Path.GetRandomFileName() + ".tmp";
                FileInfo test_file = new FileInfo(Path.Combine(path.FullName, file_name));
                try {
                    FileStream delete_me = test_file.Create();
                    delete_me.Close();
                } catch (Exception) {
                    return false;
                }
                for (int i = 0; i <= 5; i++) {
                    try {
                        test_file.Delete();
                        return true;
                    } catch (Exception) {
                        Thread.Sleep(5000);
                    }
                }
            }
            return false;
        }
        public static bool canWrite(FileInfo file) {
            try {
                if (file.Exists) {
                    FileStream close_me = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    close_me.Close();
                } else {
                    FileStream close_me = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    close_me.Close();
                    file.Delete();
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }
        #endregion

    }
}
