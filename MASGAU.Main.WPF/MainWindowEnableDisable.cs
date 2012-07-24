using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Effects;
using MASGAU.Effects;
using MVC.Communication;

namespace MASGAU.Main {
    public partial class MainWindowNew {
        private bool disabled;
        List<System.ComponentModel.BackgroundWorker> cancellables = new List<System.ComponentModel.BackgroundWorker>();

        public override void disableInterface() {
            setInterfaceEnabledness(false);

            CancelButton.IsEnabled = false;
            CancelButton.Visibility = System.Windows.Visibility.Collapsed;
            ProgressHandler.saveMessage();
        }

        public void disableInterface( System.ComponentModel.BackgroundWorker cancellable_item) {
            cancellables.Add(cancellable_item);
            cancellable_item.RunWorkerCompleted +=new System.ComponentModel.RunWorkerCompletedEventHandler(cancellable_item_RunWorkerCompleted);
            Translator.WPF.TranslationHelpers.translate(CancelButton.Label, "Stop");
            setInterfaceEnabledness(false);
            ProgressHandler.saveMessage();
        }

        void  cancellable_item_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = (System.ComponentModel.BackgroundWorker)sender;
            worker.RunWorkerCompleted -=new System.ComponentModel.RunWorkerCompletedEventHandler(cancellable_item_RunWorkerCompleted);
            cancellables.Remove(worker); 	        
        }

        public void enableInterface() {
            setInterfaceEnabledness(true);
            ProgressHandler.restoreMessage();
            statusBarLabel.Content = ProgressHandler.message;
        }

        double timing = 5.0;

        private void setInterfaceEnabledness(bool status) {
            BlurEffect blur;
            disabled = !status;
            CancelButton.IsEnabled = !status;
            ribbon.IsEnabled = status;
            GameGrid.IsEnabled = status;
            ArchiveGrid.IsEnabled = status;

            System.Windows.Visibility a, b;
            FadeEffect fade;
            if (status) {
                // this is when enabled
                fade = new FadeOutEffect(timing);
                a = System.Windows.Visibility.Collapsed;
                b = System.Windows.Visibility.Visible;
                blur = null;
            } else {
                fade = new FadeInEffect(timing);
                // this is when disabled
                a = System.Windows.Visibility.Visible;
                b = System.Windows.Visibility.Collapsed;
                blur = new BlurEffect();
                blur.Radius = 10;
            }
            fade.Start(DisablerGrid);

            //progress.Effect = blur;

            //ribbon.Effect = blur;
            //subGrid.Effect = blur;

            CancelButton.Visibility = a;
//            DisablerGrid.Visibility = a;
        }

        private void CancelButton_Click_1(object sender, RoutedEventArgs e) {
            cancelWorkers();
        }

        private void cancelWorkers() {
            CancelButton.IsEnabled = false;
            Translator.WPF.TranslationHelpers.translate(CancelButton.Label, "Stopping");
            foreach (BackgroundWorker worker in cancellables) {
                worker.CancelAsync();
            }
        }
    }
}
