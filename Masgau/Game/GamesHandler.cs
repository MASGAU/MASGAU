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
                ProgressHandler.message = "Loading Games Data...";
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
                StringBuilder message = new StringBuilder("There is a duplicate game with the name " + game_profile.id.name);
                if(game_profile.id.platform!= GamePlatform.Multiple)
                    message.Append(" for the " + game_profile.id.platform.ToString() + " platform");
                if(game_profile.id.region!=null)
                    message.Append(" for the region " + game_profile.id.region);
                message.Append(".");
                MessageHandler.SendWarning("Game Load Error",message.ToString());
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
            ProgressHandler.message = "Detecting Games";

            Dictionary<string,int> contribs = new Dictionary<string,int>();

            foreach (GameHandler game in all_games) {
                if(_cancelling)
                    break;

                if(these_games!=null&&!these_games.Contains(game.id))
                    continue;


                ProgressHandler.message = "Detecting Games (" + ProgressHandler.value + "/" + ProgressHandler.max + ")";

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
                ProgressHandler.message = detected_games_count + " Games Detected";
            } else if(game_count>0) {
                ProgressHandler.message = detected_games_count + " Game Detected";
            } else {
                ProgressHandler.message = "No Games Detected";
                GameHandler no_games = new GameHandler(new GameID("No Games Detected", GamePlatform.Multiple, null));
                no_games.title = "No Games Detected";
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
            if(RequestHandler.Request(RequestType.Question,"This could hurt.","Purging erases all detected save paths for the specified game\nAre you sure you want to continue?").cancelled) {
                e.Cancel = true;
                return;
            }

            e.Result = e.Argument;
            foreach(GameHandler game in (System.Collections.IEnumerable)e.Argument) {
                try {
                    if(!game.purgeRoot())
                        break;
                } catch (Exception ex) {
                    MessageHandler.SendError("Error While Puring Root",ex.Message,ex);
                }
            }
        }

        public Model<ContributorHandler> contributors = new Model<ContributorHandler>();

    }

}
