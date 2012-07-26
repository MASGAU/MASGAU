using System;
using System.Text;
using System.Windows;
using Email;
using Email.WPF;
using Translator;
using Translator.WPF;
using MVC.Communication;

namespace MVC.WPF {
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : ACommunicationWindow {
        private RespondableEventArg e;

        public MessageBox(string title, string message, bool suppressable, ACommunicationWindow owner, IEmailSource email_source)
            : base(owner,email_source) {
                this.Icon = owner.Icon;
            InitializeComponent();

            if (email_source != null) {
                submitButton.From = email_source.EmailSender;
                submitButton.To = email_source.EmailRecipient;
            }

            TranslationHelpers.translateWindow(this);
            this.Title = title;
            messageLabel.Content = message;
            if (owner != null)
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            else
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if (suppressable)
                this.Suppress.Visibility = System.Windows.Visibility.Visible;


            StringBuilder body = new StringBuilder();
            body.AppendLine(this.Title);
            body.AppendLine();
            body.AppendLine(messageLabel.Content.ToString());
            body.AppendLine();
            body.AppendLine(exceptionText.Text);
            body.AppendLine();
            body.AppendLine(Application.Current.Properties.ToString());
            body.AppendLine();
            body.AppendLine();

            submitButton.Message = body.ToString();
            submitButton.Subject = "MASGAU Error - " + this.Title;
        }

        public MessageBox(RequestEventArgs e, ACommunicationWindow owner, IEmailSource email_source)
            : this(e.title, e.message, e.suppressable, owner, email_source) {
                this.e = e;
            switch(e.info_type) {
                case RequestType.Question:
                    yesButton.Visibility = System.Windows.Visibility.Visible;
                    noButton.Visibility = System.Windows.Visibility.Visible;
                    questionIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    this.DialogResult = false;
                    throw new NotImplementedException();
            }
        }

        public MessageBox(MessageEventArgs e, ACommunicationWindow owner, IEmailSource email_source)
            : this(e.type, e.title, e.message, e.exception, false, owner, email_source) {
                this.e = e;
        }
        public MessageBox(MessageTypes type, string title, string message, ACommunicationWindow owner, IEmailSource email_source)
            : this(type, title, message, null, false, owner, email_source) {
        }


        public MessageBox(MessageTypes type, string title, string message, Exception e, bool suppressable, ACommunicationWindow owner, IEmailSource email_source)
            : this(title, message, suppressable, owner, email_source) {
            switch (type) {
                case MessageTypes.Error:
                    if (e != null) {
                        exceptionExpander.Visibility = System.Windows.Visibility.Visible;
                        exceptionText.Text = recurseExceptions(e);
                        if (e.GetType() == typeof(CommunicatableException)) {
                            submitButton.Visibility = System.Windows.Visibility.Visible;
                        } else {
                            submitButton.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                    closeButton.Visibility = System.Windows.Visibility.Visible;
                    errorIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageTypes.Info:
                    okButton.Visibility = System.Windows.Visibility.Visible;
                    infoIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageTypes.Warning:
                    okButton.Visibility = System.Windows.Visibility.Visible;
                    warningIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        public static string recurseExceptions(Exception e) {
            StringBuilder return_me = new StringBuilder(e.Message);
            return_me.AppendLine();
            return_me.AppendLine();

            return_me.AppendLine(e.StackTrace);
            if (e.InnerException != null) {
                return_me.AppendLine(recurseExceptions(e.InnerException));
                return_me.AppendLine();
            }
            return return_me.ToString();
            ;
        }

        public bool Suppressable {
            get {
                return this.Suppress.Visibility == System.Windows.Visibility.Visible;
            }
            set {
                if (value)
                    this.Suppress.Visibility = System.Windows.Visibility.Visible;
                else
                    this.Suppress.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        public bool Suppressed {
            get{
                return this.Suppress.IsChecked==true;
            }
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

        

    }
}
