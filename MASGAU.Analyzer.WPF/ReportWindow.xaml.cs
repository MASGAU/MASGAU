using System;
using System.Windows;
using System.IO;
using System.Text;
using Translator;
using Email;
using Translator.WPF;
namespace MASGAU.Analyzer
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : AWindow
    {
		private string report, name;
        private EmailHandler email = new EmailHandler();

        public ReportWindow(string new_report, string new_name, AWindow owner): base(owner)
        {
			InitializeComponent();
			report = new_report;
            name = new_name;
            TranslationHelpers.translateWindow(this);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			reportTxt.Text = report;
            
            //if (Core.settings.reports_okay_to_publish == null) {
              //  Core.settings.reports_okay_to_publish =
                //    this.askQuestion("Legal Mumbo Jumbo", "Is it okay to publish the reports you submit to MASGAU's public SVN server?\n"
                  //  + disclaimer + "\n"
                    //+ "(This choice will be remembered and can be changed at any time from a report window)");
            //}

            //publishPermissionCheck.DataContext = Core.settings;

            email.checkAvailability(checkAvailabilityDone);
            uploadBtn.Content = Strings.getGeneralString("CheckingConnection");
        }

        private void checkAvailabilityDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(email.email_available) {
                uploadBtn.IsEnabled = true;
                uploadBtn.Content = Strings.get("SendReport");
            } else {
                uploadBtn.IsEnabled = false;
                uploadBtn.Content = Strings.get("CantSendReport");
            }

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.DefaultExt = "txt";
            save.Filter = Strings.getGeneralString("TxtFileDescriptionPlural") + "|*.txt|" + Strings.getGeneralString("AllFileDescriptionPlural") + "|*";
            save.Title = Strings.getGeneralString("SaveReportQuestion");
            if(AnalyzerProgramHandler.last_save_path==null)
                save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                save.InitialDirectory = AnalyzerProgramHandler.last_save_path;

            save.FileName = name + ".txt";
			if(save.ShowDialog(this.GetIWin32Window())!= System.Windows.Forms.DialogResult.Cancel) {
                AnalyzerProgramHandler.last_save_path = Path.GetDirectoryName(save.FileName);
				try {
					StreamWriter writer = File.CreateText(save.FileName);
					writer.Write(reportTxt.Text);
					writer.Close();
				} catch {
                    TranslationHelpers.showTranslatedError(this,"WriteError",save.FileName);
				}
			}

        }

        private void uploadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!this.checkEmail())
                return;

            StringBuilder body = new StringBuilder();
            body.AppendLine(reportTxt.Text);

            uploadBtn.IsEnabled = false;
            closeBtn.IsEnabled = false;
            uploadBtn.Content = Strings.get("SendingReport");
            email.sendEmail(Core.submission_email,Core.submission_email,Core.settings.email,"MASGAU - " + name, body.ToString(), sendEmailDone);
        }

        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Error!=null) {
                uploadBtn.Content = Strings.get("CantSendReport");
                showError("Error time",e.Error.Message);
            } else {
                uploadBtn.Content = Strings.get("ReportSent");
            }
            uploadBtn.IsEnabled = false;
            closeBtn.IsEnabled = true;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    
    }
}
