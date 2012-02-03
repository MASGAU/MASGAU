using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

namespace MASGAU
{
    public partial class monitorForm : Form
    {
        private SettingsManager settings;
        private Dictionary<string,FileSystemWatcher> watchmen;
        private Dictionary<string,ArrayList> paths;
        private Dictionary<string,ArrayList> games;
        private ArchiveManager backup;
        private ArrayList backup_queue;
        private bool stop = false;
        private Thread backup_thread;
        private Thread setup_thread;
        private bool already_updated = false;
        private invokes invoke = new invokes();

        public monitorForm()
        {
            InitializeComponent();
        }

        private void monitorForm_Shown(object sender, EventArgs e)
        {
            backup_queue = new ArrayList();
			watchmen = new Dictionary<string,FileSystemWatcher>();
            paths = new Dictionary<string, ArrayList>();
            games = new Dictionary<string, ArrayList>();

            setup_thread = new Thread(setItUp);
            setup_thread.Start();
        }

        private void setItUp() {
            invoke.setFormShowInTaskbar(this,true);
            invoke.setControlVisible(this,true);
            invoke.setNotifyIconVisible(this,monitorNotifier,false);
            settings = new SettingsManager(null, null, progressBar1, null, this);

            if(settings.auto_update_enabled&&!already_updated) {
                checkForUpdate();
            } 

            if(settings.skip_backup_on_monitor_startup) {
                invoke.setToolStripMenuItemCheckState(this,fullBackupOnStartupToolStripMenuItem,CheckState.Unchecked);
            } else {
                invoke.setToolStripMenuItemCheckState(this,fullBackupOnStartupToolStripMenuItem,CheckState.Checked);
            }

            if(settings.auto_update_enabled) {
                invoke.setToolStripMenuItemCheckState(this,autoCheckForUpdatesToolStripMenuItem,CheckState.Checked);
            } else {
                invoke.setToolStripMenuItemCheckState(this,autoCheckForUpdatesToolStripMenuItem,CheckState.Unchecked);
            }

            if(settings.ignore_date_check) {
                invoke.setToolStripMenuItemCheckState(this,ignoreDatesDuringBackupToolStripMenuItem,CheckState.Checked);
            } else {
                invoke.setToolStripMenuItemCheckState(this,ignoreDatesDuringBackupToolStripMenuItem,CheckState.Unchecked);
            }

            populateAltMenu();

            RegistryManager reg = new RegistryManager(RegistryHive.CurrentUser,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
            if(File.Exists(Path.Combine(Application.StartupPath,"Monitor.exe"))) {
                if (reg.getValue("MASGAUMonitor")!=null) {
                    invoke.setToolStripMenuItemCheckState(this,startMonitorOnLoginToolStripMenuItem,CheckState.Checked);
                } else {
                    invoke.setToolStripMenuItemCheckState(this,startMonitorOnLoginToolStripMenuItem,CheckState.Unchecked);
                }
            } else {
                invoke.setToolStripMenuItemCheckState(this,startMonitorOnLoginToolStripMenuItem,CheckState.Unchecked);
                if (reg.getValue("MASGAUMonitor")!=null) {
                    reg.deleteValue("MASGAUMonitor");
                }
            }


            while(settings.backup_path==null) {
                if(changeBackupPath()=="cancelled") {
                    invoke.closeForm(this);
                    Application.Exit();
                }
            }

            invoke.setToolStripMenuItemToolTipText(this,changeBackupPathToolStripMenuItem,settings.backup_path);

            if(settings.steam.installed) {
                invoke.setToolStripMenuItemToolTipText(this,changeSteamPathToolStripMenuItem,settings.steam.path);
            } else {
                invoke.setToolStripMenuItemToolTipText(this,changeSteamPathToolStripMenuItem,"Steam Not Detected");
            }

            if(settings.versioning) {
                invoke.setToolStripMenuItemEnabled(this,setTimeBetweenCopiesToolStripMenuItem,true);
                invoke.setToolStripMenuItemEnabled(this,setMaxNumberOfCopiesToolStripMenuItem,true);
                invoke.setToolStripMenuItemCheckState(this,enableToolStripMenuItem,CheckState.Checked);
            } else {
                invoke.setToolStripMenuItemCheckState(this,enableToolStripMenuItem,CheckState.Unchecked);
                invoke.setToolStripMenuItemEnabled(this,setTimeBetweenCopiesToolStripMenuItem,false);
                invoke.setToolStripMenuItemEnabled(this,setMaxNumberOfCopiesToolStripMenuItem,false);
            }

            if(settings.versioning_max==1) {
                invoke.setToolStripMenuItemToolTipText(this,setMaxNumberOfCopiesToolStripMenuItem,settings.versioning_max + " Copy");
            } else {
                invoke.setToolStripMenuItemToolTipText(this,setMaxNumberOfCopiesToolStripMenuItem,settings.versioning_max + " Copies");
            }

            if(settings.versioning_frequency==1) {
                if(settings.versioning_unit=="Millenia") {
                    invoke.setToolStripMenuItemToolTipText(this,setTimeBetweenCopiesToolStripMenuItem,"Every " + settings.versioning_frequency + " Millenium");
                } else if(settings.versioning_unit=="Centuries") {
                    invoke.setToolStripMenuItemToolTipText(this,setTimeBetweenCopiesToolStripMenuItem,"Every " + settings.versioning_frequency + " Century");
                } else {
                    invoke.setToolStripMenuItemToolTipText(this,setTimeBetweenCopiesToolStripMenuItem,"Every " + settings.versioning_frequency + " " + settings.versioning_unit.Substring(0,settings.versioning_unit.Length-1));
                }
            } else {
                invoke.setToolStripMenuItemToolTipText(this,setTimeBetweenCopiesToolStripMenuItem,"Every " + settings.versioning_frequency + " " + settings.versioning_unit);
            }
            // This performs a full backup on startup, to ensure consistency
            if(!settings.skip_backup_on_monitor_startup) {
                invoke.setControlVisible(this,false);
                taskForm full_backup = new taskForm("backup",null,settings,false);
                full_backup.ShowDialog();
                invoke.setControlVisible(this,true);
            }

            backup = new ArchiveManager(settings.backup_path);

		    lock(watchmen) {
                foreach(KeyValuePair<string,FileSystemWatcher> dispose_me in watchmen) {
                    dispose_me.Value.Dispose();
                }
			    watchmen.Clear();
		    }
		    lock(paths) {
			    paths.Clear();
		    }
            lock(games) {
                games.Clear();
            }
            
            
            if(!backup.ready) {
            } else {
		        lock(watchmen) {
                    invoke.clearToolStripItems(this,watchedGamesMenu);
                    if(settings.games!=null) {
                        foreach (KeyValuePair<string, GameData> game in settings.games)
			            {
				            if (game.Value.platform == "Windows"&&game.Value.detected_locations.Count>0){
                                ToolStripMenuItem add_me = new ToolStripMenuItem(game.Key);
                                if(!game.Value.disabled)
                                    add_me.Checked = true;
                                else 
                                    add_me.Checked = false;
                                add_me.CheckOnClick = true;
                                add_me.CheckStateChanged += new System.EventHandler(this.noGamesDetectedToolStripMenuItem_CheckStateChanged);
                                invoke.addToolStripItem(this,watchedGamesMenu,add_me);

                                string the_path;
				                foreach (KeyValuePair<string,location_holder> game_root in game.Value.detected_locations){
                                    the_path = game_root.Value.getFullPath();
                                    if(!watchmen.ContainsKey(the_path)) {
					                    if(!paths.ContainsKey(the_path))
						                    paths.Add(the_path,new ArrayList());
					                    paths[the_path].Add(game.Key);
                                    
				                        watchmen.Add(the_path,new FileSystemWatcher(the_path, "*"));
					                    watchmen[the_path].IncludeSubdirectories = true;
					                    watchmen[the_path].Created += new FileSystemEventHandler(changed);
					                    watchmen[the_path].Changed += new FileSystemEventHandler(changed);
                                        if(!game.Value.disabled)
					                        watchmen[the_path].EnableRaisingEvents = true;
                                        else 
					                        watchmen[the_path].EnableRaisingEvents = false;
                                    } 
                                }
				            }
			            }
                    }
                    if(watchedGamesMenu.DropDownItems.Count==0) {
                        ToolStripMenuItem add_me = new ToolStripMenuItem("No Games Detected");
                        add_me.Checked = false;
                        add_me.Enabled = false;
                        invoke.addToolStripItem(this,watchedGamesMenu,add_me);
                    }
		        }
                if(watchmen.Count==0) 
                    invoke.setNotifyIconText(this,monitorNotifier,"MASGAU Monitor isn't stalking any games");
                else if(watchmen.Count==1)
                    invoke.setNotifyIconText(this,monitorNotifier,"MASGAU Monitor is stalking a single game");
                else
                    invoke.setNotifyIconText(this,monitorNotifier,"MASGAU Monitor is stalking " + watchmen.Count + " games");
                invoke.setNotifyIconVisible(this,monitorNotifier,true);
                invoke.setControlVisible(this, false);
                invoke.setFormShowInTaskbar(this,false);
            }
            backup_thread = new Thread(backupLoop);
            backup_thread.Start();
        }

        private void changed(Object sender, FileSystemEventArgs e) {
            monitor_file add_me;
            if(e.ChangeType==WatcherChangeTypes.Created||e.ChangeType==WatcherChangeTypes.Changed) {
                Console.WriteLine(e.FullPath);
                add_me.root = ((FileSystemWatcher)sender).Path;
                add_me.path = e.Name;
                lock(backup_queue) {
                    if(!backup_queue.Contains(add_me)) {
                        backup_queue.Add(add_me);
                    }
                }
            }
        }

        private void rescanGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            setItUp();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }


