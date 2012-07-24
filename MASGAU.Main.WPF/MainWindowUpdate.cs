using System;
using System.ComponentModel;
using System.Windows;
using MVC.Communication;
using MASGAU.Update;
using Translator.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        private void UpdateButton_Click(object sender, RoutedEventArgs e) {
            checkUpdates();

        }

        private void UpdateAvailableButton_Click(object sender, RoutedEventArgs e) {
            this.WindowStyle = System.Windows.WindowStyle.None;

            if (result == UpdateAvailability.None)
                return;

            if (result > UpdateAvailability.Data) {
                Core.updater.downloadProgramUpdate();
            } else {
                Core.updater.downloadDataUpdates();
            }
        }

        public void checkUpdates() {
            UpdateButton.IsEnabled = false;
            TranslationHelpers.translate(UpdateButton, "CheckingForUpdates");
            BackgroundWorker update = new BackgroundWorker();
            Core.updater = new Update.UpdatesHandler();
            ProgressHandler.saveMessage();
            update.DoWork += new DoWorkEventHandler(update_DoWork);
            update.RunWorkerCompleted += new RunWorkerCompletedEventHandler(update_RunWorkerCompleted);
            update.RunWorkerAsync();
        }

        void update_DoWork(Object sender, DoWorkEventArgs e) {
            e.Result = Core.updater.checkUpdates(false, false);
        }
        UpdateAvailability result = UpdateAvailability.None;
        protected virtual void update_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            result = (UpdateAvailability)e.Result;

            if (result == UpdateAvailability.None) {
                UpdateButton.IsEnabled = true;
                UpdateButton.Visibility = System.Windows.Visibility.Visible;
                UpdateAvailableButton.IsEnabled = false;
                UpdateAvailableButton.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                UpdateButton.IsEnabled = false;
                UpdateButton.Visibility = System.Windows.Visibility.Collapsed;
                UpdateAvailableButton.IsEnabled = true;
                UpdateAvailableButton.Visibility = System.Windows.Visibility.Visible;
            }

            switch (result) {
                case UpdateAvailability.Data:
                    TranslationHelpers.translate(UpdateAvailableButton, "DataUpdateAvailable");
                    break;
                case UpdateAvailability.Program:
                    TranslationHelpers.translate(UpdateAvailableButton, "ProgramUpdateAvailable");
                    break;
            }
            TranslationHelpers.translate(UpdateButton, "CheckForUpdates");

        }


    }
}
