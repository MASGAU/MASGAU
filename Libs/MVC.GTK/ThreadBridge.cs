using System;
using System.Threading;
namespace MVC.GTK {
	public class ThreadBridge: IThreadBridge {


		public ThreadBridge () {


		}

		public void Send (CommunicationDelegate send_me) {
			ManualResetEvent reset = new ManualResetEvent(false);
			Gtk.Application.Invoke(delegate {
				send_me();
				reset.Set ();
			});

			reset.WaitOne();
		}
		public void Post (CommunicationDelegate send_me) {
			Gtk.Application.Invoke(delegate{
				send_me();
			});
		}
	}
}

