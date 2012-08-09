using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using MVC.Communication;
using System.IO;
using MVC.Translator;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.WPF;
using MVC;
using Translator.WPF;
using Translator;
using System.Text;
using MVC.WPF;
using GameSaveInfo;
namespace MASGAU.Restore {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RestoreWindow : AProgramWindow {

        RestoreProgramHandler restore;
        Archive archive = null;
        ControlFlipper flipper = new ControlFlipper();


        public RestoreWindow() : this(null) { }

        public RestoreWindow(Archive archive, ACommunicationWindow owner)
            : base(new RestoreProgramHandler(archive, new Location.LocationsHandler()), owner) {
            InitializeComponent();
            Translator.WPF.TranslationHelpers.translateWindow(this);
            default_progress_color = restoreProgress.Foreground;
            this.archive = archive;
            this.Icon = owner.Icon;
        }

        public RestoreWindow(ACommunicationWindow owner)
            : base(new RestoreProgramHandler(null, new Location.LocationsHandler()), owner) {
            InitializeComponent();
            Translator.WPF.TranslationHelpers.translateWindow(this);
            default_progress_color = restoreProgress.Foreground;
            this.Icon = owner.Icon;
        }

        protected override void WindowLoaded(object sender, RoutedEventArgs e) {
            flipper.Add(ProgressBox);
            flipper.Add(selectFilesGroup);
            flipper.Add(LocationGrid);
            flipper.Add(restoreDoneLabel);

            restoreProgress.IsIndeterminate = true;
            restore = (RestoreProgramHandler)program_handler;
            base.WindowLoaded(sender, e);
        }

        protected override void setup(object sender, RunWorkerCompletedEventArgs e) {
            base.setup(sender, e);

            if (e.Error != null)
                return;

            this.archive = restore.archive;
            ProgressHandler.state = ProgressState.None;


            flipper.SwitchControl(LocationGrid);

            TranslationHelpers.translate(this, "RestoreConfirmPath");

            pathCombo.ItemsSource = restore.path_candidates;
            userCombo.ItemsSource = restore.user_candidates;
            pathLabel.DataContext = restore;
            userLabel.DataContext = restore;

            otherUserButton.Visibility = System.Windows.Visibility.Collapsed;
            selectFilesButton.Visibility = System.Windows.Visibility.Visible;
            restoreButton.Visibility = System.Windows.Visibility.Visible;
            choosePathButton.Visibility = System.Windows.Visibility.Visible;

            if (restore.game_data.RestoreComment != null) {
                restoreDoneText.Text = restore.game_data.RestoreComment + Environment.NewLine + Environment.NewLine + Strings.GetLabelString("RestoreCompleteWithComment");
            }

            refreshPaths();

            this.enableInterface();

        }


        public static void beginRestore(ACommunicationWindow parent, List<Archive> archives) {
            ProgressHandler.saveMessage();
            parent.hideInterface();
            if (archives.Count > 1 && !TranslatingRequestHandler.Request(RequestType.Question,"RestoreMultipleArchives").Cancelled ) {
                Restore.RestoreProgramHandler.use_defaults = true;
            }

            foreach (Archive archive in archives) {
                if (Restore.RestoreProgramHandler.overall_stop) {
                    break;
                }
                Restore.RestoreWindow restore = new Restore.RestoreWindow(archive, parent);
                if (restore.ShowDialog() == true) {
                    Core.redetect_games = true;
                }
            }
            Restore.RestoreProgramHandler.use_defaults = false;
            Restore.RestoreProgramHandler.overall_stop = false;
            // Restore.RestoreProgramHandler.default_user = null;
            if (Restore.RestoreProgramHandler.unsuccesfull_restores.Count > 0) {
                StringBuilder fail_list = new StringBuilder();
                foreach (string failed in Restore.RestoreProgramHandler.unsuccesfull_restores) {
                    fail_list.AppendLine(failed);
                }
                TranslatingMessageHandler.SendError("RestoreSomeFailed", fail_list.ToString());
            }
            parent.showInterface();
            ProgressHandler.restoreMessage();
        }

