using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Translator;
using Translator.WPF;
using System.IO;
using MASGAU.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        private void RestoreSelected_Click(object sender, RoutedEventArgs e) {
            int selected_count = ArchiveList.SelectedItems.Count;
            if (selected_count > 0) {
                List<Archive> archives = new List<Archive>();
                foreach (Archive archive in ArchiveList.SelectedItems) {
                    archives.Add(archive);
                }
                Restore.RestoreWindow.beginRestore(this, archives);
            }
        }

        private void ArchiveList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            int selected_count = ArchiveList.SelectedItems.Count;

            TranslationHelpers.translate(RestoreSelected, "RestoreGames", selected_count.ToString());

            RestoreSelected.IsEnabled = selected_count > 0;

        }



        private void RestoreOther_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.AutoUpgradeEnabled = true;
            open.CheckFileExists = true;
            open.CheckPathExists = true;
            open.DefaultExt = "gb7";
            open.Filter = Strings.GetLabelString("Gb7FileDescription") + " (*.gb7)|*.gb7";
            open.Multiselect = true;
            open.Title = Strings.GetLabelString("SelectBackup");
            if (open.ShowDialog(GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                if (open.FileNames.Length > 0) {
                    List<Archive> archives = new List<Archive>();
                    foreach (string file in open.FileNames) {
                        try {
                            archives.Add(new Archive(new FileInfo(file)));
                        } catch (Exception ex) {
                            showTranslatedError("FileNotArchive", ex, file);
                        }
                    }
                    Restore.RestoreWindow.beginRestore(this, archives);
                }
            }
        }


    }
}
