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

        public static void translateWindow(AWindow window)
        {
            translateTitle(window);
            translateRecursively(window.Content as UIElement);
        }

        public static void translateControl(UserControl control)
        {
            translateRecursively(control.Content as UIElement);
        }

        private static void translateRecursively(UIElement obj) {
          if (obj ==null||
              obj is TextBox ||
                obj is ProgressBar ||
                obj is ComboBox ||
                obj is Image) {
          } else            if (obj is Grid)
            {
                Grid grid = obj as Grid;
                foreach (UIElement elem in grid.Children)
                {
                    translateRecursively(elem);
                }
            }
            else if (obj is StatusBar)
            {
                StatusBar bar = obj as StatusBar;
                foreach(UIElement element in bar.Items) {
                    translateRecursively(element);
                }
            }
            else if (obj is TabControl)
            {
                TabControl tabs = obj as TabControl;
                foreach (TabItem item in tabs.Items)
                {
                    translateRecursively(item);
                }
            }
            else if (obj is StatusBarItem)
            {
                StatusBarItem item = obj as StatusBarItem;
                translateRecursively(item.Content as UIElement);
            }
            else if (obj is GroupBox || obj is TabItem || obj is Expander)
            {
                HeaderedContentControl head = obj as HeaderedContentControl;
                translateHeader(head);
                translateRecursively(head.Content as UIElement);
            }
            else if (obj is Button || obj is Label)
            {
                translateContent(obj as ContentControl);
            }
            else if (obj is ListView)
            {
                ListView list = obj as ListView;
                GridView view = list.View as GridView;
                foreach (GridViewColumn col in view.Columns)
                {
                    translateColumnHeader(col);
                }
            }
   
            else
            {
                throw new Exception("Can't translate object");
            }
        }

        private static void translateTitle(AWindow window) {
            string string_title = window.Title.ToString();
            window.Title = Strings.get(string_title);
        }
        private static void translateText(TextBlock text) {
            string string_title = text.Text.ToString();
            text.Text = Strings.get(string_title);
        }
        private static void translateContent(ContentControl control)
        {
            if (control.Content == null)
            {
                control.Content = Strings.get(null);
            } else if (control.Content is TextBlock)
            {
                translateText(control.Content as TextBlock);
            }
            else
            {
                string string_title = control.Content.ToString();
                control.Content = Strings.get(string_title);

            }
        }
        private static void translateHeader(HeaderedContentControl control) {
            if (control.Header == null)
            {
                control.Header = Strings.get(null);
                return;
            }
            string string_title = control.Header.ToString();
            control.Header = Strings.get(string_title);
        }
        private static void translateColumnHeader(GridViewColumn control) {
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
