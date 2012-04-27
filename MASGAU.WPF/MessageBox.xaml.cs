﻿using System;
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
using Translations;

namespace MASGAU
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : AWindow
    {

        public MessageBox(string title, string message, AWindow owner): base(owner) {
            InitializeComponent();
            WPFHelpers.translateWindow(this);
            this.Title = title;
            messageLabel.Content = message;
        }

        public MessageBox(string title, string message, RequestType type, AWindow owner): this(title,message,owner) {
            if(type== RequestType.Question) {
                cancelButton.Visibility = System.Windows.Visibility.Visible;
                submitButton.Visibility = System.Windows.Visibility.Collapsed;
                okButton.Content = Strings.get("YesButton");
                cancelButton.Content = Strings.get("NoButton");
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
                        exceptionText.Text = Core.recurseExceptions(e);
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
                    okButton.Content = Strings.get("CloseButton");
                    errorIcon.Visibility =  System.Windows.Visibility.Visible;
                    break;
                case MessageTypes.Info:
                    cancelButton.Visibility = System.Windows.Visibility.Collapsed;
                    exceptionExpander.Visibility = System.Windows.Visibility.Collapsed;
                    submitButton.Visibility = System.Windows.Visibility.Collapsed;
                    okButton.Content = Strings.get("OKButton");
                    infoIcon.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageTypes.Warning:
                    cancelButton.Visibility = System.Windows.Visibility.Collapsed;
                    exceptionExpander.Visibility = System.Windows.Visibility.Collapsed;
                    submitButton.Visibility = System.Windows.Visibility.Collapsed;
                    okButton.Content = Strings.get("OKButton");
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
                submitButton.Content = Strings.get("CheckingConnection");
            }
        }

        private void checkAvailabilityDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(email.email_available) {
                submitButton.IsEnabled = true;
                submitButton.Content = Strings.get("SendReport");
            } else {
                submitButton.IsEnabled = false;
                submitButton.Content = Strings.get("CantSendReport");
            }

        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WPFHelpers.checkEmail(this))
                return;

            StringBuilder body = new StringBuilder();
            body.AppendLine(this.Title);
            body.AppendLine();
            body.AppendLine(messageLabel.Content.ToString());
            body.AppendLine();
            body.AppendLine(exceptionText.Text);
            body.AppendLine();

            submitButton.IsEnabled = false;
            submitButton.Content = Strings.get("SendingReport");
            email.sendEmail("masgausubmissions@gmail.com",Core.submission_email,Core.settings.email,"MASGAU Error - " + this.Title, body.ToString(), sendEmailDone);
        }

        private void sendEmailDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Error!=null) {
                submitButton.Content = Strings.get("SendFailed");
                showError("Error time",e.Error.Message);
            } else {
                submitButton.Content = Strings.get("ReportSent");
            }
            submitButton.IsEnabled = false;
        }
    }
}
