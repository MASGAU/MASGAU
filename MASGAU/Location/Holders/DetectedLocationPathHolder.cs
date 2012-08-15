using System;
using System.IO;
using GameSaveInfo;
namespace MASGAU.Location.Holders {
    // This holds locations that have been found
    public class DetectedLocationPathHolder : LocationPath {

        public DetectedLocationPathHolder(LocationPath path): base(path) {
            this.language = path.language;
        }

        public DetectedLocationPathHolder(EnvironmentVariable ev, string absolute_root, string path)
            : base(ev, path) {
                this.AbsoluteRoot = absolute_root;
        }

        protected DetectedLocationPathHolder() {
        }

        public QuickHash RootHash {
            get {
                if (AbsoluteRoot == null)
                    return null;
                return new QuickHash(AbsoluteRoot);
            }
        }

        public bool MatchesOriginalPath = false;

        // Holds the actual, interpreted root location
        public string AbsoluteRoot;
        // Holds the associated user for this folder
        public string owner;


        // Gets the full absolute path of the folfer
        public string full_dir_path {
            get {
                if (AbsoluteRoot != null && AbsoluteRoot != "") {
                    if (Path == null || Path == "") {
                        return AbsoluteRoot;
                    } else {
                        return System.IO.Path.Combine(AbsoluteRoot, Path);
                    }
                } else {
                    return null;
                }
            }
        }

        public bool Exists {
            get {
                return Directory.Exists(full_dir_path);
            }
        }

        public override string ToString() {
            string return_me;
            switch(EV) {
                case EnvironmentVariable.AllUsersProfile:
                case EnvironmentVariable.AltSavePaths:
                case EnvironmentVariable.Drive:
                case EnvironmentVariable.InstallLocation:
                case EnvironmentVariable.ProgramFiles:
                case EnvironmentVariable.ProgramFilesX86:
                case EnvironmentVariable.PS3Export:
                case EnvironmentVariable.PS3Save:
                case EnvironmentVariable.PSPSave:
                case EnvironmentVariable.Public:
                case EnvironmentVariable.SteamCommon:
                case EnvironmentVariable.SteamSourceMods:
                case EnvironmentVariable.CommonApplicationData:
                case EnvironmentVariable.None:
                case EnvironmentVariable.StartMenu:
                case EnvironmentVariable.FlashShared:
                    return_me = AbsoluteRoot;
                    break;
                case EnvironmentVariable.UbisoftSaveStorage:
                case EnvironmentVariable.Desktop:
                case EnvironmentVariable.AppData:
                case EnvironmentVariable.LocalAppData:
                case EnvironmentVariable.SavedGames:
                case EnvironmentVariable.SteamUser:
                case EnvironmentVariable.SteamUserData:
                case EnvironmentVariable.UserDocuments:
                case EnvironmentVariable.UserProfile:
                case EnvironmentVariable.VirtualStore:
                    return_me = EV.ToString();
                    break;
                default:
                    throw new NotSupportedException(EV.ToString());
            }   
            if (Path != null)
                return_me = System.IO.Path.Combine(return_me, Path);

            if (MatchesOriginalPath)
                return_me += " (This Archive Originally Came From This Folder)";

            return return_me;
        }

        public void delete() {
            try {
                DirectoryInfo info = new DirectoryInfo(full_dir_path);
                if (info.Exists) {
                    info.Attributes = FileAttributes.Normal;
                    info.Delete(true);
                }
            } catch (Exception e) {
                throw new Translator.TranslateableException("DeleteError", e, full_dir_path);
            }
        }
    }
}
