using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Forms;
using System.Security.Permissions;
using System.IO.Compression;
using wyDay.Controls;
namespace MASGAU
{

public class SettingsManager {
    public Dictionary<string,GameData>  games;
    public Dictionary<string, string>   game_profiles;
    public static bool                  isValid = true;
    public string                       backup_path = null;
    public string                       sync_path = null;
    public SteamHandler                 steam;
    public PathHandler                  paths;
//    public gfwLiveHandler               live;
    public SyncHandler                  sync;
    public string                       height=null, width=null;
    public playstationHandler           playstation;
    public ArrayList                    alt_paths = new ArrayList();
    public ArrayList                    disabled_games = new ArrayList();
    private string                      config_path;
    public string                       steam_override = null;
    private SecurityHandler             red_shirt = new SecurityHandler();
    public bool                         ignore_date_check = false;
    public bool                         all_users_mode = false;
    public bool                         versioning = false;
    public bool                         show_undetected = false;
    public bool                         skip_backup_on_monitor_startup = false;
    public int                          versioning_frequency = 1, versioning_max = 1;
    public string                       versioning_unit = "Hours";
	public string                       windows_version = null;
    private invokes                     invokes = new invokes();
    public bool                         auto_update_enabled = false, already_updated = false;
    private FileStream                  config_stream;
//    private Dictionary<string,ArrayList> additional_paths = new Dictionary<string,ArrayList>();


    public SettingsManager(string force_config, ArrayList these_games, Windows7ProgressBar progress, object progress_label, Form mommy) {
        string[] args = Environment.GetCommandLineArgs();
        for(int i = 0;i<args.Length;i++) {
            if(args[i]=="/allusers") {
                all_users_mode = true;
            }
        }


        if (progress_label != null) {
            invokes.setControlText(progress_label,"Loading settings...");
        }

        bool config_found = false;
        if (File.Exists(Path.Combine(Application.StartupPath,"config.xml"))){
            config_path = Application.StartupPath;
            config_stream = new FileStream(Path.Combine(config_path, "config.xml"),FileMode.Append,FileAccess.Write);
            if(config_stream.Length>0) {
                config_found = true;
            } else {
                config_found = false;
            }
            config_stream.Close();
        } else {
            if(all_users_mode) {
                config_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"masgau");
            } else {
                config_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),".masgau");
            }

