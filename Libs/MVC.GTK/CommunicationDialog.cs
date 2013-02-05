using System;
using System.Text;
using MVC.Communication;
namespace MVC.GTK {
	public partial class CommunicationDialog : Gtk.Dialog {
		public ResponseType Response { get; protected set; }

		
		
		public CommunicationDialog(AViewWindow parent, MessageEventArgs e):
			this(parent,e.title,e.message,e.exception) {
		}
		
		public CommunicationDialog (AViewWindow parent, string title, string message, Exception ex): this(parent,title, message) {
			StringBuilder ex_string = new StringBuilder();
			Exception exc = ex;
			while(exc != null) {
				ex_string.AppendLine(exc.Message);
				ex_string.AppendLine();
				ex_string.AppendLine(exc.StackTrace);
				ex_string.AppendLine();
				exc = exc.InnerException;
			}
			exceptionText1.Buffer.Text = ex_string.ToString();
		}
		
		
		public CommunicationDialog (AViewWindow parent, string title, string message) {
			this.Build ();
			this.Parent = parent;
			this.Title = title;
			messageLbl.Text = message;
		}

		protected void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			Response = ResponseType.OK;
			this.Hide();
		}

		protected void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			Response = ResponseType.Cancel;
			this.Hide();
		}
	}
}

