using System;
using MASGAU.Communication;
using MASGAU.Communication.Message;
using MASGAU.Communication.Request;
using MASGAU.Communication.Progress;
using System.Threading;
namespace MASGAU
{
	public class ADialog: Gtk.Dialog, ICommunicationReceiver
	{
		
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
		
		public ADialog(Gtk.Window parent): this() {
			this.Parent = parent;
		}
		private ADialog ()
		{
			this.Modal = true;
			_context = SynchronizationContext.Current;
			CommunicationHandler.addReceiver(this);
			this.SetIconFromFile("masgau.ico");
			//this.DeleteEvent += OnDeleteEvent;
			this.Response += OnResponse;
		}

		public void sendMessage (MessageEventArgs e)
		{
			GTKHelpers.sendMessage(e);
		}
		public void requestInformation (RequestEventArgs e)
		{
			
		}
		
		protected void OnResponse (object o, Gtk.ResponseArgs args)
		{
			this.Destroy();
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
		
		
	}
}

