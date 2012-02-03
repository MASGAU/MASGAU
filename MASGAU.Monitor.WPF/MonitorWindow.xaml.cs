using System;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using MASGAU.Communication.Progress;
namespace MASGAU.Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MonitorWindow : AProgramWindow
    {
        #region Notify icon stuff
            private System.Windows.Forms.NotifyIcon monitorNotifier;
            private System.Windows.Forms.ContextMenuStrip notifierMenu;
            private System.Windows.Forms.ToolStripMenuItem exitMonitorToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem rescanGamesToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        #endregion
        
        private MonitorProgramHandler monitor;

        public MonitorWindow(): base(null)
        {
            InitializeComponent();
            setUpNotifier();
            default_progress_color = progressBar1.Foreground;
        }

        protected override void WindowLoaded(object sender, RoutedEventArgs e)
        {
            progressBar1.Value = progressBar1.Minimum;
            setUpProgramHandler();
            base.WindowLoaded(sender, e);
        }


        protected override void setUpProgramHandler() {
            disableInterface();
            if(program_handler!=null) {
                monitor = (MonitorProgramHandler)program_handler;
                monitor.CancelAsync();
                monitor.stop();
                if(monitor.monitor_thread!=null) {
                    while(monitor.monitor_thread.IsAlive)
                        System.Threading.Thread.Sleep(100);
                }
                monitor.Dispose();
                monitor = null;
            }
            monitor = new MonitorProgramHandler();
            program_handler = monitor;
            base.setUpProgramHandler();
            this.Title =  monitor.program_title + " Is Setting Up...";
        }

        protected override void setup(object sender, RunWorkerCompletedEventArgs e)
        {
            base.setup(sender, e);

            Core.settings.PropertyChanged += new PropertyChangedEventHandler(settings_PropertyChanged);

            
            Core.redetect_archives = false;
            Core.redetect_games = false;

            // This performs a full backup on startup, to ensure consistency
            if(Core.settings.monitor_startup_backup) {
                beginBackup(null);
            } else {
                enableInterface(null,null);
                setNotifyToolTip();
            }

        }

        public override void disableInterface()
        {
            base.disableInterface();
            monitorNotifier.Visible = false;
            this.Visibility = System.Windows.Visibility.Visible;
        }
        public override void enableInterface()
        {
            base.enableInterface();
            if(monitorNotifier!=null) {
                monitorNotifier.Visible = true;
                setNotifyToolTip();
            }
            this.Visibility = System.Windows.Visibility.Hidden;
        }


        void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            setNotifyToolTip();
        }



        #region Monitor Event Handlers
        public override void updateProgress(ProgressUpdatedEventArgs e) {
            if(e.message!=null)
                this.Title = e.message;
            applyProgress(progressBar1,e);
        }
                #endregion

        private void setUpNotifier() {
            //this.components = new System.ComponentModel.Container();
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorWindow));
            this.monitorNotifier = new System.Windows.Forms.NotifyIcon();
            this.notifierMenu = new System.Windows.Forms.ContextMenuStrip();

            this.rescanGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.notifierMenu.SuspendLayout();

            // 
            // monitorNotifier
            // 
            this.monitorNotifier.ContextMenuStrip = this.notifierMenu;
            this.monitorNotifier.Icon =new System.Drawing.Icon("masgau.ico");
            this.monitorNotifier.Text = "MASGAU Monitor";
            this.monitorNotifier.Visible = false;
            // 
            // notifierMenu
            // 
            this.notifierMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanGamesToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.notifierMenu.Name = "notifierMenu";
            this.notifierMenu.ShowImageMargin = false;
            this.notifierMenu.Size = new System.Drawing.Size(164, 158);
            // 
            // rescanGamesToolStripMenuItem
            // 
            this.rescanGamesToolStripMenuItem.Name = "rescanGamesToolStripMenuItem";
            this.rescanGamesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.rescanGamesToolStripMenuItem.Text = "Rescan Games";
            this.rescanGamesToolStripMenuItem.Click += new System.EventHandler(this.rescanGamesToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Checked = true;
            this.settingsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.settingsToolStripMenuItem.Text = "Settings...";
            this.settingsToolStripMenuItem.Click += new EventHandler(settingsToolStripMenuItem_Click);

            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // exitMonitorToolStripMenuItem
            // 
            this.exitMonitorToolStripMenuItem.Name = "exitMonitorToolStripMenuItem";
            this.exitMonitorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitMonitorToolStripMenuItem.Text = "Exit";

            this.monitorNotifier.DoubleClick += new EventHandler(monitorNotifier_DoubleClick);

            this.notifierMenu.ResumeLayout(false);
        
        }

        void monitorNotifier_DoubleClick(object sender, EventArgs e)
        {
            Core.openBackupPath();
        }

        void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool old_backup = Core.settings.monitor_startup_backup;

            this.disableInterface();
            this.Visibility = System.Windows.Visibility.Hidden;
            MonitorSettingsWindow settings_window = new MonitorSettingsWindow();
            settings_window.ShowDialog();
                setUpProgramHandler();

            if(Core.redetect_games||Core.rebuild_sync||Core.redetect_archives||(!old_backup&&Core.settings.monitor_startup_backup)) {
                //setUp();
            } else {
                //this.enableInterface(null,null);
            }
        }

        void changeSyncPathMenuItem_Click(object sender, EventArgs e)
        {
            monitor.sync_watcher.EnableRaisingEvents = false;
            if(changeSyncPath())
                setUpProgramHandler();
            else
                monitor.sync_watcher.EnableRaisingEvents = true;
        }

        private void setNotifyToolTip() {
            int count = Core.games.Count; //monitor.countMonitoredGames();
            if(count<0)
                monitorNotifier.Text = "MASGAU Monitor is...WHAT!??!";
            if(count==0) 
                monitorNotifier.Text = "MASGAU Monitor isn't stalking any games";
            else if(count==1)
                monitorNotifier.Text = "MASGAU Monitor is stalking a single game";
            else
                monitorNotifier.Text = "MASGAU Monitor is stalking " + count + " games";
        }

        #region Notify icon event handlers

        private void rescanGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setUpProgramHandler();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            cancelBackup();
            monitorNotifier.Visible =false;
            monitorNotifier.Dispose();
            monitor.CancelAsync();
            monitor.Dispose();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            monitorNotifier.Visible = false;
            new About(this).ShowDialog();
            monitorNotifier.Visible = true;
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            monitorNotifier.Visible = false;
            this.Visibility = System.Windows.Visibility.Visible;
            checkUpdates(false,false);
        }
        void updater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(Core.updater.shutdown_required){
                this.Close();
                return;
            }
            if(!Core.updater.redetect_required) {
                monitorNotifier.Visible = true;
                this.Visibility = System.Windows.Visibility.Hidden;

            }
            
        }


        #endregion

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            checkUpdates(false,false);
        }



    }
}
