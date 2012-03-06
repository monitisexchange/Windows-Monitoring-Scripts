namespace Monitis.Prototype.UI
{
    partial class MediatorForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblUserMediationIntervalHint = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.nupField = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.chartControl = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSyncStorageMetrics = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nupField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(15, 354);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(83, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.OnStartClick);
            // 
            // lblUserMediationIntervalHint
            // 
            this.lblUserMediationIntervalHint.AutoSize = true;
            this.lblUserMediationIntervalHint.Location = new System.Drawing.Point(0, 330);
            this.lblUserMediationIntervalHint.Name = "lblUserMediationIntervalHint";
            this.lblUserMediationIntervalHint.Size = new System.Drawing.Size(145, 13);
            this.lblUserMediationIntervalHint.TabIndex = 2;
            this.lblUserMediationIntervalHint.Text = "Update Monitis monitor each:";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(116, 354);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.OnStopClick);
            // 
            // nupField
            // 
            this.nupField.Location = new System.Drawing.Point(151, 328);
            this.nupField.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.nupField.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nupField.Name = "nupField";
            this.nupField.Size = new System.Drawing.Size(45, 20);
            this.nupField.TabIndex = 7;
            this.nupField.Value = new decimal(new int[] {
            180,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(202, 330);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "second";
            // 
            // chartControl
            // 
            chartArea3.AlignWithChartArea = "MemoryArea";
            chartArea3.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea3.AxisX.LineWidth = 0;
            chartArea3.AxisX.Title = "Time";
            chartArea3.AxisY.Title = "% usage";
            chartArea3.Name = "CPUArea";
            chartArea4.AxisX.Title = "Time";
            chartArea4.AxisY.Title = "Free bytes";
            chartArea4.Name = "MemoryArea";
            this.chartControl.ChartAreas.Add(chartArea3);
            this.chartControl.ChartAreas.Add(chartArea4);
            legend2.Name = "Legend1";
            this.chartControl.Legends.Add(legend2);
            this.chartControl.Location = new System.Drawing.Point(3, 0);
            this.chartControl.Name = "chartControl";
            series3.ChartArea = "CPUArea";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "\\Processor(_Total)\\% Processor Time";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series4.ChartArea = "MemoryArea";
            series4.Legend = "Legend1";
            series4.Name = "\\Memory\\Available Bytes";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            this.chartControl.Series.Add(series3);
            this.chartControl.Series.Add(series4);
            this.chartControl.Size = new System.Drawing.Size(1218, 318);
            this.chartControl.TabIndex = 10;
            this.chartControl.Text = "chartControl";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 294);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(55, 13);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "{lblStatus}";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Location = new System.Drawing.Point(3, 684);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(200, 20);
            this.dtpFrom.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 668);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "From";
            // 
            // dtpTo
            // 
            this.dtpTo.Location = new System.Drawing.Point(234, 684);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(200, 20);
            this.dtpTo.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 668);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "To";
            // 
            // btnSyncStorageMetrics
            // 
            this.btnSyncStorageMetrics.Location = new System.Drawing.Point(458, 681);
            this.btnSyncStorageMetrics.Name = "btnSyncStorageMetrics";
            this.btnSyncStorageMetrics.Size = new System.Drawing.Size(75, 23);
            this.btnSyncStorageMetrics.TabIndex = 16;
            this.btnSyncStorageMetrics.Text = "Sync";
            this.btnSyncStorageMetrics.UseVisualStyleBackColor = true;
            this.btnSyncStorageMetrics.Click += new System.EventHandler(this.OnSyncStorageMetricsClick);
            // 
            // MediatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1221, 716);
            this.Controls.Add(this.btnSyncStorageMetrics);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpFrom);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.chartControl);
            this.Controls.Add(this.lblUserMediationIntervalHint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nupField);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "MediatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mediator";
            ((System.ComponentModel.ISupportInitialize)(this.nupField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblUserMediationIntervalHint;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.NumericUpDown nupField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartControl;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSyncStorageMetrics;
    }
}