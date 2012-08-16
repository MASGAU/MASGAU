using System;
using System.Windows;
using System.IO;
using System.Text;
using MASGAU.Analyzer;
using MVC;
using MVC.WPF;
using MVC.Translator;
using Translator;
using Translator.WPF;
using SMJ.WPF;
using Email;
using Email.WPF;
namespace MASGAU {
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportProblemWindow : NewWindow {
        public ReportProblemWindow(ACommunicationWindow owner)
            : base(owner) {
            InitializeComponent();
            TranslationHelpers.translateWindow(this);
            this.Icon = owner.Icon;

            uploadBtn.To = Core.submission_email;
            uploadBtn.Subject = "Problem Report";
            uploadBtn.Source = Core.settings;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        private void reportTxt_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            uploadBtn.Message = reportTxt.Text;
        }


    }
}