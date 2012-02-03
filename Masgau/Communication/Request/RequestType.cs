using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Communication.Request {
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
