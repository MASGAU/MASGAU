using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MVC.Communication;
using MASGAU.Location.Holders;
using Translator;
using Translator.WPF;
namespace MASGAU.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AProgramWindow {
        private MainProgramHandler main;

        public MainWindow()
            : base(new MainProgramHandler(new Location.LocationsHandler())) {
            main = (MainProgramHandler)program_handler;
            InitializeComponent();
            TranslationHelpers.translateWindow(this);
            //restoreTree.PreviewMouseDoubleClick += (sender, e) => e.Handled = true; 
            default_progress_color = progressBar.Foreground;
            overall_progress = progressBar;
        }

        private void hookupGameContexts() {
        }

        private void hookupRestoreContexts() {
            backupPathNotSetLabel.DataContext = Core.settings;
            reloadArchivesBtn.DataContext = Core.settings;
        }


        private void hookupTaskContexts() {
        //    applyTaskBtn.DataContext = Core.task;
        //    frequencyCombo.DataContext = Core.task;
        //    dayOfWeekCombo.DataContext = Core.task;
        //    dayOfMonthCombo.DataContext = Core.task;
        //    timeHoursCombo.DataContext = Core.task;
        //    timeMinutesCombo.DataContext = Core.task;
        //    deleteTaskBtn.DataContext = Core.task;

        //    scheduleTab.DataContext = Core.task;
        }

        protected override void setup(object sender, RunWorkerCompletedEventArgs e) {
            base.setup(sender, e);
            TranslationHelpers.translate(noGamesDetectedLabel,"NoGamesDetected");
            hookupGameContexts();
            hookupRestoreContexts();
            hookupTaskContexts();

            Core.settings.PropertyChanged -= new PropertyChangedEventHandler(settings_PropertyChanged);
            Core.settings.PropertyChanged += new PropertyChangedEventHandler(settings_PropertyChanged);

            settingsControl.bindSettingsControls();


            userTxt.Text = Environment.UserName;


            enableInterface(null, null);
        }

        void settings_PropertyChanged(object sender, PropertyChangedEventArgs e) {

            switch (e.PropertyName) {
                case "alt_paths":
                    if ((int)getPropertyDispatcher(daTabs, "SelectedIndex") == 0) {
                        this.Dispatcher.BeginInvoke(new Action(this.removeStatusBar));
                        this.Dispatcher.BeginInvoke(new Action(this.redetectGames));
                    } else {
                        Core.redetect_games = true;
                    }
                    break;
                case "backup_path":
                    if ((int)getPropertyDispatcher(daTabs, "SelectedIndex") == 1) {
                        this.Dispatcher.BeginInvoke(new Action(this.removeStatusBar));
                        this.Dispatcher.BeginInvoke(new Action(this.redetectArchives));
                    } else {
                        Core.redetect_archives = true;
                    }
                    break;
            }
        }


        public override void disableInterface() {
            base.disableInterface();
            daTabs.IsEnabled = false;
            statusBar1.Visibility = System.Windows.Visibility.Visible;
        }
        public override void enableInterface() {
            base.enableInterface();
            stopButton.Visibility = System.Windows.Visibility.Collapsed;
            daTabs.IsEnabled = true;
            hookupGameContexts();
            hookupRestoreContexts();
            if (daTabs.SelectedIndex != 0)
                statusBar1.Visibility = System.Windows.Visibility.Collapsed;
        }


        #region Update stuff

        private void updateBtn_Click(object sender, RoutedEventArgs e) {
        }
        #endregion


        #region Methods For The Games List
        private void redetectGamesBtn_Click(object sender, RoutedEventArgs e) {
            redetectGames();
        }

        private void gamesLst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selected_count = gamesLst.SelectedItems.Count;
            TranslationHelpers.translate(backupSelectedBtn, "BackupGames", selected_count.ToString());

