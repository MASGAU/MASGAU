using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Xml;
using MVC.Communication;
using Communication.Translator;
using MASGAU.Location.Holders;
using MVC;
using XmlData;
using Translator;
namespace MASGAU {
    public class Archive : AModelItem<ArchiveID> {
        #region Archive identification stuff
        public string Title {
            get {
                GameVersion game = Games.Get(this.id.Game);
                if (game == null) {
                    return id.Game.Formatted;
                } else {
                    return game.Title;
                }
            }
        }


        public System.Drawing.Color BackgroundColor {
            get {
                return id.Game.BackgroundColor;
            }
        }
        public System.Drawing.Color SelectedColor {
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
        public new string ToolTip {
            get {
                return FileName;
            }
        }


        public string FileName { get; protected set; }

        public string TempFileName {
            get {
                // This ~ tells dropbox to ignore the file
                String name = Path.Combine(Path.GetDirectoryName(FileName), "~" + Path.GetFileNameWithoutExtension(FileName) + ".tmp");
                return name;
            }
        }

        public DateTime LastModified { get; set; }
        public string game {
            get {
                return id.ToString();
            }
        }
        #endregion

        public bool Exists {
            get {
                FileInfo file = new FileInfo(FileName);
                return file.Exists;
            }
        }


        #region static stuff
        private static bool ready;
        private static Process zipper;
        public static string temp_folder;
        static Archive() {
            zipper = new Process();
            temp_folder = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "MASGAU");

            string path = Path.Combine(Core.app_path, "7z.exe");
            if (File.Exists(path)) {
                zipper.StartInfo.FileName = path;
                ready = true;
            } else {
                ready = false;
                throw new TranslateableException("FileNotFoundCritical","7z.exe");
            }
            zipper.StartInfo.UseShellExecute = false;
            zipper.StartInfo.RedirectStandardOutput = true;
            zipper.StartInfo.RedirectStandardError = true;
            zipper.StartInfo.CreateNoWindow = true;
            zipper.StartInfo.WorkingDirectory = temp_folder;

        }
        private static void prepTemp() {
            try {
                if (Directory.Exists(temp_folder)) {
                    Directory.Delete(temp_folder, true);
                    Thread.Sleep(10);
                }
                Directory.CreateDirectory(temp_folder);
            } catch (Exception e) {
                throw new TranslateableException("FolderPrepError", e, temp_folder);
            }
        }
        #endregion

        #region Constructor
        public Archive(string folder, ArchiveID new_id) :
            this(new FileInfo(Path.Combine(folder, new_id.ToString()) + Core.extension), new_id) {
        }

        public Archive(FileInfo archive, ArchiveID new_id) :
            this(archive) {

            this.id = new_id;
        }

