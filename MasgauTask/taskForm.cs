using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Masgau
{
    public partial class taskForm : Form
    {
        private string[] args = Environment.GetCommandLineArgs();
        private zipLinker zipLink = new zipLinker();
        private string config_file = null, selected_game = null, backup_owner = null;
        private Thread restoreThread, backupThread;
        private SettingsManager settings = null;
        private RestoreHandler restore;
        private ArchiveManager back_up = null;
        private ArrayList restore_these = null;
		private ArrayList back_these_up = null;
		private ArrayList only_these_files = null;
        bool stop = false;
        bool all_users_mode = false;
		private string output_override = null;

        public taskForm(string do_this, ArrayList to_these, SettingsManager using_this, bool as_this)
        {
            switch (do_this) {
                case "backup":
                    if(to_these!=null) {
                        back_these_up = new ArrayList();
                        back_these_up.AddRange(to_these);
                    }
                    break;
                case "restore":
                    restore_these = new ArrayList();
                    restore_these.AddRange(to_these);
                    break;
            }
            settings = using_this;
            all_users_mode = as_this;
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public taskForm(string this_game, SettingsManager using_this, bool as_this, ArrayList only_these, string to_here)
        {
            if(this_game!=null) {
                back_these_up = new ArrayList();
                back_these_up.Add(this_game);
            }
			if(only_these!=null) {
				only_these_files = new ArrayList();
				only_these_files.AddRange(only_these);
			}
			output_override = to_here;
            settings = using_this;
            all_users_mode = as_this;
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public taskForm()
        {
            for(int i = 0;i<args.Length;i++) {
                switch(args[i]) {
                    case "/backup":
                        while (i+1<args.Length&& !args[i + 1].StartsWith("/")) {
							i++;
                            if(back_these_up==null)
                                back_these_up = new ArrayList();
							back_these_up.Add(args[i].Trim('\"'));
						}
                        break;
                    case "/restore":
                        if (i+1<args.Length&& !args[i + 1].StartsWith("/")) {
							i++;
                            if(restore_these==null)
                                restore_these = new ArrayList();
                            restore_these.Add(args[i].Trim('\"'));
						}
                        break;
                    case "/config":
                        if (i+1<args.Length&& !args[i + 1].StartsWith("/")) {
							i++;
                            config_file = args[i].Trim('\"');
						}
                        break;
                    case "/allusers":
                        all_users_mode = true;
                        break;
					default:
                        if(!args[i].StartsWith("/")) {
						    if(args[i].Trim('\"').EndsWith(".gb7")) {
                                if(restore_these==null)
                                    restore_these = new ArrayList();
							    restore_these.Add(args[i].Trim('\"'));
						    } else if(!args[i].Trim('\"').EndsWith(".exe")) {
                                if (back_these_up == null)
                                    back_these_up = new ArrayList();
                                back_these_up.Add(args[i].Trim('\"'));
						    }
                        }
						break;
                }
            }

            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(30,"MASGAU is going to do something...","What will it be?",ToolTipIcon.Info);
            if(restore_these!=null) {
                restoreThread = new Thread(restoreBackup);
                restoreThread.Start();

            } else {
                if(back_these_up==null||back_these_up.Count>1) {
                    currentProgress.Visible = true;
                }
                backupThread = new Thread(createBackup);
                backupThread.Start();
            }

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            groupBox1.Text = "Cancelling...";
            cancelButton.Enabled = false;
            stop = true;
            if(back_up!=null) {
                back_up.stop = true;
            }
        }


        private void restoreBackup() {
            groupBox1.Text = "Detecting game...";
            string restore_me = (string)restore_these[0];
            string[] hold_me = restore_me.Replace(".gb7", "").Split(Path.DirectorySeparatorChar);
            hold_me = hold_me[hold_me.Length - 1].Split('@');
            hold_me = hold_me[0].Split('«');

            selected_game = hold_me[0].Trim();
            backup_owner = null;
            if (hold_me.Length > 1)
                backup_owner = hold_me[1].Trim();

            ArrayList restore_this = new ArrayList();
            restore_this.Add(selected_game);

            if(settings==null)
                settings = new SettingsManager(config_file, restore_this, null);


            FileInfo the_backup = null;
            if (settings.backup_path!=null&&File.Exists(Path.Combine(settings.backup_path,restore_me))) {
                the_backup = new FileInfo(Path.Combine(settings.backup_path,restore_me));
                restore = new RestoreHandler(the_backup.DirectoryName,the_backup.Name);
            }
            else if (File.Exists(restore_me)) {
                the_backup = new FileInfo(restore_me);
                restore = new RestoreHandler(the_backup.DirectoryName,the_backup.Name);
            } else {
                MessageBox.Show(this,"Specified backup doesn't exist");
                this.Close();
                return;
            }

            restore.steam_path = settings.steam.path;
            restore.paths = settings.paths;

            GameData game_data = null;
            if (settings.games.ContainsKey(selected_game)) {
                game_data = settings.games[selected_game];
            } else {
                MessageBox.Show(this, "The game profile associated with this backup could not be found.", "Do You Bite Your Thumb At Me, Sir?");
                stop = true;
            }

            if(!stop&&game_data.detected_roots.Count<1) {
                if(game_data.platform=="Windows") {
                    MessageBox.Show(this,"The game associated with this backup could not be detected.\nMake sure the game is installed and try making a save with it.","Never Was There A Tale Of More Woah");
                    stop = true;
                }
            }
              
            string output_path = null;
            if(!stop&&game_data.platform!="Windows") {
                driveSelector put_it_here = new driveSelector();
                if(game_data.psp_ids.Count!=0) {
                    if(settings.playstation.psp_saves!=null)
                        put_it_here.setDrive(Path.GetPathRoot(settings.playstation.psp_saves));
                    if(put_it_here.driveCombo.Items.Count>0) {
                        if(put_it_here.ShowDialog(this)!=DialogResult.Cancel) {
                            output_path = ((string)put_it_here.driveCombo.SelectedItem).Substring(0, 3);
                            Console.WriteLine(output_path);
                        } else {
                            stop = true;
                        }
                    } else {
                        MessageBox.Show(this, "Could not detect any PSP compatible drives.","Not So Portable Now, Eh?");
                        stop = true;
                    }
                } else if(game_data.ps2_ids.Count!=0) {
                    if(settings.playstation.ps3_export!=null)
                        put_it_here.setDrive(Path.GetPathRoot(settings.playstation.ps3_export));
                    if(put_it_here.driveCombo.Items.Count>0) {
                        if(put_it_here.ShowDialog(this)!=DialogResult.Cancel) {
                            output_path = ((string)put_it_here.driveCombo.SelectedItem).Substring(0, 3);
                            Console.WriteLine(output_path);
                        } else {
                            stop = true;
                        }
                    } else {
                        MessageBox.Show(this, "Could not detect any PS3 compatible drives.","I Have A 60GB And I Love It");
                        stop = true;
                    }
                } else if(game_data.ps1_ids.Count!=0) {
                    if(settings.playstation.ps3_export!=null)
                        put_it_here.setDrive(Path.GetPathRoot(settings.playstation.ps3_export));
                    else if(settings.playstation.psp_saves!=null)
                        put_it_here.setDrive(Path.GetPathRoot(settings.playstation.psp_saves));
                    if(put_it_here.driveCombo.Items.Count>0) {
                        if(put_it_here.ShowDialog(this)!=DialogResult.Cancel) {
                            output_path = ((string)put_it_here.driveCombo.SelectedItem).Substring(0, 3);
                            Console.WriteLine(output_path);
                        } else {
                            stop = true;
                        }
                    } else {
                        MessageBox.Show(this, "Could not detect any PS3 or PSP compatible drives.","Backward Compatibility's A Bitch");
                        stop = true;
                    }
                } else if(game_data.ps3_ids.Count!=0) {
                    if(settings.playstation.ps3_saves!=null)
                        put_it_here.setDrive(Path.GetPathRoot(settings.playstation.ps3_saves));
                    if(put_it_here.driveCombo.Items.Count>0) {
                        if(put_it_here.ShowDialog(this)!=DialogResult.Cancel) {
                            output_path = ((string)put_it_here.driveCombo.SelectedItem).Substring(0, 3);
                            Console.WriteLine(output_path);
                        } else {
                            stop = true;
                        }
                    } else {
                        MessageBox.Show(this, "Could not detect any PS3 compatible drives.","$600 Paper Weight");
                        stop = true;
                    }
                }
            }

            if(!stop&&game_data.platform=="Windows") {
                rootSelector select_root = new rootSelector(game_data.detected_roots);
                if(select_root.rootCombo.Items.Count>1) {
                    if(select_root.ShowDialog(this)==DialogResult.Cancel)
                        stop = true;
                }
                output_path = (string)select_root.rootCombo.SelectedItem;
                select_root.Dispose();

                if(!stop) {
                    if(output_path.Contains("%STEAMUSER%")) {
                        if(settings.steam.path!=null) {
                            userSelector steam_user_selector = new userSelector();
                            foreach(string add_me in settings.steam.users) {
                                steam_user_selector.userSelectorCombo.Items.Add(add_me);
                            }
                            steam_user_selector.setCombo(backup_owner);
                            if(steam_user_selector.ShowDialog(this).ToString()=="Cancel")
                                stop=true;
                            else
                                output_path = output_path.Replace("%STEAMUSER%", steam_user_selector.userSelectorCombo.SelectedItem.ToString());
                        } else
                            stop=true;
                    } else if(output_path.Contains("%USERNAME%")) {
                        if(all_users_mode) {
                            userSelector system_user_selector = new userSelector();
                            foreach(string add_me in settings.paths.user_list) {
                                system_user_selector.userSelectorCombo.Items.Add(add_me);
                            }
                            system_user_selector.setCombo(backup_owner);
                            if (system_user_selector.ShowDialog(this).ToString() == "Cancel")
                                stop=true;
                            else
                                output_path = output_path.Replace("%USERNAME%", system_user_selector.userSelectorCombo.SelectedItem.ToString());
                        } else {
                            output_path = output_path.Replace("%USERNAME%", (string)settings.paths.user_list[0]);
                        }
                    }
                }
            }

            if (!stop&&MessageBox.Show(this,"Restoring will probably overwrite any existing saves.\nDo you want to continue?", "Are you sure you want to restore?", MessageBoxButtons.YesNo) == DialogResult.No)
                stop = true;

            if(!stop) {
                groupBox1.Text = "Extracting " + game_data.title + " Archive...";
                if(restore.extractBackup(the_backup.FullName,overallProgress)==-1)
                    stop = true;
            }
            if(!stop) {
                groupBox1.Text = "Copying " + game_data.title + " Data To Destination...";
                string result = "";
                
                restore.restoreBackup(output_path,backup_owner,overallProgress);
                
                switch (result) {
                    case "Steam Not Installed":
                        overallProgress.Value = 0;
                        MessageBox.Show(this,"Steam is not detected. Please use the Settings tab to locate Steam or install it from http:////steampowered.com//", "Steam is required to restore this backup");
                        break;
                    case "Backup Not Found":
                        overallProgress.Value = 0;
                        MessageBox.Show(this,"Couldn't find the backup. WTF?", "The frell happened?");
                        break;
                    case "Success":
                        overallProgress.Value = 2;
                        MessageBox.Show(this,"Restore Complete!", "Super Success!");
                        break;
                    case "Cancelled":
                        break;
                }
            }
            this.Close();
        }

        private void createBackup() {
            if(settings==null) {
                if (back_these_up!=null&&back_these_up.Count!=0) {
                    if (back_these_up.Count>1){
                        groupBox1.Text = "Detecting games...";
                        settings = new SettingsManager(config_file, back_these_up, overallProgress);
                    } else {
                        groupBox1.Text = "Detecting game...";
                        settings = new SettingsManager(config_file, back_these_up, null);
                    }
                } else {
                    groupBox1.Text = "Detecting games...";
                    settings = new SettingsManager(config_file, null, overallProgress);
                }
            }

            if(settings.backup_path!=null||output_override!=null) {
				if(output_override!=null)
					back_up = new ArchiveManager(output_override);
				else 
					back_up = new ArchiveManager(settings.backup_path);

                if(!stop){
                    if (back_up.ready) {
                        int game_count;

                        if(back_these_up!=null)
                            game_count = back_these_up.Count;
                        else
                            game_count = settings.countGames();

                        if (game_count > 0) {
                            overallProgress.Minimum = 0;
                            if (game_count > 1){
                                overallProgress.Maximum = game_count;
                                overallProgress.Value = 1;
                                currentProgress.Minimum = 0;
                            }
                            int i = 0;
                            foreach(KeyValuePair<string,GameData> parse_me in settings.games) {
                                if(stop) {
                                    break;
                                }
                                if(back_these_up==null||back_these_up.Contains(parse_me.Key)) {
                                    if (parse_me.Value.detected_roots.Count>0&&(back_these_up!=null||!parse_me.Value.disabled)) {
                                        i++;
                                        back_up.setName(parse_me.Key);
										if(only_these_files!=null)
											back_up.addSave(only_these_files);
										else 
											back_up.addSave(parse_me.Value.getSaves());
                                        Console.WriteLine("Backing up " + parse_me.Key);
                                        groupBox1.Text = "Backing up " + parse_me.Value.title + " (" + i.ToString() + "/" + game_count + ")";
                                        notifyIcon1.ShowBalloonTip(30, "MASGAU is backing up", parse_me.Value.title, ToolTipIcon.Info);
                                        try {
                                            if (game_count > 1) {
                                                back_up.archiveIt(currentProgress,settings.ignore_date_check,false,settings.versioningTimeout(),settings.versioning_max);
                                            } else {
                                                back_up.archiveIt(overallProgress, settings.ignore_date_check, false, settings.versioningTimeout(),settings.versioning_max);
                                            }
                                        } catch {
                                            MessageBox.Show("Something went wrong while trying to back up " + parse_me.Key);
                                        }

                                        back_up.clearArchive();
                                        overallProgress.PerformStep();
                                    }
                                }
                            }
                            overallProgress.Value = overallProgress.Maximum;
                            Console.WriteLine("Backup Complete");
                            notifyIcon1.ShowBalloonTip(30, "MASGAU finished it's nasty business", "And we are all the worse for it", ToolTipIcon.Info);
                            overallProgress.Value = overallProgress.Maximum;
                        } else {
                            MessageBox.Show(this,"Nothing to back up");
                            Console.WriteLine("Nothing to backup");
                        }
                    } else {
                        zipLink.ShowDialog(this);
                        Console.WriteLine("7-Zip Not Found. Please install from http://www.7-zip.org/");
                    }
                }
            } else {
                MessageBox.Show(this,"Backup path not set. Please set it from the main program.","Do Your Job");
            }
            this.Close();

        }

        private void backendForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();
            stop = true;
            if(back_up!=null)
                back_up.stop = true;
        }
    }
}