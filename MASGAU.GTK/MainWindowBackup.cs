using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using MASGAU.Backup;
using MASGAU.Location.Holders;
using MVC;
using MVC.Communication;
using MVC.Translator;

namespace MASGAU.GTK {
	public partial class MainWindow {

		public bool changeBackupPath() {

//			string old_path = Core.settings.backup_path;
//			string new_path = null;
//			System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
//			folderBrowser.ShowNewFolderButton = true;
//			folderBrowser.Description = Strings.GetLabelString("SelectBackupPath");
//			folderBrowser.SelectedPath = old_path;
//			bool try_again = false;
//			do {
//				if (folderBrowser.ShowDialog(GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
//					new_path = folderBrowser.SelectedPath;
//					if (PermissionsHelper.isReadable(new_path)) {
//						if (PermissionsHelper.isWritable(new_path)) {
//							Core.settings.backup_path = new_path;
//							return new_path != old_path;
//						} else {
//
//							this.showTranslatedError("SelectBackupPathWriteError");
//							try_again = true;
//						}
//					} else {
//						this.showTranslatedError("SelectBackupPathReadError");
//						try_again = true;
//					}
//				} else {
//					try_again = false;
//				}
//			} while (try_again);
			return false;
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

