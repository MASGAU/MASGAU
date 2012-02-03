using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MASGAU.Communication.Message;
using MASGAU.Location;
using MASGAU.Location.Holders;

namespace MASGAU
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        public void bindSettingsControls() {
            autoUpdateChk.DataContext = Core.settings;
            ignoreDatesChk.DataContext = Core.settings;
            backupPathTxt.DataContext = Core.settings;
            openBackupPathBtn.DataContext = Core.settings;
            steamPathTxt.DataContext = Core.settings;

            extraBackupsTgl.DataContext = Core.settings;
            versioningCountTxt.DataContext = Core.settings;
            versioningMaxTxt.DataContext = Core.settings;
            versioningUnitCombo.DataContext = Core.settings;
            
            emailTxt.DataContext = Core.settings;

            altPathLst.DataContext = Core.settings.alt_paths;
        }


        private void emailTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            Core.settings.email = emailTxt.Text;

        }
        private void addAltPathBtn_Click(object sender, RoutedEventArgs e)
        {
            if(addAltPath()) {
                Core.redetect_games = true;
            }
        }

        private void removeAltPathBtn_Click(object sender, RoutedEventArgs e)
        {
            List<AltPathHolder> paths = new List<AltPathHolder>();
            foreach(AltPathHolder remove_me in altPathLst.SelectedItems) {
                paths.Add(remove_me);
            }

            foreach(AltPathHolder remove_me in paths) {
                Core.settings.removeAltPath(remove_me.path);
            }
            Core.redetect_games = true;
        }

        private void altPathLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (altPathLst.SelectedItems.Count> 1){
                removeAltPathBtn.IsEnabled = true;
                removeAltPathBtn.Content = "Remove Paths";
            }
            else if (altPathLst.SelectedItems.Count > 0){
                removeAltPathBtn.IsEnabled = true;
                removeAltPathBtn.Content = "Remove Path";
            }
            else{
                removeAltPathBtn.IsEnabled = false;
                removeAltPathBtn.Content = "Remove Nothing";
            }
        }

        private void openBackupPathBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.openBackupPath();
        }

        private void resetSteamPathBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.locations.resetSteam();
        }

        private void changeBackupPathBtn_Click(object sender, RoutedEventArgs e)
        {
            getWindow().changeBackupPath();
        }

        private void changeSteamPathBtn_Click(object sender, RoutedEventArgs e)
        {
            if(getWindow().overrideSteamPath()) {
                Core.redetect_games = true;
            }
        }
        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            getWindow().checkUpdates(false,false);
        }
    
        protected void keepTextNumbersEvent(object sender, TextChangedEventArgs e)
        {
            TextBox txt_box = (TextBox)sender;
            int cursor = txt_box.SelectionStart;
            string new_text = Core.makeNumbersOnly(txt_box.Text);
            cursor += new_text.Length - txt_box.Text.Length;
            txt_box.Text = Core.makeNumbersOnly(txt_box.Text);
            txt_box.SelectionStart = cursor;
        }

        protected bool addAltPath() {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Choose a new Alternative Install Path.";
            bool try_again = false;
            do {
                if(folderBrowser.ShowDialog(getWindow().GetIWin32Window())== System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if(PermissionsHelper.isReadable(new_path)) {
                        if(Core.settings.addAltPath(new_path)){
                            try_again = false;
                            return true;
                        }else {
                            MessageHandler.SendError("Duplication","The path you selected has already been added.");
                            try_again = true;
                        }
                    } else {
                        MessageHandler.SendError("Can't Read Selected Folder","Due to permissions or something, MASGAU can't read the folder you selected.");
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while(try_again);
            return false;
        }

        private AWindow getWindow() {
            return AWindow.GetWindow(this) as AWindow;
        }


    }

}
