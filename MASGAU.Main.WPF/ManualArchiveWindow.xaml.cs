using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MASGAU.Location;
using MASGAU.Location.Holders;
using MASGAU.Game;
using System.IO;
using System.ComponentModel;
using Translator;
namespace MASGAU.Main
{
    /// <summary>
    /// Interaction logic for ManualArchiveWindow.xaml
    /// </summary>
    /// 

    public partial class ManualArchiveWindow : AWindow
    {
		private GameHandler game;


        private FileTreeViewItem file_tree;

        public ManualArchiveWindow(GameHandler new_game, AWindow owner): base(owner)
        {
            InitializeComponent();
            WPFHelpers.translateWindow(this);
			game = new_game;
            
            rootCombo.Items.Clear();

			foreach(KeyValuePair<string,DetectedLocationPathHolder> file in game.detected_locations) {
                //if(file.Value.owner!=null)
                //    rootCombo.Items.Add(file.Value.owner);
                //else
                //    rootCombo.Items.Add("Global");
                rootCombo.Items.Add(file.Key);
			}
			if(rootCombo.Items.Contains(Environment.UserName))
				rootCombo.SelectedIndex = rootCombo.Items.IndexOf(Environment.UserName);
			else 
				rootCombo.SelectedIndex = 0;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        public List<DetectedFile> getSelectedFiles() {
            return traverseTree(file_tree);;
        }

        private List<DetectedFile> traverseTree(FileTreeViewItem item) {
            List<DetectedFile> return_me = new List<DetectedFile>();
            foreach(FileTreeViewItem sub_item in item.Children) {
                if(sub_item.IsChecked!=null&&sub_item.IsChecked==true&&sub_item.file!=null) {
                    return_me.Add(sub_item.file);
                }
                if(sub_item.Children.Count>0) {
                    return_me.AddRange(traverseTree(sub_item));
                }
            }
            return return_me;
        }


        private void rootCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            file_tree = new FileTreeViewItem(null);
            file_tree.PropertyChanged += new PropertyChangedEventHandler(file_tree_PropertyChanged);

            List<DetectedFile> saves = game.getSaves().Flatten();

            // This gets every detected save file
            foreach(DetectedFile save in saves) {
                // This tests if the save is from the currently selected root folder
                if(save.abs_root==game.detected_locations[rootCombo.SelectedItem.ToString()].full_dir_path) {
                    string path = Path.Combine(save.path,save.name);
                    file_tree.addFile(new List<string>(path.Split(Path.DirectorySeparatorChar)),save);
                        // Splits the path into folders
                }
            }


            if(file_tree.Children.Count==0) {
                CheckedTreeViewItem nofiles = new CheckedTreeViewItem(null);
                nofiles.Name=Strings.get("NoFilesFound");
                file_tree.Children.Add(nofiles);
                fileTree.IsEnabled = false;
                saveButton.IsEnabled = false;
            } else {
                fileTree.IsEnabled = true;
                saveButton.IsEnabled = true;
            }
            fileTree.DataContext = file_tree;

        }

        void file_tree_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(getSelectedFiles().Count>0) {
                saveButton.IsEnabled = true;
            } else {
                saveButton.IsEnabled = false;
            }
        }

        
    }
}
