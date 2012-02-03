using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.ComponentModel;
using MASGAU.Location.Holders;
using MASGAU.Communication;
using MASGAU.Communication.Progress;
using MASGAU.Communication.Message;
using MASGAU.Communication.Request;
using MASGAU.Archive;
using MASGAU.Game;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using MASGAU.Backup;
using Translations;
namespace MASGAU
{

    public abstract class AWindow: System.Windows.Window, ICommunicationReceiver
    {
        protected Brush default_progress_color;
        protected ProgressBar overall_progress;
        
        protected bool _available = true;
        public bool available {
            get {
                return _available;
            }
        }

        private SynchronizationContext _context;
        public SynchronizationContext context {
            get {
                return _context;
            }
        }

        public AWindow(): this(null) {
        }

        public AWindow(AWindow owner): base() {
            TabItem from_me = new TabItem();
            from_me.BeginInit();
            from_me.EndInit();
            this.Background = from_me.Background;
            ProgressBar from_color = new ProgressBar();
            default_progress_color = from_color.Foreground;

            // Taskbar progress setup
            TaskbarItemInfo = new TaskbarItemInfo();
            var uriSource = new Uri(System.IO.Path.Combine(Core.app_path,"masgau.ico"), UriKind.Relative);
            this.Icon = new BitmapImage(uriSource);
            if(owner!=null) {
                this.Owner = owner;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            } else {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }

            // Jumplist setup
            JumpList masgau_jump_list = JumpList.GetJumpList(Application.Current);
            if(masgau_jump_list==null) {
                masgau_jump_list = new JumpList();
                JumpList.SetJumpList(Application.Current, masgau_jump_list);
            } else {
                masgau_jump_list.JumpItems.Clear();
                masgau_jump_list.ShowFrequentCategory = false;
                masgau_jump_list.ShowRecentCategory = false;

            }

            JumpTask masgau_jump = new JumpTask();
            masgau_jump.ApplicationPath = Path.Combine(Core.app_path,"MASGAU.Main.WPF.exe");
            masgau_jump.IconResourcePath = Path.Combine(Core.app_path,"masgau.ico");
            masgau_jump.WorkingDirectory = Core.app_path;
            masgau_jump.Title = Strings.get("JumpMainProgram");
            masgau_jump.Description = Strings.get("JumpMainProgramDescription");
            masgau_jump.CustomCategory = "MASGAU";
            masgau_jump_list.JumpItems.Add(masgau_jump);

            masgau_jump = new JumpTask();
            masgau_jump.ApplicationPath = Path.Combine(Core.app_path,"MASGAU.Main.WPF.exe");
            masgau_jump.IconResourcePath = Path.Combine(Core.app_path,"masgau.ico");
            masgau_jump.WorkingDirectory = Core.app_path;
            masgau_jump.Title = Strings.get("JumpMainProgramAllUsers");
            masgau_jump.Description = Strings.get("JumpMainProgramAllUsersDescription");
            masgau_jump.Arguments = "-allusers";
            masgau_jump.CustomCategory = "MASGAU";
            masgau_jump_list.JumpItems.Add(masgau_jump);

            masgau_jump = new JumpTask();
            masgau_jump.ApplicationPath = Path.Combine(Core.app_path,"MASGAU.Analyzer.WPF.exe");
            masgau_jump.IconResourcePath = Path.Combine(Core.app_path,"masgau.ico");
            masgau_jump.WorkingDirectory = Core.app_path;
            masgau_jump.Title = Strings.get("JumpAnalyzer");
            masgau_jump.Description = Strings.get("JumpAnalyzerDescription");
            masgau_jump.CustomCategory = "MASGAU";
            masgau_jump_list.JumpItems.Add(masgau_jump);
            
            masgau_jump = new JumpTask();
            masgau_jump.ApplicationPath = Path.Combine(Core.app_path,"MASGAU.Monitor.WPF.exe");
            masgau_jump.IconResourcePath = Path.Combine(Core.app_path,"masgau.ico");
            masgau_jump.WorkingDirectory = Core.app_path;
            masgau_jump.Title = Strings.get("JumpMonitor");
            masgau_jump.Description = Strings.get("JumpMonitorDescription");
            masgau_jump.CustomCategory = "MASGAU";
            masgau_jump_list.JumpItems.Add(masgau_jump);

            masgau_jump_list.Apply();

            //These intitialize the contexts of the CommunicationHandlers
            if(SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(this.Dispatcher));
            _context = SynchronizationContext.Current;

            CommunicationHandler.addReceiver(this);

            this.Closing += new CancelEventHandler(Window_Closing);
            this.Loaded += new System.Windows.RoutedEventHandler(AWindow_Loaded);
            
        }

