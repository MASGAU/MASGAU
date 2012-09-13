namespace MASGAU.Main {
    public partial class MainWindowNew {

        protected void setupMonitorIcon() {
            Common.Monitor.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(notifier.monitor_PropertyChanged);
        }

    }
}
