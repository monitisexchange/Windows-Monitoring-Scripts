using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Monitis.ProfilerConfigEditor
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// For correct location next ConfigXmlControl
        /// </summary>
        private int lastLocationY = 3;
        private OperationsController _controller;

        public FormMain()
        {
            InitializeComponent();

            try
            {
                _controller = new OperationsController();
                _controller.FilesChanged += _controller_FilesChanged;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
                throw;
            }
            _controller.ReloadConfigs();
        }

        void _controller_FilesChanged(object sender, EventArgs e)
        {
            CreateConfigControls();
        }

        private void CreateConfigControls()
        {
            panelConfigs.Controls.Clear();
            lastLocationY = 3;

            panelConfigs.Hide();
            foreach (var file in _controller.Files)
            {
                AddConfigXmlControl(file);
            }
            panelConfigs.Show();
        }

        private void AddConfigXmlControl(ConfigFile configFile)
        {
            var configXmlControl = new ConfigXmlControl(configFile, _controller);
            panelConfigs.Controls.Add(configXmlControl);
            OrderControls(configXmlControl);
        }

        private void OrderControls(ConfigXmlControl configXmlControl)
        {
            configXmlControl.Anchor = AnchorStyles.None;
            configXmlControl.Location = new Point(3, lastLocationY);
            configXmlControl.Width = this.Width - 20;
            configXmlControl.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            lastLocationY += configXmlControl.Height + 25;
        }

        private void LoadConfigsToolStripMenuItemClick(object sender, EventArgs e)
        {
            _controller.ReloadConfigs();
        }

        private void SaveConfigsToolStripMenuItemClick(object sender, EventArgs e)
        {
            panelConfigs.Focus();
            _controller.SaveConfigs();
            CreateConfigControls();
        }

        private void AddNewConfigFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            _controller.AddNewConfigFile();
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
            CreateConfigControls();
        }
    }
}
