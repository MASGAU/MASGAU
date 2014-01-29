using System.Collections.Generic;
using MVC;
using MVC.Communication;

namespace MASGAU.GTK {
	public partial class MainWindow {
		private bool disabled;
		List<ICancellable> cancellables = new List<ICancellable>();

		public override void disableInterface() {
			setInterfaceEnabledness(false);

			btnCancel.Sensitive = false;
			ProgressHandler.saveMessage();
		}

		public void disableInterface(ICancellable cancellable_item) {
			cancellables.Add(cancellable_item);
			cancellable_item.Completed += new System.ComponentModel.RunWorkerCompletedEventHandler(cancellable_item_RunWorkerCompleted);
			Translator.GTK.TranslationHelpers.translate(btnCancel, "Stop");
			setInterfaceEnabledness(false);
			ProgressHandler.saveMessage();
		}

		void cancellable_item_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
			ICancellable worker = (ICancellable)sender;
			worker.Completed -= new System.ComponentModel.RunWorkerCompletedEventHandler(cancellable_item_RunWorkerCompleted);
			cancellables.Remove(worker);
		}

		public override void enableInterface() {
			setInterfaceEnabledness(true);
			ProgressHandler.restoreMessage();

			setStatusBarText(ProgressHandler.message);
		}


		private void setInterfaceEnabledness(bool status) {
			disabled = !status;
			btnCancel.Sensitive = !status;
		}

		private void cancelWorkers() {
			btnCancel.Sensitive = false;
			Translator.GTK.TranslationHelpers.translate(btnCancel, "Stopping");
			foreach (ICancellable worker in cancellables) {
				worker.Cancel();
			}
		}	
	}
}

