namespace Masgau
{
    partial class monitorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(monitorForm));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.monitorNotifier = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifierMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rescanGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifierMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(-1, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(331, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // monitorNotifier
            // 
            this.monitorNotifier.ContextMenuStrip = this.notifierMenu;
            this.monitorNotifier.Icon = ((System.Drawing.Icon)(resources.GetObject("monitorNotifier.Icon")));
            this.monitorNotifier.Text = "MASGAU Monitor";
            this.monitorNotifier.Visible = true;
            // 
            // notifierMenu
            // 
            this.notifierMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanGamesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.notifierMenu.Name = "notifierMenu";
            this.notifierMenu.Size = new System.Drawing.Size(160, 48);
            // 
            // rescanGamesToolStripMenuItem
            // 
            this.rescanGamesToolStripMenuItem.Name = "rescanGamesToolStripMenuItem";
            this.rescanGamesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.rescanGamesToolStripMenuItem.Text = "Rescan Games...";
            this.rescanGamesToolStripMenuItem.Click += new System.EventHandler(this.rescanGamesToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // exitMonitorToolStripMenuItem
            // 
            this.exitMonitorToolStripMenuItem.Name = "exitMonitorToolStripMenuItem";
            this.exitMonitorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitMonitorToolStripMenuItem.Text = "Exit Monitor";
            // 
            // monitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 23);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "monitorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MASGAU Monitor Is Detecting Save Paths...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.monitorForm_FormClosing);
            this.notifierMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.NotifyIcon monitorNotifier;
        private System.Windows.Forms.ContextMenuStrip notifierMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rescanGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

