using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Masgau
{
    public partial class manualBackup : Form
    {
		private GameData game;
		private bool working = false, changing = false;
        public manualBackup(GameData new_game)
        {
            InitializeComponent();
			game = new_game;
			foreach(file_holder file in game.detected_roots) {
				if(file.owner!=null)
					rootCombo.Items.Add(file.owner);
				else
					rootCombo.Items.Add("Global");
			}
			if(rootCombo.Items.Contains(Environment.UserName))
				rootCombo.SelectedIndex = rootCombo.Items.IndexOf(Environment.UserName);
			else 
				rootCombo.SelectedIndex = 0;
        }


		private void recurseThatShit(TreeNode set_this) {
			for(int i = 0; i<set_this.Nodes.Count;i++) {
				changing = true;
				set_this.Nodes[i].Checked = set_this.Checked;
				changing = false;
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
				changing = true;
				set_this.Parent.Checked = all_checked;
				changing = false;
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
			foreach(file_holder file in game.getSaves()) {
				if(file.absolute_root==((file_holder)game.detected_roots[rootCombo.SelectedIndex]).absolute_path) {
					if(!fileTree.Nodes.ContainsKey(file.absolute_root)) {
						fileTree.Nodes.Add(file.absolute_root,file.absolute_root);
						fileTree.Nodes[file.absolute_root].Checked = true;
						fileTree.Nodes[file.absolute_root].ToolTipText = file.absolute_root;
					}
					fileTree.SelectedNode = fileTree.Nodes[file.absolute_root];
					if(file.absolute_path!=null&&file.absolute_path!="") {
						foreach(string path_segment in file.absolute_path.Split(Path.DirectorySeparatorChar)) {
							if(!fileTree.SelectedNode.Nodes.ContainsKey(path_segment)) {
								fileTree.SelectedNode.Nodes.Add(path_segment,path_segment);
								fileTree.SelectedNode.Nodes[path_segment].Checked = true;
							}
							fileTree.SelectedNode = fileTree.SelectedNode.Nodes[path_segment];
						}
					}
					fileTree.SelectedNode.Nodes.Add(file.file_name,file.file_name);
					fileTree.SelectedNode.Nodes[file.file_name].Checked = true;
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