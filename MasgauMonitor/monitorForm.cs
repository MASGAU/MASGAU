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

namespace Masgau
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
            if(setItUp()) {
                backup_thread = new Thread(backupLoop);
                backup_thread.Start();
            } else {
                this.Close();
                Application.Exit();
            }
        }

        private bool setItUp() {
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            settings = new SettingsManager(null, null, progressBar1, null);
            monitorNotifier.Visible = false;
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
                zipLinker zipper = new zipLinker();
                if(zipper.ShowDialog()!=DialogResult.Ignore) {
                    return false;
                }
            } else {
			    lock(watchmen) {
                    watchedGamesMenu.DropDownItems.Clear();
                    foreach (KeyValuePair<string, GameData> game in settings.games)
				    {
					    if (game.Value.platform == "Windows"&&game.Value.detected_roots.Count>0){
                            ToolStripMenuItem add_me = new ToolStripMenuItem(game.Key);
                            if(!game.Value.disabled)
                                add_me.Checked = true;
                            else 
                                add_me.Checked = false;
                            add_me.CheckOnClick = true;
                            add_me.CheckStateChanged += new System.EventHandler(this.noGamesDetectedToolStripMenuItem_CheckStateChanged);
                            watchedGamesMenu.DropDownItems.Add(add_me);

					        foreach (file_holder game_root in game.Value.detected_roots){
						        if(!paths.ContainsKey(game_root.absolute_path))
							        paths.Add(game_root.absolute_path,new ArrayList());
						        paths[game_root.absolute_path].Add(game.Key);
                                
					            watchmen.Add(game_root.absolute_path,new FileSystemWatcher(game_root.absolute_path, "*"));
						        watchmen[game_root.absolute_path].IncludeSubdirectories = true;
						        watchmen[game_root.absolute_path].Created += new FileSystemEventHandler(changed);
						        watchmen[game_root.absolute_path].Changed += new FileSystemEventHandler(changed);
                                if(!game.Value.disabled)
						            watchmen[game_root.absolute_path].EnableRaisingEvents = true;
                                else 
						            watchmen[game_root.absolute_path].EnableRaisingEvents = false;

                            }
					    }
				    }
			    }
                monitorNotifier.Text = "MASGAU Monitor is stalking " + watchmen.Count + " games";
                monitorNotifier.Visible = true;
                this.Visible = false;
                //this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            return true;
        }

        private void changed(Object sender, FileSystemEventArgs e)
        {
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
								file_holder this_one = settings.games[game].checkThis(Path.Combine(file.root, file.path));
								if(this_one.absolute_path!=null) {
									ArrayList send_me = new ArrayList();
									send_me.Add(this_one);
									backup.setName(game);
									backup.addSave(send_me);
									monitorNotifier.ShowBalloonTip(10, "Safety Will Robinson", "Trying to archive " + file.path, ToolTipIcon.Info);
									try {
										if(!backup.archiveIt(null, settings.ignore_date_check, true, settings.versioningTimeout(),settings.versioning_max,null)) {
										    if(!backup_queue.Contains(file)) {
											    backup_queue.Add(file);
										    }
                                        }
									} catch {
										monitorNotifier.ShowBalloonTip(10,"Danger Will Robinson","Error while trying to archive " + file.path,ToolTipIcon.Error);
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
				foreach (file_holder game_root in settings.games[game].detected_roots){
                    if(watchmen.ContainsKey(game_root.absolute_path)) {
                        if(((ToolStripMenuItem)sender).CheckState==CheckState.Checked) {
                            watchmen[game_root.absolute_path].EnableRaisingEvents = true;
                            settings.disabled_games.Remove(game);
                        } else {
                            watchmen[game_root.absolute_path].EnableRaisingEvents = false;
                            settings.disabled_games.Add(game);
                        }
                        settings.writeConfig();
                        //MessageBox.Show(game + "\n" + game_root.absolute_path + "\n" + watchmen[game_root.absolute_path].EnableRaisingEvents.ToString() );
                    }
                }
            }
        }




    }


}