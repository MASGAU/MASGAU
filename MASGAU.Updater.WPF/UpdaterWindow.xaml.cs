using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using MASGAU.Update;
using Translations;
namespace MASGAU.Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UpdaterWindow : AWindow
    {
        Color up_to_date = Color.FromArgb(255,115,210,22);
        Color out_of_date = Color.FromArgb(255,239,41,41);
        Color updating = Color.FromArgb(255,252,233,79);
        
        private UpdateProgramHandler updater;

        public UpdaterWindow(): base(null)
        {
            InitializeComponent();
            WPFHelpers.translateWindow(this);
            this.Loaded +=new RoutedEventHandler(Window_Loaded);
            //updateLst.ItemsSource = UpdateCollection;
        }

        public override void updateProgress(Communication.Progress.ProgressUpdatedEventArgs e)
        {
            if(e.message!=null)
                this.progressBox.Header = e.message;
            applyProgress(this.progressBar1,e);
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updater = new UpdateProgramHandler();
            updater.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(updateComplete);
            BackgroundWorker updatecheck = new BackgroundWorker();
            updatecheck.DoWork += new DoWorkEventHandler(updatecheck_DoWork);
            updatecheck.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updatecheck_RunWorkerCompleted);
            updatecheck.RunWorkerAsync();
        }

        void updatecheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(updater.updater.shutdown_required||!updater.updater.update_available) {
                this.Close();
                return;
            }
            
            this.tabControl1.SelectedIndex = 1;
            updateLst.ItemsSource = updater.updater;

            groupBox1.Header = Strings.get("UpdatesAvailable");
            okBtn.IsEnabled = true;
            cancelBtn.IsEnabled = true;
        }

        void updatecheck_DoWork(object sender, DoWorkEventArgs e)
        {
            updater.updater.checkUpdates(true,false);
        }            


        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            updateLst.IsEnabled = false;
            okBtn.IsEnabled = false;
            cancelBtn.IsEnabled = false;
            updater.RunWorkerAsync();
        }


        private void updateComplete(object sender, RunWorkerCompletedEventArgs e) {
            okBtn.Visibility = System.Windows.Visibility.Collapsed;
            cancelBtn.Visibility = System.Windows.Visibility.Collapsed;
            donBtn.Visibility = System.Windows.Visibility.Visible;
            //this.tabControl1.TabIndex = 3;
            Environment.ExitCode =  1;
            //this.Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void donBtn_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
        
    
    }
}
