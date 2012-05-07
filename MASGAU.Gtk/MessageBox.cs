using System;
using System.Text;
using Translations;
using MASGAU.Communication.Message;
using MASGAU.Communication.Request;
using MASGAU.Communication;

namespace MASGAU.Gtk
{
	public partial class MessageBox : ADialog
	{
        private EmailHandler email;
        public MessageBox(string title, string message, AWindow owner): base(owner) {
			this.Build ();
			GTKHelpers.translateWindow(this);

            this.Title = title;
			messageLabel.Text = message;
			
            if(submitButton!=null) {
                email = new EmailHandler();
                email.checkAvailability(checkAvailabilityDone);
                submitButton.Label = Strings.get("CheckingConnection");
            }
        }

        public MessageBox(string title, string message, RequestType type, AWindow owner): this(title,message,owner) {
            if(type== RequestType.Question) {
				submitButton.Destroy();
				buttonOk.Label = Strings.get("Yes");
                buttonCancel.Label = Strings.get("No");
				
                //questionIcon.Visibility =  System.Windows.Visibility.Visible;
				
				exceptionExpander.Destroy();
            } else {
				this.Respond(global::Gtk.ResponseType.Cancel);
                throw new NotImplementedException();
            }
        }
		
        public MessageBox(string title, string message, Exception e, MessageTypes type, AWindow owner): this(title, message, owner) {
            switch(type) {
                case MessageTypes.Error:
					buttonCancel.Destroy();
                    if(e!=null) {
                        exceptionText.Buffer.Text = Core.recurseExceptions(e);
                        if(e.GetType()==typeof(MException)) {
                            if(!((MException)e).submittable)
                            	submitButton.Destroy();    
                        }
                    } else {
						submitButton.Destroy();
						exceptionExpander.Destroy();
                    }
                    buttonOk.Label = Strings.get("Close");
                    //errorIcon.Visibility =  System.Windows.Visibility.Visible;
                    break;
                case MessageTypes.Info:
					buttonCancel.Dispose();
					exceptionExpander.Dispose();
					submitButton.Dispose();
                    buttonOk.Label = Strings.get("OK");
                    //infoIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageTypes.Warning:
					buttonCancel.Dispose();
					exceptionExpander.Dispose();
					submitButton.Dispose();
				
                    buttonOk.Label = Strings.get("OK");
                    //warningIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
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
			if(!GTKHelpers.checkEmail(this))
				return;

            StringBuilder body = new StringBuilder();
            body.AppendLine(this.Title);
            body.AppendLine();
            body.AppendLine(messageLabel.Text);
            body.AppendLine();
            body.AppendLine(exceptionText.Buffer.ToString());
            body.AppendLine();

            submitButton.Sensitive = false;
            submitButton.Label = Strings.get("SendingReport");
            email.sendEmail("masgausubmissions@gmail.com","masgausubmissions@gmail.com",Core.settings.email,"MASGAU Error - " + this.Title, body.ToString(), sendEmailDone);
		}
		
        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Error!=null) {
                submitButton.Label = Strings.get("SendFailed");
                GTKHelpers.showError(this,"Error time",e.Error.Message);
            } else {
                submitButton.Label = Strings.get("ReportSent");
            }
            submitButton.Sensitive = false;
        }
		
	}
}

