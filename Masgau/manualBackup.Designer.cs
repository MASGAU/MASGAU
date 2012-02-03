namespace Masgau
{
    partial class manualBackup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(manualBackup));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.deselectButton = new System.Windows.Forms.Button();
			this.selectAllButton = new System.Windows.Forms.Button();
			this.fileTree = new System.Windows.Forms.TreeView();
			this.nextButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rootCombo = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.deselectButton);
			this.groupBox1.Controls.Add(this.selectAllButton);
			this.groupBox1.Controls.Add(this.fileTree);
			this.groupBox1.Location = new System.Drawing.Point(10, 68);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(396, 233);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Which Files Will Go Into The Backup?";
			// 
			// deselectButton
			// 
			this.deselectButton.Location = new System.Drawing.Point(292, 204);
			this.deselectButton.Name = "deselectButton";
			this.deselectButton.Size = new System.Drawing.Size(98, 23);
			this.deselectButton.TabIndex = 2;
			this.deselectButton.Text = "Deselect All Files";
			this.deselectButton.UseVisualStyleBackColor = true;
			this.deselectButton.Click += new System.EventHandler(this.deselectButton_Click);
			// 
			// selectAllButton
			// 
			this.selectAllButton.Enabled = false;
			this.selectAllButton.Location = new System.Drawing.Point(6, 204);
			this.selectAllButton.Name = "selectAllButton";
			this.selectAllButton.Size = new System.Drawing.Size(87, 23);
			this.selectAllButton.TabIndex = 1;
			this.selectAllButton.Text = "Select All Files";
			this.selectAllButton.UseVisualStyleBackColor = true;
			this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
			// 
			// fileTree
			// 
			this.fileTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.fileTree.CheckBoxes = true;
			this.fileTree.Location = new System.Drawing.Point(6, 19);
			this.fileTree.Name = "fileTree";
			this.fileTree.ShowNodeToolTips = true;
			this.fileTree.ShowRootLines = false;
			this.fileTree.Size = new System.Drawing.Size(384, 179);
			this.fileTree.TabIndex = 0;
			this.fileTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.fileTree_AfterCheck);
			this.fileTree.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.fileTree_BeforeCheck);
			// 
			// nextButton
			// 
			this.nextButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.nextButton.Location = new System.Drawing.Point(126, 307);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(117, 23);
			this.nextButton.TabIndex = 2;
			this.nextButton.Text = "Archive These Files";
			this.nextButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(249, 307);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(157, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Too Complicated! I\'m Scared!";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rootCombo);
			this.groupBox2.Location = new System.Drawing.Point(10, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(396, 50);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Which User Do You Want To Get Saves From?";
			// 
			// rootCombo
			// 
			this.rootCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.rootCombo.FormattingEnabled = true;
			this.rootCombo.Location = new System.Drawing.Point(6, 19);
			this.rootCombo.Name = "rootCombo";
			this.rootCombo.Size = new System.Drawing.Size(384, 21);
			this.rootCombo.TabIndex = 0;
			this.rootCombo.SelectedIndexChanged += new System.EventHandler(this.rootCombo_SelectedIndexChanged);
			// 
			// manualBackup
			// 
			this.AcceptButton = this.nextButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(418, 334);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.nextButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "manualBackup";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "One Custom Backup, Coming Up";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button deselectButton;
		private System.Windows.Forms.Button selectAllButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox rootCombo;
		public System.Windows.Forms.TreeView fileTree;
    }
}