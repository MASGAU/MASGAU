using System.IO;
using GameSaveInfo;
using MASGAU.Registry;
using VDF;
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

        protected void loadSteamPaths(DirectoryInfo read_me) {
            DirectoryInfo[] read_us;
            if (read_me.Exists) {
                read_us = read_me.GetDirectories();
                foreach (DirectoryInfo subDir in read_us) {
                    if (subDir.Name.ToLower() != "common" && subDir.Name.ToLower() != "sourcemods" && subDir.Name.ToLower() != "media") {
                        addUserEv(subDir.Name, EnvironmentVariable.SteamUser, subDir.FullName, subDir.FullName);
                    }
                }
                steam_apps_path = read_me.FullName;
                DirectoryInfo common_folder = new DirectoryInfo(Path.Combine(steam_apps_path, "common"));
                if (common_folder.Exists)
                    global.addEvFolder(EnvironmentVariable.SteamCommon, common_folder.FullName, common_folder.FullName);

                DirectoryInfo source_mods = new DirectoryInfo(Path.Combine(steam_apps_path, "sourcemods"));
                if (source_mods.Exists)
                    global.addEvFolder(EnvironmentVariable.SteamSourceMods, common_folder.FullName, source_mods.FullName);
            }


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
                // Loads the steam isntall path folders
                loadSteamPaths(read_me);

                // Loads the alt steam library folders
                if (File.Exists(Path.Combine(path, "config", "config.vdf"))) {
                    config_file = new SteamConfigFile(Path.Combine(path, "config", "config.vdf"));
                    foreach (string folder in config_file.BaseInstallFolders) {
                        read_me = new DirectoryInfo(Path.Combine(folder, "SteamApps"));
                        loadSteamPaths(read_me);
                    }
                }



                // Loads the steam cloud folders
                read_me = new DirectoryInfo(Path.Combine(path, "userdata"));
                if (read_me.Exists) {
                    DirectoryInfo[] read_us;
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