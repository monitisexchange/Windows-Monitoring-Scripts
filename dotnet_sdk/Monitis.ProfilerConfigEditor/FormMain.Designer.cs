namespace Monitis.ProfilerConfigEditor
{
    partial class FormMain
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
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.loadConfigsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewConfigFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelConfigs = new System.Windows.Forms.Panel();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadConfigsToolStripMenuItem,
            this.saveConfigsToolStripMenuItem,
            this.addNewConfigFileToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(708, 24);
            this.menuStripMain.TabIndex = 1;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // loadConfigsToolStripMenuItem
            // 
            this.loadConfigsToolStripMenuItem.Name = "loadConfigsToolStripMenuItem";
            this.loadConfigsToolStripMenuItem.Size = new System.Drawing.Size(97, 20);
            this.loadConfigsToolStripMenuItem.Text = "Reload configs";
            this.loadConfigsToolStripMenuItem.Click += new System.EventHandler(this.LoadConfigsToolStripMenuItemClick);
            // 
            // saveConfigsToolStripMenuItem
            // 
            this.saveConfigsToolStripMenuItem.Name = "saveConfigsToolStripMenuItem";
            this.saveConfigsToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.saveConfigsToolStripMenuItem.Text = "Save configs";
            this.saveConfigsToolStripMenuItem.Click += new System.EventHandler(this.SaveConfigsToolStripMenuItemClick);
            // 
            // addNewConfigFileToolStripMenuItem
            // 
            this.addNewConfigFileToolStripMenuItem.Name = "addNewConfigFileToolStripMenuItem";
            this.addNewConfigFileToolStripMenuItem.Size = new System.Drawing.Size(122, 20);
            this.addNewConfigFileToolStripMenuItem.Text = "Add new config file";
            this.addNewConfigFileToolStripMenuItem.Click += new System.EventHandler(this.AddNewConfigFileToolStripMenuItemClick);
            // 
            // panelConfigs
            // 
            this.panelConfigs.AutoScroll = true;
            this.panelConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConfigs.Location = new System.Drawing.Point(0, 24);
            this.panelConfigs.Name = "panelConfigs";
            this.panelConfigs.Size = new System.Drawing.Size(708, 363);
            this.panelConfigs.TabIndex = 2;
            this.panelConfigs.TabStop = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 387);
            this.Controls.Add(this.panelConfigs);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "FormMain";
            this.Text = "Profiler Config Editor";
            this.Load += new System.EventHandler(this.FormMainLoad);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem loadConfigsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveConfigsToolStripMenuItem;
        private System.Windows.Forms.Panel panelConfigs;
        private System.Windows.Forms.ToolStripMenuItem addNewConfigFileToolStripMenuItem;
    }
}

