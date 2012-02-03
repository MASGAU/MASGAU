using System;
using System.Windows;
using System.Windows.Controls;
using MASGAU.Location;
using System.ComponentModel;
using MASGAU.Communication.Progress;
using MASGAU.Location.Holders;
namespace MASGAU.Analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AnalyzerWindow : AProgramWindow
    {
        private AnalyzerProgramHandler analyzer;

        public AnalyzerWindow(): base(new AnalyzerProgramHandler())
        {
            analyzer = (AnalyzerProgramHandler)program_handler;
            InitializeComponent();
        }

        protected override void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.Title += " - Loading Settings...";
            base.WindowLoaded(sender, e);
        }

        protected override void setup(object sender, RunWorkerCompletedEventArgs e)
        {
            base.setup(sender, e);

            ProgressHandler.progress_state = ProgressState.None;

            emailTxt.DataContext = Core.settings;

            if(Core.locations.ready) {

                if(getFolder(EnvironmentVariable.ProgramFilesX86,null)==null) {
                    programFilesx86Btn.IsEnabled = false;
                }
                if(!Core.locations.steam_detected) {
                    steamappsBtn.IsEnabled = false;
                }
                if (Core.locations.platform_version== PlatformVersion.XP) {
                    publicUserBtn.IsEnabled = false;
                    virtualStoreBtn.IsEnabled = false;
                    savedGamesBtn.IsEnabled = false;
                }
            } else {
                this.Close();
                return;
            }
            enableInterface();
        }

        private void buttonCheck(object sender,  TextChangedEventArgs e) {
			if(gamePathTxt.Text=="") {
                installFolderBtn.IsEnabled = false;
                windowsScanBtn.IsEnabled = false;
			} else {
			    if(gameSaveTxt.Text==""||gameNameTxt.Text=="") {
                    windowsScanBtn.IsEnabled = false;
			    } else {
                    windowsScanBtn.IsEnabled = true;
                }
                installFolderBtn.IsEnabled = true;
            }

            if(psPrefixTxt.Text==""||psSuffixTxt.Text==""||psGameSavePathTxt.Text==""||gameNameTxt.Text=="") {
                playstationScanButton.IsEnabled=false;
            } else {
                playstationScanButton.IsEnabled=true;
            }

        }

        #region Generic folder-getting functions
        private string getPath(string path, string description, Environment.SpecialFolder root) {
            System.Windows.Forms.FolderBrowserDialog pathBrowser = new System.Windows.Forms.FolderBrowserDialog();
            pathBrowser.RootFolder = root;
            pathBrowser.SelectedPath = path;


            if(description==null)
                pathBrowser.Description = "Select Install Folder";
            else
                pathBrowser.Description = description;

            if (pathBrowser.ShowDialog(this.GetIWin32Window()) != System.Windows.Forms.DialogResult.Cancel) {
                return pathBrowser.SelectedPath;
            } else {
                return null;
            }
        }

        private void getGamePath(string path, Environment.SpecialFolder look_here) {
            string path_result = getPath(path,"Select Game Install Location",look_here);
            if (path_result!=null) {
                gamePathTxt.Text = path_result;
            }
        }

        private void getSavePath(string path, Environment.SpecialFolder look_here) {
            string path_result = getPath(path,"Select Save Location",look_here);
            if (path_result!=null) {
                gameSaveTxt.Text = path_result;
            }
        }

        private string getFolder(EnvironmentVariable ev, string path){
            LocationPathHolder parse_me = new LocationPathHolder();
            parse_me.path = path;
            parse_me.rel_root = ev;
            foreach(string user in Core.locations.getUsers(ev)) {
                return Core.locations.getAbsoluteRoot(parse_me,user);
            }
            return Core.locations.getAbsoluteRoot(parse_me,null);
        }

        #endregion

        #region Playstation Event Handlers
        private void playstationScanButton_Click(object sender, RoutedEventArgs e)
        {
			if(psPrefixTxt.Text==""||psSuffixTxt.Text=="") {
				showError("I'm not psychic","You need to specify but the prefix and suffix of the game's code.");
				return;
			}
			if(psGameSavePathTxt.Text=="") {
				showError("I'm not omniscient","You need to specify the folder that contains the game's saves.");
				return;
			}
            SearchingWindow searcher = new SearchingWindow(null, psGameSavePathTxt.Text, gameNameTxt.Text, true, this);
			if((bool)searcher.ShowDialog()) {
				ReportWindow report = new ReportWindow(searcher.output + Environment.NewLine + "Playstation Code: " + psPrefixTxt.Text + "-" + psSuffixTxt.Text,psGameSavePathTxt.Text,this);
				report.ShowDialog();
			}

        }
        private void psMyComputerBtn_Click(object sender, RoutedEventArgs e)
        {
            string path_result = getPath("","Select Playstation Save Location",Environment.SpecialFolder.MyComputer);
            if (path_result!=null) {
                psGameSavePathTxt.Text = path_result;
            }

        }
        #endregion

        #region Other Event Handlers
        private void windowsScanBtn_Click(object sender, RoutedEventArgs e)
        {
			if(gamePathTxt.Text=="") {
				showError("I'm not psychic","You need to specify the install folder for the game.");
				return;
			}
			if(gameSaveTxt.Text=="") {
				showError("I'm not clairvoyant","You need to specify the folder that contains the game's saves.");
				return;
			}
			SearchingWindow searcher = new SearchingWindow(gamePathTxt.Text,gameSaveTxt.Text,gameNameTxt.Text,false,this);
            searcher.ShowInTaskbar = true;
            this.Visibility = System.Windows.Visibility.Hidden;
			if((bool)searcher.ShowDialog()) {
				ReportWindow report = new ReportWindow(searcher.output.ToString(),gameNameTxt.Text,this);
				report.ShowDialog();
			}
            this.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion

        #region Game Install Path Event Handlers
        private void myComputerBtn_Click(object sender, RoutedEventArgs e)
        {
            getGamePath("",Environment.SpecialFolder.MyComputer);
        }

        private void programFilesBtn_Click(object sender, RoutedEventArgs e)
        {
            //if(Environment.GetEnvironmentVariable("ProgramW6432")!=null) {
                getGamePath(getFolder(EnvironmentVariable.ProgramFiles,null),Environment.SpecialFolder.MyComputer);
            //} else {
                //getGamePath(WindowsLocationHandler.program_files,Environment.SpecialFolder.ProgramFiles);
            //}

        }

        private void programFilesx86Btn_Click(object sender, RoutedEventArgs e)
        {
            getGamePath(getFolder(EnvironmentVariable.ProgramFilesX86,null),Environment.SpecialFolder.ProgramFilesX86);

        }

        private void steamappsBtn_Click(object sender, RoutedEventArgs e)
        {
            getGamePath(Core.settings.steam_path + "\\steamapps",Environment.SpecialFolder.MyComputer);

        }

        #endregion
        
        #region Game Save Path Event Handlers
        private void installFolderBtn_Click(object sender, RoutedEventArgs e)
        {
			if(gamePathTxt.Text=="") {
				showError("Come on, man","This button doesn't do anything if you don't specify an install path.");
			} else {
                getSavePath(gamePathTxt.Text,Environment.SpecialFolder.MyComputer);
            }

        }

        private void myDocumentsBtn_Click(object sender, RoutedEventArgs e)
        {
            getSavePath(getFolder(EnvironmentVariable.UserDocuments,null),Environment.SpecialFolder.MyDocuments);

        }

        private void savedGamesBtn_Click(object sender, RoutedEventArgs e)
        {
            getSavePath(getFolder(EnvironmentVariable.SavedGames,null),Environment.SpecialFolder.UserProfile);

        }

        private void virtualStoreBtn_Click(object sender, RoutedEventArgs e)
        {
            getSavePath(System.IO.Path.Combine(getFolder(EnvironmentVariable.LocalAppData,null),"VirtualStore"),Environment.SpecialFolder.LocalApplicationData);

        }

        private void localAppDataBtn_Click(object sender, RoutedEventArgs e)
        {
            getSavePath(getFolder(EnvironmentVariable.LocalAppData,null),Environment.SpecialFolder.LocalApplicationData);

        }

        private void roamingAppDataBtn_Click(object sender, RoutedEventArgs e)
        {
            getSavePath(getFolder(EnvironmentVariable.AppData,null),Environment.SpecialFolder.ApplicationData);

        }

        private void publicUserBtn_Click(object sender, RoutedEventArgs e)
        {
            getSavePath(getFolder(EnvironmentVariable.Public,null),Environment.SpecialFolder.MyComputer);

        }

        private void programDataBtn_Click(object sender, RoutedEventArgs e)
        {
            getSavePath(getFolder(EnvironmentVariable.AllUsersProfile,null),Environment.SpecialFolder.MyComputer);
        }


        #endregion

        public override void updateProgress(MASGAU.Communication.Progress.ProgressUpdatedEventArgs e)
        {
            // Do nothing!
        }

        private void emailTxt_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void emailTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            Core.settings.email = emailTxt.Text;
        }
    }
}
