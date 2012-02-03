using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class playstationHandler {
    public string psp_saves = null, ps3_saves = null, ps3_export = null;
    public playstationHandler() {
        foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
            if(look_here.IsReady&&look_here.DriveType==DriveType.Removable&&(look_here.DriveFormat=="FAT32"||look_here.DriveFormat=="FAT16")) {
                if(Directory.Exists(Path.Combine(look_here.Name,"PSP\\SAVEDATA")))
                    psp_saves = Path.Combine(look_here.Name,"PSP\\SAVEDATA");

                if(Directory.Exists(Path.Combine(look_here.Name,"PS3\\EXPORT\\PSV")))
                    ps3_export = Path.Combine(look_here.Name,"PS3\\EXPORT\\PSV");

                if(Directory.Exists(Path.Combine(look_here.Name, "PS3\\SAVEDATA")))
                    ps3_saves = Path.Combine(look_here.Name, "PS3\\SAVEDATA");
            }
        }
    }

    public string detectPSPGame(playstation_id id) {
        if(id.suffix==null||id.prefix==null)
            return null;
        else if(new DirectoryInfo(psp_saves).GetDirectories(id.prefix + id.suffix + "*").Length>0)
            return id.prefix + id.suffix;
        else
            return null;
    }
    public string detectPS2Game(playstation_id id) {
        if (id.suffix == null || id.prefix == null)
            return null;
        else if (new DirectoryInfo(ps3_export).GetFiles("BA" + id.prefix + "-" + id.suffix + "*").Length > 0)
            return id.prefix + "-" + id.suffix;
        else if(new DirectoryInfo(ps3_export).GetFiles("BA" + id.prefix + "P" + id.suffix + "*").Length>0)
            return id.prefix + "P" + id.suffix;
        else
            return null;
    }
    public string detectPS3Game(playstation_id id) {
        if (id.suffix == null || id.prefix == null)
            return null;
        else if (new DirectoryInfo(ps3_saves).GetDirectories(id.prefix + id.suffix + "*").Length > 0)
            return id.prefix + id.suffix;
        else
            return null;
    }

    public string detectPS1PSPGame(playstation_id id) {
        if (id.suffix == null || id.prefix == null)
            return null;
        else if (new DirectoryInfo(psp_saves).GetDirectories(id.prefix + id.suffix + "*").Length > 0)
            return id.prefix + id.suffix;
        else
            return null;
    }
    public string detectPS1PS3Game(playstation_id id) {
        if (id.suffix == null || id.prefix == null)
            return null;
        else if (new DirectoryInfo(ps3_export).GetFiles("BA" + id.prefix + "-" + id.suffix + "*").Length > 0)
            return id.prefix + "-" + id.suffix;
        else if(new DirectoryInfo(ps3_export).GetFiles("BA" + id.prefix + "P" + id.suffix + "*").Length>0)
            return id.prefix + "P" + id.suffix;
        else
            return null;
    }
}
