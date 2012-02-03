using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location {
    public class UserData : AModelItem<StringID> {
        public string name { 
            get {
                return id.ToString();
            }
        }
        //, user_dir, user_documents, app_data, local_app_data, start_menu, virtual_store, saved_games;
        public Dictionary<EnvironmentVariable, string> folders = new Dictionary<EnvironmentVariable, string>();

        public UserData(string name): base(new StringID(name)) {
        }

        public string getFolder(EnvironmentVariable ev) {
            if (folders.ContainsKey(ev))
                return folders[ev];
            else
                return null;
        }
        public void setEvFolder(EnvironmentVariable ev, string folder) {
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
