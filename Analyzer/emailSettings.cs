using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Forms;


namespace MASGAU
{
    class emailSettings
    {
        public string email = null;
        public string smtp = null;
        private string config_path = null, config_file = null;
        private FileStream config_stream;
        private invokes invokes = new invokes();

        public emailSettings(Form mommy) {
            bool config_found = false;
            if (File.Exists(Path.Combine(Application.StartupPath,"analyzer_config.xml"))){
                config_path = Application.StartupPath;
                config_stream = new FileStream(Path.Combine(config_path, "analyzer_config.xml"),FileMode.Append,FileAccess.Write);
                if(config_stream.Length>0) {
                    config_found = true;
                } else {
                    config_found = false;
                }
                config_stream.Close();
            } else {
                config_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),".masgau");

                if(!Directory.Exists(config_path)) {
                    try {
                        Directory.CreateDirectory(config_path);
                    } catch(Exception exception) {
                        invokes.showMessageBox(mommy,"Can't Go On Like This","The config path couldn't be opened for writing." + Environment.NewLine + "MASGAU can't work without that." + Environment.NewLine + config_path + Environment.NewLine + exception.Message,MessageBoxButtons.OK,MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
                config_file = Path.Combine(config_path,"analyzer_config.xml");
                if (File.Exists(config_file)){
                    try {
                        config_stream = new FileStream(config_file,FileMode.Append,FileAccess.Write);
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
                    try {
                        config_stream = new FileStream(config_file,FileMode.Create,FileAccess.Write);
                        config_found = false;
                        config_stream.Close();
                    } catch (Exception exception) {
                        invokes.showMessageBox(mommy,"Can't Continue Like This","The config file couldn't be opened for writing." + Environment.NewLine + "MASGAU can't work without that." + Environment.NewLine + Path.Combine(config_path, "config.xml") + Environment.NewLine + exception.Message,MessageBoxButtons.OK,MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }

            }

            if(config_found) {
                XmlReaderSettings xml_settings = new XmlReaderSettings();
                xml_settings.ConformanceLevel = ConformanceLevel.Document;
                xml_settings.IgnoreComments = true;
                xml_settings.IgnoreWhitespace = true;
                lock(config_stream) {
                    config_stream = new FileStream(config_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    XmlReader load_me = XmlReader.Create(config_stream,xml_settings);
                    while (load_me.Read()) {
    			        if(load_me.NodeType==XmlNodeType.Element) {
				            switch (load_me.Name) {
                                case "email":
                                    email = load_me.ReadString();
                                    break;
                            }
                        }
                    }
                    load_me.Close();
                    config_stream.Close();
                }
            }

        }

        public void writeConfig() {
            XmlDocument write_me = new XmlDocument();
            XmlElement node;
            //XmlAttribute attribute;
            lock(config_stream) {
                if(File.Exists(Path.Combine(config_path,"analyzer_config.xml"))) {
                    File.Delete(Path.Combine(config_path,"analyzer_config.xml"));
                }

                XmlTextWriter write_here = new XmlTextWriter(Path.Combine(config_path,"analyzer_config.xml"), System.Text.Encoding.UTF8);
                write_here.Formatting = Formatting.Indented;
                write_here.WriteProcessingInstruction("xml","version='1.0' encoding='UTF-8'");
                write_here.WriteStartElement("analyzer_config");
                write_here.Close();

                write_me.Load(Path.Combine(config_path,"analyzer_config.xml"));
                XmlNode root = write_me.DocumentElement;

                if(email!=null&&email!="") {
                    node = write_me.CreateElement("email");
                    node.InnerText = email;
                    write_me.DocumentElement.InsertAfter(node,write_me.DocumentElement.LastChild);
                }

                config_stream = new FileStream(config_path + "\\analyzer_config.xml", FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                write_me.Save(config_stream);
                write_here.Close();
                config_stream.Close();
            }

        }

        public bool checkConnection() {
            try
            {
            System.Net.Sockets.TcpClient clnt=new System.Net.Sockets.TcpClient("smtp.gmail.com",587);
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
