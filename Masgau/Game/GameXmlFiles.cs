using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MVC.Translator;
using MASGAU.Update;
using Translator;
using XmlData;
using GameSaveInfo;
namespace MASGAU.Game {
    public class GameXmlFiles: AXmlDataFileCollection<GameXmlFile,GameSaveInfo.Game> {

        public DirectoryInfo common = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "masgau"));
        protected DirectoryInfo source = new DirectoryInfo(Path.Combine(Core.app_path, "data"));

        FileInfo common_schema;
        FileInfo master_schema;

        public const string SchemaName = "GameSaveInfo20.xsd";

        public GameXmlFiles() {
            if (!common.Exists)
                common.Create();

            master_schema = new FileInfo(Path.Combine(Core.app_path, "data", SchemaName));
            if (!master_schema.Exists)
                throw new TranslateableException("SchemaNotFound",master_schema.FullName);

            common_schema = new FileInfo(Path.Combine(common.FullName, SchemaName));

            if (!common_schema.Exists||common_schema.LastWriteTime<master_schema.LastWriteTime) {
                master_schema.CopyTo(common_schema.FullName, true);
            }

            List<FileInfo> files = prepareDataFiles();

            try {
                this.LoadXml(files);
            } catch (DirectoryNotFoundException e) {
                throw new TranslateableException("CouldNotFindGameProfilesFolder",e);
            } catch (FileNotFoundException e) {
                throw new TranslateableException("NoXmlFilesInDataFolder",e);
            }
        }
        public GameXmlFile getFile(string name) {
            foreach(GameXmlFile file in this) {
                if (file.File.Name == name)
                    return file;
            }
            return null;
        }

        protected virtual List<FileInfo> prepareDataFiles() {
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo original in source.GetFiles("*.xml")) {
                FileInfo file = new FileInfo(Path.Combine(common.FullName, original.Name));
                if (!file.Exists || original.LastWriteTime>file.LastWriteTime) {
                    original.CopyTo(file.FullName, true);
                }
                files.Add(file);
            }
            return files;
        }

        private bool IsRestorable(FileInfo file) {
            FileInfo original = new FileInfo(Path.Combine(source.FullName,file.Name));
            return original.Exists;
        }


        protected override GameXmlFile ReadFile(FileInfo path) {
            bool keep_trying = true;
            while (keep_trying) {
                try {
                    GameXmlFile file = new GameXmlFile(path);
                    return file;
                } catch (XmlException ex) {
                    TranslatingMessageHandler.SendError("XMLFormatError", ex, path.FullName);
                    if (IsRestorable(path)) {
                        if (!TranslatingRequestHandler.Request(MVC.Communication.RequestType.Question, "GameDataCorruptedRestore", path.Name).Cancelled) {
                            path.Delete();
                            prepareDataFiles();
                        } else {
                            keep_trying = false;
                        }
                    } else {
                        if (!TranslatingRequestHandler.Request(MVC.Communication.RequestType.Question, "GameDataCorruptedDelete", path.Name).Cancelled) {
                            path.Delete();
                        }
                        keep_trying = false;
                    }
                }
            }
            return null;
        }

    }
}
