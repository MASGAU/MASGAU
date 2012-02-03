namespace Masgau
{
    partial class Splash
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
			this.detectingProgress = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// detectingProgress
			// 
			this.detectingProgress.Location = new System.Drawing.Point(0, 0);
			this.detectingProgress.Name = "detectingProgress";
			this.detectingProgress.Size = new System.Drawing.Size(390, 34);
			this.detectingProgress.Step = 1;
			this.detectingProgress.TabIndex = 1;
			// 
			// Splash
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(390, 35);
			this.Controls.Add(this.detectingProgress);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Splash";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MASGAU Is Detecting Stuff";
			this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ProgressBar detectingProgress;
    }
}