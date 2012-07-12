using System.Collections.Generic;
using MASGAU.Location;
using MASGAU.Location.Holders;

namespace MASGAU.Backup {
    public class BackupProgramHandler : ABackupProgramHandler<LocationsHandler> {

        public BackupProgramHandler() : base(Interface.WPF) { }

        public BackupProgramHandler(List<GameVersion> backup_list) : base(backup_list, Interface.WPF) { }

        public BackupProgramHandler(GameVersion this_game, List<DetectedFile> only_these, string archive_name) :
            base(this_game, only_these, archive_name, Interface.WPF) { }


    }
}
