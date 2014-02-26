using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using MASGAU.Backup;
using MASGAU.Location.Holders;
using MVC.Communication;
using MVC.Translator;
using Translator;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        private void BackupAllGames_Click(object sender, RoutedEventArgs e) {
            if (Core.settings.IsBackupPathSet || changeBackupPath()) {
                beginBackup(null);
            }

        }
        private void BackupSelectedGames_Click(object sender, RoutedEventArgs e) {
            if (Core.settings.IsBackupPathSet || changeBackupPath()) {
                List<GameEntry> these = new List<GameEntry>();

                foreach (GameEntry game in gamesLst.SelectedItems) {
                    if (game.IsDetected) {
                        these.Add(game);
                    }
                }
                beginBackup(these, null);
            }

        }

        private string last_archive_create = null;
        private void CustomBackup_Click(object sender, RoutedEventArgs e) {
            GameEntry game = gamesLst.SelectedItem as GameEntry;
            ManualArchiveWindow manual = new ManualArchiveWindow(game, this);
            if ((bool)manual.ShowDialog()) {

                List<DetectedFile> selected_files = manual.getSelectedFiles();
                DateTime right_now = DateTime.Now;

                if (selected_files.Count > 0) {
                    string initial_directory;
                    if (String.IsNullOrEmpty(last_archive_create)) {
                        initial_directory = Core.settings.backup_path;
                    } else {
                        initial_directory = last_archive_create;
                    }
                    ArchiveID archive = new ArchiveID(game.id, selected_files[0]);

                    StringBuilder initial_name = new StringBuilder(archive.ToString());

                    initial_name.Append(Core.owner_seperator + right_now.ToString().Replace('/', '-').Replace(':', '-'));

                    Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
                    save.Title = Strings.GetLabelString("WhereSaveArchive");
                    save.AddExtension = true;
                    save.InitialDirectory = initial_directory;
                    save.FileName = initial_name.ToString();
                    ;
                    save.DefaultExt = "gb7";
                    save.Filter = Strings.GetLabelString("Gb7FileDescription") + " (*.gb7)|*.gb7";
                    save.OverwritePrompt = true;

                    if ((bool)save.ShowDialog(this)) {
                        string file = save.FileName;
                        beginBackup(game, selected_files, file, null);
                        last_archive_create = Path.GetDirectoryName(file);
                    }
                }
            }
        }

        private BackupProgramHandler backup;
        protected void cancelBackup() {
            TranslatingProgressHandler.setTranslatedMessage("Cancelling");
            if (backup != null && backup.IsBusy)
                backup.CancelAsync();
        }
        protected void beginBackup(RunWorkerCompletedEventHandler when_done) {
            backup = new BackupProgramHandler(Core.locations);
            startBackup(when_done);
        }
        protected void beginBackup(List<GameEntry> backup_list, RunWorkerCompletedEventHandler when_done) {
            backup = new BackupProgramHandler(backup_list, Core.locations);
            startBackup(when_done);
        }
        protected void beginBackup(GameEntry game, List<DetectedFile> files, string archive_name, RunWorkerCompletedEventHandler when_done) {
            backup = new BackupProgramHandler(game, files, archive_name, Core.locations);
            startBackup(when_done);
        }

        private void startBackup(RunWorkerCompletedEventHandler when_done) {
            ProgressHandler.saveMessage();
            backup.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backup_RunWorkerCompleted);
            disableInterface(backup.worker);
            backup.RunWorkerAsync();
        }

        void backup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            endOfOperations();
            updateArchiveList();
        }



    }
}
