using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Windows.Forms;
using System.Net;

namespace MASGAU
{
    public struct update_data {
        public string name;
        public string path;
        public int majorVersion;
        public int minorVersion;
        public int revisionVersion;
    }

    class updateHandler
    {
        private update_data version_compatibility;
        private update_data program_version;
        public update_data latest_program_version;
        private update_data updates_file_version;
        public bool update_program;
        public Dictionary<string,update_data> existing_data = new Dictionary<string,update_data>();
        public Dictionary<string,update_data> new_data = new Dictionary<string,update_data>();
        private Dictionary<string,string> update_us = new Dictionary<string,string>();
        private ArrayList update_sources = new ArrayList();


        public updateHandler() {
            update_program = false;
            // This restricts what file versions can be used by the program
            // It is very important that this gets updated for new versions
            version_compatibility.majorVersion = 1;
            version_compatibility.minorVersion = 0;

            program_version.majorVersion = 0;
            program_version.minorVersion = 8;
            program_version.revisionVersion = 0;

            string game_configs = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"data");
            if(Directory.Exists(game_configs)) {
                FileInfo[] read_us;
	            DirectoryInfo read_me = new DirectoryInfo(game_configs);
                string current_file = "No File";
                try {
                    read_us = read_me.GetFiles("*.xml");
                    int i = 1;
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
                                        addfileVersion(major_version,minor_version,revision_version,Path.Combine("data",me_me.Name),me_me.FullName);
                                        continue;
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
        
        
        
        }


        private void addfileVersion(int newMajor, int newMinor, int newRev, string newFile, string newPath) {
            update_data new_file;
            new_file.majorVersion = newMajor;
            new_file.minorVersion = newMinor;
            new_file.revisionVersion = newRev;
            new_file.name = newFile;
            new_file.path = newPath;
            if(existing_data.ContainsKey(newFile)) {
                existing_data[newFile] = new_file;
            } else {
                existing_data.Add(newFile,new_file);
            }
        }

        public bool checkVersions() {
            bool return_me = false;
            if(checkConnection()) {
                string updates_file = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "updates.xml");
                if(File.Exists(updates_file)) {
                    XmlReaderSettings xml_settings = new XmlReaderSettings();
                    xml_settings.ConformanceLevel = ConformanceLevel.Document;
                    xml_settings.IgnoreComments = true;
                    xml_settings.IgnoreWhitespace = true;
                    XmlReader load_me = XmlReader.Create(updates_file);
                    while (load_me.Read()) {
        			    if(load_me.NodeType==XmlNodeType.Element) {
				            switch (load_me.Name) {
                                case "updates":
                                    while(load_me.MoveToNextAttribute()) {
				                        switch (load_me.Name) {
                                            case "majorVersion":
                                                    updates_file_version.majorVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                            case "minorVersion":
                                                    updates_file_version.minorVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                            case "revisionVersion":
                                                    updates_file_version.revisionVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                        }
                                    }
                                    break;
                                case "source":
                                    update_sources.Add(load_me.ReadString());
                                    break;
                            }
                        }
                    }

                    load_me.Close();
                    addfileVersion(updates_file_version.majorVersion,updates_file_version.minorVersion,updates_file_version.revisionVersion,"updates.xml",updates_file);


                    WebClient Client = new WebClient();
                    update_data new_file;
                    new_file.majorVersion = 0;
                    new_file.minorVersion = 0;
                    new_file.revisionVersion = 0;
                    new_file.path = "";
                    new_file.name = "";



                    foreach(string update_source in update_sources) {
                        try{
                            Stream updates = Client.OpenRead(update_source);
                            load_me = XmlReader.Create(updates);
                            while (load_me.Read()) {
        			            if(load_me.NodeType==XmlNodeType.Element&&load_me.Name=="file") {
                                    while(load_me.MoveToNextAttribute()) {
                                        switch(load_me.Name) {
                                            case "name":
                                                new_file.name = load_me.Value;
                                                break;
                                            case "url":
                                                new_file.path = load_me.Value;
                                                break;
                                            case "majorVersion":
                                                new_file.majorVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                            case "minorVersion":
                                                new_file.minorVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                            case "revisionVersion":
                                                new_file.revisionVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                        }
                                    }
        			            } else if(load_me.NodeType==XmlNodeType.Element&&load_me.Name=="program") {
                                    while(load_me.MoveToNextAttribute()) {
                                        switch(load_me.Name) {
                                            case "url":
                                                latest_program_version.path = load_me.Value;
                                                break;
                                            case "majorVersion":
                                                latest_program_version.majorVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                            case "minorVersion":
                                                latest_program_version.minorVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                            case "revisionVersion":
                                                latest_program_version.revisionVersion = Convert.ToInt32(load_me.Value);
                                                break;
                                        }
                                    }
                                } else {
                                    continue;
                                }

                                if(new_file.majorVersion!=version_compatibility.majorVersion||new_file.minorVersion!=version_compatibility.minorVersion)
                                    continue;

                                if(existing_data.ContainsKey(new_file.name)) {
                                    if(new_data.ContainsKey(new_file.name)) {
                                        if(new_data[new_file.name].revisionVersion<new_file.revisionVersion) {
                                            new_data[new_file.name] = new_file;
                                        }
                                    } else {
                                        new_data.Add(new_file.name,new_file);
                                    }
                                }
                            }
                        } catch(WebException exception) {
                            if(exception.Message.Contains("404")) {
                                MessageBox.Show("The latest update file could not be found. It probably moved, and you're probably going to have to install a newer version of MASGAU.","We never planned for this...",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            } else {
                                MessageBox.Show("There was an error while downloading the latest update file:" + Environment.NewLine + exception.Message,"The cloud has failed us",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            }
                            return return_me;
                        }
                    }

                    foreach(update_data update_me in new_data.Values) {
                        if(update_me.revisionVersion>existing_data[update_me.name].revisionVersion) {
                            return_me = true;
                        }
                    } 
                    if(latest_program_version.majorVersion>program_version.majorVersion) {
                        return_me = true;
                        update_program = true;
                    } else {
                        if(latest_program_version.majorVersion==program_version.majorVersion&&
                            latest_program_version.minorVersion>program_version.minorVersion) {
                            return_me = true;
                            update_program = true;
                        } else {
                            if(latest_program_version.majorVersion==program_version.majorVersion&&
                                latest_program_version.minorVersion==program_version.minorVersion&&
                                latest_program_version.revisionVersion>program_version.revisionVersion) {
                                return_me = true;
                                update_program = true;
                            }
                        }
                    }
                } else {
                    MessageBox.Show("The updates.xml file couldn't be found. You probably need to re-install.","Not cool",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            } else {
                Console.WriteLine("No Internet Connection");
            }
            return return_me;
        }
        private static void validationHandler(object sender, ValidationEventArgs args){
            throw new XmlException(args.Message);
        }
        private bool checkConnection() {
            try
            {
            System.Net.Sockets.TcpClient clnt=new System.Net.Sockets.TcpClient("www.google.com",80);
            clnt.Close();
            return true;
            }
            catch
            {
            return false;
            }
        }
    }
}
