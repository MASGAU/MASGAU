﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MASGAU.Location.Holders;
using MVC;
using Translator;
using Translator.WPF;
namespace MASGAU.Main {
    /// <summary>
    /// Interaction logic for ManualArchiveWindow.xaml
    /// </summary>
    /// 

    public partial class ManualArchiveWindow : AWindow {
        private GameEntry game;


        private FileTreeViewItem file_tree;

        public ManualArchiveWindow(GameEntry new_game, IWindow owner)
            : base(owner) {
            InitializeComponent();
            TranslationHelpers.translateWindow(this);
            game = new_game;

            rootCombo.Items.Clear();

            foreach (DetectedLocationPathHolder file in game.DetectedLocations) {
                //if(file.Value.owner!=null)
                //    rootCombo.Items.Add(file.Value.owner);
                //else
                //    rootCombo.Items.Add("Global");
                rootCombo.Items.Add(file.full_dir_path);
            }
            if (rootCombo.Items.Contains(Environment.UserName))
                rootCombo.SelectedIndex = rootCombo.Items.IndexOf(Environment.UserName);
            else
                rootCombo.SelectedIndex = 0;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {

            this.DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }


        public List<DetectedFile> getSelectedFiles() {
            return traverseTree(file_tree);
            ;
        }

        private List<DetectedFile> traverseTree(FileTreeViewItem item) {
            List<DetectedFile> return_me = new List<DetectedFile>();
            foreach (FileTreeViewItem sub_item in item.Children) {
                if (sub_item.IsChecked != null && sub_item.IsChecked == true && sub_item.file != null) {
                    return_me.Add(sub_item.file);
                }
                if (sub_item.Children.Count > 0) {
                    return_me.AddRange(traverseTree(sub_item));
                }
            }
            return return_me;
        }


        private void rootCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            file_tree = new FileTreeViewItem(null);
            file_tree.PropertyChanged += new PropertyChangedEventHandler(file_tree_PropertyChanged);

            List<DetectedFile> saves = game.Saves.Flatten();

            // This gets every detected save file
            foreach (DetectedFile save in saves) {
                // This tests if the save is from the currently selected root folder
                if (save.AbsoluteRoot == game.DetectedLocations[rootCombo.SelectedItem.ToString()].full_dir_path) {
                    string path = Path.Combine(save.Path, save.Name);
                    file_tree.addFile(new List<string>(path.Split(Path.DirectorySeparatorChar)), save);
                    // Splits the path into folders
                }
            }


            if (file_tree.Children.Count == 0) {
                CheckedTreeViewItem nofiles = new CheckedTreeViewItem(null);
                nofiles.Name = Strings.GetLabelString("NoFilesFound");
                file_tree.Children.Add(nofiles);
                fileTree.IsEnabled = false;
                saveButton.IsEnabled = false;
            } else {
                fileTree.IsEnabled = true;
                saveButton.IsEnabled = true;
            }
            fileTree.DataContext = file_tree;

        }

        void file_tree_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (getSelectedFiles().Count > 0) {
                saveButton.IsEnabled = true;
            } else {
                saveButton.IsEnabled = false;
            }
        }


    }
}
