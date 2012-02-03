using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using System.Security.AccessControl;
using wyDay.Controls;

namespace Masgau
{
    public class RestoreHandler
    {
        public Dictionary<string, backup_holder> backups = new Dictionary<string,backup_holder>();
        //public ArrayList backups;
        public string backup_path = null, steam_path = null;
        public PathHandler paths;
        private string temp;
        private bool all_users_mode = false, ready = false;
        private SecurityHandler red_shirt = new SecurityHandler();
        Process unzipper = new Process();
        invokes invokes = new invokes();

        public RestoreHandler(string from_here, string back_this)
        {
            string[] args = Environment.GetCommandLineArgs();
            for(int i = 0;i<args.Length;i++) {
                if(args[i]=="/allusers") {
                    all_users_mode = true;
                }
            }

            RegistryManager zip_path = new RegistryManager("SOFTWARE\\7-Zip");
            string path = zip_path.getValue("Path") + Path.DirectorySeparatorChar + "7z.exe";
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
            if(ready&&backup_path!=null&&Directory.Exists(backup_path)) {
                if (Directory.Exists(temp))
                    Directory.Delete(temp,true);
                Directory.CreateDirectory(temp);
                unzipper.StartInfo.UseShellExecute = false;
                unzipper.StartInfo.RedirectStandardOutput = true;
                unzipper.StartInfo.RedirectStandardError = true;
                unzipper.StartInfo.WorkingDirectory = temp;
                unzipper.StartInfo.CreateNoWindow = true;

                backup_holder add_me;
                string[] hold_me;
                backups = new Dictionary<string,backup_holder>();
                FileInfo[] read_us = new DirectoryInfo(backup_path).GetFiles("*.gb7");
                if(read_us.Length>0) {
                    foreach(FileInfo read_me in read_us) {
                        add_me.file_date = read_me.LastWriteTime.ToString();
                        add_me.file_name = read_me.Name;
                        add_me.owner = null;
                        add_me.game_name = null;
                        unzipper.StartInfo.Arguments = "x \"" + read_me.FullName + "\" \"masgau.xml\"";
                        unzipper.Start();
                        Console.WriteLine(unzipper.StandardOutput.ReadLine());
                        Console.WriteLine(unzipper.StandardError.ReadLine());
                        unzipper.WaitForExit();
                        if(File.Exists(Path.Combine(temp, "masgau.xml"))) {
                            XmlReaderSettings xml_settings = new XmlReaderSettings();
                            xml_settings.ConformanceLevel = ConformanceLevel.Document;
                            xml_settings.IgnoreComments = true;
                            xml_settings.IgnoreWhitespace = true;
                            XmlReader load_me = XmlReader.Create(Path.Combine(temp, "masgau.xml"),xml_settings);
                            while (load_me.Read()) {
    			                if(load_me.NodeType==XmlNodeType.Element) {
				                    switch (load_me.Name) {
                                        case "game":
                                            while(load_me.MoveToNextAttribute()) {
                                                switch(load_me.Name) {
                                                    case "name":
                                                        add_me.game_name = load_me.Value;
                                                        break;
                                                }
                                            }
                                            break;
                                        case "owner":
                                            while(load_me.MoveToNextAttribute()) {
                                                switch(load_me.Name) {
                                                    case "name":
                                                        add_me.owner = load_me.Value;
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            load_me.Close();
                            File.Delete(Path.Combine(temp, "masgau.xml"));
                        } else {
                            hold_me = add_me.file_name.Replace(".gb7", "").Split('@');
                            hold_me = hold_me[0].Split('«');
                            add_me.game_name = hold_me[0].Trim(); ;
                            if(hold_me.Length>1) {
                                add_me.owner = hold_me[1].Trim();
                            } else {
                                add_me.owner = null;
                            }
                        }
                            
                        if(add_me.game_name!=null)
                            backups.Add(add_me.file_name,add_me);

                    }
                } 
            }
        }

        public int extractBackup(string restore_me, Windows7ProgressBar progress) {

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
                invokes.setProgressBarMin(progress,0);
                invokes.setProgressBarValue(progress,1);
                invokes.setProgressBarMax(progress,Int32.Parse(hold_this[0]));
                unzipper.Close();

                unzipper.StartInfo.Arguments = "x \"" + restore_me + "\"";
                Console.WriteLine(unzipper.StartInfo.Arguments);
                unzipper.Start();
                output = unzipper.StandardOutput.ReadLine();
                while(output != null){
                    if(output.StartsWith("Extracting"))
                        invokes.performStep(progress);
                    Console.WriteLine(output);
                    output = unzipper.StandardOutput.ReadLine();
                }
                unzipper.Close();
                if(File.Exists(Path.Combine(temp,"masgau.xml")))
                    File.Delete(Path.Combine(temp,"masgau.xml"));
                return progress.Value;
            } else {
                MessageBox.Show("Can't read " + Path.GetFileName(restore_me) + ".","I'm Illiterate!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return -1;
            }
        }


        public void restoreBackup(string put_here, string owner, Windows7ProgressBar progress, IWin32Window parent, string archive_name) {
            DirectoryInfo from_here = new DirectoryInfo(temp);
            DirectoryInfo to_here = new DirectoryInfo(put_here);


			if(!to_here.Exists) {
				try {
					to_here.Create();
					DirectorySecurity everyone = to_here.GetAccessControl();
					string eveyrones_name = @"Everyone";
					everyone.AddAccessRule(new FileSystemAccessRule(eveyrones_name,FileSystemRights.FullControl,InheritanceFlags.ContainerInherit|InheritanceFlags.ObjectInherit,PropagationFlags.None,AccessControlType.Allow));
					to_here.SetAccessControl(everyone);
				} catch {
				    if(!red_shirt.amAdmin()) {
				        if (invokes.showConfirmDialog((Form)parent,"Cheese and Crackers","Unable to create output folder:" + Environment.NewLine + to_here + Environment.NewLine + "Would you like to try again as admin?") != DialogResult.No){
				            ProcessStartInfo superMode = new ProcessStartInfo();
				            superMode.UseShellExecute = true;
				            superMode.WorkingDirectory = Application.ExecutablePath;
				            superMode.FileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"MasgauTask.exe");
				            superMode.Arguments = archive_name;
				            superMode.Verb = "runas";
				            try {
				                Process p = Process.Start(superMode);
				                p.WaitForExit();
				            }
				            catch{
                                invokes.showMessageBox((Form)parent,"People people people get up get up",superMode.FileName + "can't be found.\nYou probably need to reinstall.",MessageBoxButtons.OK,MessageBoxIcon.Error);
				            }
				            return;
				        }
				    } else {
                        invokes.showMessageBox((Form)parent,"And that's how it is","Unable to create output folder:" + Environment.NewLine + to_here,MessageBoxButtons.OK,MessageBoxIcon.Error);
				    }
				    return;
				}
			}

            if(canWrite(to_here.FullName)) {
                if(!copyFolders(from_here, to_here, progress, parent)){
					Directory.Delete(temp, true);
                    if(!red_shirt.amAdmin()) {
                        if (invokes.showConfirmDialog((Form)parent, "Cheese and Crackers","Writing to " + to_here + " is denied.\nWould you like to try running as admin?") != DialogResult.No){
							ProcessStartInfo superMode = new ProcessStartInfo();
							superMode.UseShellExecute = true;
							superMode.WorkingDirectory = Application.ExecutablePath;
						    superMode.FileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"MasgauTask.exe");
							superMode.Arguments = archive_name;
							superMode.Verb = "runas";
							try {
								Process p = Process.Start(superMode);
								p.WaitForExit();
							}
							catch{
                                invokes.showMessageBox((Form)parent,"Everybody everybody stand up stand up",superMode.FileName + "can't be found.\nYou probably need to reinstall.",MessageBoxButtons.OK,MessageBoxIcon.Error);
							}
                            return;
                        }
                    } else {
                        invokes.showMessageBox((Form)parent,"Cheese and Crackers","Writing to " + to_here + " is denied.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            } else {
				Directory.Delete(temp, true);
                if(!red_shirt.amAdmin()) {
                    if (invokes.showConfirmDialog((Form)parent,"Cheese and Crackers","Writing to " + to_here + " is denied.\nWould you like to try running as admin?") != DialogResult.No){
						ProcessStartInfo superMode = new ProcessStartInfo();
						superMode.UseShellExecute = true;
							superMode.WorkingDirectory = Application.ExecutablePath;
						superMode.FileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"MasgauTask.exe");
						superMode.Arguments = archive_name;
						superMode.Verb = "runas";
						try {
							Process p = Process.Start(superMode);
							p.WaitForExit();
						}
						catch{
                            invokes.showMessageBox((Form)parent,"Everybody everybody testify",superMode.FileName + " can't be found.\nYou probably need to reinstall.",MessageBoxButtons.OK,MessageBoxIcon.Error);
						}
                    }
                } else {
                    invokes.showMessageBox((Form)parent,"Crackers and Cheese","Writing to " + to_here + " is denied.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

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

        private bool copyFolders(DirectoryInfo from_here, DirectoryInfo to_here, Windows7ProgressBar progress, IWin32Window parent) {
            DirectoryInfo now_here;
            if(!to_here.Exists){
	            try {
		            Directory.CreateDirectory(to_here.FullName);
				} catch {
                    invokes.showMessageBox((Form)parent,"My genetics are repressed","Error while creating " + to_here.FullName,MessageBoxButtons.OK,MessageBoxIcon.Error);
					return false;
				}
	            try {
				} catch {
                    invokes.showMessageBox((Form)parent,"My genetics are repressed","Error while settings permissions on " + to_here.FullName,MessageBoxButtons.OK,MessageBoxIcon.Error);
					return false;
				}
			}
            foreach(FileInfo copy_me in from_here.GetFiles()) {
				try {
					Console.WriteLine("Copying file: " + copy_me.Name);
                    invokes.performStep(progress);
					File.Copy(copy_me.FullName,Path.Combine(to_here.FullName,copy_me.Name),true);
				} catch {
                    invokes.showMessageBox((Form)parent,"We started this Op'ra","Error while copying " + copy_me.FullName + "\nto " + to_here.FullName,MessageBoxButtons.OK,MessageBoxIcon.Error);
					return false;
				}
            }
            foreach(DirectoryInfo check_me in from_here.GetDirectories()) {
                now_here = new DirectoryInfo(Path.Combine(to_here.FullName,check_me.Name));
                if(!copyFolders(check_me, now_here,progress,parent))
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
