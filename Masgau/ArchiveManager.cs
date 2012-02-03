using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

class ArchiveManager{
    private Process zipper = new Process();
    private string title = "default", output_path, file_name;
    public ArrayList saves = new ArrayList();
    public bool ready = false, creating_file = false, stop = false;
    private Dictionary<string, DateTime> modified_times = new Dictionary<string, DateTime>();
    private Dictionary<string, DateTime> created_times = new Dictionary<string, DateTime>();
    private Dictionary<string, int> files_added = new Dictionary<string,int>();

	public ArchiveManager(string new_path) {
        output_path = new_path;

        string path = Environment.GetEnvironmentVariable("PROGRAMFILES") + "\\7-Zip\\7z.exe";
        if (File.Exists(path)){
            zipper.StartInfo.FileName = path;
            ready = true;
        } 


        //if(zipper.StartInfo.FileName = null) {
        //    path = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") + "\\7-Zip\\7z.dll";
        //    if (File.Exists(path))
        //        zipper.StartInfo.FileName = path;
        //}



        zipper.StartInfo.UseShellExecute = false;
        zipper.StartInfo.CreateNoWindow = true;
        zipper.StartInfo.RedirectStandardOutput = true;
        zipper.StartInfo.RedirectStandardError = true;
        zipper.StartInfo.WorkingDirectory = output_path;
    }
    public void clearArchive() {
        saves = new ArrayList();
        title = "default";
    }

    public void setName(string set_me) {
        title = set_me;
    }
    public void addSave(ArrayList add_me) {
        saves.AddRange(add_me);
    }

    private void runBinary() {
		using(Process p = Process.Start(zipper.StartInfo)) {
            string output = p.StandardOutput.ReadToEnd();
            string errors = p.StandardError.ReadToEnd();
            Console.WriteLine(output);
            Console.WriteLine(errors);
            Console.WriteLine(zipper.StartInfo.Arguments);
            p.WaitForExit();
        }
    }

