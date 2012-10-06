using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Collections;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MVC.Communication;
using MVC.Translator;
namespace MASGAU.Backup {
    public class BackupProgramHandler : AProgramHandler {
        public string archive_name_override = null;

        private List<GameEntry> back_these_up = null;
        private List<DetectedFile> only_these_files = new List<DetectedFile>();

        public BackupProgramHandler(List<GameEntry> backup_list, ALocationsHandler loc)
            : this(loc) {
            back_these_up = backup_list;
        }
        public BackupProgramHandler(GameEntry this_game, List<DetectedFile> only_these, string archive_name, ALocationsHandler loc)
            : this(loc) {
            back_these_up = new List<GameEntry>();
            back_these_up.Add(this_game);

            if (only_these != null) {
                only_these_files = only_these;
            }
            archive_name_override = archive_name;
        }
        public BackupProgramHandler(ALocationsHandler loc)
            : base(loc) {
            worker.DoWork += new DoWorkEventHandler(BackupProgramHandler_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackupProgramHandler_RunWorkerCompleted);
        }

        void BackupProgramHandler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            // Make this check for things errors so it can clean up after itself
            if (output_path == null)
                return;
            foreach (FileInfo delete_me in new DirectoryInfo(output_path).GetFiles("~*.tmp")) {
                try {
                    delete_me.Delete();
                } catch (Exception ex) {

                    TranslatingMessageHandler.SendError("DeleteError", ex, delete_me.Name);
                }
            }
        }

        string output_path;
        void BackupProgramHandler_DoWork(object sender, DoWorkEventArgs e) {

            if (Core.settings.IsBackupPathSet || archive_name_override != null) {
                if (archive_name_override != null)
                    output_path = Path.GetDirectoryName(archive_name_override);
                else
                    output_path = Core.settings.backup_path;


                IList<GameEntry> games;

                if (back_these_up != null && back_these_up.Count > 0) {
                    games = back_these_up;
                } else {
                    if (Games.detected_games_count == 0)
                        Games.detectGames();
                    games = Games.DetectedGames.Items;
                }

                if (games.Count > 0) {
                    ProgressHandler.value = 1;
                    ProgressHandler.max = games.Count;
                    TranslatingProgressHandler.setTranslatedMessage("GamesToBeBackedUpCount", games.Count.ToString());


                    foreach (GameEntry game in games) {
                        if (CancellationPending)
                            return;

                        //if(archive_name_override!=null)
                        //all_users_archive = new ArchiveHandler(new FileInfo(archive_name_override),game.id);

                        if (games.Count == 1) {
                            TranslatingProgressHandler.setTranslatedMessage("BackingUpSingleGame", game.Title);
                        } else {
                            TranslatingProgressHandler.setTranslatedMessage("BackingUpMultipleGames", game.Title, ProgressHandler.value.ToString(), games.Count.ToString());
                        }

                        List<DetectedFile> files;
                        if (only_these_files != null && only_these_files.Count > 0) {
                            files = only_these_files;
                        } else {
                            files = game.Saves.Flatten();
                            ;
                        }


                        Archive override_archive = null;

                        try {
                            DictionaryList<Archive, DetectedFile> backup_files = new DictionaryList<Archive, DetectedFile>();
                            foreach (DetectedFile file in files) {
                                ArchiveID archive_id;
                                Archive archive;
                                if (CancellationPending)
                                    return;

                                archive_id = new ArchiveID(game.id, file);

                                if (archive_name_override != null) {
                                    if (override_archive == null)
                                        file.Type = null;
                                    override_archive = new Archive(new FileInfo(archive_name_override), new ArchiveID(game.id, file));
                                    archive = override_archive;
                                } else {
                                    if (Archives.Get(archive_id) == null) {
                                        Archives.Add(new Archive(output_path, new ArchiveID(game.id, file)));
                                    }
                                    archive = Archives.Get(archive_id);
                                }

                                backup_files.Add(archive, file);
                            }
                            if (CancellationPending)
                                return;

                            foreach (KeyValuePair<Archive, List<DetectedFile>> backup_file in backup_files) {
                                if (override_archive == null)
                                    backup_file.Key.backup(backup_file.Value, false, false);
                                else
                                    backup_file.Key.backup(backup_file.Value, true, false);
                            }

                        } catch (Exception ex) {
                            TranslatingMessageHandler.SendException(ex);
                        } finally {
                            ProgressHandler.value++;
                        }
                    }
                } else {
                    TranslatingMessageHandler.SendError("NothingToBackup");
                }
            } else {
                TranslatingMessageHandler.SendError("BackupPathNotSet");
            }
        }

    }
}
