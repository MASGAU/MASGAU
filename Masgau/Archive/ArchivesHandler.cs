using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using MASGAU.Location;
using MASGAU.Game;
using MASGAU.Communication.Progress;
using MASGAU.Communication.Message;

namespace MASGAU.Archive
{
    public class ArchivesHandler: Model<ArchiveID,ArchiveHandler> {
        public Boolean no_archives_detected
        {
            get
            {
                if(Core.settings.backup_path_set) {
                    return this.Count == 0;
                } else {
                    return false;
                }
            }
        }

        public ArchiveHandler get(GameID id, String owner, String type) {
            ArchiveID find_me = new ArchiveID(id, owner,type);
            return get(find_me);
        }

        public void detectBackups() {
            this.Clear();
            if(!Core.settings.backup_path_set)
                return;
            ProgressHandler.progress_state = ProgressState.Normal;
            string path = null;
            path = Core.settings.backup_path;
            FileInfo[] read_us = new DirectoryInfo(path).GetFiles("*.gb7");

            ProgressHandler.progress = 0;
            if(read_us.Length>0) {
                ProgressHandler.progress_max = read_us.Length;
                foreach(FileInfo read_me in read_us) {
                    ProgressHandler.progress++;
                    ProgressHandler.progress_message = "Scanning Backups (" + ProgressHandler.progress + "/" + read_us.Length + ")";
                            
                    try {
                        ArchiveHandler add_me = new ArchiveHandler(read_me);
                        if(add_me!=null) {
                            this.AddWithSort(add_me);
                        }
                    } catch (Exception e) {
                        MessageHandler.SendException(e);
                    }

                }
            }

            ProgressHandler.progress_state = ProgressState.None;
            ProgressHandler.progress = 0;
        }


    }
}
