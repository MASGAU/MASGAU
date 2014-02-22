using System;
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
using Microsoft.Windows.Controls.Ribbon;
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
            SteamUserData,
            VirtualStore,
            PublicUser
        }

        private void addGameSetup() {

            AddGameLocation.Button.clearOptions();
			SubmitOther.Items.Clear();
            //            AddGameLocation.ButtonText = Strings.GetLabelString(AddGameLocation.ButtonText);
            Array values = Enum.GetValues(typeof(game_locations));
            foreach (game_locations val in values) {
                if ((val == game_locations.Steamapps || val== game_locations.SteamUserData )&& !Core.locations.steam_detected)
                    continue;
                else if (val == game_locations.ProgramFilesX86 && Core.locations.getFolder(EnvironmentVariable.ProgramFilesX86, null) == null)
                    continue;
                else if ((val == game_locations.PublicUser || val == game_locations.VirtualStore || val == game_locations.SavedGames) &&
                    Core.locations.platform_version == "WindowsXP")
                    continue;

				AddPathToButtons(val.ToString(),val);
            }

			// Adds alternative save paths
			foreach (Location.Holders.AltPathHolder path in Core.settings.save_paths) {
				AddPathToButtons(abbreviateString(path.path,50), path.path);
			}

	
			// Adds alternative steam library paths
			foreach (string  path in Core.locations.getPaths(EnvironmentVariable.SteamCommon)) {
				if (!path.ToLower().StartsWith(Path.Combine(Core.settings.steam_path, "steamapps").ToLower())) {
					AddPathToButtons(abbreviateString(path,50), path);
				}
			}


			SubmitCustomGames.IsEnabled = Games.HasUnsubmittedGames;
        }
		
		private string abbreviateString(string input, int max_length) {
			if (input.Length <= max_length) {
				return input;
			}
			int diff = input.Length - max_length;
			double half_max_length = (max_length-3) / 2;

			string pre = input.Substring(0, Convert.ToInt32(Math.Floor(half_max_length)));
			string post = input.Substring(input.Length - Convert.ToInt32(Math.Ceiling(half_max_length)));
			return String.Concat(pre, "...", post);
		}

		private void AddPathToButtons(string label, object val) {
			ComboBoxItem item = new ComboBoxItem();
			item.Content = Strings.GetLabelString(label);
			item.Tag = val;
			helpone.ToolTip = Strings.GetToolTipString("AddGameSaves");
			helptwo.ToolTip = Strings.GetToolTipString("AddGameSaves");
			AddGameLocation.Button.addOption(item, new EventHandler(folderChoice));

			RibbonMenuItem menu_item = new RibbonMenuItem();
			menu_item.Header = Strings.GetLabelString(label);
			menu_item.Tag = val;
			menu_item.Click += otherFolderClick;
			SubmitOther.Items.Add(menu_item);
		}


        private void folderChoice(object sender, EventArgs e) {
			
            if (e != null) {
				SuperButtonEventArgs ev = (SuperButtonEventArgs)e;
				ComboBoxItem item = (ComboBoxItem)ev.SelectedItem;
				AddGameLocation.Value = openFolderPicker(item.Tag);
			} else {
				AddGameLocation.Value = openFolderPicker(game_locations.MyComputer);
			}
			// The user could end up selecting a custom game, gotta mark that shit
			SubmitCustomGames.IsEnabled = Games.HasUnsubmittedGames;
		}

		private void otherFolderClick(object sender, EventArgs e) {
			string path = "";
			if (e != null) {
				RibbonMenuItem item = (RibbonMenuItem)sender;
								path = openFolderPicker(item.Tag);
			} else {
				path = openFolderPicker(game_locations.MyComputer);
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

		private string openFolderPicker(object loc) {
			Environment.SpecialFolder folder = Environment.SpecialFolder.MyComputer;
			string default_folder = null;

			if (loc is game_locations) {
				switch ((game_locations)loc) {
					case game_locations.MyComputer:
						break;
					case game_locations.ProgramFiles:
						folder = Environment.SpecialFolder.ProgramFiles;
						break;
					case game_locations.ProgramFilesX86:
						folder = Environment.SpecialFolder.ProgramFilesX86;
						break;
					case game_locations.Steamapps:
						default_folder = Path.Combine(Core.settings.steam_path, "steamapps");
						break;
					case game_locations.SteamUserData:
						default_folder = Path.Combine(Core.settings.steam_path, "userdata");
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
					default:
						throw new NotImplementedException("The requested folder is not known: " + loc.ToString());
				}
			} else {
				default_folder = loc.ToString();
			}
            return this.promptForPath(Strings.GetLabelString("SelectSaveFolder"), folder, default_folder);
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
