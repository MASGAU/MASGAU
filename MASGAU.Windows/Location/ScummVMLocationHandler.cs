﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MASGAU.Location.Holders;
namespace MASGAU.Location
{
    public class ScummVMLocationHandler: AScummVMLocationHandler
    {

        protected override Dictionary<string, FileInfo> collectConfigFiles()
        {
            Dictionary<string, FileInfo> files = new Dictionary<string,FileInfo>();
            ASystemLocationHandler handler = Core.locations.getHandler(HandlerType.System) as ASystemLocationHandler;

            foreach(UserData user in handler) {
                if(user.hasFolderFor(EnvironmentVariable.AppData)) {
                    FileInfo file = new FileInfo(
                        Path.Combine(user.getFolder(EnvironmentVariable.AppData),
                        Path.Combine("ScummVM","scummvm.ini")));
                    if(file.Exists) {
                        files.Add(user.name,file);
                    }
                }
            }
            return files;
        }

        protected override string findInstallPath()
        {
            Registry.RegistryHandler reg = new Registry.RegistryHandler( Registry.RegRoot.local_machine,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ScummVM_is1",false);
            if (reg.key_found && reg.hasValue("InstallLocation")&&
                File.Exists(Path.Combine(reg.getValue("InstallLocation"),"scummvm.exe")))
            {
                return reg.getValue("InstallLocation");
            } else {
                string file_path = Path.Combine("ScummVM","scummvm.exe");
                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), file_path)))
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ScummVM");
                }
                else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), file_path)))
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ScummVM");
                }
            }
            return null;
        }

        protected override List<DetectedLocationPathHolder> getPaths(ScummVMName get_me)
        {
            List <DetectedLocationPathHolder>  locs = base.getPaths(get_me);

            if (install_path != null)
            {
                LocationPathHolder loc = SystemLocationHandler.translateToVirtualStore(install_path);
                List<DetectedLocationPathHolder> vlocs = Core.locations.getPaths(loc);

                for (int i = 0; i < vlocs.Count; i++)
                {

                    if (!filterLocation(vlocs[i], get_me, vlocs[i].owner))
                    {
                        locs.RemoveAt(i);
                        i--;
                    }

                }


                locs.AddRange(locs);
            }
            


            return locs;
        }


    }
}
