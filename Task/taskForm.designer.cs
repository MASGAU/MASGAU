namespace MASGAU
{
    partial class taskForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(taskForm));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.currentProgress = new wyDay.Controls.Windows7ProgressBar();
            this.cancelButton = new System.Windows.Forms.Button();
            this.overallProgress = new wyDay.Controls.Windows7ProgressBar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "MASGAU Is Touching Your Stuff";
            this.notifyIcon1.Visible = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.currentProgress);
            this.groupBox1.Controls.Add(this.overallProgress);
            this.groupBox1.Controls.Add(this.cancelButton);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 3, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(387, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Gonna do something";
            // 
            // currentProgress
            // 
            this.currentProgress.ContainerControl = this;
            this.currentProgress.Location = new System.Drawing.Point(7, 28);
            this.currentProgress.Name = "currentProgress";
            this.currentProgress.Size = new System.Drawing.Size(302, 14);
            this.currentProgress.State = wyDay.Controls.ProgressBarState.Pause;
            this.currentProgress.TabIndex = 2;
            this.currentProgress.Visible = false;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(315, 19);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(65, 32);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // overallProgress
            // 
            this.overallProgress.ContainerControl = this;
            this.overallProgress.Location = new System.Drawing.Point(9, 19);
            this.overallProgress.Name = "overallProgress";
            this.overallProgress.ShowInTaskbar = true;
            this.overallProgress.Size = new System.Drawing.Size(298, 32);
            this.overallProgress.State = wyDay.Controls.ProgressBarState.Error;
            this.overallProgress.TabIndex = 1;
            // 
            // taskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 71);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "taskForm";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MASGAU is...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.backendForm_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cancelButton;
        private wyDay.Controls.Windows7ProgressBar currentProgress;
        private wyDay.Controls.Windows7ProgressBar overallProgress;
    }
}

