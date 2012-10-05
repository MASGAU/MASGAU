using System.Collections.Generic;
using System.IO;
using GameSaveInfo;
using MASGAU.Location.Holders;
namespace MASGAU.Location {
    public class EvFolder : Dictionary<string, string> {
        public bool Matches(string path) {
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



        public IEnumerable<string> Folders {
            get {
                return this.Values;
            }
        }
        protected IEnumerable<string> SubFolders {
            get {
                return this.Keys;
            }
        }

        public string BaseFolder { get; protected set; }

        public EvFolder(string folder) {
            this.Add("", folder);
            BaseFolder = folder;
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

        public IEnumerable<DetectedLocationPathHolder> createDetectedLocations(LocationPath loc, string owner) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            foreach (string folder in this.Keys) {
                DetectedLocationPathHolder add_me = new DetectedLocationPathHolder(loc.EV, this[folder], loc.Path, owner);
                return_me.Add(add_me);
            }
            return return_me;
        }

        public EvFolder(DirectoryInfo parent) {

            BaseFolder = parent.FullName;

            DirectoryInfo[] subs = parent.GetDirectories();
            foreach (DirectoryInfo dir in subs) {
                this.Add(dir.Name, dir.FullName);
            }
        }
    }
}
