using System;
using System.Windows;
using System.Windows.Media.Imaging;
using MVC.WPF;
using Translator;
namespace MASGAU {
    public class NewWindow : ACommunicationWindow {


        public NewWindow()
            : this(null) {

        }
        public NewWindow(ACommunicationWindow owner)
            : base(owner, Common.Settings) {
            this.Owner = owner;
            //            var uriSource = new Uri(System.IO.Path.Combine(Core.ExecutablePath, "masgau.ico"), UriKind.Relative);
            //          this.Icon = new BitmapImage(uriSource);

            System.Drawing.Icon ico = Properties.Resources.MASGAUIcon;

            this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ico.ToBitmap().GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

        }

        protected bool toggleMinimize() {
            if (this.WindowState == System.Windows.WindowState.Minimized) {
                this.WindowState = System.Windows.WindowState.Normal;
                return false;
            } else {
                this.WindowState = System.Windows.WindowState.Minimized;
                return true;
            }
        }


        public bool addSavePath(AWindow window) {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = Strings.GetLabelString("SelectAltPath");
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(window.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (Common.Settings.addSavePath(new_path)) {
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

        public string promptForPath(string description, Environment.SpecialFolder root, string path) {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = description;

            if (path != null)
                folderBrowser.SelectedPath = path;


            folderBrowser.RootFolder = root;


            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    try_again = true;
                    return new_path;
                } else {
                    try_again = false;
                }
            } while (try_again);
            return null;
        }


        public bool changeSyncPath() {
            string old_path = Common.Settings.sync_path;
            string new_path = null;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Choose where the saves will be synced.";
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (PermissionsHelper.isWritable(new_path)) {
                            Common.Settings.sync_path = new_path;
                            //if (new_path != old_path)
                              //  Core.rebuild_sync = true;
                            return new_path != old_path;
                        } else {
                            this.displayError("Config File Error", "You don't have permission to write to the selected sync folder:" + Environment.NewLine + new_path);
                            try_again = true;
                        }
                    } else {
                        this.displayError("Config File Error", "You don't have permission to read from the selected sync folder:" + Environment.NewLine + new_path);
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while (try_again);
            return false;
        }
    }
}
