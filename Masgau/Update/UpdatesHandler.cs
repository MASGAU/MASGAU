using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using Communication.Translator;
using MVC;
using Translator;
namespace MASGAU.Update {

    public class UpdatesHandler : Model<UpdateHandler> {
        // The update urls
        private List<string> update_sources;

        private UpdateHandler program;

        public bool restart_required = false;
        public bool shutdown_required = false;
        public bool redetect_required = false;
        public bool update_available = false;

        public UpdatesHandler() {
            program = new UpdateHandler(Core.program_version, null, null);
        }


        #region Version adders
        private UpdateHandler getUpdateHandler(string name) {
            foreach (UpdateHandler update in this) {
                if (update.name == name)
                    return update;
            }
            return null;
        }
        #endregion

        public UpdateAvailability checkUpdates(bool updater_program, bool suppress_no_update_message) {
            GameXmlFiles xml;
            if (Games.XmlLoaded) {
                xml = Games.xml;
            } else {
                xml = new GameXmlFiles();
//                if (!xml.ready) {
  //                  xml.loadXml();
    //            } else {
        //            return UpdateAvailability.None;
      //          }

            }

            foreach (UpdateHandler check_me in xml.xml_file_versions) {
                this.Add(check_me);
            }

            if (!checkConnection())
                return UpdateAvailability.None;

            string updates_file = Path.Combine(Core.app_path, "updates.xml");

            if (!File.Exists(updates_file)) {
                shutdown_required = true;
                throw new TranslateableException("FileNotFoundCritical","updates.xml");
            }

            XmlReaderSettings xml_settings = new XmlReaderSettings();
            xml_settings.ConformanceLevel = ConformanceLevel.Document;
            xml_settings.IgnoreComments = true;
            xml_settings.IgnoreWhitespace = true;
            XmlDocument document = new XmlDocument();

            FileStream stream = new FileStream(updates_file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            XmlReader reader = XmlReader.Create(stream, xml_settings);
            try {
                document.Load(reader);
            } catch (XmlException e) {
                throw new TranslateableException("XMLFormatError", e, "updates.xml");
            } finally {
                reader.Close();
                stream.Close();
            }

            XmlElement updates_node = null;
            foreach (XmlNode node in document.ChildNodes) {
                if (node.Name == "updates") {
                    updates_node = node as XmlElement;
                }
            }

            if (updates_node == null) {
                throw new TranslateableException("XMLErrorMissingTag", "updates", updates_file);
            }

            //this.Add(new UpdateHandler(updates_node,"updates.xml",updates_file));

            update_sources = new List<string>();

            foreach (XmlElement element in updates_node.ChildNodes) {
                if (element.Name != "source")
                    continue;

                update_sources.Add(element.InnerText);
            }


            WebClient Client = new WebClient();

            foreach (string update_source in update_sources) {
                document = new XmlDocument();
                //stream = new FileStream(updates_file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                Stream updates;
                try {
                    updates = Client.OpenRead(update_source);
                    reader = XmlReader.Create(updates, xml_settings);
                    document.Load(reader);
                    updates.Close();
                } catch (WebException e) {
                    if (e.Message.Contains("404")) {
                        //SendWarning("404 - Awesome Not Found","Couldn't download fileversions.xml from " + update_source);
                    } else {
                        //SendWarning("The cloud has failed us","There was an error while downloading the update file " + update_source, e.Message);
                    }
                    continue;
                } catch (XmlException e) {
                    TranslatingMessageHandler.SendWarning("XMLFormatError", e, update_source);
                    continue;
                } finally {
                    reader.Close();
                }

                XmlElement files_node = null;
                foreach (XmlNode node in document.ChildNodes) {
                    if (node.Name == "files") {
                        files_node = node as XmlElement;
                    }
                }

                if (files_node == null) {
                    TranslatingMessageHandler.SendWarning("XMLErrorMissingTag","files", update_source);
                    continue;
                }

                foreach (XmlElement element in files_node.ChildNodes) {
                    switch (element.Name) {
                        case "program":
                            if(element.Attributes["stable"].Value==Core.stable)
                                program.setLatestVersion(element);
                            break;
                        case "file":
                            string name;
                            if (element.HasAttribute("name")) {
                                name = element.GetAttribute("name");
                            } else {
                                TranslatingMessageHandler.SendWarning("XMLErrorMissingAttribute", "name", "file", update_source);
                                continue;
                            }

                            UpdateHandler new_file = getUpdateHandler(name);
                            if (new_file == null) {
                                new_file = new UpdateHandler(new UpdateVersion(0, 0, 0), name, Path.Combine(Core.app_path, name));
                            }

                            //UpdateVersion test = UpdateVersion.getVersionFromXml(element);
                            //if(Core.update_compatibility.compatibleWith(test))
                            //continue;

                            new_file.setLatestVersion(element);
                            if (!this.Contains(new_file))
                                this.Add(new_file);

                            break;
                    }
                }
            }

            if (program.needs_update) {
                return UpdateAvailability.Program;
            }

            for (int i = 0; i < this.Count; i++) {
                UpdateHandler update_me = this[i];
                if (update_me.needs_update) {
                    if ((bool)update_me.update_me) {
                        update_available = true;
                    }
                } else {
                    this.RemoveAt(i);
                    i--;
                }
            }

            if (!update_available) {
                return UpdateAvailability.None;
            }

            return UpdateAvailability.Data;
        }

        public void downloadDataUpdates() {
            if (File.Exists(Core.programs.updater)) {
                ProcessStartInfo updater = new ProcessStartInfo();
                updater.FileName = Core.programs.updater;
                if (Core.portable_mode)
                    updater.Arguments = "-portable";

                if (!SecurityHandler.amAdmin()) {
                    updater.Verb = "runas";
                }
                using (Process p = Process.Start(updater)) {
                    p.WaitForExit();
                    if (p.ExitCode == 1)
                        redetect_required = true;
                }
                return;
            } else {
                throw new TranslateableException("FileNotFoundCritical",Core.programs.updater);
            }

        }

        public void downloadProgramUpdate() {
            System.Diagnostics.Process.Start(program.latest_version_urls[0]);
            shutdown_required = true;
        }

        private bool checkConnection() {
            try {
                System.Net.Sockets.TcpClient clnt = new System.Net.Sockets.TcpClient("www.google.com", 80);
                clnt.Close();
                return true;
            } catch {
                return false;
            }
        }

    }
}
