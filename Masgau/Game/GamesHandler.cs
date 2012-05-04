using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.ComponentModel;
using System.Data;
using Communication;
using Communication.Progress;
using Communication.Message;
using Communication.Request;
using MVC;
using Translator;
using Communication.Translator;
namespace MASGAU.Game
{


    public class GamesHandler: Model<GameID,GameHandler>
    {
        public Model<GameID,GameHandler> all_games = new Model<GameID,GameHandler>();

        public GamesXMLHandler xml;
        public List<GameHandler> enabled_games {
            get {
                List<GameHandler> return_me = new List<GameHandler>();
                foreach (GameHandler game in this)
                {
                if(game.backup_enabled&&game.detected_locations!=null&& game.detected_locations.Count > 0)
                        return_me.Add(game);
                }
                return return_me;

            }
        }

        public Boolean no_games_detected
        {

            get
            {
                return !games_detected;
            }
        }

        public Boolean games_detected
        {
            get
            {
                return detected_games_count!=0;
            }
        }

        public int enabled_games_count {
            get {
                int i = 0;
                foreach (GameHandler game in this)
                {
                if (game.backup_enabled &&game.detected_locations!=null&& game.detected_locations.Count > 0)
                        i++;
                }
                return i;
            }
        }
        public int detected_games_count {
            get {
                int i = 0;
                foreach (GameHandler game in this)
                {
                if (game.detected_locations!=null&& game.detected_locations.Count > 0)
                        i++;
                }
                return i;
            }
        }

        public GamesHandler() {
            loadXml();

            this.PropertyChanged += new PropertyChangedEventHandler(GamesHandler_PropertyChanged);
            this.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(GamesHandler_CollectionChanged);
        
        }

        void GamesHandler_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
            //Console.Write("test");
        }

        void GamesHandler_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void loadXml() {
            xml = new GamesXMLHandler();
            xml.loadXml();

            all_games.Clear();

            if(xml.game_profiles.Count>0) {
                TranslatingProgressHandler.setTranslatedMessage("LoadingGamesData");
                foreach(GameXMLHolder game_profile in xml.game_profiles) {
                    if(_cancelling)
                        break;

                    createGameObject(game_profile);

                    if(game_profile.id.platform== GamePlatform.PS1) {
                        GameXMLHolder psp_game = game_profile;
                        psp_game = new GameXMLHolder(new GameID(psp_game.id.name,GamePlatform.PSP,psp_game.id.region),psp_game.xml);
                        createGameObject(psp_game);
                    }
                }
            }

        }

        private void createGameObject(GameXMLHolder game_profile) {
            if(!all_games.containsId(game_profile.id)) {
                GameHandler add_me = new GameHandler(game_profile);
                all_games.Add(add_me);
            } else {
                TranslatingMessageHandler.SendWarning("DuplicateGame", game_profile.id.ToString());
            }

        }

        public void redetectGames(object sender, DoWorkEventArgs e) {
            detectGames(null,true);
        }

        public void detectGames() {
            detectGames(null,false);
        }
        public void detectGames(List<GameID> these_games, bool force_redetect) {
            ProgressHandler.state = ProgressState.Normal;

            int game_count;
            game_count = all_games.Count;

            this.Clear();

            if(these_games!=null)
                ProgressHandler.max = these_games.Count;
            else
                ProgressHandler.max = game_count;


            ProgressHandler.value = 1;
            TranslatingProgressHandler.setTranslatedMessage("DetectingGames");

            Dictionary<string,int> contribs = new Dictionary<string,int>();

            foreach (GameHandler game in all_games) {
                if(_cancelling)
                    break;

                if(these_games!=null&&!these_games.Contains(game.id))
                    continue;

                TranslatingProgressHandler.setTranslatedMessage("DetectingGamesProgress", ProgressHandler.value.ToString(), ProgressHandler.max.ToString() );

                // If a game has a game root and thus forced a game to detect early, then this will skip re-detecting
                if(!game.detection_completed)
                    game.detect();

                foreach(string contrib in game.contributors) {
                    if(contribs.ContainsKey(contrib))
                        contribs[contrib]++;
                    else
                        contribs.Add(contrib,1);
                }

                    
                if(!force_redetect&&game.detected&&!game.id.deprecated) 
                    this.AddWithSort(game);

                ProgressHandler.value++;
            }

            foreach(KeyValuePair<string,int> pair in contribs) {
                contributors.AddWithSort(new ContributorHandler(pair.Key,pair.Value));
            }

            ProgressHandler.state = ProgressState.None;
            ProgressHandler.value = 0;

            game_count = detected_games_count;

            IsEnabled = true;
            if(game_count>1){
                TranslatingProgressHandler.setTranslatedMessage("GameDetected");
            } else if(game_count>0) {
                TranslatingProgressHandler.setTranslatedMessage("GamesDetected",detected_games_count.ToString());
            } else {
                TranslatingProgressHandler.setTranslatedMessage("NoGamesDetected");
                GameHandler no_games = new GameHandler(new GameID(Strings.getGeneralString("NoGamesDetected"), GamePlatform.Multiple, null));
                no_games.title = Strings.getGeneralString("NoGamesDetected");
                IsEnabled = false;
            }
            this.NotifyPropertyChanged("games_detected");
        }

        private BackgroundWorker purger;
        public void purgeGames(System.Collections.IEnumerable games, RunWorkerCompletedEventHandler e) {
            purger = new BackgroundWorker();
            purger.DoWork +=new DoWorkEventHandler(purgeRoots);
            purger.RunWorkerCompleted += e;
            purger.RunWorkerAsync(games);
        }

        private static void purgeRoots(object sender, DoWorkEventArgs e) {
            if(TranslatingRequestHandler.Request(RequestType.Question,"PurgeConfirmation").cancelled) {
                e.Cancel = true;
                return;
            }

            e.Result = e.Argument;
            foreach(GameHandler game in (System.Collections.IEnumerable)e.Argument) {
                try {
                    if(!game.purgeRoot())
                        break;
                } catch (Exception ex) {
                    TranslatingMessageHandler.SendError("PurgeError", ex);
                }
            }
        }

        public Model<ContributorHandler> contributors = new Model<ContributorHandler>();

    }

}
