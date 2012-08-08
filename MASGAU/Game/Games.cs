using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using MVC.Communication;
using MVC.Translator;
using MVC;
using MASGAU.Game;
using GameSaveInfo;
namespace MASGAU {
    public class Games : StaticModel<GameID, GameEntry> {
        public static GameXmlFiles xml;
        public static void saveCustomGames() {
            xml.custom.Save();
        }

        public static bool HasUnsubmittedGames {
            get {
                if (xml.custom == null)
                    return false;

                foreach (CustomGame game in xml.custom.Entries) {
                    if (!game.Submitted)
                        return true;
                }
                return false;
            }
        }
        public static Queue<CustomGameEntry> UnsubmittedGames {
            get {
                Queue<CustomGameEntry> games = new Queue<CustomGameEntry>();
                foreach (CustomGame game in xml.custom.Entries) {
                    foreach (CustomGameVersion version in game.Versions) {
                        CustomGameEntry entry = Games.Get(version.ID) as CustomGameEntry;
                        if (entry!=null&&!entry.Submitted)
                            games.Enqueue(entry);

                    }
                }
                return games;
            }
        }

        public static bool XmlLoaded {
            get {
                return xml != null;
            }
        }


        private static FilteredModel<GameID, GameEntry> _DetectedGames;
        public static Model<GameID,GameEntry> DetectedGames {
            get {
                return _DetectedGames;
            }
        }

        public Boolean NoGamesDetected {

            get {
                return !GamesDetected;
            }
        }

        public Boolean GamesDetected {
            get {
                return DetectedGames.Count != 0;
            }
        }

        public static int detected_games_count {
            get {
                int i = 0;
                foreach (GameEntry game in model) {
                    if (game.IsDetected)
                        i++;
                }
                return i;
            }
        }
        public static int monitored_games_count {
            get {
                int i = 0;
                foreach (GameEntry game in model) {
                    if (game.IsMonitored)
                        i++;
                }
                return i;
            }
        }

        public static string GameDataFolder {
            get {
                string path = xml.DataSource.FullName;

                return path;
            }
        }

        static Games() {
            model.PropertyChanged += new PropertyChangedEventHandler(GamesHandler_PropertyChanged);
            model.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(GamesHandler_CollectionChanged);


            _DetectedGames = new FilteredModel<GameID,GameEntry>(model);
            _DetectedGames.AddFilter("IsDetected", true);
        }

        public static bool Contains(GameIdentifier game) {
            GameID id = new GameID(game);
            return model.containsId(id);
        }
        public static GameEntry Get(GameIdentifier game) {
            GameID id = new GameID(game);
            return model.get(id);
        }


        static void GamesHandler_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {

            //Console.Write("test");
        }

        static void GamesHandler_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            //throw new NotImplementedException();
        }

        public static CustomGameEntry addCustomGame(string title, DirectoryInfo location, string saves, string ignores) {
            CustomGame game = xml.custom.createCustomGame(title, location, saves, ignores);
            CustomGameEntry entry = new CustomGameEntry(game.Versions[0] as CustomGameVersion);
            entry.Detect();
            addGame(entry);

            xml.custom.Save();
            _DetectedGames.Refresh();
            return entry;
        }
        public static void deleteCustomGame(GameEntry version) {
            model.Remove(version);
            GameSaveInfo.Game game = xml.custom.getGame(version.id.Name);
            xml.custom.removeEntry(game);
            xml.custom.Save();
            _DetectedGames.Refresh();
        }

        private static void addGame(GameEntry game) {
            if (model.containsId(game.id)) {
                throw new Translator.TranslateableException("DuplicateGame", game.id.ToString(),game.SourceFile,model.get(game.id).SourceFile);
            }
            if (game.id.OS == "PS1") {
                //                        GameVersion psp_game = 
                //                    GameXML psp_game = game_profile;
                //                      psp_game = new GameXML(new GameID(psp_game.id.name, "PSP", psp_game.id.region), psp_game.xml);
                //                  createGameObject(psp_game);
            }
            model.AddWithSort(game);
        }

        public static void loadXml() {
            TranslatingProgressHandler.setTranslatedMessage("LoadingGameXmls");

            xml = new GameXmlFiles();
            model.Clear();
            if (xml.Entries.Count > 0) {
                TranslatingProgressHandler.setTranslatedMessage("LoadingGamesData");
                foreach (GameSaveInfo.Game game in xml.Entries) {
                    foreach (GameVersion version in game.Versions) {
                        try {
                            GameEntry entry = new GameEntry(version);
                            addGame(entry);
                        } catch (Exception e) {
                            TranslatingMessageHandler.SendException(e);
                        }
                    }
                }
            }
        }