        void AWindow_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            loadTranslations();
        }

        protected virtual void loadTranslations() {
            //throw new NotImplementedException();
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            _available = false;
        }

        protected Boolean disable_close = false;
        public virtual void disableInterface() {
            disable_close = true;
        }
        public virtual void enableInterface() {
            disable_close = false;
        }
        protected void enableInterface(object sender, RunWorkerCompletedEventArgs e) {
            this.enableInterface();
        }
        public void closeInterface() {
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(disable_close) {
                e.Cancel = true;
            } else {
                base.OnClosing(e);
            }

        }

        public void hideInterface() {
            this.Visibility = System.Windows.Visibility.Hidden;
        }
        public void showInterface() {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        // Stuff to purge games
        protected void purgeGames(System.Collections.IEnumerable games) {
            this.disableInterface();
            Core.games.purgeGames(games,purgeComplete);
        }

        private void purgeComplete(object sender, RunWorkerCompletedEventArgs e) {
            if(!e.Cancelled) {
                Core.games.detectGames();
                redetectGames();
            }
            this.enableInterface(null,null);
        }

        // Generic delegate and dispatcher stuff so other threads can retreive information

        private delegate object getPropertyDelegate(object from_me, String get_me);
        private delegate void setPropertyDelegate(object set_me);
        protected object getPropertyDispatcher(object from_me, String get_me) {
            DispatcherOperation dispatcho = this.Dispatcher.BeginInvoke(new getPropertyDelegate(this.getProperty),from_me, get_me);
            dispatcho.Wait();
            return dispatcho.Result;
        }
        private object getProperty(object from_me, String get_me) {
            System.Reflection.PropertyInfo info = from_me.GetType().GetProperty(get_me);
            return info.GetValue(from_me,null);
        }


        public new bool ShowDialog() {
            if(((AWindow)this.Owner)!=null)
                ((AWindow)this.Owner).blur();
            bool return_me = (bool)base.ShowDialog();
            if(((AWindow)this.Owner)!=null)
                ((AWindow)this.Owner).unBlur();
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

        private bool updater_program = false, suppress_message = false;
        public void checkUpdates(bool updater_program, bool suppress_message) {
            this.updater_program = updater_program;
            this.suppress_message = suppress_message;
            BackgroundWorker update = new BackgroundWorker();
            Core.updater = new Update.UpdatesHandler();
            old_progress = ProgressHandler.progress_message;
            update.DoWork += new DoWorkEventHandler(update_DoWork);
            update.RunWorkerCompleted += new RunWorkerCompletedEventHandler(update_RunWorkerCompleted);
            update.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(enableInterface);
            update.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(resetStatus);
            disableInterface();
            update.RunWorkerAsync();
        }

        protected virtual void update_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(Core.updater.shutdown_required) {
                this.Close();
                return;
            }

            if(Core.updater.redetect_required) {
                Core.games.loadXml();
                Core.redetect_games = true;
                Core.redetect_archives = true;
            }
        }

        void update_DoWork(Object sender, DoWorkEventArgs e)
        {
            Core.updater.checkUpdates(updater_program,suppress_message);
        }


        protected void beginRestore(List<ArchiveHandler> archives) {
            this.Visibility = System.Windows.Visibility.Hidden;

            if(archives.Count>1&&this.askTranslatedQuestion("RestoreMultipleArchives")) {
                Restore.RestoreProgramHandler.use_defaults = true;
            }

            foreach(ArchiveHandler archive in archives) {
                if(Restore.RestoreProgramHandler.overall_stop) {
                    break;
                }
                Restore.RestoreWindow restore = new Restore.RestoreWindow(archive,this);
                if(restore.ShowDialog())
                    Core.redetect_games = true;
            }
            Restore.RestoreProgramHandler.use_defaults = false;
            Restore.RestoreProgramHandler.overall_stop = false;
            // Restore.RestoreProgramHandler.default_user = null;
            if(Restore.RestoreProgramHandler.unsuccesfull_restores.Count>0) {
                StringBuilder fail_list = new StringBuilder();
                foreach(string failed in Restore.RestoreProgramHandler.unsuccesfull_restores) {
                    fail_list.AppendLine(failed);
                }
                this.showError(Strings.get("RestoreSomeFailed"),fail_list.ToString());
            }
            this.Visibility = System.Windows.Visibility.Visible;

        }

        protected void redetectArchives() {
            disableInterface();
            old_progress = ProgressHandler.progress_message;
            BackgroundWorker redetect = new BackgroundWorker();
            redetect.DoWork += new DoWorkEventHandler(redetectArchives);
            redetect.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(enableInterface);
            redetect.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(resetStatus);
            redetect.RunWorkerAsync();
        }

        private void redetectArchives(object sender, DoWorkEventArgs e)
        {
            if(Core.redetect_games) {
                redetectGames(sender,e);
            }
            Core.archives = new ArchivesHandler();
            Core.archives.detectBackups();
            Core.redetect_archives = false;
        }

        protected void redetectGames() {
            disableInterface();
            BackgroundWorker redetect = new BackgroundWorker();
            redetect.DoWork += new DoWorkEventHandler(redetectGames);
            redetect.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(enableInterface);
            redetect.RunWorkerAsync();
        }

        private void redetectGames(object sender, DoWorkEventArgs e)
        {
            Core.games = new GamesHandler();
            Core.games.detectGames();
            Core.redetect_games = false;
        }

        private BackupProgramHandler backup;
        private string old_progress;
        protected void cancelBackup() {
            if(backup!=null&&backup.IsBusy)
                backup.CancelAsync();
        }
        protected void beginBackup(RunWorkerCompletedEventHandler when_done) {
            backup = new BackupProgramHandler();
            startBackup(when_done);
        }
        protected void beginBackup(List<GameHandler> backup_list, RunWorkerCompletedEventHandler when_done) {
            backup = new BackupProgramHandler(backup_list);
            startBackup(when_done);
        }
        protected void beginBackup(GameHandler game, List<DetectedFile> files, string archive_name, RunWorkerCompletedEventHandler when_done) {
            backup = new BackupProgramHandler(game,files,archive_name);
            startBackup(when_done);
        }
        private void startBackup(RunWorkerCompletedEventHandler when_done) {
            old_progress = ProgressHandler.progress_message;
            backup.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(resetStatus);
            backup.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(enableInterface);
            disableInterface();
            backup.RunWorkerAsync();
        }

        void resetStatus(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressHandler.progress = 0;
            ProgressHandler.progress_message = old_progress ;
        }

        private void ApplyEffect(AWindow win) 
        { 
            System.Windows.Media.Effects.BlurEffect objBlur = 
           new System.Windows.Media.Effects.BlurEffect(); 
            objBlur.Radius = 4; 
            win.Effect = objBlur; 
        } 
        /// <summary> 
        /// Remove Blur Effects 
        /// </summary> 
        /// <param name=”win”></param> 
        private void ClearEffect(AWindow win) 
        { 
            win.Effect = null; 
        }

        #region TranslatedMessageBoxes
        public bool askTranslatedQuestion(String string_name) {
            return askQuestion(Strings.get(string_name + "Title"),
                Strings.get(string_name + "Message"));
        }
        public bool showTranslatedWarning(String string_name) {
            return showWarning(Strings.get(string_name + "Title"),
                Strings.get(string_name + "Message"));
        }
        public bool showTranslatedError(String string_name) {
            return showError(Strings.get(string_name + "Title"),
                Strings.get(string_name + "Message"));
        }
        #endregion

        #region MessageBox showing things
        protected bool askQuestion(string title, string message) {
            MessageBox box = new MessageBox(title,message, RequestType.Question, this);
            return (bool)box.ShowDialog();
        }
        public bool showError(string title, string message) {
            return showError(title,message,null);
        }
        protected bool showError(string title, string message, Exception e) {
            return displayMessage(title,message, MessageTypes.Error, e);
        }
        protected bool showWarning(string title, string message) {
            return displayMessage(title,message, MessageTypes.Warning,null);
        }
        protected bool showInfo(string title, string message) {
            return displayMessage(title,message, MessageTypes.Info,null);
        }
        protected bool displayMessage(string title, string message, MessageTypes type, Exception e) {
            MessageBox box = new MessageBox(title,message, e,type, this);
            return (bool)box.ShowDialog();
        }
        #endregion

        #region Progress stuff
        public virtual void updateProgress(ProgressUpdatedEventArgs e) {
        }
        protected void applyProgress(ProgressBar progress, ProgressUpdatedEventArgs e) {
            progress.IsEnabled = e.state!= ProgressState.None;
            progress.IsIndeterminate = e.state== ProgressState.Indeterminate;
            switch(e.state) {
                case ProgressState.Normal:
                    progress.Foreground = default_progress_color;
                    break;
                case ProgressState.Error:
                    progress.Foreground = Brushes.Red;
                    break;
                case ProgressState.Wait:
                    progress.Foreground = Brushes.Yellow;
                    break;
            }

            progress.Visibility = System.Windows.Visibility.Visible;
            if(e.max==0)
                progress.Value = 0;
            else {
                progress.Maximum = e.max;
                progress.Value = e.value;
            }
        }
        #endregion

        #region Path choosing stuff
        public bool overrideSteamPath() {
            string old_path = Core.settings.steam_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = false;
            folderBrowser.Description = Strings.get("SelectSteamPath");
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if(folderBrowser.ShowDialog(this.GetIWin32Window())== System.Windows.Forms.DialogResult.OK) {
                    Core.settings.steam_path = folderBrowser.SelectedPath;
                    if(Core.settings.steam_path==folderBrowser.SelectedPath||Core.settings.steam_path!=old_path)
                        return true;
                    else 
                        showTranslatedWarning("SelectSteamPathRejected");
                } else {
                    try_again = false;
                }
            } while(try_again);
            return false;
        }

