using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MVC.Communication;
using Communication.WPF;
using Microsoft.Windows.Controls.Ribbon;
using Translator;
using Translator.WPF;
using System.Windows.Forms;
namespace MASGAU.Main {
    /// <summary>
    /// Interaction logic for MainWindowNew.xaml
    /// </summary>
    public partial class MainWindowNew : NewWindow, IWindow {
        MainProgramHandler masgau;
        NotifyIcon notifyIcon = new NotifyIcon();

        public MainWindowNew() {
            InitializeComponent();
            notifyIcon.Icon = new System.Drawing.Icon("masgau.ico");
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            MenuItem exit = new MenuItem();
            exit.Text = "Exit MASGAU";
            exit.Click += new EventHandler(exit_Click);
            notifyIcon.ContextMenu.MenuItems.Add(exit);
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            notifyIcon.Visible = true;


            this.DataContext = Core.settings;
            bindSettingsControls();

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

            masgau = new MainProgramHandler(new Location.LocationsHandler());
            setupJumpList();
        }

        #region Window event handlers
        void Window_Closing(object sender, CancelEventArgs e) {
            _available = false;
        }
        #endregion

        #region Program handler setup
        protected virtual void WindowLoaded(object sender, System.Windows.RoutedEventArgs e) {

            switch (Core.settings.WindowState) {
                case global::Config.WindowState.Maximized:
                    this.WindowState = System.Windows.WindowState.Maximized;
                    break;
                case global::Config.WindowState.Iconified:
                    this.ShowInTaskbar = false;
                    this.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }

            setUpProgramHandler();
        }
        protected virtual void setUpProgramHandler() {
            this.Title = masgau.program_title;
            disableInterface();
            gamesLst.DataContext = Games.DetectedGames;
            gamesLst.ItemsSource = Games.DetectedGames;

            
            ArchiveList.DataContext = null;



            masgau.RunWorkerCompleted += new RunWorkerCompletedEventHandler(setup);
            masgau.RunWorkerAsync();
        }

        protected virtual void setup(object sender, RunWorkerCompletedEventArgs e) {
            this.enableInterface();
            if (e.Error != null) {
                this.Close();
            }


            OpenBackupFolder.DataContext = Core.settings;
            OpenBackupFolderTwo.DataContext = Core.settings;
            monitorStatusLabel.DataContext = Core.monitor;

            setupSteamButton();
            populateAltPaths();
            setupMonitorIcon();

            if (!Core.initialized) {
                Communication.Translator.TranslatingMessageHandler.SendException(new TranslateableException("CriticalSettingsFailure"));
                this.Close();
            }
            this.Title = masgau.program_title;
            addGameSetup();
            this.checkUpdates();
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
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
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
            this.changeBackupPath();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        protected void updateWindowState() {
            switch(this.WindowState) {
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
            if(!this.ShowInTaskbar)
                Core.settings.WindowState = global::Config.WindowState.Iconified;
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e) {
            toggleMinimize();
            updateWindowState();
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e) {
            if(this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
            updateWindowState();

        }

        private void closeButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
