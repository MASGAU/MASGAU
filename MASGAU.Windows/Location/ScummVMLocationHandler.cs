using System;
using System.Collections.Generic;
using System.IO;
using GameSaveInfo;
namespace MASGAU.Location {
    public class ScummVMLocationHandler : AScummVMLocationHandler {

        protected override Dictionary<string, FileInfo> collectConfigFiles() {
            Dictionary<string, FileInfo> files = new Dictionary<string, FileInfo>();
            ASystemLocationHandler handler = Core.locations.getHandler(HandlerType.System) as ASystemLocationHandler;

            foreach (UserData user in handler) {
                if (user.hasFolderFor(EnvironmentVariable.AppData)) {
                    FileInfo file = new FileInfo(
                        Path.Combine(user.getFolder(EnvironmentVariable.AppData).BaseFolder,
                        Path.Combine("ScummVM", "scummvm.ini")));
                    if (file.Exists) {
                        files.Add(user.name, file);
                    }
                }
            }
            return files;
        }

        protected override string findInstallPath() {
            Registry.RegistryHandler reg = new Registry.RegistryHandler("local_machine", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ScummVM_is1", false);
            if (reg.key_found && reg.hasValue("InstallLocation") &&
                File.Exists(Path.Combine(reg.getValue("InstallLocation"), "scummvm.exe"))) {
                return reg.getValue("InstallLocation");
            } else {
                string file_path = Path.Combine("ScummVM", "scummvm.exe");
                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), file_path))) {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ScummVM");
                } else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), file_path))) {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ScummVM");
                }
            }
            return null;
        }

        protected override DetectedLocations getPaths(ScummVM get_me) {
            DetectedLocations locs = base.getPaths(get_me);

            if (install_path != null) {
                LocationPath loc = SystemLocationHandler.translateToVirtualStore(install_path);
                DetectedLocations vlocs = Core.locations.getPaths(loc);
                List<string> keys = new List<string>(vlocs.Keys);
                foreach (string key in keys) {
                    if (!filterLocation(vlocs[key], get_me, vlocs[key].owner)) {
                        locs.Remove(key);
                    }

                }


                locs.AddRange(locs);
            }



            return locs;
        }


    }
}
