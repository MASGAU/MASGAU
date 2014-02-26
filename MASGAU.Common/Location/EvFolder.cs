using System;
using System.Collections.Generic;
using System.IO;
using GameSaveInfo;
using MASGAU.Location.Holders;
namespace MASGAU.Location {
    public class EvFolder : Dictionary<string, string> {
        public IEnumerable<string> Folders {
            get {
                return this.Values;
            }
        }

        protected IEnumerable<string> FolderNames {
            get {
                return this.Keys;
            }
        }
        public bool HasDirs {
            get {
                return this.Count > 0;
            }

        }

        public bool HasMultipleDirs {
            get {
                return this.Count > 1;
            }
        }
        public string BaseFolder { get; protected set; }

        public EvFolder(string folder): this("",folder) { }

        public EvFolder(string name, string folder) {
            this.Add(name, folder);
            BaseFolder = folder;
        }

        public EvFolder(DirectoryInfo parent, bool create_from_subfolders) {
            if (create_from_subfolders) {
                BaseFolder = parent.FullName;

                DirectoryInfo[] subs = parent.GetDirectories();
                foreach (DirectoryInfo dir in subs) {
                    this.Add(dir.Name, dir.FullName);
                }
            } else {
                this.Add("", parent.FullName);
                BaseFolder = parent.FullName;
            }
        }

        public bool Matches(string path) {
            if (String.IsNullOrEmpty(path)) {
                return false;
            }
            foreach (string folder in this.Values) {
                string[] split = path.Split(Path.DirectorySeparatorChar);
                for (int i = 0; i < split.Length; i++) {
                    string new_path = split[0] + Path.DirectorySeparatorChar;
                    for (int j = 1; j <= i; j++) {
                        new_path = Path.Combine(new_path, split[j]);
                    }
                    if (new_path.ToLower().Equals(folder.ToLower()))
                        return true;
                }
            }
            return false;
        }

        public IEnumerable<DetectedLocationPathHolder> createDetectedLocations(LocationPath loc, string owner) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            foreach (string folder in this.Keys) {
                DetectedLocationPathHolder add_me = new DetectedLocationPathHolder(loc.EV, this[folder], loc.Path, owner);
                return_me.Add(add_me);
            }
            return return_me;
        }



        public void AddFolder(string name, string path) {
            this.Add(name, path);

        }
    }
}
