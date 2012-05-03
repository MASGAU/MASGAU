using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Xml;
using MVC;
using Translations;
namespace Config
{

    public class ConfigFileHandler: AModelItem
    {
        protected System.Threading.Mutex mutex = null;

        public const char                   value_divider = '|';
        private XmlDocument                 config;
        private FileStream                  config_stream;
        protected string                    file_path = null, 
                                            file_name = null;
        protected string file_full_path {
            get {
                return Path.Combine(file_path,file_name);
            }
        }
        private bool                        config_ready;

        bool enable_writing = true;

        protected List<string> shared_settings = new List<string>();

        private FileSystemWatcher _config_watcher;
        protected FileSystemWatcher config_watcher {
            get {
                return _config_watcher;
            }
            set {
                _config_watcher = value;
            }
        }

        protected ConfigFileHandler(string new_file_path, string new_file_name, System.Threading.Mutex mutex): this(new_file_path, new_file_name) {
            this.mutex = mutex;
        }
        protected ConfigFileHandler(string new_file_path, string new_file_name): base(new_file_name) {
            file_path = new_file_path;
            file_name = new_file_name;
        }

        protected void lockFile() {
            enable_writing = false;
            if(mutex!=null)
                mutex.WaitOne();
        }

        protected void releaseFile() {
            if(mutex!=null)
                mutex.ReleaseMutex();
            enable_writing = true;
        }
        protected void setupWatcher() {
            if(config_watcher==null) {
                config_watcher = new FileSystemWatcher(file_path);
                config_watcher.Changed += new FileSystemEventHandler(config_watcher_Changed);
            }
            if(!config_watcher.EnableRaisingEvents)
                config_watcher.EnableRaisingEvents = true;
        }

        protected virtual void config_watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if(e.ChangeType== WatcherChangeTypes.Changed&&e.Name==file_name) {
                try {
                    loadSettings();
                } catch (Exception ex) {
                    throw new TranslateableException("SettingsWatcherError",ex);
                }
            }
        }


        protected bool ready {
            get {
               return config_ready;
            }
        }

        private void createConfig() {
            XmlTextWriter write_here = new XmlTextWriter(file_full_path, System.Text.Encoding.UTF8);
            write_here.Formatting = Formatting.Indented;
            write_here.WriteProcessingInstruction("xml","version='1.0' encoding='UTF-8'");
            write_here.WriteStartElement("config");
            write_here.Close();
        }

