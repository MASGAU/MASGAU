using System;
using System.IO;
using MASGAU.Location.Holders;
using MVC.Communication;
using MVC.Translator;
namespace MASGAU.Monitor {
    public class MonitorPath : FileSystemWatcher {
        public new DetectedLocationPathHolder Path { get; protected set; }
        public GameEntry Game { get; protected set; }
        public MonitorPath(GameEntry game, DetectedLocationPathHolder path)
            : base(path.full_dir_path, "*") {
            this.Path = path;
            this.Game = game;

            this.IncludeSubdirectories = true;
            this.Created += new FileSystemEventHandler(changed);
            this.Changed += new FileSystemEventHandler(changed);
            this.Deleted += new FileSystemEventHandler(changed);
            this.Renamed += new RenamedEventHandler(changed);
        }


        private void changed(Object sender, FileSystemEventArgs e) {
            MonitorFile add_me = new MonitorFile(this);
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
            Monitor.EnqueueFile(add_me);
        }

        private static bool backuppathwarned = false;


        public void start() {
            while (!Core.settings.IsBackupPathSet && !backuppathwarned) {
                RequestReply reply = RequestHandler.Request(RequestType.BackupFolder, false);
                if (reply.Cancelled) {
                    TranslatingMessageHandler.SendWarning("MonitorNeedsBackupPath");
                    backuppathwarned = true;
                    //throw new TranslateableException("BackupPathNotSet");
                }
            }
            this.EnableRaisingEvents = true;
        }

        public void stop() {
            this.EnableRaisingEvents = false;

        }

        ~MonitorPath() {
            stop();
        }
    }
}
