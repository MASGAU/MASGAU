using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using MASGAU.Update;
using Communication;
using Communication.Progress;
using Communication.Message;
using Communication.Translator;
using Translations;
namespace MASGAU.Game
{
    public class GamesXMLHandler
    {
        // This stores all the XML game configurations 
        public List<GameXMLHolder>      game_profiles;
        public List<UpdateHandler>    xml_file_versions;

        public GamesXMLHandler() {

        }
        
        public bool ready = false;
        public void loadXml() {
            TranslatingProgressHandler.setTranslatedMessage("LoadingGameXmls");
            game_profiles = new List<GameXMLHolder>();
            xml_file_versions = new List<UpdateHandler>();
            string game_configs = Path.Combine(Core.app_path,"data");
            if (!Directory.Exists(game_configs))
                throw new TranslateableException("CouldNotFindGameProfilesFolder");

            List<FileInfo> read_us;
	        DirectoryInfo read_me = new DirectoryInfo(game_configs);

            read_us = new List<FileInfo>(read_me.GetFiles("*.xml"));

            if(Core.portable_mode) {
                FileInfo custom_xml = new FileInfo(Path.Combine("..","..","Data","custom.xml"));
                if(custom_xml.Exists)
                    read_us.Add(custom_xml);
            }

            if (read_us.Count == 0)
                throw new TranslateableException("NoXmlFilesInDataFolder");

            int i = 1;
            foreach(FileInfo me_me in read_us) {
                i++;

                XmlDocument game_config;
                try {
                    game_config = Core.readXmlFile(me_me.FullName);
                } catch (XmlException ex) {
                    TranslatingMessageHandler.SendError("GameXmlParseError", ex, me_me.FullName);
                    continue;
                }


                string hold_this = "";

                XmlElement games_node = null;
                foreach(XmlNode node in game_config.ChildNodes) {
                    if(node.Name=="games") {
                        games_node = node as XmlElement;
                    }
                }

                if(games_node==null) {
                    TranslatingMessageHandler.SendWarning("GameXmlNoGamesTag", me_me.Name);
                    continue;
                }

                UpdateHandler new_update = new UpdateHandler(games_node,"data"+ "/" + me_me.Name,me_me.FullName);
                xml_file_versions.Add(new_update);

                foreach(XmlElement element in games_node.ChildNodes) {
                    if(element.Name!="game")
                        continue;

                    GameXMLHolder add_me;
                    if(element.GetAttribute("name").Contains(" ")) {
                        TranslatingMessageHandler.SendWarning("SpaceInGameName",hold_this);
                        continue;
                    }
                    String name = element.GetAttribute("name");
                    GamePlatform platform;
                    String region;

                    if(element.HasAttribute("platform"))
                        platform = GameHandler.parseGamePlatform(element.GetAttribute("platform"));
                    else
                        platform = GamePlatform.Multiple;

                    if (element.HasAttribute("region"))
                        region = element.GetAttribute("region");
                    else
                        region = null;

                    bool deprecated = false;
                    if(element.HasAttribute("deprecated"))
                        deprecated = Boolean.Parse(element.GetAttribute("deprecated"));

                    add_me = new GameXMLHolder(new GameID(name,platform,region, deprecated),element);

                    game_profiles.Add(add_me);
                }

            }

            ready = true;
        }

    }
}
