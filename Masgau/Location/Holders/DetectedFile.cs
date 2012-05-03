using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MASGAU.Location.Holders {
    // This holds files that have been found
    public class DetectedFile : DetectedLocationPathHolder, IEquatable<DetectedFile> {
        // public DetectedFile(): base() {
        // }

        public DetectedFile(DetectedLocationPathHolder location, string type)
            : base(location) {
            abs_root = location.full_dir_path;
            owner = location.owner;
            this.type = type;
        }

        public bool Equals(DetectedFile file) {
            return this.full_file_path.Equals(file.full_file_path);
        }

        // This is the name of the file
        public string name;

        public string type;

        public override string ToString() {
            return Path.Combine(base.ToString(), name);
        }

        // Gets the full path, including file name
        public string full_file_path {
            get {
                if (full_dir_path != null) {
                    if (name != null && name != "") {
                        return Path.Combine(full_dir_path, name);
                    }
                    else {
                        return full_dir_path;
                    }
                }
                else {
                    return null;
                }
            }
        }
        public new bool exists {
            get {
                return File.Exists(full_file_path);
            }
        }
        public new void delete() {
            try {
                Directory.Delete(full_file_path, true);
            }
            catch (Exception e) {
                throw new CommunicatableException("Delete Error", "Error while trying to delete this:\n" + full_file_path + "\nYou probably don't have permission to do that.", e, false);
            }
        }
    }
}
