using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.ComponentModel;
using MASGAU.LocationHandlers;

namespace MASGAU.ArchiveHandlers {

public class BackupHandler {
    private string archive_name;
    private GameId game;
    public List<DetectedFile> saves = new List<DetectedFile>();
    public bool creating_file = false;
    private Dictionary<string, DateTime> modified_times = new Dictionary<string, DateTime>();
    private Dictionary<string, DateTime> created_times = new Dictionary<string, DateTime>();
    private Dictionary<string, int> files_added = new Dictionary<string,int>();

    public BackupHandler(GameId game_id, string archive_name, List<DetectedFile> saves) {
        game = game_id;
        this.archive_name = archive_name;
        this.saves = saves;
    }

    public bool error_sent;
    public DetectedFile archiveIt(string force_name) {
        error_sent = false;
        DetectedFile return_me  = null;
        if(saves.Count>0)
            return_me = saves[0];
        if(ready) {
            string temp = Path.Combine(Path.GetTempPath(),"MASGAUArchiving");
            string from_here, to_here, in_here;
            String[] source = new String[1];
            DirectoryInfo read_me = new DirectoryInfo(temp);
            DateTime right_now = DateTime.Now;
            
            Dictionary<string, string> add_us = new Dictionary<string, string>();

            if(Directory.Exists(temp)) {
                try {
                    Directory.Delete(temp, true);
                } catch(Exception e) {
                     throw new MException("Every Day Is?","An error occured while trying to delete " + temp,e.Message,false);
                }
            }
            if (saves.Count>=2) {
                ProgressHandler.progress_max = saves.Count;
                ProgressHandler.progress = 1;
            } else {
                ProgressHandler.progress_max = saves.Count;
                ProgressHandler.progress = saves.Count;
            }
            string full_path;

        }
        clearArchive();
        return return_me;
    }
    public void cleanUp(string backup_path) {
        if(backup_path!=null) {
            foreach(FileInfo delete_me in new DirectoryInfo(backup_path).GetFiles("*.tmp*")) {
                try {
                    delete_me.Delete();
                } catch(Exception e) {
                    MessageHandler.SendError("It's hard coming up with titles","An error occured while trying to delete " + delete_me.Name, e.Message);
                }
            }
            if(creating_file) {
                try {
                    File.Delete(Path.Combine(output_path,file_name));
                } catch(Exception e) {
                    MessageHandler.SendError("I'm just taking it in","An error occured while trying to delete " + file_name,e.Message);
                }

            }
        }
        modified_times = new Dictionary<string, DateTime>();
        created_times = new Dictionary<string, DateTime>();
    }
}
}