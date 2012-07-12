using Translator;
using Translator.WPF;
namespace MASGAU {
    public class WPFHelpers {
        public static bool addSavePath(AWindow window) {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = Strings.GetLabelString("SelectAltPath");
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(window.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (Core.settings.addSavePath(new_path)) {
                            try_again = false;
                            return true;
                        } else {
                            TranslationHelpers.showTranslatedError(window, "SelectAltPathDuplicate");
                            try_again = true;
                        }
                    } else {
                        TranslationHelpers.showTranslatedError(window, "SelectAltPathDuplicate");
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while (try_again);
            return false;
        }

        public static string promptForPath(System.Windows.Window parent, string description) {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = description;
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(GetIWin32Window(parent)) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    try_again = true;
                    return new_path;
                } else {
                    try_again = false;
                }
            } while (try_again);
            return null;
        }

        #region stuff for interacting with windows.forms controls
        // Ruthlessly stolen from http://stackoverflow.com/questions/315164/how-to-use-a-folderbrowserdialog-from-a-wpf-application

        public static System.Windows.Forms.IWin32Window GetIWin32Window(System.Windows.Window window) {
            var source = System.Windows.PresentationSource.FromVisual(window) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle) {
                _handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle {
                get { return _handle; }
            }
            #endregion
        }
        #endregion

    }
}
