using System;
using System.Collections.Generic;
using System.IO;
using MVC.Communication;
using Communication.Translator;
using MVC;
namespace MASGAU {
    public class Archives : StaticModel<ArchiveID, Archive> {
        private Archives() { }

        public static Boolean NoArchivesDetected {
            get {
                if (Core.settings.IsBackupPathSet) {
                    return model.Count == 0;
                } else {
                    return false;
                }
            }
        }

        public static IList<Archive> GetArchives(GameID id) {
            IList<Archive> archives = new List<Archive>();
            foreach (Archive archive in model.Items) {
                if (archive.id.Game.Equals(id))
                    archives.Add(archive);
            }
            return archives;
        }

        public static Archive GetArchive(GameID id, String owner, String type, QuickHash hash) {
            ArchiveID find_me = new ArchiveID(id, owner, type, hash);
            return model.get(find_me);
        }

        public static void DetectBackups() {
            model.Clear();
            if (!Core.settings.IsBackupPathSet)
                return;
            ProgressHandler.state = ProgressState.Normal;
            string path = null;
            path = Core.settings.backup_path;
            FileInfo[] read_us = new DirectoryInfo(path).GetFiles("*.gb7");

            ProgressHandler.value = 0;
            if (read_us.Length > 0) {
                ProgressHandler.max = read_us.Length;
                foreach (FileInfo read_me in read_us) {
                    ProgressHandler.value++;
                    TranslatingProgressHandler.setTranslatedMessage("LoadingArchives", ProgressHandler.value.ToString(), read_us.Length.ToString());

                    try {
                        Archive add_me = new Archive(read_me);
                        if (add_me != null) {
                            model.AddWithSort(add_me);
                        }
                    } catch (Exception e) {
                        TranslatingMessageHandler.SendException(e);
                    }

                }
            }

            ProgressHandler.state = ProgressState.None;
            ProgressHandler.value = 0;
        }


    }
}
