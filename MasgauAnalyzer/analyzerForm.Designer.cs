namespace Masgau
{
	partial class analyzerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(analyzerForm));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.saveFolderButton = new System.Windows.Forms.Button();
            this.saveFolderText = new System.Windows.Forms.TextBox();
            this.submitButton = new System.Windows.Forms.Button();
            this.gamePathBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.gamePathButton = new System.Windows.Forms.Button();
            this.gamePathText = new System.Windows.Forms.TextBox();
            this.savePathBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.playstationDirTxt = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.suffixTxt = new System.Windows.Forms.TextBox();
            this.prefixTxt = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.saveFolderButton);
            this.groupBox3.Controls.Add(this.saveFolderText);
            this.groupBox3.Location = new System.Drawing.Point(6, 64);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(316, 48);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Where Does The Game Keep Its Saves?";
            // 
            // saveFolderButton
            // 
            this.saveFolderButton.Location = new System.Drawing.Point(235, 17);
            this.saveFolderButton.Name = "saveFolderButton";
            this.saveFolderButton.Size = new System.Drawing.Size(75, 23);
            this.saveFolderButton.TabIndex = 1;
            this.saveFolderButton.Text = "Browse";
            this.saveFolderButton.UseVisualStyleBackColor = true;
            this.saveFolderButton.Click += new System.EventHandler(this.saveFolderButton_Click);
            // 
            // saveFolderText
            // 
            this.saveFolderText.Enabled = false;
            this.saveFolderText.Location = new System.Drawing.Point(6, 19);
            this.saveFolderText.Name = "saveFolderText";
            this.saveFolderText.Size = new System.Drawing.Size(223, 20);
            this.saveFolderText.TabIndex = 0;
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(6, 118);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(316, 23);
            this.submitButton.TabIndex = 3;
            this.submitButton.Text = "Scan And Generate Report";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // gamePathBrowser
            // 
            this.gamePathBrowser.Description = "Select the install location of the game";
            this.gamePathBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.gamePathBrowser.ShowNewFolderButton = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.gamePathButton);
            this.groupBox4.Controls.Add(this.gamePathText);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(316, 52);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Where Is The Game Installed?";
            // 
            // gamePathButton
            // 
            this.gamePathButton.Location = new System.Drawing.Point(235, 17);
            this.gamePathButton.Name = "gamePathButton";
            this.gamePathButton.Size = new System.Drawing.Size(75, 23);
            this.gamePathButton.TabIndex = 1;
            this.gamePathButton.Text = "Browse";
            this.gamePathButton.UseVisualStyleBackColor = true;
            this.gamePathButton.Click += new System.EventHandler(this.gamePathButton_Click);
            // 
            // gamePathText
            // 
            this.gamePathText.Enabled = false;
            this.gamePathText.Location = new System.Drawing.Point(6, 19);
            this.gamePathText.Name = "gamePathText";
            this.gamePathText.Size = new System.Drawing.Size(223, 20);
            this.gamePathText.TabIndex = 0;
            // 
            // savePathBrowser
            // 
            this.savePathBrowser.Description = "Select the folder that the saves are kept in";
            this.savePathBrowser.ShowNewFolderButton = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(340, 174);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.submitButton);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(332, 148);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Windows";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(332, 148);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "PlayStation";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.playstationDirTxt);
            this.groupBox2.Location = new System.Drawing.Point(6, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(316, 48);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Where Does The Game Keep Its Saves?";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(235, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Browse";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // playstationDirTxt
            // 
            this.playstationDirTxt.Enabled = false;
            this.playstationDirTxt.Location = new System.Drawing.Point(6, 19);
            this.playstationDirTxt.Name = "playstationDirTxt";
            this.playstationDirTxt.Size = new System.Drawing.Size(223, 20);
            this.playstationDirTxt.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.suffixTxt);
            this.groupBox1.Controls.Add(this.prefixTxt);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(316, 52);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What Is The Game\'s Code?";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(144, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "-";
            // 
            // suffixTxt
            // 
            this.suffixTxt.Location = new System.Drawing.Point(160, 19);
            this.suffixTxt.Name = "suffixTxt";
            this.suffixTxt.Size = new System.Drawing.Size(150, 20);
            this.suffixTxt.TabIndex = 1;
            // 
            // prefixTxt
            // 
            this.prefixTxt.Location = new System.Drawing.Point(6, 19);
            this.prefixTxt.Name = "prefixTxt";
            this.prefixTxt.Size = new System.Drawing.Size(132, 20);
            this.prefixTxt.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 118);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(316, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Scan And Generate Report";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // analyzerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 173);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "analyzerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MASGAU Save Game Analyzer";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button saveFolderButton;
		private System.Windows.Forms.TextBox saveFolderText;
		private System.Windows.Forms.Button submitButton;
		private System.Windows.Forms.FolderBrowserDialog gamePathBrowser;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button gamePathButton;
		private System.Windows.Forms.TextBox gamePathText;
		private System.Windows.Forms.FolderBrowserDialog savePathBrowser;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox suffixTxt;
        private System.Windows.Forms.TextBox prefixTxt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox playstationDirTxt;
	}
}