        private void backupLoop() {
            int count;
            monitor_file file;
            while(!stop) {
                lock(backup_queue) {
                    count = backup_queue.Count;
                }
                if(count>0) {
                    lock(backup_queue) {
                        file = ((monitor_file)backup_queue[0]);
                        backup_queue.RemoveAt(0);
                    }
                    if(File.Exists(Path.Combine(file.root,file.path))) {
						lock(paths) {
							foreach(string game in paths[file.root]) {
								file_holder this_file = settings.games[game].checkThis((Path.Combine(file.root, file.path)));
								if(this_file.getFullDirPath()!=null) {
									ArrayList send_me = new ArrayList();
									send_me.Add(this_file);
									backup.setName(game);
									backup.addSave(send_me);
									//monitorNotifier.ShowBalloonTip(10, "Safety Will Robinson", "Trying to archive " + file.path, ToolTipIcon.Info);
									try {
										if(!backup.archiveIt(null, settings.ignore_date_check, true, settings.versioningTimeout(),settings.versioning_max,null)) {
										    if(!backup_queue.Contains(file)) {
											    backup_queue.Add(file);
										    }
                                        }
									} catch {
										//monitorNotifier.ShowBalloonTip(10,"Danger Will Robinson","Error while trying to archive " + file.path,ToolTipIcon.Error);
										// If something goes wrong during backup, it's probable the file copy.
										// Reinsert the file to the end of the queue, then move on to the next one.
										if(!backup_queue.Contains(file)) {
											backup_queue.Add(file);
										}
									}
									backup.clearArchive();
								}
							}
						}
                    }
                } else {
                    // No new files? Take a nap.
                    Thread.Sleep(1000);
                }
            }
            Application.ExitThread();
        }

