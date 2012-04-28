using System;
using System.ComponentModel;
using MASGAU.Location;
using System.Collections.ObjectModel;
using Gtk;
using System.Threading;
using MASGAU.Communication;
using MASGAU.Communication.Message;
using MASGAU.Communication.Request;
using MASGAU.Communication.Progress;
using Translations;

namespace MASGAU.Gtk
{
	public abstract class AWindow: global::Gtk.Window, ICommunicationReceiver
	{
		public AWindow(WindowType window_type): base(window_type)
		{
			_context = SynchronizationContext.Current;
			CommunicationHandler.addReceiver(this);
			this.WindowPosition = WindowPosition.CenterOnParent;
			this.SetIconFromFile("masgau.ico");
			this.DeleteEvent += OnDeleteEvent;
			this.WindowPosition = WindowPosition.Center;
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
			GTKHelpers.sendMessage(e);
		}
		public void requestInformation (RequestEventArgs e)
		{
			
		}
		
        #region Progress stuff
        public virtual void updateProgress(ProgressUpdatedEventArgs e) {
		}
        #endregion
		
		#region Communication Handler event handlers
        public virtual void disableInterface() {
			this.Sensitive = false;
        }
        public virtual void enableInterface() {
			this.Sensitive = true;
		}
        protected virtual void enableInterface(object sender, RunWorkerCompletedEventArgs e) {
			enableInterface();
        }

        public virtual void hideInterface() {
			this.Hide();
		}
        public virtual void showInterface() {
			this.Show();
		}
        public virtual void closeInterface() {
			throw new NotImplementedException();
		}
        #endregion
		
		#region default event handlers
		protected virtual void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
		#endregion
		
	}
}

