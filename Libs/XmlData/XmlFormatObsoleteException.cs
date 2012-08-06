using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
namespace XmlData {
    class XmlFormatObsoleteException: Exception {
        public FileInfo File { get; protected set; }
        public Version FileVersion { get; protected set; }
        public Version LatestVersion { get; protected set; }

        public XmlFormatObsoleteException(FileInfo file, Version file_version, Version latest_version) {
            File = file;
            FileVersion = file_version;
            LatestVersion = latest_version;
        }
        public XmlFormatObsoleteException(string file, Version file_version, Version latest_version) {
            File = new FileInfo(file);
            FileVersion = file_version;
            LatestVersion = latest_version;
        }

    }
}
