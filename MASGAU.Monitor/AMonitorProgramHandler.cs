using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Collections;
using Communication;
using Communication.Translator;
using MASGAU.Location;
using MASGAU.Location.Holders;
using Translator;
namespace MASGAU.Monitor {

    public class AMonitorProgramHandler<L> : AProgramHandler<L> where L : ALocationsHandler {
        public static System.Threading.Mutex monitor_mutex = new System.Threading.Mutex(false, "MASGAUMonitor");

        public Dictionary<string, FileSystemWatcher> watchmen;
        private DictionaryList<string, GameID> paths_to_games;
        private DictionaryList<GameID, string> games_to_paths;
        //private Dictionary<GameID, string> sync_paths;

        private List<MonitorFile> backup_queue;


        private Thread _monitor_thread;
        public Thread monitor_thread {
            get {
                return _monitor_thread;
            }
            set {
                _monitor_thread = value;
            }
        }

        public FileSystemWatcher sync_watcher;

        public AMonitorProgramHandler(Interface new_iface)
            : base(new_iface) {
            this._program_title += " " + Strings.GetLabelString("Monitor");
        }


        public void stop() {
            this.CancelAsync();
            if (watchmen != null) {
                lock (watchmen) {
                    foreach (KeyValuePair<string, FileSystemWatcher> dispose_me in watchmen) {
                        dispose_me.Value.EnableRaisingEvents = false;
                        dispose_me.Value.Dispose();
                    }
                    watchmen.Clear();
                }
            }
            if (sync_watcher != null) {
                sync_watcher.EnableRaisingEvents = false;
                sync_watcher.Dispose();
                sync_watcher = null;
            }
            //monitor_mutex.ReleaseMutex();
        }

        ~AMonitorProgramHandler() {
            stop();
            monitor_mutex.ReleaseMutex();
        }
        private static Boolean mutex_acquired = false;
        protected override void doWork(object sender, DoWorkEventArgs e) {
            if (!mutex_acquired && !monitor_mutex.WaitOne(100)) {
                throw new TranslateableException("OnlyOneMonitorAtATime");
            }
            mutex_acquired = true;

            if (initialized) {
                Games.Clear();
            }

            base.doWork(sender, e);

            while (!Core.settings.IsBackupPathSet) {
                RequestReply reply = RequestHandler.Request(RequestType.BackupFolder);
                if (reply.cancelled) {
                    throw new TranslateableException("MonitorBackupPathNotSet");
                }
            }

            backup_queue = new List<MonitorFile>();

            watchmen = new Dictionary<string, FileSystemWatcher>();

            paths_to_games = new DictionaryList<string, GameID>();
            games_to_paths = new DictionaryList<GameID, string>();

            //if (Core.settings.auto_update && !Core.settings.already_updated) {
            //    Core.updater.checkUpdates(false, true);
            //    if (Core.updater.redetect_required) {
            //        Core.games.loadXml();
            //        if (Core.updater.shutdown_required)
            //            return;
            //    }
            //}

            TranslatingProgressHandler.setTranslatedMessage("MonitorDetectingSavePaths");

            Games.detectGames();

            TranslatingProgressHandler.setTranslatedMessage("MonitorDetectingSavePaths");

            lock (watchmen) {
                foreach (KeyValuePair<string, FileSystemWatcher> dispose_me in watchmen) {
                    dispose_me.Value.Dispose();
                }
                watchmen.Clear();
            }
            lock (paths_to_games) {
                paths_to_games.Clear();
            }

            lock (watchmen) {
                foreach (GameVersion game in Games.Items) {
                    // M1ake sure this doesn't pick up PSP paths!
                    if (!game.IsDetected)
                        continue;
                    string the_path;
                    foreach (KeyValuePair<string, DetectedLocationPathHolder> game_root in game.DetectedLocations) {
                        the_path = game_root.Value.full_dir_path;
                        if (!watchmen.ContainsKey(the_path)) {
                            paths_to_games.Add(the_path, game.id);

                            watchmen.Add(the_path, new FileSystemWatcher(the_path, "*"));
                            watchmen[the_path].IncludeSubdirectories = true;
                            watchmen[the_path].Created += new FileSystemEventHandler(changed);
                            watchmen[the_path].Changed += new FileSystemEventHandler(changed);
                            watchmen[the_path].Deleted += new FileSystemEventHandler(changed);
                            watchmen[the_path].Renamed += new RenamedEventHandler(changed);
                            if (game.IsMonitored)
                                watchmen[the_path].EnableRaisingEvents = true;
                            else
                                watchmen[the_path].EnableRaisingEvents = false;
                        }
                    }

                }
            }

            //if (Core.settings.any_sync_enabled) {
            //    if (!Core.settings.sync_path_set) {
            //        RequestHandler.Request(RequestType.SyncFolder);
            //    }

            //    if (Core.settings.sync_path_set) {
            //        DirectoryInfo sync_folder = new DirectoryInfo(Core.settings.sync_path);
            //        if (sync_folder.Exists) {
            //            populateSyncPaths();
            //            synchronize();

            //            sync_watcher = new FileSystemWatcher(sync_folder.FullName, "*");
            //            sync_watcher.IncludeSubdirectories = true;
            //            sync_watcher.Created += new FileSystemEventHandler(syncFolderChanged);
            //            sync_watcher.Changed += new FileSystemEventHandler(syncFolderChanged);
            //            sync_watcher.Renamed += new RenamedEventHandler(syncFolderChanged);
            //            sync_watcher.Deleted += new FileSystemEventHandler(syncFolderChanged);

            //            sync_watcher.EnableRaisingEvents = true;
            //        }
            //    }
            //}

            monitor_thread = new Thread(backupLoop);
            monitor_thread.Start();
        }

