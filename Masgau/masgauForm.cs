using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Security.Principal;
using Microsoft.Win32;

namespace Masgau
{

    public partial class masgauForm : Form
    {
        public SettingsManager settings;
        string program_title = "MASGAU v.0.4";
        private TaskHandler taskmaster;
        private RestoreHandler restore;
        private Splash sp = new Splash();
        private SecurityHandler red_shirt = new SecurityHandler();
        private bool all_users_mode = false;
        private bool redetect_required = false;

        public masgauForm() {
            string[] args = Environment.GetCommandLineArgs();
            for(int i = 0;i<args.Length;i++) {
                if(args[i]=="/allusers") {
                    all_users_mode = true;
                }
            }
            if(all_users_mode)
                program_title += " - All Users Mode";
            else
                program_title += " - Single User Mode";
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            detectGames();
            if (settings.backup_path != null){
                populateRestoreList();
                openBackupPath.Enabled = true;
            } else {
                backupPathInput.Text = "Backup path not set";
                restoreTree.Nodes.Add("Nothing", "No Backups detected");
            }

            if(settings.versioning) {
                duplicateCount.Value = settings.versioning_max;
                duplicateFrequencyNumber.Value = settings.versioning_frequency;
                duplicateFrequencyCombo.SelectedIndex = duplicateFrequencyCombo.Items.IndexOf(settings.versioning_unit);
                versioningCheck.Checked = true;
            } else {
                duplicateCount.Enabled = false;
                duplicateCountBox.Enabled = false;
                duplicateFrequencyBox.Enabled = false;
                duplicateFrequencyCombo.Enabled = false;
                duplicateFrequencyCombo.SelectedIndex = 0;
                duplicateFrequencyNumber.Enabled = false;
            }
            if(all_users_mode) {
                monitorCheck.Enabled = false;
            } else {
                RegistryKey startup_keys = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (startup_keys.GetValue("MASGAUMonitor")!=null) {
                    monitorCheck.Checked = true;
                }
            }

            if (settings.ignore_date_check)
                dateCheck.Checked = true;

			//if (settings.show_undetected)
			//    undetectedCheck.Checked = true;

            this.Text = program_title;
            if(all_users_mode) {
                populateTaskScheduler();
            } else {
                tabControl1.TabPages.Remove(scheduleTab);
            }
        }

        private void showSplash()
        {
            sp.ShowDialog();
        }

        private void detectGames() {
            Thread th = new Thread(new ThreadStart(showSplash));
            th.Start();

            settings = new SettingsManager(null, null, sp.detectingProgress);
            if (settings.steam.installed)
                steamPathInput.Text = settings.steam.path;
            else
                steamPathInput.Text = "Steam Not Detected";

            populateGameList();
            populateAltList();
            th.Abort();
        }

        private void populateGameList() {
            ListViewItem new_item;
            detectedList.Items.Clear();
            if (settings.games != null)
            {
                foreach(KeyValuePair<string,GameData> game in settings.games) {
                    if (game.Value.detected_roots.Count>0) {
                        new_item = new ListViewItem(game.Value.title);
                        new_item.ToolTipText= ((file_holder)game.Value.detected_roots[0]).absolute_path;
                        for(int i = 1; i< game.Value.detected_roots.Count;i++) {
                            new_item.ToolTipText += Environment.NewLine + ((file_holder)game.Value.detected_roots[i]).absolute_path;
                        }
                        new_item.SubItems.Add(game.Value.platform);
                        new_item.SubItems.Add(game.Key);
                        if(game.Value.disabled)
                            new_item.ForeColor = Color.Red;
                        else 
                            new_item.ForeColor = Color.Black;
                        detectedList.Items.Add(new_item);
                    } else if(settings.show_undetected) {
                        new_item = new ListViewItem(game.Value.title);
                        new_item.ToolTipText = "No Path Detected";
                        for (int i = 1; i < game.Value.detected_roots.Count; i++)
                        {
                            new_item.ToolTipText += Environment.NewLine + ((file_holder)game.Value.detected_roots[i]).absolute_path;
                        }
                        new_item.SubItems.Add(game.Value.platform);
                        new_item.SubItems.Add(game.Key);
                        if (game.Value.disabled)
                            new_item.ForeColor = Color.Salmon;
                        else 
                            new_item.ForeColor = Color.Gray;
                        detectedList.Items.Add(new_item);
                    }
                }
                startBackup.Enabled = true;
            } else {
                startBackup.Enabled = false;
                new_item = new ListViewItem("nobackup");
                new_item.SubItems.Add("No Games Detected");
                new_item.ToolTipText = "Where have all the games gone?";
                detectedList.Items.Add(new_item);
            }
            sp.Visible = false;
        }

