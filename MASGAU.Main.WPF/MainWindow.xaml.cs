using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.ComponentModel;
using System.IO;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Archive;
using MASGAU.Game;
using MASGAU.Communication.Progress;
using MASGAU.Communication.Message;
namespace MASGAU.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AProgramWindow
    {
        private MainProgramHandler  main;

        public MainWindow(): base(new MainProgramHandler())
        {
            main = (MainProgramHandler)program_handler;
            InitializeComponent();
            //restoreTree.PreviewMouseDoubleClick += (sender, e) => e.Handled = true; 
            default_progress_color = progressBar.Foreground;
            overall_progress = progressBar;
        }

        private void hookupGameContexts()
        {
            gamesLst.DataContext = Core.games;
            backupEverythingBtn.DataContext = Core.games;
            noGamesDetectedLabel.DataContext = Core.games;
            contributersLst.DataContext = Core.games.contributors;
        }

        private void hookupRestoreContexts()
        {
            restoreLst.DataContext = Core.archives;
            noArchivesLabel.DataContext = Core.archives;
            backupPathNotSetLabel.DataContext = Core.settings;
            reloadArchivesBtn.DataContext = Core.settings;
        }


        private void hookupTaskContexts()
        {
            applyTaskBtn.DataContext = Core.task;
            frequencyCombo.DataContext = Core.task;
            dayOfWeekCombo.DataContext = Core.task;
            dayOfMonthCombo.DataContext = Core.task;
            timeHoursCombo.DataContext = Core.task;
            timeMinutesCombo.DataContext = Core.task;
            deleteTaskBtn.DataContext = Core.task;

            scheduleTab.DataContext = Core.task;
        }

        protected override void setup(object sender, RunWorkerCompletedEventArgs e)
        {
            base.setup(sender, e);

            hookupGameContexts();
            hookupRestoreContexts();
            hookupTaskContexts();

            Core.settings.PropertyChanged -= new PropertyChangedEventHandler(settings_PropertyChanged);
            Core.settings.PropertyChanged += new PropertyChangedEventHandler(settings_PropertyChanged);

            settingsControl.bindSettingsControls();

            versionLabel.Content += Core.version;

            userTxt.Text = Environment.UserName;
            
            customGamesCombo.ItemsSource = main.custom_games;
            siteLink.NavigateUri = new Uri(Core.site_url);
            siteLink.Inlines.Clear();
            siteLink.Inlines.Add(Core.site_url);

            enableInterface(null,null);
        }

        void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            switch(e.PropertyName) {
                case "alt_paths":
                    if((int)getPropertyDispatcher(daTabs,"SelectedIndex")==0) {
                        this.Dispatcher.BeginInvoke(new Action(this.removeStatusBar));
                        this.Dispatcher.BeginInvoke(new Action(this.redetectGames));
                    } else {
                        Core.redetect_games = true;
                    }
                    break;
                case "backup_path":
                    if((int)getPropertyDispatcher(daTabs,"SelectedIndex")==1) {
                        this.Dispatcher.BeginInvoke(new Action(this.removeStatusBar));
                        this.Dispatcher.BeginInvoke(new Action(this.redetectArchives));
                    } else {
                        Core.redetect_archives = true;
                    }
                    break;
            }
        }


        public override void disableInterface()
        {
            base.disableInterface();
            daTabs.IsEnabled = false;
            statusBar1.Visibility = System.Windows.Visibility.Visible;
        }
        public override void enableInterface()
        {
            base.enableInterface();
            stopButton.Visibility = System.Windows.Visibility.Collapsed;
            daTabs.IsEnabled = true;
            hookupGameContexts();
            hookupRestoreContexts();
            restoreLst.DataContext = Core.archives;
            if(daTabs.SelectedIndex!=0)
                statusBar1.Visibility = System.Windows.Visibility.Collapsed;
        }


        #region Update stuff

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            checkUpdates(false,false);
        }
        #endregion


        #region Methods For The Games List
        private void redetectGamesBtn_Click(object sender, RoutedEventArgs e)
        {
            redetectGames();
        }

        private void gamesLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected_count = gamesLst.SelectedItems.Count;
            if(selected_count>0) {
                backupSelectedBtn.IsEnabled = true;
                if(selected_count==1) {
                    backupSelectedBtn.Content = "Back This Up";
                } else {
                    backupSelectedBtn.Content = "Back These " + selected_count + " Up";
                }
            } else {
                backupSelectedBtn.IsEnabled = false;
                backupSelectedBtn.Content = "Back Nothing Up";
            }
        }

        private void backupSelected() {
            if (Core.settings.backup_path_set||changeBackUpPath())
            {
                List<GameHandler> these = new List<GameHandler>();
                foreach(GameHandler game in gamesLst.SelectedItems) {
                    these.Add(game);
                }
                stopButton.Visibility = System.Windows.Visibility.Visible;
                stopButton.IsEnabled = true;
                beginBackup(these,null);
                Core.redetect_archives = true;
            }
        }
        private void backupSelectedBtn_Click(object sender, RoutedEventArgs e)
        {
            backupSelected();
        }
        private void backupEverythingBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Core.settings.backup_path_set||changeBackUpPath()) {
                stopButton.Visibility = System.Windows.Visibility.Visible;
                stopButton.IsEnabled = true;
                beginBackup(null);
                Core.redetect_archives = true;
            }
        }
        private void gamesLst_DragOver(object sender, DragEventArgs e)
        {
            resizeGameColumns();
        }
        private void gamesLst_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeGameColumns();
        }
        private void resizeGameColumns() {
            double new_width = gamesLst.ActualWidth - gameNameColumn.Width - gamePlatformColumn.Width - backupColumn.Width - 20;
            if(new_width>0) {
                gameTitleColumn.Width = new_width;
            } else {
                gameTitleColumn.Width = 0;
            }

        }
        #endregion

        #region Games List Context Handlers
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {

            if (gamesLst.SelectedItems.Count==0){
                (sender as ContextMenu).IsOpen = false;
            }
            createCustomArchiveMenu.Visibility = System.Windows.Visibility.Visible;
            enableBackupMenu.Visibility = System.Windows.Visibility.Visible;
            purgeMenu.Visibility = System.Windows.Visibility.Visible;

            if (gamesLst.SelectedItems.Count> 1){
                bool show_enable = false, show_disable = false;

                foreach(GameHandler game in gamesLst.SelectedItems) {
                    switch(game.id.platform) {
                        case GamePlatform.PS1:
                        case GamePlatform.PS2:
                        case GamePlatform.PS3:
                        case GamePlatform.PSP:
                            purgeMenu.Visibility = System.Windows.Visibility.Collapsed;
                            break;
                    }

                    if(game.backup_enabled) {
                        show_enable = true;
                    } else {
                        show_disable = true;
                    }
                }


                if(show_disable&&show_enable)
                    // Need a maybe here
                    enableBackupMenu.IsChecked = false;
                else if(show_enable)
                    enableBackupMenu.IsChecked = true;
                else
                    enableBackupMenu.IsChecked = false;

                backupMenuItem.Header = "Back These Up";
                createCustomArchiveMenu.Visibility = System.Windows.Visibility.Collapsed;
            } else if (gamesLst.SelectedItems.Count==1) {
                GameHandler game = gamesLst.SelectedItem as GameHandler;
                enableBackupMenu.IsChecked = game.backup_enabled;

                backupMenuItem.Header = "Back This Up";
                createCustomArchiveMenu.Visibility = System.Windows.Visibility.Visible;
            } 

        }
        private void disableMenu_Click(object sender, RoutedEventArgs e)
        {
            foreach(GameHandler check_me in gamesLst.SelectedItems) {
                check_me.backup_enabled= enableBackupMenu.IsChecked;
            }
        }
        private void backupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            backupSelected();

        }
        private void purgeMenu_Click(object sender, RoutedEventArgs e)
        {
            this.purgeGames((IEnumerable)gamesLst.SelectedItems);
        }

        private string last_archive_create = null;
        private void createCustomArchiveMenu_Click(object sender, RoutedEventArgs e)
        {
            GameHandler game = gamesLst.SelectedItem as GameHandler;
            ManualArchiveWindow manual = new ManualArchiveWindow(game,this);
            if((bool)manual.ShowDialog()) {
                List<DetectedFile> selected_files = manual.getSelectedFiles();
                DateTime right_now = DateTime.Now;

                if(selected_files.Count>0) {
                    string initial_directory;
                    if(last_archive_create==null) {
                        initial_directory = Core.settings.backup_path;
                    } else {
                        initial_directory = last_archive_create;
                    }
                    ArchiveID archive = new ArchiveID(game.id,selected_files[0].owner,null);

                    StringBuilder initial_name = new StringBuilder(archive.ToString());

                    initial_name.Append(Core.owner_seperator + right_now.ToString().Replace('/','-').Replace(':','-'));

                    Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
                    save.Title = "And where would you like to put this backup?";
                    save.AddExtension = true;
                    save.InitialDirectory = initial_directory;
                    save.FileName = initial_name.ToString(); ;
                    save.DefaultExt = "gb7";
                    save.Filter = "MASGAU Save Archive (*.gb7)|*.gb7";
                    save.OverwritePrompt = true;

                    if((bool)save.ShowDialog(this)) {
                        string file = save.FileName;
                        beginBackup(game,selected_files,file,null);
                        Core.redetect_archives = true;
                        last_archive_create = Path.GetDirectoryName(file);
                    }
                }
            }

        }
        #endregion

        #region Methods for task handling
        public void deleteTask() {
            Core.task.deleteTask();
            deleteTaskBtn.IsEnabled = false;
        }
        #endregion

        #region Methods for changing settings
        public bool changeBackUpPath() {
            if(base.changeBackupPath()) {
                Core.redetect_archives = true;
                return true;
            } else {
                Core.redetect_archives = false;
                return false;
            }
        }
        public void chooseSyncPath() {
            //if(Common.settings.changeSyncPath()) {
                //syncPathInput.Text = SettingsHandler.getSyncPath();
                //openSyncPath.Enabled = true;
            //}
        }
        #endregion

        #region Main Event Handlers
        public override void updateProgress(ProgressUpdatedEventArgs e) {
            if(e.message!=null)
                progressTxt.Text = e.message;
            applyProgress(progressBar,e);
        }
        #endregion

        #region Task methods

        private void deleteTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.task.deleteTask();
            deleteTaskBtn.IsEnabled = false;
        }
        private void frequencyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(frequencyCombo.SelectedIndex) {
                case 0:
                    dayOfWeekCombo.IsEnabled = false;
                    dayOfMonthCombo.IsEnabled = false;
                    break;
                case 1:
                    dayOfWeekCombo.IsEnabled = true;
                    dayOfMonthCombo.IsEnabled = false;
                    break;
                case 2:
                    dayOfWeekCombo.IsEnabled = false;
                    dayOfMonthCombo.IsEnabled = true;
                    break;
            }
        }
        private void applyTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if(passwordTxt.Password!="") {
                Core.task.createTask(userTxt.Text,passwordTxt.Password);

                if(Core.task.exists) {
                    deleteTaskBtn.IsEnabled = true;
                } else {
                    deleteTaskBtn.IsEnabled = false;
                    showError("What Did I Just Tell You","Unable to create task. Here's the excuse:" + Environment.NewLine + Core.task.output + Environment.NewLine + "The task has been deleted.");
                }
            } else {
                showError("Pander To Me","You must enter a password for the user the task will be running as,\nwhich is shown in the little text box right there.");
            }
        }
        #endregion

        #region Ohter interface methods
        private void removeStatusBar() {
            if(daTabs.SelectedIndex!=0) {
                statusBar1.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                statusBar1.Visibility = System.Windows.Visibility.Visible;
            }
        }
        
        int current_tab = 0;
        private void daTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(main==null)
                return;
            
            removeStatusBar();

            if(daTabs.SelectedIndex==current_tab)
                return;

            current_tab = daTabs.SelectedIndex;

            switch(daTabs.SelectedIndex) {
                case 0:
                    if (Core.redetect_games) {
                        redetectGames();
                    }
                    break;
                case 1:
                    if(Core.redetect_games||Core.redetect_archives) {
                        redetectArchives();
                    }
                    break;
            }
        }
        #endregion


        #region Restore List Methods
        private void reloadArchivesBtn_Click(object sender, RoutedEventArgs e)
        {
            redetectArchives();
        }

        private void restoreLst_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeRestoreList();
        }

        private void resizeRestoreList() {
            double new_width = restoreLst.ActualWidth - 
                                archiveTypeColumn.Width - 
                                archiveOwnerColumn.Width - 
                                archiveDateColumn.Width - 20;
            if(new_width>0) {
                archiveNameColumn.Width = new_width;
            } else {
                archiveNameColumn.Width = 0;
            }

        }

        private void restoreLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected_count = restoreLst.SelectedItems.Count;
            if(selected_count>0) {
                restoreSaveBtn.IsEnabled = true;
                if(selected_count==1) {
                    restoreSaveBtn.Content = "Restore This";
                } else {
                    restoreSaveBtn.Content = "Restore These " + selected_count;
                }
            } else {
                restoreSaveBtn.IsEnabled = false;
                restoreSaveBtn.Content = "Restore Nothing";
            }

        }
        private void restoreSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            int selected_count = restoreLst.SelectedItems.Count;
            if(selected_count>0) {
                List<ArchiveHandler> archives = new List<ArchiveHandler>();
                foreach(ArchiveHandler archive in restoreLst.SelectedItems) {
                    archives.Add(archive);
                }
                beginRestore(archives);
            }
        }
        private void restoreOtherBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.AutoUpgradeEnabled = true;
            open.CheckFileExists = true;
            open.CheckPathExists = true;
            open.DefaultExt = "gb7";
            open.Filter = "MASGAU Save Archive (*.gb7)|*.gb7";
            open.Multiselect = true;
            open.Title = "Select Backup(s) To Restore";
            if(open.ShowDialog(this.GetIWin32Window())== System.Windows.Forms.DialogResult.OK) {
                if(open.FileNames.Length>0) {
                    List<ArchiveHandler> archives = new List<ArchiveHandler>();
                    foreach(string file in open.FileNames) {
                        try{
                            archives.Add(new ArchiveHandler(new FileInfo(file)));
                        } catch (Exception ex) {
                            MessageHandler.SendError("File not archive","The file " + file + " is not a properly formatted MASGAU archive.",ex);
                        }
                    }
                    beginRestore(archives);
                }
            }
        }
        #endregion

        private void contributersLst_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            contributorCountClm.Width = 50;
            contributorNameClm.Width = contributersLst.ActualWidth - 75;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            stopButton.IsEnabled = false;
            cancelBackup();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            cancelBackup();
        }




        #region methods for dealing with custom game data

        #endregion

    }
}
