using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using MVC.Communication;
using MVC.Translator;
using MVC.WPF;
namespace MASGAU {

    public abstract class AWindow : ACommunicationWindow, IWindow {
        protected ProgressBar overall_progress;


        public AWindow()
            : this(null) {
        }



        public AWindow(IWindow owner)
            : base(owner, Common.Settings) {
            TabItem from_me = new TabItem();
            from_me.BeginInit();
            from_me.EndInit();
            this.Background = from_me.Background;
            ProgressBar from_color = new ProgressBar();
            default_progress_color = from_color.Foreground;

            // Taskbar progress setup
            TaskbarItemInfo = new TaskbarItemInfo();
            //            var uriSource = new Uri(System.IO.Path.Combine(Core.ExecutablePath, "masgau.ico"), UriKind.Relative);


            System.Drawing.Icon ico = Properties.Resources.MASGAUIcon;

            this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ico.ToBitmap().GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            if (owner != null) {
                this.Owner = owner as System.Windows.Window;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            } else {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }



        }



        // Generic delegate and dispatcher stuff so other threads can retreive information
        private delegate object getPropertyDelegate(object from_me, String get_me);
        private delegate void setPropertyDelegate(object set_me);
        protected object getPropertyDispatcher(object from_me, String get_me) {
            DispatcherOperation dispatcho = this.Dispatcher.BeginInvoke(new getPropertyDelegate(this.getProperty), from_me, get_me);
            dispatcho.Wait();
            return dispatcho.Result;
        }
        private object getProperty(object from_me, String get_me) {
            System.Reflection.PropertyInfo info = from_me.GetType().GetProperty(get_me);
            return info.GetValue(from_me, null);
        }


        public new bool ShowDialog() {
            //if (((ICommunicationReceiver)this.Owner) != null)
            //    ((AWindow)this.Owner).blur();
            bool return_me = (bool)base.ShowDialog();
            //if (((AWindow)this.Owner) != null)
            //    ((AWindow)this.Owner).unBlur();
            return return_me;
        }


        void resetStatus(object sender, RunWorkerCompletedEventArgs e) {
            ProgressHandler.value = 0;
            ProgressHandler.restoreMessage();
        }




        #region BackgroundWorker setup stuff
        private void subProgressChanged(ProgressUpdatedEventArgs e) {
            setTaskBarState(e.state);
        }
        private void progressChanged(ProgressUpdatedEventArgs e) {
            setTaskBarState(e.state);
            if (e.max == 0) {
                TaskbarItemInfo.ProgressValue = 0;
            } else {
                TaskbarItemInfo.ProgressValue = (double)e.value / (double)e.max;
            }

        }

        private void setTaskBarState(ProgressState state) {
            switch (state) {
                case ProgressState.Indeterminate:
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                    break;
                case ProgressState.None:
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                    break;
                case ProgressState.Normal:
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    break;
                case ProgressState.Error:
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                    break;
                case ProgressState.Wait:
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
                    break;
                default:
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    break;
            }
        }

        private void checkExceptions(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                TranslatingMessageHandler.SendException(e.Error);
            }
        }



        public override void requestInformation(RequestEventArgs e) {
            switch (e.info_type) {
                default:
                    base.requestInformation(e);
                    return;
            }
        }
        #endregion

        protected void keepTextNumbersEvent(object sender, TextChangedEventArgs e) {
            TextBox txt_box = (TextBox)sender;
            int cursor = txt_box.SelectionStart;
            string new_text = Common.makeNumbersOnly(txt_box.Text);
            cursor += new_text.Length - txt_box.Text.Length;
            txt_box.Text = Common.makeNumbersOnly(txt_box.Text);
            txt_box.SelectionStart = cursor;
        }
    }

}
