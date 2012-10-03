using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Collections;
using GameSaveInfo;
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

        public void AddFiles(FileType type, DetectedLocationPathHolder location) {
            foreach (Include file in type.Inclusions) {
                AddFiles(file, location);
            }
        }
        public void AddFiles(Include save, DetectedLocationPathHolder location) {
            AddFiles(save, location, new Regex(".*"));
        }
        public void AddFiles(Include save, DetectedLocationPathHolder location, Regex reg) {
            List<DetectedFile> files = new List<DetectedFile>();
            foreach (DetectedFile file in GatherFiles(save, location, reg)) {
                if(!files.Contains(file))
                    files.Add(file);
            }

            foreach (Exclude exclude in save.Exclusions) {
                foreach (DetectedFile file in GatherFiles(exclude, location, new Regex(".*"))) {
                    if (files.Contains(file))
                        files.Remove(file);
                }
            }

            foreach (DetectedFile file in files) {
                if(!this.Contains(file))
                    this.AddRange(files);
            }
        }

        private static List<DetectedFile> GatherFiles(Exclude save, DetectedLocationPathHolder location, Regex reg) {
            List<DetectedFile> files = new List<DetectedFile>();
            foreach (string file in save.FindMatching(location.full_dir_path)) {

                string name = Path.GetFileName(file);
                string path = file.Substring(0, file.Length - name.Length).Trim(Path.DirectorySeparatorChar);

                if (!reg.IsMatch(file)) {
                    continue;
                }

                DetectedFile detected = new DetectedFile(location, path, name, save.Type);
                files.Add(detected);
            }
            return files;
        }

        public void AddRange(IEnumerable<DetectedFile> files) {
            foreach (DetectedFile file in files) {
                Add(file);
            }
        }

        public bool Contains(DetectedFile file) {
            return this.Contains(file.Type, file);
        }

        public void Add(DetectedFile file) {
            List<DetectedFile> add_here;
            if (file.Type == null) {
                add_here = null_type;
            } else {
                add_here = this.GetList(file.Type);
            }

            int index = add_here.IndexOf(file);
            if (index == -1) {
                add_here.Add(file);
            } else {
                DetectedFile existing = add_here[index];
                if (file.EV > existing.EV)
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
            if (file.Type == null) {
                files = null_type;
            } else if (this.ContainsKey(file.Type)) {
                files = this[file.Type];
            } else {
                return;
            }

            for (int i = 0; i < files.Count; i++) {
                DetectedFile check_me = files[i];
                if (file.Type == check_me.Type &&
                    file.full_file_path == check_me.full_file_path) {
                    files.RemoveAt(i);
                    i--;
                }
            }

        }
    }
}
