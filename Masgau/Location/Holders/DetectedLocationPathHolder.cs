using System;
using System.IO;
using GameSaveInfo;
namespace MASGAU.Location.Holders {
    // This holds locations that have been found
    public class DetectedLocationPathHolder : LocationPath {

        public DetectedLocationPathHolder(LocationPath path): base(path) {
            this.language = path.language;
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

        public string full_relative_dir_path {
            get {
                if (Path == null || Path == "") {
                    return rel_root.ToString();
                } else {
                    return System.IO.Path.Combine(rel_root.ToString(), Path);
                }
            }
        }

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
            if (rel_root == EnvironmentVariable.AllUsersProfile ||
                rel_root == EnvironmentVariable.AltSavePaths ||
                rel_root == EnvironmentVariable.Drive ||
                rel_root == EnvironmentVariable.InstallLocation ||
                rel_root == EnvironmentVariable.ProgramFiles ||
                rel_root == EnvironmentVariable.ProgramFilesX86 ||
                rel_root == EnvironmentVariable.Public ||
                rel_root == EnvironmentVariable.SteamCommon ||
                rel_root == EnvironmentVariable.SteamSourceMods)
                return_me = AbsoluteRoot;
            else
                return_me = rel_root.ToString();

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
