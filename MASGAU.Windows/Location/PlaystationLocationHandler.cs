using System.IO;
using GameSaveInfo;
namespace MASGAU.Location {
    public class PlaystationLocationHandler : APlaystationLocationHandler {
        public PlaystationLocationHandler()
            : base() {
            foreach (string drive in GetDriveCandidates()) {
                setUserEv(drive, EnvironmentVariable.PSPSave, Path.Combine(drive, "PSP","SAVEDATA"));
                setUserEv(drive, EnvironmentVariable.PS3Export, Path.Combine(drive, "PS3","EXPORT","PSV"));
                setUserEv(drive, EnvironmentVariable.PS3Save, Path.Combine(drive, "PS3","SAVEDATA"));
            }
        }

        public override System.Collections.Generic.List<string> GetDriveCandidates() {
            System.Collections.Generic.List<string> return_me = new System.Collections.Generic.List<string>();
            foreach (DriveInfo look_here in DriveInfo.GetDrives()) {
                if (!look_here.IsReady || look_here.DriveType != DriveType.Removable ||
                    !(look_here.DriveFormat == "FAT32" || look_here.DriveFormat == "FAT16" || look_here.DriveFormat == "FAT"))
                    continue;

                return_me.Add(look_here.Name);
            }

            return return_me;
        }
    }
}