        public bool changeBackupPath() {
            string old_path = Core.settings.backup_path;
            string new_path = null;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = Strings.get("SelectBackupPath");
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if(folderBrowser.ShowDialog(this.GetIWin32Window())== System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if(PermissionsHelper.isReadable(new_path)) {
                        if(PermissionsHelper.isWritable(new_path)) {
                            Core.settings.backup_path = new_path;
                            return new_path!=old_path;
                        } else {
                            showError(Strings.get("ReadWriteErrorTitle"),Strings.get("SelectBackupPathWriteError") + ":" + Environment.NewLine + new_path);
                            try_again = true;
                        }
                    } else {
                        showError(Strings.get("ReadWriteErrorTitle"),Strings.get("SelectBackupPathReadError") + ":" + Environment.NewLine + new_path);
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while(try_again);
            return false;
        }
        public bool changeSyncPath() {
            string old_path = Core.settings.sync_path;
            string new_path = null;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = Strings.get("SelectSyncPath");
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if(folderBrowser.ShowDialog(this.GetIWin32Window())== System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if(PermissionsHelper.isReadable(new_path)) {
                        if(PermissionsHelper.isWritable(new_path)) {
                            Core.settings.sync_path = new_path;
                            if(new_path!=old_path)
                                Core.rebuild_sync = true;
                            return new_path!=old_path;
                        } else {
                            showError(Strings.get("ReadWriteErrorTitle"),Strings.get("SelectSyncPathWriteError") + ":" + Environment.NewLine + new_path);
                            try_again = true;
                        }
                    } else {
                        showError(Strings.get("ReadWriteErrorTitle"),Strings.get("SelectSyncPathReadError") + ":" + Environment.NewLine + new_path);
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while(try_again);
            return false;
        }

        protected bool addAltPath() {
            return WPFHelpers.addAltPath(this);
        }

        protected string promptForPath(string description) {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = description;
            bool try_again = false;
            do {
                if(folderBrowser.ShowDialog(this.GetIWin32Window())== System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    try_again = true;
                    return new_path;
                } else {
                    try_again = false;
                }
            } while(try_again);
            return null;
        }
        #endregion

        #region BackgroundWorker setup stuff
        private void subProgressChanged(ProgressUpdatedEventArgs e) {
            setTaskBarState(e.state);
        }
        private void progressChanged(ProgressUpdatedEventArgs e) {
            setTaskBarState(e.state);
            if(e.max==0) {
                TaskbarItemInfo.ProgressValue = 0;
            } else {
                TaskbarItemInfo.ProgressValue = (double)e.value/(double)e.max;
            }

        }

        private void setTaskBarState(ProgressState state) {
            switch(state) {
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
            if(e.Error!=null) {
                MessageHandler.SendException(e.Error);
            }
        }
        
        public void sendMessage(MessageEventArgs e) {
            bool response = false;
            switch(e.type) {
                case MessageTypes.Error:
                    response = showError(e.title,e.message,e.exception);
                    break;
                case MessageTypes.Info:
                    response = showInfo(e.title,e.message);
                    break;
                case MessageTypes.Warning:
                    response = showWarning(e.title,e.message);
                    break;
            }
            e.response = ResponseType.OK;
        }


        protected void openHyperlink(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        
        public void requestInformation(RequestEventArgs e) {
            switch(e.info_type) {
                case RequestType.Question:
                    if(askQuestion(e.title,e.message)) {
                        e.result.selected_option = "Yes";
                        e.result.selected_index = 1;
                        e.response = ResponseType.OK;
                    } else {
                        e.response = ResponseType.Cancel;
                    }
                    return;
                case RequestType.Choice:
                    ChoiceWindow choice = new ChoiceWindow(e.title,e.message,e.options,e.default_option, this);
                    if((bool)choice.ShowDialog()) {
                        choice.Close();
                        e.result.selected_index = choice.selected_index;
                        e.result.selected_option = choice.selected_item;
                        e.response = ResponseType.OK;
                    } else {
                        e.response = ResponseType.Cancel;
                    }
                    return;
                case RequestType.BackupFolder:
                    if(changeBackupPath()) {
                        e.result.cancelled = false;
                        e.response = ResponseType.OK;
                    } else {
                        e.result.cancelled = true;
                        e.response = ResponseType.Cancel;
                    }
                    return;
                case RequestType.SyncFolder:
                    if(changeSyncPath()) {
                        e.result.cancelled = false;
                        e.response = ResponseType.OK;
                    } else {
                        e.result.cancelled = true;
                        e.response = ResponseType.Cancel;
                    }
                    return;
                default:
                    throw new NotImplementedException("The specified request type " + e.info_type.ToString() + " is not supported in this GUI toolkit.");
            }
        }
        #endregion

        #region stuff for interacting with windows.forms controls
        // Ruthlessly stolen from http://stackoverflow.com/questions/315164/how-to-use-a-folderbrowserdialog-from-a-wpf-application
        public System.Windows.Forms.IWin32Window GetIWin32Window()
        {
            var source = System.Windows.PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }
        #endregion
        protected void keepTextNumbersEvent(object sender, TextChangedEventArgs e)
        {
            TextBox txt_box = (TextBox)sender;
            int cursor = txt_box.SelectionStart;
            string new_text = Core.makeNumbersOnly(txt_box.Text);
            cursor += new_text.Length - txt_box.Text.Length;
            txt_box.Text = Core.makeNumbersOnly(txt_box.Text);
            txt_box.SelectionStart = cursor;
        }
    }

}