        public static bool IsNameUsed(string name) {
            foreach (GameSaveInfo.Game game in xml.Entries) {
                if (game.Name == name)
                    return true;
            }
            foreach (GameSaveInfo.Game game in xml.custom.Entries) {
                if (game.Name == name)
                    return true;
            }
            return false;
        }

        public static void redetectGames(object sender, DoWorkEventArgs e) {
            detectGames(null, true);
        }

        public static void detectGames() {
            detectGames(null, false);
        }
        public static void detectGames(List<GameID> these_games, bool force_redetect) {
            ProgressHandler.state = ProgressState.Normal;

            Core.monitor.stop();

            if (model.Count == 0) {
                loadXml();
            }

            int game_count;
            game_count = model.Count;

            if (these_games != null)
                ProgressHandler.max = these_games.Count;
            else
                ProgressHandler.max = game_count;


            ProgressHandler.value = 1;
            TranslatingProgressHandler.setTranslatedMessage("DetectingGames");

            Dictionary<string, int> contribs = new Dictionary<string, int>();

            string string_to_use= "DetectingGamesProgress";
            if(Core.settings.MonitoredGames.Count>0)
                    string_to_use = "DetectingMonitoringGamesProgress";


            foreach (GameEntry game in model) {
                //if (_cancelling)
                //    break;

                if (these_games != null && !these_games.Contains(game.id))
                    continue;



                TranslatingProgressHandler.setTranslatedMessage(string_to_use, ProgressHandler.value.ToString(), model.Count.ToString());

                ProgressHandler.suppress_communication = true;

                // If a game has a game root and thus forced a game to detect early, then this will skip re-detecting
                if (!game.DetectionAttempted)
                    game.Detect();

                if (game.IsMonitored) {
                    game.startMonitoring(null,null);
                }
                ProgressHandler.suppress_communication = false;


                //foreach (string contrib in game.Contributors) {
                //    if (contribs.ContainsKey(contrib))
                //        contribs[contrib]++;
                //    else
                //        contribs.Add(contrib, 1);
                //}


                //if (!force_redetect && game.IsDetected && !game.id.deprecated)
                //    model.AddWithSort(game);

                ProgressHandler.value++;
            }

            //foreach (KeyValuePair<string, int> pair in contribs) {
            //    contributors.AddWithSort(new ContributorHandler(pair.Key, pair.Value));
            //}

            ProgressHandler.state = ProgressState.None;
            ProgressHandler.value = 0;

            game_count = detected_games_count;

            Archives.DetectBackups();

            model.IsEnabled = true;
            if (game_count > 1) {
                TranslatingProgressHandler.setTranslatedMessage("GamesDetected", detected_games_count.ToString());
            } else if (game_count > 0) {
                TranslatingProgressHandler.setTranslatedMessage("GameDetected");
            } else {
                TranslatingProgressHandler.setTranslatedMessage("NoGamesDetected");
                //GameHandler no_games = new GameHandler(new GameID(Strings.getGeneralString("NoGamesDetected"), GamePlatform.Multiple, null));
                //no_games.title = Strings.getGeneralString("NoGamesDetected");
                model.IsEnabled = false;
            }
            Monitor.Monitor.flushQueue();
            Core.monitor.start();

            StaticNotifyPropertyChanged("GamesDetected");
        }

        public static void purgeGames(System.Collections.IEnumerable games, RunWorkerCompletedEventHandler e) {
            BackgroundWorker purger = new BackgroundWorker();
            purger.DoWork += new DoWorkEventHandler(purgeRoots);
            purger.RunWorkerCompleted += e;
            purger.RunWorkerAsync(games);
        }

        private static void purgeRoots(object sender, DoWorkEventArgs e) {
            if (TranslatingRequestHandler.Request(RequestType.Question, "PurgeConfirmation").Cancelled) {
                e.Cancel = true;
                return;
            }

            e.Result = e.Argument;
            foreach (GameEntry game in (System.Collections.IEnumerable)e.Argument) {
                try {
                    if (!game.purgeRoot())
                        break;
                } catch (Exception ex) {
                    TranslatingMessageHandler.SendError("PurgeError", ex);
                }
            }
        }


    }

}
