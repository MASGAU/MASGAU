using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MASGAU.Location {
    public class PlaystationLocationHandler: APlaystationLocationHandler  {
        public PlaystationLocationHandler(): base() {
            foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
                if(!look_here.IsReady||look_here.DriveType!=DriveType.Removable||
                    !(look_here.DriveFormat=="FAT32"||look_here.DriveFormat=="FAT16"||look_here.DriveFormat=="FAT"))
                    continue;

                setUserEv(look_here.Name,EnvironmentVariable.PSPSave,Path.Combine(look_here.Name,"PSP\\SAVEDATA"));
                setUserEv(look_here.Name,EnvironmentVariable.PS3Export,Path.Combine(look_here.Name,"PS3\\EXPORT\\PSV"));
                setUserEv(look_here.Name,EnvironmentVariable.PS3Save,Path.Combine(look_here.Name,"PS3\\SAVEDATA"));
            }
        }
    }
}