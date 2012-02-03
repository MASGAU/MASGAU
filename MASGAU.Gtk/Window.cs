using System;
using System.ComponentModel;
using MASGAU.Location;
using System.Collections.ObjectModel;
using Gtk;
using System.Threading;
using MASGAU.Communication;
using MASGAU.Communication.Message;
using MASGAU.Communication.Request;

namespace MASGAU
{
	public abstract class Window: Gtk.Window, ICommunicationReceiver
	{
		public Window(WindowType window_type): base(window_type)
		{
			_context = SynchronizationContext.Current;
			CommunicationHandler.addReceiver(this);
			this.SetIconFromFile("masgau.ico");
		}
		
		private SynchronizationContext _context;
		public SynchronizationContext context {
			get {
				return _context;
			}
		}
		
        protected bool _available = true;
        public bool available {
            get {
                return _available;
            }
        }
		
		public void sendMessage (MessageEventArgs e)
		{
			switch(e.type) {
			case MessageTypes.Error:
				//showError(e.title,e.message);
				break;
			case MessageTypes.Info:
				//showInfo(e.title,e.message);
				break;
			case MessageTypes.Warning:
				//showWarning(e.title,e.message);
				break;
			}
			e.response = ResponseType.OK;
		}
		public void requestInformation (RequestEventArgs e)
		{
			
		}
		public abstract void updateProgress (ProgressChangedEventArgs e);
		
		
        protected virtual void disableInterface() {
        }
        protected virtual void enableInterface(object sender, RunWorkerCompletedEventArgs e) {
        }

        #region Progress stuff
        protected void applyProgress(ProgressBar progress, ProgressChangedEventArgs e) {
			if(e.message!=null)
				progress.Text = e.message;
			progress.Sensitive = e.state== ProgressState.None;
			//progress.
            //progress.IsIndeterminate = e.state== ProgressState.Indeterminate;
            switch(e.state) {
                case ProgressState.Normal:
                    //progress.Foreground = default_progress_color;
                    break;
                case ProgressState.Error:
                    //progress.Foreground = Brushes.Red;
                    break;
                case ProgressState.Wait:
                    //progress.Foreground = Brushes.Yellow;
                    break;
            }

            progress.Visible = true;
            if(e.max==0)
                progress.Fraction = 0;
            else {
				progress.Fraction = (double)e.max/(double)e.value;
            }
        }
        #endregion

		
		#region MessageBox showing things
		protected bool showMessageDialog(string title, string message, MessageType type) {
			MessageDialog dialog;
			switch(type) {
			case MessageType.Error:
				dialog = new MessageDialog(this,DialogFlags.Modal,type,ButtonsType.Close,true,message);
				return (ResponseType) dialog.Run()==ResponseType.Cancel;
			case MessageType.Question:
				dialog = new MessageDialog(this,DialogFlags.Modal,type,ButtonsType.YesNo,true,message);
				return (ResponseType) dialog.Run()==ResponseType.Yes;
			default:
				dialog = new MessageDialog(this,DialogFlags.Modal,type,ButtonsType.Ok,true,message);
				return (ResponseType) dialog.Run()==ResponseType.OK;
			}
		}
        protected bool askQuestion(string title, string message) {
			return showMessageDialog(title,message,MessageType.Question);
        }
        protected bool showError(string title, string message) {
			return showMessageDialog(title,message,MessageType.Error);
        }
        protected bool showWarning(string title, string message) {
			return showMessageDialog(title,message,MessageType.Warning);
        }
        protected bool showInfo(string title, string message) {
			return showMessageDialog(title,message,MessageType.Info);
        }
        #endregion
		
		#region Communication Handler event handlers
        #endregion
		
	}
}

