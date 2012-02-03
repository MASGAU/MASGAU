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
        private userSelector system_user_selector = new userSelector();
        private userSelector steam_user_selector = new userSelector();

        public RestoreHandler(string from_here, string back_this)
        {
            temp = Environment.GetEnvironmentVariable("TEMP") + "\\MASGAURestoring\\";
            backup_path = from_here;
            detectBackups(back_this);
        }

        public void clearSystemUsers() {
            system_user_selector.userSelectorCombo.Items.Clear();
        }

        public void addSystemUser(string add_me) {
            system_user_selector.userSelectorCombo.Items.Add(add_me);
        }

        public void clearSteamUsers() {
            steam_user_selector.userSelectorCombo.Items.Clear();
        }

        public void addSteamUser(string add_me) {
            steam_user_selector.userSelectorCombo.Items.Add(add_me);
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
                        hold_me = add_me.file_name.Replace(".gb7","").Split('-');
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

        public string extractBackup(string restore_me) {
            int i = findBackup(restore_me);
            if (Directory.Exists(temp))
                Directory.Delete(temp,true);
            Directory.CreateDirectory(temp);

            Process unzipper = new Process();
            unzipper.StartInfo.UseShellExecute = false;
            unzipper.StartInfo.RedirectStandardOutput = true;
            unzipper.StartInfo.RedirectStandardError = true;
            unzipper.StartInfo.WorkingDirectory = temp;
            unzipper.StartInfo.CreateNoWindow = true;
            unzipper.StartInfo.FileName = null;

            string path = Environment.GetEnvironmentVariable("PROGRAMFILES") + "\\7-Zip\\7z.exe";
            if (File.Exists(path)){
                unzipper.StartInfo.FileName = path;
            } else {
                path = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") + "\\7-Zip\\7z.exe";
                if (File.Exists(path))
                    unzipper.StartInfo.FileName = path;
            }
            if(unzipper.StartInfo.FileName==path) {
                unzipper.StartInfo.Arguments = "x \"" + backup_path + "\\" + restore_me + "\"";
                Console.WriteLine(unzipper.StartInfo.Arguments);
                unzipper.Start();
                Console.WriteLine(unzipper.StandardOutput.ReadToEnd());
                unzipper.WaitForExit();
                unzipper.Close();
                return "Extraction Complete";
            } else {
                Directory.Delete(temp, true);
                unzipper.Close();
                return "Zip Not Found";
            }
        }

        public string restoreBackup(int i, GameData game_data) {
            if(i!=-1) {
                DirectoryInfo from_here = new DirectoryInfo(temp);
                backup_holder data = (backup_holder)backups[i];
                DirectoryInfo to_here = new DirectoryInfo("here");
                if(game_data!=null&&game_data.root_detected) {
                    file_holder goes_here = game_data.getPath(paths, steam_path);
                    if(goes_here.relative_path==null)
                        return "Game Not Detected";
                    else if(goes_here.relative_root.Contains("%STEAMUSER%")) {
                        if(steam_path!=null) {
                            steam_user_selector.setCombo(data.owner);
                            if(steam_user_selector.ShowDialog().ToString()=="Cancel")
                                return "Cancelled";
                            else
                                to_here = new DirectoryInfo(goes_here.relative_root.Replace("%STEAMUSER%",steam_user_selector.userSelectorCombo.SelectedItem.ToString()));
                        } else
                            return "Steam Not Installed";
                    } else if(goes_here.relative_root.Contains("%USERNAME%")) {
                        system_user_selector.setCombo(data.owner);
                        if (system_user_selector.ShowDialog().ToString() == "Cancel")
                            return "Cancelled";
                        else
                            to_here = new DirectoryInfo(goes_here.relative_root.Replace("%USERNAME%", system_user_selector.userSelectorCombo.SelectedItem.ToString()));
                    } else
                        to_here = new DirectoryInfo(goes_here.absolute_root);
                    copyFolders(from_here, to_here);
                } else
                    return "Game Not Detected";
            } else
                return "Backup Not Found";
            Directory.Delete(temp, true);
            return "Success";
        }

        private void createPath(DirectoryInfo create_me) {
        }

        private void copyFolders(DirectoryInfo from_here, DirectoryInfo to_here) {
            DirectoryInfo now_here;
            if(!to_here.Exists)
                Directory.CreateDirectory(to_here.FullName);

            foreach(FileInfo copy_me in from_here.GetFiles()) {
                Console.WriteLine("Copying file: " + copy_me.Name);
                File.Copy(copy_me.FullName,to_here.FullName + "\\" + copy_me.Name,true);
            }

            foreach(DirectoryInfo check_me in from_here.GetDirectories()) {
                now_here = new DirectoryInfo(to_here.FullName + "\\" + check_me.Name);
                copyFolders(check_me, now_here);
            }
        }

    }
}