        public Archive(FileInfo archive)
            : base() {
            if (!ready)
                throw new TranslateableException("FileNotFoundCritical","7z.exe");

            FileName = archive.FullName;

            if (Exists)
                LastModified = archive.LastWriteTime;
            else // A red-letter date in the history of science
                LastModified = new DateTime(1955, 11, 5);

            // If the archive is new, then the following won't apply
            if (!Exists)
                return;

            // Attempts to extract only the masgau.xml identifying file from the game
            List<string> xml_file = new List<string>();
            xml_file.Add("masgau.xml");
            extract(xml_file, false);

            if (File.Exists(Path.Combine(temp_folder, "masgau.xml"))) {
                XmlFile file = new XmlFile(new FileInfo(Path.Combine(temp_folder, "masgau.xml")),false);
                XmlElement root = file.ChildNodes[1] as XmlElement;



                id = new ArchiveID(root);


                File.Delete(Path.Combine(temp_folder, "masgau.xml"));
            } else {
                throw new TranslateableException("ArchiveMissingData", FileName);
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
        public void backup(ICollection<DetectedFile> files, bool disable_versioning) {
            prepTemp();


            if (Exists) {
                if (!canWrite(new FileInfo(FileName)))
                    throw new TranslateableException("WriteDenied", FileName);
            } else {
                if (!canWrite(new DirectoryInfo(Path.GetDirectoryName(FileName))))
                    throw new TranslateableException("WriteDenied", Path.GetDirectoryName(FileName));

                // If this is the first time writing to the archive, we create the identifying XML
                XmlDocument write_me = new XmlDocument();
                if (File.Exists(Path.Combine(temp_folder, "masgau.xml")))
                    File.Delete(Path.Combine(temp_folder, "masgau.xml"));

                XmlTextWriter write_here = new XmlTextWriter(Path.Combine(temp_folder, "masgau.xml"), System.Text.Encoding.UTF8);
                write_here.Formatting = Formatting.Indented;
                write_here.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                write_here.WriteStartElement("masgau_archive");
                write_here.Close();
                write_me.Load(Path.Combine(temp_folder, "masgau.xml"));
                //XmlNode root = write_me.DocumentElement;

                id.AddElements(write_me);

                FileStream this_file = new FileStream(Path.Combine(temp_folder, "masgau.xml"), FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                write_me.Save(this_file);
                write_here.Close();
                this_file.Close();
            }
            bool files_added = false;
            foreach (DetectedFile file in files) {
                // Copies the particular file to a relative path inside the temp folder
                string from_here, in_here, to_here;
                if (file.Path == "") {
                    from_here = file.AbsoluteRoot;
                    in_here = file.name;
                } else {
                    from_here = Path.Combine(file.AbsoluteRoot, file.Path);
                    in_here = Path.Combine(file.Path, file.name);
                }

                from_here = Path.Combine(from_here, file.name);
                to_here = Path.Combine(temp_folder, in_here);

                DateTime file_write_time = File.GetLastWriteTime(from_here);
                int time_comparison = DateTime.Compare(LastModified, file_write_time);
                if (Core.settings.IgnoreDateCheck || time_comparison <= 0) {
                    try {
                        if (!Directory.Exists(Path.GetDirectoryName(to_here)))
                            Directory.CreateDirectory(Path.GetDirectoryName(to_here));
                    } catch (Exception e) {
                        TranslatingMessageHandler.SendError("CreateError", e, Path.GetDirectoryName(to_here));
                        continue;
                    }

                    if (File.Exists(from_here)) {
                        try {
                            File.Copy(from_here, to_here, true);
                        } catch (Exception e) {
                            TranslatingMessageHandler.SendError("CopyError", e, from_here, to_here);
                            continue;
                        }
                    } else {
                        TranslatingMessageHandler.SendError("FileToCopyNotFound", from_here);
                        continue;
                    }

                    if (File.Exists(to_here)) {
                        try {
                            if ((File.GetAttributes(to_here) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                File.SetAttributes(to_here, FileAttributes.Normal);
                        } catch (Exception e) {
                            TranslatingMessageHandler.SendError("PermissionChangeError", e, to_here);
                            continue;
                        }
                    } else {
                        TranslatingMessageHandler.SendError("FileCopiedNotFound", from_here, to_here);
                        continue;
                    }
                    files_added = true;
                }
                //file_date = new FileInfo(file_name).LastWriteTime;
            }
            if (files_added)
                add(temp_folder, disable_versioning);
            Directory.Delete(temp_folder, true);
        }

        #endregion

        #region Restore stuff
        public bool cancel_restore = false;
        public void restore(DirectoryInfo destination, List<string> only_these) {
            cancel_restore = false;
            TranslatingProgressHandler.setTranslatedMessage("CheckingDestination");
            // The files get extracted to the temp folder, so this sets up our ability to read them
            DirectoryInfo from_here = new DirectoryInfo(temp_folder);

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
                        restoreElevation(destination.FullName);
                        cancel_restore = true;
                    } else {
                        throw new TranslateableException("UnableToCreateOutputFolder", destination.FullName);
                    }
                }
            }
            if (cancel_restore)
                return;

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
            if (File.Exists(Path.Combine(temp_folder, "masgau.xml")))
                File.Delete(Path.Combine(temp_folder, "masgau.xml"));

            if (cancel_restore)
                return;

            TranslatingProgressHandler.setTranslatedMessage("CopyingFilesToDestination");
            if (!canWrite(destination)) {
                Directory.Delete(temp_folder, true);
                restoreElevation(destination.FullName);
                cancel_restore = true;
            }
            if (cancel_restore)
                return;

            copyFolders(from_here, destination, true);

            if (cancel_restore)
                return;

            ProgressHandler.state = ProgressState.None;
        }
        private bool restoreElevation(string destination) {
            try {
                if (SecurityHandler.amAdmin()) {
                    TranslatingMessageHandler.SendError("UnableToCreateOutputFolderAdmin", destination);
                    return false;
                }

                MVC.Communication.Interface.InterfaceHandler.disableInterface();
                if (!TranslatingRequestHandler.Request(RequestType.Question, "UnableToCreateOutputFolderRequest", destination).cancelled) {
                    try {
                        SecurityHandler.elevation(Core.programs.restore, "\"" + FileName + "\"");
                        return true;
                    } catch (Exception e) {
                        throw new TranslateableException("RestoreProgramNotFound", e, Core.programs.restore);
                    }
                } else {
                    return false;
                }
            } finally {
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
                for (int count = 0; !canRead(FileName); count++) {
                    TranslatingProgressHandler.setTranslatedMessage("ArchiveInUseWaiting", status.TrimEnd('.'), (count + 1).ToString(), wait_max.ToString());
                    Thread.Sleep(1000);
                    if (count == wait_max - 1)
                        throw new TranslateableException("FileInUseTimeout", FileName, wait_max.ToString());
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
                throw new TranslateableException("ExeRunError", e,"7z.exe", zipper.StartInfo.Arguments);
            } finally {
                try {
                    zipper.Close();
                } catch { }
            }
        }

        private void add(string file, bool disable_versioning) {
            if (File.Exists(TempFileName))
                File.Delete(TempFileName);

            if (this.Exists) {
                File.Copy(FileName, TempFileName);
                File.SetAttributes(TempFileName, FileAttributes.Hidden);
            }

            zipper.StartInfo.Arguments = compress_switches + " \"" + TempFileName + "\" \"" + file + "\\*\"";
            run7z(false);

            File.SetAttributes(TempFileName, FileAttributes.Hidden);

            // This here's the versioning stuff. Since it's here, it's universal.
            // This handles versioning copies
            //if (!disable_versioning && Core.settings.versioning) {
            //    if (Core.settings.versioning_max != 0) {
            //        DateTime right_now = DateTime.Now;
            //        FileInfo original_file = new FileInfo(file_name);
            //        if (original_file.Exists) {
            //            if (right_now.Ticks - original_file.CreationTime.Ticks > Core.settings.versioning_ticks) {
            //                string new_path = Path.Combine(original_file.DirectoryName, Path.GetFileNameWithoutExtension(original_file.FullName) + "@" + right_now.ToString().Replace('/', '-').Replace(':', '-') + Path.GetExtension(original_file.FullName));
            //                try {
            //                    File.Move(file_name, new_path);
            //                    //File.SetCreationTime(temp_file_name,right_now);
            //                } catch (Exception ex) {
            //                    throw new TranslateableException("RevisionCopyError", ex, original_file.FullName);
            //                }
            //            } else {
            //                // This is if it hasn't been long enough for a new file
            //            }
            //        } else {
            //            // This shouldn't really be an error, as it just means that it's a new archive
            //            //MessageHandler.SendError("Where'd That Come From?","The file " + file_name + " can't be found.");
            //        }
            //    } else {
            //        //this is if the Settings for versioning are fucked up
            //    }

            //    FileInfo[] count_us = new DirectoryInfo(Path.GetDirectoryName(file_name)).GetFiles(
            //        Path.GetFileNameWithoutExtension(file_name) + "@*");


            //    if (count_us.Length > Core.settings.versioning_max) {
            //        Array.Sort(count_us, new MASGAU.Comparers.FileInfoComparer(true));
            //        for (long i = Core.settings.versioning_max; i < count_us.Length; i++) {
            //            try {
            //                (count_us[i] as FileInfo).Delete();
            //            } catch (Exception ex) {
            //                TranslatingMessageHandler.SendError("DeleteError", ex, (count_us[i] as FileInfo).Name);
            //            }
            //        }
            //    }
            //} else {
            //    // This is if versioning is disabled, or something

            //}

            if (File.Exists(FileName))
                File.Delete(FileName);

            File.Move(TempFileName, FileName);
            File.SetAttributes(FileName, FileAttributes.Normal);
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
            zipper.StartInfo.Arguments = list_switches + " \"" + FileName + "\"";
            string old_dir = zipper.StartInfo.WorkingDirectory;
            zipper.StartInfo.WorkingDirectory = "";
            zipper.Start();

            string output = zipper.StandardOutput.ReadLine();
            while (output != null) {
                return_me.Add(output);
                output = zipper.StandardOutput.ReadLine();
            }

            zipper.Close();

            zipper.StartInfo.WorkingDirectory = old_dir;

            return return_me;
        }

        private string extract_switches = "x";
        private void extract(bool send_progress) {
            prepTemp();
            zipper.StartInfo.Arguments = extract_switches + " \"" + FileName + "\"";
            run7z(send_progress);
        }
        private void extract(List<string> files, bool send_progress) {
            prepTemp();
            if (send_progress) {
                ProgressHandler.max = files.Count;
                ProgressHandler.value = 0;
            }
            foreach (string file in files) {
                zipper.StartInfo.Arguments = extract_switches + " \"" + this.FileName + "\" \"" + file + "\"";
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
            } catch (Exception e) {
                return false;
            }
        }
        public static bool canWrite(DirectoryInfo path) {
            if (!path.Exists) {
                try {
                    //createPath(path); WTF was I thinking? This shall remain here out of shame.
                    path.Create();
                } catch (Exception e) {
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
                } catch (Exception e) {
                    return false;
                }
                for (int i = 0; i <= 5; i++) {
                    try {
                        test_file.Delete();
                        return true;
                    } catch (Exception e) {
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
            } catch (Exception e) {
                return false;
            }
        }
        #endregion

    }
}
