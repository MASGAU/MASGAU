using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Masgau
{
    public class RestoreHandler
    {
        public ArrayList backups;
        public string backup_path = null, steam_path = null;
        public PathHandler paths;
        private string temp;
        private bool all_users_mode = false, ready = false;
        private SecurityHandler red_shirt = new SecurityHandler();
        Process unzipper = new Process();

        public RestoreHandler(string from_here, string back_this)
        {
            string[] args = Environment.GetCommandLineArgs();
            for(int i = 0;i<args.Length;i++) {
                if(args[i]=="/allusers") {
                    all_users_mode = true;
                }
            }

            string path = Environment.GetEnvironmentVariable("PROGRAMFILES") + "\\7-Zip\\7z.exe";
            if (File.Exists(path))
            {
                unzipper.StartInfo.FileName = path;
                ready = true;
            } 

            
            temp = Path.Combine(Environment.GetEnvironmentVariable("TEMP"),"MASGAURestoring");
            backup_path = from_here;
            detectBackups(back_this);
        }

        public void detectBackups(string back_this) {
            if(backup_path!=null&&Directory.Exists(backup_path)) {
                backup_holder add_me;
                string[] hold_me;
                backups = new ArrayList();
                FileInfo[] read_us = new DirectoryInfo(backup_path).GetFiles("*.gb7");
                if(read_us.Length>0) {
                    foreach(FileInfo read_me in read_us) {
                        add_me.file_date = read_me.LastWriteTime.ToString();
                        add_me.file_name = read_me.Name;
                        hold_me = add_me.file_name.Replace(".gb7", "").Split('@');
                        hold_me = hold_me[0].Split('«');
                        add_me.game_name = hold_me[0].Trim(); ;
                        if(hold_me.Length>1) {
                            add_me.owner = hold_me[1].Trim();
                        } else {
                            add_me.owner = null;
                        }
                        backups.Add(add_me);
                    }
                } 
            }
        }

        public int findBackup(string find_me) {
            for(int i = 0;i<backups.Count;i++) {
                if(((backup_holder)backups[i]).file_name==find_me)
                    return i;
            }
            return -1;
        }

        public int extractBackup(string restore_me, ProgressBar progress) {

            if(canRead(restore_me)) {
                if (Directory.Exists(temp))
                    Directory.Delete(temp,true);
                Directory.CreateDirectory(temp);

                unzipper.StartInfo.UseShellExecute = false;
                unzipper.StartInfo.RedirectStandardOutput = true;
                unzipper.StartInfo.RedirectStandardError = true;
                unzipper.StartInfo.WorkingDirectory = temp;
                unzipper.StartInfo.CreateNoWindow = true;
                unzipper.StartInfo.Arguments = "l \"" + restore_me + "\"";
                unzipper.Start();

                string output = unzipper.StandardOutput.ReadLine();
                string last_line = null;
                while(output!=null) {
                    Console.WriteLine(output);
                    last_line = output;
                    output = unzipper.StandardOutput.ReadLine();
                }
                string[] hold_this = last_line.Substring(53).Split(' ');
                progress.Minimum = 0;
                progress.Value = 1;
                progress.Maximum = Int32.Parse(hold_this[0]);
                unzipper.Close();

                unzipper.StartInfo.Arguments = "x \"" + restore_me + "\"";
                Console.WriteLine(unzipper.StartInfo.Arguments);
                unzipper.Start();
                output = unzipper.StandardOutput.ReadLine();
                while(output != null){
                    if(output.StartsWith("Extracting"))
                        progress.PerformStep();
                    Console.WriteLine(output);
                    output = unzipper.StandardOutput.ReadLine();
                }
                unzipper.Close();
                return progress.Value;
            } else {
                MessageBox.Show("Can't read " + Path.GetFileName(restore_me) + ".","I'm Illiterate!");
                return -1;
            }
        }

        public void restoreBackup(string put_here, string owner, ProgressBar progress) {
            DirectoryInfo from_here = new DirectoryInfo(temp);
            DirectoryInfo to_here = new DirectoryInfo(put_here);

            if(canWrite(to_here.FullName)) {
                if(!copyFolders(from_here, to_here, progress)){
                    if(!red_shirt.amAdmin()) {
                        if (MessageBox.Show("Writing to " + to_here + " is denied.\nWould you like to try running as admin?", "Cheese and Crackers", MessageBoxButtons.YesNo) != DialogResult.No){
                            red_shirt.elevation(null);
                            return;
                        }
                    } else {
                        MessageBox.Show("Writing to " + to_here + " is denied.", "Cheese and Crackers");
                    }
                }
            } else {
                if(!red_shirt.amAdmin()) {
                    if (MessageBox.Show("Writing to " + to_here + " is denied.\nWould you like to try running as admin?", "Cheese and Crackers", MessageBoxButtons.YesNo) != DialogResult.No){
                        red_shirt.elevation(null);
                    }
                } else {
                    MessageBox.Show("Writing to " + to_here + " is denied.", "Cheese and Crackers");
                }
            }

            Directory.Delete(temp, true);
        }

        //private bool createPath(string create_me) {
        //    string[] split = create_me.Split(Path.DirectorySeparatorChar);
        //    string path = split[0] + Path.DirectorySeparatorChar;
        //    for(int i = 1;i<split.Length;i++) {
        //        path = Path.Combine(path,split[i]);
        //        if(!Directory.Exists(path)) {
        //            try {
        //                Directory.CreateDirectory(path);
        //            } catch {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        private bool copyFolders(DirectoryInfo from_here, DirectoryInfo to_here, ProgressBar progress) {
            DirectoryInfo now_here;
            try {
                if(!to_here.Exists)
                    Directory.CreateDirectory(to_here.FullName);
                foreach(FileInfo copy_me in from_here.GetFiles()) {
                    Console.WriteLine("Copying file: " + copy_me.Name);
                    progress.PerformStep();
                    File.Copy(copy_me.FullName,Path.Combine(to_here.FullName,copy_me.Name),true);
                }
            } catch {
                return false;
            }
            foreach(DirectoryInfo check_me in from_here.GetDirectories()) {
                now_here = new DirectoryInfo(Path.Combine(to_here.FullName,check_me.Name));
                if(!copyFolders(check_me, now_here,progress))
                    return false;
            }
            return true;
        }

        private bool canRead(string file) {
            try {
                FileStream close_me = File.Open(file,FileMode.Open);
                close_me.Close();
                return true;
            } catch {
                return false;
            }
        }

        public bool canWrite(string path) {
            if(!Directory.Exists(path)) {
                try {
                    //createPath(path); WTF was I thinking? This shall remain here out of shame.
                    Directory.CreateDirectory(path);
                } catch {
                    return false;
                }
            }

            if (Directory.Exists(path)) {
                string file_name = Path.GetRandomFileName();
                FileInfo test_file = new FileInfo(Path.Combine(path,file_name));
                try {
                    FileStream delete_me = test_file.Create();
                    delete_me.Close();
                    test_file.Delete();
                    return true;
                } catch {
                    return false;
                }
            }
            return false;
        }
    }
}