        private void populateAltList() {
            altPathList.Items.Clear();
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

            String game_title=null;
            restoreTree.Nodes.Clear();
            if(settings.backup_path!=null) {
                if(restore.backups.Count>0) {
                    foreach(backup_holder add_me in restore.backups) {
                        if(settings.games!=null&&settings.games.ContainsKey(add_me.game_name)) {
                            if(settings.games[add_me.game_name].platform=="Windows")
                                game_title = settings.games[add_me.game_name].title;
                            else
                                game_title = settings.games[add_me.game_name].title + " (" + settings.games[add_me.game_name].platform + ")";
                        }
                        else
                        {
                            game_title = add_me.file_name;
                        }
                        if(add_me.owner!=null) {
                            if(!restoreTree.Nodes.ContainsKey(add_me.game_name))
                                restoreTree.Nodes.Add(add_me.game_name, game_title);
                            restoreTree.Nodes[add_me.game_name].Nodes.Add(add_me.file_name, "User: " + add_me.owner + " - " + add_me.file_date);
                        } else {
                            if(!restoreTree.Nodes.ContainsKey(add_me.game_name))
                                restoreTree.Nodes.Add(add_me.game_name, game_title);
                            restoreTree.Nodes[add_me.game_name].Nodes.Add(add_me.file_name, "Global - " + add_me.file_date);
                        }
                    }
                } else {
                    restoreTree.Nodes.Add("Nothing", "No Backups detected");
                }
            } else {
                backupPathInput.Text = "Backup path not set";
                restoreTree.Nodes.Add("Nothing", "No Backups detected");
            }

        }

        public bool chooseBackUpPath() {
            bool return_me = false;
            if(settings.backup_path!=null)
                folderBrowser.SelectedPath = settings.backup_path;
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Choose where the backups will be saved.";
            if(folderBrowser.ShowDialog(this).ToString()!="Cancel") {
                if(settings.isReadable(folderBrowser.SelectedPath)) {
                    if(settings.isWritable(folderBrowser.SelectedPath)) {
                        settings.backup_path = folderBrowser.SelectedPath;
                        settings.writeConfig();
                        populateRestoreList();
                        openBackupPath.Enabled = true;
                        return_me = true;
                    } else {
                        MessageBox.Show(this, "You don't have permission to write to that folder.", "Freeze, Dirtbag!");
                        return_me = false;
                    }
                } else {
                    MessageBox.Show(this, "You don't have permission to read that folder.", "Freeze, Dirtbag!");
                    return_me = false;
                }
            }
            folderBrowser.ShowNewFolderButton = false;
            return return_me;
        }






        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            populateRestoreList();
        }


