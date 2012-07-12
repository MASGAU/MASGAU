using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MASGAU.Location;
using Translator;
using Translator.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        private Analyzer.AAnalyzer analyzer;

        private void setupAnalyzer() {
            disclaimerLabel.Background = ribbon.Background;
            Core.email.checkAvailability(analyzeEmailCheckDone);
            if (!Core.locations.ps.ready) {
                AnalyzerPlatformGroup.Visibility = System.Windows.Visibility.Collapsed;
                WindowsRadioButton.IsChecked = true;
                AnalyzerPlayStationGroup.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                AnalyzerPlatformGroup.Visibility = System.Windows.Visibility.Visible;
            }


            if (Core.locations.getFolder(EnvironmentVariable.ProgramFilesX86, null) == null) {
                ProgramFilesX86Browse.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (Core.locations.steam.steam_apps_path == null) {
                SteamappsBrowse.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (Core.locations.platform_version == "WindowsXP") {
                PublicUserBrowse.Visibility = System.Windows.Visibility.Collapsed;
                VirtualStoreBrowse.Visibility = System.Windows.Visibility.Collapsed;
                SavedGamesBrowse.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void analyzeEmailCheckDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if ((Email.EmailResponse)e.Result== Email.EmailResponse.ServerReachable) {
                TranslationHelpers.translate(ReportEmailButton, "EmailReport");
                ReportEmailButton.IsEnabled = true;
            } else {
                TranslationHelpers.translate(ReportEmailButton, "CantSendReport");
                ReportEmailButton.IsEnabled = false;
            }
        }


        #region Generic folder-getting functions
        private string getPath(string path, string description, Environment.SpecialFolder root) {

            System.Windows.Forms.FolderBrowserDialog pathBrowser = new System.Windows.Forms.FolderBrowserDialog();
            pathBrowser.RootFolder = root;
            pathBrowser.SelectedPath = path;


            if (description == null)
                pathBrowser.Description = Strings.GetLabelString("GameInstallPrompt");
            else
                pathBrowser.Description = description;

            if (pathBrowser.ShowDialog(WPFHelpers.GetIWin32Window(this)) != System.Windows.Forms.DialogResult.Cancel) {
                return pathBrowser.SelectedPath;
            } else {
                return null;
            }
        }

        private void getGamePath(string path, Environment.SpecialFolder look_here) {
            string path_result = getPath(path, Strings.GetLabelString("GameInstallPrompt"), look_here);
            if (path_result != null) {
                InstallPath.Text = path_result;
            }
        }

        private void getSavePath(string path, Environment.SpecialFolder look_here) {
            string path_result = getPath(path, Strings.GetLabelString("GameSavesPrompt"), look_here);
            if (path_result != null) {
                SavePath.Text = path_result;
            }
        }

        #endregion


        #region browse button event handlers
        private void MyComputerBrowse_Click(object sender, RoutedEventArgs e) {
            getGamePath("", Environment.SpecialFolder.MyComputer);
        }

        private void ProgramFilesBrowse_Click(object sender, RoutedEventArgs e) {
            getGamePath(Core.locations.getFolder(EnvironmentVariable.ProgramFiles, null), Environment.SpecialFolder.MyComputer);

        }

        private void ProgramFilesX86Browse_Click(object sender, RoutedEventArgs e) {
            getGamePath(Core.locations.getFolder(EnvironmentVariable.ProgramFilesX86, null), Environment.SpecialFolder.ProgramFilesX86);

        }

        private void SteamappsBrowse_Click(object sender, RoutedEventArgs e) {
            getGamePath(Core.settings.steam_path + "\\steamapps", Environment.SpecialFolder.MyComputer);

        }

        private void InstallFolderBrowse_Click(object sender, RoutedEventArgs e) {
            if (InstallPath.Text == "") {
                TranslationHelpers.showTranslatedError(this, "NeedGameInstallFolderFirst");
            } else {
                getSavePath(InstallPath.Text, Environment.SpecialFolder.MyComputer);
            }

        }

        private void MyDocumentsBrowse_Click(object sender, RoutedEventArgs e) {
            getSavePath(Core.locations.getFolder(EnvironmentVariable.UserDocuments, null), Environment.SpecialFolder.MyDocuments);

        }

        private void SavedGamesBrowse_Click(object sender, RoutedEventArgs e) {
            getSavePath(Core.locations.getFolder(EnvironmentVariable.SavedGames, null), Environment.SpecialFolder.UserProfile);

        }

        private void VirtualStoreBrowse_Click(object sender, RoutedEventArgs e) {
            getSavePath(System.IO.Path.Combine(Core.locations.getFolder(EnvironmentVariable.LocalAppData, null), "VirtualStore"), Environment.SpecialFolder.LocalApplicationData);

        }

        private void LocalAppDataBrowse_Click(object sender, RoutedEventArgs e) {
            getSavePath(Core.locations.getFolder(EnvironmentVariable.LocalAppData, null), Environment.SpecialFolder.LocalApplicationData);

        }

        private void AppData_Click(object sender, RoutedEventArgs e) {
            getSavePath(Core.locations.getFolder(EnvironmentVariable.AppData, null), Environment.SpecialFolder.ApplicationData);

        }

        private void PublicUserBrowse_Click(object sender, RoutedEventArgs e) {
            getSavePath(Core.locations.getFolder(EnvironmentVariable.Public, null), Environment.SpecialFolder.MyComputer);

        }

        private void AllUsersBrowse_Click(object sender, RoutedEventArgs e) {
            getSavePath(Core.locations.getFolder(EnvironmentVariable.AllUsersProfile, null), Environment.SpecialFolder.MyComputer);

        }
        #endregion

        #region button check
        private void buttonCheck(object sender, TextChangedEventArgs e) {
            buttonCheck();
        }

        private void buttonCheck() {
            if (WindowsRadioButton.IsChecked == true) {
                if (InstallPath.Text == "") {
                    InstallFolderBrowse.Visibility = System.Windows.Visibility.Collapsed;
                    AnalyzeButton.IsEnabled = false;
                } else {
                    if (SavePath.Text == "" || AnalyzerGameTitle.Text == "") {
                        AnalyzeButton.IsEnabled = false;
                    } else {
                        AnalyzeButton.IsEnabled = true;
                    }
                    InstallFolderBrowse.Visibility = System.Windows.Visibility.Visible;
                }
            } else {
                AnalyzeButton.IsEnabled = true;
            }
        }


        #endregion

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e) {
            AnalyzerReportGroup.Visibility = System.Windows.Visibility.Collapsed;

            AnalyzeButton.Visibility = System.Windows.Visibility.Collapsed;

            if (WindowsRadioButton.IsChecked == true)
                analyzer = new Analyzer.PCAnalyzer(AnalyzerGameTitle.Text, InstallPath.Text, SavePath.Text, analyzeDone);
            else
                analyzer = new Analyzer.PSAnalyzer(analyzeDone);

            AnalyzerReportTextBox.DataContext = analyzer;

            disableInterface(analyzer.worker);

            analyzer.analyze();

        }

        void analyzeDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            AnalyzeButton.Visibility = System.Windows.Visibility.Visible;
            if (e.Cancelled) {
                AnalyzeButton.IsEnabled = false;
                AnalyzerReportGroup.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                AnalyzerReportGroup.Visibility = System.Windows.Visibility.Visible;

            }


            enableInterface();
        }

        private void AnalyzeCancelButton_Click(object sender, RoutedEventArgs e) {
            analyzer.Cancel();
        }

        private string last_analyzer_save_path = null;
        private void ReportSaveButton_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.DefaultExt = "txt";
            save.Filter = Strings.GetLabelString("TxtFileDescriptionPlural") + "|*.txt|" + Strings.GetLabelString("AllFileDescriptionPlural") + "|*";
            save.Title = Strings.GetLabelString("SaveReportQuestion");
            if (last_analyzer_save_path == null)
                save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                save.InitialDirectory = last_analyzer_save_path;

            save.FileName = AnalyzerTitle + ".txt";
            if (save.ShowDialog(this.GetIWin32Window()) != System.Windows.Forms.DialogResult.Cancel) {
                last_analyzer_save_path = Path.GetDirectoryName(save.FileName);
                try {
                    StreamWriter writer = File.CreateText(save.FileName);
                    writer.Write(analyzer.report);
                    writer.Close();
                } catch {
                    TranslationHelpers.showTranslatedError(this, "WriteError", save.FileName);
                }
            }

        }
        private string AnalyzerTitle {
            get {
                if (WindowsRadioButton.IsChecked == true) {
                    return AnalyzerGameTitle.Text;
                } else {
                    return "PlayStation Saves";
                }
            }
        }
        private void ReportEmailButton_Click(object sender, RoutedEventArgs e) {
            StringBuilder body = new StringBuilder();
            body.AppendLine(analyzer.report);

            ReportEmailButton.IsEnabled = false;
            TranslationHelpers.translate(ReportEmailButton,"SendingReport");
            Core.email.sendEmail("MASGAU - " + AnalyzerTitle, body.ToString(), sendEmailDone);
        }

        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                TranslationHelpers.translate(ReportEmailButton,"CantSendReport");
                displayError("Error time", e.Error.Message);
            } else {
                TranslationHelpers.translate(ReportEmailButton,"ReportSent");
            }
            ReportEmailButton.IsEnabled = false;
        }



        private void AnalyzerPlatformChange(object sender, RoutedEventArgs e) {

            if (!this.IsLoaded)
                return;

            if (WindowsRadioButton.IsChecked == true) {
                AnalyzerPlayStationGroup.Visibility = System.Windows.Visibility.Collapsed;
                AnalyzerGameTitleGroup.Visibility = System.Windows.Visibility.Visible;
                WindowsPathGroup.Visibility = System.Windows.Visibility.Visible;
            } else {
                AnalyzerPlayStationGroup.Visibility = System.Windows.Visibility.Visible;
                AnalyzerGameTitleGroup.Visibility = System.Windows.Visibility.Collapsed;
                WindowsPathGroup.Visibility = System.Windows.Visibility.Collapsed;
            }
            buttonCheck();
        }


    }
}
