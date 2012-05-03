using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication.Request {
    public enum RequestType {
        Question,
        Choice,
        Folder,
        File,
        Files,
        BackupFolder,
        SyncFolder
    }
}