        private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            settings.detectGames(null,sp.detectingProgress);
            detectGames();

        }

        private void populateTaskScheduler() {
            //DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName);
            //DirectoryEntry admGroup = localMachine.Children.Find("administrators", "group");
            //object members = admGroup.Invoke("members", null);
            //foreach (object groupMember in (System.Collections.IEnumerable)members)
            //{
            //    DirectoryEntry member = new DirectoryEntry(groupMember);
            //    if(member.Name!="Administrator")
            //        taskUser.Items.Add(member.Name);
            //} 

            //for(int i=0;i<taskUser.Items.Count;i++) {
            //    if(((string)taskUser.Items[i])==Environment.UserName)
            //        taskUser.SelectedIndex=i;
            //}

            taskUser.Text = Environment.UserName;

//            red_shirt.addShield(taskApply);
//            red_shirt.addShield(deleteTask);
            taskmaster = new TaskHandler();
            if(taskmaster.schtasks_available)
                taskApply.Enabled = true;
            taskFrequency.SelectedIndex = 0;
            weekDay.SelectedIndex = 0;
            monthDay.Value = 1;
            timeOfDay.Value = taskmaster.the_times;
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
            if(taskPassword.Text!="") {
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
                taskmaster.createTask(taskUser.Text,taskPassword.Text);
                if(taskmaster.exists) {
                    deleteTask.Enabled = true;
                } else {
                    MessageBox.Show(this,"Unable to create task. Here's the excuse:" + Environment.NewLine + taskmaster.output + Environment.NewLine + "The task has been deleted.","What Did I Just Tell You");
                    deleteTask.Enabled = false;
                }
            } else {
                MessageBox.Show(this, "You must enter a password for the user the task will be running as,\nwhich is shown in the little text box right there.","Pander To Me");
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
            this.Close();
        }

        private void startBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startBackup_Click(sender, e);
        }

        public void startBackup_Click(object sender, EventArgs e)
        {
            if(settings.backup_path!=null||chooseBackUpPath()) {
                taskForm backup = new taskForm("backup",null,settings,all_users_mode);
                backup.ShowDialog(this);
                populateRestoreList();
            }
        }


        private void restoreTree_DoubleClick(object sender, EventArgs e)
        {
            if(restoreTree.SelectedNode!=null&&restoreTree.SelectedNode.Parent!=null) {
                ArrayList restore_me = new ArrayList();
                restore_me.Add(restoreTree.SelectedNode.Name);
                Console.WriteLine(restoreTree.SelectedNode.Name);
                taskForm restore = new taskForm("restore",restore_me,settings,all_users_mode);
                restore.ShowDialog();
            }
        }


        // World domination code goes here
        private void detectedTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void detectedList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (detectedList.SelectedItems.Count != 0&&((ListViewItem)detectedList.SelectedItems[0]).Text!="nobackup") {
                backupSelection.Enabled = true;
                if(detectedList.SelectedItems.Count==1) {
                    backupSelection.Text = "Back This Up";
                } else {
                    backupSelection.Text = "Back These Up";
                }
            } else {
                backupSelection.Enabled = false;
                backupSelection.Text = "Back Nothing Up";
            }
        }



        private void detectedList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
			if(e.Column==0)
				this.detectedList.ListViewItemSorter = new ListViewItemComparer(2);
			else
				this.detectedList.ListViewItemSorter = new ListViewItemComparer(e.Column);

        }


        // Ruthlessly snatched form the ListView.Sort page on MSDN
        class ListViewItemComparer : System.Collections.IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }
        }

        private void versioningCheck_CheckedChanged(object sender, EventArgs e)
        {
            if(versioningCheck.Checked) {
                settings.versioning = true;
                duplicateCount.Enabled = true;
                settings.versioning_max = (int)duplicateCount.Value;
                duplicateCountBox.Enabled = true;
                duplicateFrequencyBox.Enabled = true;
                duplicateFrequencyCombo.Enabled = true;
                settings.versioning_unit = duplicateFrequencyCombo.SelectedItem.ToString();
                duplicateFrequencyNumber.Enabled = true;
                settings.versioning_frequency = (int)duplicateFrequencyNumber.Value;
            } else {
                settings.versioning = false;
                duplicateCount.Enabled = false;
                duplicateCountBox.Enabled = false;
                duplicateFrequencyBox.Enabled = false;
                duplicateFrequencyCombo.Enabled = false;
                duplicateFrequencyNumber.Enabled = false;
            }
            settings.writeConfig();
        }

        private void duplicateFrequencyNumber_ValueChanged(object sender, EventArgs e)
        {
            settings.versioning_frequency = (int)duplicateFrequencyNumber.Value;
            settings.writeConfig();
        }

        private void duplicateFrequencyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.versioning_unit = duplicateFrequencyCombo.SelectedItem.ToString();
            settings.writeConfig();
        }

        private void duplicateCount_ValueChanged(object sender, EventArgs e)
        {
            settings.versioning_max = (int)duplicateCount.Value;
            settings.writeConfig();
        }




        private void undetectedCheck_CheckedChanged(object sender, EventArgs e)
        {
			//if(undetectedCheck.Checked) {
			//    settings.show_undetected = true;
			//} else {
			//    settings.show_undetected = false;
			//}
            settings.writeConfig();
            populateGameList();
        }
        // Functions
        private void backupSelectedGames() {
            if (settings.backup_path != null || chooseBackUpPath())
            {
                ArrayList these = new ArrayList();

                foreach (ListViewItem chosen in detectedList.SelectedItems)
                {
                    these.Add(chosen.SubItems[2].Text);
                }

                taskForm backup = new taskForm("backup", these, settings, all_users_mode);
                backup.ShowDialog(this);

                populateRestoreList();
            }
        }
        private void disableSelectedGames() {
        }


        // Event handlers
        // Detected Game List Context Menu
        private void gamesContext_Opening(object sender, CancelEventArgs e)
        {
            if(detectedList.SelectedItems.Count!=0) {
                backThisUpToolStripMenuItem.Visible = true;
                disableToolStripMenuItem.Visible = true;
                purgeToolStripMenuItem.Visible = true;
                toolStripSeparator1.Visible = true;
                addPathToolStripMenuItem.Visible = false;
                removePathToolStripMenuItem.Visible = false;
                if (detectedList.SelectedItems.Count > 1){
                    bool show_enable = false, show_disable = false;
                    foreach(ListViewItem check_me in detectedList.SelectedItems) {
                        if(settings.games[check_me.SubItems[2].Text].disabled) {
                            show_enable = true;
                        } else {
                            show_disable = true;
                        }
                    }
                    if(show_disable)
                        disableToolStripMenuItem.Visible = true;
                    else
                        disableToolStripMenuItem.Visible = false;
                    if (show_enable)
                        enableToolStripMenuItem.Visible = true;
                    else
                        enableToolStripMenuItem.Visible = false;

                    backThisUpToolStripMenuItem.Text = "Back These Up";
                    createArchiveToolStripMenuItem.Visible = false;
                } else {
                    if(settings.games[detectedList.SelectedItems[0].SubItems[2].Text].disabled) {
                        disableToolStripMenuItem.Visible = false;
                        enableToolStripMenuItem.Visible = true;
                    } else {
                        disableToolStripMenuItem.Visible = true;
                        enableToolStripMenuItem.Visible = false;
                    }
                    backThisUpToolStripMenuItem.Text = "Back This Up";
                    //addPathToolStripMenuItem.Visible = true;
					//ArrayList manual_paths = settings.getManualPaths(detectedList.SelectedItems[0].SubItems[2].Text);
					//if(manual_paths.Count>0) {
					//    removePathToolStripMenuItem.Visible = true;
					//} else {
					//    removePathToolStripMenuItem.Visible = false;
					//}
                    createArchiveToolStripMenuItem.Visible = true;
                }
            } else {
                backThisUpToolStripMenuItem.Visible = false;
                addPathToolStripMenuItem.Visible = false;
                removePathToolStripMenuItem.Visible = false;
                disableToolStripMenuItem.Visible = false;
                purgeToolStripMenuItem.Visible = false;
                toolStripSeparator1.Visible = false;
                createArchiveToolStripMenuItem.Visible = false;
            }
        }

        private void backThisUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backupSelectedGames();
        }
        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem chosen in detectedList.SelectedItems){
                if(chosen.ForeColor==Color.Gray)
                    chosen.ForeColor=Color.Salmon;
                if(chosen.ForeColor==Color.Black)
                    chosen.ForeColor=Color.Red;
                if (!settings.disabled_games.Contains(chosen.SubItems[2].Text)) {
                    settings.disabled_games.Add(chosen.SubItems[2].Text);
                    settings.games[chosen.SubItems[2].Text].disabled = true;
                    settings.writeConfig();
                }
            }

        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem chosen in detectedList.SelectedItems){
                if(chosen.ForeColor==Color.Salmon)
                    chosen.ForeColor=Color.Gray;
                if(chosen.ForeColor==Color.Red)
                    chosen.ForeColor=Color.Black;
                if (settings.disabled_games.Contains(chosen.SubItems[2].Text)) {
                    settings.disabled_games.Remove(chosen.SubItems[2].Text);
                    settings.games[chosen.SubItems[2].Text].disabled = false;
                    settings.writeConfig();
                }
            }

        }
        private void purgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            purgeSelector select_root;
            if (MessageBox.Show(this, "Purging erases all detected save paths for the specified game\nAre you sure you want to continue?", "This could hurt.", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            foreach (ListViewItem chosen in detectedList.SelectedItems)
            {
                select_root = new purgeSelector(settings.games[chosen.SubItems[2].Text].detected_roots);
                if(select_root.purgeCombo.Items.Count>2) {
                    select_root.purgeCombo.SelectedIndex = 0;
                    if(select_root.ShowDialog(this)==DialogResult.Cancel)
                        break;
                }
                if(select_root.purgeCombo.SelectedIndex!=0){
                    try {
                        Directory.Delete(select_root.purgeCombo.SelectedItem.ToString(),true);
                    } catch {
                        MessageBox.Show(this,"Error while trying to delete this:\n" + select_root.purgeCombo.SelectedItem.ToString() + "\nYou probably don't have permission to do that.","I'm Just Not That Creative");
                    }
                } else {
                    foreach(file_holder delete_me in settings.games[chosen.SubItems[2].Text].detected_roots) {
                        try {
                            Directory.Delete(delete_me.absolute_path,true);
                        } catch {
                            MessageBox.Show(this, "Error while trying to delete this:\n" + delete_me.absolute_path + "\nYou probably don't have permission to do that.", "I'm Just Not That Creative");
                        }
                    }
                }

                
                
                
                select_root.Dispose();
            }
            detectGames();

        }

		private ArrayList getChecks(TreeNodeCollection nodes)
		{
			ArrayList return_me = new ArrayList();
			foreach (TreeNode node in nodes){
				if(node.Nodes.Count==0&&node.Checked)
					return_me.Add(node.FullPath);
				else 
					return_me.AddRange(getChecks(node.Nodes));
			}
			return return_me;
		}
		private void createArchiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GameData game = settings.games[detectedList.SelectedItems[0].SubItems[2].Text];
			manualBackup manual_backup = new manualBackup(game);
			if(manual_backup.ShowDialog(this)!=DialogResult.Cancel) {
				ArrayList selected_files = new ArrayList();
				selected_files.AddRange(getChecks(manual_backup.fileTree.Nodes));
				ArrayList back_these_up = new ArrayList();
				string file_path;
				foreach(file_holder file in game.getSaves()) {
					file_path = file.absolute_root;
					if(file.absolute_path!=null&&file.absolute_path!="")
						file_path += Path.DirectorySeparatorChar + file.absolute_path;
					file_path += Path.DirectorySeparatorChar + file.file_name;
					if(selected_files.Contains(file_path))
						back_these_up.Add(file);
				}
				if(back_these_up.Count>0) {
				    if(settings.backup_path!=null)
				        folderBrowser.SelectedPath = settings.backup_path;

					folderBrowser.Description = "And where would you like to put this backup??";
					if(folderBrowser.ShowDialog(this).ToString()!="Cancel") {
						ArrayList one_game = new ArrayList();
						taskForm backup = new taskForm(detectedList.SelectedItems[0].SubItems[2].Text,settings,all_users_mode, back_these_up,folderBrowser.SelectedPath);
						backup.ShowDialog(this);
						populateRestoreList();
					}
				}
				Console.WriteLine(manual_backup.fileTree.SelectedNode.FullPath);
			}
		}

								// Maybe someday...

		//private void overridePathToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//    string game_name = detectedList.SelectedItems[0].SubItems[2].Text;
		//    if(settings.games[game_name].detected_roots.Count>0)
		//        folderBrowser.SelectedPath = settings.games[game_name].detected_roots[0].ToString();

		//    folderBrowser.Description = "Alright smarty-pants, where do you think this game keeps it's saves?";
		//    if(folderBrowser.ShowDialog(this).ToString()!="Cancel") {
		//        settings.addManualPath(game_name,folderBrowser.SelectedPath);
		//        settings.writeConfig();
		//    }
		//}
		//private void removePathToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//    pathSelector select_path = new pathSelector(settings.getManualPaths(detectedList.SelectedItems[0].SubItems[2].Text));
		//    if(select_path.ShowDialog(this)!=DialogResult.Cancel) {
		//        if(select_path.pathCombo.SelectedIndex==0) {
		//            settings.removeAllManualPaths(detectedList.SelectedItems[0].SubItems[2].Text);
		//        } else {
		//            settings.removeManualPath(detectedList.SelectedItems[0].SubItems[2].Text,select_path.pathCombo.SelectedItem.ToString());
		//        }
		//        settings.writeConfig();
		//    }
		//}


        private void redetectGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detectGames();
        }

        // Backup buttons
        private void backupSelection_Click(object sender, EventArgs e)
        {
            backupSelectedGames();
        }


        // Settings Tab Stuff
        // Set the backup path
        private void backupPathButton_Click(object sender, EventArgs e)
        {
            chooseBackUpPath();
        }
        // Open the backup path
        private void button2_Click_1(object sender, EventArgs e)
        {
            Process.Start("explorer", "\"" + backupPathInput.Text + "\"");
        }
        // Enable or disable the date check
        private void dateCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (dateCheck.Checked){
                settings.ignore_date_check = true;
            }else{
                settings.ignore_date_check = false;
            }
            settings.writeConfig();
        }
        // Enable or disable monitor autostart
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey startup_keys = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if(monitorCheck.Checked) {
                if (startup_keys.GetValue("MASGAUMonitor")==null)
                    startup_keys.SetValue("MASGAUMonitor", Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"MasgauMonitor.exe"));
            } else {
                startup_keys.DeleteValue("MASGAUMonitor", false);
            }
        }
        // Redetect Steam path
        private void steamClearButton_Click(object sender, EventArgs e)
        {
            settings.resetSteam();
            settings.writeConfig();
            settings.playstation = new playstationHandler();
            redetect_required = true;
        }
        // Manually set Steam path
        private void steamPathButton_Click(object sender, EventArgs e)
        {
            if(settings.steam.path!=null)
                folderBrowser.SelectedPath = settings.steam.path;
                folderBrowser.Description = "Choose where Steam is located.";
                if(folderBrowser.ShowDialog(this).ToString()!="Cancel") {
                settings.overrideSteam(folderBrowser.SelectedPath);
                settings.writeConfig();
                settings.playstation = new playstationHandler();
                redetect_required = true;
            }
        }
        // Changes the alt path remove button text and enability
        private void altPathList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (altPathList.SelectedItems.Count > 1){
                removePath.Enabled = true;
                removePath.Text = "Remove Paths";
            }
            else if (altPathList.SelectedItems.Count > 0){
                removePath.Enabled = true;
                removePath.Text = "Remove Path";
            }
            else{
                removePath.Enabled = false;
                removePath.Text = "Remove Nothing";
            }
        }
        // Removes alt paths
        private void button2_Click_2(object sender, EventArgs e)
        {
            foreach(ListViewItem remove_me in altPathList.SelectedItems) {
                if(settings.alt_paths.Contains(remove_me.Text))
                    settings.alt_paths.Remove(remove_me.Text);
            }

            settings.writeConfig();
            settings.playstation = new playstationHandler();
            redetect_required = true;   
            populateAltList();
        }
        // Adds alt paths
        private void button1_Click(object sender, EventArgs e) {
            folderBrowser.Description = "Choose an additional path to search for games under.";
            folderBrowser.SelectedPath = Environment.GetLogicalDrives()[0];
            if(folderBrowser.ShowDialog(this).ToString()!="Cancel") {
                settings.addAltPath(folderBrowser.SelectedPath);
                settings.writeConfig();
                settings.playstation = new playstationHandler();
                redetect_required = true;
                populateAltList();
            }
        }


        // On tab change, if something happened that necessitates redetecting the games, do it
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (redetect_required)
            {
                detectGames();
                redetect_required = false;
            }

        }


    }
}