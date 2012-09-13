﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GameSaveInfo;
using MASGAU.Analyzer;
using MVC.Communication;
using MVC.Translator;
using SMJ.WPF;
using Translator;
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

            AddGameLocation.Button.clearOptions();
            //            AddGameLocation.ButtonText = Strings.GetLabelString(AddGameLocation.ButtonText);
            Array values = Enum.GetValues(typeof(game_locations));
            foreach (game_locations val in values) {
                if (val == game_locations.Steamapps && !Common.Locations.steam_detected)
                    continue;
                else if (val == game_locations.ProgramFilesX86 && Common.Locations.getFolder(EnvironmentVariable.ProgramFilesX86, null) == null)
                    continue;
                else if ((val == game_locations.PublicUser || val == game_locations.VirtualStore || val == game_locations.SavedGames) &&
                    Common.Locations.platform_version == "WindowsXP")
                    continue;

                ComboBoxItem item = new ComboBoxItem();
                item.Content = Strings.GetLabelString(val.ToString());
                item.Tag = val;
                helpone.ToolTip = Strings.GetToolTipString("AddGameSaves");
                helptwo.ToolTip = Strings.GetToolTipString("AddGameSaves");
                AddGameLocation.Button.addOption(item, new EventHandler(folderChoice));
            }
            submitGame.IsEnabled = Games.HasUnsubmittedGames;
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
                        default_folder = Path.Combine(Common.Settings.steam_path, "steamapps");
                        break;
                    case game_locations.MyDocuments:
                        folder = Environment.SpecialFolder.MyDocuments;
                        break;
                    case game_locations.SavedGames:
                        folder = Environment.SpecialFolder.UserProfile;
                        default_folder = Common.Locations.getFolder(EnvironmentVariable.SavedGames, null);
                        break;
                    case game_locations.VirtualStore:
                        folder = Environment.SpecialFolder.LocalApplicationData;
                        default_folder = Path.Combine(Common.Locations.getFolder(EnvironmentVariable.LocalAppData, null), "VirtualStore");
                        break;
                    case game_locations.LocalAppData:
                        folder = Environment.SpecialFolder.LocalApplicationData;
                        break;
                    case game_locations.RoamingAppData:
                        folder = Environment.SpecialFolder.ApplicationData;
                        break;
                    case game_locations.PublicUser:
                        default_folder = Common.Locations.getFolder(EnvironmentVariable.Public, null);
                        break;
                    case game_locations.AllUsers:
                        default_folder = Common.Locations.getFolder(EnvironmentVariable.AllUsersProfile, null);
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
            openSubWindow(AddGameGrid);
        }

        private void closeAddGame(bool reset) {
            closeSubWindow(AddGameGrid);

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
            if (Games.IsNameUsed(name)) {
                AddGameTitle.Header = Strings.GetLabelString("AddGameTitleNotUnique");
                AddGameTitle.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                AddGameButton.IsEnabled = false;
                return;
            } else {
                AddGameTitle.Header = Strings.GetLabelString("AddGameTitle");
                AddGameTitle.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            }

            if (AddGameButton != null)
                AddGameButton.IsEnabled = AddGameTitle.Value != "" && AddGameLocation.Value != "";
        }


        private void AddGameButton_Click(object sender, RoutedEventArgs e) {
            CustomGameEntry game = Games.addCustomGame(AddGameTitle.Value, new System.IO.DirectoryInfo(AddGameLocation.Value), AddGameSaves.Value, AddGameExclusions.Value);
            if (!Common.Settings.SuppressSubmitRequests) {
                RequestReply reply = TranslatingRequestHandler.Request(RequestType.Question, "PleaseSubmitGame", true);
                if (!reply.Cancelled) {
                    createGameSubmission(game);
                } else {
                    if (reply.Suppressed)
                        Common.Settings.SuppressSubmitRequests = true;
                }
            }
            closeAddGame(true);
            submitGame.IsEnabled = Games.HasUnsubmittedGames;
        }


        private void deleteGame_Click(object sender, RoutedEventArgs e) {
            if (TranslatingRequestHandler.Request(RequestType.Question, "DeleteGameConfirm", gamesLst.SelectedItems.Count.ToString()).Cancelled)
                return;

            List<GameEntry> games = new List<GameEntry>();
            foreach (GameEntry game in gamesLst.SelectedItems) {
                games.Add(game);
            }
            foreach (GameEntry game in games) {
                Games.deleteCustomGame(game);
            }

            submitGame.IsEnabled = Games.HasUnsubmittedGames;
        }

        Queue<CustomGameEntry> submitting_games = new Queue<CustomGameEntry>();

        private void submitGame_Click(object sender, RoutedEventArgs e) {
            submitting_games.Clear();
            submitting_games = Games.UnsubmittedGames;

            submitPromptSuppress = false;

            askAboutGame();
        }

        protected void createGameSubmission(CustomGameEntry game) {
            analyzer = new PCAnalyzer(game, gameSubmissionDone);
            disableInterface(analyzer);
            analyzer.analyze();
        }

        private bool submitPromptSuppress = false;

        private bool askAboutGame() {
            if (!submitPromptSuppress) {
                while (submitting_games.Count > 0) {
                    CustomGameEntry game = submitting_games.Peek();
                    RequestReply reply = TranslatingRequestHandler.Request(RequestType.Question, "AskSubmitGame", true, game.Title);
                    submitPromptSuppress = reply.Suppressed;
                    if (reply.Cancelled) {
                        if (reply.Suppressed) {
                            submitting_games.Clear();
                            return false;
                        } else {
                            submitting_games.Dequeue();
                        }
                    } else {
                        createGameSubmission(submitting_games.Dequeue());
                        return true;
                    }
                }
            }

            if (submitting_games.Count > 0) {
                createGameSubmission(submitting_games.Dequeue());
                return true;
            }

            return false;
        }

        protected void gameSubmissionDone(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                this.enableInterface();
                return;
            }

            ReportWindow report = new ReportWindow(analyzer, this);
            report.ShowDialog();
            if (report.SentOrSaved) {
                analyzer.game.Submitted = true;
                Games.saveCustomGames();
            }
            if (askAboutGame())
                return;

            submitGame.IsEnabled = Games.HasUnsubmittedGames;

            this.enableInterface();
        }

    }
}
