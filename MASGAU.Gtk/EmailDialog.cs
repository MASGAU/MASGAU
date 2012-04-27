using System;
using Translations;
namespace MASGAU
{
	public partial class EmailDialog : ADialog
	{
		public EmailDialog (Gtk.Window parent):base(parent)
		{
			this.Build ();

			GTKHelpers.translateWindow(this);
		}
		public string email {
            get {
                return emailTxt.Text;
            }
        }		
		
		protected void OnEmailTxtChanged (object sender, System.EventArgs e)
		{
            if(emailTxt.Text.Contains("@"))
                buttonSave.Sensitive = true;
            else
                buttonSave.Sensitive = false;
		}
	}
}

