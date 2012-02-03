using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MASGAU
{
    public partial class manualBackup : Form
    {
		private GameData game;
		private bool working = false;
        private invokes invokes = new invokes();
        public manualBackup(GameData new_game)
        {
            InitializeComponent();
			game = new_game;
			foreach(KeyValuePair<string,location_holder> file in game.detected_locations) {
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

            invokes.setControlTheme(fileTree, "explorer");

        }


		private void recurseThatShit(TreeNode set_this) {
			for(int i = 0; i<set_this.Nodes.Count;i++) {
				set_this.Nodes[i].Checked = set_this.Checked;
				recurseThatShit(set_this.Nodes[i]);
			}
		}

		private void decurseThatShit(TreeNode set_this) {
			if(set_this.Parent!=null) {
				bool all_checked = true;
				foreach(TreeNode node in set_this.Parent.Nodes) {
					if(!node.Checked)
						all_checked = false;
				}
				set_this.Parent.Checked = all_checked;
				decurseThatShit(set_this.Parent);
			}
		}

		private bool anythingSelected(TreeNodeCollection nodes) {
			for(int i = 0;i<nodes.Count;i++) {
				if(nodes[i].Checked||anythingSelected(nodes[i].Nodes))
					return true;
			}
			return false;
		}

		private void selectAllButton_Click(object sender, EventArgs e)
		{
			if(!working) {
				working = true;
				for(int i = 0;i<fileTree.Nodes.Count;i++) {
					fileTree.Nodes[i].Checked = true;
					recurseThatShit(fileTree.Nodes[i]);
				}
				deselectButton.Enabled = true;
				selectAllButton.Enabled = false;
				nextButton.Enabled = true;
				working = false;
			}
		}

		private void deselectButton_Click(object sender, EventArgs e)
		{
			if(!working) {
				working = true;
				for(int i = 0;i<fileTree.Nodes.Count;i++) {
					fileTree.Nodes[i].Checked = false;
					recurseThatShit(fileTree.Nodes[i]);
				}
				selectAllButton.Enabled = true;
				deselectButton.Enabled = false;
				nextButton.Enabled = false;
				working = false;
			}
		}

		private void rootCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			fileTree.Nodes.Clear();
			fileTree.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(fileTree_AfterCheck);
			fileTree.BeforeCheck -= new System.Windows.Forms.TreeViewCancelEventHandler(fileTree_BeforeCheck);
            ArrayList saves = game.getSaves();
            // This gets every detected save file
			foreach(file_holder save in saves) {
                // This tests if the save is from the currently selected root folder
				if(save.root==game.detected_locations[rootCombo.SelectedItem.ToString()].getFullPath()) {
                    // Since this all happens every file, this checks if the root folder node is already made
					if(!fileTree.Nodes.ContainsKey(save.root)) {
                        // Makes it if it isn't
						fileTree.Nodes.Add(save.root,save.root);
						fileTree.Nodes[save.root].Checked = true;
						fileTree.Nodes[save.root].ToolTipText = save.root;
					}
                    // Selects the root node fo realz
                    // This makes the next created node created under the selected node
					fileTree.SelectedNode = fileTree.Nodes[save.root];
                    // Checks if there is a path at all
					if(save.path!=null&&save.path!="") {
                        // Splits the path into folders
						foreach(string path_segment in save.path.Split(Path.DirectorySeparatorChar)) {
                            // Checks if the current folder in the path has a node
							if(!fileTree.SelectedNode.Nodes.ContainsKey(path_segment)) {
                                // Creates it if it doesn't
								fileTree.SelectedNode.Nodes.Add(path_segment,path_segment);
								fileTree.SelectedNode.Nodes[path_segment].Checked = true;
							}
                            // Move the node selection to the next path folder down
							fileTree.SelectedNode = fileTree.SelectedNode.Nodes[path_segment];
						}
					}
                    // Adds the node for the actual file. Finally!
					fileTree.SelectedNode.Nodes.Add(save.name,save.name);
					fileTree.SelectedNode.Nodes[save.name].Checked = true;
				}
			}
			fileTree.ExpandAll();
			if(fileTree.Nodes.Count==0) {
				fileTree.Nodes.Add("No Files Found");
				fileTree.Enabled = false;
				nextButton.Enabled = false;
			} else {
				fileTree.Enabled = true;
				nextButton.Enabled = true;
			}
			fileTree.SelectedNode = fileTree.Nodes[0];
			fileTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(fileTree_AfterCheck);
			fileTree.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(fileTree_BeforeCheck);

		}



		private void fileTree_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if(e.Action != TreeViewAction.Unknown) {
				working = true;
				recurseThatShit(e.Node);
				decurseThatShit(e.Node);

				selectAllButton.Enabled = false;
				for(int i = 0;i<fileTree.Nodes.Count;i++) {
					if(!fileTree.Nodes[i].Checked)
						selectAllButton.Enabled = true;
				}
				if(anythingSelected(fileTree.Nodes)) {
					deselectButton.Enabled = true;
					nextButton.Enabled = true;
				} else {
					deselectButton.Enabled = false;
					nextButton.Enabled = false;
				}
				working = false;
			}

		}

		private void fileTree_BeforeCheck(object sender, TreeViewCancelEventArgs e)
		{
			if(working&&e.Action != TreeViewAction.Unknown) {
				e.Cancel = true;
			}
		}

    }
}