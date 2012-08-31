using System.Collections.Generic;
using MASGAU.Location.Holders;
using MVC;
namespace MASGAU.Main {
    class FileTreeViewItem : CheckedTreeViewItem {
        public FileTreeViewItem(CheckedTreeViewItem new_parent)
            : base(new_parent) {

        }
        public void addFile(List<string> file_path, DetectedFile file) {
            foreach (FileTreeViewItem item in _children) {
                if (file_path[0] == item.Name) {
                    file_path.RemoveAt(0);
                    item.addFile(file_path, file);
                    return;
                }
            }

            FileTreeViewItem new_item = new FileTreeViewItem(this);

            if (file_path.Count > 0) {
                new_item.Name = file_path[0];
                if (file_path.Count > 1) {
                    file_path.RemoveAt(0);
                    new_item.addFile(file_path, file);
                } else {
                    new_item.file = file;
                }
                _children.Add(new_item);
            }
        }
        public DetectedFile file = null;
    }
}
