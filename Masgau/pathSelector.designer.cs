namespace MASGAU
{
    partial class pathSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(pathSelector));
            this.pathCombo = new System.Windows.Forms.ComboBox();
            this.randomeLAbel = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.randomeLAbel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pathCombo
            // 
            this.pathCombo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pathCombo.FormattingEnabled = true;
            this.pathCombo.Location = new System.Drawing.Point(3, 16);
            this.pathCombo.Name = "pathCombo";
            this.pathCombo.Size = new System.Drawing.Size(396, 21);
            this.pathCombo.TabIndex = 0;
            // 
            // randomeLAbel
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.randomeLAbel, 2);
            this.randomeLAbel.Controls.Add(this.pathCombo);
            this.randomeLAbel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.randomeLAbel.Location = new System.Drawing.Point(3, 3);
            this.randomeLAbel.Name = "randomeLAbel";
            this.randomeLAbel.Size = new System.Drawing.Size(402, 41);
            this.randomeLAbel.TabIndex = 1;
            this.randomeLAbel.TabStop = false;
            this.randomeLAbel.Text = "Please Pick A Path To Remove";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(198, 24);
            this.button1.TabIndex = 2;
            this.button1.Text = "There Is No Need For This One";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(207, 50);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(198, 24);
            this.button2.TabIndex = 3;
            this.button2.Text = "On Second Thought...";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.randomeLAbel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(408, 77);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // pathSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 77);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "pathSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Remove A Path";
            this.randomeLAbel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox randomeLAbel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.ComboBox pathCombo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}