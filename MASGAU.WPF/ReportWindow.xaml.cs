using System;
using System.IO;
using System.Text;
using System.Windows;
using MASGAU.Analyzer;
using MVC.Translator;
using MVC.WPF;
using Translator;
using Translator.WPF;
namespace MASGAU {
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : NewWindow {
        AAnalyzer analyzer;
        public bool SentOrSaved {
            get {
                return uploadBtn.Sent || saved;
            }
        }
        public ReportWindow(AAnalyzer analyzer, ACommunicationWindow owner)
            : base(owner) {
            InitializeComponent();
            TranslationHelpers.translateWindow(this);
            this.Icon = owner.Icon;
            this.analyzer = analyzer;
            reportTxt.Text = analyzer.report;

            uploadBtn.To = Core.submission_email;
            uploadBtn.Subject = "Game Data - " + analyzer.game.Title;
            uploadBtn.Message = analyzer.report;
            uploadBtn.Source = Core.settings;
        }

        private string prepareReport() {
            StringBuilder return_me = new StringBuilder();
            return return_me.ToString();
        }


        private bool saved = false;
        private void saveBtn_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.DefaultExt = "txt";
            save.Filter = Strings.GetLabelString("TxtFileDescriptionPlural") + "|*.txt|" + Strings.GetLabelString("AllFileDescriptionPlural") + "|*";
            save.Title = Strings.GetLabelString("SaveReportQuestion");

            if (AAnalyzer.LastSavePath == null)
                save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                save.InitialDirectory = AAnalyzer.LastSavePath;

            save.FileName = analyzer.game.Name + ".txt";
            if (save.ShowDialog(this.GetIWin32Window()) != System.Windows.Forms.DialogResult.Cancel) {
                AAnalyzer.LastSavePath = Path.GetDirectoryName(save.FileName);
                try {
                    StreamWriter writer = File.CreateText(save.FileName);
                    writer.Write(prepareReport());
                    writer.Close();
                    saved = true;
                } catch (Exception ex) {
                    TranslatingMessageHandler.SendError("WriteError", ex, save.FileName);
                }
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        private void reportTxt_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            uploadBtn.Message = reportTxt.Text;
        }


    }
}