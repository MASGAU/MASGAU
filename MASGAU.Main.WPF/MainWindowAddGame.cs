using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Communication.Translator;
using MASGAU.Effects;
using MASGAU.Location;
using MVC.Communication;
using Translator;
using MASGAU.Analyzer;
using System.ComponentModel;
using System.Threading;
namespace MASGAU.Main {
    public partial class MainWindowNew {
        private enum game_locations {
            AllUsers,
            LocalAppData,
            MyComputer,
            MyDocuments,
            ProgramFiles,
            ProgramFilesX86,
            RoamingAppData,
            SavedGames,
            Steamapps,
            VirtualStore,
            PublicUser
        }

        private void addGameSetup() {
            AddGameSaves.Button.clearOptions();
//            AddGameLocation.ButtonText = Strings.GetLabelString(AddGameLocation.ButtonText);
            Array values = Enum.GetValues(typeof(game_locations));
            foreach (game_locations val in values) {
                if (val == game_locations.Steamapps && !Core.locations.steam_detected)
                    continue;
                else if (val == game_locations.ProgramFilesX86 && Core.locations.getFolder(EnvironmentVariable.ProgramFilesX86, null) == null)
                    continue;
                else if ((val == game_locations.PublicUser || val == game_locations.VirtualStore || val == game_locations.SavedGames) &&
                    Core.locations.platform_version == "WindowsXP")
                    continue;

                ComboBoxItem item = new ComboBoxItem();
                item.Content = Strings.GetLabelString(val.ToString());
                item.Tag = val;
                AddGameLocation.Button.addOption(item, new EventHandler(folderChoice));
            }
        }


        private void folderChoice(object sender, EventArgs e) {
            Environment.SpecialFolder folder = Environment.SpecialFolder.MyComputer;
            string default_folder = null;
            if (e != null) {
                SuperButtonEventArgs ev = (SuperButtonEventArgs)e;
                ComboBoxItem item = (ComboBoxItem)ev.SelectedItem;
                game_locations loc = (game_locations)item.Tag;
                switch (loc) {
                    case game_locations.ProgramFiles:
                        folder = Environment.SpecialFolder.ProgramFiles;
                        break;
                    case game_locations.ProgramFilesX86:
                        folder = Environment.SpecialFolder.ProgramFilesX86;
                        break;
                    case game_locations.Steamapps:
                        default_folder = Path.Combine(Core.settings.steam_path, "steamapps");
                        break;
                    case game_locations.MyDocuments:
                        folder = Environment.SpecialFolder.MyDocuments;
                        break;
                    case game_locations.SavedGames:
                        folder = Environment.SpecialFolder.UserProfile;
                        default_folder = Core.locations.getFolder(EnvironmentVariable.SavedGames, null);
                        break;
                    case game_locations.VirtualStore:
                        folder = Environment.SpecialFolder.LocalApplicationData;
                        default_folder = Path.Combine(Core.locations.getFolder(EnvironmentVariable.LocalAppData, null), "VirtualStore");
                        break;
                    case game_locations.LocalAppData:
                        folder = Environment.SpecialFolder.LocalApplicationData;
                        break;
                    case game_locations.RoamingAppData:
                        folder = Environment.SpecialFolder.ApplicationData;
                        break;
                    case game_locations.PublicUser:
                        default_folder = Core.locations.getFolder(EnvironmentVariable.Public, null);
                        break;
                    case game_locations.AllUsers:
                        default_folder = Core.locations.getFolder(EnvironmentVariable.AllUsersProfile, null);
                        break;
                }
            } else {
                folder = Environment.SpecialFolder.MyComputer;
            }

            AddGameLocation.Value = this.promptForPath(Strings.GetLabelString("SelectSaveFolder"), folder, default_folder);
        }

        private void AddGameLocation_ButtonClick(object sender, RoutedEventArgs e) {
            folderChoice(null, null);
        }


        private void addGame_Click(object sender, RoutedEventArgs e) {
            FadeEffect fade = new FadeInEffect(timing);
            fade.Start(AddGameGrid);
            ribbon.IsEnabled = false;
            GameGrid.IsEnabled = false;
            ArchiveGrid.IsEnabled = false;
        }

        private void closeAddGame(bool reset) {
            FadeEffect fade = new FadeOutEffect(timing);
            fade.Start(AddGameGrid);
            ribbon.IsEnabled = true;
            GameGrid.IsEnabled = true;
            ArchiveGrid.IsEnabled = true;

            if (reset) {
                AddGameTitle.Value = "";
                AddGameLocation.Value = "";
                AddGameSaves.Value = "";
                AddGameExclusions.Value = "";
            }
        }

        private void AddGameCancelButton_Click(object sender, RoutedEventArgs e) {
            closeAddGame(false);
        }

        private void addGameButtonCheck(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            string name = CustomGame.prepareGameName(AddGameTitle.Value);
            GameID id = new GameID(name,"Custom");
            if(Games.Get(id)!=null) {
                AddGameTitle.Header = Strings.GetLabelString("AddGameTitleNotUnique");
                AddGameTitle.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                AddGameButton.IsEnabled = false;
                return;
            } else {
                AddGameTitle.Header = Strings.GetLabelString("AddGameTitle");
                AddGameTitle.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0,0,0));
            }
                
            if(AddGameButton!=null)
                AddGameButton.IsEnabled = AddGameTitle.Value!=""&&AddGameLocation.Value!="";
        }


        private void AddGameButton_Click(object sender, RoutedEventArgs e) {
            CustomGame game = Games.addCustomGame(AddGameTitle.Value, new System.IO.DirectoryInfo(AddGameLocation.Value), AddGameSaves.Value, AddGameExclusions.Value);
            if(!Core.settings.SuppressSubmitRequests) {
                RequestReply reply = TranslatingRequestHandler.Request(RequestType.Question, "PleaseSubmitGame", true);
                if (!reply.cancelled) {
                    createGameSubmission(game);
                } else {
                    if (reply.suppress)
                        Core.settings.SuppressSubmitRequests = true;
                }
            }
            closeAddGame(true);
        }


        private void deleteGame_Click(object sender, RoutedEventArgs e) {
            List<GameVersion> games = new List<GameVersion>();
            foreach (GameVersion game in gamesLst.SelectedItems) {
                games.Add(game);
            }
            foreach (GameVersion game in games) {
                Games.deleteCustomGame(game);
            }
        }

        protected void createGameSubmission(CustomGame game) {
            PCAnalyzer analyzer = new PCAnalyzer(game, gameSubmissionDone);
            disableInterface(analyzer);


        }

        protected void gameSubmissionDone(object sender, RunWorkerCompletedEventArgs e) {
            this.enableInterface();
        }

    }
}
