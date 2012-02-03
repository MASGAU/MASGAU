namespace Masgau
{
    partial class settingsForm
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.dateCheck = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.altPathList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openBackupPath = new System.Windows.Forms.Button();
            this.backupPathInput = new System.Windows.Forms.TextBox();
            this.backupPathButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.steamClearButton = new System.Windows.Forms.Button();
            this.steamPathInput = new System.Windows.Forms.TextBox();
            this.steamPathButton = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(244, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(181, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Start MASGAU Monitor On Login";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // dateCheck
            // 
            this.dateCheck.AutoSize = true;
            this.dateCheck.Location = new System.Drawing.Point(11, 12);
            this.dateCheck.Name = "dateCheck";
            this.dateCheck.Size = new System.Drawing.Size(161, 17);
            this.dateCheck.TabIndex = 10;
            this.dateCheck.Text = "Ignore Dates During Backup";
            this.dateCheck.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.altPathList);
            this.groupBox3.Location = new System.Drawing.Point(11, 244);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(432, 166);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Alternate Install Paths (Double-click to remove)";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 137);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(420, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add New Path!";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // altPathList
            // 
            this.altPathList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.altPathList.FullRowSelect = true;
            this.altPathList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.altPathList.Location = new System.Drawing.Point(6, 20);
            this.altPathList.Name = "altPathList";
            this.altPathList.Size = new System.Drawing.Size(420, 111);
            this.altPathList.TabIndex = 0;
            this.altPathList.UseCompatibleStateImageBehavior = false;
            this.altPathList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Path";
            this.columnHeader1.Width = 400;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.openBackupPath);
            this.groupBox2.Controls.Add(this.backupPathInput);
            this.groupBox2.Controls.Add(this.backupPathButton);
            this.groupBox2.Location = new System.Drawing.Point(11, 144);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(432, 44);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Backup Path";
            // 
            // openBackupPath
            // 
            this.openBackupPath.Enabled = false;
            this.openBackupPath.Location = new System.Drawing.Point(322, 15);
            this.openBackupPath.Name = "openBackupPath";
            this.openBackupPath.Size = new System.Drawing.Size(43, 23);
            this.openBackupPath.TabIndex = 2;
            this.openBackupPath.Text = "Open";
            this.openBackupPath.UseVisualStyleBackColor = true;
            // 
            // backupPathInput
            // 
            this.backupPathInput.Enabled = false;
            this.backupPathInput.Location = new System.Drawing.Point(6, 17);
            this.backupPathInput.Margin = new System.Windows.Forms.Padding(2);
            this.backupPathInput.Name = "backupPathInput";
            this.backupPathInput.Size = new System.Drawing.Size(311, 20);
            this.backupPathInput.TabIndex = 1;
            // 
            // backupPathButton
            // 
            this.backupPathButton.Location = new System.Drawing.Point(370, 15);
            this.backupPathButton.Margin = new System.Windows.Forms.Padding(2);
            this.backupPathButton.Name = "backupPathButton";
            this.backupPathButton.Size = new System.Drawing.Size(56, 23);
            this.backupPathButton.TabIndex = 0;
            this.backupPathButton.Text = "Change";
            this.backupPathButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.steamClearButton);
            this.groupBox1.Controls.Add(this.steamPathInput);
            this.groupBox1.Controls.Add(this.steamPathButton);
            this.groupBox1.Location = new System.Drawing.Point(11, 192);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(432, 47);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Steam Path";
            // 
            // steamClearButton
            // 
            this.steamClearButton.Location = new System.Drawing.Point(382, 15);
            this.steamClearButton.Name = "steamClearButton";
            this.steamClearButton.Size = new System.Drawing.Size(44, 23);
            this.steamClearButton.TabIndex = 2;
            this.steamClearButton.Text = "Reset";
            this.steamClearButton.UseVisualStyleBackColor = true;
            // 
            // steamPathInput
            // 
            this.steamPathInput.Enabled = false;
            this.steamPathInput.Location = new System.Drawing.Point(6, 17);
            this.steamPathInput.Margin = new System.Windows.Forms.Padding(2);
            this.steamPathInput.Name = "steamPathInput";
            this.steamPathInput.Size = new System.Drawing.Size(311, 20);
            this.steamPathInput.TabIndex = 1;
            // 
            // steamPathButton
            // 
            this.steamPathButton.Location = new System.Drawing.Point(321, 15);
            this.steamPathButton.Margin = new System.Windows.Forms.Padding(2);
            this.steamPathButton.Name = "steamPathButton";
            this.steamPathButton.Size = new System.Drawing.Size(56, 23);
            this.steamPathButton.TabIndex = 0;
            this.steamPathButton.Text = "Change";
            this.steamPathButton.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(6, 0);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(160, 17);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "Enable Redundant Backups";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(6, 19);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 13;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Controls.Add(this.checkBox2);
            this.groupBox4.Location = new System.Drawing.Point(11, 39);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(377, 89);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.numericUpDown1);
            this.groupBox5.Location = new System.Drawing.Point(6, 23);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(174, 48);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "groupBox5";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.numericUpDown2);
            this.groupBox6.Location = new System.Drawing.Point(186, 23);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(131, 48);
            this.groupBox6.TabIndex = 14;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "groupBox6";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(6, 19);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown2.TabIndex = 13;
            // 
            // settingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 422);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.dateCheck);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "settingsForm";
            this.Text = "settingsForm";
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox dateCheck;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView altPathList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button openBackupPath;
        private System.Windows.Forms.TextBox backupPathInput;
        private System.Windows.Forms.Button backupPathButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button steamClearButton;
        private System.Windows.Forms.TextBox steamPathInput;
        private System.Windows.Forms.Button steamPathButton;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}