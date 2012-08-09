using System;
using System.ComponentModel;
using System.Windows;
using MVC.Communication;
using MASGAU.Update;
using Translator;
using Translator.WPF;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        private void UpdateButton_Click(object sender, RoutedEventArgs e) {
            checkUpdates();

        }

        private BackgroundWorker updateWorker;
        private void UpdateAvailableButton_Click(object sender, RoutedEventArgs e) {
            if (result == UpdateAvailability.None)
                return;

            if (result == UpdateAvailability.Data) {
                updateWorker = new BackgroundWorker();
                updateWorker.DoWork += new DoWorkEventHandler(updateWorker_DoWork);
                updateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updateWorker_RunWorkerCompleted);
                disableInterface();
                updateWorker.RunWorkerAsync();
            } else {
                Core.updater.downloadProgramUpdate();
            }
        }

        void updateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            enableInterface();
            askRefreshGames("RefreshForUpdate");
            UpdateButton.IsEnabled = true;
            UpdateButton.Visibility = System.Windows.Visibility.Visible;
            UpdateAvailableButton.IsEnabled = false;
            UpdateAvailableButton.Visibility = System.Windows.Visibility.Collapsed;
        }

        void updateWorker_DoWork(object sender, DoWorkEventArgs e) {
            Core.updater.downloadDataUpdates();
        }



        public void checkUpdates() {
            UpdateButton.IsEnabled = false;
            TranslationHelpers.translate(UpdateButton, "CheckingForUpdates");
            BackgroundWorker update = new BackgroundWorker();
            Core.updater = new Update.Updater();
            ProgressHandler.saveMessage();
            update.DoWork += new DoWorkEventHandler(update_DoWork);
            update.RunWorkerCompleted += new RunWorkerCompletedEventHandler(update_RunWorkerCompleted);
            update.RunWorkerAsync();
        }

        void update_DoWork(Object sender, DoWorkEventArgs e) {
            e.Result = Core.updater.checkUpdates();
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
                    if (this.Visibility != System.Windows.Visibility.Visible)
                        notifier.sendBalloon(Strings.GetLabelString("DataUpdateAvailable"));
                    break;
                case UpdateAvailability.DataAndProgram:
                case UpdateAvailability.Program:
                    TranslationHelpers.translate(UpdateAvailableButton, "ProgramUpdateAvailable");
                    if (this.Visibility != System.Windows.Visibility.Visible)
                        notifier.sendBalloon(Strings.GetLabelString("ProgramUpdateAvailable"));
                    break;
            }
            TranslationHelpers.translate(UpdateButton, "CheckForUpdates");

        }


    }
}
