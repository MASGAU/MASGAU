using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MASGAU.Location.Holders {
    // This holds locations that have been found
    public class DetectedLocationPathHolder : LocationPathHolder {
        public DetectedLocationPathHolder(LocationPathHolder path) {
            this.append_path = path.append_path;
            this.detract_path = path.detract_path;
            this.IsEnabled = path.IsEnabled;
            this.IsExpanded = path.IsExpanded;
            this.IsSelected = path.IsSelected;
            this.language = path.language;
            this.platform_version = path.platform_version;
            this.path = path.path;
            this.read_only = path.read_only;
            this.rel_root = path.rel_root;
        }

        protected DetectedLocationPathHolder() {
        }

        // Holds the actual, interpreted root location
        public string abs_root;
        // Holds the associated user for this folder
        public string owner;

        public string full_relative_dir_path {
            get {
                if (path == null || path == "") {
                    return rel_root.ToString();
                }
                else {
                    return Path.Combine(rel_root.ToString(), path);
                }
            }
        }

        // Gets the full absolute path of the folfer
        public string full_dir_path {
            get {
                if (abs_root != null && abs_root != "") {
                    if (path == null || path == "") {
                        return abs_root;
                    }
                    else {
                        return Path.Combine(abs_root, path);
                    }
                }
                else {
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
                return_me = abs_root;
            else
                return_me = rel_root.ToString();

            if (path != null)
                return_me = Path.Combine(return_me, path);

            return return_me;
        }

        public void delete() {
            try {
                DirectoryInfo info = new DirectoryInfo(full_dir_path);
                if (info.Exists) {
                    info.Attributes = FileAttributes.Normal;
                    info.Delete(true);
                }
            }
            catch (Exception e) {
                throw new MException("Delete Error", "Error while trying to delete this:\n" + full_dir_path + "\nYou probably don't have permission to do that.", e, false);
            }
        }
    }
}
