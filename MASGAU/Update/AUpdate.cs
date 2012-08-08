using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using MVC.Translator;
using MVC;
using XmlData;
namespace MASGAU.Update {
    public abstract class AUpdate: IComparable<AUpdate> {
        public DateTime Date { get; protected set; }
        public List<Uri> URLs { get; protected set; }

        protected AUpdate(XmlElement xml) {
            URLs = new List<Uri>();
            this.Date = DateTime.Parse(xml.Attributes["date"].Value);
            addURL(xml);
        }

        public void addURL(XmlElement xml) {
            if (xml.HasAttribute("url"))
                this.URLs.Add(new Uri(xml.Attributes["url"].Value));
            else
                throw new KeyNotFoundException();
        }
        public void addURL(AUpdate update) {
            this.URLs.AddRange(update.URLs);
        }

        public abstract bool UpdateAvailable { get; }
        public abstract int CompareTo(AUpdate update);
        public abstract string getName();
        public abstract bool Update();


        protected string downloadFile(Uri url) {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = WebRequestMethods.Http.Get;
            webRequest.KeepAlive = true;
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Stream remote_file = response.GetResponseStream();
            //remote_file.ReadTimeout = 10000;

            string tmp_name = System.IO.Path.GetTempFileName();

            FileStream local_file = new FileStream(tmp_name, FileMode.Create, FileAccess.Write);

            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = remote_file.Read(buffer, 0, Length);
            while (bytesRead > 0) {
                local_file.Write(buffer, 0, bytesRead);
                bytesRead = remote_file.Read(buffer, 0, Length);
            }

            local_file.Close();
            remote_file.Close();

            return tmp_name;
        }

        protected bool downloadHelper(string target) {
            string tmp_name = null;
            foreach (Uri url in URLs) {
                try {
                    tmp_name = downloadFile(url);

                    
                    XmlFile game_config;
                    try {
                        game_config = new XmlFile(new FileInfo(tmp_name), false);
                    } catch (Exception e) {
                        Logger.Logger.log("Error while downloading " + url.ToString());
                        Logger.Logger.log(e);
                        File.Delete(tmp_name);
                        continue;
                    }


                    if (File.Exists(target))
                        File.Delete(target);

                    File.Move(tmp_name, target);
                    break;
                } catch (Exception exception) {
                    Logger.Logger.log(exception);
                    if(File.Exists(tmp_name))
                        File.Delete(tmp_name);
                    return false;
                }
            }

            return true;
        }

    }
}
