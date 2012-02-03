using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Security.Principal;

namespace Masgau
{
    public partial class Form1 : Form
    {
        public SettingsManager settings;
        string program_title = "MASGAU v.0.2";
        TaskHandler taskmaster;
        RestoreHandler restore;
        Process backend;
        Splash sp = new Splash();

        public Form1() {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            populateGameList();
            if (settings.backup_path != null){
                populateRestoreList();
                openBackupPath.Enabled = true;
            } else {
                backupPathInput.Text = "Backup path not set";
                restoreTree.Nodes.Add("Nothing", "No Backups detected");
            }
            Console.WriteLine(System.Environment.GetEnvironmentVariable("USER"));
            this.Text = program_title;
            populateTaskScheduler();

        }
        private void showSplash()
        {
            sp.ShowDialog();
        }

        private void populateGameList() {
            Thread th = new Thread(new ThreadStart(showSplash));
            th.Start();
			detectedTree.Nodes.Clear();
            settings = new SettingsManager(null, null, sp.detectingProgress);
            if (settings.steam.installed) 
                steamPathInput.Text = settings.steam.path;
            else
                steamPathInput.Text = "Steam Not Detected";

            foreach(GameData game in settings.games) {
                if (game.root_detected) {
                    detectedTree.Nodes.Add(game.name,game.title);
                    if(game.mods.Count!=0) {
                        foreach(ModData add_me in game.mods) {
                            if(add_me.root_detected) {
                                detectedTree.Nodes[game.name].Nodes.Add(add_me.name,add_me.title);
                            }
                        }
                    }
                }
            }
            if(settings.games.Count==0) {
                detectedTree.Nodes.Add("No Games Detected");
            }
            detectedTree.ExpandAll();
            sp.Visible = false;
            populateAltList();
            th.Abort();
        }

        private void populateAltList() {
            altPathList.Clear();
            altPathList.Columns.Add("Path", "Path", 410);
            ListViewItem add_me;
            foreach(string new_path in settings.alt_paths) {
                add_me = new ListViewItem(new_path);
                altPathList.Items.Add(add_me);
                
            }
        }

        private void populateRestoreList() {
            if(settings.backup_path!=null) {
                restore = new RestoreHandler(settings.backup_path,null);
                backupPathInput.Text = settings.backup_path;
            } else {
                restore = new RestoreHandler(null, null);
                backupPathInput.Text = "Backup path not set";
                restoreTree.Nodes.Add("Nothing", "No Backups detected");
            }

            String game_title=null, mod_title=null;
            int game_id, mod_id;
            restoreTree.Nodes.Clear();
            if(settings.backup_path!=null) {
                FileInfo[] add_us = new DirectoryInfo(settings.backup_path).GetFiles("*.gb7");
                if(add_us.Length>0) {
                    foreach(backup_holder add_me in restore.backups) {
                        game_id = settings.findGame(add_me.game_name);
                        if(game_id!=-1){
                            game_title = ((GameData)settings.games[game_id]).title;
                            if(add_me.owner!=null) {
                                if(add_me.mod_name!=null) {
                                    mod_id = ((GameData)settings.games[game_id]).findMod(add_me.mod_name);
                                    if(mod_id==-1)
                                        mod_title=add_me.mod_name;
                                    else
                                        mod_title = ((ModData)((GameData)settings.games[game_id]).mods[mod_id]).title;
                                    if (!restoreTree.Nodes.ContainsKey(add_me.game_name))
                                        restoreTree.Nodes.Add(add_me.game_name, game_title);
                                    if (!restoreTree.Nodes[add_me.game_name].Nodes.ContainsKey(add_me.mod_name))
                                        restoreTree.Nodes[add_me.game_name].Nodes.Add(add_me.mod_name, mod_title);
                                    restoreTree.Nodes[add_me.game_name].Nodes[add_me.mod_name].Nodes.Add(add_me.owner, "User: " + add_me.owner + " - " + add_me.file_date);
                                } else {
                                    if(!restoreTree.Nodes.ContainsKey(add_me.game_name))
                                        restoreTree.Nodes.Add(add_me.game_name, game_title);
                                    restoreTree.Nodes[add_me.game_name].Nodes.Add(add_me.owner, "User: " + add_me.owner + " - " + add_me.file_date);
                                }
                            } else {
                                if(add_me.mod_name!=null) {
                                    mod_id = ((GameData)settings.games[game_id]).findMod(add_me.mod_name);
                                    if(mod_id==-1)
                                        mod_title=add_me.mod_name;
                                    else
                                        mod_title = ((ModData)((GameData)settings.games[game_id]).mods[mod_id]).title;
                                    if (!restoreTree.Nodes.ContainsKey(add_me.game_name))
                                        restoreTree.Nodes.Add(add_me.game_name, game_title);
                                    if (!restoreTree.Nodes[add_me.game_name].Nodes.ContainsKey(add_me.mod_name))
                                        restoreTree.Nodes[add_me.game_name].Nodes.Add(add_me.mod_name, mod_title);
                                    restoreTree.Nodes[add_me.game_name].Nodes[add_me.mod_name].Nodes.Add("Global", "Global - " + add_me.file_date);
                                } else {
                                    if(!restoreTree.Nodes.ContainsKey(add_me.game_name))
                                        restoreTree.Nodes.Add(add_me.game_name, game_title);
                                    restoreTree.Nodes[add_me.game_name].Nodes.Add("Global", "Global - " + add_me.file_date);
                                }
                            }
                        }
                    }
                } else {
                    restoreTree.Nodes.Add("Nothing", "No Backups detected");
                }
            } else {
                backupPathInput.Text = "Backup path not set";
                restoreTree.Nodes.Add("Nothing", "No Backups detected");
            }

//            restoreTree.ExpandAll();
        }

