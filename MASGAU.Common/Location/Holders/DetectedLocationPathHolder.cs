using System;
using System.IO;
using GameSaveInfo;
using Exceptions;
namespace MASGAU.Location.Holders {
    // This holds locations that have been found
    public class DetectedLocationPathHolder : LocationPath {

        public DetectedLocationPathHolder(LocationPath path, string absolute_root, string owner)
            : base(path) {
            this.language = path.language;
            this.AbsoluteRoot = absolute_root;
            this.owner = owner;
        }


        public DetectedLocationPathHolder(EnvironmentVariable ev, string absolute_root, string path, string owner)
            : base(ev, path) {
            if (absolute_root == null)
                throw new Exception("ABSOLUTE ROOT MUST BE PROVIDED");
            this.owner = owner;
			this.AbsoluteRoot = absolute_root;



			if (this.Exists) {
				string[] parts = this.FullDirPath.Split(System.IO.Path.DirectorySeparatorChar);
				DirectoryInfo dir = new DirectoryInfo(parts[0] + System.IO.Path.DirectorySeparatorChar);
				int i = 1;
				bool found;
				while (i < parts.Length) {
					found = false;
					foreach (DirectoryInfo folder in dir.GetDirectories()) {
						if (folder.Name.ToLower() == parts[i].ToLower()) {
							found = true;
							dir = folder;
							i++;
							break;
						}
					}
					if (!found) {
						throw new Exceptions.WTFException("The folder " + parts[i] + " could not be found in " + dir.FullName);
					}
				}
				string new_root = dir.FullName.Substring(0, absolute_root.Length).TrimEnd(System.IO.Path.DirectorySeparatorChar);
				string new_path = dir.FullName.Substring(absolute_root.Length).Trim(System.IO.Path.DirectorySeparatorChar);
				if(this.Path==null) {
					if (this.Path == null && !String.IsNullOrEmpty(new_path) || this.AbsoluteRoot.ToLower() != new_root.ToLower()) {
						throw new Exceptions.WTFException(String.Concat(this.Path,new_path,this.AbsoluteRoot,new_root));
					} 

				} else {
					if (new_path.ToLower() !=  this.Path.ToLower() || this.AbsoluteRoot.ToLower() != new_root.ToLower()) {
						throw new Exceptions.WTFException(String.Concat(this.Path, new_path, this.AbsoluteRoot, new_root));
					}

				}

				this.AbsoluteRoot = new_root;
				this.Path = new_path; 
			}
        }

        protected DetectedLocationPathHolder() {
        }

        public QuickHash RootHash {
            get {
                if (AbsoluteRoot == null)
                    return null;
                return new QuickHash(AbsoluteRoot);
            }
        }

        public bool MatchesOriginalPath = false;

        // Holds the actual, interpreted root location
        public string AbsoluteRoot { get; protected set; }
        // Holds the associated user for this folder
        public string owner;


        // Gets the full absolute path of the folfer
        public string FullDirPath {
            get {
                if (AbsoluteRoot != null && AbsoluteRoot != "") {
                    if (Path == null || Path == "") {
                        return AbsoluteRoot;
                    } else {
                        return System.IO.Path.Combine(AbsoluteRoot, Path);
                    }
                } else {
                    return null;
                }
            }
        }

        public bool Exists {
            get {
                return Directory.Exists(FullDirPath);
            }
        }

        public override int CompareTo(ALocation comparable) {
            return this.ToString().CompareTo(comparable.ToString());
        }

        public override string FullRelativeDirPath {
            get {
                string return_me;
                switch (EV) {
                    case EnvironmentVariable.AltSavePaths:
                    case EnvironmentVariable.Drive:
                    case EnvironmentVariable.InstallLocation:
                    case EnvironmentVariable.ProgramFiles:
                    case EnvironmentVariable.ProgramFilesX86:
                    case EnvironmentVariable.PS3Export:
                    case EnvironmentVariable.PS3Save:
                    case EnvironmentVariable.PSPSave:
                    case EnvironmentVariable.SteamCommon:
                    case EnvironmentVariable.SteamSourceMods:
                    case EnvironmentVariable.None:
                    case EnvironmentVariable.StartMenu:
                    case EnvironmentVariable.FlashShared:
                    case EnvironmentVariable.UbisoftSaveStorage:
                        return_me = AbsoluteRoot;
                        break;
                    case EnvironmentVariable.Public:
                    case EnvironmentVariable.AllUsersProfile:
                    case EnvironmentVariable.CommonApplicationData:
                    case EnvironmentVariable.Desktop:
                    case EnvironmentVariable.AppData:
                    case EnvironmentVariable.LocalAppData:
                    case EnvironmentVariable.SavedGames:
                    case EnvironmentVariable.SteamUser:
                    case EnvironmentVariable.SteamUserData:
                    case EnvironmentVariable.UserDocuments:
                    case EnvironmentVariable.UserProfile:
                    case EnvironmentVariable.VirtualStore:
                        return_me = EV.ToString();
                        break;
                    default:
                        throw new NotSupportedException(EV.ToString());
                }
                if (Path != null)
                    return_me = System.IO.Path.Combine(return_me, Path);

                return return_me;
            }
        }

        public override string ToString() {
            string return_me = FullRelativeDirPath;

            if (MatchesOriginalPath)
                return_me += " (This Archive Originally Came From This Folder)";

            return return_me;
        }

        public void delete() {
            try {
                DirectoryInfo info = new DirectoryInfo(FullDirPath);
                if (info.Exists) {
                    info.Attributes = FileAttributes.Normal;
                    info.Delete(true);
                }
            } catch (Exception e) {
                throw new Translator.TranslateableException("DeleteError", e, FullDirPath);
            }
        }
    }
}
