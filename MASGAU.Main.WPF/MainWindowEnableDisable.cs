using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Effects;

namespace MASGAU.Main {
    public partial class MainWindowNew {
        private bool disabled;
        List<System.ComponentModel.BackgroundWorker> cancellables = new List<System.ComponentModel.BackgroundWorker>();

        public void disableInterface() {
            setInterfaceEnabledness(false);

            CancelButton.IsEnabled = false;
            CancelButton.Visibility = System.Windows.Visibility.Collapsed;
            Communication.ProgressHandler.saveMessage();
        }

        public void disableInterface(System.ComponentModel.BackgroundWorker cancellable_item) {
            cancellables.Add(cancellable_item);
            cancellable_item.RunWorkerCompleted +=new System.ComponentModel.RunWorkerCompletedEventHandler(cancellable_item_RunWorkerCompleted);
            Translator.WPF.TranslationHelpers.translate(CancelText, "Stop");
            setInterfaceEnabledness(false);
            Communication.ProgressHandler.saveMessage();
        }

        void  cancellable_item_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            worker.RunWorkerCompleted -=new System.ComponentModel.RunWorkerCompletedEventHandler(cancellable_item_RunWorkerCompleted);
            cancellables.Remove(worker); 	        
        }

        public void enableInterface() {
            setInterfaceEnabledness(true);
            Communication.ProgressHandler.restoreMessage();
            statusBarLabel.Content = Communication.ProgressHandler.message;
        }


        private void setInterfaceEnabledness(bool status) {
            BlurEffect blur;
            disabled = !status;
            CancelButton.IsEnabled = !status;

            System.Windows.Visibility a, b;
            if (status) {
                // this is when enabled
                a = System.Windows.Visibility.Collapsed;
                b = System.Windows.Visibility.Visible;
                blur = null;
            } else {
                // this is when disabled
                a = System.Windows.Visibility.Visible;
                b = System.Windows.Visibility.Collapsed;
                blur = new BlurEffect();
                blur.Radius = 10;
            }

            progress.Effect = blur;

            //ribbon.Effect = blur;
            //subGrid.Effect = blur;

            CancelButton.Visibility = a;
            DisablerGrid.Visibility = a;
        }

        private void CancelButton_Click_1(object sender, RoutedEventArgs e) {
            cancelWorkers();
        }

        private void cancelWorkers() {
            CancelButton.IsEnabled = false;
            Translator.WPF.TranslationHelpers.translate(CancelText, "Stopping");
            foreach (BackgroundWorker worker in cancellables) {
                worker.CancelAsync();
            }
        }
    }
}
