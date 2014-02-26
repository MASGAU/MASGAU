using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GameSaveInfo;
using MASGAU.Analyzer;
using MASGAU.Location;
using MVC.Communication;
using MVC.Translator;
using SMJ.WPF;
using Translator;
using Microsoft.Windows.Controls.Ribbon;
namespace MASGAU.Main {
    public partial class MainWindowNew {

        private void addGameSetup() {

            AddGameLocation.Button.clearOptions();
			SubmitOther.Items.Clear();

            foreach(QuickBrowsePath path in Core.locations.QuickBrowsePaths) {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = path.Name;
                item.Tag = path;
                helpone.ToolTip = Strings.GetToolTipString("AddGameSaves");
                helptwo.ToolTip = Strings.GetToolTipString("AddGameSaves");
                AddGameLocation.Button.addOption(item, new EventHandler(folderChoice));

                RibbonMenuItem menu_item = new RibbonMenuItem();
                menu_item.Header = path.Name;
                menu_item.Tag = path;
                menu_item.Click += otherFolderClick;
                SubmitOther.Items.Add(menu_item);

            }
            
			SubmitCustomGames.IsEnabled = Games.HasUnsubmittedGames;
        }
		



        private void folderChoice(object sender, EventArgs e) {
			
            if (e != null) {
				SuperButtonEventArgs ev = (SuperButtonEventArgs)e;
				ComboBoxItem item = (ComboBoxItem)ev.SelectedItem;
                AddGameLocation.Value = openFolderPicker(item.Tag, Strings.GetLabelString("SelectSaveFolder"));
			} else {
                AddGameLocation.Value = openFolderPicker(null, Strings.GetLabelString("SelectSaveFolder"));
			}
			// The user could end up selecting a custom game, gotta mark that shit
			SubmitCustomGames.IsEnabled = Games.HasUnsubmittedGames;
		}

		private void otherFolderClick(object sender, EventArgs e) {
			string path = "";
			if (e != null) {
				RibbonMenuItem item = (RibbonMenuItem)sender;
                path = openFolderPicker(item.Tag, Strings.GetLabelString("SelectSaveFolder"));
			} else {
                path = openFolderPicker(null, Strings.GetLabelString("SelectSaveFolder"));
			}

			if (string.IsNullOrEmpty(path)) {
				return;
			}

			CustomGameEntry game = Games.addCustomGame(System.Guid.NewGuid().ToString(), new System.IO.DirectoryInfo(path), "", "");

			createGameSubmission(game);

			Games.deleteCustomGame(game);
		}

		private void SubmitSelectedGames_Click(object sender, RoutedEventArgs e) {
			submitting_games.Clear();

			foreach (GameEntry game in gamesLst.SelectedItems) {
				if (game.IsDetected) {
					submitting_games.Enqueue(game);
				}
			}

			submitPromptSuppress = false;

			askAboutGame(false);
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
                AddGameButton.IsEnabled = !String.IsNullOrEmpty(AddGameTitle.Value) && !String.IsNullOrEmpty(AddGameLocation.Value);
        }


        private void AddGameButton_Click(object sender, RoutedEventArgs e) {
            CustomGameEntry game = Games.addCustomGame(AddGameTitle.Value, new System.IO.DirectoryInfo(AddGameLocation.Value), AddGameSaves.Value, AddGameExclusions.Value);
            if (!Core.settings.SuppressSubmitRequests) {
                RequestReply reply = TranslatingRequestHandler.Request(RequestType.Question, "PleaseSubmitGame", true);
                if (!reply.Cancelled) {
                    createGameSubmission(game);
                } else {
                    if (reply.Suppressed)
                        Core.settings.SuppressSubmitRequests = true;
                }
            }
            closeAddGame(true);
			SubmitCustomGames.IsEnabled = Games.HasUnsubmittedGames;
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

			SubmitCustomGames.IsEnabled = Games.HasUnsubmittedGames;
        }

        Queue<GameEntry> submitting_games = new Queue<GameEntry>();

        private void submitGame_Click(object sender, RoutedEventArgs e) {
            submitting_games.Clear();
            submitting_games = Games.UnsubmittedGames;

            submitPromptSuppress = false;

            askAboutGame(true);
        }

        protected void createGameSubmission(GameEntry game) {
            analyzer = new PCAnalyzer(game, gameSubmissionDone);
            disableInterface(analyzer);
            analyzer.analyze();
        }

        private bool submitPromptSuppress = false;

        private bool askAboutGame(bool allow_suppress) {
            if (!allow_suppress||!submitPromptSuppress) {
                while (submitting_games.Count > 0) {
                    GameEntry game = submitting_games.Peek();
					RequestReply reply = TranslatingRequestHandler.Request(RequestType.Question, "AskSubmitGame", allow_suppress, game.Title);
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
            if (report.SentOrSaved&& analyzer.game is CustomGameEntry) {
				CustomGameEntry entry = (CustomGameEntry)analyzer.game;
                entry.Submitted = true;
                Games.saveCustomGames();
            }
            if (askAboutGame(true))
                return;

			SubmitCustomGames.IsEnabled = Games.HasUnsubmittedGames;

            this.enableInterface();
        }

    }
}