            if (selected_count > 0) {
                backupSelectedBtn.IsEnabled = true;
            } else {
                backupSelectedBtn.IsEnabled = false;
            }
        }

        private void gamesLst_DragOver(object sender, DragEventArgs e) {
            resizeGameColumns();
        }
        private void gamesLst_SizeChanged(object sender, SizeChangedEventArgs e) {
            resizeGameColumns();
        }
        private void resizeGameColumns() {
            double new_width = gamesLst.ActualWidth - gameNameColumn.Width - gamePlatformColumn.Width - backupColumn.Width - 20;
            if (new_width > 0) {
                gameTitleColumn.Width = new_width;
            } else {
                gameTitleColumn.Width = 0;
            }

        }
        #endregion

        #region Games List Context Handlers
        private void ContextMenu_Opened(object sender, RoutedEventArgs e) {

            //if (gamesLst.SelectedItems.Count == 0) {
            //    (sender as ContextMenu).IsOpen = false;
            //}
            //createCustomArchiveMenu.Visibility = System.Windows.Visibility.Visible;
            //enableBackupMenu.Visibility = System.Windows.Visibility.Visible;
            //purgeMenu.Visibility = System.Windows.Visibility.Visible;

            //if (gamesLst.SelectedItems.Count > 1) {
            //    bool show_enable = false, show_disable = false;

            //    foreach (Game game in gamesLst.SelectedItems) {
            //        switch (game.id.platform) {
            //            case GamePlatform.PS1:
            //            case GamePlatform.PS2:
            //            case GamePlatform.PS3:
            //            case GamePlatform.PSP:
            //                purgeMenu.Visibility = System.Windows.Visibility.Collapsed;
            //                break;
            //        }

            //        if (game.IsMonitored) {
            //            show_enable = true;
            //        } else {
            //            show_disable = true;
            //        }
            //    }


            //    if (show_disable && show_enable)
            //        // Need a maybe here
            //        enableBackupMenu.IsChecked = false;
            //    else if (show_enable)
            //        enableBackupMenu.IsChecked = true;
            //    else
            //        enableBackupMenu.IsChecked = false;

            //    TranslationHelpers.translate(backupMenuItem,"BackupMultipleGamesMenu");
            //    createCustomArchiveMenu.Visibility = System.Windows.Visibility.Collapsed;
            //} else if (gamesLst.SelectedItems.Count == 1) {
            //    Game game = gamesLst.SelectedItem as Game;
            //    enableBackupMenu.IsChecked = game.backup_enabled;

            //    TranslationHelpers.translate(backupMenuItem,"BackupOneGameMenu");
            //    createCustomArchiveMenu.Visibility = System.Windows.Visibility.Visible;
            //}

        }
        private void disableMenu_Click(object sender, RoutedEventArgs e) {
            foreach (Game check_me in gamesLst.SelectedItems) {
                //check_me.IsMonitored = enableBackupMenu.IsChecked;
            }
        }
        private void backupMenuItem_Click(object sender, RoutedEventArgs e) {

        }
        private void purgeMenu_Click(object sender, RoutedEventArgs e) {
            //this.purgeGames((IEnumerable)gamesLst.SelectedItems);
        }

        private void createCustomArchiveMenu_Click(object sender, RoutedEventArgs e) {


        }
        #endregion

        #region Methods for task handling
        public void deleteTask() {
//            Core.task.deleteTask();
  //          deleteTaskBtn.IsEnabled = false;
        }
        #endregion

        #region Methods for changing settings
        public void chooseSyncPath() {
            //if(Common.settings.changeSyncPath()) {
            //syncPathInput.Text = SettingsHandler.getSyncPath();
            //openSyncPath.Enabled = true;
            //}
        }
        #endregion

        #region Main Event Handlers
        public override void updateProgress(ProgressUpdatedEventArgs e) {
            if (e.message != null)
                progressTxt.Text = e.message;
            applyProgress(progressBar, e);
        }
        #endregion

        #region Task methods

        private void deleteTaskBtn_Click(object sender, RoutedEventArgs e) {
//            Core.task.deleteTask();
  //          deleteTaskBtn.IsEnabled = false;
        }
        private void frequencyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //switch (frequencyCombo.SelectedIndex) {
            //    case 0:
            //        dayOfWeekCombo.IsEnabled = false;
            //        dayOfMonthCombo.IsEnabled = false;
            //        break;
            //    case 1:
            //        dayOfWeekCombo.IsEnabled = true;
            //        dayOfMonthCombo.IsEnabled = false;
            //        break;
            //    case 2:
            //        dayOfWeekCombo.IsEnabled = false;
            //        dayOfMonthCombo.IsEnabled = true;
            //        break;
            //}
        }
        private void applyTaskBtn_Click(object sender, RoutedEventArgs e) {

            //if (passwordTxt.Password != "") {
            //    Core.task.createTask(userTxt.Text, passwordTxt.Password);

            //    if (Core.task.exists) {
            //        deleteTaskBtn.IsEnabled = true;
            //    } else {
            //        deleteTaskBtn.IsEnabled = false;
            //        TranslationHelpers.showTranslatedError(this, "UnableToCreateTask", Core.task.output);
            //    }
            //} else {
            //    TranslationHelpers.showTranslatedError(this, "EnterTaskCredentials");
            //}
        }
        #endregion

        #region Ohter interface methods
        private void removeStatusBar() {
            if (daTabs.SelectedIndex != 0) {
                statusBar1.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                statusBar1.Visibility = System.Windows.Visibility.Visible;
            }
        }

        int current_tab = 0;
        private void daTabs_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (main == null)
                return;

            removeStatusBar();

            if (daTabs.SelectedIndex == current_tab)
                return;

            current_tab = daTabs.SelectedIndex;

            switch (daTabs.SelectedIndex) {
                case 0:
                    if (Core.redetect_games) {
                        redetectGames();
                    }
                    break;
                case 1:
                    if (Core.redetect_games || Core.redetect_archives) {
                        redetectArchives();
                    }
                    break;
            }
        }
        #endregion


        #region Restore List Methods
        private void reloadArchivesBtn_Click(object sender, RoutedEventArgs e) {
            redetectArchives();
        }

        private void restoreLst_SizeChanged(object sender, SizeChangedEventArgs e) {
            resizeRestoreList();
        }

        private void resizeRestoreList() {
            double new_width = restoreLst.ActualWidth -
                                archiveTypeColumn.Width -
                                archiveOwnerColumn.Width -
                                archiveDateColumn.Width - 20;
            if (new_width > 0) {
                archiveNameColumn.Width = new_width;
            } else {
                archiveNameColumn.Width = 0;
            }

        }

        private void restoreLst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selected_count = restoreLst.SelectedItems.Count;
            if (selected_count > 0) {
                restoreSaveBtn.IsEnabled = true;
                if (selected_count == 1) {
                    TranslationHelpers.translate(restoreSaveBtn,"RestoreOne");
                } else {
                    TranslationHelpers.translate(restoreSaveBtn,"RestoreMultiple", selected_count.ToString());
                }
            } else {
                restoreSaveBtn.IsEnabled = false;
                TranslationHelpers.translate(restoreSaveBtn,"RestoreNone");
            }

        }
        private void restoreSaveBtn_Click(object sender, RoutedEventArgs e) {
        }
        private void restoreOtherBtn_Click(object sender, RoutedEventArgs e) {

        }

        protected override void redetectArchivesComplete(object sender, RunWorkerCompletedEventArgs e) {
            TranslationHelpers.translate(noArchivesLabel,"NoArchivesFound");
        }
        #endregion

        private void contributersLst_SizeChanged(object sender, SizeChangedEventArgs e) {
            contributorCountClm.Width = 50;
            contributorNameClm.Width = contributersLst.ActualWidth - 75;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e) {
            stopButton.IsEnabled = false;

        }

        private void Window_Closing(object sender, CancelEventArgs e) {
        }




        #region methods for dealing with custom game data

        #endregion

    }
}
