namespace MASGAU
{
    partial class versioningCountForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(versioningCountForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.duplicateCountBox = new System.Windows.Forms.GroupBox();
            this.duplicateCount = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.duplicateCountBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.duplicateCount)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.duplicateCountBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.button2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(262, 77);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // duplicateCountBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.duplicateCountBox, 2);
            this.duplicateCountBox.Controls.Add(this.duplicateCount);
            this.duplicateCountBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.duplicateCountBox.Location = new System.Drawing.Point(3, 3);
            this.duplicateCountBox.Name = "duplicateCountBox";
            this.duplicateCountBox.Size = new System.Drawing.Size(256, 41);
            this.duplicateCountBox.TabIndex = 3;
            this.duplicateCountBox.TabStop = false;
            this.duplicateCountBox.Text = "At Most This Many Copies";
            // 
            // duplicateCount
            // 
            this.duplicateCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.duplicateCount.Location = new System.Drawing.Point(3, 16);
            this.duplicateCount.Maximum = new decimal(new int[] {
            276447231,
            23283,
            0,
            0});
            this.duplicateCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.duplicateCount.Name = "duplicateCount";
            this.duplicateCount.Size = new System.Drawing.Size(250, 20);
            this.duplicateCount.TabIndex = 0;
            this.duplicateCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 24);
            this.button1.TabIndex = 4;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(134, 50);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(125, 24);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // versioningCountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 77);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "versioningCountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "I Want There To Be";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.duplicateCountBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.duplicateCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox duplicateCountBox;
        private System.Windows.Forms.NumericUpDown duplicateCount;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;

    }
}