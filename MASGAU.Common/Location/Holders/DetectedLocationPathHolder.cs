using System;
using System.IO;
using GameSaveInfo;
namespace MASGAU.Location.Holders {
    // This holds locations that have been found
    public class DetectedLocationPathHolder : LocationPath {

        public DetectedLocationPathHolder(LocationPath path, string absolute_root, string owner)
            : base(path) {
            this.language = path.language;
            this.AbsoluteRoot = absolute_root;
            this.owner = owner;
        }


        public DetectedLocationPathHolder(EnvironmentVariable ev, string absolute_root, string path, string owner)
            : base(ev, path) {
            if (absolute_root == null)
                throw new Exception("ABSOLUTE ROOT MUST BE PROVIDED");
            this.AbsoluteRoot = absolute_root;
            this.owner = owner;
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
        public string AbsoluteRoot { get; protected set; }
        // Holds the associated user for this folder
        public string owner;


        // Gets the full absolute path of the folfer
        public string FullDirPath {
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
                return Directory.Exists(FullDirPath);
            }
        }

        public override int CompareTo(ALocation comparable) {
            return this.ToString().CompareTo(comparable.ToString());
        }

        public override string FullRelativeDirPath {
            get {
                string return_me;
                switch (EV) {
                    case EnvironmentVariable.AltSavePaths:
                    case EnvironmentVariable.Drive:
                    case EnvironmentVariable.InstallLocation:
                    case EnvironmentVariable.ProgramFiles:
                    case EnvironmentVariable.ProgramFilesX86:
                    case EnvironmentVariable.PS3Export:
                    case EnvironmentVariable.PS3Save:
                    case EnvironmentVariable.PSPSave:
                    case EnvironmentVariable.SteamCommon:
                    case EnvironmentVariable.SteamSourceMods:
                    case EnvironmentVariable.None:
                    case EnvironmentVariable.StartMenu:
                    case EnvironmentVariable.FlashShared:
                    case EnvironmentVariable.UbisoftSaveStorage:
                        return_me = AbsoluteRoot;
                        break;
                    case EnvironmentVariable.Public:
                    case EnvironmentVariable.AllUsersProfile:
                    case EnvironmentVariable.CommonApplicationData:
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

                return return_me;
            }
        }

        public override string ToString() {
            string return_me = FullRelativeDirPath;

            if (MatchesOriginalPath)
                return_me += " (This Archive Originally Came From This Folder)";

            return return_me;
        }

        public void delete() {
            try {
                DirectoryInfo info = new DirectoryInfo(FullDirPath);
                if (info.Exists) {
                    info.Attributes = FileAttributes.Normal;
                    info.Delete(true);
                }
            } catch (Exception e) {
                throw new Translator.TranslateableException("DeleteError", e, FullDirPath);
            }
        }
    }
}
