using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MVC;
using MVC.Communication;
using Translator.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew {
        #region Methods For The Games List
        private void redetectGamesBtn_Click(object sender, RoutedEventArgs e) {
            redetectGames();
        }


        #region redetect games

        private void askRefreshGames(string str) {
            if (str == null)
                str = "AskRefreshGames";

            if (this.askTranslatedQuestion(str, false)) {
                this.redetectGames();
            }
        }

        protected void redetectGames() {
            gamesLst.DeselectAll();
            updateArchiveList();
            masgau.detectGamesAsync();
        }

        #endregion

        protected void updateArchiveList() {
            int selected_count = Games.SelectedItems.Count;

            TranslationHelpers.translate(BackupSelectedGames, "BackupGames", selected_count.ToString());

            if (selected_count > 0) {
                BackupSelectedGames.IsEnabled = true;
                PurgeButton.IsEnabled = true;
                if (selected_count == 1) {
                    CustomBackup.IsEnabled = true;
                } else {
                    CustomBackup.IsEnabled = false;
                }
            } else {
                PurgeButton.IsEnabled = false;
                CustomBackup.IsEnabled = false;
                BackupSelectedGames.IsEnabled = false;
                RestoreSelected.IsEnabled = false;
                deleteGame.IsEnabled = false;
            }

            Model<ArchiveID, Archive> archives = new Model<ArchiveID, Archive>();
            deleteGame.IsEnabled = Games.OnlyCustomGamesSelected;
            archives.AddRange(Games.SelectedGamesArchives);

            if (ArchiveList.DataContext != null) {
                lock (ArchiveList.DataContext) {
                    foreach (Archive archive in (Model<ArchiveID, Archive>)ArchiveList.DataContext) {
                        if (!archives.containsId(archive.id))
                            archive.IsSelected = false;
                    }
                }
            }

            ArchiveList.DataContext = archives;
            ArchiveList.Model = archives;

            if (archives.Count > 0) {
                ArchiveColumn.MinWidth = 300;
                ArchiveColumn.Width = new GridLength(2, GridUnitType.Star);
                ArchiveGrid.Visibility = System.Windows.Visibility.Visible;
                SelectGameLabel.Visibility = System.Windows.Visibility.Collapsed;
                TranslationHelpers.translate(ArchiveCount, "NumberOfArchives", archives.Count.ToString(), gamesLst.SelectedItems.Count.ToString());
            } else {
                ArchiveColumn.MinWidth = 0;
                ArchiveColumn.Width = new GridLength(0, GridUnitType.Pixel);
                ArchiveGrid.Visibility = System.Windows.Visibility.Collapsed;
                SelectGameLabel.Visibility = System.Windows.Visibility.Visible;
            }

            ListSplitter.Visibility = ArchiveGrid.Visibility;
            updateRestoreButton();
        }

        private void gamesLst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            updateArchiveList();
        }


        private void gamesLst_DragOver(object sender, DragEventArgs e) {
            resizeGameColumns();
        }
        private void gamesLst_SizeChanged(object sender, SizeChangedEventArgs e) {
            resizeGameColumns();
        }
        private void resizeGameColumns() {
            double monitor_width = monitorColumnLabel.ActualWidth;
            //            gameMonitorColumn.Width = monitor_width;
            //          double new_width = gamesLst.ActualWidth - gameNameColumn.Width - gameMonitorColumn.Width - gameLinkColumn.Width - 20;
            //        if (new_width > 0) {
            //          gameTitleColumn.Width = new_width;
            //    } else {
            //      gameTitleColumn.Width = 0;
            // }

        }
        #endregion


    }
}