        public override void updateProgress(MVC.Communication.ProgressUpdatedEventArgs e) {
            if (e.message != null)
                ProgressBox.Header = e.message;
            applyProgress(restoreProgress, e);
        }

        private void refreshPaths() {
            if (restore.path_candidates.Count == 0) {
                pathBox.Visibility = System.Windows.Visibility.Collapsed;
                singlePathBox.Visibility = System.Windows.Visibility.Visible;
                restoreButton.IsEnabled = false;
                TranslationHelpers.translate(pathLabel, "RestoreNoCandidatesFound");
                Restore.RestoreProgramHandler.unsuccesfull_restores.Add(archive.ArchivePath + " - " + Strings.GetLabelString("RestoreNoLocationFound"));
                this.Close();
            } else if (restore.path_candidates.Count == 1) {
                if (restore.recommended_path.EV == EnvironmentVariable.PS3Export ||
                    restore.recommended_path.EV == EnvironmentVariable.PS3Save ||
                    restore.recommended_path.EV == EnvironmentVariable.PSPSave) {
                    TranslationHelpers.translate(userBox, "RestoreRemovableDriveChoice");
                    TranslationHelpers.translate(singleUserBox,"RestoreRemovableDrive");
                    singlePathBox.Visibility = System.Windows.Visibility.Collapsed;
                } else {
                    singlePathBox.Visibility = System.Windows.Visibility.Visible;
                }
                pathBox.Visibility = System.Windows.Visibility.Collapsed;
                pathCombo.SelectedItem = restore.recommended_path;
                if (Restore.RestoreProgramHandler.use_defaults) {
                    close_when_done = true;
                    if (restore.user_candidates.Count <= 1) {
                        this.beginRestoration();
                    }
                }
            } else {
                pathLabel.Visibility = System.Windows.Visibility.Collapsed;
                pathBox.Visibility = System.Windows.Visibility.Visible;
                singlePathBox.Visibility = System.Windows.Visibility.Collapsed;
                pathCombo.SelectedItem = restore.recommended_path;
            }
        }



