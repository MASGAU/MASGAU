using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MASGAU.Monitor
{
    class MonitorFile
    {
        public string root, path, old_path;
        public WatcherChangeTypes change_type;
        public Origin origin;
        public string full_path {
            get {
                return Path.Combine(root,path);
            }
        }
    }
}
