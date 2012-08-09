using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Translator;
using Translator.WPF;
using System.IO;
using MVC.Communication;
using MASGAU.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        protected void beginRestore(List<Archive> archives) {
            Restore.RestoreWindow.beginRestore(this, archives);

        }


        private void RestoreSelected_Click(object sender, RoutedEventArgs e) {
            int selected_count = ArchiveList.SelectedItems.Count;
            if (selected_count > 0) {
                List<Archive> archives = new List<Archive>();
                foreach (Archive archive in ArchiveList.SelectedItems) {
                    archives.Add(archive);
                }
                beginRestore(archives);
            }
        }

        private void ArchiveList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            int selected_count = ArchiveList.SelectedItems.Count;

            TranslationHelpers.translate(RestoreSelected, "RestoreGames", selected_count.ToString());

            RestoreSelected.IsEnabled = selected_count > 0;

        }

        void MainWindowNew_Drop(object sender, System.Windows.DragEventArgs e) {
            if (!disabled) {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (files.Length == 0)
                    return;

                List<Archive> archives = new List<Archive>();
                foreach (string file in files) {
                    FileInfo info = new FileInfo(file);
                    if (info.Extension != Core.Extension)
                        break;
                    try {
                        archives.Add(new Archive(new FileInfo(file)));
                    } catch (Exception ex) {
                        showTranslatedError("FileNotArchive", ex, file);
                    }
                }
                beginRestore(archives);
            }
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
                    beginRestore(archives);
                }
            }
        }


    }
}