        private void monitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop = true;

            setup_thread.Abort();

            lock(backup_queue) {
                backup_queue.Clear();
            }
           
			lock(watchmen) {
                foreach(KeyValuePair<string,FileSystemWatcher> dispose_me in watchmen) {
                    dispose_me.Value.Dispose();
                }
				watchmen.Clear();
			}
			lock(paths) {
				paths.Clear();
			}
            lock(games) {
                games.Clear();
            }
			Application.Exit();
        }

        private void noGamesDetectedToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            string game = ((ToolStripMenuItem)sender).ToString();
            if(settings.games.ContainsKey(game)) {
				foreach (KeyValuePair<string,location_holder> game_root in settings.games[game].detected_locations){
                    if(watchmen.ContainsKey(game_root.Value.getFullPath())) {
                        if(((ToolStripMenuItem)sender).CheckState==CheckState.Checked) {
                            watchmen[game_root.Value.getFullPath()].EnableRaisingEvents = true;
                            settings.disabled_games.Remove(game);
                        } else {
                            watchmen[game_root.Value.getFullPath()].EnableRaisingEvents = false;
                            settings.disabled_games.Add(game);
                        }
                        settings.writeConfig();
                        //MessageBox.Show(game + "\n" + game_root.absolute_path + "\n" + watchmen[game_root.absolute_path].EnableRaisingEvents.ToString() );
                    }
                }
            }
        }

        private string changeBackupPath() {
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Choose where the backups will be saved.";

            if(settings.backup_path!=null) {
                folderBrowser.SelectedPath = settings.backup_path;
            }

            if(invoke.showFolderBrowserDialog(this,folderBrowser)!=DialogResult.Cancel) {
                folderBrowser.ShowNewFolderButton = false;
                if(settings.isReadable(folderBrowser.SelectedPath)) {
                    if(settings.isWritable(folderBrowser.SelectedPath)) {
                        settings.backup_path = folderBrowser.SelectedPath;
                        settings.writeConfig();
                        return "success";
                    } else {
                        MessageBox.Show(this, "You don't have permission to write to that folder.", "Freeze, Dirtbag!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return "permissionerror";
                    }
                } else {
                    MessageBox.Show(this, "You don't have permission to read that folder.", "Freeze, Dirtbag!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return "permissionerror";
                }
            } else {
                folderBrowser.ShowNewFolderButton = false;
                return "cancelled";
            }
        }


        public string checkForUpdate() {
            updateHandler updates = new updateHandler();
            already_updated = true;
            if(updates.checkVersions()) {
                if(updates.update_program) {
                    programUpdateForm program_update = new programUpdateForm(updates.latest_program_version.path);
                    if(invoke.showDialog(this,program_update)==DialogResult.OK) {
                        invoke.closeForm(this);
                        return "updated";
                    } else {
                        return "declined";
                    }
                } else {
                    if(File.Exists("Updater.exe")) {
                        if(invoke.showConfirmDialog(this,"MASGAU wishes to evolve","There are data updates available." + Environment.NewLine + "Would you like to update?")==DialogResult.Yes) {
                            invoke.setControlVisible(this, false);
                            invoke.setNotifyIconVisible(this,monitorNotifier,false);

                            ProcessStartInfo updater = new ProcessStartInfo();
                            updater.FileName = Path.Combine(Application.StartupPath,"Updater.exe");
                            SecurityHandler red_shirt = new SecurityHandler();
                            if(!red_shirt.amAdmin()) {
                                updater.Verb =  "runas";
                            }
                            try {
		                        using(Process p = Process.Start(updater)) {
                                    p.WaitForExit();
                                }
                            } catch {
                                //invokes.showMessageBox(this,"Something's wrong","The update process failed with this excuse:" + Environment.NewLine + exception.Message,MessageBoxButtons.OK,MessageBoxIcon.Error);
                            }

                            invoke.setControlVisible(this, true);
                            invoke.setNotifyIconVisible(this,monitorNotifier,false);

                            settings = new SettingsManager(null, null, progressBar1, null, this);
                            invoke.setNotifyIconVisible(this,monitorNotifier,true);
                            invoke.setControlVisible(this, false);
                            return "updated";
                        } else {
                            return "declined";
                        }
                    } else {
                        MessageBox.Show(this,"The updater executable is missing." + Environment.NewLine + "You'll probably have to reinstall MASGAU before updating will work again","How could you let this happen?",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return "error";
                    }
                }
            } else {
                return "none";
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog(this);
        }

        private void fullBackupOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if(fullBackupOnStartupToolStripMenuItem.CheckState==CheckState.Checked) {
                settings.skip_backup_on_monitor_startup = false;
                settings.writeConfig();
            } else {
                settings.skip_backup_on_monitor_startup = true;
                settings.writeConfig();
            }
        }

        private void autoCheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(autoCheckForUpdatesToolStripMenuItem.CheckState==CheckState.Checked) {
                settings.auto_update_enabled = true;
                settings.writeConfig();
            } else {
                settings.auto_update_enabled= false;
                settings.writeConfig();
            }
        }

        private void ignoreDatesDuringBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ignoreDatesDuringBackupToolStripMenuItem.CheckState==CheckState.Checked) {
                settings.ignore_date_check = true;
                settings.writeConfig();
            } else {
                settings.ignore_date_check= false;
                settings.writeConfig();
            }
        }

        private void startMonitorOnLoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistryManager reg = new RegistryManager(RegistryHive.CurrentUser,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
            if(startMonitorOnLoginToolStripMenuItem.CheckState==CheckState.Checked) {
                if (reg.getValue("MASGAUMonitor")==null)
                    reg.setValue("MASGAUMonitor", Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"Monitor.exe"));
            } else {
                reg.deleteValue("MASGAUMonitor");
            }

        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            already_updated = true;
            string result = checkForUpdate();
            if(result=="none") {
                MessageBox.Show(this,"Not A Thing","No Updates Found",MessageBoxButtons.OK,MessageBoxIcon.Information);
            } 

        }

        private void changeBackupPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result = changeBackupPath();
            while(result!="cancelled"&&result!="success") {
                result = changeBackupPath();
            }
            changeBackupPathToolStripMenuItem.ToolTipText = settings.backup_path;
            backup = new ArchiveManager(settings.backup_path);
        }

        private void changeSteamPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(settings.steam.path!=null)
                folderBrowser.SelectedPath = settings.steam.path;
            folderBrowser.Description = "Choose where Steam is located.";
            while(folderBrowser.ShowDialog(this).ToString()!="Cancel") {
                settings.overrideSteam(folderBrowser.SelectedPath);
                settings.writeConfig();
                settings.playstation = new playstationHandler();
                if (settings.steam.installed) {
                    changeSteamPathToolStripMenuItem.ToolTipText = settings.steam.path;
                    setItUp();
                    break;
                } else {
                    changeSteamPathToolStripMenuItem.ToolTipText = "Steam Not Detected";
                    folderBrowser.Description = "Steam not found there. Try again.";
                }
            }

        }

        private void populateAltMenu() {
            invoke.clearToolStripItems(this,alternateInstallPathsToolStripMenuItem);
            ToolStripMenuItem add_me;
            foreach(string new_path in settings.alt_paths) {
                add_me = new ToolStripMenuItem(new_path);
                add_me.Click += new System.EventHandler(this.removePath_Click);
                invoke.addToolStripItem(this,alternateInstallPathsToolStripMenuItem,add_me);
            }
            add_me = new ToolStripMenuItem("Add New Path...");
            add_me.Click += new System.EventHandler(this.addNewPathToolStripMenuItem_Click);
                invoke.addToolStripItem(this,alternateInstallPathsToolStripMenuItem,add_me);
        }

        private void removePath_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show(this,"Are you sure you want to remove this path:" + Environment.NewLine + ((ToolStripMenuItem)sender).Text,"It'll Go Bye Bye",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes) {
                lock(settings.alt_paths) {
                    if(settings.alt_paths.Contains(((ToolStripMenuItem)sender).Text)) {
                        settings.alt_paths.Remove(((ToolStripMenuItem)sender).Text);
                        settings.writeConfig();
                        setItUp();
                    }
                }
            }
        }

        private void addNewPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowser.Description = "Choose an additional path to search for games under.";
            folderBrowser.SelectedPath = Environment.GetLogicalDrives()[0];
            if(folderBrowser.ShowDialog(this).ToString()!="Cancel") {
                settings.addAltPath(folderBrowser.SelectedPath);
                settings.writeConfig();
                setItUp();
            }

        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(enableToolStripMenuItem.CheckState==CheckState.Checked) {
                settings.versioning = true;
                setMaxNumberOfCopiesToolStripMenuItem.Enabled= true;
                setTimeBetweenCopiesToolStripMenuItem.Enabled= true;
            } else {
                settings.versioning = false;
                setMaxNumberOfCopiesToolStripMenuItem.Enabled= false;
                setTimeBetweenCopiesToolStripMenuItem.Enabled= false;
            }
            settings.writeConfig();

        }

        private void setTimeBetweenCopiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            versioningFrequencyForm frequency = new versioningFrequencyForm(settings.versioning_frequency,settings.versioning_unit);
            if(frequency.ShowDialog(this)==DialogResult.OK) {
                settings.versioning_frequency = frequency.getFrequency();
                settings.versioning_unit = frequency.getUnit();
                settings.writeConfig();
                if(settings.versioning_frequency==1) {
                    if(settings.versioning_unit=="Millenia") {
                        setTimeBetweenCopiesToolStripMenuItem.ToolTipText = "Every " + settings.versioning_frequency + " Millenium";
                    } else if(settings.versioning_unit=="Centuries") {
                        setTimeBetweenCopiesToolStripMenuItem.ToolTipText = "Every " + settings.versioning_frequency + " Century";
                    } else {
                        setTimeBetweenCopiesToolStripMenuItem.ToolTipText = "Every " + settings.versioning_frequency + " " + settings.versioning_unit.Substring(0,settings.versioning_unit.Length-1);
                    }
                } else {
                    setTimeBetweenCopiesToolStripMenuItem.ToolTipText = "Every " + settings.versioning_frequency + " " + settings.versioning_unit;
                }
            }


        }

        private void setMaxNumberOfCopiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            versioningCountForm count = new versioningCountForm(settings.versioning_max);
            if(count.ShowDialog(this)==DialogResult.OK) {
                settings.versioning_max = count.getCount();
                settings.writeConfig();
                if(settings.versioning_max==1) {
                    setMaxNumberOfCopiesToolStripMenuItem.ToolTipText = settings.versioning_max + " Copy";
                } else {
                    setMaxNumberOfCopiesToolStripMenuItem.ToolTipText = settings.versioning_max + " Copies";
                }
            }
        }

    }


}