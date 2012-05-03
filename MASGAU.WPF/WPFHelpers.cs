using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Translations;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
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



    }
}
