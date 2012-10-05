using System.IO;

namespace MASGAU.Monitor {
    public class MonitorFile {
        public MonitorPath Path;

        public string root, path, old_path;

        public WatcherChangeTypes change_type;
        public Origin origin;
        public string full_path {
            get {
                return System.IO.Path.Combine(root, path);
            }
        }
        public MonitorFile(MonitorPath path) {
            this.Path = path;
        }

    }
}
