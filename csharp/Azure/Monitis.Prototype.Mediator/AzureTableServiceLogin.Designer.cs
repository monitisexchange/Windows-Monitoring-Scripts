namespace Monitis.Prototype.UI
{
    partial class AzureTableServiceLogin
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
            this.tbxStorageAccountName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxAccountPrivateKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxRoleInstanceName = new System.Windows.Forms.TextBox();
            this.cbUseDefault = new System.Windows.Forms.CheckBox();
            this.btnApplySettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbxStorageAccountName
            // 
            this.tbxStorageAccountName.Location = new System.Drawing.Point(12, 28);
            this.tbxStorageAccountName.Name = "tbxStorageAccountName";
            this.tbxStorageAccountName.Size = new System.Drawing.Size(253, 20);
            this.tbxStorageAccountName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Storage Account Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(280, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Storage Account Private Key";
            // 
            // tbxAccountPrivateKey
            // 
            this.tbxAccountPrivateKey.Location = new System.Drawing.Point(280, 28);
            this.tbxAccountPrivateKey.Name = "tbxAccountPrivateKey";
            this.tbxAccountPrivateKey.Size = new System.Drawing.Size(253, 20);
            this.tbxAccountPrivateKey.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Role Instance Name";
            // 
            // tbxRoleInstanceName
            // 
            this.tbxRoleInstanceName.Location = new System.Drawing.Point(12, 78);
            this.tbxRoleInstanceName.Name = "tbxRoleInstanceName";
            this.tbxRoleInstanceName.Size = new System.Drawing.Size(253, 20);
            this.tbxRoleInstanceName.TabIndex = 4;
            // 
            // cbUseDefault
            // 
            this.cbUseDefault.AutoSize = true;
            this.cbUseDefault.Location = new System.Drawing.Point(283, 81);
            this.cbUseDefault.Name = "cbUseDefault";
            this.cbUseDefault.Size = new System.Drawing.Size(82, 17);
            this.cbUseDefault.TabIndex = 6;
            this.cbUseDefault.Text = "Use Default";
            this.cbUseDefault.UseVisualStyleBackColor = true;
            this.cbUseDefault.CheckedChanged += new System.EventHandler(this.OnUseDefaultCheckedChanged);
            // 
            // btnApplySettings
            // 
            this.btnApplySettings.Location = new System.Drawing.Point(458, 117);
            this.btnApplySettings.Name = "btnApplySettings";
            this.btnApplySettings.Size = new System.Drawing.Size(75, 23);
            this.btnApplySettings.TabIndex = 7;
            this.btnApplySettings.Text = "Apply";
            this.btnApplySettings.UseVisualStyleBackColor = true;
            this.btnApplySettings.Click += new System.EventHandler(this.OnApplySettingsClick);
            // 
            // AzureTableServiceLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 192);
            this.Controls.Add(this.btnApplySettings);
            this.Controls.Add(this.cbUseDefault);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxRoleInstanceName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxAccountPrivateKey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxStorageAccountName);
            this.Name = "AzureTableServiceLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login to Windows Azure Table service";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxStorageAccountName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxAccountPrivateKey;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxRoleInstanceName;
        private System.Windows.Forms.CheckBox cbUseDefault;
        private System.Windows.Forms.Button btnApplySettings;
    }
}