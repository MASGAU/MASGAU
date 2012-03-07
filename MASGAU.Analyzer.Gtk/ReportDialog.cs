using System;
using System.Text;
using System.IO;
using Translations;
namespace MASGAU.Analyzer
{
	public partial class ReportDialog : MASGAU.ADialog
	{
		private string name;
		private EmailHandler email;
		public ReportDialog (string report, string name, AWindow parent): base(parent)
		{
			this.Build ();
			this.name = name;
			disclaimerLabel.Text = Strings.get("AnalyzerDisclaimer");
			this.reportText.Buffer.Text = report;
			this.Title = Strings.get("ReportWindowTitle");
			this.uploadButton.Label = Strings.get("Upload");
            email.checkAvailability(checkAvailabilityDone);
            uploadButton.Label = Strings.get("CheckingConnection");
		}
        private void checkAvailabilityDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(email.email_available) {
                uploadButton.Sensitive = true;
                uploadButton.Label = Strings.get("SendReport");
            } else {
                uploadButton.Sensitive = false;
                uploadButton.Label = Strings.get("CantSendReport");
            }

        }

		protected void OnSaveButtonClicked (object sender, System.EventArgs e)
		{
			Gtk.FileChooserDialog save = new Gtk.FileChooserDialog(Strings.get("SaveReportQuestion"),this,Gtk.FileChooserAction.Save,null);
			Gtk.FileFilter filter = new Gtk.FileFilter();
			filter.Name = Strings.get("TxtFileDescriptionPlural");
			filter.AddPattern("*.txt");
			save.AddFilter(filter);
			filter = new Gtk.FileFilter();
			filter.Name = Strings.get("AllFileDescriptionPlural");
			filter.AddPattern("*");
			save.AddFilter(filter);

            if(AnalyzerProgramHandler.last_save_path==null)
                save.SelectFilename(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),name + ".txt"));
            else
                save.SelectFilename(System.IO.Path.Combine(AnalyzerProgramHandler.last_save_path,name + ".txt"));

			if(save.Run()!= (int)Gtk.ResponseType.Cancel) {
                AnalyzerProgramHandler.last_save_path = System.IO.Path.GetDirectoryName(save.Filename);
				try {
					StreamWriter writer = File.CreateText(save.Filename);
					writer.Write(reportText.Buffer.Text);
					writer.Close();
				} catch {
					GTKHelpers.showError(this,Strings.get("WriteErrorPrompt"), Strings.get("WriteError") + " " + save.Filename);
				}
			}
		}

		protected void OnUploadButtonClicked (object sender, System.EventArgs e)
		{
			if(!GTKHelpers.checkEmail(this))
				return;

            StringBuilder body = new StringBuilder();
            body.AppendLine(reportText.Buffer.Text);

            uploadButton.Sensitive = false;
            closebutton.Sensitive = false;
            uploadButton.Label = Strings.get("SendingReport");
            email.sendEmail(Core.submission_email,Core.submission_email,Core.settings.email,"MASGAU - " + name, body.ToString(), sendEmailDone);
		}
		
        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Error!=null) {
                uploadButton.Label = Strings.get("CantSendReport");
                GTKHelpers.showError(this, "Error time",e.Error.Message);
            } else {
                uploadButton.Label = Strings.get("ReportSent");
            }
            uploadButton.Sensitive = false;
			closebutton.Sensitive = true;
        }

	}
}

