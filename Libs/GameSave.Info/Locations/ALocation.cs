using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using XmlData;
namespace GameSaveInfo {
    public abstract class ALocation: AXmlDataEntry, IComparable<ALocation> {
        public static readonly List<string> attributes = new List<string> {"append", "detract", "only_for","deprecated","gsm_id"};
        public abstract int CompareTo(ALocation comparable);

        protected ALocation(ALocation loc): base() {
            this.Append = loc.Append;
            this.Detract = loc.Detract;
            this.OnlyFor = loc.OnlyFor;
            this.IsDeprecated = loc.IsDeprecated;

        }


        // Used to add or remove path elements
        public string Append { get; protected set; }
        public string Detract { get; protected set; }
        public bool IsDeprecated { get; protected set; }
        public string OnlyFor { get; protected set; }

        // Used to filter user locations by windows versions and language
        public string language = null;
        public bool override_virtual_store = false;

        protected ALocation(XmlElement element)
            : base(element) {
        }
        protected ALocation() : base() { }

        protected override void LoadData(XmlElement element) {
            foreach (XmlAttribute attrib in element.Attributes) {
                switch (attrib.Name) {
                    case "append":
                        this.Append = attrib.Value;
                        break;
                    case "detract":
                        this.Detract = attrib.Value;
                        break;
                    case "deprecated":
                        this.IsDeprecated = Boolean.Parse(attrib.Value);
                        break;
                    case "only_for":
                        this.OnlyFor = attrib.Value;
                        break;
                }
            }
            LoadMoreData(element);
        }
        protected abstract void LoadMoreData(XmlElement element);

        protected override XmlElement WriteData(XmlElement element) {
            addAtribute(element,"append",Append);
            addAtribute(element,"detract",Detract);
            addAtribute(element,"deprecated",IsDeprecated.ToString());
            addAtribute(element,"only_for",OnlyFor);
            return WriteMoreData(element);
        }
        protected abstract XmlElement WriteMoreData(XmlElement element);

        // This receives a path and modifies it based on the object's append and detract settings
        public static string modifyPath(string path, ALocation holder) {
            path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (holder.Detract != null) {
                if (path.EndsWith(holder.Detract))
                    path = path.Substring(0, path.Length - holder.Detract.Length);
            }
            if (holder.Append != null)
                path = Path.Combine(path, holder.Append);
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
                case "virtualstore":
                    return EnvironmentVariable.VirtualStore;
                case "commonapplicationdata":
                    return EnvironmentVariable.CommonApplicationData;
            }
            throw new NotImplementedException("Unrecognized environment variable: " + parse_me);
        }


        protected static int compare(IComparable a, IComparable b) {
            if (a == null) {
                if (b == null)
                    return 0;
                else
                    return -1;
            } else {
                return a.CompareTo(b);
            }
        }

    }

}