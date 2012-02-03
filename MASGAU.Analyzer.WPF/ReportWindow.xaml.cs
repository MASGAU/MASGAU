using System;
using System.Windows;
using System.IO;
using System.Text;

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

        }

    
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			reportTxt.Text = report;

            disclaimerText.Text    = "Submitted reports are published to a public server. "
                                    + "No one involved with MASGAU can be held liable for any repurcusions from the publishing of "
                                    + "sensitive information through these reports, so please carefully "
                                    + "look over and edit the report for any information that you do not want to be made public. "
                                    + "By submitting a report you are agreeing that it is free for public dispersal.";

            
            //if (Core.settings.reports_okay_to_publish == null) {
              //  Core.settings.reports_okay_to_publish =
                //    this.askQuestion("Legal Mumbo Jumbo", "Is it okay to publish the reports you submit to MASGAU's public SVN server?\n"
                  //  + disclaimer + "\n"
                    //+ "(This choice will be remembered and can be changed at any time from a report window)");
            //}

            //publishPermissionCheck.DataContext = Core.settings;

            email.checkAvailability(checkAvailabilityDone);
            uploadBtn.Content = "Checking E-Mail Server...";
        }

        private void checkAvailabilityDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(email.email_available) {
                uploadBtn.IsEnabled = true;
                uploadBtn.Content = "E-Mail Report";
            } else {
                uploadBtn.IsEnabled = false;
                uploadBtn.Content = "E-Mail Server Blocked";
            }

        }

        private string prepareReport() {
            StringBuilder return_me = new StringBuilder(reportTxt.Text);
            //if((bool)publishPermissionCheck.IsChecked) {
              //  return_me.AppendLine();
              //  return_me.AppendLine("Permission to publish has been granted");
            //}
            return return_me.ToString();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.DefaultExt = "txt";
            save.Filter = "Text files|*.txt|All files|*";
            save.Title = "Where Do You Want To Save The Report?";
            if(AnalyzerProgramHandler.last_save_path==null)
                save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                save.InitialDirectory = AnalyzerProgramHandler.last_save_path;

            save.FileName = name + ".txt";
			if(save.ShowDialog(this.GetIWin32Window())!= System.Windows.Forms.DialogResult.Cancel) {
                AnalyzerProgramHandler.last_save_path = Path.GetDirectoryName(save.FileName);
				try {
					StreamWriter writer = File.CreateText(save.FileName);
					writer.Write(prepareReport());
					writer.Close();
				} catch {
                    showError("Pick somewhere else","Eror while trying to write " + save.FileName);
				}
			}

        }

        private void uploadBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Core.settings.email==null||Core.settings.email=="") {
                EmailWindow get_email = new EmailWindow(this);
                if((bool)get_email.ShowDialog()) {
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
            email.sendEmail(EmailHandler.email_recepient,EmailHandler.email_recepient,Core.settings.email,"MASGAU - " + name, body.ToString(), sendEmailDone);
        }
        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Error!=null) {
                uploadBtn.Content = "E-Mail Not Working";
                showError("Error time",e.Error.Message);
            } else {
                uploadBtn.Content = "E-Mail Sent";
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
