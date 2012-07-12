using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using Communication.Translator;
namespace MASGAU.Location.Holders {
    // This lets me abstract out the loading logic for those below :D
    public abstract class AGenericHolder {
        public string Type { get; protected set; }
        public string Name { get; protected set; }
        public string Path { get; protected set; }
        public string OnlyFor { get; protected set; }
        public DateTime ModifiedAfter { get; protected set; }

        protected AGenericHolder() { }

        public AGenericHolder(XmlElement element) {
            foreach(XmlAttribute attr in element.Attributes) {
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
        }

        public virtual List<DetectedFile> FindMatching(DetectedLocationPathHolder location) {
            List<DirectoryInfo> directories = new List<DirectoryInfo>();
            List<DetectedFile> return_me = new List<DetectedFile>();
            if (Name == null) {
                if (Path == null) {
                    if (Directory.Exists(location.full_dir_path)) {
                        return_me.AddRange(findTheseFilesHelper(location, gatherFiles(location.full_dir_path)));
                    }
                } else {
                    directories.AddRange(getPaths(location.full_dir_path, Path));
                    foreach (DirectoryInfo directory in directories) {
                        return_me.AddRange(findTheseFilesHelper(location, gatherFiles(directory.FullName)));
                    }
                }
            } else if (Path == null) {
                if (Directory.Exists(location.full_dir_path)) {
                    List<string> files = new List<string>();
                    foreach (FileInfo read_me in new DirectoryInfo(location.full_dir_path).GetFiles(Name)) {
                        files.Add(read_me.FullName);
                    }
                    return_me.AddRange(findTheseFilesHelper(location, files));
                }
            } else {
                directories.AddRange(getPaths(location.full_dir_path, Path));
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

        private List<DetectedFile> findTheseFilesHelper(DetectedLocationPathHolder location, List<string> files) {
            List<DetectedFile> return_me = new List<DetectedFile>();
            DetectedFile add_me;
            foreach (string file_name in files) {
                FileInfo file = new FileInfo(file_name);
                if (!file.Exists)
                    continue;

                if (file.LastWriteTime <= ModifiedAfter)
                    continue;

                add_me = new DetectedFile(location,Type);


                if (file.DirectoryName.Length == add_me.AbsoluteRoot.Length)
                    add_me.Path = "";
                else
                    add_me.Path = file.DirectoryName.Substring(add_me.AbsoluteRoot.Trim(System.IO.Path.DirectorySeparatorChar).Length + 1);

                add_me.name = file.Name;


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
            } catch (UnauthorizedAccessException e) {
                throw e;
            } catch (Exception e) {
                TranslatingMessageHandler.SendError("FileGatherError", e, root);
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
                TranslatingMessageHandler.SendError("ReadError", e, root);
            }
            return return_me;
        }
    }


    // This holds the save detection information loaded straight from the XML file
    public class SaveHolder : ExceptHolder {
        private List<ExceptHolder> Excepts = new List<ExceptHolder>();

        public SaveHolder(string name, string path, string type) {
            Name = name;
            Path = path;
            Type = type;
        }

        public SaveHolder(XmlElement element, string type): base(element, type) {
            foreach (XmlElement child in element.ChildNodes) {
                switch (child.Name) {
                    case "except":
                        ExceptHolder except = new ExceptHolder(child, type);
                        Excepts.Add(except);
                        break;
                    default:
                        throw new NotSupportedException(child.Name);
                }
            }
        }

        public override List<DetectedFile> FindMatching(DetectedLocationPathHolder location) {
            return base.FindMatching(location);
        }
    }
    // This holds the ignore information loaded straight from the XML file
    public class ExceptHolder : AGenericHolder {
        protected ExceptHolder() { }
        public ExceptHolder(XmlElement element, string type): base(element) {
            this.Type = type;   
        }
    }

    // This holds the information for an identifier
    public class IdentifierHolder : AGenericHolder {
        public IdentifierHolder(XmlElement element): base(element) { }
    }

}
