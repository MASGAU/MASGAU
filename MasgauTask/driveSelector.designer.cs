namespace Masgau
{
    partial class driveSelector
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
            this.driveCombo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.notToBe = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // driveCombo
            // 
            this.driveCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveCombo.FormattingEnabled = true;
            this.driveCombo.Location = new System.Drawing.Point(6, 19);
            this.driveCombo.Name = "driveCombo";
            this.driveCombo.Size = new System.Drawing.Size(132, 21);
            this.driveCombo.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.driveCombo);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(144, 48);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "To Restore The Save To";
            // 
            // notToBe
            // 
            this.notToBe.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.notToBe.Location = new System.Drawing.Point(81, 66);
            this.notToBe.Name = "notToBe";
            this.notToBe.Size = new System.Drawing.Size(75, 23);
            this.notToBe.TabIndex = 2;
            this.notToBe.Text = "Don\'t Do It!";
            this.notToBe.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(12, 66);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(63, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Do It!";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // driveSelector
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.notToBe;
            this.ClientSize = new System.Drawing.Size(169, 106);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.notToBe);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "driveSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please Select A Disk";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button notToBe;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.ComboBox driveCombo;
    }
}