using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Translations;
using System.Windows.Controls;
namespace MASGAU {
    public class WPFHelpers {
        public static bool addAltPath(AWindow window) {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = Strings.get("SelectAltPath");
            bool try_again = false;
            do {
                if(folderBrowser.ShowDialog(window.GetIWin32Window())== System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if(PermissionsHelper.isReadable(new_path)) {
                        if(Core.settings.addAltPath(new_path)){
                            try_again = false;
                            return true;
                        }else {
                            window.showTranslatedError("SelectAltPathDuplicate");
                            try_again = true;
                        }
                    } else {
                        window.showError(Strings.get("ReadWriteErrorTitle"),Strings.get("SelectAltPathReadError") + ":" + Environment.NewLine + new_path);
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while(try_again);
            return false;
        }

        public static void translateTitle(AWindow window) {
            string string_title = window.Title.ToString();
            window.Title = Strings.get(string_title);
        }
        public static void translateText(TextBlock text) {
            string string_title = text.Text.ToString();
            text.Text = Strings.get(string_title);
        }
        public static void translateContent(ContentControl control) {
            string string_title = control.Content.ToString();
            control.Content = Strings.get(string_title);
        }
        public static void translateHeader(HeaderedContentControl control) {
            string string_title = control.Header.ToString();
            control.Header = Strings.get(string_title);
        }
        public static void translateColumnHeader(GridViewColumn control) {
            string string_title = control.Header.ToString();
            control.Header = Strings.get(string_title);
        }

        public static bool checkEmail(AWindow parent) {
            if (Core.settings.email == null || Core.settings.email == "") {
                EmailWindow get_email = new EmailWindow(parent);
                if ((bool)get_email.ShowDialog()) {
                    Core.settings.email = get_email.email;
                }
                else {
                    return false;
                }
            }
            return true;
        }
    }
}
