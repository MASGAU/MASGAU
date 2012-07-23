using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Communication;
using Communication.Translator;
using MVC;
namespace MASGAU {
    public class Games : StaticModel<GameID, GameVersion> {
        public static GameXmlFiles xml;
        protected static CustomGameXmlFile custom;

        public static bool XmlLoaded {
            get {
                return xml != null;
            }
        }


        private static FilteredModel<GameID, GameVersion> _DetectedGames;
        public static Model<GameID,GameVersion> DetectedGames {
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
                foreach (GameVersion game in model) {
                    if (game.IsDetected)
                        i++;
                }
                return i;
            }
        }
        public static int monitored_games_count {
            get {
                int i = 0;
                foreach (GameVersion game in model) {
                    if (game.IsMonitored)
                        i++;
                }
                return i;
            }
        }


        static Games() {
            model.PropertyChanged += new PropertyChangedEventHandler(GamesHandler_PropertyChanged);
            model.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(GamesHandler_CollectionChanged);


            _DetectedGames = new FilteredModel<GameID,GameVersion>(model);
            _DetectedGames.AddFilter("IsDetected", true);
        }

        static void GamesHandler_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {

            //Console.Write("test");
        }

        static void GamesHandler_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            //throw new NotImplementedException();
        }

        public static void addCustomGame(string title, DirectoryInfo location, string saves, string ignores) {
            Game game = custom.createCustomGame(title, location, saves, ignores);
            foreach (GameVersion ver in game.Versions) {
                ver.Detect();
                addGame(ver);
            }
            custom.Add(game);
            custom.Save();
            _DetectedGames.Refresh();
        }

        private static void addGame(GameVersion game) {
            if (model.containsId(game.id)) {
                throw new Translator.TranslateableException("DuplicateGame", game.id.ToString());
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
            xml = new GameXmlFiles();
            model.Clear();

            if (xml.Entries.Count > 0) {
                TranslatingProgressHandler.setTranslatedMessage("LoadingGamesData");
                foreach (Game game in xml.Entries) {
                    foreach (GameVersion version in game.Versions) {
                        addGame(version);
                    }
                }
            }
            DirectoryInfo common = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "masgau"));
            if(!common.Exists)
                common.Create();

            if(!File.Exists(Path.Combine(common.FullName,"games.xsd"))) {
                FileInfo schema = new FileInfo(Path.Combine(Core.app_path, "data","games.xsd"));
                if(!schema.Exists)
                    throw new Exception("Schema file not found at data/games.xsd! Please Re-install");
                schema.CopyTo(Path.Combine(common.FullName,"games.xsd"),true);
            }

            FileInfo custom_xml = new FileInfo(Path.Combine(common.FullName,"custom.xml"));
            custom = new CustomGameXmlFile(custom_xml);

            if (custom.entries.Count > 0) {
                foreach (CustomGame game in custom.entries) {
                    foreach (CustomGameVersion version in game.Versions) {
                        addGame(version);
                    }
                }
            }
        }


        public static void redetectGames(object sender, DoWorkEventArgs e) {
            detectGames(null, true);
        }

        public static void detectGames() {
            detectGames(null, false);
        }
        public static void detectGames(List<GameID> these_games, bool force_redetect) {
            ProgressHandler.state = ProgressState.Normal;

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

            foreach (GameVersion game in model) {
                //if (_cancelling)
                //    break;

                if (these_games != null && !these_games.Contains(game.id))
                    continue;

                TranslatingProgressHandler.setTranslatedMessage("DetectingGamesProgress", ProgressHandler.value.ToString(), ProgressHandler.max.ToString());

                // If a game has a game root and thus forced a game to detect early, then this will skip re-detecting
                if (!game.DetectionAttempted)
                    game.Detect();

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
            StaticNotifyPropertyChanged("GamesDetected");
        }

        public static void purgeGames(System.Collections.IEnumerable games, RunWorkerCompletedEventHandler e) {
            BackgroundWorker purger = new BackgroundWorker();
            purger.DoWork += new DoWorkEventHandler(purgeRoots);
            purger.RunWorkerCompleted += e;
            purger.RunWorkerAsync(games);
        }

        private static void purgeRoots(object sender, DoWorkEventArgs e) {
            if (TranslatingRequestHandler.Request(RequestType.Question, "PurgeConfirmation").cancelled) {
                e.Cancel = true;
                return;
            }

            e.Result = e.Argument;
            foreach (GameVersion game in (System.Collections.IEnumerable)e.Argument) {
                try {
                    if (!game.purgeRoot())
                        break;
                } catch (Exception ex) {
                    TranslatingMessageHandler.SendError("PurgeError", ex);
                }
            }
        }

        public Model<ContributorHandler> contributors = new Model<ContributorHandler>();

    }

}