        //private void synchronize() {
        //    TranslatingProgressHandler.setTranslatedMessage("MonitorSynchronizingSaves");
        //    DirectoryInfo dir = new DirectoryInfo(Core.settings.sync_path);
        //    DirectoryInfo[] dirs = dir.GetDirectories();
        //    int count = dirs.Length;
        //    ProgressHandler.max = Core.games.enabled_games_count + count;
        //    ProgressHandler.value = 0;
        //    foreach (Game game in Core.games) {
        //        // Make sure this doesn't pick up PSP paths!
        //        if (!game.sync_available || !game.IsDetected || !game.sync_enabled)
        //            continue;

        //        DirectoryInfo sync = new DirectoryInfo(Path.Combine(Core.settings.sync_path, game.id.ToString()));
        //        if (!sync.Exists) {
        //            sync.Create();
        //            sync.Refresh();
        //        }

        //        if (!sync.Exists) {
        //            TranslatingMessageHandler.SendError("CouldNotCreateSyncFolder", sync.FullName);
        //            game.sync_enabled = false;
        //            continue;
        //        }

        //        ProgressHandler.value++;
        //        foreach (DetectedFile save in game.getSaves().Flatten()) {
        //            copyFile(save.abs_root, sync.FullName, Path.Combine(save.path, save.name));
        //        }
        //    }
        //    foreach (DirectoryInfo sync_dir in new DirectoryInfo(Core.settings.sync_path).GetDirectories()) {
        //        ProgressHandler.value++;
        //        Game game = getGameHandler(sync_dir.FullName);
        //        if (game == null || !game.backup_enabled || !sync_paths.ContainsKey(game.id))
        //            continue;

        //        string destination = sync_paths[game.id];
        //        synchronizeRecursor(sync_dir, destination);
        //    }
        //}

        //private void synchronizeRecursor(DirectoryInfo dir, string target_dir) {
        //    foreach (FileInfo file in dir.GetFiles()) {
        //        copyFile(dir.FullName, target_dir, file.Name);
        //    }
        //    foreach (DirectoryInfo new_dir in dir.GetDirectories()) {
        //        synchronizeRecursor(new_dir, Path.Combine(target_dir, new_dir.Name));
        //    }
        //}

