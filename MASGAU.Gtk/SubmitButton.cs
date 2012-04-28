using System;
using Translations;
namespace MASGAU
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SubmitButton : Gtk.Button
	{

		public EmailHandler email;
		public String subject = "", body = "", from = Core.submission_email, to = Core.submission_email,
		reply_to = Core.settings.email;
		public Gtk.Window window;
		public SubmitButton(Gtk.Window window, String subject, String body, Boolean cancel)
		{
			this.Build ();
			this.window = window;
			this.subject = subject;
			this.body = body;
			
			cancelButton.Visible = cancel;
			closeButton.Visible = !cancel;
		}
		
		public void checkConnection() {
			email = new EmailHandler();
            email.checkAvailability(checkAvailabilityDone);
            submitButton.Label = Strings.get("CheckingConnection");
		}
		
		
        private void checkAvailabilityDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(email.email_available) {
                submitButton.Sensitive = true;
                submitButton.Label = Strings.get("SendReport");
            } else {
                submitButton.Sensitive = false;
                submitButton.Label = Strings.get("CantSendReport");
            }
        }
		
		
		protected void OnSubmitButtonClicked (object sender, System.EventArgs e)
		{
			if(!GTKHelpers.checkEmail(window))
				return;


            submitButton.Sensitive = false;
            closeButton.Sensitive = false;
			cancelButton.Sensitive = false;
			
            submitButton.Label = Strings.get("SendingReport");
            email.sendEmail(from,to,reply_to,subject,body, sendEmailDone);
		}
		
		
        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Error!=null) {
                submitButton.Label = Strings.get("CantSendReport");
                GTKHelpers.showError(window, "Error time",e.Error.Message);
            } else {
                submitButton.Label = Strings.get("ReportSent");
            }
            submitButton.Sensitive = false;
			closeButton.Sensitive = true;
			cancelButton.Sensitive = true;
        }
		
		protected void OnCloseButtonClicked (object sender, System.EventArgs e)
		{
		}
	}
}

