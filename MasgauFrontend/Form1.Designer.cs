namespace Masgau
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.game_list = new System.Windows.Forms.ListView();
            this.gamesContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.startBackup = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.altPathList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openBackupPath = new System.Windows.Forms.Button();
            this.backupPathInput = new System.Windows.Forms.TextBox();
            this.backupPathButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.steamClearButton = new System.Windows.Forms.Button();
            this.steamPathInput = new System.Windows.Forms.TextBox();
            this.steamPathButton = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.restoreTree = new System.Windows.Forms.TreeView();
            this.restoreContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.weekDay = new System.Windows.Forms.ComboBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.monthDay = new System.Windows.Forms.NumericUpDown();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.timeOfDay = new System.Windows.Forms.DateTimePicker();
            this.deleteTask = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.taskFrequency = new System.Windows.Forms.ComboBox();
            this.taskApply = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.altPathContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gamesContext.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.restoreContext.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.monthDay)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.altPathContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // game_list
            // 
            this.game_list.ContextMenuStrip = this.gamesContext;
            this.game_list.FullRowSelect = true;
            this.game_list.Location = new System.Drawing.Point(5, 18);
            this.game_list.Margin = new System.Windows.Forms.Padding(2);
            this.game_list.Name = "game_list";
            this.game_list.Size = new System.Drawing.Size(419, 232);
            this.game_list.TabIndex = 0;
            this.game_list.UseCompatibleStateImageBehavior = false;
            this.game_list.View = System.Windows.Forms.View.Details;
            this.game_list.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.game_list_MouseDoubleClick);
            // 
            // gamesContext
            // 
            this.gamesContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem1});
            this.gamesContext.Name = "gamesContext";
            this.gamesContext.Size = new System.Drawing.Size(114, 26);
            // 
            // refreshToolStripMenuItem1
            // 
            this.refreshToolStripMenuItem1.Name = "refreshToolStripMenuItem1";
            this.refreshToolStripMenuItem1.Size = new System.Drawing.Size(113, 22);
            this.refreshToolStripMenuItem1.Text = "Refresh";
            this.refreshToolStripMenuItem1.Click += new System.EventHandler(this.refreshToolStripMenuItem1_Click);
            // 
            // startBackup
            // 
            this.startBackup.Location = new System.Drawing.Point(7, 265);
            this.startBackup.Margin = new System.Windows.Forms.Padding(2);
            this.startBackup.Name = "startBackup";
            this.startBackup.Size = new System.Drawing.Size(422, 23);
            this.startBackup.TabIndex = 2;
            this.startBackup.Text = "Back Them All Up";
            this.startBackup.UseVisualStyleBackColor = true;
            this.startBackup.Click += new System.EventHandler(this.startBackup_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(450, 323);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.startBackup);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(442, 297);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Backup";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.game_list);
            this.groupBox5.Location = new System.Drawing.Point(5, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(429, 255);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Detected Games (Double-click to back up a specific game)";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(442, 297);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.altPathList);
            this.groupBox3.Location = new System.Drawing.Point(5, 102);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(432, 189);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Alternate Install Paths (Double-click to remove)";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 160);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(420, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add New Path!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // altPathList
            // 
            this.altPathList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.altPathList.FullRowSelect = true;
            this.altPathList.Location = new System.Drawing.Point(6, 20);
            this.altPathList.Name = "altPathList";
            this.altPathList.Size = new System.Drawing.Size(420, 134);
            this.altPathList.TabIndex = 0;
            this.altPathList.UseCompatibleStateImageBehavior = false;
            this.altPathList.View = System.Windows.Forms.View.Details;
            this.altPathList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.altPathList_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Path";
            this.columnHeader1.Width = 400;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.openBackupPath);
            this.groupBox2.Controls.Add(this.backupPathInput);
            this.groupBox2.Controls.Add(this.backupPathButton);
            this.groupBox2.Location = new System.Drawing.Point(6, 4);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(432, 42);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Backup Path";
            // 
            // openBackupPath
            // 
            this.openBackupPath.Enabled = false;
            this.openBackupPath.Location = new System.Drawing.Point(322, 15);
            this.openBackupPath.Name = "openBackupPath";
            this.openBackupPath.Size = new System.Drawing.Size(43, 23);
            this.openBackupPath.TabIndex = 2;
            this.openBackupPath.Text = "Open";
            this.openBackupPath.UseVisualStyleBackColor = true;
            this.openBackupPath.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // backupPathInput
            // 
            this.backupPathInput.Enabled = false;
            this.backupPathInput.Location = new System.Drawing.Point(6, 17);
            this.backupPathInput.Margin = new System.Windows.Forms.Padding(2);
            this.backupPathInput.Name = "backupPathInput";
            this.backupPathInput.Size = new System.Drawing.Size(311, 20);
            this.backupPathInput.TabIndex = 1;
            // 
            // backupPathButton
            // 
            this.backupPathButton.Location = new System.Drawing.Point(370, 15);
            this.backupPathButton.Margin = new System.Windows.Forms.Padding(2);
            this.backupPathButton.Name = "backupPathButton";
            this.backupPathButton.Size = new System.Drawing.Size(56, 23);
            this.backupPathButton.TabIndex = 0;
            this.backupPathButton.Text = "Change";
            this.backupPathButton.UseVisualStyleBackColor = true;
            this.backupPathButton.Click += new System.EventHandler(this.backupPathButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.steamClearButton);
            this.groupBox1.Controls.Add(this.steamPathInput);
            this.groupBox1.Controls.Add(this.steamPathButton);
            this.groupBox1.Location = new System.Drawing.Point(6, 50);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(432, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Steam Path";
            // 
            // steamClearButton
            // 
            this.steamClearButton.Location = new System.Drawing.Point(382, 15);
            this.steamClearButton.Name = "steamClearButton";
            this.steamClearButton.Size = new System.Drawing.Size(44, 23);
            this.steamClearButton.TabIndex = 2;
            this.steamClearButton.Text = "Reset";
            this.steamClearButton.UseVisualStyleBackColor = true;
            this.steamClearButton.Click += new System.EventHandler(this.steamClearButton_Click);
            // 
            // steamPathInput
            // 
            this.steamPathInput.Enabled = false;
            this.steamPathInput.Location = new System.Drawing.Point(6, 17);
            this.steamPathInput.Margin = new System.Windows.Forms.Padding(2);
            this.steamPathInput.Name = "steamPathInput";
            this.steamPathInput.Size = new System.Drawing.Size(311, 20);
            this.steamPathInput.TabIndex = 1;
            // 
            // steamPathButton
            // 
            this.steamPathButton.Location = new System.Drawing.Point(321, 15);
            this.steamPathButton.Margin = new System.Windows.Forms.Padding(2);
            this.steamPathButton.Name = "steamPathButton";
            this.steamPathButton.Size = new System.Drawing.Size(56, 23);
            this.steamPathButton.TabIndex = 0;
            this.steamPathButton.Text = "Change";
            this.steamPathButton.UseVisualStyleBackColor = true;
            this.steamPathButton.Click += new System.EventHandler(this.steamPathButton_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(442, 297);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Restore";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.restoreTree);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(433, 287);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Double-click a backup to restore it";
            // 
            // restoreTree
            // 
            this.restoreTree.ContextMenuStrip = this.restoreContext;
            this.restoreTree.FullRowSelect = true;
            this.restoreTree.Location = new System.Drawing.Point(2, 19);
            this.restoreTree.Name = "restoreTree";
            this.restoreTree.Size = new System.Drawing.Size(426, 262);
            this.restoreTree.TabIndex = 0;
            this.restoreTree.DoubleClick += new System.EventHandler(this.restoreTree_DoubleClick);
            // 
            // restoreContext
            // 
            this.restoreContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.restoreContext.Name = "contextMenuStrip1";
            this.restoreContext.Size = new System.Drawing.Size(114, 26);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox8);
            this.tabPage4.Controls.Add(this.groupBox11);
            this.tabPage4.Controls.Add(this.groupBox10);
            this.tabPage4.Controls.Add(this.deleteTask);
            this.tabPage4.Controls.Add(this.groupBox6);
            this.tabPage4.Controls.Add(this.taskApply);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(442, 297);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Schedule";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.weekDay);
            this.groupBox8.Location = new System.Drawing.Point(8, 117);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(422, 49);
            this.groupBox8.TabIndex = 11;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Day of the Week";
            // 
            // weekDay
            // 
            this.weekDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.weekDay.FormattingEnabled = true;
            this.weekDay.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
            this.weekDay.Location = new System.Drawing.Point(6, 18);
            this.weekDay.Name = "weekDay";
            this.weekDay.Size = new System.Drawing.Size(410, 21);
            this.weekDay.TabIndex = 0;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.monthDay);
            this.groupBox11.Location = new System.Drawing.Point(8, 172);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(422, 47);
            this.groupBox11.TabIndex = 10;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Day of the Month";
            // 
            // monthDay
            // 
            this.monthDay.Location = new System.Drawing.Point(6, 19);
            this.monthDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.monthDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.monthDay.Name = "monthDay";
            this.monthDay.Size = new System.Drawing.Size(410, 20);
            this.monthDay.TabIndex = 0;
            this.monthDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.timeOfDay);
            this.groupBox10.Location = new System.Drawing.Point(8, 62);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(422, 49);
            this.groupBox10.TabIndex = 9;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Time of Day";
            // 
            // timeOfDay
            // 
            this.timeOfDay.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeOfDay.Location = new System.Drawing.Point(6, 19);
            this.timeOfDay.Name = "timeOfDay";
            this.timeOfDay.ShowUpDown = true;
            this.timeOfDay.Size = new System.Drawing.Size(410, 20);
            this.timeOfDay.TabIndex = 0;
            this.timeOfDay.Value = new System.DateTime(2009, 7, 5, 12, 0, 0, 0);
            // 
            // deleteTask
            // 
            this.deleteTask.Enabled = false;
            this.deleteTask.Location = new System.Drawing.Point(265, 264);
            this.deleteTask.Name = "deleteTask";
            this.deleteTask.Size = new System.Drawing.Size(84, 23);
            this.deleteTask.TabIndex = 8;
            this.deleteTask.Text = "Delete Task";
            this.deleteTask.UseVisualStyleBackColor = true;
            this.deleteTask.Click += new System.EventHandler(this.ableTask_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.taskFrequency);
            this.groupBox6.Location = new System.Drawing.Point(8, 7);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(422, 49);
            this.groupBox6.TabIndex = 3;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Frequency";
            // 
            // taskFrequency
            // 
            this.taskFrequency.DisplayMember = "Daily";
            this.taskFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.taskFrequency.FormattingEnabled = true;
            this.taskFrequency.Items.AddRange(new object[] {
            "Daily",
            "Weekly",
            "Monthly"});
            this.taskFrequency.Location = new System.Drawing.Point(6, 20);
            this.taskFrequency.Name = "taskFrequency";
            this.taskFrequency.Size = new System.Drawing.Size(410, 21);
            this.taskFrequency.TabIndex = 1;
            this.taskFrequency.SelectedIndexChanged += new System.EventHandler(this.taskFrequency_SelectedIndexChanged);
            // 
            // taskApply
            // 
            this.taskApply.Enabled = false;
            this.taskApply.Location = new System.Drawing.Point(355, 264);
            this.taskApply.Name = "taskApply";
            this.taskApply.Size = new System.Drawing.Size(75, 23);
            this.taskApply.TabIndex = 0;
            this.taskApply.Text = "Apply";
            this.taskApply.UseVisualStyleBackColor = true;
            this.taskApply.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.label2);
            this.tabPage5.Controls.Add(this.linkLabel1);
            this.tabPage5.Controls.Add(this.label1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(442, 297);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "About";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(161, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Pretty logo to go here";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(105, 247);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(237, 13);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://sourceforge.net/apps/mediawiki/masga/";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(398, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "MASGAU Automatic Save Game Archive Utility v.0.1";
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // altPathContext
            // 
            this.altPathContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.altPathContext.Name = "altPathContext";
            this.altPathContext.Size = new System.Drawing.Size(118, 26);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 321);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "MASGA";
            this.gamesContext.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.restoreContext.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.monthDay)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.altPathContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView game_list;
        private System.Windows.Forms.Button startBackup;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox steamPathInput;
        private System.Windows.Forms.Button steamPathButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox backupPathInput;
        private System.Windows.Forms.Button backupPathButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView altPathList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button steamClearButton;
        private System.Windows.Forms.TreeView restoreTree;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip restoreContext;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button taskApply;
        private System.Windows.Forms.ComboBox taskFrequency;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ContextMenuStrip gamesContext;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip altPathContext;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.Button deleteTask;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.DateTimePicker timeOfDay;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ComboBox weekDay;
        private System.Windows.Forms.NumericUpDown monthDay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button openBackupPath;
    }
}

