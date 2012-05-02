using System;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Registry;

namespace MASGAU.Analyzer
{
 
    /// <summary>
    /// Interaction logic for SearchingWindow.xaml
    /// </summary>
    public partial class SearchingWindow : AWindow
    {
		private string game_path, save_path, game_name;

        private AnalyzerProgramHandler analyzer;

        private bool playstation_search;
        private bool cancelled = false;

        public SearchingWindow(AnalyzerProgramHandler analyzer, string new_game_path, string new_save_path, string new_game_name, bool search_playstation, AWindow owner): base(owner)
        {
            this.analyzer = analyzer;
			InitializeComponent();
            WPFHelpers.translateWindow(this);
			game_path = new_game_path;
			save_path = new_save_path;
            game_name = new_game_name;
            playstation_search = search_playstation;
        }

        public override void updateProgress(Communication.Progress.ProgressUpdatedEventArgs e) {
            if (e.message != null)
                groupBox1.Header = e.message;

            this.applyProgress(progressBar1, e);
        }

        #region BackgroundWorker Event Handlers
        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
            TaskbarItemInfo.ProgressValue = e.ProgressPercentage/(double)6;
            progressBar1.Value = e.ProgressPercentage/(double)6*100;
            groupBox1.Header = (string)e.UserState;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.output = analyzer.output.ToString();
            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            this.Closing -= new CancelEventHandler(Window_Closing);
            if(cancelled)
                this.DialogResult = false;
            else 
			    this.DialogResult = true;
        }
        #endregion



        #region Other Event Handlers


        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            cancelled = true;
            cancelBtn.IsEnabled = false;
            analyzer.cancelAnalyzer();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            cancelled = true;
            cancelBtn.IsEnabled = false;
            analyzer.cancelAnalyzer();
            e.Cancel = true;
        }
        public string output { get; protected set; }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            analyzer.runAnalyzer(backgroundWorker1_RunWorkerCompleted);
        }

        #endregion
    }
}
