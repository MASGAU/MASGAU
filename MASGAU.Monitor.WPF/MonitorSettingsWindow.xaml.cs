using System.Windows;
using System.Windows.Controls;
using Communication;
using Translator.WPF;
namespace MASGAU.Monitor {
    /// <summary>
    /// Interaction logic for MonitorSettingsWindow.xaml
    /// </summary>
    public partial class MonitorSettingsWindow : AWindow {
        public MonitorSettingsWindow() {
            InitializeComponent();
            TranslationHelpers.translateWindow(this);
//            this.gamesLst.DataContext = Core.games;

            this.syncPathText.DataContext = Core.settings;
            this.openSyncPathButton.DataContext = Core.settings;

            this.backupOnStartupCheck.DataContext = Core.settings;
            this.monitorOnLoginCheck.DataContext = Core.monitor;

            settingsControl.bindSettingsControls();
        }

        private void changeSyncPathButton_Click(object sender, RoutedEventArgs e) {
        }

        //private void openSyncPathButton_Click(object sender, RoutedEventArgs e) {
        //    Core.openSyncPath();
        //}

        private void enableBackupMenu_Click(object sender, RoutedEventArgs e) {
            foreach (Game check_me in gamesLst.SelectedItems) {
//                check_me.backup_enabled = enableBackupMenu.IsChecked;
            }

        }

        private void enableSyncMenu_Click(object sender, RoutedEventArgs e) {
            foreach (Game check_me in gamesLst.SelectedItems) {
  //              check_me.sync_enabled = enableSyncMenu.IsChecked;
            }

        }

        private void okButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void gamesLst_SizeChanged(object sender, SizeChangedEventArgs e) {
            double new_size = gamesLst.ActualWidth - backupColumn.ActualWidth - syncColumn.ActualWidth - gamePlatformColumn.ActualWidth;
            gameTitleColumn.Width = new_size * 0.5;
            defaultPathColumn.Width = new_size * 0.5;
        }

        private void gamesMenu_Opened(object sender, RoutedEventArgs e) {

            switch (gamesLst.SelectedItems.Count) {
                case 0:
                    (sender as ContextMenu).IsOpen = false;
                    return;
                case 1:
                    Game the_game = gamesLst.SelectedItem as Game;
                    //enableBackupMenu.IsChecked = the_game.backup_enabled;
                    //enableSyncMenu.IsChecked = the_game.sync_enabled;
                    return;
                default:
                    bool show_enable = false, show_disable = false;
                    bool sync_enable = false, sync_disable = false;
                    //foreach (Game game in gamesLst.SelectedItems) {
                    //    if (game.sync_enabled) {
                    //        sync_enable = true;
                    //    } else {
                    //        sync_disable = true;
                    //    }
                    //    if (game.backup_enabled) {
                    //        show_enable = true;
                    //    } else {
                    //        show_disable = true;
                    //    }
                    //}


                    if (show_disable && show_enable)
                        enableBackupMenu.IsChecked = false;
                    else if (show_enable)
                        enableBackupMenu.IsChecked = true;
                    else
                        enableBackupMenu.IsChecked = false;

                    if (sync_disable && sync_enable)
                        enableSyncMenu.IsChecked = false;
                    else if (sync_enable)
                        enableSyncMenu.IsChecked = true;
                    else
                        enableSyncMenu.IsChecked = false;
                    return;
            }

        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (tabControl1.SelectedIndex == 0) {
                if (Core.redetect_games)
                    this.redetectGames();
            }
        }
        public override void updateProgress(ProgressUpdatedEventArgs e) {
            if (e.message != null)
                progressTxt.Text = e.message;
            applyProgress(progressBar, e);
        }
        public override void disableInterface() {
            base.disableInterface();
            tabControl1.IsEnabled = false;
            okButton.IsEnabled = false;
            progressStatus.Visibility = System.Windows.Visibility.Visible;
        }
        public override void enableInterface() {
            base.enableInterface();
            tabControl1.IsEnabled = true;
            okButton.IsEnabled = true;
            progressStatus.Visibility = System.Windows.Visibility.Collapsed;
        }

    }
}
