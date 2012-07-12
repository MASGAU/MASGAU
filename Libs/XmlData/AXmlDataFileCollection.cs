using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
namespace XmlData {
    public abstract class AXmlDataFileCollection<T> : List<T> where  T : AXmlDataFile<IXmlDataEntry> {
        private DirectoryInfo path;
        private List<FileInfo> files;
        private string file_pattern;

        protected AXmlDataFileCollection(string path, string file_pattern) {
            LoadXml(path, file_pattern);
        }
        protected AXmlDataFileCollection() { }

        protected void LoadXml(string path, string file_pattern) {
            this.Clear();

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);

            this.path = new DirectoryInfo(path);
            files = new List<FileInfo>(this.path.GetFiles(file_pattern));

            if (files.Count == 0)
                throw new FileNotFoundException(Path.Combine(path, file_pattern));


            foreach (FileInfo file in files) {
                T data_file;
                data_file = ReadFile(file);
                this.Add(data_file);
            }


        }

        protected abstract T ReadFile(FileInfo path);

        public List<IXmlDataEntry> Entries {
            get {
                List<IXmlDataEntry> return_me = new List<IXmlDataEntry>();
                foreach(AXmlDataFile<IXmlDataEntry> file in this) {
                    return_me.AddRange(file.entries);
                }
                return return_me;
            }
        }

        public void addFile(FileInfo path) {

        }

    }
}