        private void pathCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            otherUserButton.Visibility = System.Windows.Visibility.Collapsed;
            restore.populateUsers(pathCombo.SelectedItem as LocationPath);
            if (restore.user_candidates.Count == 0) {
                userBox.Visibility = System.Windows.Visibility.Collapsed;
                singleUserBox.Visibility = System.Windows.Visibility.Collapsed;
                userCombo.SelectedIndex = 0;
            } else {
                if (restore.user_candidates.Count == 1) {
                    userBox.Visibility = System.Windows.Visibility.Collapsed;
                    singleUserBox.Visibility = System.Windows.Visibility.Visible;
                    userLabel.Content = restore.user_candidates[0];
                    userCombo.SelectedIndex = 0;
                } else {
                    singleUserBox.Visibility = System.Windows.Visibility.Collapsed;
                    userBox.Visibility = System.Windows.Visibility.Visible;
                    if (restore.user_candidates.Contains(restore.archive.id.Owner))
                        userCombo.SelectedItem = restore.archive.id.Owner;
                    else
                        userCombo.SelectedIndex = 0;
                }
                if (!Core.StaticAllUsersMode && restore.recommended_path.EV != EnvironmentVariable.PS3Export &&
                    restore.recommended_path.EV != EnvironmentVariable.PS3Save &&
                    restore.recommended_path.EV != EnvironmentVariable.PSPSave)
                    otherUserButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private bool close_when_done = false;

        private void shutDownWindow() {
            restore.cancel();
            TranslatingProgressHandler.setTranslatedMessage("Stopping");
            if (!flipper.IsActiveControl(restoreDoneLabel))
                Restore.RestoreProgramHandler.overall_stop = true;

            cancelButton.IsEnabled = false;
            close_when_done = true;


            if (restore.restore_worker == null || !restore.restore_worker.IsBusy) {
                try {
                    if (flipper.IsActiveControl(restoreDoneLabel))
                        this.DialogResult = true;
                    else
                        this.DialogResult = false;
                } catch (InvalidOperationException) {
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            this.shutDownWindow();
        }


        public override void enableInterface() {
            base.enableInterface();
            this.IsEnabled = true;
        }

        public override void disableInterface() {
            base.disableInterface();
            this.IsEnabled = false;
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e) {
            beginRestoration();
        }


        private void beginRestoration() {
            if (flipper.IsActiveControl(selectFilesGroup)) {
                if (files.SelectedItems.Count == 0) {
                    TranslatingMessageHandler.SendError("RestoreNoFiles");
                    return;
                }
            }

            TranslationHelpers.translate(this,"RestoringFile", restore.archive.id.ToString());
            restoreButton.Visibility = System.Windows.Visibility.Collapsed;
            choosePathButton.Visibility = System.Windows.Visibility.Collapsed;
            selectFilesButton.Visibility = System.Windows.Visibility.Collapsed;
            otherUserButton.Visibility = System.Windows.Visibility.Collapsed;
            TranslationHelpers.translate(cancelButton,"Stop");

            flipper.SwitchControl(ProgressBox);

            if (Restore.RestoreProgramHandler.use_defaults && userCombo.SelectedItem != null) {
                //RestoreProgramHandler.default_user = (string)userCombo.SelectedItem;
            }


            if (files != null) {
                foreach (SelectFile file in files) {
                    if (file.IsSelected) {
                        restore.specifyFileToRestore(file.name);
                    }
                }
            }
            restore.restoreBackup((LocationPath)pathCombo.SelectedItem, (string)userCombo.SelectedItem, restoreComplete);
        }


        void restoreComplete(object sender, RunWorkerCompletedEventArgs e) {
            TranslationHelpers.translate(cancelButton,"Close");
            TranslationHelpers.translate(this,"Finished");

            flipper.SwitchControl(restoreDoneLabel);
            if (close_when_done)
                this.Close();
        }


        private void otherUserButton_Click(object sender, RoutedEventArgs e) {
            if (!File.Exists(Core.programs.restore)) {
                this.showTranslatedError("FileNotFoundCritical", Core.programs.restore);
                return;
            }

            this.Visibility = System.Windows.Visibility.Collapsed;
            if (SecurityHandler.elevation(Core.programs.restore, "-allusers \"" + restore.archive.ArchiveFile.FullName + "\""))
                this.Close();
            else
                this.Visibility = System.Windows.Visibility.Visible;

        }

        private void choosePathButton_Click(object sender, RoutedEventArgs e) {
            string target = null;
            target = this.promptForPath(Strings.GetLabelString("RestoreLocationChoice"), Environment.SpecialFolder.MyComputer, null);
            if (target != null) {
                restore.addPathCandidate(new ManualLocationPathHolder(target));
                refreshPaths();
            }
        }

        Model<SelectFile> files = null;

        private void selectFilesButton_Click(object sender, RoutedEventArgs e) {
            this.setTranslatedTitle("SelectFiles");

            flipper.SwitchControl(selectFilesGroup);

            choosePathButton.Visibility = System.Windows.Visibility.Collapsed;
            selectFilesButton.Visibility = System.Windows.Visibility.Collapsed;
            otherUserButton.Visibility = System.Windows.Visibility.Collapsed;
            restoreButton.Visibility = System.Windows.Visibility.Visible;
            files = new Model<SelectFile>();
            foreach (string file in restore.archive.file_list) {
                files.Add(new SelectFile(file));
            }
            ICollectionView files_list_view = System.Windows.Data.CollectionViewSource.GetDefaultView(files);
            files_list_view.SortDescriptions.Clear();
            files_list_view.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));

            listView1.DataContext = files;
        }

        private void selectAllCheck_Checked(object sender, RoutedEventArgs e) {

        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            foreach (SelectFile file in files) {
                file.IsSelected = true;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            foreach (SelectFile file in files) {
                file.IsSelected = false;
            }
        }


    }

}
