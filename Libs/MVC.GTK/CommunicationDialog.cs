using System;
using System.Text;
using MVC.Communication;
namespace MVC.GTK {
	public partial class CommunicationDialog : Gtk.Dialog {
		private MessageTypes MessageType = MessageTypes.None;
		private RequestType RequestType = RequestType.None;
		
		public new ResponseType Response { get; protected set; }

		
		public CommunicationDialog(AViewWindow parent, Exception e):
			this(parent,"Exception occured","Except it", MessageTypes.Error,e) {
		}
		
		public CommunicationDialog(AViewWindow parent, MessageEventArgs e):
			this(parent,e.title,e.message, e.type,e.exception) {
		}
		
		public CommunicationDialog(AViewWindow parent, RequestEventArgs e):
			this(parent, e.title,e.message) {
			questionImage.Visible = true;
			switch(e.info_type) {
			case RequestType.Question:
				buttonYes.Visible = true;
				buttonNo.Visible = true;
				break;
			default:
				throw new NotSupportedException(e.info_type.ToString());
			}
			
		}
		
		
		private CommunicationDialog(AViewWindow parent, string title, string message, MessageTypes mtype, Exception ex):
			this(parent,title,message) {
			this.MessageType = mtype;
			this.buttonOk.Visible = true;
			switch(mtype) {
				case MessageTypes.Error:
					errorImage.Visible = true;
					break;
			case MessageTypes.Info:
				infoImage.Visible = true;
				break;
			case MessageTypes.Warning:
				warningImage.Visible = true;
				break;
			default:
				throw new NotSupportedException(mtype.ToString());
			}
			
			loadException(ex);
		}
		private CommunicationDialog(AViewWindow parent, string title, string message) {
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
		
		private void loadException(Exception ex) {
			if(ex == null)
				return;
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
			exceptionExpander.Visible = true;
		}

		protected void OnButtonYesClicked (object sender, System.EventArgs e)
		{
			Response = ResponseType.Yes;
			this.Hide();
		}

		protected void OnButtonNoClicked (object sender, System.EventArgs e)
		{
			Response = ResponseType.No;
			this.Hide();
		}
	}
}

