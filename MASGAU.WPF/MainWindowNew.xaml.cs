using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MVC.Communication;
using MVC.WPF;
using SMJ.WPF.Effects;
using Translator;
using Translator.WPF;
namespace MASGAU.Main {
    /// <summary>
    /// Interaction logic for MainWindowNew.xaml
    /// </summary>
    public partial class MainWindowNew : NewWindow, IMainWindow {
        public MainProgramHandler masgau { get; protected set; }
        NotifierIcon notifier;

        public MainWindowNew() {
            InitializeComponent();
            gamesLst.TemplateItem = new GameListViewItem();
            ArchiveList.TemplateItem = new ArchiveListViewItem();


            notifier = new NotifierIcon(this);
            this.AllowDrop = true;
            this.Drop += new System.Windows.DragEventHandler(MainWindowNew_Drop);

            VersionLabel.Content = Strings.GetLabelString("MASGAUAboutVersion", Core.ProgramVersion.ToString());

            if (Core.Ready)
                this.DataContext = Core.settings;

            TranslationHelpers.translateWindow(this);



            //CommunicationHandler.addReceiver(this);

            WPFCommunicationHelpers.default_progress_color = progress.Foreground;

            // Insert code required on object creation below this point.

            appMenu.SmallImageSource = this.Icon;

            this.Loaded += new System.Windows.RoutedEventHandler(WindowLoaded);
            this.Closing += new CancelEventHandler(Window_Closing);

            AllUsersModeButton.ToolTip = Strings.GetToolTipString("AllUserModeButton");
            SingleUserModeButton.ToolTip = Strings.GetToolTipString("SingleUserModeButton");



            masgau = new MainProgramHandler(new Location.LocationsHandler(), this);
            setupJumpList();
        }

        #region Window event handlers
        void Window_Closing(object sender, CancelEventArgs e) {
            _available = false;
        }
        #endregion

        #region Program handler setup
        public Config.WindowState StartupState {
            set {
                switch (value) {
                    case global::Config.WindowState.Maximized:
                        this.WindowState = System.Windows.WindowState.Maximized;
                        break;
                    case global::Config.WindowState.Iconified:
                        this.ShowInTaskbar = false;
                        this.Visibility = System.Windows.Visibility.Hidden;
                        break;
                }

            }
        }

        protected virtual void WindowLoaded(object sender, System.Windows.RoutedEventArgs e) {
            masgau.setupMainProgram();
        }

        public void unHookData() {
            ArchiveList.DataContext = null;
        }

        public void hookData() {
            gamesLst.DataContext = Games.DetectedGames;
            gamesLst.Model = Games.DetectedGames;
            AllUsersModeButton.DataContext = masgau;


            bindSettingsControls();


            OpenBackupFolder.DataContext = Core.settings;
            OpenBackupFolderTwo.DataContext = Core.settings;
            monitorStatusLabel.DataContext = Core.monitor;

            setupSteamButton();
            populateAltPaths();
            setupMonitorIcon();

            addGameSetup();
            this.checkUpdates();
        }



        #endregion

        #region About stuff

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
            notifier.Visible = false;
            notifier.Dispose();
        }



        private void endOfOperations() {
            ProgressHandler.restoreMessage();
            ProgressHandler.value = 0;
            enableInterface();
        }


        private void OpenBackupFolder_Click(object sender, RoutedEventArgs e) {
            Core.openBackupPath();
        }

        private void ChangeBackupFolder_Click(object sender, RoutedEventArgs e) {
            if (this.changeBackupPath())
                askRefreshGames("RefreshForChangedBackupPath");
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }


        public void updateWindowState() {
            switch (this.WindowState) {
                case System.Windows.WindowState.Normal:
                    Core.settings.WindowState = global::Config.WindowState.Normal;
                    break;
                case System.Windows.WindowState.Maximized:
                    Core.settings.WindowState = global::Config.WindowState.Maximized;
                    break;
                case System.Windows.WindowState.Minimized:
                    Core.settings.WindowState = global::Config.WindowState.Minimized;
                    break;
            }
            if (!this.ShowInTaskbar)
                Core.settings.WindowState = global::Config.WindowState.Iconified;
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e) {
            toggleMinimize();
            updateWindowState();
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e) {
            if (this.WindowState == System.Windows.WindowState.Maximized) {
                this.WindowState = System.Windows.WindowState.Normal;
            } else {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            updateWindowState();

        }

        private void closeButton_Click(object sender, RoutedEventArgs e) {
            this.toggleVisibility();
            notifier.sendBalloon(Strings.GetMessageString("RunningInTray"));
        }

        private void SingleUserModeButton_Click(object sender, RoutedEventArgs e) {
            if (!System.IO.File.Exists(Core.ExecutableName)) {
                this.showTranslatedError("FileNotFoundCritical", Core.ExecutableName);
                return;
            }

            toggleVisibility();
            if (SecurityHandler.elevation(Core.ExecutableName, "-allusers", false) == ElevationResult.Success)
                this.Close();
            else
                toggleVisibility();
        }

        private void AllUsersModeButton_Click(object sender, RoutedEventArgs e) {
            if (!System.IO.File.Exists(Core.ExecutableName)) {
                this.showTranslatedError("FileNotFoundCritical", Core.ExecutableName);
                return;
            }


            toggleVisibility();
            if (SecurityHandler.runExe(Core.ExecutableName, "", false, false) == ElevationResult.Success)
                this.Close();
            else
                toggleVisibility();

        }

        private void gameSaveLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        protected void openSubWindow(System.Windows.Controls.Grid grid) {
            ribbon.IsEnabled = false;
            GameGrid.IsEnabled = false;
            ArchiveGrid.IsEnabled = false;
            FadeEffect fade = new FadeInEffect(timing);
            fade.Start(grid);

        }
        protected void closeSubWindow(System.Windows.Controls.Grid grid) {
            FadeEffect fade = new FadeOutEffect(timing);
            fade.Start(grid);
            ribbon.IsEnabled = true;
            GameGrid.IsEnabled = true;
            ArchiveGrid.IsEnabled = true;

        }

        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            openSubWindow(AboutGrid);

        }

        private void AboutCloseButton_Click(object sender, RoutedEventArgs e) {
            closeSubWindow(AboutGrid);

        }

        private void RibbonMenuItem_Click(object sender, RoutedEventArgs e) {

        }

        private void PurgeGameButton_Click(object sender, RoutedEventArgs e) {
            Games.purgeGames(gamesLst.SelectedItems, purgeDone);
        }
        protected virtual void purgeDone(object sender, RunWorkerCompletedEventArgs e) {
            if (((bool)e.Result) == true) {
                this.askRefreshGames("RefreshForPurge");

            }
        }

        private void PurgeGameArchivesButton_Click(object sender, RoutedEventArgs e) {

        }

        private void ReportButton_Click(object sender, RoutedEventArgs e) {
            ReportProblemWindow prob = new ReportProblemWindow(this);
            prob.ShowDialog();
        }



    }
}
