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
using Translator;
namespace MVC.GTK {
	public abstract class AViewWindow: Gtk.Window, ICommunicationReceiver, ITranslateableWindow  {

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
			GLib.ExceptionManager.UnhandledException +=	handleOtherExceptions;
			CommunicationHandler.addReceiver(this);
		}
		//void Window_Closing(object sender, CancelEventArgs e) {
		//	_available = false;
		//	if (this.Visibility == System.Windows.Visibility.Visible)
		//		toggleVisibility();
			
		public void setTranslatedTitle(string name, params string[] variables) {
			this.Title = Strings.GetLabelString(name, variables);
		}

		protected void handleOtherExceptions(GLib.UnhandledExceptionArgs e) {
			
			CommunicationDialog dialog = new CommunicationDialog(this,e.ExceptionObject as Exception);
			dialog.Run();
			dialog.Hide();
			dialog.Dispose();
			
		}
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
			CommunicationDialog dialog = new CommunicationDialog(this,e);
			dialog.Run();
			dialog.Hide();
			dialog.Dispose();
		}

		public abstract void updateProgress(ProgressUpdatedEventArgs e);


		public virtual void disableInterface () {
			this.Sensitive = false;
		}
		public virtual void enableInterface () {
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


		#region MessageBox showing things
		public bool displayQuestion(RequestEventArgs e) {
			MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, e.message);
			Gtk.ResponseType response = (Gtk.ResponseType)md.Run ();
			if (response== Gtk.ResponseType.Yes) {
				e.result.SelectedOption = "Yes";
				e.result.SelectedIndex = 1;
				e.response = MVC.Communication.ResponseType.OK;
				return true;
			} else {
				e.response = MVC.Communication.ResponseType.Cancel;
				return false;
			}
			//e.result.Suppressed = box.Suppressed;
		}
		public void displayError(string title, string message) {
			displayError(title, message, null);
		}
		public void displayError(string title, string message, Exception e) {
			displayMessage(title, message, MessageTypes.Error, e, false);
		}

		public MVC.Communication.ResponseType displayWarning(string title, string message, bool suppressable) {
			//return displayMessage(title, message, MessageTypes.Warning, null, suppressable);
			MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Warning, ButtonsType.OkCancel, message);
			Gtk.ResponseType response = (Gtk.ResponseType)md.Run ();
			if (response == Gtk.ResponseType.Ok) {
				return MVC.Communication.ResponseType.OK;
			} else {
				return MVC.Communication.ResponseType.Cancel;
			}
		}

		public void displayInfo(string title, string message) {
			MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Question, ButtonsType.Ok, message);
			Gtk.ResponseType response = (Gtk.ResponseType)md.Run ();
		}
		private MVC.Communication.ResponseType displayMessage(string title, string message, MessageTypes type, Exception e, bool suppressable) {
			switch (type) {
			case MessageTypes.Info:
				this.displayInfo (title, message);
				return MVC.Communication.ResponseType.OK;
			case MessageTypes.Warning:
				return this.displayWarning (title, message, false);
			default:
				this.displayInfo (title, message);
				return MVC.Communication.ResponseType.OK;
			}

		}
		#endregion


		#region TranslatedMessageBoxes
		public bool askTranslatedQuestion(String string_name, bool suppressable, params string[] variables) {
			StringCollection mes = Strings.getStrings(string_name);
			string title, message;

			if (mes.ContainsKey(StringType.Title))
				title = mes[StringType.Title].interpret(variables);
			else
				title = string_name;

			if (mes.ContainsKey(StringType.Message))
				message = mes[StringType.Message].interpret(variables);
			else
				message = string_name;

			RequestEventArgs e = new RequestEventArgs(RequestType.Question, title, message, null, null, new RequestReply(), suppressable);
			return displayQuestion(e);
		}


		public MVC.Communication.ResponseType showTranslatedWarning(String string_name, params string[] variables) {
			StringCollection mes = Strings.getStrings(string_name);
			return displayWarning(mes[StringType.Title].interpret(variables),
				mes[StringType.Message].interpret(variables), false);
		}
		public void showTranslatedError(String string_name, params string[] variables) {
			showTranslatedError(string_name, null, variables);
		}
		public void showTranslatedError(String string_name, Exception ex, params string[] variables) {
			StringCollection mes = Strings.getStrings(string_name);
			string title, message;

			if (mes.ContainsKey(StringType.Title))
				title = mes[StringType.Title].interpret(variables);
			else
				title = string_name;

			if (mes.ContainsKey(StringType.Message))
				message = mes[StringType.Message].interpret(variables);
			else
				message = string_name;

			displayError(title,
				message, ex);
		}
		//public static bool showTranslatedInfo(ITranslateableWindow window, String string_name, params string[] variables) {
		//    StringCollection mes = Strings.getStrings(string_name);
		//    return displayInfo(mes[StringType.Title].interpret(variables),
		//        mes[StringType.Message].interpret(variables));
		//}
		#endregion



	}
}

