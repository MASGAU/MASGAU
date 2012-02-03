namespace MASGAU
{
    partial class versioningFrequencyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(versioningFrequencyForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.duplicateFrequencyBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.duplicateFrequencyNumber = new System.Windows.Forms.NumericUpDown();
            this.duplicateFrequencyCombo = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.duplicateFrequencyBox.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.duplicateFrequencyNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.duplicateFrequencyBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.button2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(246, 77);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // duplicateFrequencyBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.duplicateFrequencyBox, 2);
            this.duplicateFrequencyBox.Controls.Add(this.tableLayoutPanel10);
            this.duplicateFrequencyBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.duplicateFrequencyBox.Location = new System.Drawing.Point(3, 3);
            this.duplicateFrequencyBox.Name = "duplicateFrequencyBox";
            this.duplicateFrequencyBox.Size = new System.Drawing.Size(240, 41);
            this.duplicateFrequencyBox.TabIndex = 2;
            this.duplicateFrequencyBox.TabStop = false;
            this.duplicateFrequencyBox.Text = "At Least This Long Between Copies";
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.duplicateFrequencyNumber, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.duplicateFrequencyCombo, 1, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(234, 22);
            this.tableLayoutPanel10.TabIndex = 2;
            // 
            // duplicateFrequencyNumber
            // 
            this.duplicateFrequencyNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.duplicateFrequencyNumber.Location = new System.Drawing.Point(3, 0);
            this.duplicateFrequencyNumber.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.duplicateFrequencyNumber.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.duplicateFrequencyNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.duplicateFrequencyNumber.Name = "duplicateFrequencyNumber";
            this.duplicateFrequencyNumber.Size = new System.Drawing.Size(111, 20);
            this.duplicateFrequencyNumber.TabIndex = 0;
            this.duplicateFrequencyNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // duplicateFrequencyCombo
            // 
            this.duplicateFrequencyCombo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.duplicateFrequencyCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.duplicateFrequencyCombo.FormattingEnabled = true;
            this.duplicateFrequencyCombo.Items.AddRange(new object[] {
            "Seconds",
            "Minutes",
            "Hours",
            "Days",
            "Weeks",
            "Months",
            "Years",
            "Decades",
            "Centuries",
            "Millenia"});
            this.duplicateFrequencyCombo.Location = new System.Drawing.Point(120, 0);
            this.duplicateFrequencyCombo.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.duplicateFrequencyCombo.Name = "duplicateFrequencyCombo";
            this.duplicateFrequencyCombo.Size = new System.Drawing.Size(111, 21);
            this.duplicateFrequencyCombo.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(126, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 24);
            this.button1.TabIndex = 3;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(3, 50);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(117, 24);
            this.button2.TabIndex = 4;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // versioningFrequencyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 77);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "versioningFrequencyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "I Want It To Be";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.duplicateFrequencyBox.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.duplicateFrequencyNumber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox duplicateFrequencyBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.NumericUpDown duplicateFrequencyNumber;
        private System.Windows.Forms.ComboBox duplicateFrequencyCombo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;


    }
}