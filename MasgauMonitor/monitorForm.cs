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
        private ArrayList watchmen;
        private Dictionary<string,ArrayList> paths;
        private ArchiveManager backup;
        private ArrayList backup_queue;
        private bool stop = false;
        private Thread backup_thread;

        public monitorForm()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            backup_queue = new ArrayList();
            setItUp();
            backup_thread = new Thread(backupLoop);
            backup_thread.Start();
        }

        private void setItUp() {
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            settings = new SettingsManager(null, null, progressBar1);
            monitorNotifier.Visible = false;
            backup = new ArchiveManager(settings.backup_path);
            paths = new Dictionary<string, ArrayList>();
            int i;
			watchmen = new ArrayList();
			lock(watchmen) {
				foreach (KeyValuePair<string, GameData> game in settings.games)
				{
					if (game.Value.platform == "Windows"&&!game.Value.disabled){
						foreach (file_holder game_root in game.Value.detected_roots){
							if(!paths.ContainsKey(game_root.absolute_path))
								paths.Add(game_root.absolute_path,new ArrayList());
							paths[game_root.absolute_path].Add(game.Key);

							i = watchmen.Add(new FileSystemWatcher(game_root.absolute_path, "*"));
							((FileSystemWatcher)watchmen[i]).IncludeSubdirectories = true;
							((FileSystemWatcher)watchmen[i]).Created += new FileSystemEventHandler(changed);
							((FileSystemWatcher)watchmen[i]).Changed += new FileSystemEventHandler(changed);
							((FileSystemWatcher)watchmen[i]).EnableRaisingEvents = true;
						}
					}
				}
			}
            monitorNotifier.Visible = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ShowInTaskbar = false;
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
            setItUp();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
									try {
										//monitorNotifier.ShowBalloonTip(10, "Safety Will Robinson", "Trying to archive " + file.path, ToolTipIcon.Info);
										backup.archiveIt(null, settings.ignore_date_check, true, settings.versioningTimeout(),settings.versioning_max);
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
                    } else {
                        lock(backup_queue) {
                            backup_queue.RemoveAt(0);
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
				for(int i = 0;i<watchmen.Count;i++) {
					((FileSystemWatcher)watchmen[i]).Dispose();
				}
				watchmen.Clear();
			}
			lock(paths) {
				paths.Clear();
			}
			Application.Exit();
        }



    }


}