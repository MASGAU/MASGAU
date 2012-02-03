using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MASGAU.Communication.Message;
using MASGAU.Communication.Request;
using MASGAU.Communication;

namespace MASGAU
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : AWindow
    {
        //public MessageBox(string title, string message, MessageTypes type):this(title,message,null,type) {
        //}

        private string recurseExceptions(Exception e) {
            StringBuilder return_me = new StringBuilder(e.Message);
            return_me.AppendLine();
            return_me.AppendLine();
            
            //var frames = new System.Diagnostics.StackTrace(e).GetFrames();
            //for (int i = 1; i < frames.Length; i++) /* Ignore current StackTraceToString method...*/
            //{
            //    var currFrame = frames[i];
            //    var method = currFrame.GetMethod();
            //    return_me.Append(string.Format("{0}:{1}\n",                    
            //        method.ReflectedType != null ? method.ReflectedType.Name : string.Empty,
            //        method.Name));
            //}


            return_me.AppendLine(e.StackTrace);
            if (e.InnerException != null) {
                return_me.AppendLine(recurseExceptions(e.InnerException));
                return_me.AppendLine();
            }
            return return_me.ToString(); ;
        }
        public MessageBox(string title, string message, AWindow owner): base(owner) {
            InitializeComponent();
            this.Title = title;
            messageLabel.Content = message;
        }
        public MessageBox(string title, string message, RequestType type, AWindow owner): this(title,message,owner) {
            if(type== RequestType.Question) {
                cancelButton.Visibility = System.Windows.Visibility.Visible;
                submitButton.Visibility = System.Windows.Visibility.Collapsed;
                okButton.Content = "Yes";
                cancelButton.Content = "No";
                questionIcon.Visibility =  System.Windows.Visibility.Visible;
                exceptionExpander.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                this.DialogResult = false;
                throw new NotImplementedException();
            }
        }


        public MessageBox(string title, string message, Exception e, MessageTypes type, AWindow owner): this(title, message, owner) {
            switch(type) {
                case MessageTypes.Error:
                    cancelButton.Visibility = System.Windows.Visibility.Collapsed;
                    if(e!=null) {
                        exceptionExpander.Visibility = System.Windows.Visibility.Visible;
                        exceptionText.Text = recurseExceptions(e);
                        if(e.GetType()==typeof(MException)) {
                            if(((MException)e).submittable)
                                submitButton.Visibility = System.Windows.Visibility.Visible;
                            else
                                submitButton.Visibility = System.Windows.Visibility.Collapsed;
                        } else {
                            submitButton.Visibility = System.Windows.Visibility.Visible;
                        }
                    } else {
                        submitButton.Visibility = System.Windows.Visibility.Collapsed;
                        exceptionExpander.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    okButton.Content = "Close";
                    errorIcon.Visibility =  System.Windows.Visibility.Visible;
                    break;
                case MessageTypes.Info:
                    cancelButton.Visibility = System.Windows.Visibility.Collapsed;
                    exceptionExpander.Visibility = System.Windows.Visibility.Collapsed;
                    submitButton.Visibility = System.Windows.Visibility.Collapsed;
                    okButton.Content = "OK";
                    infoIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageTypes.Warning:
                    cancelButton.Visibility = System.Windows.Visibility.Collapsed;
                    exceptionExpander.Visibility = System.Windows.Visibility.Collapsed;
                    submitButton.Visibility = System.Windows.Visibility.Collapsed;
                    okButton.Content = "OK";
                    warningIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult  =true;
        }
        private EmailHandler email;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(submitButton.Visibility== System.Windows.Visibility.Visible) {
                email = new EmailHandler();
                email.checkAvailability(checkAvailabilityDone);
                submitButton.Content = "Checking Connection...";
            }
        }

        private void checkAvailabilityDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(email.email_available) {
                submitButton.IsEnabled = true;
                submitButton.Content = "Send Report";
            } else {
                submitButton.IsEnabled = false;
                submitButton.Content = "Can't Send Report";
            }

        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
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
            body.AppendLine(this.Title);
            body.AppendLine();
            body.AppendLine(messageLabel.Content.ToString());
            body.AppendLine();
            body.AppendLine(exceptionText.Text);
            body.AppendLine();

            submitButton.IsEnabled = false;
            submitButton.Content = "Sending Report...";
            email.sendEmail("masgausubmissions@gmail.com","masgausubmissions@gmail.com",Core.settings.email,"MASGAU Error - " + this.Title, body.ToString(), sendEmailDone);
        }

        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Error!=null) {
                submitButton.Content = "Send Failed";
                showError("Error time",e.Error.Message);
            } else {
                submitButton.Content = "Report Sent";
            }
            submitButton.IsEnabled = false;
        }
    }
}
