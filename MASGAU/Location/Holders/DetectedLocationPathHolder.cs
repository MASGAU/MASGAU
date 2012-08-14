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
                return new QuickHash(AbsoluteRoot);
            }
        }

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

        public bool exists {
            get {
                return Directory.Exists(full_dir_path);
            }
        }

        public override string ToString() {
            string return_me;
            if (EV == EnvironmentVariable.AllUsersProfile ||
                EV == EnvironmentVariable.AltSavePaths ||
                EV == EnvironmentVariable.Drive ||
                EV == EnvironmentVariable.InstallLocation ||
                EV == EnvironmentVariable.ProgramFiles ||
                EV == EnvironmentVariable.ProgramFilesX86 ||
                EV == EnvironmentVariable.Public ||
                EV == EnvironmentVariable.SteamCommon ||
                EV == EnvironmentVariable.SteamSourceMods)
                return_me = AbsoluteRoot;
            else
                return_me = EV.ToString();

            if (Path != null)
                return_me = System.IO.Path.Combine(return_me, Path);

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
