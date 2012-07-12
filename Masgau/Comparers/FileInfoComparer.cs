using System;
using System.IO;

namespace MASGAU.Comparers {
    public class FileInfoComparer : ReversableComparer<FileInfo> {

        public FileInfoComparer()
            : base(false) {
        }

        public FileInfoComparer(Boolean reverse)
            : base(reverse) {
        }

        public override int Compare(FileInfo file1, FileInfo file2) {
            return this.ReversableCompare(file1.FullName, file2.FullName);
        }
    }
}
