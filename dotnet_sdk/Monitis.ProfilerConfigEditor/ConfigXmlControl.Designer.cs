namespace Monitis.ProfilerConfigEditor
{
    partial class ConfigXmlControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvConfig = new System.Windows.Forms.DataGridView();
            this.labelFileName = new System.Windows.Forms.Label();
            this.btnDeleteConfig = new System.Windows.Forms.Button();
            this.btnAddConfig = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConfig)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvConfig
            // 
            this.dgvConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvConfig.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConfig.Location = new System.Drawing.Point(3, 32);
            this.dgvConfig.Name = "dgvConfig";
            this.dgvConfig.Size = new System.Drawing.Size(674, 194);
            this.dgvConfig.TabIndex = 0;
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.Location = new System.Drawing.Point(3, 8);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(73, 13);
            this.labelFileName.TabIndex = 1;
            this.labelFileName.Text = "labelFileName";
            // 
            // btnDeleteConfig
            // 
            this.btnDeleteConfig.Location = new System.Drawing.Point(117, 232);
            this.btnDeleteConfig.Name = "btnDeleteConfig";
            this.btnDeleteConfig.Size = new System.Drawing.Size(139, 23);
            this.btnDeleteConfig.TabIndex = 2;
            this.btnDeleteConfig.Text = "Delete selected configs";
            this.btnDeleteConfig.UseVisualStyleBackColor = true;
            this.btnDeleteConfig.Click += new System.EventHandler(this.btnDeleteConfig_Click);
            // 
            // btnAddConfig
            // 
            this.btnAddConfig.Location = new System.Drawing.Point(3, 232);
            this.btnAddConfig.Name = "btnAddConfig";
            this.btnAddConfig.Size = new System.Drawing.Size(108, 23);
            this.btnAddConfig.TabIndex = 3;
            this.btnAddConfig.Text = "Add new config";
            this.btnAddConfig.UseVisualStyleBackColor = true;
            this.btnAddConfig.Click += new System.EventHandler(this.btnAddConfig_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(262, 232);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ConfigXmlControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAddConfig);
            this.Controls.Add(this.btnDeleteConfig);
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.dgvConfig);
            this.MinimumSize = new System.Drawing.Size(280, 260);
            this.Name = "ConfigXmlControl";
            this.Size = new System.Drawing.Size(680, 258);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConfig)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvConfig;
        public System.Windows.Forms.Label labelFileName;
        public System.Windows.Forms.Button btnDeleteConfig;
        public System.Windows.Forms.Button btnAddConfig;
        private System.Windows.Forms.Button btnSave;
    }
}
