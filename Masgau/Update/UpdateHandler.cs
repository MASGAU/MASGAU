using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Net;
using System.ComponentModel;
using MASGAU.Communication.Message;

using System.Diagnostics;

namespace MASGAU.Update
{
    public class UpdateHandler: AModelItem {

        public string name { get; set;}

        // This restricts what file versions can be used by the program
        // It is very important that this gets updated for new versions
        private string current_version_path;
        private UpdateVersion current_version;
        public string current_version_string {
            get{
                return current_version.ToString();
            }
        }

        public List<string> latest_version_urls;
        private UpdateVersion latest_version;
        public string latest_version_string {
            get{
                return latest_version.ToString();
            }
        }
        
        protected UpdateHandler(String name): base(name) {
            this.name = name;
            latest_version = new UpdateVersion(0, 0, 0);
        }

        protected UpdateHandler(string name, string path): this(name)
        {
            this.current_version_path = path;
        }

        public UpdateHandler(UpdateVersion current_version, string name, string path): this(name,path) {
            this.current_version = current_version;
        }

        // This is the constructor for when reading the current data files
        public UpdateHandler(XmlElement element, string name, string path): this(name,path) {
            current_version = UpdateVersion.getVersionFromXml(element);
        }


        public void setLatestVersion(XmlElement element) {
            UpdateVersion test = UpdateVersion.getVersionFromXml(element);

            if(latest_version.CompareTo(test)>0)
                return;


            string latest_version_url;
            if(element.HasAttribute("url")) {
                latest_version_url = element.GetAttribute("url");
            } else {
                throw new MException("Update XML Error","url attribute missing in " + name,true);
            }

            if(latest_version.CompareTo(test)<0)
                latest_version_urls = new List<string>();

            latest_version_urls.Add(latest_version_url);
            latest_version = test;

            NotifyPropertyChanged("latest_version_string");
        }

        public bool needs_update {
            get {
                return latest_version.CompareTo(current_version)>0;
            }
        }
        private bool? _updating = false;
        public bool? updating {
            get { 
                if(needs_update) {
                    return _updating;
                } else {
                    return null;
                }
            }
            set { 
                _updating = value;
                NotifyPropertyChanged("updating");
            }
        }
        

        public void update() {
            string tmp_name = current_version_path.Substring(0,current_version_path.Length-3) + "TMP";
            WebClient Client;
            Stream new_file;
            FileStream writer;
            Client = new WebClient();

            updating = true;

            foreach(string latest_version_url in latest_version_urls) {
                try {
                    new_file = Client.OpenRead(latest_version_url);
                    writer = new FileStream(tmp_name,FileMode.Create,FileAccess.Write);

                    int Length = 256;
                    Byte [] buffer = new Byte[Length];
                    int bytesRead = new_file.Read(buffer,0,Length);
                    while( bytesRead > 0 ) {
                        writer.Write(buffer,0,bytesRead);
                        bytesRead = new_file.Read(buffer,0,Length);
                    }

                    writer.Close();
                    new_file.Close();

                    if(File.Exists(current_version_path))
                        File.Delete(current_version_path);
                    File.Move(tmp_name,current_version_path);
                    NotifyPropertyChanged("needs_update");
                    current_version = latest_version;
                    break;
                } catch(WebException exception)  {
                    File.Delete(tmp_name);
                    MessageHandler.SendError("Getting Old",name + " failed to download. Here's why:" + Environment.NewLine + latest_version_url,exception);
                }
            }
            updating = false;
            NotifyPropertyChanged("updating");
            NotifyPropertyChanged("needs_updating");
            NotifyPropertyChanged("current_version_string");
        }
    }
}