        //private Game getGameHandler(string sync_root) {
        //    string[] split = sync_root.Split(Path.DirectorySeparatorChar);
        //    string game_id = split[split.Length - 1];
        //    foreach (Game test in Core.games) {
        //        if (test.id.ToString().Equals(game_id))
        //            return test;
        //    }
        //    return null;
        //}
        //private void populateSyncPaths() {
        //    sync_paths = new Dictionary<GameID, string>();
        //    foreach (Game game in Core.games) {
        //        if (!game.sync_enabled)
        //            continue;
        //        if (game.detected_locations.Count == 0) {
        //            continue;
        //            // Well...this would be interesting to see.
        //        }
        //        if (game.detected_locations.Count == 1) {
        //            foreach (KeyValuePair<string, DetectedLocationPathHolder> game_root in game.detected_locations) {
        //                if (Archive.canWrite(new DirectoryInfo(game_root.Value.full_dir_path))) {
        //                    sync_paths.Add(game.id, game_root.Value.full_dir_path);
        //                } else {
        //                    TranslatingMessageHandler.SendError("OnlySyncPathNotWritable", game.title,
        //                        game_root.Value.full_dir_path);
        //                    game.clearSyncPath();
        //                    game.sync_enabled = false;
        //                }
        //            }
        //        } else {
        //            bool found = false;
        //            if (game.has_sync_location) {
        //                foreach (KeyValuePair<string, DetectedLocationPathHolder> game_root in game.detected_locations) {
        //                    if (game_root.Value.full_dir_path == game.sync_location) {
        //                        if (Archive.canWrite(new DirectoryInfo(game.sync_location))) {
        //                            sync_paths.Add(game.id, game.sync_location);
        //                            found = true;
        //                            break;
        //                        } else {
        //                            RequestReply response = TranslatingRequestHandler.Request(RequestType.Question, "SyncPathNotWritableAnymore", game.title);
        //                            if (response.cancelled) {
        //                                game.clearSyncPath();
        //                                game.sync_enabled = false;
        //                                found = true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if (found)
        //                continue;

