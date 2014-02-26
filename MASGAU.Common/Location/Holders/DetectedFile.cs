using System;
using System.IO;

namespace MASGAU.Location.Holders {
    // This holds files that have been found
    public class DetectedFile : DetectedLocationPathHolder, IEquatable<DetectedFile> {
        // public DetectedFile(): base() {
        // }
        public DetectedLocationPathHolder OriginalLocation { get; protected set; }
        public DetectedFile(DetectedLocationPathHolder location, string path, string name, string type)
            : base(location, location.FullDirPath, location.owner) {
            OriginalLocation = location;

            this.Path = path;
            this.Name = name;
            this.Type = type;
        }

        public bool Equals(DetectedFile file) {
            return this.full_file_path.Equals(file.full_file_path);
        }

        // This is the name of the file
        public string Name { get; protected set; }

        public string Type { get; set; }

        public override string ToString() {
            return System.IO.Path.Combine(base.ToString(), Name);
        }

        // Gets the full path, including file name
        public string full_file_path {
            get {
                if (!String.IsNullOrEmpty(FullDirPath)) {
                    if (!String.IsNullOrEmpty(Name)) {
                        return System.IO.Path.Combine(FullDirPath, Name);
                    } else {
                        return FullDirPath;
                    }
                } else {
                    return null;
                }
            }
        }
        public new bool Exists {
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
