using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MASGAU.Location.Holders;
using MVC;
using MVC.Communication;
using MVC.Translator;
using Translator;
namespace MASGAU.Monitor {
    public class Monitor : ANotifyingObject {
        private static Queue<MonitorFile> FileQueue = new Queue<MonitorFile>();
        private BackgroundWorker worker = new BackgroundWorker();

        public static void flushQueue() {
            lock (FileQueue) {
                FileQueue.Clear();
            }
        }

        private string _status = null;
        public string Status {
            get {
                if (!worker.IsBusy)
                    return Strings.GetMessageString("MonitorNotRunning");

                if (MonitoredCount > 0 && Core.settings.backup_path_not_set)
                    return Strings.GetMessageString("MonitorNeedsBackupPath");

                if (_status != null)
                    return _status;

                return Strings.GetMessageString("MonitoredGameCount", MonitoredCount.ToString());
            }
        }

        public bool Active {
            get {
                return worker.IsBusy && MonitoredCount > 0;
            }
        }

        public void stop() {
            if (worker.IsBusy)
                worker.CancelAsync();

            while (worker.IsBusy)
                Thread.Sleep(100);
        }
        public void start() {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }
        public int MonitoredCount {
            get {
                int count = 0;
                lock (Games.DetectedGames) {
                    foreach (GameEntry game in Games.DetectedGames) {
                        if (game.IsMonitored)
                            count++;
                    }
                }
                return count;
            }
        }

        public Monitor() {
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }




        void worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                TranslatingMessageHandler.SendException(e.Error);
                worker.RunWorkerAsync();
            }
            NotifyPropertyChanged("Status");
            NotifyPropertyChanged("Active");

        }

        public static void EnqueueFile(MonitorFile file) {
            lock (FileQueue) {
                if (FileQueue.Count > 0) {
                    MonitorFile top = FileQueue.Peek();
                    if (top != null && top.full_path == file.full_path)
                        return;
                }
                FileQueue.Enqueue(file);
            }
        }

        private int QueueCount {
            get {
                int count;
                lock (FileQueue) {
                    count = FileQueue.Count;
                }
                return count;
            }
        }

        void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            MonitorFile file;
            while (!worker.CancellationPending) {
                NotifyPropertyChanged("Active");
                NotifyPropertyChanged("Status");
                if (QueueCount == 0) {
                    // No new files? Take a nap.
                    try {
                        Thread.Sleep(100);
                    } catch (Exception ex) {
                        TranslatingMessageHandler.SendException(ex);
                    }
                    continue;
                }

                lock (FileQueue) {
                    file = FileQueue.Dequeue();
                }

                if (!file.Path.Game.IsMonitored)
                    continue;

                FileInfo fi = new FileInfo(Path.Combine(file.root, file.path));
                GameID game = file.Path.Game.id;
                switch (file.change_type) {
                    case System.IO.WatcherChangeTypes.Changed:
                    case System.IO.WatcherChangeTypes.Created:
                    case System.IO.WatcherChangeTypes.Renamed:
                        if (!fi.Exists)
                            continue;
                        List<DetectedFile> these_files = file.Path.Game.GetSavesMatching(file.full_path);
                        if (these_files.Count == 0)
                            continue;
                        foreach (DetectedFile this_file in these_files) {
                            _status = game.Name + " updating " + Path.Combine(this_file.Path, this_file.Name);
                            NotifyPropertyChanged("Status");

                            if (this_file.FullDirPath == null)
                                continue;

                            Archive archive = Archives.GetArchive(game, this_file);

                            try {
                                if (archive == null) {
                                    if (this_file.owner == null)
                                        archive = new Archive(Core.settings.backup_path, new ArchiveID(game, this_file));
                                    else
                                        archive = new Archive(Core.settings.backup_path, new ArchiveID(game, this_file));
                                    Archives.Add(archive);
                                }
                                //monitorNotifier.ShowBalloonTip(10, "Safety Will Robinson", "Trying to archive " + file.path, ToolTipIcon.Info);
                                MessageHandler.suppress_messages = true;
                                List<DetectedFile> temp_list = new List<DetectedFile>();
                                temp_list.Add(this_file);
                                archive.backup(temp_list, false, true);
                            } catch {
                                //monitorNotifier.ShowBalloonTip(10,"Danger Will Robinson","Error while trying to archive " + file.path,ToolTipIcon.Error);
                                // If something goes wrong during backup, it's probable the file copy.
                                // Reinsert the file to the end of the queue, then move on to the next one.
                                if (!FileQueue.Contains(file)) {
                                    FileQueue.Enqueue(file);
                                }
                            } finally {
                                MessageHandler.suppress_messages = false;
                            }
                        }
                        break;
                }
                if (worker.CancellationPending)
                    e.Cancel = true;

                _status = null;
            }
            NotifyPropertyChanged("Status");
            NotifyPropertyChanged("Active");
        }
    }
}