        private void button1_Click(object sender, EventArgs e) {
            folderBrowser.Description = "Choose an additional path to search for games under.";
            if(folderBrowser.ShowDialog().ToString()!="Cancel") {
                settings.addAltPath(folderBrowser.SelectedPath);
                settings.writeConfig();
                populateGameList();
            }
        }

        private void backupPathButton_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Choose where the backups will be saved.";
            if(folderBrowser.ShowDialog().ToString()!="Cancel") {
                settings.backup_path = folderBrowser.SelectedPath;
                settings.writeConfig();
                populateRestoreList();
            }
            folderBrowser.ShowNewFolderButton = false;
            openBackupPath.Enabled = true;
        }

        private void steamPathButton_Click(object sender, EventArgs e)
        {
            folderBrowser.Description = "Choose where Steam is located.";
            if(folderBrowser.ShowDialog().ToString()!="Cancel") {
                settings.overrideSteam(folderBrowser.SelectedPath);
                settings.writeConfig();
                populateGameList();
            }
        }

        private void steamClearButton_Click(object sender, EventArgs e)
        {
            settings.resetSteam();
            settings.writeConfig();
            populateGameList();     
        }

        private void altPathList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int i = altPathList.SelectedIndices[0];
            string path = altPathList.Items[i].Text;
            altPathList.Items[i].Remove();
            settings.removeAltPath(path);
            settings.writeConfig();
            populateGameList();     

        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            populateRestoreList();
        }


        private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            settings.detectGames(null,sp.detectingProgress);
            populateGameList();

        }

        private void populateTaskScheduler() {
            taskmaster = new TaskHandler();
            if(taskmaster.schtasks_available)
                taskApply.Enabled = true;
            taskFrequency.SelectedIndex = 0;
            weekDay.SelectedIndex = 0;
            monthDay.Value = 1;
            timeOfDay.Value = taskmaster.the_times;
            //timeOfDay.Value = 
            switch(taskmaster.frequency) {
                case "daily":
                    taskFrequency.SelectedIndex = 0;
                    break;
                case "weekly":
                    taskFrequency.SelectedIndex = 1;
                    weekDay.SelectedIndex = taskmaster.day;
                    break;
                case "monthly":
                    taskFrequency.SelectedIndex = 2;
                    monthDay.Value = taskmaster.day;
                    break;
            }

            if(taskmaster.exists) {
                deleteTask.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch(taskFrequency.SelectedIndex) {
                case 0:
                    taskmaster.frequency = "daily";
                    break;
                case 1:
                    taskmaster.frequency = "weekly";
                    taskmaster.day = weekDay.SelectedIndex;
                    break;
                case 2:
                    taskmaster.frequency = "monthly";
                    taskmaster.day = (int)monthDay.Value;
                    break;
            }
            taskmaster.the_times = timeOfDay.Value;
            taskmaster.createTask();

            if(taskmaster.exists) {
                deleteTask.Enabled = true;
            }

        }

        private void taskFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(taskFrequency.SelectedIndex) {
                case 0:
                    weekDay.Enabled = false;
                    monthDay.Enabled = false;
                    break;
                case 1:
                    weekDay.Enabled = true;
                    monthDay.Enabled = false;
                    break;
                case 2:
                    weekDay.Enabled = false;
                    monthDay.Enabled = true;
                    break;
            }
        }

        private void ableTask_Click(object sender, EventArgs e)
        {
            taskmaster.deleteTask();
            deleteTask.Enabled = false;
        }

        private void exitMASGAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startBackup_Click(sender, e);
            Application.Exit();
        }

        private void startBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startBackup_Click(sender, e);
        }

        public void startBackup_Click(object sender, EventArgs e)
        {
            if(settings.backup_path!=null) {
                if(File.Exists("MasgauBackend.exe")) {
                    backend = new Process();
                    backend.StartInfo.FileName = "MasgauBackend.exe";
                    backend.StartInfo.UseShellExecute = false;
                    backend.StartInfo.CreateNoWindow = true;
                    backend.Start();
                    backend.WaitForExit();
                    populateRestoreList();
                } else {
                    MessageBox.Show("Masga's Backend is missing. Reinstall it or something.","Catastrophic Devastation!");
                }
            } else {
                MessageBox.Show("You need to set a location to put your backups before you can start backing up.","Back to the place");
            }
        }


        private void restoreTree_DoubleClick(object sender, EventArgs e)
        {
            if(restoreTree.SelectedNode!=null&&restoreTree.SelectedNode.Nodes.Count==0) {
                string restore_this;
                string restore_parent;
                if(restoreTree.SelectedNode.Parent.Parent!=null) {
                    restore_parent = restoreTree.SelectedNode.Parent.Parent.Name + "»" + restoreTree.SelectedNode.Parent.Name;
                } else {
                    restore_parent = restoreTree.SelectedNode.Parent.Name;
                }
                if(restoreTree.SelectedNode.Name=="Global") {
                    restore_this = restore_parent + ".gb7";
                } else {
                    restore_this = restore_parent + "«" + restoreTree.SelectedNode.Name + ".gb7";
                }
                if(File.Exists("MasgauBackend.exe")) {
                    Console.Write(restore_this);
                    backend = new Process();
                    backend.StartInfo.Arguments= "\"" + restore_this + "\"";
                    backend.StartInfo.FileName = "MasgauBackend.exe";
                    backend.StartInfo.UseShellExecute = false;
                    backend.StartInfo.CreateNoWindow = true;
                    backend.Start();
                    backend.WaitForExit();
                }
                else
                {
                    MessageBox.Show("Masga's Backend is missing. Reinstall it or something.","Catastrophic Devastation!");
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Process.Start("explorer","\"" + backupPathInput.Text + "\"");
        }

        private void detectedTree_DoubleClick(object sender, EventArgs e)
        {
            if(detectedTree.SelectedNode!=null) {
                detectedTree.ExpandAll();
                if(settings.backup_path!=null) {
                    string backup_this;
                    if(detectedTree.SelectedNode.Parent==null) {
                        backup_this = detectedTree.SelectedNode.Name;
                    } else {
                        backup_this = detectedTree.SelectedNode.Parent.Name + "\\" + detectedTree.SelectedNode.Name;
                    }
                    if(File.Exists("MasgauBackend.exe")) {
                        backend = new Process();
                        backend.StartInfo.Arguments = "/backup " + backup_this;

                        backend.StartInfo.FileName = "MasgauBackend.exe";
                        backend.StartInfo.UseShellExecute = false;
                        backend.StartInfo.CreateNoWindow = true;
                        backend.Start();
                        backend.WaitForExit();
                        populateRestoreList();
                    }
                    else {
                        MessageBox.Show("Masga's Backend is missing. Reinstall it or something.","Catastrophic Devastation!");
                    }
                } else {
                    MessageBox.Show("You need to set a location to put your backups before you can start backing up.","Back to the place");
                }
            }
 
        }

        private void detectedTree_Click(object sender, EventArgs e)
        {

        }
        // World domination code goes here
        private void detectedTree_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

    }
}