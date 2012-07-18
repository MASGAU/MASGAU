using System;
using System.IO;
using System.Collections.Generic;
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

        public string getFolder(EnvironmentVariable ev) {
            if (folders.ContainsKey(ev)) {
                if (folders[ev].HasMultipleDirs)
                    throw new Exception("This folder has multiple candidates, and cannot be auto computed");
                return folders[ev].base_folder;
            }  else
                return null;
        }
        public void setEvFolder(EnvironmentVariable ev, string folder) {
            this.setEvFolder(ev, new EvFolder(folder));
        }
        public void setEvFolder(EnvironmentVariable ev, DirectoryInfo folder) {
            if (!folder.Exists||folder.GetDirectories().Length==0)
                return;

            this.setEvFolder(ev, new EvFolder(folder));

        }
        public void setEvFolder(EnvironmentVariable ev, EvFolder folder) {
            if (folders.ContainsKey(ev))
                folders[ev] = folder;
            else
                folders.Add(ev, folder);
        }
        public bool hasFolderFor(EnvironmentVariable ev) {
            return getFolder(ev) != null;
        }
    }
}
