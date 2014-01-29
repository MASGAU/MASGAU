using System;

namespace MVC.GTK {
	public partial class MessageBox : Gtk.Dialog, IMessageBox {
		public MessageBox () {
			this.Build ();
		}
	}
}

