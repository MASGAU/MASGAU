using System.IO;
using MASGAU.Registry;

namespace MASGAU.Location {
    public class SteamLocationHandler : ASteamLocationHandler {
        // Custom Methods
        public SteamLocationHandler()
            : base() {
        }

        private string isSteamPath(string check_me) {
            if (Directory.Exists(check_me)) {
                if (File.Exists(Path.Combine(check_me, "Steam.exe"))) {
                    return check_me;
                }
            }
            return null;
        }

        protected override void resetSteamPath() {
            string reg_path = null, set_path = null;
            RegistryHandler steam = new RegistryHandler("local_machine", "SOFTWARE\\Valve\\Steam", false);
            reg_path = isSteamPath(steam.getValue("InstallPath"));
            set_path = isSteamPath(Core.settings.steam_override);

            if (set_path != null)
                path = set_path;
            else if (reg_path != null)
                path = reg_path;
            else
                path = null;

            if (path != null) {
                DirectoryInfo read_me = new DirectoryInfo(Path.Combine(path, "steamapps"));
                DirectoryInfo[] read_us;
                if (read_me.Exists) {
                    read_us = read_me.GetDirectories();
                    foreach (DirectoryInfo subDir in read_us) {
                        if (subDir.Name.ToLower() != "common" && subDir.Name.ToLower() != "sourcemods" && subDir.Name.ToLower() != "media") {
                            setUserEv(subDir.Name, EnvironmentVariable.SteamUser, subDir.FullName);
                        }
                    }
                    steam_apps_path = read_me.FullName;
                    DirectoryInfo common_folder = new DirectoryInfo(Path.Combine(steam_apps_path, "common"));
                    if (common_folder.Exists)
                        global.setEvFolder(EnvironmentVariable.SteamCommon, common_folder.FullName);

                    DirectoryInfo source_mods = new DirectoryInfo(Path.Combine(steam_apps_path, "SourceMods"));
                    if (source_mods.Exists)
                        global.setEvFolder(EnvironmentVariable.SteamSourceMods, source_mods.FullName);
                }

                read_me = new DirectoryInfo(Path.Combine(path, "userdata"));
                if (read_me.Exists) {
                    read_us = read_me.GetDirectories();
                    foreach (DirectoryInfo subDir in read_us) {
                        setUserEv(subDir.Name, EnvironmentVariable.SteamUserData, subDir.FullName);
                    }
                    userdata_path = read_me.FullName;
                }

            } else {
                userdata_path = null;
                steam_apps_path = null;
            }
        }
    }
}