    private void copyDirectory(string from_here, string to_here) {
        if (!Directory.Exists(to_here))
            Directory.CreateDirectory(to_here);
        DirectoryInfo read_me = new DirectoryInfo(from_here);
        DirectoryInfo[] read_us = read_me.GetDirectories();
        FileInfo[] copy_us = read_me.GetFiles();
        foreach(FileInfo copy_me in copy_us) {
            File.Copy(copy_me.FullName, Path.Combine(to_here,copy_me.Name), true);
            if ((File.GetAttributes(Path.Combine(to_here,copy_me.Name)) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                File.SetAttributes(Path.Combine(to_here,copy_me.Name), FileAttributes.Normal);
        }
        foreach(DirectoryInfo copy_me in read_us)
            copyDirectory(copy_me.FullName, Path.Combine(to_here,copy_me.Name));
    }

    public bool archiveIt(ProgressBar progress, bool ignore_date_check, bool suppress_messages, long versioning_ticks, int versioning_max){
        if(ready) {

            string temp = Path.Combine(Path.GetTempPath(),"MASGAUArchiving");
            string from_here, to_here, in_here;
            String[] source = new String[1];
            DirectoryInfo read_me = new DirectoryInfo(temp);
            DateTime right_now = DateTime.Now;

            Dictionary<string, string> add_us = new Dictionary<string, string>();

            if(Directory.Exists(temp)) {
                try {
                    Directory.Delete(temp, true);
                } catch {
                    MessageBox.Show("An error occured while trying to delete " + temp,"Every Day Is?");
                    stop = true;
                }
            }
            if(progress!=null) {
                progress.Maximum = saves.Count;
                progress.Minimum = 0;
                if (saves.Count>=2)
                    progress.Value = 2;
                else
                    progress.Value = saves.Count;
            }                
            string full_path;
            foreach (file_holder copy_me in saves)
            {
                if(progress!=null)
                    progress.PerformStep();
                if (stop)
                    break;
                try {
                    Directory.CreateDirectory(temp);
                } catch {
                    MessageBox.Show("An error occured while trying to create " + temp, "Look Out!");
                    stop = true;
                }
                if (copy_me.owner == null)
                    file_name = title + ".gb7";
                else
                    file_name = title + "«" + copy_me.owner + ".gb7";

                full_path = Path.Combine(output_path,file_name);
                if(!stop) {
                    if(File.Exists(full_path)) {
                        if(!modified_times.ContainsKey(full_path)) {
                            modified_times.Add(full_path,File.GetLastWriteTime(full_path));
                        } 
                        if(!created_times.ContainsKey(full_path)) {
                            created_times.Add(full_path,File.GetCreationTime(full_path));
                        }

                        if(!File.Exists(Path.Combine(output_path,file_name + "BACKUP"))) {
                            try {
                                File.Copy(full_path,full_path + "BACKUP");
                            } catch {
                                if (!suppress_messages)
                                    MessageBox.Show("An error occured while trying to make a copy of " + file_name);
                                stop = true;
                            }
                        }
                    } else {
                        if(!modified_times.ContainsKey(full_path)) {
                            // A red letter date in the history of science
                            modified_times.Add(full_path, new DateTime(1955, 11, 5));
                        }
                    }
                }

                creating_file = !File.Exists(Path.Combine(output_path,file_name));
                if(!stop) {
                    if (creating_file||canWrite(Path.Combine(output_path, file_name))) {

                        if (copy_me.absolute_path == "") {
                            from_here = copy_me.absolute_root;
                            in_here = copy_me.file_name;
                        } else {
                            from_here = Path.Combine(copy_me.absolute_root, copy_me.absolute_path);
                            in_here = Path.Combine(copy_me.absolute_path, copy_me.file_name);
                        }
                        from_here = Path.Combine(from_here,copy_me.file_name);
                        to_here = Path.Combine(temp, in_here);

                        if(ignore_date_check||DateTime.Compare(modified_times[full_path],File.GetLastWriteTime(from_here))<0) {
                            if(files_added.ContainsKey(full_path)) {
                                files_added[full_path]++;
                            } else {
                                files_added.Add(full_path,1);
                            }
                            try {
                                if (!Directory.Exists(Path.GetDirectoryName(to_here)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(to_here));
                            } catch {
                                MessageBox.Show("An error occured when trying to create " + Path.GetDirectoryName(to_here),"I Am The High Commander");
                                stop = true;
                            }
                            try {
                                File.Copy(from_here, to_here, true);
                            } catch {
                                MessageBox.Show("An error occured while trying to copy " + from_here, "Eccentric!");
                                stop = true;
                            }
                            try {
                                if ((File.GetAttributes(to_here) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                    File.SetAttributes(to_here, FileAttributes.Normal);
                            } catch {
                                MessageBox.Show("An error occcured while trying to change the permissions on " + to_here);
                                stop = true;
                            }

                            zipper.StartInfo.Arguments = "a -t7z -ms=off \"" + file_name + "\" \"" + temp + "\\*\"";
                            try {
                                runBinary();
                            } catch {
                                MessageBox.Show("An error occured while trying to run 7-zip with the arguments:\n" + zipper.StartInfo.Arguments);
                            }
                        }

                    } else {
                        MessageBox.Show("The file " + file_name + " is not writable","Gadzooks!");
                    }
                }
                creating_file = !File.Exists(Path.Combine(output_path, file_name));

                Directory.Delete(temp, true);
            }
            if(stop) {
                foreach(FileInfo copy_me in new DirectoryInfo(output_path).GetFiles("*BACKUP")) {
                    try {
                        if(File.Exists(copy_me.FullName.Substring(0,copy_me.FullName.Length-6)))
                            File.Delete(copy_me.FullName.Substring(0,copy_me.FullName.Length-6));
                        copy_me.MoveTo(copy_me.FullName.Substring(0,copy_me.FullName.Length-6));
                    } catch {
                        MessageBox.Show("An error occured while trying to restore " + copy_me.Name);
                    }
                }
            } else {
                if(versioning_ticks!=0L&&versioning_max!=0) {
                    foreach(FileInfo delete_me in new DirectoryInfo(output_path).GetFiles("*BACKUP")) {
                        string original_file = delete_me.FullName.Substring(0,delete_me.FullName.Length-6);
                        if(File.Exists(original_file)&&created_times.ContainsKey(original_file)&&files_added.ContainsKey(original_file)) {
                            if(right_now.Ticks-created_times[original_file].Ticks>versioning_ticks) {
                                string new_path = Path.Combine(Path.GetDirectoryName(original_file),Path.GetFileNameWithoutExtension(original_file) + "@" + right_now.ToString().Replace('/','-').Replace(':','-') + Path.GetExtension(original_file));
                                try {
                                    delete_me.MoveTo(new_path);
                                    File.SetCreationTime(original_file,right_now);
                                } catch {
                                    if (!suppress_messages)
                                        MessageBox.Show("An error occured while trying to make a revision copy of " + file_name);
                                    stop = true;
                                }
                            } else {
                                try {    
                                    delete_me.Delete();
                                } catch {
                                    MessageBox.Show("An error occured while trying to delete " + delete_me.Name);
                                }
                            }
                        } else {
                            try {    
                                delete_me.Delete();
                            } catch {
                                MessageBox.Show("An error occured while trying to delete " + delete_me.Name);
                            }
                        }
                        FileInfo[] count_us = new DirectoryInfo(output_path).GetFiles(Path.GetFileNameWithoutExtension(original_file) + "@*");
                        if(count_us.Length>=versioning_max) {
                            for(int i = 0;i<=(count_us.Length-versioning_max);i++) {
                                try {
                                    ((FileInfo)count_us[i]).Delete();
                                } catch {
                                    MessageBox.Show("An error occured while trying to delete " + delete_me.Name);
                                }
                            }
                        }
                    }
                } else {
                    foreach(FileInfo delete_me in new DirectoryInfo(output_path).GetFiles("*BACKUP")) {
                        try {    
                            delete_me.Delete();
                        } catch {
                            MessageBox.Show("An error occured while trying to delete " + delete_me.Name);
                        }
                    }
                }
            }
            Console.WriteLine("Backed Up!");
            return true;
        }
        else {
            return false;
        }
    }
    public void cleanUp(string backup_path) {
        if(backup_path!=null) {
            foreach(FileInfo delete_me in new DirectoryInfo(backup_path).GetFiles("*.tmp*")) {
                try {
                    delete_me.Delete();
                } catch {
                    MessageBox.Show("An error occured while trying to delete " + delete_me.Name);
                }
            }
            if(creating_file) {
                try {
                    File.Delete(Path.Combine(output_path,file_name));
                } catch {
                    MessageBox.Show("An error occured while trying to delete " + file_name);
                }

            }
        }
        modified_times = new Dictionary<string, DateTime>();
        created_times = new Dictionary<string, DateTime>();
    }

    private bool canWrite(string file) {
        try {
            FileStream close_me = File.Open(file,FileMode.Append);
            close_me.Close();
            return true;
        } catch {
            return false;
        }
    }
}
