using System.Collections.Generic;
using System.IO;
using GameSaveInfo;
using MVC;
namespace MASGAU.Location {
    public class UserData : AModelItem<StringID> {
        public string name {
            get {
                return id.ToString();
            }
        }
        //, user_dir, user_documents, app_data, local_app_data, start_menu, virtual_store, saved_games;
        public Dictionary<EnvironmentVariable, EvFolder> folders = new Dictionary<EnvironmentVariable, EvFolder>();

        public UserData(string name)
            : base(new StringID(name)) {
        }


        public EvFolder getFolder(EnvironmentVariable ev) {
            if (folders.ContainsKey(ev)) {
                return folders[ev];
            } else
                return null;
        }

        public void setEvFolder(EnvironmentVariable ev, string folder) {
            this.setEvFolder(ev, new EvFolder(folder));
        }
        public void setEvFolder(EnvironmentVariable ev, string name, string folder) {
            this.setEvFolder(ev, new EvFolder(name, folder));
        }
        public void setEvFolder(EnvironmentVariable ev, DirectoryInfo folder, bool user_sub_folders) {
            if (user_sub_folders) {
                if (!folder.Exists || folder.GetDirectories().Length == 0)
                    return;

                this.setEvFolder(ev, new EvFolder(folder, user_sub_folders));
            } else {
                this.setEvFolder(ev, folder.FullName);
            }
        }
        public void setEvFolder(EnvironmentVariable ev, EvFolder folder) {
            if (folders.ContainsKey(ev))
                folders[ev] = folder;
            else
                folders.Add(ev, folder);
        }
        public void addEvFolder(EnvironmentVariable ev, string name, string folder) {
            if (folders.ContainsKey(ev)) {
                EvFolder evf = folders[ev];
                evf.AddFolder(name, folder);
            } else {
                setEvFolder(ev, name, folder);
            }
        }

        public bool hasFolderFor(EnvironmentVariable ev) {
            return folders.ContainsKey(ev);
        }
    }
}
