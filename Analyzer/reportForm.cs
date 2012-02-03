using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;

namespace MASGAU
{
	public partial class reportForm : Form
	{
		private string report, name;
		public reportForm(string new_report, string new_name)
		{
			InitializeComponent();
			report = new_report;
            name = new_name;
		}

		private void reportForm_Shown(object sender, EventArgs e)
		{
			reportText.Text = report;
            if(checkConnection()) {
                uploadButton.Enabled = true;
                uploadButton.Text = "E-Mail Report";
            } else {
                uploadButton.Enabled = false;
                uploadButton.Text = "E-Mail Server Blocked";
            }
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			if(saveFileDialog1.ShowDialog(this)!=DialogResult.Cancel) {
				try {
					StreamWriter writer = File.CreateText(saveFileDialog1.FileName);
                    if(commentsTxt.Text!="") {
                        writer.Write("Comments:" + Environment.NewLine + commentsTxt.Text + Environment.NewLine + Environment.NewLine);
                    }
					writer.Write(report);
					writer.Close();
				} catch {
					MessageBox.Show(this,"Eror while trying to write " + saveFileDialog1.FileName,"Pick somewhere else");
				}
			}
		}



        private void uploadButton_Click(object sender, EventArgs e)
        {
            emailSettings email = new emailSettings(this);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("masgausubmissions@gmail.com", "MASGAU Submission");

            if(email.email==null||email.email=="") {
                emailForm get_email = new emailForm();
                if(get_email.ShowDialog(this)==DialogResult.OK) {
                    email.email = get_email.emailText.Text;
                    email.writeConfig();
                    mail.ReplyToList.Add(new MailAddress(email.email));
                } else {
                    return;
                }
            } else {
                mail.ReplyToList.Add(new MailAddress(email.email));
            }


            mail.To.Add("sanmadjack@users.sourceforge.net");
            mail.Subject = "MASGAU - " + name;
            mail.Body = "Here you go, oh master:" + Environment.NewLine + Environment.NewLine;
            if(commentsTxt.Text!="") {
                mail.Body += "Comments:" + Environment.NewLine + commentsTxt.Text + Environment.NewLine + Environment.NewLine;
            }
            mail.Body += report;
                
            //AlternateView planview = AlternateView.CreateAlternateViewFromString("This is my plain text content, viewable tby those clients that don't support html");
            //AlternateView htmlview = AlternateView.CreateAlternateViewFromString("<b>This is bold text and viewable by those mail clients that support html<b>");
            // mail.AlternateViews.Add(planview);
            //  mail.AlternateViews.Add(htmlview);

            mail.IsBodyHtml = false;
            mail.Priority = MailPriority.High;
            mail.Headers.Add("Disposition-Notification-To", "<sanmadjack@gmail.com>");
            // mail.Attachments.Add(Server.MapPath("/"));
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential("masgausubmissions@gmail.com", "0WCM;i$N"),
                EnableSsl = true
            };
            try
            {
                uploadButton.Enabled = false;
                uploadButton.Text = "Sending E-Mail...";
                smtp.Send(mail);
                uploadButton.Text = "E-Mail Sent";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,ex.ToString(),"Error time",MessageBoxButtons.OK,MessageBoxIcon.Error);
                uploadButton.Text = "E-Mail Not Working";
            }
        }
        private bool checkConnection() {
            try
            {
            System.Net.Sockets.TcpClient clnt=new System.Net.Sockets.TcpClient("smtp.gmail.com",587);
            clnt.Close();
            return true;
            }
            catch
            {
            return false;
            }
        }
	}
}