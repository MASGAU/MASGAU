using System;
using System.Collections.Generic;
using System.IO;
using MASGAU.Location.Holders;
using MVC;
using MVC.Communication;
using MVC.Translator;
namespace MASGAU {
    public class Archives : StaticModel<ArchiveID, Archive> {
        private Archives() { }

        private static bool _IgnoreRevisionValue = true;
        public static bool IgnoreRevisionValue
        {
            set
            {
                _IgnoreRevisionValue = value;
            }
        }


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
                if (archive.id.Game.Equals(id,_IgnoreRevisionValue))
                    archives.Add(archive);
            }
            return archives;
        }

        public static Archive GetArchive(GameID id, DetectedFile file) {
            ArchiveID find_me = new ArchiveID(id, file);
            return model.get(find_me);
        }

        public static void DetectBackups() {
            ProgressHandler.saveMessage();
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
                TranslatingProgressHandler.setTranslatedMessage("LoadingArchives", ProgressHandler.value.ToString(), read_us.Length.ToString());
                foreach (FileInfo read_me in read_us) {
                    ProgressHandler.value++;

                    try {
                        Archive add_me = new Archive(read_me);
                        if (add_me != null) {
                            model.AddWithSort(add_me);
                        }
						try {
							GameEntry game = Games.Get(add_me.id.Game);
							game.TriggerUpdate();
						} catch { }

                    } catch (Exception e) {
                        TranslatingMessageHandler.SendException(e);
                    }

                }
            }

            ProgressHandler.state = ProgressState.None;
            ProgressHandler.value = 0;
            ProgressHandler.restoreMessage();
        }


    }
}
