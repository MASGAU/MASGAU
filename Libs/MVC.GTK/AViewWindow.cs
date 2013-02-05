using System;
using System.Text;
using Gtk;
using MVC.Communication;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
//using System.Windows;
//using System.Windows.Input;
using System.Windows.Threading;
//using Translator;
namespace MVC.GTK {
	public abstract class AViewWindow: Gtk.Window, ICommunicationReceiver {

		private ThreadBridge _ThreadBridge = new MVC.GTK.ThreadBridge();
		public IThreadBridge ThreadBridge {
			get {
				return _ThreadBridge;
			}
		}

		protected bool _available = true;
		public bool Available {
			get {
				return _available;
			}
		}

		protected AViewWindow (Gtk.WindowType type): base(type) {
			//this.Closing += new CancelEventHandler(Window_Closing);
			//These intitialize the contexts of the CommunicationHandlers

			CommunicationHandler.addReceiver(this);
		}
		//void Window_Closing(object sender, CancelEventArgs e) {
		//	_available = false;
		//	if (this.Visibility == System.Windows.Visibility.Visible)
		//		toggleVisibility();
			
		//}

		public MVC.Communication.ResponseType sendMessage (MessageEventArgs e) {
				CommunicationDialog dialog = new CommunicationDialog(this,e);
			dialog.Run();
			dialog.Hide();
			dialog.Dispose();
			return dialog.Response;
		}
		
		public MVC.Communication.ResponseType sendMessage(string title, string message) {
			MessageDialog dialog  = new MessageDialog(this, DialogFlags.Modal,
			                                          MessageType.Info,ButtonsType.Ok,message);
			
			dialog.Run();
			dialog.Hide();
			dialog.Dispose();
			return MVC.Communication.ResponseType.OK;
		}

		public void requestInformation (RequestEventArgs e) {
			throw new NotSupportedException (e.ToString());
		}

		public abstract void updateProgress(ProgressUpdatedEventArgs e);


		public void disableInterface () {
			this.Sensitive = false;
		}
		public void enableInterface () {
			this.Sensitive = true;
		}

		public void hideInterface () {
			this.Visible = false;
		}
		public void showInterface () {
			this.Visible = true;
		}

		public void closeInterface () {
			this.Dispose ();
		}

		
		public bool isSameContext() {
			return Dispatcher.FromThread(Thread.CurrentThread) != null;
		}

	}
}

