﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using MASGAU.Archive;
using MASGAU.Location;
using MASGAU.Location.Holders;
using Communication.Progress;
using Communication.Translator;
using Translator;
using MVC;
namespace MASGAU.Restore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RestoreWindow : AProgramWindow//, IRestoreWindow
    {
        
        RestoreProgramHandler restore;
        ArchiveHandler archive = null;

        public RestoreWindow(): this(null) {}

        public RestoreWindow(ArchiveHandler archive, AWindow owner): base(new RestoreProgramHandler(archive), owner) {
            InitializeComponent();
            Translator.WPF.TranslationHelpers.translateWindow(this);
            
            default_progress_color = restoreProgress.Foreground;
            this.archive = archive;
        }

        public RestoreWindow(AWindow owner): base(new RestoreProgramHandler(null), owner) {
            InitializeComponent();
            Translator.WPF.TranslationHelpers.translateWindow(this);
            default_progress_color = restoreProgress.Foreground;
        }

        protected override void WindowLoaded(object sender, RoutedEventArgs e)
        {
            restoreProgress.IsIndeterminate = true;
            restore = (RestoreProgramHandler)program_handler;
            base.WindowLoaded(sender, e);
        }

        protected override void setup(object sender, RunWorkerCompletedEventArgs e)
        {
            base.setup(sender, e);

            if(e.Error!=null)
                return;

            this.archive = restore.archive;
            ProgressHandler.state = ProgressState.None;

            tabControl1.SelectedIndex = 1;
            this.Title = Strings.getGeneralString("RestoreConfirmPath");

            pathCombo.ItemsSource = restore.path_candidates;
            userCombo.ItemsSource = restore.user_candidates;
            pathLabel.DataContext = restore;
            userLabel.DataContext = restore;

            otherUserButton.Visibility = System.Windows.Visibility.Collapsed;
            selectFilesButton.Visibility = System.Windows.Visibility.Visible;
            restoreButton.Visibility = System.Windows.Visibility.Visible;
            choosePathButton.Visibility = System.Windows.Visibility.Visible;

            if(restore.game_data.restore_comment!=null) {
                restoreDoneText.Text = restore.game_data.restore_comment + Environment.NewLine + Environment.NewLine + Strings.get("RestoreCompleteWithComment") ;
            }

            refreshPaths();

            this.enableInterface();

        }

        public override void updateProgress(Communication.Progress.ProgressUpdatedEventArgs e) {
            if(e.message!=null)
                groupBox1.Header = e.message;
            applyProgress(restoreProgress,e);
        }

        private void refreshPaths() {
            if(restore.path_candidates.Count==0) {
                pathBox.Visibility = System.Windows.Visibility.Collapsed;
                singlePathBox.Visibility = System.Windows.Visibility.Visible;
                restoreButton.IsEnabled = false;
                pathLabel.Content = Strings.get("RestoreNoCandidatesFound");
                Restore.RestoreProgramHandler.unsuccesfull_restores.Add(archive.file_name + " - " + Strings.get("RestoreNoLocationFound"));
                this.Close();
            } else if(restore.path_candidates.Count==1) {
                if(restore.recommended_path.rel_root== EnvironmentVariable.PS3Export||
                    restore.recommended_path.rel_root== EnvironmentVariable.PS3Save||
                    restore.recommended_path.rel_root== EnvironmentVariable.PSPSave) {
                    userBox.Header = Strings.get("RestoreRemovableDriveChoice");
                    singleUserBox.Header = Strings.get("RestoreRemovableDrive");
                    singlePathBox.Visibility = System.Windows.Visibility.Collapsed;
                } else {
                    singlePathBox.Visibility = System.Windows.Visibility.Visible;
                }
                pathBox.Visibility = System.Windows.Visibility.Collapsed;
                pathCombo.SelectedItem = restore.recommended_path;
                if(Restore.RestoreProgramHandler.use_defaults) {
                    close_when_done = true;
                    if(restore.user_candidates.Count<=1) {
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



        private void pathCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            otherUserButton.Visibility = System.Windows.Visibility.Collapsed;
            restore.populateUsers(pathCombo.SelectedItem as LocationPathHolder);
            if(restore.user_candidates.Count==0) {
                userBox.Visibility = System.Windows.Visibility.Collapsed;
                singleUserBox.Visibility = System.Windows.Visibility.Collapsed;
                userCombo.SelectedIndex = 0;
            } else {
                if(restore.user_candidates.Count==1) {
                    userBox.Visibility = System.Windows.Visibility.Collapsed;
                    singleUserBox.Visibility = System.Windows.Visibility.Visible;
                    userLabel.Content = restore.user_candidates[0];
                    userCombo.SelectedIndex = 0;
                } else {
                    singleUserBox.Visibility = System.Windows.Visibility.Collapsed;
                    userBox.Visibility = System.Windows.Visibility.Visible;
                    if(restore.user_candidates.Contains(restore.archive.id.owner))
                        userCombo.SelectedItem = restore.archive.id.owner;
                    else
                        userCombo.SelectedIndex = 0;
                }
                if (!Core.all_users_mode && restore.recommended_path.rel_root != MASGAU.Location.EnvironmentVariable.PS3Export &&
                    restore.recommended_path.rel_root != MASGAU.Location.EnvironmentVariable.PS3Save &&
                    restore.recommended_path.rel_root!= MASGAU.Location.EnvironmentVariable.PSPSave)
                    otherUserButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private bool close_when_done = false;

        private void shutDownWindow() {
            restore.cancel();
            TranslatingProgressHandler.setTranslatedMessage("Stopping") ;
            if(tabControl1.SelectedIndex != 3)
                Restore.RestoreProgramHandler.overall_stop = true;
            
            cancelButton.IsEnabled = false;
            close_when_done = true;

            if(restore.restore_worker==null||!restore.restore_worker.IsBusy) {
                try {
                    if(tabControl1.SelectedIndex == 3)
                        this.DialogResult = true;
                    else
                        this.DialogResult = false;
                } catch (InvalidOperationException) {
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.shutDownWindow();
        }

        
        public override void enableInterface()
        {
            base.enableInterface();
            this.IsEnabled = true;
        }

        public override void disableInterface()
        {
            base.disableInterface();
            this.IsEnabled = false;
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            beginRestoration();
        }


        private void beginRestoration() {
            if(tabControl1.SelectedIndex == 2) {
                if(files.SelectedItems.Count==0) {
                    TranslatingMessageHandler.SendError("RestoreNoFiles");
                    return;
                }
            }

            this.Title = Strings.getGeneralString("RestoringFile", restore.archive.id.ToString());
            restoreButton.Visibility = System.Windows.Visibility.Collapsed;
            choosePathButton.Visibility = System.Windows.Visibility.Collapsed;
            selectFilesButton.Visibility = System.Windows.Visibility.Collapsed;
            otherUserButton.Visibility = System.Windows.Visibility.Collapsed;
            cancelButton.Content = Strings.get("StopButton");
            tabControl1.SelectedIndex = 0;

            if(Restore.RestoreProgramHandler.use_defaults&&userCombo.SelectedItem!=null) {
                //RestoreProgramHandler.default_user = (string)userCombo.SelectedItem;
            }


            if(files!=null) {
                foreach(SelectFile file in files) {
                    if(file.IsSelected) {
                        restore.specifyFileToRestore(file.name);
                    }
                }
            }
            restore.restoreBackup((LocationPathHolder)pathCombo.SelectedItem,(string)userCombo.SelectedItem,restoreComplete);
        }


        void restoreComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            cancelButton.Content = Strings.get("CloseButton");
            this.Title = Strings.getGeneralString("Finished");
            tabControl1.SelectedIndex = 3;
            if(close_when_done)
                this.Close();
        }


        private void otherUserButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            if(SecurityHandler.elevation(Core.programs.restore,"-allusers \"" + restore.archive.file_name + "\""))
                this.Close();
            else
                this.Visibility = System.Windows.Visibility.Visible;

        }

        private void choosePathButton_Click(object sender, RoutedEventArgs e)
        {
            string target = promptForPath(Strings.getGeneralString("RestoreLocationChoice"));
            if(target!=null) {
                restore.addPathCandidate(new ManualLocationPathHolder(target));
                refreshPaths();
            }
        }

        Model<SelectFile> files = null;

        private void selectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Title = Strings.getGeneralString("SelectFiles");
            tabControl1.SelectedIndex = 2;
            choosePathButton.Visibility = System.Windows.Visibility.Collapsed;
            selectFilesButton.Visibility = System.Windows.Visibility.Collapsed;
            otherUserButton.Visibility = System.Windows.Visibility.Collapsed;
            restoreButton.Visibility = System.Windows.Visibility.Visible;
            files = new Model<SelectFile>();
            foreach(string file in restore.archive.file_list) {
                files.Add(new SelectFile(file));
            }
            ICollectionView files_list_view = System.Windows.Data.CollectionViewSource.GetDefaultView(files);
            files_list_view.SortDescriptions.Clear();
            files_list_view.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));

            listView1.DataContext = files;
        }

        private void selectAllCheck_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            foreach(SelectFile file in files) {
                file.IsSelected = true;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            foreach(SelectFile file in files) {
                file.IsSelected = false;
            }
        }


    }

}
