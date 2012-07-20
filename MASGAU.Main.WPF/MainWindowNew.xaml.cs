using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Communication;
using Communication.WPF;
using Microsoft.Windows.Controls.Ribbon;
using Translator;
using Translator.WPF;
namespace MASGAU.Main {
    /// <summary>
    /// Interaction logic for MainWindowNew.xaml
    /// </summary>
    public partial class MainWindowNew : Window, IWindow {
        MainProgramHandler masgau;
        public MainWindowNew() {
            InitializeComponent();
            var uriSource = new Uri(System.IO.Path.Combine(Core.app_path, "masgau.ico"), UriKind.Relative);

            this.Icon = new BitmapImage(uriSource);

            TranslationHelpers.translateWindow(this);

            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(this.Dispatcher));
            _context = SynchronizationContext.Current;

            CommunicationHandler.addReceiver(this);

            WPFCommunicationHelpers.default_progress_color = progress.Foreground;

            // Insert code required on object creation below this point.

            appMenu.SmallImageSource = this.Icon;

            this.Loaded += new System.Windows.RoutedEventHandler(WindowLoaded);
            this.Closing += new CancelEventHandler(Window_Closing);

            masgau = new MainProgramHandler();
            setupJumpList();
        }

        #region redetect games
        protected void redetectGames() {
            BackgroundWorker redetect = new BackgroundWorker();
            redetect.DoWork += new DoWorkEventHandler(redetectGames);
            redetect.RunWorkerCompleted += new RunWorkerCompletedEventHandler(setup);
            disableInterface();
            redetect.RunWorkerAsync();
        }

        private void redetectGames(object sender, DoWorkEventArgs e) {
            Games.Clear();
            Games.detectGames();
            Core.redetect_games = false;
        }
        #endregion

        #region Window event handlers
        void Window_Closing(object sender, CancelEventArgs e) {
            _available = false;
        }
        #endregion

        #region Program handler setup
        protected virtual void WindowLoaded(object sender, System.Windows.RoutedEventArgs e) {
            setUpProgramHandler();
        }
        protected virtual void setUpProgramHandler() {
            this.Title = masgau.program_title;
            disableInterface();
            gamesLst.DataContext = Games.DetectedGames;
            gamesLst.ItemsSource = Games.DetectedGames;

            masgau.RunWorkerCompleted += new RunWorkerCompletedEventHandler(setup);
            masgau.RunWorkerAsync();


        }
        protected virtual void setup(object sender, RunWorkerCompletedEventArgs e) {
            this.enableInterface();
            if (e.Error != null) {
                this.Close();
            }

            OpenBackupFolder.DataContext = Core.settings;

            if (!Core.initialized) {
                Communication.Translator.TranslatingMessageHandler.SendException(new TranslateableException("CriticalSettingsFailure"));
                this.Close();
            }
            this.Title = masgau.program_title;
            setupAnalyzer();
            this.checkUpdates();
        }
        #endregion

        #region Window enable/disables

        public void hideInterface() {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        public void showInterface() {
            this.Visibility = System.Windows.Visibility.Visible;

        }
        public void closeInterface() {
            this.Close();
        }

        #endregion

        #region About stuff
        private void AboutButton_Click(object sender, RoutedEventArgs e) {

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
        }

        #endregion


        private void RibbonButton_Click(object sender, RoutedEventArgs e) {
            redetectGames();
        }


        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Window_Closing_1(object sender, CancelEventArgs e) {
            cancelWorkers();
        }



        private void endOfOperations() {
            ProgressHandler.restoreMessage();
            ProgressHandler.value = 0;
            enableInterface();
        }



        protected System.Windows.Forms.IWin32Window GetIWin32Window() {
            return WPFHelpers.GetIWin32Window(this);
        }

        private void OpenBackupFolder_Click(object sender, RoutedEventArgs e) {
            Core.openBackupPath();
        }

        private void ChangeBackupFolder_Click(object sender, RoutedEventArgs e) {
            this.changeBackupPath();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e) {
            if (this.WindowState == System.Windows.WindowState.Minimized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Minimized;

        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e) {
            if(this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }



    }
}
