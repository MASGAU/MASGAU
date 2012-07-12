using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVC;
using Translator.WPF;
using System.Windows;
using System.Windows.Controls;
namespace MASGAU.Main {
    public partial class MainWindowNew {
        #region Methods For The Games List
        private void redetectGamesBtn_Click(object sender, RoutedEventArgs e) {
            redetectGames();
        }

        private void gamesLst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selected_count = gamesLst.SelectedItems.Count;
            TranslationHelpers.translate(BackupSelectedGames, "BackupGames",selected_count.ToString());

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
            }

            Model<ArchiveID, Archive> archives = new Model<ArchiveID, Archive>();
            foreach (GameVersion game in gamesLst.SelectedItems) {
                archives.AddRange(game.Archives);
            }
            if (ArchiveList.DataContext != null) {
                foreach (Archive archive in (Model<ArchiveID, Archive>)ArchiveList.DataContext) {
                    if(!archives.containsId(archive.id))
                        archive.IsSelected = false;
                }
            }
            ArchiveList.DataContext = archives;
            ArchiveList.ItemsSource = archives;
            if (archives.Count > 0) {
                ArchiveGrid.Visibility = System.Windows.Visibility.Visible;
                TranslationHelpers.translate(ArchiveCount, "NumberOfArchives",archives.Count.ToString(),gamesLst.SelectedItems.Count.ToString());
            } else {
                ArchiveGrid.Visibility = System.Windows.Visibility.Collapsed;
            }

        }


        private void gamesLst_DragOver(object sender, DragEventArgs e) {
            resizeGameColumns();
        }
        private void gamesLst_SizeChanged(object sender, SizeChangedEventArgs e) {
            resizeGameColumns();
        }
        private void resizeGameColumns() {
            double new_width = gamesLst.ActualWidth - gameNameColumn.Width -  20;
            if (new_width > 0) {
                gameTitleColumn.Width = new_width;
            } else {
                gameTitleColumn.Width = 0;
            }

        }
        #endregion


    }
}
