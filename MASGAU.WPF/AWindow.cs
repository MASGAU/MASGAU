using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using Communication;
using Communication.Translator;
using Communication.WPF;
using MASGAU.Backup;
using MASGAU.Location.Holders;
using Translator;
using Translator.WPF;
namespace MASGAU {

    public abstract class AWindow : ACommunicationWindow, IWindow {
        protected ProgressBar overall_progress;


        public AWindow()
            : this(null) {
        }



        public AWindow(IWindow owner)
            : base(owner, Core.settings) {
            TabItem from_me = new TabItem();
            from_me.BeginInit();
            from_me.EndInit();
            this.Background = from_me.Background;
            ProgressBar from_color = new ProgressBar();
            default_progress_color = from_color.Foreground;

            // Taskbar progress setup
            TaskbarItemInfo = new TaskbarItemInfo();
            var uriSource = new Uri(System.IO.Path.Combine(Core.app_path, "masgau.ico"), UriKind.Relative);

            this.Icon = new BitmapImage(uriSource);

            if (owner != null) {
                this.Owner = owner as System.Windows.Window;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            } else {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }



        }




        // Stuff to purge games
        protected void purgeGames(System.Collections.IEnumerable games) {
            this.disableInterface();
            Games.purgeGames(games, purgeComplete);
        }

        private void purgeComplete(object sender, RunWorkerCompletedEventArgs e) {
            if (!e.Cancelled) {
                Games.detectGames();
                redetectGames();
            }
            this.enableInterface(null, null);
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

        public void blur() {
            ApplyEffect(this);
            this.ShowInTaskbar = false;
        }
        public void unBlur() {
            ClearEffect(this);
            this.ShowInTaskbar = true;
        }


        void resetStatus(object sender, RunWorkerCompletedEventArgs e) {
            ProgressHandler.value = 0;
            ProgressHandler.restoreMessage();
        }




        protected void redetectArchives() {
            disableInterface();
            ProgressHandler.saveMessage();
            BackgroundWorker redetect = new BackgroundWorker();
            redetect.DoWork += new DoWorkEventHandler(redetectArchives);
            redetect.RunWorkerCompleted += new RunWorkerCompletedEventHandler(enableInterface);
            redetect.RunWorkerCompleted += new RunWorkerCompletedEventHandler(resetStatus);
            redetect.RunWorkerCompleted += new RunWorkerCompletedEventHandler(redetectArchivesComplete);
            redetect.RunWorkerAsync();
        }

        protected virtual void redetectArchivesComplete(object sender, RunWorkerCompletedEventArgs e) {
            return;
        }

        private void redetectArchives(object sender, DoWorkEventArgs e) {
            if (Core.redetect_games) {
                redetectGames(sender, e);
            }
            Core.redetect_archives = false;
        }

        protected void redetectGames() {
            disableInterface();
            BackgroundWorker redetect = new BackgroundWorker();
            redetect.DoWork += new DoWorkEventHandler(redetectGames);
            redetect.RunWorkerCompleted += new RunWorkerCompletedEventHandler(enableInterface);
            redetect.RunWorkerAsync();
        }

        private void redetectGames(object sender, DoWorkEventArgs e) {
            Games.Clear();
            Games.detectGames();
            Core.redetect_games = false;
        }



        private void ApplyEffect(AWindow win) {
            System.Windows.Media.Effects.BlurEffect objBlur =
           new System.Windows.Media.Effects.BlurEffect();
            objBlur.Radius = 4;
            win.Effect = objBlur;
        }
        /// <summary> 
        /// Remove Blur Effects 
        /// </summary> 
        /// <param name=”win”></param> 
        private void ClearEffect(AWindow win) {
            win.Effect = null;
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
            string new_text = Core.makeNumbersOnly(txt_box.Text);
            cursor += new_text.Length - txt_box.Text.Length;
            txt_box.Text = Core.makeNumbersOnly(txt_box.Text);
            txt_box.SelectionStart = cursor;
        }
    }

}
