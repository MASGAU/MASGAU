using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
namespace XmlData {
    public abstract class AXmlDataFileCollection<F,E> : List<F> where  F : AXmlDataFile<E> where E: AXmlDataEntry {
        private DirectoryInfo path;
        private List<FileInfo> files;
        //private string file_pattern;

        protected AXmlDataFileCollection(string path, string file_pattern) {

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);

            this.path = new DirectoryInfo(path);
            files = new List<FileInfo>(this.path.GetFiles(file_pattern));

            if (files.Count == 0)
                throw new FileNotFoundException(Path.Combine(path, file_pattern));


            LoadXml(files);
        }
        protected AXmlDataFileCollection() { }

        protected AXmlDataFileCollection(IEnumerable<FileInfo> files) {
            LoadXml(files);
        }

        protected void LoadXml(IEnumerable<FileInfo> files) {
            this.Clear();

            foreach (FileInfo file in files) {
                F data_file;
                data_file = ReadFile(file);
                this.Add(data_file);
            }
        }

        protected abstract F ReadFile(FileInfo path);

        public List<E> Entries {
            get {
                List<E> return_me = new List<E>();
                foreach(F file in this) {
                    return_me.AddRange(file.Entries);
                }
                return return_me;
            }
        }

        public void addFile(FileInfo path) {

        }

    }
}
