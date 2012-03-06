namespace Monitis.Prototype.UI
{
    partial class MonitorsForm
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
            this.btnNext = new System.Windows.Forms.Button();
            this.btnCreateMonitors = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(278, 122);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(112, 23);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.OnApplyMonitorClick);
            // 
            // btnCreateMonitors
            // 
            this.btnCreateMonitors.Location = new System.Drawing.Point(145, 122);
            this.btnCreateMonitors.Name = "btnCreateMonitors";
            this.btnCreateMonitors.Size = new System.Drawing.Size(112, 23);
            this.btnCreateMonitors.TabIndex = 3;
            this.btnCreateMonitors.Text = "Create monitors";
            this.btnCreateMonitors.UseVisualStyleBackColor = true;
            this.btnCreateMonitors.Click += new System.EventHandler(this.OnAddAzureMonitorClick);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(142, 46);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(0, 13);
            this.lblInfo.TabIndex = 4;
            // 
            // MonitorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 192);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnCreateMonitors);
            this.Controls.Add(this.btnNext);
            this.Name = "MonitorsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitors";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnCreateMonitors;
        private System.Windows.Forms.Label lblInfo;
    }
}