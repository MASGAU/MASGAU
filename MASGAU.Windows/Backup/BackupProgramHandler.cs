using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASGAU.Location;
using MASGAU.Game;
using MASGAU.Location.Holders;

namespace MASGAU.Backup {
    public class BackupProgramHandler: ABackupProgramHandler<LocationsHandler> {

        public BackupProgramHandler() : base(Interface.WPF) { }

        public BackupProgramHandler(List<GameHandler> backup_list): base(backup_list,Interface.WPF) {}

        public BackupProgramHandler(GameHandler this_game, List<DetectedFile> only_these, string archive_name) :
            base(this_game,only_these,archive_name,Interface.WPF) { } 


    }
}
