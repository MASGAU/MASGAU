using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Translations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Translations.WPF
{
    public abstract class ATranslateableWindow : System.Windows.Window
    {
        protected ATranslateableWindow(Window owner): base()
        {
        }

        protected void translateThisWindow()
        {
            translateWindow(this);
        }

        #region Translating methods
        public static void translateWindow(Window window)
        {
            translateTitle(window);
            translateRecursively(window.Content as UIElement);
        }

        public static void translateControl(UserControl control)
        {
            translateRecursively(control.Content as UIElement);
        }

        private static void translateRecursively(UIElement obj)
        {
            if (obj is FrameworkElement)
            {
                FrameworkElement fe = obj as FrameworkElement;
                if (fe.ContextMenu != null)
                {
                    foreach (MenuItem item in fe.ContextMenu.Items)
                    {
                        translateMenuItem(item);
                    }
                }
            }

            if (obj == null ||
                obj is TextBox ||
                  obj is ProgressBar ||
                  obj is ComboBox ||
                  obj is Image ||
                  obj is TreeView ||
                obj is PasswordBox)
            {
            }
            else if (obj is Grid)
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
                foreach (UIElement element in bar.Items)
                {
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
            else if (obj is Button || obj is Label || obj is CheckBox || obj is ToggleButton)
            {
                translateContent(obj as ContentControl);
            }
            else if (obj is ListView)
            {
                ListView list = obj as ListView;
                if (list.View != null)
                {
                    GridView view = list.View as GridView;
                    if (view.Columns != null)
                    {
                        foreach (GridViewColumn col in view.Columns)
                        {
                            translateColumnHeader(col);
                        }
                    }
                }
            }
            else if (obj is TextBlock)
            {
                translateText(obj as TextBlock);
            }
            else if (obj is UserControl)
            {
                translateControl(obj as UserControl);
            }
            else
            {
                throw new Exception("Can't translate object");
            }
        }


        private static void translateTitle(Window window)
        {
            string string_title = window.Title.ToString();
            window.Title = Strings.getInterfaceString(string_title);
        }

        private static void translateText(TextBlock text)
        {
            string string_title = text.Text.ToString();
            if (string_title != "")
            {
                text.Text = Strings.getInterfaceString(string_title);
            }
        }
        private static void translateMenuItem(MenuItem item)
        {
            string string_title = item.Header.ToString();
            if (string_title != "")
            {
                item.Header = Strings.getInterfaceString(string_title);
            }
        }

        private static void translateContent(ContentControl control)
        {
            if (control.Content == null)
            {
                control.Content = Strings.getInterfaceString(null);
            }
            else if (control.Content is TextBlock)
            {
                translateText(control.Content as TextBlock);
            }
            else
            {
                string string_title = control.Content.ToString();
                control.Content = Strings.getInterfaceString(string_title);

            }
        }
        private static void translateHeader(HeaderedContentControl control)
        {
            if (control.Header == null)
            {
                control.Header = Strings.getInterfaceString(null);
                return;
            }
            string string_title = control.Header.ToString();
            control.Header = Strings.getInterfaceString(string_title);
        }
        private static void translateColumnHeader(GridViewColumn control)
        {
            if (control.Header == null)
            {
                control.Header = Strings.getInterfaceString(null);
                return;
            }
            string string_title = control.Header.ToString();
            control.Header = Strings.getInterfaceString(string_title);
        }
        #endregion


        protected abstract bool askQuestion(string title, string message);
        protected abstract bool showError(string title, string message);
        protected abstract bool showWarning(string title, string message);
        protected abstract bool showInfo(string title, string message);

        #region TranslatedMessageBoxes
        public bool askTranslatedQuestion(String string_name, params string[] variables)
        {
            StringCollection mes = Strings.getTitleMessagePair(string_name);
            return askQuestion(mes[StringType.Title],
                mes[StringType.Message]);
        }
        public bool showTranslatedWarning(String string_name, params string[] variables)
        {
            StringCollection mes = Strings.getTitleMessagePair(string_name);
            return showWarning(mes[StringType.Title],
                mes[StringType.Message]);
        }
        public bool showTranslatedError(String string_name, params string[] variables)
        {
            StringCollection mes = Strings.getTitleMessagePair(string_name);
            return showError(mes[StringType.Title],
                mes[StringType.Message]);
        }
        public bool showTranslatedInfo(String string_name, params string[] variables)
        {
            StringCollection mes = Strings.getTitleMessagePair(string_name);
            return showInfo(mes[StringType.Title],
                mes[StringType.Message]);
        }
        #endregion
    }
}
