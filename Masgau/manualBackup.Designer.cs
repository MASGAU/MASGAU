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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.fileTree = new System.Windows.Forms.TreeView();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.deselectButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rootCombo = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(412, 246);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Which Files Will Go Into The Backup?";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.selectAllButton, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.fileTree, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.deselectButton, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(406, 227);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // fileTree
            // 
            this.fileTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileTree.CheckBoxes = true;
            this.tableLayoutPanel2.SetColumnSpan(this.fileTree, 2);
            this.fileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTree.Location = new System.Drawing.Point(3, 3);
            this.fileTree.Name = "fileTree";
            this.fileTree.ShowNodeToolTips = true;
            this.fileTree.ShowRootLines = false;
            this.fileTree.Size = new System.Drawing.Size(400, 192);
            this.fileTree.TabIndex = 0;
            this.fileTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.fileTree_AfterCheck);
            this.fileTree.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.fileTree_BeforeCheck);
            // 
            // selectAllButton
            // 
            this.selectAllButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.selectAllButton.Enabled = false;
            this.selectAllButton.Location = new System.Drawing.Point(3, 201);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(87, 23);
            this.selectAllButton.TabIndex = 1;
            this.selectAllButton.Text = "Select All Files";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // deselectButton
            // 
            this.deselectButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.deselectButton.Location = new System.Drawing.Point(305, 201);
            this.deselectButton.Name = "deselectButton";
            this.deselectButton.Size = new System.Drawing.Size(98, 23);
            this.deselectButton.TabIndex = 2;
            this.deselectButton.Text = "Deselect All Files";
            this.deselectButton.UseVisualStyleBackColor = true;
            this.deselectButton.Click += new System.EventHandler(this.deselectButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.nextButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextButton.Location = new System.Drawing.Point(3, 308);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(203, 23);
            this.nextButton.TabIndex = 2;
            this.nextButton.Text = "Archive These Files";
            this.nextButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cancelButton.Location = new System.Drawing.Point(212, 308);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(203, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Too Complicated! I\'m Scared!";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox2, 2);
            this.groupBox2.Controls.Add(this.rootCombo);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(412, 47);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Which User Do You Want To Get Saves From?";
            // 
            // rootCombo
            // 
            this.rootCombo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rootCombo.FormattingEnabled = true;
            this.rootCombo.Location = new System.Drawing.Point(3, 16);
            this.rootCombo.Name = "rootCombo";
            this.rootCombo.Size = new System.Drawing.Size(406, 21);
            this.rootCombo.TabIndex = 0;
            this.rootCombo.SelectedIndexChanged += new System.EventHandler(this.rootCombo_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.nextButton, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(418, 334);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // manualBackup
            // 
            this.AcceptButton = this.nextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(418, 334);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(424, 358);
            this.Name = "manualBackup";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "One Custom Backup, Coming Up";
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button deselectButton;
		private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.TreeView fileTree;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        public System.Windows.Forms.ComboBox rootCombo;
    }
}