        //            List<string> options = new List<string>();
        //            foreach (KeyValuePair<string, DetectedLocationPathHolder> game_root in game.detected_locations) {
        //                options.Add(game_root.Value.full_dir_path);
        //            }
        //            bool retry = true;
        //            while (retry) {
        //                RequestReply reply = TranslatingRequestHandler.Request(RequestType.Choice, "ChooseSyncPath", options[0], options, game.title);
        //                if (reply.cancelled) {
        //                    game.clearSyncPath();
        //                    game.sync_enabled = false;
        //                    break;
        //                } else {
        //                    if (Archive.canWrite(new DirectoryInfo(reply.selected_option))) {
        //                        sync_paths.Add(game.id, reply.selected_option);
        //                        game.sync_location = reply.selected_option;
        //                        retry = false;
        //                    } else {
        //                        RequestReply response = TranslatingRequestHandler.Request(RequestType.Question, "SyncPathNotWritable");
        //                        if (response.cancelled) {
        //                            game.clearSyncPath();
        //                            game.sync_enabled = false;
        //                            retry = false;
        //                        } else {
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private void syncFolderChanged(Object sender, FileSystemEventArgs e) {
        //    string game_name = e.Name.Split(Path.DirectorySeparatorChar)[0];
        //    MonitorFile add_me = new MonitorFile();
        //    add_me.root = Path.Combine(Core.settings.sync_path, game_name);

        //    if (e.Name.Length == game_name.Length)
        //        add_me.path = "";
        //    else
        //        add_me.path = e.Name.Substring(game_name.Length + 1);

        //    add_me.change_type = e.ChangeType;
        //    add_me.origin = Origin.Sync;
        //    if (e.ChangeType == WatcherChangeTypes.Renamed) {
        //        try {
        //            add_me.old_path = ((RenamedEventArgs)e).OldName.Substring(game_name.Length + 1);
        //        } catch {
        //            add_me.old_path = null;
        //        }
        //    } else {
        //        add_me.old_path = null;
        //    }
        //    lock (backup_queue) {
        //        if (!backup_queue.Contains(add_me)) {
        //            backup_queue.Add(add_me);
        //        }
        //    }
        //}

        private void changed(Object sender, FileSystemEventArgs e) {
            MonitorFile add_me = new MonitorFile();
            add_me.root = (sender as FileSystemWatcher).Path;
            add_me.path = e.Name;
            add_me.change_type = e.ChangeType;
            add_me.origin = Origin.Game;
            if (e.ChangeType == WatcherChangeTypes.Renamed) {
                try {
                    add_me.old_path = ((RenamedEventArgs)e).OldName;
                } catch {
                    add_me.old_path = null;
                }
            } else {
                add_me.old_path = null;
            }
            lock (backup_queue) {
                if (!backup_queue.Contains(add_me)) {
                    backup_queue.Add(add_me);
                }
            }
        }

        public bool copyFile(string from_folder, string to_folder, string file) {
            FileInfo source = new FileInfo(Path.Combine(from_folder, file));
            FileInfo destination = new FileInfo(Path.Combine(to_folder, file));
            if (destination.Exists) {
                if (destination.LastWriteTime >= source.LastWriteTime) {
                    return false;
                }
            }
            if (!destination.Directory.Exists) {
                destination.Directory.Create();
                destination.Refresh();
            }
            if (!destination.Directory.Exists) {
                return false;
            }
            try {
                source.CopyTo(destination.FullName, true);
            } catch (UnauthorizedAccessException e) {

            }
            return true;
        }

        private void backupLoop() {
            int count;
            MonitorFile file;
            while (!CancellationPending) {
                lock (backup_queue) {
                    count = backup_queue.Count;
                }
                if (count == 0) {
                    // No new files? Take a nap.
                    try {
                        Thread.Sleep(1000);
                    } catch (Exception e) {
                        TranslatingMessageHandler.SendException(e);
                    }
                    continue;
                }

                lock (backup_queue) {
                    file = backup_queue[0];
                    backup_queue.RemoveAt(0);
                }

                //if (file.origin == Origin.Game) {
                    if (file.change_type == WatcherChangeTypes.Changed || 
                        file.change_type == WatcherChangeTypes.Created || 
                        file.change_type == WatcherChangeTypes.Renamed) {

                        if (!File.Exists(Path.Combine(file.root, file.path)))
                            continue;

                        lock (paths_to_games) {
                            foreach (GameID game in paths_to_games[file.root]) {
                                if (!Games.Get(game).IsMonitored)
                                    continue;

                                List<DetectedFile> these_files = Games.Get(game).GetSavesMatching(Path.Combine(file.root, file.path));
                                if (these_files.Count == 0)
                                    continue;

                                foreach (DetectedFile this_file in these_files) {

                                    if (this_file.full_dir_path == null)
                                        continue;

                                    QuickHash hash = this_file.RootHash;

                                    Archive archive = Archives.GetArchive(game, this_file.owner, this_file.type, hash);

                                    try {
                                        if (archive == null) {
                                            if (this_file.owner == null)
                                                archive = new Archive(Core.settings.backup_path, new ArchiveID(game, null, this_file.type, hash));
                                            else
                                                archive = new Archive(Core.settings.backup_path, new ArchiveID(game, this_file.owner, this_file.type, hash));
                                            Archives.Add(archive);
                                        }
                                        //monitorNotifier.ShowBalloonTip(10, "Safety Will Robinson", "Trying to archive " + file.path, ToolTipIcon.Info);
                                        Communication.MessageHandler.suppress_messages = true;
                                        List<DetectedFile> temp_list = new List<DetectedFile>();
                                        temp_list.Add(this_file);
                                        archive.backup(temp_list, false);
                                    } catch {
                                        //monitorNotifier.ShowBalloonTip(10,"Danger Will Robinson","Error while trying to archive " + file.path,ToolTipIcon.Error);
                                        // If something goes wrong during backup, it's probable the file copy.
                                        // Reinsert the file to the end of the queue, then move on to the next one.
                                        if (!backup_queue.Contains(file)) {
                                            backup_queue.Insert(backup_queue.Count, file);
                                        }
                                    } finally {
                                        Communication.MessageHandler.suppress_messages = false;
                                    }


                                    //if (Core.games.all_games.get(game).sync_enabled) {
                                    //    DirectoryInfo sync_folder = new DirectoryInfo(Path.Combine(Core.settings.sync_path, game.ToString()));
                                    //    if (!sync_folder.Exists) {
                                    //        sync_folder.Create();
                                    //        sync_folder.Refresh();
                                    //    }
                                    //    if (!sync_folder.Exists) {
                                    //        TranslatingMessageHandler.SendError("CouldNotCreateSyncFolder", sync_folder.FullName);
                                    //        Core.games.all_games.get(game).sync_enabled = false;
                                    //        continue;
                                    //    }

                                    //    sync_watcher.EnableRaisingEvents = false;

                                    //    try {
                                    //        if (file.change_type == WatcherChangeTypes.Renamed && File.Exists(Path.Combine(sync_folder.FullName, file.old_path))) {
                                    //            File.Delete(Path.Combine(sync_folder.FullName, file.old_path));
                                    //        }
                                    //        copyFile(this_file.abs_root, sync_folder.FullName, Path.Combine(this_file.path, this_file.name));
                                    //    } catch (Exception e) {
                                    //        if (!backup_queue.Contains(file)) {
                                    //            backup_queue.Insert(backup_queue.Count, file);
                                    //        }
                                    //    }

                                    //    sync_watcher.EnableRaisingEvents = true;
                                    //}
                                }
                            }
                        }
                    } else if (file.change_type == WatcherChangeTypes.Deleted) {
                        //foreach (GameID game in paths_to_games[file.root]) {
                        //    if (!Core.games.all_games.get(game).sync_enabled)
                        //        continue;

                        //    DirectoryInfo sync_folder = new DirectoryInfo(Path.Combine(Core.settings.sync_path, game.ToString()));
                        //    if (!sync_folder.Exists)
                        //        continue;

                        //    if (File.Exists(Path.Combine(sync_folder.FullName, file.path))) {

                        //        sync_watcher.EnableRaisingEvents = false;

                        //        try {
                        //            File.Delete(Path.Combine(sync_folder.FullName, file.path));
                        //        } catch (Exception e) {
                        //            if (!backup_queue.Contains(file)) {
                        //                backup_queue.Insert(backup_queue.Count, file);
                        //            }
                        //        }
                        //        sync_watcher.EnableRaisingEvents = true;
                        //    }
                        //}
                    }
                //} else if (file.origin == Origin.Sync) {
                //    // this checks if the deleted file is already gone, and skips it becuase hey what is there to do
                //    if (!File.Exists(file.full_path) && file.change_type != WatcherChangeTypes.Deleted)
                //        continue;
                //    Game game = getGameHandler(file.root);
                //    if (game == null || !game.IsMonitored || !sync_paths.ContainsKey(game.id))
                //        continue;

                //    string destination = sync_paths[game.id];

                //    if (!watchmen.ContainsKey(destination))
                //        continue;

                //    bool old_raising = watchmen[destination].EnableRaisingEvents;
                //    watchmen[destination].EnableRaisingEvents = false;

                //    try {
                //        switch (file.change_type) {
                //            case WatcherChangeTypes.Created:
                //            case WatcherChangeTypes.Changed:
                //                copyFile(file.root, destination, file.path);
                //                break;
                //            case WatcherChangeTypes.Deleted:
                //                if (File.Exists(Path.Combine(destination, file.path))) {
                //                    File.Delete(Path.Combine(destination, file.path));
                //                }
                //                break;
                //            case WatcherChangeTypes.Renamed:
                //                copyFile(file.root, destination, file.path);
                //                if (File.Exists(Path.Combine(destination, file.old_path))) {
                //                    File.Delete(Path.Combine(destination, file.old_path));
                //                }
                //                break;
                //        }
                //    } catch (Exception e) {
                //        if (!backup_queue.Contains(file)) {
                //            backup_queue.Insert(backup_queue.Count, file);
                //        }
                //    }
                //    watchmen[destination].EnableRaisingEvents = old_raising;
                //}
            }
            // shutdown cleanup
            if (backup_queue != null) {
                lock (backup_queue) {
                    backup_queue.Clear();
                }
            }
            if (watchmen != null) {
                lock (watchmen) {
                    foreach (KeyValuePair<string, FileSystemWatcher> dispose_me in watchmen) {
                        dispose_me.Value.EnableRaisingEvents = false;
                        dispose_me.Value.Dispose();
                    }
                    watchmen.Clear();
                }
            }
            if (paths_to_games != null) {
                lock (paths_to_games) {
                    paths_to_games.Clear();
                }
            }
            this.monitor_thread.Interrupt();

        }


    }
}
