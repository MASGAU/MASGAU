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

        protected bool downloadHelper(string target) {
            string tmp_name = target.Substring(0, target.Length - 3) + "TMP";

            WebClient Client;
            Stream new_file;
            FileStream writer;
            Client = new WebClient();
            foreach (Uri url in URLs) {
                try {
                    new_file = Client.OpenRead(url);
                    writer = new FileStream(tmp_name, FileMode.Create, FileAccess.Write);

                    int Length = 256;
                    Byte[] buffer = new Byte[Length];
                    int bytesRead = new_file.Read(buffer, 0, Length);
                    while (bytesRead > 0) {
                        writer.Write(buffer, 0, bytesRead);
                        bytesRead = new_file.Read(buffer, 0, Length);
                    }

                    writer.Close();
                    new_file.Close();

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
                    File.Delete(tmp_name);
                    return false;
                }
            }

            return true;
        }

    }
}