        protected virtual void loadSettings() {
            lockFile();

            if(config_watcher!=null) {
                config_watcher.EnableRaisingEvents = false;
                config_watcher.Dispose();
                config_watcher = null;
            }
            for(int i = 0;i<=5;i++) {
                // An optimistic initializer
                config_ready = true;
                try {
                    if(!Directory.Exists(file_path))
                        Directory.CreateDirectory(file_path);

                    if(!File.Exists(file_full_path)) {
                        createConfig();
                    }

                    config_stream = new FileStream(file_full_path,FileMode.OpenOrCreate,FileAccess.ReadWrite);
                    break;
                } catch (Exception e) {
                    config_ready = false;
                    if(i<5)
                        System.Threading.Thread.Sleep(100);
                    else
                        throw new TranslateableException("ConfigOpenError", e);
                } finally {
                    if(config_stream!=null)
                        config_stream.Close();
                }
            }
            
            XmlReaderSettings xml_settings = new XmlReaderSettings();
            xml_settings.ConformanceLevel = ConformanceLevel.Document;
            xml_settings.IgnoreComments = true;
            xml_settings.IgnoreWhitespace = true;
            config = new XmlDocument();
            lock(config_stream) {
                config_stream = new FileStream(file_full_path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                XmlReader reader = XmlReader.Create(config_stream, xml_settings);
                try {
                    config.Load(reader);
                } catch (XmlException e) {
                    if(e.Message.StartsWith("Root element is missing")) {
                        reader.Close();
                        config_stream.Close();
                        createConfig();
                        config_stream = new FileStream(file_full_path,FileMode.OpenOrCreate,FileAccess.ReadWrite);
                        reader = XmlReader.Create(config_stream, xml_settings);
                        config.Load(reader);
                    } else {
                        config_ready = false;
                        throw new TranslateableException("ConfigXMLError", e);
                    }
                } finally {
                    reader.Close();
                    config_stream.Close();
                }
            }

            foreach(string setting in shared_settings) {
                NotifyPropertyChanged(setting);
            }

            setupWatcher();

            releaseFile();
        }
        protected void purgeConfig() {
            lockFile();
            try {
                config_stream = new FileStream(file_full_path,FileMode.Truncate,FileAccess.Read);
            } catch (Exception e) {
                throw new TranslateableException("ConfigPurgeError", e);
            } finally {
                config_stream.Close();
            }
            releaseFile();
        }
        private bool writeConfig() {
            lockFile();
            lock(config_stream) {
                config_watcher.EnableRaisingEvents = false;
                config_stream = new FileStream(file_full_path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                lock(config) {
                    config.Save(config_stream);
                }
                config_stream.Close();
                config_watcher.EnableRaisingEvents = true;
            }
            releaseFile();
            return true;
        }
        private XmlElement getNode(bool create, params string[] children) {
            if(!config.HasChildNodes)
                return null;

            XmlElement nodes = config.ChildNodes[1] as XmlElement;
            foreach(string child in children) {
                bool found = false;
                foreach(XmlElement node in nodes.ChildNodes) {
                    if(node.Name==child) {
                        nodes = node;
                        found = true;
                        break;
                    }
                }
                if(!found) {
                    if(create) {
                        XmlElement new_node = config.CreateElement(child);
                        nodes.AppendChild(new_node); 
                        nodes = new_node;
                    } else {
                        nodes = null;
                        break;
                    }
                }
            }
            return nodes;
        }



        protected List<string> getNodeGroupValues(string name, params string[] names) {
            List<string> return_me = new List<string>();;
            lock(config) {
                XmlElement node = getNode(false, names);
                if(node==null)
                    return return_me;
                foreach(XmlElement child in node.ChildNodes) {
                    if(child.Name==name)
                        return_me.Add(child.InnerText);
                }
            }
            return return_me;
        }
        protected bool setNodeGroupValues(string name, List<string> values, params string[] names) {
            lock(config) {
                XmlElement node = getNode(false, names);
                if(node!=null)
                    node.ParentNode.RemoveChild(node);

                node = getNode(true, names);

                foreach(string value in values) {
                    XmlElement element = config.CreateElement(name);
                    element.InnerText = value;
                    node.AppendChild(element);
                }
            }
            return writeConfig();
        }
        protected string getNodeValue(params string[] names) {
            lock(config) {
                XmlElement node = getNode(false, names);
                if(node==null)
                    return null;
                return node.InnerText;
            }
        }
        protected string getNodeAttribute(string attribute, params string[] name) {
            lock(config) {
                XmlElement node = getNode(false,name);
                if(node==null)
                    return null;

                if(!node.HasAttribute(attribute))
                    return null;

                return node.GetAttribute(attribute);
            }
        }

        protected bool setNodeValue(string value, params string[] name) {
            lock(config) {
                XmlElement node = getNode(true,name);
                if(node==null)
                    return false;
                node.InnerText = value;
            }
            return writeConfig();
        }
        protected bool setNodeAttribute(string attribute, string value, params string[] name) {
            lock(config) {
                XmlElement node = getNode(true, name);
                if(node==null)
                    return false;
                node.SetAttribute(attribute,value);
            }
            return writeConfig();
        }
        protected XmlElement getSpecificNode(string name, bool create, params string[] attribs) {
            if(!config.HasChildNodes)
                return null;

            if(attribs.Length%2!=0)
                throw new Exception("An uneven number of identifying attribute values has been supplied");

            XmlElement return_me = null;
            XmlElement nodes = config.ChildNodes[1] as XmlElement;
            foreach(XmlElement node in nodes.ChildNodes) {
                if(node.Name!=name)
                    continue;

                return_me = node;
                for(int i = 0; i<attribs.Length;i++) {
                    if(!node.HasAttribute(attribs[i])) {
                        return_me = null;
                        break;
                    }
                    string attrib = node.GetAttribute(attribs[i]);
                    i++;
                    if(!attrib.Equals(attribs[i])) {
                        return_me = null;
                        break;
                    }
                }
                if(return_me!=null)
                    break;
            }
            if(return_me==null) {
                if(create) {
                    return_me = config.CreateElement(name);
                    for(int i = 0; i<attribs.Length;i++) {
                        return_me.SetAttribute(attribs[i],attribs[++i]);
                    }
                    nodes.AppendChild(return_me); 
                }
            }
            return return_me;
        }
        protected bool setSpecificNodeValue(string name, string value, params string[] attribs) {
            lock(config) {
                XmlElement node = getSpecificNode(name,true,attribs);
                if(node==null)
                    return false;
                node.InnerText = value;
            }
            return writeConfig();
        }
        protected bool setSpecificNodeAttrib(string name, string attrib_name, string attrib_value, params string[] attribs) {
            lock(config) {
                XmlElement node = getSpecificNode(name, true, attribs);
                if(node==null)
                    return false;
                node.SetAttribute(attrib_name,attrib_value);
            }
            return writeConfig();
        }
        protected string getSpecificNodeValue(string name, params string[] attribs) {
            lock(config) {
                XmlElement node = getSpecificNode(name, false,attribs);
                if(node==null)
                    return null;
                return node.InnerText;
            }
        }
        protected string getSpecificNodeAttribute(string name, string attrib, params string[] attribs) {
            lock(config) {
                XmlElement node = getSpecificNode(name,false,attribs);
                if(node==null)
                    return null;

                if(!node.HasAttribute(attrib))
                    return null;

                return node.GetAttribute(attrib);
            }
        }

        protected bool clearNode(params string[] names) {
            lock(config) {
                XmlElement node = getNode(false, names);
                if(node==null)
                    return true;

                node = node.ParentNode as XmlElement;
                string name = names[names.Length-1];
                for(int i = 0; i<node.ChildNodes.Count;i++) {
                    if(node.ChildNodes[i].Name==name) {
                        node.RemoveChild(node.ChildNodes[i]);
                        i--;
                    }
                }


            }
            return writeConfig();
        }
        protected bool clearSpecificNodeAttribute(string name, string attrib, params string[] attribs) {
            lock(config) {
                XmlElement node = getSpecificNode(name,false,attribs);
                if(node==null)
                    return false;

                if(!node.HasAttribute(attrib))
                    return false;

                node.RemoveAttribute(attrib);

                return writeConfig();
            }
        }
    }
}
