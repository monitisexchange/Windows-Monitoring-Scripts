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
            this.ddlMonitorList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnApplyMonitor = new System.Windows.Forms.Button();
            this.btnAddAzureMonitor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ddlMonitorList
            // 
            this.ddlMonitorList.FormattingEnabled = true;
            this.ddlMonitorList.Location = new System.Drawing.Point(108, 33);
            this.ddlMonitorList.Name = "ddlMonitorList";
            this.ddlMonitorList.Size = new System.Drawing.Size(231, 21);
            this.ddlMonitorList.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(105, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Monitors:";
            // 
            // btnApplyMonitor
            // 
            this.btnApplyMonitor.Location = new System.Drawing.Point(366, 33);
            this.btnApplyMonitor.Name = "btnApplyMonitor";
            this.btnApplyMonitor.Size = new System.Drawing.Size(75, 23);
            this.btnApplyMonitor.TabIndex = 2;
            this.btnApplyMonitor.Text = "Select";
            this.btnApplyMonitor.UseVisualStyleBackColor = true;
            this.btnApplyMonitor.Click += new System.EventHandler(this.OnApplyMonitorClick);
            // 
            // btnAddAzureMonitor
            // 
            this.btnAddAzureMonitor.Location = new System.Drawing.Point(131, 122);
            this.btnAddAzureMonitor.Name = "btnAddAzureMonitor";
            this.btnAddAzureMonitor.Size = new System.Drawing.Size(182, 23);
            this.btnAddAzureMonitor.TabIndex = 3;
            this.btnAddAzureMonitor.Text = "Add monitor for Azure counters";
            this.btnAddAzureMonitor.UseVisualStyleBackColor = true;
            this.btnAddAzureMonitor.Click += new System.EventHandler(this.OnAddAzureMonitorClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(111, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(245, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Click if you don\'t see Windows Azure Monitor in list";
            // 
            // MonitorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 192);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAddAzureMonitor);
            this.Controls.Add(this.btnApplyMonitor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ddlMonitorList);
            this.Name = "MonitorsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MonitorsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ddlMonitorList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApplyMonitor;
        private System.Windows.Forms.Button btnAddAzureMonitor;
        private System.Windows.Forms.Label label2;
    }
}