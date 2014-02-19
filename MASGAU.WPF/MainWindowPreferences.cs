﻿using System;
using System.Windows;
using System.Windows.Controls;
using MASGAU.Location.Holders;
using MASGAU.Settings;
using Microsoft.Windows.Controls.Ribbon;
using Translator;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        private void versioningTimingUnit_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            Core.settings.VersioningUnit = (VersioningUnit)versioningTimingUnit.SelectedIndex;
        }

        public void bindSettingsControls() {
            versioningTimingUnit.Items.Clear();
            foreach (VersioningUnit suit in Enum.GetValues(typeof(VersioningUnit))) {
                versioningTimingUnit.Items.Add(Strings.GetLabelString(suit.ToString()));
            }
            versioningTimingUnit.SelectedIndex = (int)Core.settings.VersioningUnit;

            versioningButton.DataContext = Core.settings;

            versioningMax.DataContext = Core.settings;
            versioningTiming.DataContext = Core.settings;

            versioningTimingUnit.DataContext = Core.settings;

            ignoreDates.DataContext = Core.settings;
            autoStart.DataContext = Core.startup;

            emailText.DataContext = Core.settings;

            enableDebug.DataContext = Core.settings;
        }

        private void populateAltPaths() {
            AltSaveButton.Items.Clear();
            AltSaveButton.Items.Add(AddAltSaveFolder);
            foreach (AltPathHolder alt in Core.settings.save_paths) {
                RibbonMenuItem item = new RibbonMenuItem();
                item.Header = Strings.GetLabelString("RemoteAltSavePath", alt.path);
                item.ToolTip = alt.path;
                item.Click += new RoutedEventHandler(item_Click);
                AltSaveButton.Items.Add(item);
            }
        }

        void item_Click(object sender, RoutedEventArgs e) {
            RibbonMenuItem item = sender as RibbonMenuItem;
            string name = item.ToolTip.ToString();
            Core.settings.removeSavePath(name);
            populateAltPaths();
            askRefreshGames("RefreshForRemovedSavePath");
        }


        private void OverrideSteamButton_Click(object sender, RoutedEventArgs e) {
            if (overrideSteamPath())
                askRefreshGames("RefreshForNewSteamPath");
            setupSteamButton();
        }
        protected void setupSteamButton() {
            if (Core.locations.steam_detected) {
                OverrideSteamButton.ToolTip = Strings.GetToolTipString("SteamFound", Core.locations.steam_path);
                SteamImage.Opacity = 1.0;
            } else {
                OverrideSteamButton.ToolTip = Strings.GetToolTipString("SteamNotFound");
                SteamImage.Opacity = 0.5;
            }
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
                if (folderBrowser.ShowDialog(GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    Core.settings.steam_path = folderBrowser.SelectedPath;
                    if (Core.settings.steam_path == folderBrowser.SelectedPath)
                        return Core.settings.steam_path != old_path;
                    else
                        this.showTranslatedWarning("SelectSteamPathRejected");
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
            folderBrowser.Description = Strings.GetTitleString("SelectBackupPath");
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (PermissionsHelper.isWritable(new_path)) {
                            Core.settings.backup_path = new_path;
                            return new_path != old_path;
                        } else {

                            this.showTranslatedError("SelectBackupPathWriteError");
                            try_again = true;
                        }
                    } else {
                        this.showTranslatedError("SelectBackupPathReadError");
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
                            this.showTranslatedError("SelectAltPathDuplicate");
                            try_again = true;
                        }
                    } else {
                        this.showTranslatedError("SelectAltPathDuplicate");
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
                askRefreshGames("");
            }
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

        private void ChangeSyncFolder_Click(object sender, RoutedEventArgs e) {

        }

        private void OpenSyncFolder_Click(object sender, RoutedEventArgs e) {

        }

        private void AddAltSaveFolder_Click(object sender, RoutedEventArgs e) {
            if (addAltPath()) {
                populateAltPaths();
                askRefreshGames("RefreshForNewSavePath");
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
