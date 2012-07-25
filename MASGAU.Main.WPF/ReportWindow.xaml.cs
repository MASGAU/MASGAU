using System;
using System.Windows;
using System.IO;
using System.Text;
using MASGAU.Analyzer;
using MVC;
using Communication.WPF;
using Translator;
using Email.WPF;
namespace MASGAU {
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : NewWindow {
        AAnalyzer analyzer;

        public ReportWindow(AAnalyzer analyzer, ACommunicationWindow owner)
            : base(owner) {
            InitializeComponent();
            this.analyzer = analyzer;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {

//            email.checkAvailability(checkAvailabilityDone);
            uploadBtn.Content = "Checking E-Mail Server...";
        }

        private void checkAvailabilityDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            //if (email.email_available) {
            //    uploadBtn.IsEnabled = true;
            //    uploadBtn.Content = "E-Mail Report";
            //} else {
            //    uploadBtn.IsEnabled = false;
            //    uploadBtn.Content = "E-Mail Server Blocked";
            //}
        }

        private string prepareReport() {
            StringBuilder return_me = new StringBuilder(reportTxt.Text);
            //if((bool)publishPermissionCheck.IsChecked) {
            //  return_me.AppendLine();
            //  return_me.AppendLine("Permission to publish has been granted");
            //}
            return return_me.ToString();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.DefaultExt = "txt";
            save.Filter = "Text files|*.txt|All files|*";
            save.Title = "Where Do You Want To Save The Report?";

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
                } catch {
                    displayError("Pick somewhere else", "Eror while trying to write " + save.FileName);
                }
            }

        }

        private void uploadBtn_Click(object sender, RoutedEventArgs e) {
            if (Core.settings.email == null || Core.settings.email == "") {
                EmailWindow get_email = new EmailWindow(this);
                if ((bool)get_email.ShowDialog()) {
                    Core.settings.email = get_email.email;
                } else {
                    return;
                }
            }

            StringBuilder body = new StringBuilder();
            body.AppendLine(prepareReport());

            uploadBtn.IsEnabled = false;
            closeBtn.IsEnabled = false;
            uploadBtn.Content = "Sending E-Mail...";
            //email.sendEmail(EmailHandler.email_recepient, EmailHandler.email_recepient, Core.settings.email, "MASGAU - " + name, body.ToString(), sendEmailDone);
        }
        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                uploadBtn.Content = "E-Mail Not Working";
                displayError("Error time", e.Error.Message);
            } else {
                uploadBtn.Content = "E-Mail Sent";
            }
            uploadBtn.IsEnabled = false;
            closeBtn.IsEnabled = true;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }


    }
}