            if (File.Exists(Path.Combine(config_path,"config.xml"))){
                try {
                    config_stream = new FileStream(Path.Combine(config_path, "config.xml"),FileMode.Append,FileAccess.Write);
                    if(config_stream.Length>0) {
                        config_found = true;
                    } else {
                        config_found = false;
                    }
                    config_stream.Close();
                } catch (Exception exception) {
                    invokes.showMessageBox(mommy,"Can't Continue Like This","The config file couldn't be opened for writing." + Environment.NewLine + "MASGAU can't work without that." + Environment.NewLine + Path.Combine(config_path, "config.xml") + Environment.NewLine + exception.Message,MessageBoxButtons.OK,MessageBoxIcon.Error);
                    Application.Exit();
                }
            } else {
                if(!Directory.Exists(config_path))
                try {
                    Directory.CreateDirectory(config_path);
                } catch(Exception exception) {
                    invokes.showMessageBox(mommy,"Can't Go On Like This","The config path couldn't be opened for writing." + Environment.NewLine + "MASGAU can't work without that." + Environment.NewLine + config_path + Environment.NewLine + exception.Message,MessageBoxButtons.OK,MessageBoxIcon.Error);
                    Application.Exit();
                }

                try {
                    config_stream = new FileStream(Path.Combine(config_path, "config.xml"),FileMode.Create,FileAccess.Write);
                    config_found = false;
                    config_stream.Close();
                } catch (Exception exception) {
                    invokes.showMessageBox(mommy,"Can't Continue Like This","The config file couldn't be opened for writing." + Environment.NewLine + "MASGAU can't work without that." + Environment.NewLine + Path.Combine(config_path, "config.xml") + Environment.NewLine + exception.Message,MessageBoxButtons.OK,MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }

        string temp_path;
        if(config_found) {
            XmlReaderSettings xml_settings = new XmlReaderSettings();
            xml_settings.ConformanceLevel = ConformanceLevel.Document;
            xml_settings.IgnoreComments = true;
            xml_settings.IgnoreWhitespace = true;
            lock(config_stream) {
                config_stream = new FileStream(Path.Combine(config_path, "config.xml"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlReader load_me = XmlReader.Create(config_stream,xml_settings);
                while (load_me.Read()) {
    			    if(load_me.NodeType==XmlNodeType.Element) {
				        switch (load_me.Name) {
                            case "backup_path":
                                temp_path = load_me.ReadString();
                                if(isReadable(temp_path)&&isWritable(temp_path)) 
                                    backup_path = temp_path;
                                break;
                            case "sync_path":
                                temp_path = load_me.ReadString();
                                if(isReadable(temp_path)&&isWritable(temp_path)) 
                                    sync_path = temp_path;
                                break;
                            case "steam_override":
                                steam_override = load_me.ReadString();
                                break;
						    // Maybe someday...
						    //case "manual_path":
						    //    while(load_me.MoveToNextAttribute()) {
						    //        switch(load_me.Name) {
						    //            case "game":
						    //                game_name = load_me.Value;
						    //                temp_path = load_me.ReadString();
						    //                if (Directory.Exists(temp_path)) {
						    //                    addManualPath(game_name,temp_path);
						    //                }
						    //                break;
						    //        }
						    //    }
						    //    break;
                            case "alt_path":
                                temp_path = load_me.ReadString();
                                alt_paths.Add(temp_path);
                                break;
                            case "date_check":
                                while(load_me.MoveToNextAttribute()) {
                                    switch(load_me.Name) {
                                        case "ignore":
                                            if(load_me.Value=="yes")
                                                ignore_date_check = true;
                                            break;
                                    }
                                }
                                break;
                            case "auto_update":
                                while(load_me.MoveToNextAttribute()) {
                                    switch(load_me.Name) {
                                        case "enable":
                                            if(load_me.Value=="yes")
                                                auto_update_enabled = true;
                                            break;
                                    }
                                }
                                break;
                            case "monitor_startup_backup":
                                while(load_me.MoveToNextAttribute()) {
                                    switch(load_me.Name) {
                                        case "skip":
                                            if(load_me.Value=="yes")
                                                skip_backup_on_monitor_startup = true;
                                            break;
                                    }
                                }
                                break;
                            case "window":
                                while(load_me.MoveToNextAttribute()) {
                                    switch(load_me.Name) {
                                        case "height":
                                            height = load_me.Value;
                                            break;
                                        case "width":
                                            width = load_me.Value;
                                            break;
                                    }
                                }
                                break;
                            case "show_undetected_games":
                                while(load_me.MoveToNextAttribute()) {
                                    switch(load_me.Name) {
                                        case "enable":
                                            if(load_me.Value=="yes")
                                                show_undetected = true;
                                            break;
                                    }
                                }
                                break;
                            case "disable":
                                while(load_me.MoveToNextAttribute()) {
                                    switch(load_me.Name) {
                                        case "game":
                                            disabled_games.Add(load_me.Value);
                                            break;
                                    }
                                }
                                break;
                            case "versioning":
                                while(load_me.MoveToNextAttribute()) {
                                    switch(load_me.Name) {
                                        case "enabled":
                                            if(load_me.Value=="yes")
                                                versioning = true;
                                            break;
                                        case "frequency":
                                            versioning_frequency = Int32.Parse(load_me.Value);
                                            break;
                                        case "frequency_unit":
                                            versioning_unit = load_me.Value;
                                            break;
                                        case "max":
                                            versioning_max = Int32.Parse(load_me.Value);
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
                load_me.Close();
                config_stream.Close();
            }
        }

        if (progress_label != null) {
            invokes.setControlText(progress_label,"Checking for Steam...");
        }

        if(steam_override!=null)
            steam = new SteamHandler(steam_override);
        else 
            steam = new SteamHandler();

        if (progress_label != null) {
            invokes.setControlText(progress_label,"Detecting paths...");
        }
        paths = new PathHandler();

        //if (progress_label != null) {
        //    invokes.setControlText(progress_label,"Detecting G4W...");
        //}
        //live = new gfwLiveHandler(paths);

		if(paths.xp) {
			windows_version = "XP";
		} else {
			windows_version = "Vista";
		}

        if (alt_paths.Count > 0)
            paths.addAltPath(alt_paths);
        
        if (progress_label != null) {
            invokes.setControlText(progress_label,"Detecting Playstations...");
        }
        playstation = new playstationHandler();


        detectGames(these_games, progress, progress_label, mommy);

        if (sync_path != null) {
            invokes.setControlText(progress_label,"Updating syncs...");
            sync = new SyncHandler(sync_path);
        }
    
    }

    private static void validationHandler(object sender, ValidationEventArgs args){
        throw new XmlException(args.Message);
    }

    public void detectGames(ArrayList these_games, Windows7ProgressBar progress, object progress_label, Form mommy) {
        game_profiles = new Dictionary<string, string>();
        string game_configs = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"data");
        if(Directory.Exists(game_configs)) {
            FileInfo[] read_us;
	        DirectoryInfo read_me = new DirectoryInfo(game_configs);
            if(progress!=null) {
                invokes.setProgressBarMin(progress,0);
                invokes.setProgressBarValue(progress,0);
            }
            if (progress_label != null) {
                invokes.setControlText(progress_label,"Loading XML files...");
            }
            string current_file = "No File";
            try {
                read_us = read_me.GetFiles("*.xml");
                int i = 1;
                isValid = true;
                if(read_us.Length>0) {
			        foreach(FileInfo me_me in read_us) {
                        i++;
                        Console.Write(i.ToString());
                        XmlReaderSettings xml_settings = new XmlReaderSettings();
                        xml_settings.ConformanceLevel = ConformanceLevel.Auto;
                        xml_settings.IgnoreWhitespace = true;
                        xml_settings.IgnoreComments = false;
                        xml_settings.IgnoreProcessingInstructions = false;
                        xml_settings.DtdProcessing = DtdProcessing.Parse;
                        //xml_settings.ProhibitDtd = false;
                        xml_settings.ValidationType = ValidationType.Schema;
                        xml_settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
                        xml_settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                        xml_settings.ValidationEventHandler += new ValidationEventHandler(validationHandler);
                        XmlReader parse_me = XmlReader.Create(me_me.FullName, xml_settings);

                        string hold_this = "";
                        bool keep_going = true;
                        int major_version = 0;
                        int minor_version = 0;
                        int revision_version = 0;
                        while(keep_going) {
                            try {
		                        keep_going = parse_me.Read();
                                if (keep_going&&parse_me.NodeType == XmlNodeType.Element&&parse_me.Name == "games"){

                                    while(parse_me.MoveToNextAttribute()) {
				                        switch (parse_me.Name) {
                                            case "majorVersion":
                                                    major_version = Convert.ToInt32(parse_me.Value);
                                                break;
                                            case "minorVersion":
                                                    minor_version = Convert.ToInt32(parse_me.Value);
                                                break;
                                            case "revisionVersion":
                                                    revision_version = Convert.ToInt32(parse_me.Value);
                                                break;
                                        }
                                    }
                                }
                                if (keep_going&&parse_me.NodeType == XmlNodeType.Element&&parse_me.Name == "game"){
                                    while(parse_me.MoveToNextAttribute()) {
                                        if(parse_me.Name=="name") {
                                            hold_this = parse_me.Value;
                                            if(hold_this.Contains(" ")) {
                                                MessageBox.Show("There's a space in the game name " + hold_this,"No spaces!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                                            } else {
                                                if(these_games!=null&&these_games.Count>0) {
                                                    if(these_games.Contains(hold_this)) {
                                                        parse_me.MoveToElement();
                                                        game_profiles.Add(hold_this, parse_me.ReadInnerXml());
                                                    }
                                                } else {
                                                    parse_me.MoveToElement();
                                                    game_profiles.Add(hold_this, parse_me.ReadInnerXml());
                                                }
                                            }
                                        }
                                    }
                                }
                            } catch (XmlException exception) {
                                IXmlLineInfo info = parse_me as IXmlLineInfo;
                                MessageBox.Show("The file " + me_me.Name + " has produced this error:" + Environment.NewLine + exception.Message + Environment.NewLine + "The error is on or near line " + info.LineNumber + ", possibly at column " + info.LinePosition + "." + Environment.NewLine + "Go fix it.","What A Duketastrophe",MessageBoxButtons.OK,MessageBoxIcon.Error);
                                return;
                            } catch (System.ArgumentException exception) {
                                IXmlLineInfo info = parse_me as IXmlLineInfo;
                                MessageBox.Show("The file " + me_me.Name + " has produced this error:" + Environment.NewLine + exception.Message + Environment.NewLine + "This is usually caused by a duplicate game name," + Environment.NewLine + "which happens to be " + hold_this + ".","Dukelicious Irony",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            } catch {
                                IXmlLineInfo info = parse_me as IXmlLineInfo;
                                MessageBox.Show("There was an error while trying to read the XML file " + me_me.Name + "", "The Humanity",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            }
                        }
                        parse_me.Close();

                    }
                } else {
                    MessageBox.Show("There are no XML files in the Data folder.","What the heck?",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            } catch {
                MessageBox.Show("Could not get one or more game profiles.\nLast read was at: " + current_file,"Silly Penguins!",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        } else {
            MessageBox.Show("Could not find game profiles folder","Trashy Talk, Yes?",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
        games = new Dictionary<string,GameData>();
        if(game_profiles.Count>0) {
            if(progress!=null) {
                invokes.setProgressBarMax(progress,game_profiles.Count);
            }
            int detection_progress = 1;
            foreach(KeyValuePair<string,string> game_profile in game_profiles) {
                if (progress_label != null) {
                    invokes.setControlText(progress_label,"Detecting games (" + detection_progress + "/" + game_profiles.Count + ")...");
                }
                if(progress!=null) {
                    invokes.performStep(progress);
                    //invokes.setProgressBarValue(progress,detection_progress);
                }
                games.Add(game_profile.Key,new GameData(game_profile.Value, paths, steam, playstation,game_profiles,windows_version));
                Console.Write(games[game_profile.Key].title + ": ");
                if(games[game_profile.Key].detected_locations!=null&&games[game_profile.Key].detected_locations.Count>0) {
                    Console.WriteLine("Detected!");
                } else {
                    Console.WriteLine("Not Detected!");
                }
                if(disabled_games.Contains(game_profile.Key))
                    games[game_profile.Key].disabled = true;
                detection_progress++;

            }
        }
        Console.WriteLine("Convenient Break Point");


    }

    public bool writeConfig() {
        XmlDocument write_me = new XmlDocument();
        XmlElement node;
        XmlAttribute attribute;
        lock(config_stream) {
            if(File.Exists(config_path + "\\config.xml")) {
                File.Delete(config_path + "\\config.xml");
            }

            XmlTextWriter write_here = new XmlTextWriter(config_path + "\\config.xml", System.Text.Encoding.UTF8);
            write_here.Formatting = Formatting.Indented;
            write_here.WriteProcessingInstruction("xml","version='1.0' encoding='UTF-8'");
            write_here.WriteStartElement("config");
            write_here.Close();

            write_me.Load(config_path + "\\config.xml");
            XmlNode root = write_me.DocumentElement;

            if(backup_path!=null) {
                node = write_me.CreateElement("backup_path");
                node.InnerText = backup_path;
                write_me.DocumentElement.InsertAfter(node,write_me.DocumentElement.LastChild);
            }
            if(sync_path!=null) {
                node = write_me.CreateElement("sync_path");
                node.InnerText = sync_path;
                write_me.DocumentElement.InsertAfter(node,write_me.DocumentElement.LastChild);
            }
            if(steam_override!=null) {
                node = write_me.CreateElement("steam_override");
                node.InnerText = steam_override;
                write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
            }
            foreach(string me in alt_paths) {
                if(me!="") {
                    node = write_me.CreateElement("alt_path");
                    node.InnerText = me;
                    write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
                }
            }
            if(ignore_date_check) {
                node = write_me.CreateElement("date_check");
                attribute = write_me.CreateAttribute("ignore");
                attribute.Value = "yes";
                node.SetAttributeNode(attribute);
                write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
            }
            if(skip_backup_on_monitor_startup) {
                node = write_me.CreateElement("monitor_startup_backup");
                attribute = write_me.CreateAttribute("skip");
                attribute.Value = "yes";
                node.SetAttributeNode(attribute);
                write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
            }

            if(height!=null||width!=null) {
                node = write_me.CreateElement("window");
                if(height!=null) {
                    attribute = write_me.CreateAttribute("height");
                    attribute.Value = height;
                    node.SetAttributeNode(attribute);
                }
                if(width!=null) {
                    attribute = write_me.CreateAttribute("width");
                    attribute.Value = width;
                    node.SetAttributeNode(attribute);
                }
                write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
            }

            if(show_undetected) {
                node = write_me.CreateElement("show_undetected_games");
                attribute = write_me.CreateAttribute("enable");
                attribute.Value = "yes";
                node.SetAttributeNode(attribute);
                write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
            }
            
            if(auto_update_enabled) {
                node = write_me.CreateElement("auto_update");
                attribute = write_me.CreateAttribute("enable");
                attribute.Value = "yes";
                node.SetAttributeNode(attribute);
                write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
            }

            foreach(string disable_me in disabled_games) {
                node = write_me.CreateElement("disable");
                attribute = write_me.CreateAttribute("game");
                attribute.Value = disable_me;
                node.SetAttributeNode(attribute);
                write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
            }
						    // Maybe someday...

		    //if(additional_paths.Count>0) {
		    //    foreach(KeyValuePair<string,ArrayList> key in additional_paths){
		    //        foreach(string path in key.Value) {
		    //            node = write_me.CreateElement("manual_path");
		    //            attribute = write_me.CreateAttribute("game");
		    //            attribute.Value = key.Key;
		    //            node.SetAttributeNode(attribute);
		    //            node.InnerText = path;
		    //            write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);
		    //        }
		    //    }
		    //}

            node = write_me.CreateElement("versioning");
            if(versioning) {
                attribute = write_me.CreateAttribute("enabled");
                attribute.Value = "yes";
                node.SetAttributeNode(attribute);
            } else {
                attribute = write_me.CreateAttribute("enabled");
                attribute.Value = "no";
                node.SetAttributeNode(attribute);
            }
            attribute = write_me.CreateAttribute("frequency");
            attribute.Value = versioning_frequency.ToString();
            node.SetAttributeNode(attribute);
            attribute = write_me.CreateAttribute("frequency_unit");
            attribute.Value = versioning_unit;
            node.SetAttributeNode(attribute);
            attribute = write_me.CreateAttribute("max");
            attribute.Value = versioning_max.ToString();
            node.SetAttributeNode(attribute);
            write_me.DocumentElement.InsertAfter(node, write_me.DocumentElement.LastChild);

            config_stream = new FileStream(config_path + "\\config.xml", FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

            write_me.Save(config_stream);
            write_here.Close();
            config_stream.Close();
        }
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
    }
    public void resetSteam() {
        steam_override = null;
        steam = new SteamHandler();
    }



    public void addAltPath(string look_here) {
        if(Directory.Exists(look_here)) {
            alt_paths.Add(look_here);
            paths.addAltPath(look_here);
        }
    }

    public void removeAltPath(string remove_me) {
        for(int i=0;i<alt_paths.Count;i++) {
            if((string)alt_paths[i]==remove_me)
                alt_paths.RemoveAt(i);
        }
        paths.removeAltPath(remove_me);
    }

    public string getGameTitle(string get_me) {
        if(games.ContainsKey(get_me))
            return games[get_me].title;
        else 
            return null;
    }

    public bool isReadable(string path) {
        DirectoryInfo read_me = new DirectoryInfo(path);
        if(read_me.Exists) {
            try {
                read_me.GetFiles();
                return true;
            } catch {
                return false;
            }
        }
        return false;
    }

    public bool isWritable(string path) {
        if (Directory.Exists(path)) {
            string file_name = Path.GetRandomFileName();
            FileInfo test_file = new FileInfo(Path.Combine(path,file_name));
            try {
                FileStream delete_me = test_file.Create();
                delete_me.Close();
                test_file.Delete();
                return true;
            } catch {
                return false;
            }
        }
        return false;
    }

    public int countGames()
    {
        int i = 0;
        if(games!=null) {
            foreach (KeyValuePair<string, GameData> check_me in games)
            {
                if (!check_me.Value.disabled &&check_me.Value.detected_locations!=null&& check_me.Value.detected_locations.Count > 0)
                    i++;
            }
        }
        return i;
    }

    public long versioningTimeout() {
        switch(versioning_unit) {
            case "Seconds":
                return versioning_frequency*10000000;
            case "Minutes":
                return versioning_frequency*10000000*60;
            case "Hours":
                return versioning_frequency*10000000*60*60;
            case "Days":
                return versioning_frequency*10000000*60*60*24;
            case "Weeks":
                return versioning_frequency*10000000*60*60*24*7;
            case "Months":
                return versioning_frequency*10000000*60*60*24*7*4;
            case "Years":
                return versioning_frequency*10000000*60*60*24*365;
            case "Decades":
                return versioning_frequency*10000000*60*60*24*365*10;
            case "Centuries":
                return versioning_frequency*10000000*60*60*24*365*100;
            case "Millenia":
                return versioning_frequency*10000000*60*60*24*365*1000;
            default:
                return versioning_frequency*10000000*60*60;
        }
    }

    public bool checkForUpdate() {
        updateHandler updates = new updateHandler();
        if(updates.checkVersions()) {
            if(File.Exists("Updater.exe")) {
                MessageBox.Show("Update available");
            } else {
                MessageBox.Show("Updater missing");
            }
            return true;
        } else {
            MessageBox.Show("No updates");
            return false;
        }
    }

							// Maybe someday...

	//public void addManualPath(string game, string path)
	//{
	//    if(!additional_paths.ContainsKey(game))
	//        additional_paths.Add(game, new ArrayList());
	//    additional_paths[game].Add(path);
	//}

	//public void removeManualPath(string game, string path)
	//{
	//    if (additional_paths.ContainsKey(game)){
	//        if(additional_paths[game].Contains(path))
	//            additional_paths[game].Remove(path);
	//        if(additional_paths[game].Count==0)
	//            additional_paths.Remove(game);
	//    }
	//}

	//public void removeAllManualPaths(string game) {
	//    if(additional_paths.ContainsKey(game))
	//        additional_paths.Remove(game);
	//}

	//public ArrayList getManualPaths(string game)
	//{
	//    ArrayList return_me = new ArrayList();
	//    if(additional_paths.ContainsKey(game)) {
	//        return_me.AddRange(additional_paths[game]);
	//    }
	//    return return_me;
	//}
}
}