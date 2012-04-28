using System;
using System.Text;
using System.IO;
using Translations;
using MASGAU.Gtk;
namespace MASGAU.Analyzer
{
	public partial class ReportDialog : MASGAU.Gtk.ADialog
	{
		private string name;
		private EmailHandler email;
		public ReportDialog (string report, string name, AWindow parent): base(parent)
		{
			this.Build ();
			GTKHelpers.translateWindow(this);
			this.name = name;
			this.reportText.Buffer.Text = report;
			email = new EmailHandler();
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
			this.disableInterface();
			global::Gtk.FileChooserDialog save = new global::Gtk.FileChooserDialog(Strings.get("SaveReportQuestion"),this,global::Gtk.FileChooserAction.Save,Strings.get("CancelButton"),global::Gtk.ResponseType.Cancel,Strings.get("SaveButton"),global::Gtk.ResponseType.Ok);
			global::Gtk.FileFilter filter = new global::Gtk.FileFilter();
			filter.Name = Strings.get("TxtFileDescriptionPlural");
			filter.AddPattern("*.txt");
			save.AddFilter(filter);
			filter = new global::Gtk.FileFilter();
			filter.Name = Strings.get("AllFileDescriptionPlural");
			filter.AddPattern("*");
			save.AddFilter(filter);

            if(AnalyzerProgramHandler.last_save_path==null)
                save.SelectFilename(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),name + ".txt"));
            else
                save.SelectFilename(System.IO.Path.Combine(AnalyzerProgramHandler.last_save_path,name + ".txt"));

			if(save.Run()!= (int)global::Gtk.ResponseType.Cancel) {
                AnalyzerProgramHandler.last_save_path = System.IO.Path.GetDirectoryName(save.Filename);
				try {
					StreamWriter writer = File.CreateText(save.Filename);
					writer.Write(reportText.Buffer.Text);
					writer.Close();
				} catch {
					GTKHelpers.showError(this,Strings.get("WriteErrorPrompt"), Strings.get("WriteError") + " " + save.Filename);
				}
			}
			
			this.enableInterface();
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

