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

namespace MASGAU
{
    public partial class taskForm : Form
    {
        private string[] args = Environment.GetCommandLineArgs();
        private string config_file = null, selected_game = null, backup_owner = null, force_name = null;
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
        invokes invokes = new invokes();

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
            InitializeComponent();
        }

        public taskForm(string this_game, SettingsManager using_this, bool as_this, ArrayList only_these, string to_here, string called_this)
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
            force_name = called_this;
            settings = using_this;
            all_users_mode = as_this;
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
						    } else if(!args[i].Trim('\"').EndsWith(".exe")&&!args[i].Trim('\"').EndsWith(".EXE")) {
                                if (back_these_up == null)
                                    back_these_up = new ArrayList();
                                back_these_up.Add(args[i].Trim('\"'));
						    }
                        }
						break;
                }
            }
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
                backupThread = new Thread(createBackup);
                backupThread.Start();
            }

        }

        private void restoreBackup() {
            foreach(string restore_me in restore_these) {
                stop = false; 
                invokes.setControlText(groupBox1,"Detecting game for restoration...");

                FileInfo the_backup = null;
                if (File.Exists(restore_me)) {
                    the_backup = new FileInfo(restore_me);
                    restore = new RestoreHandler(the_backup.DirectoryName,the_backup.Name,null,null);
                } else {
                    invokes.showMessageBox(this,"Much like your dignity","The specified backup doesn't exist",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    invokes.closeForm(this);
                    return;
                }

                selected_game = restore.backups[the_backup.Name].game_name;
                backup_owner = restore.backups[the_backup.Name].owner;

                ArrayList restore_this = new ArrayList();
                restore_this.Add(selected_game);

                if(settings==null)
                    settings = new SettingsManager(config_file, restore_this, null, groupBox1, this);

                restore.steam_path = settings.steam.path;
                restore.paths = settings.paths;

                GameData game_data = null;
                if (settings.games.ContainsKey(selected_game)) {
                    game_data = settings.games[selected_game];
                } else {
                    invokes.showMessageBox(this,"Do You Bite Your Thumb At Me, Sir?","MASGAU doesn't have information for the game: " + selected_game,MessageBoxButtons.OK,MessageBoxIcon.Error);
                    stop = true;
                }
              
                location_holder output_location;
                output_location.abs_root = null;
                output_location.owner = null;
                output_location.path = null;
                output_location.rel_root = null;

                if(!stop&&game_data.platform!="Windows") {
                    driveSelector put_it_here = new driveSelector();
                    if(game_data.psp_ids.Count!=0) {
                        if(settings.playstation.psp_saves!=null)
                            put_it_here.setDrive(Path.GetPathRoot(settings.playstation.psp_saves));
                        if(put_it_here.driveCombo.Items.Count>0) {
                            if(invokes.showDriveSelector(this,put_it_here)!=DialogResult.Cancel) {
                                output_location.rel_root = "drive";
                                output_location.abs_root = ((string)put_it_here.driveCombo.SelectedItem).Substring(0, 3);
                            } else {
                                stop = true;
                            }
                        } else {
                            invokes.showMessageBox(this,"Not So Portable Now, Eh?","Could not detect any PSP compatible drives.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            stop = true;
                        }
                    } else if(game_data.ps2_ids.Count!=0) {
                        if(settings.playstation.ps3_export!=null)
                            put_it_here.setDrive(Path.GetPathRoot(settings.playstation.ps3_export));
                        if(put_it_here.driveCombo.Items.Count>0) {
                            if(invokes.showDriveSelector(this,put_it_here)!=DialogResult.Cancel) {
                                output_location.rel_root = "drive";
                                output_location.abs_root = ((string)put_it_here.driveCombo.SelectedItem).Substring(0, 3);
                            } else {
                                stop = true;
                            }
                        } else {
                            invokes.showMessageBox(this,"I Have A 60GB And I Love It","Could not detect any PS3 compatible drives.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            stop = true;
                        }
                    } else if(game_data.ps1_ids.Count!=0) {
                        if(settings.playstation.ps3_export!=null)
                            put_it_here.setDrive(Path.GetPathRoot(settings.playstation.ps3_export));
                        else if(settings.playstation.psp_saves!=null)
                            put_it_here.setDrive(Path.GetPathRoot(settings.playstation.psp_saves));
                        if(put_it_here.driveCombo.Items.Count>0) {
                            if(invokes.showDriveSelector(this,put_it_here)!=DialogResult.Cancel) {
                                output_location.rel_root = "drive";
                                output_location.abs_root = ((string)put_it_here.driveCombo.SelectedItem).Substring(0, 3);
                            } else {
                                stop = true;
                            }
                        } else {
                            invokes.showMessageBox(this,"Backward Compatibility's A Bitch","Could not detect any PS3 or PSP compatible drives.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            stop = true;
                        }
                    } else if(game_data.ps3_ids.Count!=0) {
                        if(settings.playstation.ps3_saves!=null)
                            put_it_here.setDrive(Path.GetPathRoot(settings.playstation.ps3_saves));
                        if(put_it_here.driveCombo.Items.Count>0) {
                            if(invokes.showDriveSelector(this,put_it_here)!=DialogResult.Cancel) {
                                output_location.rel_root = "drive";
                                output_location.abs_root = ((string)put_it_here.driveCombo.SelectedItem).Substring(0, 3);
                            } else {
                                stop = true;
                            }
                        } else {
                            invokes.showMessageBox(this,"$600 Paper Weight","Could not detect any PS3 compatible drives.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            stop = true;
                        }
                    }
                }

                if(!stop&&game_data.platform=="Windows") {
				    ArrayList eligable_roots = new ArrayList();
				    if(game_data.detected_locations.Count<1) {
                        // This is when there are no detected roots
                        // This generates roots that can be figured out without them being detected
                        stop = true;
                        location_holder new_root;
                        new_root.owner = null;
                        new_root.abs_root = null;
					    rootSelector select_root = new rootSelector();
                        select_root.Text = "Game Not Detected, But...";
                        select_root.groupBox1.Text = "These Locations Can Be Restored To Automatically";

                        bool add_it;
                        string path_entry = null;
					    foreach(location_path_holder location in game_data.location_paths) {
                            add_it = true;
					        if(!game_data.detection_required&&
                                    (location.windows_version==null||location.windows_version==settings.windows_version)&&
                                    location.environment_variable!=null&&
                                    location.path!=null&&
                                    location.environment_variable!="installlocation"&&
                                    location.environment_variable!="steamcommon"&&
                                    location.environment_variable!="steamsourcemods") {
                                new_root.path = location.path;
                                new_root.rel_root = location.environment_variable;
                                // Adds user-friendly menu entries to the root selector
                                switch(location.environment_variable) {
                                    case "steamuser":
                                        if(settings.steam.installed&&settings.steam.users.Count>0) {
                                            //eligable_roots.Add(new_root);
    					                    //select_root.rootCombo.Items.Add(Path.Combine("Steam User",new_root.path));
                                            path_entry = Path.Combine("Steam User", new_root.path);
                                        } else {
                                            add_it = false;
                                        }
                                        break;
                                    case "steamuserdata":
                                        if(settings.steam.installed&&settings.steam.userdatas.Count>0) {
                                            //eligable_roots.Add(new_root);
    					                    //select_root.rootCombo.Items.Add(Path.Combine("Steam Userdata",new_root.path));
                                            path_entry = Path.Combine("Steam Userdata",new_root.path);
                                        } else {
                                            add_it = false;
                                        }
                                        break;
                                    case "allusersprofile":
                                        path_entry = Path.Combine("All Users",new_root.path);
                                        break;
                                    case "appdata":
                                        path_entry = Path.Combine("App Data",new_root.path);
                                        break;
                                    case "localappdata":
                                        path_entry = Path.Combine("Local App Data",new_root.path);
                                        break;
                                    case "public":
                                        path_entry = Path.Combine("Public",new_root.path);
                                        break;
                                    case "savedgames":
                                        path_entry = Path.Combine("Saved Games",new_root.path);
                                        break;
                                    case "userprofile":
                                        path_entry = Path.Combine("User",new_root.path);
                                        break;
                                    case "userdocuments":
                                        path_entry = Path.Combine("Documents",new_root.path);
                                        break;
                                }
                                if(add_it) {
                                    if(!select_root.rootCombo.Items.Contains(path_entry)) {
                                        eligable_roots.Add(new_root);
    					                select_root.rootCombo.Items.Add(path_entry);
                                    }
                                }
					        }
					    }
					    if(eligable_roots.Count==0) {
                            invokes.showMessageBox(this,"Never Was There A Tale Of More Woah","The game associated with this backup could not be detected.\nMake sure the game is installed and try making a save with it.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            stop = true;
                        } else {
                            if(eligable_roots.Count>0) {
					            select_root.rootCombo.SelectedIndex = 0;
                                if(invokes.showRootSelector(this,select_root)!=DialogResult.Cancel) {
                                    output_location = (location_holder)eligable_roots[select_root.rootCombo.SelectedIndex];
					                stop = false;
					            } else {
                                    stop = true;
                                }
                            } else {
                                output_location = (location_holder)eligable_roots[0];
                                stop = false;
                            }
					    }
				    } else {
					    rootSelector select_root = new rootSelector();
                        bool add_it;
                        string path_entry = null;
					    foreach(KeyValuePair<string,location_holder> location in game_data.detected_locations) {
                            add_it = true;
                            switch(location.Value.rel_root) {
                                case "steamcommon":
                                    if(settings.steam.installed) {
    					                path_entry = Path.Combine("Steam Common",location.Value.path);
                                    } else {
                                        add_it = false;
                                    }
                                    break;
                                case "steamuser":
                                    if(settings.steam.installed) {
    					                path_entry = Path.Combine("Steam User",location.Value.path);
                                    } else {
                                        add_it = false;
                                    }
                                    break;
                                case "steamuserdata":
                                    if(settings.steam.installed) {
    					                path_entry = Path.Combine("Steam Userdata",location.Value.path);
                                    } else {
                                        add_it = false;
                                    }
                                    break;
                                case "steamsourcemods":
                                    if(settings.steam.installed) {
    					                path_entry = Path.Combine("Steam Source Mods",location.Value.path);
                                    } else {
                                        add_it = false;
                                    }
                                    break;
                                case "allusersprofile":
    					            path_entry = Path.Combine("All Users",location.Value.path);
                                    break;
                                case "appdata":
    					            path_entry = Path.Combine("AppData",location.Value.path);
                                    break;
                                case "localappdata":
    					            path_entry = Path.Combine("LocalAppdata",location.Value.path);
                                    break;
                                case "public":
    					            path_entry = Path.Combine("Public",location.Value.path);
                                    break;
                                case "savedgames":
    					            path_entry = Path.Combine("Saved Games",location.Value.path);
                                    break;
                                case "userprofile":
    					            path_entry = Path.Combine("User",location.Value.path);
                                    break;
                                case "userdocuments":
    					            path_entry = Path.Combine("Documents",location.Value.path);
                                    break;
                                default:
    					            path_entry = location.Value.getFullPath();
                                    break;
                            }
                            if(add_it) {
                                if(!select_root.rootCombo.Items.Contains(path_entry)) {
                                    eligable_roots.Add(location.Value);
    					            select_root.rootCombo.Items.Add(path_entry);
                                }
                            }
					    }


					    select_root.rootCombo.SelectedIndex = 0;
					    if(select_root.rootCombo.Items.Count>0) {
                            if(invokes.showRootSelector(this,select_root)==DialogResult.Cancel)
							    stop = true;
					    }
					    output_location = (location_holder)eligable_roots[select_root.rootCombo.SelectedIndex];

					    select_root.Dispose();
                    }
                }

                if(!stop) {
                    if(output_location.rel_root=="steamuser") {
                        if(settings.steam.users.Count>0) {
                            userSelector steam_user_selector = new userSelector();
                            foreach(string add_me in settings.steam.users) {
                                steam_user_selector.userSelectorCombo.Items.Add(add_me);
                            }
                            steam_user_selector.setCombo(backup_owner);
                            if(invokes.showUserSelector(this,steam_user_selector)==DialogResult.Cancel)
                                stop=true;
                            else
                                output_location.abs_root = settings.steam.getAbsoluteRoot(output_location,steam_user_selector.userSelectorCombo.SelectedItem.ToString());
                        } else {
                            invokes.showMessageBox(this,"No Steam Ahead!","This restore path requires a detected Steam user.\nTry running one of Valve's games or Rag Doll Kung Fu through Steam then try again.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            stop=true;
                        }
                    } else if(output_location.rel_root=="steamuserdata") {
                        if(settings.steam.userdatas.Count>0) {
                            userSelector steam_user_selector = new userSelector();
                            foreach(string add_me in settings.steam.userdatas) {
                                steam_user_selector.userSelectorCombo.Items.Add(add_me);
                            }
                            steam_user_selector.setCombo(backup_owner);
                            if(invokes.showUserSelector(this,steam_user_selector)==DialogResult.Cancel)
                                stop=true;
                            else
                                output_location.abs_root = settings.steam.getAbsoluteRoot(output_location,steam_user_selector.userSelectorCombo.SelectedItem.ToString());
                        } else {
                            invokes.showMessageBox(this,"No Steam Data!","This restore path requires a detected Steam userdata folder.\nTry running Team Fortress 3 or something through Steam then try again.",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            stop=true;
                        }
                    } else if(output_location.rel_root=="appdata"||
                                output_location.rel_root=="localappdata"||
                                output_location.rel_root=="savedgames"||
                                output_location.rel_root=="userprofile"||
                                output_location.rel_root=="userdocuments") {

                        userSelector system_user_selector = new userSelector();
                        foreach(KeyValuePair<string,user_data> add_me in settings.paths.users) {
                            system_user_selector.userSelectorCombo.Items.Add(add_me.Value.name);
                        }
                        system_user_selector.userSelectorCombo.SelectedIndex = 0;
                        system_user_selector.setCombo(backup_owner);
                        if(system_user_selector.userSelectorCombo.Items.Count>1) {
                            if(invokes.showUserSelector(this,system_user_selector)==DialogResult.Cancel)
                                stop=true;
                            else
                                output_location.abs_root = settings.paths.getAbsoluteRoot(output_location,system_user_selector.userSelectorCombo.SelectedItem.ToString());
                        } else {
                            output_location.abs_root = settings.paths.getAbsoluteRoot(output_location, system_user_selector.userSelectorCombo.SelectedItem.ToString());
                        }
                    } else {
                        if(output_location.abs_root==null) {
                            if(output_location.rel_root.StartsWith("steam")) {
                                output_location.abs_root = settings.steam.getAbsoluteRoot(output_location,null);
                            } else {
                                output_location.abs_root = settings.paths.getAbsoluteRoot(output_location,null);
                            }
                        }
                    }
                }
                if(!stop)
                    invokes.setControlText(groupBox1,"Waiting on restoring " + game_data.title);

                if (!stop&&invokes.showConfirmDialog(this,"Are you sure you want to restore?","Restoring will probably overwrite any existing saves.\nDo you want to continue?") == DialogResult.No)
                    stop = true;

                if(!stop) {
                    invokes.setControlText(groupBox1,"Extracting " + game_data.title + " Archive...");
                    if(restore.extractBackup(the_backup.FullName,overallProgress)==-1)
                        stop = true;
                }
                if(!stop) {
                    invokes.setControlText(groupBox1,"Copying " + game_data.title + " Data To Destination...");
                    string result = "";
                
                    restore.restoreBackup(output_location.getFullPath(),backup_owner,overallProgress,this,the_backup.FullName);
                
                    switch (result) {
                        case "Steam Not Installed":
                            invokes.setProgressBarValue(overallProgress,1);
                            invokes.showMessageBox(this,"Steam is required to restore this backup","Steam is not detected. Please use the Settings tab to locate Steam or install it from http:////steampowered.com//",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            break;
                        case "Backup Not Found":
                            invokes.setProgressBarValue(overallProgress,1);
                            invokes.showMessageBox(this,"The frell happened?","Couldn't find the backup. WTF?",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            break;
                        case "Success":
                            invokes.setProgressBarValue(overallProgress,2);
                            invokes.showMessageBox(this,"Super Success!","Restore Complete!",MessageBoxButtons.OK,MessageBoxIcon.Information);
                            break;
                        case "Cancelled":
                            break;
                        default:
                            invokes.setProgressBarValue(overallProgress,2);
                            invokes.showMessageBox(this,"Super Success!","Restore Complete!",MessageBoxButtons.OK,MessageBoxIcon.Information);
                            break;
                    }
                }
            }
            invokes.closeForm(this);
        }

        private void createBackup() {
            if(settings==null) {
                if (back_these_up!=null&&back_these_up.Count!=0) {
                    if (back_these_up.Count>1){
                        invokes.setControlText(groupBox1,"Detecting selected games for backup...");
                        settings = new SettingsManager(config_file, back_these_up, overallProgress, groupBox1,this);
                    } else {
                        invokes.setControlText(groupBox1,"Detecting selected game for backup...");
                        settings = new SettingsManager(config_file, back_these_up, null, groupBox1,this);
                    }
                } else {
                    invokes.setControlText(groupBox1,"Detecting games for backup...");
                    settings = new SettingsManager(config_file, null, overallProgress, groupBox1, this);
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
                            invokes.setProgressBarMin(overallProgress,0);
                            if (game_count > 1){
                                invokes.setControlVisible(currentProgress, true);
                                invokes.setProgressBarMax(overallProgress, game_count);
                                invokes.setProgressBarValue(overallProgress, 1);
                                invokes.setProgressBarMin(currentProgress, 0);
                            }
                            int i = 0;
                            foreach(KeyValuePair<string,GameData> parse_me in settings.games) {
                                if(stop) {
                                    break;
                                }
                                if(back_these_up==null||back_these_up.Contains(parse_me.Key)) {
                                    if (parse_me.Value.detected_locations!=null&&parse_me.Value.detected_locations.Count>0&&(back_these_up!=null||!parse_me.Value.disabled)) {
                                        i++;
                                        back_up.setName(parse_me.Key);
										if(only_these_files!=null)
											back_up.addSave(only_these_files);
										else 
											back_up.addSave(parse_me.Value.getSaves());
                                        Console.WriteLine("Backing up " + parse_me.Key);
                                        invokes.setControlText(groupBox1,"Backing up " + parse_me.Value.title + " (" + i.ToString() + "/" + game_count + ")");
                                        notifyIcon1.ShowBalloonTip(30, "MASGAU is backing up", parse_me.Value.title, ToolTipIcon.Info);
                                        try {
                                            if (game_count > 1) {
                                                if(force_name!=null) 
                                                    back_up.archiveIt(currentProgress,settings.ignore_date_check,false,settings.versioningTimeout(),settings.versioning_max,force_name);
                                                else 
                                                    back_up.archiveIt(currentProgress,settings.ignore_date_check,false,settings.versioningTimeout(),settings.versioning_max,null);
                                            } else {
                                                if(force_name!=null)
                                                    back_up.archiveIt(overallProgress,settings.ignore_date_check,false, settings.versioningTimeout(),settings.versioning_max,force_name);
                                                else 
                                                    back_up.archiveIt(overallProgress,settings.ignore_date_check,false, settings.versioningTimeout(),settings.versioning_max,null);
                                            }
                                        } catch {
                                            invokes.showMessageBox(this,"Not sure what, but","Something went wrong while trying to back up " + parse_me.Key,MessageBoxButtons.OK,MessageBoxIcon.Error);
                                        }

                                        back_up.clearArchive();
                                        back_up.cleanUp(settings.backup_path);
                                        invokes.performStep(overallProgress);
                                    }
                                }
                            }
                            invokes.setProgressBarValue(overallProgress,overallProgress.Maximum);
                            Console.WriteLine("Backup Complete");
                            notifyIcon1.ShowBalloonTip(30, "MASGAU finished it's nasty business", "And we are all the worse for it", ToolTipIcon.Info);
                            invokes.setProgressBarValue(overallProgress,overallProgress.Maximum);
                        } else {
                            invokes.showMessageBox(this,"Feels like there's...","Nothing at all to back up",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            Console.WriteLine("Nothing to backup");
                        }
                    } else {
                        Console.WriteLine("7-Zip Not Found. Please install from http://www.7-zip.org/");
                    }
                }
            } else {
                invokes.showMessageBox(this,"Do Your Job","Backup path not set. Please set it from the main program.",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            invokes.closeForm(this);

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

        private void backendForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            groupBox1.Text = "Cancelling...";
            cancelButton.Enabled = false;
            stop = true;
            while(backupThread!=null&&backupThread.IsAlive) {
                if(back_up!=null) {
                    back_up.stop = true;
                }
                Thread.Sleep(100);
            }
            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();
        }
    }
}