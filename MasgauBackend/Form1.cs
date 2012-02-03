using System;
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
    public partial class Form1 : Form
    {
        private string[] args = Environment.GetCommandLineArgs();
        private zipLinker zipLink = new zipLinker();
        private string back_me_up = null, restore_me = null, config_file = null, selected_game = null;
        private Thread thread;
        private SettingsManager settings;
        private RestoreHandler restore;
        private ArchiveManager back_up;

        bool log = false;

        public Form1()
        {
            if(args.Length==2) {
                restore_me = args[1].Trim('\"');
            } else {
                for(int i = 0;i<args.Length;i++) {
                    switch(args[i]) {
                        case "/log":
                            log = true;
                            break;
                        case "/backup":
                            if (args[i + 1] != null && !args[i + 1].StartsWith("/"))
                                back_me_up = args[i + 1].Trim('\"');
                            break;
                        case "/restore":
                            if (args[i + 1] != null && !args[i + 1].StartsWith("/"))
                                restore_me = args[i + 1].Trim('\"');
                            break;
                        case "/config":
                            if (args[i + 1] != null && !args[i + 1].StartsWith("/"))
                                config_file = args[i + 1].Trim('\"');
                            break;
                    }
                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(30,"MASGAU is going to do something...","What will it be?",ToolTipIcon.Info);
            if(restore_me!=null) {
                thread = new Thread(restoreBackup);
                thread.Start();

            } else {
                thread = new Thread(createBackup);
                thread.Start();

            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void restoreBackup() {
            string[] hold_me = restore_me.Replace(".gb7", "").Split('\\');
            hold_me = hold_me[hold_me.Length - 1].Split('-');
            selected_game = hold_me[0].Trim();

            settings = new SettingsManager(config_file, selected_game);
            FileInfo the_backup = null;
            if (File.Exists(settings.backup_path + "\\" + restore_me)) {
                the_backup = new FileInfo(settings.backup_path + "\\" + restore_me);
                restore = new RestoreHandler(the_backup.DirectoryName,the_backup.Name);
            }
            else if (File.Exists(restore_me)) {
                the_backup = new FileInfo(restore_me);
                restore = new RestoreHandler(the_backup.DirectoryName,the_backup.Name);
            } else {
                MessageBox.Show("Specified backup doesn't exist");
                this.Close();
                return;
            }

            restore.steam_path = settings.steam.path;
            restore.paths = settings.paths;

            foreach(string add_me in settings.paths.user_list) {
                restore.addSystemUser(add_me);
            }
            
            foreach(string add_me in settings.steam.users) {
                restore.addSteamUser(add_me);
            }

            int i = settings.findGame(selected_game);
            GameData game_data;
            if (i != -1) {
                game_data = (GameData)(settings.games[i]);
                groupBox1.Text = "Restoring " + game_data.title;
            } else {
                game_data = null;
                groupBox1.Text = "Restoring " + selected_game;
            }

            progressBar1.Value = 0;
            progressBar1.Maximum = 2;
            switch (restore.extractBackup(the_backup.Name)) {
                case "Zip Not Found":
                    zipLink.ShowDialog();
                    return;
            }

            int j = restore.findBackup(the_backup.Name);
            progressBar1.Value = 1;

            switch (restore.restoreBackup(j,game_data)) {
                case "Steam Not Installed":
                    MessageBox.Show("Steam is not detected. Please use the Settings tab to locate Steam or install it from http:////steampowered.com//", "Steam is required to restore this backup");
                    break;
                case "Backup Not Found":
                    MessageBox.Show("Couldn't find the backup. WTF?", "The frell happened?");
                    break;
                case "Game Not Detected":
                    MessageBox.Show("This backup requires for its game to be detected, and it has not been. If the game is not installed, install it. If it is installed, try running it and saving a game. If it still isn't detected, check the settings tab to see if your alternate paths are properly set. If that doesn't fix it, then the game isn't supported, which makes the existence of this backup really weird.", "Game not detected");
                    break;
                case "Success":
                    MessageBox.Show("Restore Complete!", "Super Success!");
                    progressBar1.Value = progressBar1.Maximum;
                    break;
                case "Cancelled":
                    break;
            }
            this.Close();
        }

        private void createBackup() {
            groupBox1.Text = "Detecting game saves...";
            settings = new SettingsManager(config_file, back_me_up);
            back_up = new ArchiveManager(settings.backup_path);

            if (back_up.ready) {
                if (settings.games.Count > 0) {
                    progressBar1.Maximum = settings.countDetectedGames();
                    progressBar1.Value = 0;
                    foreach (GameData parse_me in settings.games) {
                        if (parse_me.name==back_me_up||(back_me_up==null&&parse_me.root_detected)) {
                            back_up.setName(parse_me.name);
                            back_up.addSave(parse_me.getSaves());
                            Console.WriteLine("Backing up " + parse_me.name);
                            groupBox1.Text = "Backing up " + parse_me.title + " (" + (progressBar1.Value+1).ToString() + "/" + progressBar1.Maximum + ")";
                            notifyIcon1.ShowBalloonTip(30, "MASGAU is backing up", parse_me.title, ToolTipIcon.Info);
                            back_up.archiveIt();
                            back_up.clearArchive();
                            progressBar1.Value++;
                        }
                    }
                    Console.WriteLine("Backup Complete");
                    notifyIcon1.ShowBalloonTip(30, "MASGAU finished it's nasty business", "And we are all the worse for it", ToolTipIcon.Info);
                    progressBar1.Value = progressBar1.Maximum;
                } else {
                    MessageBox.Show("Nothing to back up");
                    Console.WriteLine("Nothing to backup");
                }
            } else {
                zipLink.ShowDialog();
                Console.WriteLine("7-Zip Not Found. Please install from http://www.7-zip.org/");
            }
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(thread!=null) {
                if(thread.ThreadState==System.Threading.ThreadState.Running)
                    thread.Abort();
            }


            foreach (Process kill_me in Process.GetProcesses()) {
                if (kill_me.ProcessName.StartsWith("7z")) {
                    kill_me.Kill();
                    kill_me.WaitForExit();
                }
            }
            if(settings.backup_path!=null) {
                foreach(FileInfo delete_me in new DirectoryInfo(settings.backup_path).GetFiles("*.tmp*"))
                    delete_me.Delete();
            }

        }

    }
}