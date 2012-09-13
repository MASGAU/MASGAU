namespace MASGAU.Main {
    public partial class MainWindowNew {

        protected void setupMonitorIcon() {
            Core.monitor.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(notifier.monitor_PropertyChanged);
        }

    }
}
