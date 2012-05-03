using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.ComponentModel;
using MASGAU.Game;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Archive;
using Communication;
using Communication.Message;
using Communication.Progress;
using Collections;

namespace MASGAU.Backup
{
    public class ABackupProgramHandler<L> : AProgramHandler<L> where L : ALocationsHandler
    {
        private new ArchivesHandler archives = null;

        public string               archive_name_override = null;

		private List<GameHandler>        back_these_up = null;
		private List<DetectedFile>  only_these_files = new List<DetectedFile>();

        public ABackupProgramHandler(List<GameHandler> backup_list, Interface new_iface): this(new_iface) {
            back_these_up = backup_list;
        }
        public ABackupProgramHandler(GameHandler this_game, List<DetectedFile> only_these, string archive_name, Interface new_iface): this(new_iface) {
            back_these_up = new List<GameHandler>();
            back_these_up.Add(this_game);

			if(only_these!=null) {
                only_these_files = only_these;
			}
            archive_name_override  = archive_name;
        }

        public ABackupProgramHandler(Interface new_iface): base(new_iface) {
            archives = new ArchivesHandler();
            this.DoWork += new DoWorkEventHandler(BackupProgramHandler_DoWork);
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackupProgramHandler_RunWorkerCompleted);
        }

        void BackupProgramHandler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Make this check for things errors so it can clean up after itself
            if(output_path==null)
                return;
            foreach(FileInfo delete_me in new DirectoryInfo(output_path).GetFiles("~*.tmp")) {
                try {    
                    delete_me.Delete();
                } catch(Exception ex) {
                    MessageHandler.SendError("Final Cleanup","An error occured while trying to delete " + delete_me.Name, ex);
                }
            }
        }

        string output_path;
        void BackupProgramHandler_DoWork(object sender, DoWorkEventArgs e)
        {
            if(Core.settings.backup_path_set||archive_name_override!=null) {
                if(archive_name_override!=null)
                    output_path = Path.GetDirectoryName(archive_name_override);
                else
                    output_path = Core.settings.backup_path;


                List<GameHandler> games;

                if(back_these_up!=null&&back_these_up.Count>0) {
                    games = back_these_up;
                } else {
                    if(Core.games.enabled_games_count==0)
                        Core.games.detectGames();
                    games = Core.games.enabled_games;
                }

                if (games.Count> 0) {
                    ProgressHandler.value = 1;
                    ProgressHandler.max = games.Count;
                    ProgressHandler.setTranslatedMessage("GamesToBeBackedUpCount", games.Count.ToString());


                    foreach(GameHandler game in games) {
                        if(CancellationPending)
                            return;

                        //if(archive_name_override!=null)
                            //all_users_archive = new ArchiveHandler(new FileInfo(archive_name_override),game.id);

                        if(games.Count==1) {
                            ProgressHandler.setTranslatedMessage("BackingUpSingleGame", game.title);
                        } else {
                            ProgressHandler.setTranslatedMessage("BackingUpMultipleGames", ProgressHandler.value.ToString(),games.Count.ToString(), game.title);
                        }

                        List<DetectedFile> files;
                        if(only_these_files!=null&&only_these_files.Count>0) {
                            files = only_these_files;
                        } else  {
                            files = game.getSaves().Flatten(); ;
                        }


                        ArchiveHandler override_archive = null;

                        try {
                            DictionaryList<ArchiveHandler,DetectedFile> backup_files = new DictionaryList<ArchiveHandler,DetectedFile>();
                            foreach(DetectedFile file in files){
                                ArchiveID archive_id;
                                ArchiveHandler archive;
                                if(CancellationPending)
                                    return;

                                archive_id = new ArchiveID(game.id,file.owner,file.type);

                                if(archive_name_override!=null) {
                                    if(override_archive==null)
                                        override_archive = new ArchiveHandler(new FileInfo(archive_name_override),new ArchiveID(game.id,file.owner,null));
                                    archive = override_archive;
                                } else {
                                    if(archives.get(archive_id)==null) {
                                        if(file.owner!=null) {
                                            archives.Add(new ArchiveHandler(output_path,new ArchiveID(game.id,file.owner,file.type)));
                                        } else {
                                            archives.Add(new ArchiveHandler(output_path,new ArchiveID(game.id,null,file.type)));
                                        }
                                    }
                                    archive = archives.get(archive_id);
                                }
                                
                                backup_files.Add(archive,file);
                            }
                            if(CancellationPending)
                                return;

                            foreach(KeyValuePair<ArchiveHandler,List<DetectedFile>> backup_file in backup_files) {
                                if(override_archive==null)
                                    backup_file.Key.backup(backup_file.Value, false);
                                else 
                                    backup_file.Key.backup(backup_file.Value, true);
                            }

                        } catch (Exception ex) {
                            MessageHandler.SendException(ex);
                        } finally {
                            ProgressHandler.value++;
                        }
                    }
                } else {
                    MessageHandler.SendError("It feels like there's","Nothing at all to back up");
                }
            } else {
                MessageHandler.SendError("Can't continue","Backup path not set. Please set it from the main program.");
            }
        }

    }
}
