using System.IO;
using System;
using System.Xml;
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
        private bool _deprecated = false;
        public bool deprecated {
            set {
                _deprecated = value;
            }
            get {
                return _deprecated;
            }
        }

        // Used to filter user locations by windows versions and language
        public string language = null;


        public bool override_virtual_store = false;
        public PlatformVersion platform_version = PlatformVersion.All;

        protected ALocationHolder(): base(new StringID(null)) { }

        protected ALocationHolder(XmlElement element): this() {
            if (element.HasAttribute("append"))
                this.append_path = element.GetAttribute("append");
            else
                this.append_path = null;

            if (element.HasAttribute("deprecated"))
                this.deprecated = true;
            else
                this.deprecated = false;

            if (element.HasAttribute("detract"))
                this.detract_path = element.GetAttribute("detract");
            else
                this.detract_path = null;

            if (element.HasAttribute("platform_version"))
                this.platform_version = (PlatformVersion)Enum.Parse(typeof(PlatformVersion), element.GetAttribute("platform_version"), true);
            else
                this.platform_version = PlatformVersion.All;

            if (element.HasAttribute("language"))
                this.language = element.GetAttribute("language");
            else
                this.language = null;

        }

        public ALocationHolder(ALocationHolder copy_me)
            : base(new StringID(copy_me.ToString())) {
            append_path = copy_me._append_path;
            detract_path = copy_me._detract_path;
            language = copy_me.language;
            platform_version = copy_me.platform_version;
        }
        
        // This receives a path and modifies it based on the object's append and detract settings
        public static string modifyPath(string path, ALocationHolder holder) {
            path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (holder.detract_path!= null) {
                if(path.EndsWith(holder.detract_path))
                    path = path.Substring(0,path.Length-holder.detract_path.Length);
            }
            if (holder.append_path != null)
                path = Path.Combine(path,holder.append_path);
            return path.TrimEnd(Path.DirectorySeparatorChar);
        }

        public string modifyPath(string path) {
            return modifyPath(path,this);
        }

        public static EnvironmentVariable parseEnvironmentVariable(string parse_me)
        {
            switch (parse_me.ToLower())
            {
                case "allusersprofile":
                    return EnvironmentVariable.AllUsersProfile;
                case "altsavepaths":
                    return EnvironmentVariable.AltSavePaths;
                case "appdata":
                    return EnvironmentVariable.AppData;
                case "drive":
                    return EnvironmentVariable.Drive;
                case "installlocation":
                    return EnvironmentVariable.InstallLocation;
                case "localappdata":
                    return EnvironmentVariable.LocalAppData;
                case "public":
                    return EnvironmentVariable.Public;
                case "savedgames":
                    return EnvironmentVariable.SavedGames;
                case "steamuser":
                    return EnvironmentVariable.SteamUser;
                case "steamcommon":
                    return EnvironmentVariable.SteamCommon;
                case "steamsourcemods":
                    return EnvironmentVariable.SteamSourceMods;
                case "steamuserdata":
                    return EnvironmentVariable.SteamUserData;
                case "userprofile":
                    return EnvironmentVariable.UserProfile;
                case "userdocuments":
                    return EnvironmentVariable.UserDocuments;
                case "flashshared":
                    return EnvironmentVariable.FlashShared;
                case "startmenu":
                    return EnvironmentVariable.StartMenu;
                case "desktop":
                    return EnvironmentVariable.Desktop;
            }
            throw new NotImplementedException("Unrecognized environment variable: " + parse_me);
        }

    }

}