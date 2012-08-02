using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using XmlData;
namespace GameSaveInfo {
    public abstract class AFile: AXmlDataEntry {
        public string Name { get; protected set; }
        public string Path { get; protected set; }
        public string OnlyFor { get; protected set; }
        public DateTime ModifiedAfter { get; protected set; }

        protected AFile(string name, string path, XmlDocument doc): base(doc) {
            Name = name;
            Path = path;
        }

        protected AFile(XmlElement element): base(element) {

        }

        protected override void LoadData(XmlElement element) {
            foreach (XmlAttribute attr in element.Attributes) {
                switch (attr.Name) {
                    case "path":
                        Path = attr.Value;
                        break;
                    case "filename":
                        Name = attr.Value;
                        break;
                    case "modified_after":
                        ModifiedAfter = DateTime.Parse(attr.Value);
                        break;
                    case "only_for":
                        OnlyFor = attr.Value;
                        break;
                    default:
                        throw new NotSupportedException(attr.Name);
                }
            }
            LoadMoreData(element);
        }
        protected abstract void LoadMoreData(XmlElement element);
        protected abstract XmlElement WriteMoreData(XmlElement element);

        protected override XmlElement WriteData(XmlElement element) {
            addAtribute(element, "path", Path);
            addAtribute(element, "filename", Name);

            if (ModifiedAfter != new DateTime())
                throw new Exception("Don't know ho to write modified after!");

            addAtribute(element, "only_for", Path);
            return WriteMoreData(element);
        }


        public virtual List<string> FindMatching(string location) {
            List<DirectoryInfo> directories = new List<DirectoryInfo>();
            List<string> return_me = new List<string>();

            if (Name == null) {
                if (Path == null) {
                    if (Directory.Exists(location)) {
                        return_me.AddRange(findTheseFilesHelper(location, gatherFiles(location)));
                    }
                } else {
                    directories.AddRange(getPaths(location, Path));
                    foreach (DirectoryInfo directory in directories) {
                        return_me.AddRange(findTheseFilesHelper(location, gatherFiles(directory.FullName)));
                    }
                }
            } else if (Path == null) {
                if (Directory.Exists(location)) {
                    List<string> files = new List<string>();
                    foreach (FileInfo read_me in new DirectoryInfo(location).GetFiles(Name)) {
                        files.Add(read_me.FullName);
                    }
                    return_me.AddRange(findTheseFilesHelper(location, files));
                }
            } else {
                directories.AddRange(getPaths(location, Path));
                foreach (DirectoryInfo directory in directories) {
                    List<string> files = new List<string>();
                    foreach (FileInfo read_me in directory.GetFiles(Name)) {
                        files.Add(read_me.FullName);
                    }
                    return_me.AddRange(findTheseFilesHelper(location, files));
                }
            }
            return return_me;
        }

        private List<string> findTheseFilesHelper(string location, List<string> files) {
            List<string> return_me = new List<string>();
            foreach (string file_name in files) {
                FileInfo file = new FileInfo(file_name);
                if (!file.Exists)
                    continue;

                if (file.LastWriteTime <= ModifiedAfter)
                    continue;

                string add_me = file.FullName;


                if (file.DirectoryName.Length != add_me.Length)
                    add_me = add_me.Substring(location.Trim(System.IO.Path.DirectorySeparatorChar).Length + 1);
                else
                    throw new Exception("what?");

                return_me.Add(add_me);
            }
            return return_me;
        }


        private static List<string> gatherFiles(string root) {
            List<string> return_me = new List<string>();
            try {
                foreach (FileInfo file in new DirectoryInfo(root).GetFiles()) {
                    return_me.Add(file.FullName);
                }
                foreach (DirectoryInfo sub_folder in new DirectoryInfo(root).GetDirectories()) {
                    return_me.AddRange(gatherFiles(sub_folder.FullName));
                }
            } catch (Exception e) {
                throw e;
            }
            return return_me;
        }


        private static List<DirectoryInfo> getPaths(string root, string path) {
            List<DirectoryInfo> return_me = new List<DirectoryInfo>();
            DirectoryInfo root_directory = new DirectoryInfo(root);

            string[] split = path.Split(System.IO.Path.DirectorySeparatorChar);
            string forward_me = "";

            for (int i = 1; i < split.Length; i++) {
                forward_me = System.IO.Path.Combine(forward_me, split[i]);
            }
            try {
                DirectoryInfo[] directories = root_directory.GetDirectories(split[0]);
                if (split.Length == 1) {
                    foreach (DirectoryInfo add_me in directories) {
                        return_me.Add(add_me);
                    }
                } else {
                    foreach (DirectoryInfo add_me in directories) {
                        return_me.AddRange(getPaths(add_me.FullName, forward_me));
                    }
                }
            } catch (Exception e) {
                throw e;
            }
            return return_me;
        }



    }
}
