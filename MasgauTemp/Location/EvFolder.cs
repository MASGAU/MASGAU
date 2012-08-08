using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace MASGAU.Location {
    public class EvFolder: Dictionary<string,string> {

        public string getFolder() {
            if (this.Count > 1)
                throw new Exception("too many paths!" + base_folder);

            return base_folder;
        }

        public string base_folder { get; protected set; }
        public EvFolder(string folder) {
            base_folder = folder;
        }

        public bool HasDirs {
            get {
                return this.Count > 0;
            }

        }

        public bool HasMultipleDirs {
            get {
                return this.Count>1;
            }
        }

        public EvFolder(DirectoryInfo parent)
            : this(parent.FullName) {
            DirectoryInfo[] subs = parent.GetDirectories();
            if (subs.Length == 1) {
                base_folder = subs[0].FullName;
            } else {
                foreach (DirectoryInfo dir in subs) {
                    this.Add(dir.Name, dir.FullName);
                }
            }
        }
    }
}
