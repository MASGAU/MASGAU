using System;
using System.Collections.Generic;
using Collections;

namespace MASGAU.Location.Holders {
    public class DetectedFiles : DictionaryList<string, DetectedFile> {
        public new KeyCollection Keys {
            get {
                throw new NotImplementedException("NO KEYS FOR YOU");
            }
        }

        public override List<DetectedFile> Flatten() {
            List<DetectedFile> files = new List<DetectedFile>(null_type);
            files.AddRange(base.Flatten());
            return files;
        }

        List<DetectedFile> null_type = new List<DetectedFile>();

        public void AddRange(List<DetectedFile> files) {
            foreach (DetectedFile file in files) {
                Add(file);
            }
        }

        public void Add(DetectedFile file) {
            List<DetectedFile> add_here;
            if (file.type == null) {
                add_here = null_type;
            } else {
                add_here = this.GetList(file.type);
            }

            int index = add_here.IndexOf(file);
            if (index == -1) {
                add_here.Add(file);
            } else {
                DetectedFile existing = add_here[index];
                if (file.rel_root > existing.rel_root)
                    add_here[index] = file;
            }
        }

        public void Remove(DetectedFiles files) {
            foreach (DetectedFile file in files.Flatten()) {
                Remove(file);
            }
        }

        public void Remove(DetectedFile file) {
            List<DetectedFile> files;
            if (file.type == null) {
                files = null_type;
            } else if (this.ContainsKey(file.type)) {
                files = this[file.type];
            } else {
                return;
            }

            for (int i = 0; i < files.Count; i++) {
                DetectedFile check_me = files[i];
                if (file.type == check_me.type &&
                    file.full_file_path == check_me.full_file_path) {
                    files.RemoveAt(i);
                    i--;
                }
            }

        }
    }
}
