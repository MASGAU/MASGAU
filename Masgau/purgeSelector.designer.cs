namespace Masgau
{
    partial class purgeSelector
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
            this.purgeCombo = new System.Windows.Forms.ComboBox();
            this.randomeLAbel = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.randomeLAbel.SuspendLayout();
            this.SuspendLayout();
            // 
            // purgeCombo
            // 
            this.purgeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.purgeCombo.FormattingEnabled = true;
            this.purgeCombo.Location = new System.Drawing.Point(6, 19);
            this.purgeCombo.Name = "purgeCombo";
            this.purgeCombo.Size = new System.Drawing.Size(364, 21);
            this.purgeCombo.TabIndex = 0;
            // 
            // randomeLAbel
            // 
            this.randomeLAbel.Controls.Add(this.purgeCombo);
            this.randomeLAbel.Location = new System.Drawing.Point(11, 12);
            this.randomeLAbel.Name = "randomeLAbel";
            this.randomeLAbel.Size = new System.Drawing.Size(376, 47);
            this.randomeLAbel.TabIndex = 1;
            this.randomeLAbel.TabStop = false;
            this.randomeLAbel.Text = "Please Pick A Root To Purge";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(154, 65);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Let Them Burn";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(254, 65);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(133, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "No! I Changed My Mind!";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // purgeSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 97);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.randomeLAbel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "purgeSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Multiple Roots Detected";
            this.randomeLAbel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox randomeLAbel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.ComboBox purgeCombo;
    }
}