using System.IO;
using System;

namespace MASGAU.Location.Holders {
    public abstract class ALocationHolder: AModelItem<StringID> {
        // Used to add or remove path elements
        private string _append_path = null, _detract_path = null;
        public string append_path {
            set {
                _append_path = value;
            }
            get {
                return _append_path;
            }
        }
        public string detract_path {
            set {
                _detract_path = value;
            }
            get {
                return _detract_path;
            }
        }
        private bool _read_only = false;
        public bool read_only {
            set {
                _read_only = value;
            }
            get {
                return _read_only;
            }
        }

        // Used to filter user locations by windows versions and language
        public string language = null;
        // This receives a path and modifies it based on the object's append and detract settings
        public static string modifyPath(string path, ALocationHolder holder) {
            path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (holder.detract_path!= null) {
                if(path.EndsWith(holder.detract_path))
                    path = path.Substring(0,path.Length-holder.detract_path.Length);
            }
            if (holder.append_path != null)
                path = Path.Combine(path,holder.append_path);
            return path.Trim(Path.DirectorySeparatorChar);
        }

        public string modifyPath(string path) {
            return modifyPath(path,this);
        }
        public bool override_virtual_store = false;
        public PlatformVersion platform_version = PlatformVersion.All;

        protected ALocationHolder(): base(new StringID(null)) {
        }

        public ALocationHolder(ALocationHolder copy_me)
            : base(new StringID(copy_me.ToString())) {
            append_path = copy_me._append_path;
            detract_path = copy_me._detract_path;
            language = copy_me.language;
            platform_version = copy_me.platform_version;
        }


    }

}