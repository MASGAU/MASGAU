using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MVC;
namespace MASGAU.Location.Holders {
    public abstract class ALocationHolder : AModelItem<StringID> {
        protected XmlElement xml = null;

        public static readonly List<string> attributes = new List<string> {"append", "detract", "only_for","deprecated"};

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

        public string OnlyFor { get; protected set; }

        protected ALocationHolder() : base(new StringID(null)) { }

        protected ALocationHolder(XmlElement element)
            : this() {
                this.xml = element;
            foreach (XmlAttribute attrib in element.Attributes) {
                switch (attrib.Name) {
                    case "append":
                        this.append_path = attrib.Value;
                        break;
                    case "detract":
                        this.detract_path = attrib.Value;
                        break;
                    case "deprecated":
                        this.deprecated = Boolean.Parse(attrib.Value);
                        break;
                    case "only_for":
                        this.OnlyFor = attrib.Value;
                        break;
                }
            }
        }

        public ALocationHolder(ALocationHolder copy_me)
            : base(new StringID(copy_me.ToString())) {
            append_path = copy_me._append_path;
            detract_path = copy_me._detract_path;
            language = copy_me.language;
            OnlyFor = copy_me.OnlyFor;
        }

        // This receives a path and modifies it based on the object's append and detract settings
        public static string modifyPath(string path, ALocationHolder holder) {
            path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (holder.detract_path != null) {
                if (path.EndsWith(holder.detract_path))
                    path = path.Substring(0, path.Length - holder.detract_path.Length);
            }
            if (holder.append_path != null)
                path = Path.Combine(path, holder.append_path);
            return path.TrimEnd(Path.DirectorySeparatorChar);
        }

        public string modifyPath(string path) {
            return modifyPath(path, this);
        }

        public static EnvironmentVariable parseEnvironmentVariable(string parse_me) {
            switch (parse_me.ToLower()) {
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
                case "programfiles":
                    return EnvironmentVariable.ProgramFiles;
                case "programfilesx86":
                    return EnvironmentVariable.ProgramFilesX86;
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
                case "ubisoftsavestorage":
                    return EnvironmentVariable.UbisoftSaveStorage;
            }
            throw new NotImplementedException("Unrecognized environment variable: " + parse_me);
        }

    }

}