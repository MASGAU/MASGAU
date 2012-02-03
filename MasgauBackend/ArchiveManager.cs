using System;
using System.IO;
using System.Collections;
using System.Diagnostics;

class ArchiveManager{
    private Process zipper = new Process();
    private string title = "default", output_path;
    private ArrayList saves = new ArrayList();

    public bool ready = false;


	public ArchiveManager(string new_path) {
        output_path = new_path;
        string path = Environment.GetEnvironmentVariable("PROGRAMFILES") + "\\7-Zip\\7z.exe";
        if (File.Exists(path)){
            zipper.StartInfo.FileName = path;
        } else {
            path = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") + "\\7-Zip\\7z.exe";
            if (File.Exists(path))
                zipper.StartInfo.FileName = path;
        }
        if(zipper.StartInfo.FileName==path)
            ready = true;

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
            File.Copy(copy_me.FullName, to_here + "\\" + copy_me.Name, true);
            if ((File.GetAttributes(to_here + "\\" + copy_me.Name) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                File.SetAttributes(to_here + "\\" + copy_me.Name, FileAttributes.Normal);
        }
        foreach(DirectoryInfo copy_me in read_us)
            copyDirectory(copy_me.FullName, to_here + "\\" + copy_me.Name);
    }

    public bool archiveIt(){
        Random rand = new Random();
//        string temp = Environment.GetEnvironmentVariable("TEMP") + "\\" + rand.Next(100000, 999999).ToString() + "\\";
        string temp = Environment.GetEnvironmentVariable("TEMP") + "\\MASGAUArchiving\\";

        DirectoryInfo read_me = new DirectoryInfo(temp);
        DirectoryInfo[] read_us;

        if(Directory.Exists(temp))
            Directory.Delete(temp, true);
        Directory.CreateDirectory(temp);

//        int i = 1;
        foreach (file_holder copy_me in saves)
        {
            if (copy_me.file_name == null) {
                copyDirectory(copy_me.absolute_path,temp + copy_me.relative_path);
                read_us = read_me.GetDirectories();
            } else if(copy_me.relative_path=="") {
                if (!Directory.Exists(temp))
                    Directory.CreateDirectory(temp);
                File.Copy(copy_me.absolute_path + "\\" + copy_me.file_name, temp + copy_me.file_name, true);
                if ((File.GetAttributes(temp + copy_me.file_name) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(temp + copy_me.file_name, FileAttributes.Normal);
            } else {
                if (!Directory.Exists(temp + copy_me.relative_path))
                    Directory.CreateDirectory(temp + copy_me.relative_path);
                File.Copy(copy_me.absolute_path + "\\" + copy_me.file_name, temp + copy_me.relative_path + "\\" + copy_me.file_name,true);
                if ((File.GetAttributes(temp + copy_me.relative_path + "\\" + copy_me.file_name) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(temp + copy_me.relative_path + "\\" + copy_me.file_name, FileAttributes.Normal);
                read_us = read_me.GetDirectories();
            }

            if (copy_me.owner == null)
                zipper.StartInfo.Arguments = "a -t7z -ms=off \"" + title + ".gb7\" \"" + temp + "\\*\"";
            else
                zipper.StartInfo.Arguments = "a -t7z -ms=off \"" + title + " - " + copy_me.owner + ".gb7\" \"" + temp + "\\*\"";
            runBinary();

            Directory.Delete(temp, true);

        }
        Console.WriteLine("Backed Up!");
        return true;
    }
}
