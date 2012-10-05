﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Effects;
using MVC;
using MVC.Communication;
using SMJ.WPF.Effects;
namespace MASGAU.Main {
    public partial class MainWindowNew {
        private bool disabled;
        List<ICancellable> cancellables = new List<ICancellable>();

        public override void disableInterface() {
            setInterfaceEnabledness(false);

            CancelButton.IsEnabled = false;
            CancelButton.Visibility = System.Windows.Visibility.Collapsed;
            ProgressHandler.saveMessage();
        }

        public void disableInterface(ICancellable cancellable_item) {
            cancellables.Add(cancellable_item);
            cancellable_item.Completed += new System.ComponentModel.RunWorkerCompletedEventHandler(cancellable_item_RunWorkerCompleted);
            Translator.WPF.TranslationHelpers.translate(CancelButton.Label, "Stop");
            setInterfaceEnabledness(false);
            ProgressHandler.saveMessage();
        }

        void cancellable_item_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            ICancellable worker = (ICancellable)sender;
            worker.Completed -= new System.ComponentModel.RunWorkerCompletedEventHandler(cancellable_item_RunWorkerCompleted);
            cancellables.Remove(worker);
        }

        public override void enableInterface() {
            setInterfaceEnabledness(true);
            ProgressHandler.restoreMessage();
            statusBarLabel.Content = ProgressHandler.message;
        }


        private void setInterfaceEnabledness(bool status) {
            BlurEffect blur;
            disabled = !status;
            CancelButton.IsEnabled = !status;
            ribbon.IsEnabled = status;
            GameGrid.IsEnabled = status;
            ArchiveGrid.IsEnabled = status;

            System.Windows.Visibility a;
            FadeEffect fade;
            if (status) {
                // this is when enabled
                fade = new FadeOutEffect(timing);
                a = System.Windows.Visibility.Collapsed;
                //                b = System.Windows.Visibility.Visible;
                blur = null;
            } else {
                fade = new FadeInEffect(timing);
                // this is when disabled
                a = System.Windows.Visibility.Visible;
                //              b = System.Windows.Visibility.Collapsed;
                blur = new BlurEffect();
                blur.Radius = 10;
            }
            fade.Start(DisablerGrid);

            notifier.Animated = !status;

            notifier.MenuEnabled = status;
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
            foreach (ICancellable worker in cancellables) {
                worker.Cancel();
            }
        }
    }
}
