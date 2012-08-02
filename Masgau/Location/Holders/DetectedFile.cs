using System;
using System.IO;

namespace MASGAU.Location.Holders {
    // This holds files that have been found
    public class DetectedFile : DetectedLocationPathHolder, IEquatable<DetectedFile> {
        // public DetectedFile(): base() {
        // }

        public DetectedFile(DetectedLocationPathHolder location, string path, string name, string type)
            : base(location) {
            AbsoluteRoot = location.full_dir_path;
            owner = location.owner;
            this.Name = name;
            this.Type = type;
        }

        public bool Equals(DetectedFile file) {
            return this.full_file_path.Equals(file.full_file_path);
        }

        // This is the name of the file
        public string Name { get; protected set; }

        public string Type { get; protected set; }

        public override string ToString() {
            return System.IO.Path.Combine(base.ToString(), Name);
        }

        // Gets the full path, including file name
        public string full_file_path {
            get {
                if (full_dir_path != null) {
                    if (Name != null && Name != "") {
                        return System.IO.Path.Combine(full_dir_path, Name);
                    } else {
                        return full_dir_path;
                    }
                } else {
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
            } catch (Exception e) {
                throw new Translator.TranslateableException("DeleteError", e, full_file_path);
            }
        }
    }
}
