using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MASGAU.Location.Holders;
using Translator;
using Translator.WPF;

namespace MASGAU.Main {
    public partial class MainWindowNew {
        public void bindSettingsControls() {
            //ignoreDatesChk.DataContext = Core.settings;
            //backupPathTxt.DataContext = Core.settings;
            //openBackupPathBtn.DataContext = Core.settings;
            //steamPathTxt.DataContext = Core.settings;

            //extraBackupsTgl.DataContext = Core.settings;
            //versioningCountTxt.DataContext = Core.settings;
            //versioningMaxTxt.DataContext = Core.settings;
            //versioningUnitCombo.DataContext = Core.settings;

            //emailTxt.DataContext = Core.settings;

            //altPathLst.DataContext = Core.settings.save_paths;
        }

        #region Path choosing stuff
        public bool overrideSteamPath() {
            string old_path = Core.settings.steam_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = false;
            folderBrowser.Description = Strings.GetLabelString("SelectSteamPath");
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(WPFHelpers.GetIWin32Window(this)) == System.Windows.Forms.DialogResult.OK) {
                    Core.settings.steam_path = folderBrowser.SelectedPath;
                    if (Core.settings.steam_path == folderBrowser.SelectedPath || Core.settings.steam_path != old_path)
                        return true;
                    else
                        TranslationHelpers.showTranslatedWarning(this, "SelectSteamPathRejected");
                } else {
                    try_again = false;
                }
            } while (try_again);
            return false;
        }

        public bool changeBackupPath() {
            string old_path = Core.settings.backup_path;
            string new_path = null;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = Strings.GetLabelString("SelectBackupPath");
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(WPFHelpers.GetIWin32Window(this)) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (PermissionsHelper.isWritable(new_path)) {
                            Core.settings.backup_path = new_path;
                            return new_path != old_path;
                        } else {
                            TranslationHelpers.showTranslatedError(this, "SelectBackupPathWriteError");
                            try_again = true;
                        }
                    } else {
                        TranslationHelpers.showTranslatedError(this, "SelectBackupPathReadError");
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while (try_again);
            return false;
        }
        //public bool changeSyncPath() {
        //    string old_path = Core.settings.sync_path;
        //    string new_path = null;
        //    System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
        //    folderBrowser.ShowNewFolderButton = true;
        //    folderBrowser.Description = Strings.GetLabelString("SelectSyncPath");
        //    folderBrowser.SelectedPath = old_path;
        //    bool try_again = false;
        //    do {
        //        if (folderBrowser.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
        //            new_path = folderBrowser.SelectedPath;
        //            if (PermissionsHelper.isReadable(new_path)) {
        //                if (PermissionsHelper.isWritable(new_path)) {
        //                    Core.settings.sync_path = new_path;
        //                    if (new_path != old_path)
        //                        Core.rebuild_sync = true;
        //                    return new_path != old_path;
        //                } else {
        //                    TranslationHelpers.showTranslatedError(this, "SelectSyncPathWriteError");
        //                    try_again = true;
        //                }
        //            } else {
        //                TranslationHelpers.showTranslatedError(this, "SelectSyncPathReadError");
        //                try_again = true;
        //            }
        //        } else {
        //            try_again = false;
        //        }
        //    } while (try_again);
        //    return false;
        //}

        public bool addAltPath() {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = Strings.GetLabelString("SelectAltPath");
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (Core.settings.addSavePath(new_path)) {
                            try_again = false;
                            return true;
                        } else {
                            TranslationHelpers.showTranslatedError(this, "SelectAltPathDuplicate");
                            try_again = true;
                        }
                    } else {
                        TranslationHelpers.showTranslatedError(this, "SelectAltPathDuplicate");
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while (try_again);
            return false;
        }
        #endregion

        private void emailTxt_LostFocus(object sender, RoutedEventArgs e) {
            //Core.settings.email = emailTxt.Text;

        }
        private void addAltPathBtn_Click(object sender, RoutedEventArgs e) {
            if (addAltPath()) {
                Core.redetect_games = true;
            }
        }

        private void removeAltPathBtn_Click(object sender, RoutedEventArgs e) {
            List<AltPathHolder> paths = new List<AltPathHolder>();
            //foreach (AltPathHolder remove_me in altPathLst.SelectedItems) {
            //    paths.Add(remove_me);
            //}

            foreach (AltPathHolder remove_me in paths) {
                Core.settings.removeSavePath(remove_me.path);
            }
            Core.redetect_games = true;
        }

        private void altPathLst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //if (altPathLst.SelectedItems.Count > 1) {
            //    removeAltPathBtn.IsEnabled = true;
            //    TranslationHelpers.translate(removeAltPathBtn,"RemoveAltPaths");
            //} else if (altPathLst.SelectedItems.Count > 0) {
            //    removeAltPathBtn.IsEnabled = true;
            //    TranslationHelpers.translate(removeAltPathBtn,"RemoveAltPath");
            //} else {
            //    removeAltPathBtn.IsEnabled = false;
            //    TranslationHelpers.translate(removeAltPathBtn,"RemoveNoAltPaths");
            //}
        }

        private void openBackupPathBtn_Click(object sender, RoutedEventArgs e) {
            Core.openBackupPath();
        }

        private void resetSteamPathBtn_Click(object sender, RoutedEventArgs e) {
            Core.locations.resetSteam();
        }

        private void changeBackupPathBtn_Click(object sender, RoutedEventArgs e) {
            this.changeBackupPath();
        }

        private void changeSteamPathBtn_Click(object sender, RoutedEventArgs e) {
            if (this.overrideSteamPath()) {
                Core.redetect_games = true;
            }
        }
        protected void keepTextNumbersEvent(object sender, TextChangedEventArgs e) {
            TextBox txt_box = (TextBox)sender;
            int cursor = txt_box.SelectionStart;
            string new_text = Core.makeNumbersOnly(txt_box.Text);
            cursor += new_text.Length - txt_box.Text.Length;
            txt_box.Text = Core.makeNumbersOnly(txt_box.Text);
            txt_box.SelectionStart = cursor;
        }



    }
}
