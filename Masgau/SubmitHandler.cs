using System;
using System.ComponentModel;
using System.Net.Mail;

namespace MASGAU {
    public class SubmitHandler : BackgroundWorker {
        public bool available = false;
        public const string server = "masgau.org";
        public const string email_password = "0WCM;i$N";

        public SubmitHandler() {

        }

        public void checkAvailability(RunWorkerCompletedEventHandler target) {
            this.DoWork += new System.ComponentModel.DoWorkEventHandler(checkAvailability);
            RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(target);
            this.RunWorkerAsync();
        }

        public bool email_available = false;
        private void checkAvailability(object sender, System.ComponentModel.DoWorkEventArgs e) {
            try {
                System.Net.Sockets.TcpClient clnt = new System.Net.Sockets.TcpClient(server, 80);
                clnt.Close();
                email_available = true;
            } catch {
                email_available = false;
            }
        }

        private string from, to, reply_to, body, title;
        public void sendEmail(string new_from, string new_to, string new_reply_to, string new_title, string new_body, RunWorkerCompletedEventHandler target) {
            this.DoWork += new System.ComponentModel.DoWorkEventHandler(sendEmail);
            RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(target);
            from = new_from;
            to = new_to;
            reply_to = new_reply_to;
            body = new_body;
            title = new_title;
            this.RunWorkerAsync();

        }

        private void sendEmail(object sender, System.ComponentModel.DoWorkEventArgs e) {
            MailMessage mail = new MailMessage();
            //mail.From = new MailAddress(email_sender, "MASGAU Submission");

            mail.To.Add(to);
            mail.Subject = title;
            mail.Body = body + Environment.NewLine + Environment.NewLine + "Sent from version " + Core.version;
            mail.ReplyToList.Add(new MailAddress(Core.settings.EmailSender));

            //AlternateView planview = AlternateView.CreateAlternateViewFromString("This is my plain text content, viewable tby those clients that don't support html");
            //AlternateView htmlview = AlternateView.CreateAlternateViewFromString("<b>This is bold text and viewable by those mail clients that support html<b>");
            // mail.AlternateViews.Add(planview);
            //  mail.AlternateViews.Add(htmlview);

            mail.IsBodyHtml = false;
            mail.Priority = MailPriority.High;
            mail.Headers.Add("Disposition-Notification-To", "<" + Core.submission_email + ">");
            // mail.Attachments.Add(Server.MapPath("/"));
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587) {
                //Credentials = new System.Net.NetworkCredential(email_sender, email_password),
                EnableSsl = true
            };
            smtp.Send(mail);
        }
    }
}
