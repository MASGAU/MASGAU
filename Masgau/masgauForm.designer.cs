namespace Masgau
{
    partial class masgauForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(masgauForm));
			this.gamesContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.createArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backThisUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.disableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.purgeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.redetectGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startBackup = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.backupTab = new System.Windows.Forms.TabPage();
			this.backupSelection = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.detectedList = new System.Windows.Forms.ListView();
			this.title = new System.Windows.Forms.ColumnHeader();
			this.platform = new System.Windows.Forms.ColumnHeader();
			this.name = new System.Windows.Forms.ColumnHeader();
			this.settingsTab = new System.Windows.Forms.TabPage();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.duplicateCountBox = new System.Windows.Forms.GroupBox();
			this.duplicateCount = new System.Windows.Forms.NumericUpDown();
			this.duplicateFrequencyBox = new System.Windows.Forms.GroupBox();
			this.duplicateFrequencyCombo = new System.Windows.Forms.ComboBox();
			this.duplicateFrequencyNumber = new System.Windows.Forms.NumericUpDown();
			this.versioningCheck = new System.Windows.Forms.CheckBox();
			this.monitorCheck = new System.Windows.Forms.CheckBox();
			this.dateCheck = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.removePath = new System.Windows.Forms.Button();
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
			this.restoreTab = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.restoreTree = new System.Windows.Forms.TreeView();
			this.restoreContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.scheduleTab = new System.Windows.Forms.TabPage();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.taskUser = new System.Windows.Forms.TextBox();
			this.taskPassword = new System.Windows.Forms.TextBox();
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
			this.aboutTab = new System.Windows.Forms.TabPage();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.altPathContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.deleteSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gamesContext.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.backupTab.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.settingsTab.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.duplicateCountBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.duplicateCount)).BeginInit();
			this.duplicateFrequencyBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.duplicateFrequencyNumber)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.restoreTab.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.scheduleTab.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox11.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.monthDay)).BeginInit();
			this.groupBox10.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.aboutTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.altPathContext.SuspendLayout();
			this.SuspendLayout();
			// 
			// gamesContext
			// 
			this.gamesContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createArchiveToolStripMenuItem,
            this.backThisUpToolStripMenuItem,
            this.enableToolStripMenuItem,
            this.disableToolStripMenuItem,
            this.addPathToolStripMenuItem,
            this.removePathToolStripMenuItem,
            this.purgeToolStripMenuItem,
            this.toolStripSeparator1,
            this.redetectGamesToolStripMenuItem});
			this.gamesContext.Name = "gamesContext";
			this.gamesContext.ShowImageMargin = false;
			this.gamesContext.Size = new System.Drawing.Size(172, 186);
			this.gamesContext.Opening += new System.ComponentModel.CancelEventHandler(this.gamesContext_Opening);
			// 
			// createArchiveToolStripMenuItem
			// 
			this.createArchiveToolStripMenuItem.Name = "createArchiveToolStripMenuItem";
			this.createArchiveToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.createArchiveToolStripMenuItem.Text = "Create Custom Archive";
			this.createArchiveToolStripMenuItem.Click += new System.EventHandler(this.createArchiveToolStripMenuItem_Click);
			// 
			// backThisUpToolStripMenuItem
			// 
			this.backThisUpToolStripMenuItem.Name = "backThisUpToolStripMenuItem";
			this.backThisUpToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.backThisUpToolStripMenuItem.Text = "Back Nothing Up";
			this.backThisUpToolStripMenuItem.Click += new System.EventHandler(this.backThisUpToolStripMenuItem_Click);
			// 
			// enableToolStripMenuItem
			// 
			this.enableToolStripMenuItem.Name = "enableToolStripMenuItem";
			this.enableToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.enableToolStripMenuItem.Text = "Enable";
			this.enableToolStripMenuItem.Click += new System.EventHandler(this.enableToolStripMenuItem_Click);
			// 
			// disableToolStripMenuItem
			// 
			this.disableToolStripMenuItem.Name = "disableToolStripMenuItem";
			this.disableToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.disableToolStripMenuItem.Text = "Disable";
			this.disableToolStripMenuItem.Click += new System.EventHandler(this.disableToolStripMenuItem_Click);
			// 
			// addPathToolStripMenuItem
			// 
			this.addPathToolStripMenuItem.Name = "addPathToolStripMenuItem";
			this.addPathToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.addPathToolStripMenuItem.Text = "Add Manual Path";
			// 
			// removePathToolStripMenuItem
			// 
			this.removePathToolStripMenuItem.Name = "removePathToolStripMenuItem";
			this.removePathToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.removePathToolStripMenuItem.Text = "Remove Manual Path";
			// 
			// purgeToolStripMenuItem
			// 
			this.purgeToolStripMenuItem.Name = "purgeToolStripMenuItem";
			this.purgeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.purgeToolStripMenuItem.Text = "Purge";
			this.purgeToolStripMenuItem.Click += new System.EventHandler(this.purgeToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(168, 6);
			// 
			// redetectGamesToolStripMenuItem
			// 
			this.redetectGamesToolStripMenuItem.Name = "redetectGamesToolStripMenuItem";
			this.redetectGamesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.redetectGamesToolStripMenuItem.Text = "Redetect Games";
			this.redetectGamesToolStripMenuItem.Click += new System.EventHandler(this.redetectGamesToolStripMenuItem_Click);
			// 
			// startBackup
			// 
			this.startBackup.Location = new System.Drawing.Point(222, 265);
			this.startBackup.Margin = new System.Windows.Forms.Padding(2);
			this.startBackup.Name = "startBackup";
			this.startBackup.Size = new System.Drawing.Size(216, 23);
			this.startBackup.TabIndex = 2;
			this.startBackup.Text = "Back Everything Up";
			this.startBackup.UseVisualStyleBackColor = true;
			this.startBackup.Click += new System.EventHandler(this.startBackup_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.backupTab);
			this.tabControl1.Controls.Add(this.settingsTab);
			this.tabControl1.Controls.Add(this.restoreTab);
			this.tabControl1.Controls.Add(this.scheduleTab);
			this.tabControl1.Controls.Add(this.aboutTab);
			this.tabControl1.Location = new System.Drawing.Point(-1, -1);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(1);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(450, 319);
			this.tabControl1.TabIndex = 4;
			this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
			// 
			// backupTab
			// 
			this.backupTab.Controls.Add(this.backupSelection);
			this.backupTab.Controls.Add(this.startBackup);
			this.backupTab.Controls.Add(this.groupBox5);
			this.backupTab.Location = new System.Drawing.Point(4, 22);
			this.backupTab.Margin = new System.Windows.Forms.Padding(2);
			this.backupTab.Name = "backupTab";
			this.backupTab.Padding = new System.Windows.Forms.Padding(2);
			this.backupTab.Size = new System.Drawing.Size(442, 293);
			this.backupTab.TabIndex = 0;
			this.backupTab.Text = "Backup";
			this.backupTab.UseVisualStyleBackColor = true;
			// 
			// backupSelection
			// 
			this.backupSelection.Enabled = false;
			this.backupSelection.Location = new System.Drawing.Point(5, 265);
			this.backupSelection.Name = "backupSelection";
			this.backupSelection.Size = new System.Drawing.Size(216, 23);
			this.backupSelection.TabIndex = 4;
			this.backupSelection.Text = "Back Nothing Up";
			this.backupSelection.UseVisualStyleBackColor = true;
			this.backupSelection.Click += new System.EventHandler(this.backupSelection_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.detectedList);
			this.groupBox5.Location = new System.Drawing.Point(5, 5);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(429, 255);
			this.groupBox5.TabIndex = 3;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Detected Games (Hover Mouse For Detected Paths)";
			// 
			// detectedList
			// 
			this.detectedList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.title,
            this.platform,
            this.name});
			this.detectedList.ContextMenuStrip = this.gamesContext;
			this.detectedList.FullRowSelect = true;
			this.detectedList.HideSelection = false;
			this.detectedList.Location = new System.Drawing.Point(6, 19);
			this.detectedList.Name = "detectedList";
			this.detectedList.ShowItemToolTips = true;
			this.detectedList.Size = new System.Drawing.Size(417, 230);
			this.detectedList.TabIndex = 2;
			this.detectedList.UseCompatibleStateImageBehavior = false;
			this.detectedList.View = System.Windows.Forms.View.Details;
			this.detectedList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.detectedList_ColumnClick);
			this.detectedList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.detectedList_ItemSelectionChanged);
			// 
			// title
			// 
			this.title.DisplayIndex = 1;
			this.title.Text = "Game";
			this.title.Width = 335;
			// 
			// platform
			// 
			this.platform.DisplayIndex = 2;
			this.platform.Text = "Platform";
			// 
			// name
			// 
			this.name.DisplayIndex = 0;
			this.name.Text = "Secret Name! Ssssh!";
			this.name.Width = 0;
			// 
			// settingsTab
			// 
			this.settingsTab.Controls.Add(this.groupBox9);
			this.settingsTab.Controls.Add(this.monitorCheck);
			this.settingsTab.Controls.Add(this.dateCheck);
			this.settingsTab.Controls.Add(this.groupBox3);
			this.settingsTab.Controls.Add(this.groupBox2);
			this.settingsTab.Controls.Add(this.groupBox1);
			this.settingsTab.Location = new System.Drawing.Point(4, 22);
			this.settingsTab.Margin = new System.Windows.Forms.Padding(2);
			this.settingsTab.Name = "settingsTab";
			this.settingsTab.Padding = new System.Windows.Forms.Padding(2);
			this.settingsTab.Size = new System.Drawing.Size(442, 293);
			this.settingsTab.TabIndex = 1;
			this.settingsTab.Text = "Settings";
			this.settingsTab.UseVisualStyleBackColor = true;
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.duplicateCountBox);
			this.groupBox9.Controls.Add(this.duplicateFrequencyBox);
			this.groupBox9.Controls.Add(this.versioningCheck);
			this.groupBox9.Location = new System.Drawing.Point(234, 145);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(203, 136);
			this.groupBox9.TabIndex = 7;
			this.groupBox9.TabStop = false;
			// 
			// duplicateCountBox
			// 
			this.duplicateCountBox.Controls.Add(this.duplicateCount);
			this.duplicateCountBox.Location = new System.Drawing.Point(6, 81);
			this.duplicateCountBox.Name = "duplicateCountBox";
			this.duplicateCountBox.Size = new System.Drawing.Size(190, 49);
			this.duplicateCountBox.TabIndex = 2;
			this.duplicateCountBox.TabStop = false;
			this.duplicateCountBox.Text = "At Most This Many Copies";
			// 
			// duplicateCount
			// 
			this.duplicateCount.Location = new System.Drawing.Point(7, 17);
			this.duplicateCount.Maximum = new decimal(new int[] {
            276447231,
            23283,
            0,
            0});
			this.duplicateCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.duplicateCount.Name = "duplicateCount";
			this.duplicateCount.Size = new System.Drawing.Size(177, 20);
			this.duplicateCount.TabIndex = 0;
			this.duplicateCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.duplicateCount.ValueChanged += new System.EventHandler(this.duplicateCount_ValueChanged);
			// 
			// duplicateFrequencyBox
			// 
			this.duplicateFrequencyBox.Controls.Add(this.duplicateFrequencyCombo);
			this.duplicateFrequencyBox.Controls.Add(this.duplicateFrequencyNumber);
			this.duplicateFrequencyBox.Location = new System.Drawing.Point(6, 29);
			this.duplicateFrequencyBox.Name = "duplicateFrequencyBox";
			this.duplicateFrequencyBox.Size = new System.Drawing.Size(190, 49);
			this.duplicateFrequencyBox.TabIndex = 1;
			this.duplicateFrequencyBox.TabStop = false;
			this.duplicateFrequencyBox.Text = "At Least This Long Between Copies";
			// 
			// duplicateFrequencyCombo
			// 
			this.duplicateFrequencyCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.duplicateFrequencyCombo.FormattingEnabled = true;
			this.duplicateFrequencyCombo.Items.AddRange(new object[] {
            "Seconds",
            "Minutes",
            "Hours",
            "Days",
            "Weeks",
            "Months",
            "Years",
            "Decades",
            "Centuries",
            "Millenia"});
			this.duplicateFrequencyCombo.Location = new System.Drawing.Point(102, 19);
			this.duplicateFrequencyCombo.Name = "duplicateFrequencyCombo";
			this.duplicateFrequencyCombo.Size = new System.Drawing.Size(82, 21);
			this.duplicateFrequencyCombo.TabIndex = 1;
			this.duplicateFrequencyCombo.SelectedIndexChanged += new System.EventHandler(this.duplicateFrequencyCombo_SelectedIndexChanged);
			// 
			// duplicateFrequencyNumber
			// 
			this.duplicateFrequencyNumber.Location = new System.Drawing.Point(6, 19);
			this.duplicateFrequencyNumber.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
			this.duplicateFrequencyNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.duplicateFrequencyNumber.Name = "duplicateFrequencyNumber";
			this.duplicateFrequencyNumber.Size = new System.Drawing.Size(90, 20);
			this.duplicateFrequencyNumber.TabIndex = 0;
			this.duplicateFrequencyNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.duplicateFrequencyNumber.ValueChanged += new System.EventHandler(this.duplicateFrequencyNumber_ValueChanged);
			// 
			// versioningCheck
			// 
			this.versioningCheck.Appearance = System.Windows.Forms.Appearance.Button;
			this.versioningCheck.AutoSize = true;
			this.versioningCheck.BackColor = System.Drawing.Color.Transparent;
			this.versioningCheck.Location = new System.Drawing.Point(43, 0);
			this.versioningCheck.Name = "versioningCheck";
			this.versioningCheck.Size = new System.Drawing.Size(116, 23);
			this.versioningCheck.TabIndex = 0;
			this.versioningCheck.Text = "Make Extra Backups";
			this.versioningCheck.UseVisualStyleBackColor = false;
			this.versioningCheck.CheckedChanged += new System.EventHandler(this.versioningCheck_CheckedChanged);
			// 
			// monitorCheck
			// 
			this.monitorCheck.AutoSize = true;
			this.monitorCheck.Location = new System.Drawing.Point(224, 15);
			this.monitorCheck.Name = "monitorCheck";
			this.monitorCheck.Size = new System.Drawing.Size(181, 17);
			this.monitorCheck.TabIndex = 6;
			this.monitorCheck.Text = "Start MASGAU Monitor On Login";
			this.monitorCheck.UseVisualStyleBackColor = true;
			this.monitorCheck.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// dateCheck
			// 
			this.dateCheck.AutoSize = true;
			this.dateCheck.Location = new System.Drawing.Point(34, 15);
			this.dateCheck.Name = "dateCheck";
			this.dateCheck.Size = new System.Drawing.Size(161, 17);
			this.dateCheck.TabIndex = 5;
			this.dateCheck.Text = "Ignore Dates During Backup";
			this.dateCheck.UseVisualStyleBackColor = true;
			this.dateCheck.CheckedChanged += new System.EventHandler(this.dateCheck_CheckedChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.removePath);
			this.groupBox3.Controls.Add(this.button1);
			this.groupBox3.Controls.Add(this.altPathList);
			this.groupBox3.Location = new System.Drawing.Point(5, 145);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(223, 136);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Alternate Install Paths";
			// 
			// removePath
			// 
			this.removePath.Enabled = false;
			this.removePath.Location = new System.Drawing.Point(6, 107);
			this.removePath.Name = "removePath";
			this.removePath.Size = new System.Drawing.Size(101, 23);
			this.removePath.TabIndex = 8;
			this.removePath.Text = "Remove Nothing";
			this.removePath.UseVisualStyleBackColor = true;
			this.removePath.Click += new System.EventHandler(this.button2_Click_2);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(113, 107);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(104, 23);
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
			this.altPathList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.altPathList.HideSelection = false;
			this.altPathList.Location = new System.Drawing.Point(6, 20);
			this.altPathList.Name = "altPathList";
			this.altPathList.ShowItemToolTips = true;
			this.altPathList.Size = new System.Drawing.Size(211, 81);
			this.altPathList.TabIndex = 0;
			this.altPathList.UseCompatibleStateImageBehavior = false;
			this.altPathList.View = System.Windows.Forms.View.Details;
			this.altPathList.SelectedIndexChanged += new System.EventHandler(this.altPathList_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Path";
			this.columnHeader1.Width = 200;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.openBackupPath);
			this.groupBox2.Controls.Add(this.backupPathInput);
			this.groupBox2.Controls.Add(this.backupPathButton);
			this.groupBox2.Location = new System.Drawing.Point(4, 45);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox2.Size = new System.Drawing.Size(432, 44);
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
			this.groupBox1.Location = new System.Drawing.Point(5, 93);
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
			this.toolTip1.SetToolTip(this.steamClearButton, "This will cause MASGAU to attempt to re-detect Steam.");
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
			// restoreTab
			// 
			this.restoreTab.Controls.Add(this.groupBox4);
			this.restoreTab.Location = new System.Drawing.Point(4, 22);
			this.restoreTab.Name = "restoreTab";
			this.restoreTab.Padding = new System.Windows.Forms.Padding(3);
			this.restoreTab.Size = new System.Drawing.Size(442, 293);
			this.restoreTab.TabIndex = 2;
			this.restoreTab.Text = "Restore";
			this.restoreTab.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.restoreTree);
			this.groupBox4.Location = new System.Drawing.Point(4, 6);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(433, 281);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Double-click a backup to restore it";
			// 
			// restoreTree
			// 
			this.restoreTree.ContextMenuStrip = this.restoreContext;
			this.restoreTree.FullRowSelect = true;
			this.restoreTree.Location = new System.Drawing.Point(6, 19);
			this.restoreTree.Name = "restoreTree";
			this.restoreTree.Size = new System.Drawing.Size(419, 256);
			this.restoreTree.TabIndex = 0;
			this.restoreTree.DoubleClick += new System.EventHandler(this.restoreTree_DoubleClick);
			// 
			// restoreContext
			// 
			this.restoreContext.Name = "contextMenuStrip1";
			this.restoreContext.Size = new System.Drawing.Size(61, 4);
			// 
			// scheduleTab
			// 
			this.scheduleTab.Controls.Add(this.groupBox7);
			this.scheduleTab.Controls.Add(this.groupBox8);
			this.scheduleTab.Controls.Add(this.groupBox11);
			this.scheduleTab.Controls.Add(this.groupBox10);
			this.scheduleTab.Controls.Add(this.deleteTask);
			this.scheduleTab.Controls.Add(this.groupBox6);
			this.scheduleTab.Controls.Add(this.taskApply);
			this.scheduleTab.Location = new System.Drawing.Point(4, 22);
			this.scheduleTab.Name = "scheduleTab";
			this.scheduleTab.Size = new System.Drawing.Size(442, 293);
			this.scheduleTab.TabIndex = 3;
			this.scheduleTab.Text = "Schedule";
			this.scheduleTab.UseVisualStyleBackColor = true;
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.taskUser);
			this.groupBox7.Controls.Add(this.taskPassword);
			this.groupBox7.Location = new System.Drawing.Point(8, 212);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(422, 47);
			this.groupBox7.TabIndex = 11;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "User And Password To Run Task As";
			// 
			// taskUser
			// 
			this.taskUser.Enabled = false;
			this.taskUser.Location = new System.Drawing.Point(6, 19);
			this.taskUser.Name = "taskUser";
			this.taskUser.Size = new System.Drawing.Size(204, 20);
			this.taskUser.TabIndex = 1;
			// 
			// taskPassword
			// 
			this.taskPassword.Location = new System.Drawing.Point(212, 19);
			this.taskPassword.Name = "taskPassword";
			this.taskPassword.PasswordChar = '§';
			this.taskPassword.Size = new System.Drawing.Size(204, 20);
			this.taskPassword.TabIndex = 0;
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.weekDay);
			this.groupBox8.Location = new System.Drawing.Point(8, 107);
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
			this.groupBox11.Location = new System.Drawing.Point(8, 161);
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
			this.groupBox10.Location = new System.Drawing.Point(8, 56);
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
			this.groupBox6.Location = new System.Drawing.Point(8, 5);
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
			this.taskApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.taskApply.Location = new System.Drawing.Point(355, 264);
			this.taskApply.Name = "taskApply";
			this.taskApply.Size = new System.Drawing.Size(75, 23);
			this.taskApply.TabIndex = 0;
			this.taskApply.Text = "Apply";
			this.taskApply.UseVisualStyleBackColor = true;
			this.taskApply.Click += new System.EventHandler(this.button2_Click);
			// 
			// aboutTab
			// 
			this.aboutTab.Controls.Add(this.pictureBox1);
			this.aboutTab.Controls.Add(this.linkLabel1);
			this.aboutTab.Controls.Add(this.label1);
			this.aboutTab.Location = new System.Drawing.Point(4, 22);
			this.aboutTab.Name = "aboutTab";
			this.aboutTab.Padding = new System.Windows.Forms.Padding(3);
			this.aboutTab.Size = new System.Drawing.Size(442, 293);
			this.aboutTab.TabIndex = 4;
			this.aboutTab.Text = "About";
			this.aboutTab.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackgroundImage = global::Masgau.Properties.Resources.masgau_wallpaper;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBox1.Location = new System.Drawing.Point(65, 51);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(305, 72);
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox1, "Who do you think you are?");
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(140, 244);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(151, 13);
			this.linkLabel1.TabIndex = 3;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "http://masga.sourceforge.net/";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(21, 182);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(395, 19);
			this.label1.TabIndex = 1;
			this.label1.Text = "MASGAU Automatic Save Game Archive Utility v.0.4";
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.refreshToolStripMenuItem.Text = "Refresh";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
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
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 0;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 0;
			// 
			// deleteSelectionToolStripMenuItem
			// 
			this.deleteSelectionToolStripMenuItem.Name = "deleteSelectionToolStripMenuItem";
			this.deleteSelectionToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.deleteSelectionToolStripMenuItem.Text = "Delete Selection";
			// 
			// masgauForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(446, 314);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.Name = "masgauForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MASGAU";
			this.gamesContext.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.backupTab.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.settingsTab.ResumeLayout(false);
			this.settingsTab.PerformLayout();
			this.groupBox9.ResumeLayout(false);
			this.groupBox9.PerformLayout();
			this.duplicateCountBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.duplicateCount)).EndInit();
			this.duplicateFrequencyBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.duplicateFrequencyNumber)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.restoreTab.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.scheduleTab.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			this.groupBox8.ResumeLayout(false);
			this.groupBox11.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.monthDay)).EndInit();
			this.groupBox10.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.aboutTab.ResumeLayout(false);
			this.aboutTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.altPathContext.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startBackup;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage backupTab;
        private System.Windows.Forms.TabPage settingsTab;
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
        private System.Windows.Forms.TabPage restoreTab;
        private System.Windows.Forms.TabPage scheduleTab;
        private System.Windows.Forms.Button steamClearButton;
        private System.Windows.Forms.TreeView restoreTree;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TabPage aboutTab;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip restoreContext;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button taskApply;
        private System.Windows.Forms.ComboBox taskFrequency;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ContextMenuStrip gamesContext;
        private System.Windows.Forms.ContextMenuStrip altPathContext;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.Button deleteTask;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.DateTimePicker timeOfDay;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ComboBox weekDay;
        private System.Windows.Forms.NumericUpDown monthDay;
		private System.Windows.Forms.Button openBackupPath;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button backupSelection;
        private System.Windows.Forms.ListView detectedList;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox taskPassword;
        private System.Windows.Forms.CheckBox dateCheck;
        private System.Windows.Forms.TextBox taskUser;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader title;
        private System.Windows.Forms.ColumnHeader platform;
        private System.Windows.Forms.CheckBox monitorCheck;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox duplicateCountBox;
        private System.Windows.Forms.GroupBox duplicateFrequencyBox;
        private System.Windows.Forms.CheckBox versioningCheck;
        private System.Windows.Forms.NumericUpDown duplicateCount;
        private System.Windows.Forms.ComboBox duplicateFrequencyCombo;
        private System.Windows.Forms.NumericUpDown duplicateFrequencyNumber;
        private System.Windows.Forms.Button removePath;
        private System.Windows.Forms.ToolStripMenuItem purgeToolStripMenuItem;
		private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem addPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backThisUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem redetectGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createArchiveToolStripMenuItem;
    }
}

