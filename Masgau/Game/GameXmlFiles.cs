using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Communication.Translator;
using MASGAU.Update;
using Translator;
using XmlData;
namespace MASGAU {
    public class GameXmlFiles: AXmlDataFileCollection<GameXmlFile,Game> {

        public List<UpdateHandler> xml_file_versions;

        public GameXmlFiles() {
            TranslatingProgressHandler.setTranslatedMessage("LoadingGameXmls");
            xml_file_versions = new List<UpdateHandler>();

            string game_configs = Path.Combine(Core.app_path, "data");

            try {
                this.LoadXml(game_configs, "*.xml");
            } catch (DirectoryNotFoundException e) {
                throw new TranslateableException("CouldNotFindGameProfilesFolder",e);
            } catch (FileNotFoundException e) {
                throw new TranslateableException("NoXmlFilesInDataFolder",e);
            }

            if (Core.portable_mode) {
                FileInfo custom_xml = new FileInfo(Path.Combine("..", "..", "Data", "custom.xml"));
                if (custom_xml.Exists)
                    this.addFile(custom_xml);
            }


        }

        protected override GameXmlFile ReadFile(FileInfo path) {
            try {
                GameXmlFile file = new GameXmlFile(path);
                UpdateHandler new_update = new UpdateHandler(file.RootNode, Path.Combine("data",path.Name), path.FullName);
                xml_file_versions.Add(new_update);
                return file;
            } catch (XmlException ex) {
                TranslatingMessageHandler.SendError("XMLFormatError", ex, path.FullName);
            }
            return null;
        }

    }
}
