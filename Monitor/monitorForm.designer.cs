namespace MASGAU
{
    partial class monitorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(monitorForm));
            this.monitorNotifier = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifierMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rescanGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.watchedGamesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoCheckForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullBackupOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreDatesDuringBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMonitorOnLoginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.alternateInstallPathsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeBackupPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeSteamPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeExtraBackupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.setTimeBetweenCopiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setMaxNumberOfCopiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noGamesDetectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new wyDay.Controls.Windows7ProgressBar();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.notifierMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // monitorNotifier
            // 
            this.monitorNotifier.ContextMenuStrip = this.notifierMenu;
            this.monitorNotifier.Icon = ((System.Drawing.Icon)(resources.GetObject("monitorNotifier.Icon")));
            this.monitorNotifier.Text = "MASGAU Monitor";
            this.monitorNotifier.Visible = true;
            // 
            // notifierMenu
            // 
            this.notifierMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanGamesToolStripMenuItem,
            this.watchedGamesMenu,
            this.settingsToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.notifierMenu.Name = "notifierMenu";
            this.notifierMenu.ShowImageMargin = false;
            this.notifierMenu.Size = new System.Drawing.Size(206, 158);
            // 
            // rescanGamesToolStripMenuItem
            // 
            this.rescanGamesToolStripMenuItem.Name = "rescanGamesToolStripMenuItem";
            this.rescanGamesToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.rescanGamesToolStripMenuItem.Text = "Rescan Games...";
            this.rescanGamesToolStripMenuItem.Click += new System.EventHandler(this.rescanGamesToolStripMenuItem_Click);
            // 
            // watchedGamesMenu
            // 
            this.watchedGamesMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.watchedGamesMenu.Name = "watchedGamesMenu";
            this.watchedGamesMenu.Size = new System.Drawing.Size(205, 22);
            this.watchedGamesMenu.Text = "Watched Games";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Checked = true;
            this.settingsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoCheckForUpdatesToolStripMenuItem,
            this.fullBackupOnStartupToolStripMenuItem,
            this.ignoreDatesDuringBackupToolStripMenuItem,
            this.startMonitorOnLoginToolStripMenuItem,
            this.toolStripSeparator1,
            this.alternateInstallPathsToolStripMenuItem,
            this.changeBackupPathToolStripMenuItem,
            this.changeSteamPathToolStripMenuItem,
            this.makeExtraBackupsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // autoCheckForUpdatesToolStripMenuItem
            // 
            this.autoCheckForUpdatesToolStripMenuItem.CheckOnClick = true;
            this.autoCheckForUpdatesToolStripMenuItem.Name = "autoCheckForUpdatesToolStripMenuItem";
            this.autoCheckForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.autoCheckForUpdatesToolStripMenuItem.Text = "Auto-Check For Updates";
            this.autoCheckForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.autoCheckForUpdatesToolStripMenuItem_Click);
            // 
            // fullBackupOnStartupToolStripMenuItem
            // 
            this.fullBackupOnStartupToolStripMenuItem.CheckOnClick = true;
            this.fullBackupOnStartupToolStripMenuItem.Name = "fullBackupOnStartupToolStripMenuItem";
            this.fullBackupOnStartupToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.fullBackupOnStartupToolStripMenuItem.Text = "Full Backup On Monitor Startup";
            this.fullBackupOnStartupToolStripMenuItem.Click += new System.EventHandler(this.fullBackupOnStartupToolStripMenuItem_Click);
            // 
            // ignoreDatesDuringBackupToolStripMenuItem
            // 
            this.ignoreDatesDuringBackupToolStripMenuItem.CheckOnClick = true;
            this.ignoreDatesDuringBackupToolStripMenuItem.Name = "ignoreDatesDuringBackupToolStripMenuItem";
            this.ignoreDatesDuringBackupToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.ignoreDatesDuringBackupToolStripMenuItem.Text = "Ignore Dates During Backup";
            this.ignoreDatesDuringBackupToolStripMenuItem.Click += new System.EventHandler(this.ignoreDatesDuringBackupToolStripMenuItem_Click);
            // 
            // startMonitorOnLoginToolStripMenuItem
            // 
            this.startMonitorOnLoginToolStripMenuItem.CheckOnClick = true;
            this.startMonitorOnLoginToolStripMenuItem.Name = "startMonitorOnLoginToolStripMenuItem";
            this.startMonitorOnLoginToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.startMonitorOnLoginToolStripMenuItem.Text = "Start Monitor On Login";
            this.startMonitorOnLoginToolStripMenuItem.Click += new System.EventHandler(this.startMonitorOnLoginToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(238, 6);
            // 
            // alternateInstallPathsToolStripMenuItem
            // 
            this.alternateInstallPathsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewPathToolStripMenuItem});
            this.alternateInstallPathsToolStripMenuItem.Name = "alternateInstallPathsToolStripMenuItem";
            this.alternateInstallPathsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.alternateInstallPathsToolStripMenuItem.Text = "Alternate Install Paths";
            // 
            // addNewPathToolStripMenuItem
            // 
            this.addNewPathToolStripMenuItem.Name = "addNewPathToolStripMenuItem";
            this.addNewPathToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.addNewPathToolStripMenuItem.Text = "Add New Path...";
            this.addNewPathToolStripMenuItem.Click += new System.EventHandler(this.addNewPathToolStripMenuItem_Click);
            // 
            // changeBackupPathToolStripMenuItem
            // 
            this.changeBackupPathToolStripMenuItem.Name = "changeBackupPathToolStripMenuItem";
            this.changeBackupPathToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.changeBackupPathToolStripMenuItem.Text = "Change Backup Path...";
            this.changeBackupPathToolStripMenuItem.Click += new System.EventHandler(this.changeBackupPathToolStripMenuItem_Click);
            // 
            // changeSteamPathToolStripMenuItem
            // 
            this.changeSteamPathToolStripMenuItem.Name = "changeSteamPathToolStripMenuItem";
            this.changeSteamPathToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.changeSteamPathToolStripMenuItem.Text = "Change Steam Path...";
            this.changeSteamPathToolStripMenuItem.Click += new System.EventHandler(this.changeSteamPathToolStripMenuItem_Click);
            // 
            // makeExtraBackupsToolStripMenuItem
            // 
            this.makeExtraBackupsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableToolStripMenuItem,
            this.toolStripSeparator2,
            this.setTimeBetweenCopiesToolStripMenuItem,
            this.setMaxNumberOfCopiesToolStripMenuItem});
            this.makeExtraBackupsToolStripMenuItem.Name = "makeExtraBackupsToolStripMenuItem";
            this.makeExtraBackupsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.makeExtraBackupsToolStripMenuItem.Text = "Make Extra Backups";
            // 
            // enableToolStripMenuItem
            // 
            this.enableToolStripMenuItem.CheckOnClick = true;
            this.enableToolStripMenuItem.Name = "enableToolStripMenuItem";
            this.enableToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.enableToolStripMenuItem.Text = "Enable";
            this.enableToolStripMenuItem.Click += new System.EventHandler(this.enableToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(223, 6);
            // 
            // setTimeBetweenCopiesToolStripMenuItem
            // 
            this.setTimeBetweenCopiesToolStripMenuItem.Enabled = false;
            this.setTimeBetweenCopiesToolStripMenuItem.Name = "setTimeBetweenCopiesToolStripMenuItem";
            this.setTimeBetweenCopiesToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.setTimeBetweenCopiesToolStripMenuItem.Text = "Set Time Between Copies...";
            this.setTimeBetweenCopiesToolStripMenuItem.Click += new System.EventHandler(this.setTimeBetweenCopiesToolStripMenuItem_Click);
            // 
            // setMaxNumberOfCopiesToolStripMenuItem
            // 
            this.setMaxNumberOfCopiesToolStripMenuItem.Enabled = false;
            this.setMaxNumberOfCopiesToolStripMenuItem.Name = "setMaxNumberOfCopiesToolStripMenuItem";
            this.setMaxNumberOfCopiesToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.setMaxNumberOfCopiesToolStripMenuItem.Text = "Set Max Number Of Copies...";
            this.setMaxNumberOfCopiesToolStripMenuItem.Click += new System.EventHandler(this.setMaxNumberOfCopiesToolStripMenuItem_Click);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check For Updates...";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // noGamesDetectedToolStripMenuItem
            // 
            this.noGamesDetectedToolStripMenuItem.Name = "noGamesDetectedToolStripMenuItem";
            this.noGamesDetectedToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.noGamesDetectedToolStripMenuItem.Text = "No Games Detected";
            // 
            // exitMonitorToolStripMenuItem
            // 
            this.exitMonitorToolStripMenuItem.Name = "exitMonitorToolStripMenuItem";
            this.exitMonitorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitMonitorToolStripMenuItem.Text = "Exit Monitor";
            // 
            // progressBar1
            // 
            this.progressBar1.ContainerControl = this;
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.ShowInTaskbar = true;
            this.progressBar1.Size = new System.Drawing.Size(328, 23);
            this.progressBar1.State = wyDay.Controls.ProgressBarState.Error;
            this.progressBar1.TabIndex = 2;
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // monitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 23);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "monitorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MASGAU Monitor Is Detecting Save Paths...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.monitorForm_FormClosing);
            this.Shown += new System.EventHandler(this.monitorForm_Shown);
            this.notifierMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon monitorNotifier;
        private System.Windows.Forms.ContextMenuStrip notifierMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rescanGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem watchedGamesMenu;
        private System.Windows.Forms.ToolStripMenuItem noGamesDetectedToolStripMenuItem;
        private wyDay.Controls.Windows7ProgressBar progressBar1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullBackupOnStartupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoCheckForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreDatesDuringBackupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startMonitorOnLoginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem changeBackupPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeSteamPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeExtraBackupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setTimeBetweenCopiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setMaxNumberOfCopiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alternateInstallPathsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}

