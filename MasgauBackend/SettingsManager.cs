using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Windows.Forms;

namespace Masgau
{

public class SettingsManager {
    public ArrayList games;
    public string backup_path = null;
    public SteamHandler steam;
    public PathHandler  paths;
    public ArrayList alt_paths = new ArrayList();
    private string config_path;
    public string steam_override = null;


    public SettingsManager(string force_config, string force_game, ProgressBar progress) {
        config_path = System.Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + "\\MASGAU";
        bool config_found = false;

        if(File.Exists(Application.StartupPath + "\\config.xml")){
            config_found = true;
            config_path = Application.StartupPath;
        } else if (File.Exists(config_path + "\\config.xml")){
            config_found = true;
        } else {
            if(!Directory.Exists(config_path))
                Directory.CreateDirectory(config_path);
        }
        string temp_path;
        if(config_found) {
            XmlTextReader load_me = new XmlTextReader(config_path + "\\config.xml");
            while (load_me.Read()) {
    			if(load_me.NodeType==XmlNodeType.Element) {
				    switch (load_me.Name) {
                        case "backup_path":
                            temp_path = load_me.ReadString();
                            if(Directory.Exists(temp_path)) 
                                backup_path = temp_path;
                            break;
                        case "steam_override":
                            steam_override = load_me.ReadString();
                            break;
                        case "alt_path":
                            temp_path = load_me.ReadString();
                            if(Directory.Exists(temp_path))
                                alt_paths.Add(temp_path);
                            break;
                    }
                }
            }
            load_me.Close();
        }
        if(steam_override!=null)
            steam = new SteamHandler(steam_override);
        else 
            steam = new SteamHandler();

        paths = new PathHandler();

        if (alt_paths.Count > 0)
            paths.addAltPath(alt_paths);
        
        detectGames(force_game, progress);
    }

    public void detectGames(string force_game, ProgressBar progress) {
        games = new ArrayList();
        GameData new_game;
        string game_configs = null;
        if(Directory.Exists(Application.StartupPath + "\\games\\")) {
            game_configs = Application.StartupPath + "\\games\\";
        }
        if(game_configs!=null) {
	        DirectoryInfo read_me = new DirectoryInfo(game_configs);
            FileInfo[] read_us;
            if(force_game!=null) {
                read_us = read_me.GetFiles(force_game + ".xml");
                if (read_us.Length == 0)
                    Console.WriteLine("No config file found for " + force_game);
            } else   {
                read_us = read_me.GetFiles("*.xml");
            }
            progress.Maximum = read_us.Length;
            progress.Minimum = 0;
            progress.Value = 0;
            foreach(FileInfo me_me in read_us) {
                new_game = new GameData(me_me.FullName, paths, steam);
                Console.Write(new_game.title + ": ");
                if(new_game.root_detected) {
                    Console.WriteLine("Detected!");
                    games.Add(new_game);
                } else {
                    Console.WriteLine("Not Detected!");
                }
                progress.Value++;
            }
        }
    }

    public int countDetectedGames() {
        int return_me = 0;
        foreach(GameData check_me in games) {
            if(check_me.root_detected)
                return_me++;
        }
        return return_me;
    }

    public bool isDetected(string am_i) {
        return ((GameData)games[findGame(am_i)]).save_detected;
    }

    public int findGame(string find_me) {
        int return_me = 0;
        foreach(GameData check_me in games) {
            if(check_me.name==find_me) {
                return return_me;
            }
            return_me++;
        }
        return -1;
    }

    public bool writeConfig() {
        XmlDocument write_me = new XmlDocument();
        XmlElement node_me;
        if(File.Exists(config_path + "\\config.xml"))
            File.Delete(config_path + "\\config.xml");

        XmlTextWriter write_here = new XmlTextWriter(config_path + "\\config.xml", System.Text.Encoding.UTF8);
        write_here.Formatting = Formatting.Indented;
        write_here.WriteProcessingInstruction("xml","version='1.0' encoding='UTF-8'");
        write_here.WriteStartElement("config");
        write_here.Close();

        write_me.Load(config_path + "\\config.xml");
        XmlNode root = write_me.DocumentElement;

        if(backup_path!=null) {
            node_me = write_me.CreateElement("backup_path");
            node_me.InnerText = backup_path;
            write_me.DocumentElement.InsertAfter(node_me,write_me.DocumentElement.LastChild);
        }
        if(steam_override!=null) {
            node_me = write_me.CreateElement("steam_override");
            node_me.InnerText = steam_override;
            write_me.DocumentElement.InsertAfter(node_me, write_me.DocumentElement.LastChild);
        }
        foreach(string me in alt_paths) {
            if(me!="") {
                node_me = write_me.CreateElement("alt_path");
                node_me.InnerText = me;
                write_me.DocumentElement.InsertAfter(node_me, write_me.DocumentElement.LastChild);
            }
        }

        FileStream this_file = new FileStream(config_path + "\\config.xml", FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

        write_me.Save(this_file);
        write_here.Close();
        this_file.Close();
        return true;
    }

    public void overrideSteam(string look_here) {
        steam = new SteamHandler(look_here);
        if(steam.installed)  {
            steam_override = look_here;
        } else  {
            steam_override = null;
            steam = new SteamHandler();
        }

        for(int i=0;i<games.Count;i++) {
            ((GameData)games[i]).detect(paths,steam);
        }
    }
    public void resetSteam() {
        steam_override = null;
        steam = new SteamHandler();
        for(int i = 0; i < games.Count; i++){
            ((GameData)games[i]).detect(paths,steam);
        }
    }
    public void addAltPath(string look_here) {
        if(Directory.Exists(look_here)) {
            alt_paths.Add(look_here);
            paths.addAltPath(look_here);
            for(int i = 0; i < games.Count; i++){
                ((GameData)games[i]).detect(paths,steam);
            }
        }
    }
    public void removeAltPath(string remove_me) {
        for(int i=0;i<alt_paths.Count;i++) {
            if((string)alt_paths[i]==remove_me)
                alt_paths.RemoveAt(i);
        }
        paths.removeAltPath(remove_me);
        for(int i = 0; i < games.Count; i++){
            ((GameData)games[i]).detect(paths,steam);
        }
    }
    public string getGameTitle(string get_me) {
        foreach(GameData check_me in games) {
            if(check_me.name==get_me)
                return check_me.title;
        }
        return null;
    }
}
}