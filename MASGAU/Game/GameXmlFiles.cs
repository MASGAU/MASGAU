using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MVC.Translator;
using MASGAU.Update;
using Translator;
using XmlData;
using GameSaveInfo;
using System.Reflection;
namespace MASGAU.Game {
    public class GameXmlFiles: AXmlDataFileCollection<GameXmlFile,GameSaveInfo.Game> {
        public CustomGameXmlFile custom { get; protected set;  }
        public DirectoryInfo DataFolder {
            get {
                switch (Core.settings.mode) {
                    case Config.ConfigMode.Portable:
                        return new DirectoryInfo(Path.Combine(Core.app_path, "data"));
                    case Config.ConfigMode.AllUsers:
                    case Config.ConfigMode.SingleUser:
                        return new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "masgau"));
                }
                return null;
            }
        }
        public FileInfo SchemaFile {
            get {
                return new FileInfo(Path.Combine(DataFolder.FullName, GameXmlFile.Schema));
            }
        }


        public GameXmlFiles() {
            if (!DataFolder.Exists)
                DataFolder.Create();

            prepareDataFiles();


            List<FileInfo> files = new List<FileInfo>();
            files.AddRange(DataFolder.GetFiles("*.xml"));

            try {
                this.LoadXml(files);
                if (this.custom == null)
                    this.custom = new CustomGameXmlFile(new FileInfo(Path.Combine(DataFolder.FullName, "custom.xml")));
            } catch (DirectoryNotFoundException e) {
                throw new TranslateableException("CouldNotFindGameProfilesFolder",e);
            } catch (FileNotFoundException e) {
                throw new TranslateableException("NoXmlFilesInDataFolder",e);
            }

            if (this.custom == null) {
                this.custom = new CustomGameXmlFile(new FileInfo(Path.Combine(DataFolder.FullName, "custom.xml")));
            }
        }
        public GameXmlFile getFile(string name) {
            foreach(GameXmlFile file in this) {
                if (file.File.Name == name)
                    return file;
            }
            return null;
        }

        protected FileInfo extractResourceFile(string name) {
            FileInfo file = new FileInfo(Path.Combine(DataFolder.FullName,name.Substring(12)));
            string assembly = Assembly.GetExecutingAssembly().Location;

            ManifestResourceInfo info = Assembly.GetExecutingAssembly().GetManifestResourceInfo(name);

            FileInfo assm = new FileInfo(assembly);
            if (!file.Exists || assm.LastWriteTime > file.LastWriteTime) {
                using (FileStream ResourceFile = new FileStream(file.FullName, FileMode.Create)) {
                    Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);

                    byte[] b = new byte[s.Length + 1];
                    s.Read(b, 0, Convert.ToInt32(s.Length));
                    ResourceFile.Write(b, 0, Convert.ToInt32(b.Length - 1));
                    ResourceFile.Flush();
                    ResourceFile.Close();
                }
            }
            file.Refresh();
            return file;
        }

        protected virtual List<FileInfo> prepareDataFiles() {
            List<FileInfo> files = new List<FileInfo>();
            string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string name in names) {
                if(name.StartsWith("MASGAU.Data"))
                    extractResourceFile(name);
            }
            return files;
        }

        private bool IsRestorable(FileInfo file) {
//            FileInfo original = new FileInfo(Path.Combine(source.FullName,file.Name));
            return false; 
//            original.Exists;
        }


        protected override GameXmlFile ReadFile(FileInfo path) {
            bool keep_trying = true;
            while (keep_trying) {
                try {
                    if (path.Name == "custom.xml") {
                        this.custom = new CustomGameXmlFile(path);
                        return this.custom;
                    } else {
                        GameXmlFile file = new GameXmlFile(path);
                        return file;
                    }
                } catch (VersionNotSupportedException ex) {
                    if (ex.FileVersion==null||!GameSaveInfo.Converters.AConverter.CanConvert(ex.FileVersion)) {
                        string version_string;
                        if (ex.FileVersion == null)
                            version_string = Strings.GetLabelString("UnknownVersion");
                        else
                            version_string = ex.FileVersion.ToString();

                        if (!TranslatingRequestHandler.Request(MVC.Communication.RequestType.Question, "GameDataObsoleteDelete", path.Name, version_string).Cancelled) {
                            path.Delete();
                        }
                        keep_trying = false;
                    }
                } catch (XmlException ex) {
                    TranslatingMessageHandler.SendError("XMLFormatError", ex, path.FullName);
                    keep_trying = handleCorruptedFile(path);
                } catch (NotSupportedException ex) {
                    TranslatingMessageHandler.SendError("XMLFormatError", ex, path.FullName);
                    keep_trying = handleCorruptedFile(path);
                }
            }
            return null;
        }

        private bool handleCorruptedFile(FileInfo path) {
            if (IsRestorable(path)) {
                if (!TranslatingRequestHandler.Request(MVC.Communication.RequestType.Question, "GameDataCorruptedRestore", path.Name).Cancelled) {
                    path.Delete();
                    prepareDataFiles();
                } else {
                    return false;
                }
            } else {
                if (!TranslatingRequestHandler.Request(MVC.Communication.RequestType.Question, "GameDataCorruptedDelete", path.Name).Cancelled) {
                    path.Delete();
                }
                return false;
            }
            return true;
        }
    }
}
