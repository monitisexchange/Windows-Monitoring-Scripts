using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public FormMain()
        {
            InitializeComponent();
        }

        private void CreateConfigControls()
        {
            panelConfigs.Controls.Clear();
            Config.Documents.Clear();
            lastLocationY = 3;

            string[] configsPaths = Config.GetConfigsPaths();

            if (null != configsPaths)
            {
                panelConfigs.Hide();
                foreach (string configPath in configsPaths)
                {
                    AddConfigXmlControl(configPath);
                }
                panelConfigs.Show();
            }
            else
            {
                MessageBox.Show(@"Cannot find path to config folder. Check [HKLM\SOFTWARE\Monitis] 'InstalledPath' key.");
            }
        }

        /// <summary>
        /// Adds ConfigXmlControl to form
        /// </summary>
        private void AddConfigXmlControl(string configPath)
        {
            var configXmlControl = new ConfigXmlControl(configPath);
            //If xml file is correct and config list exist
            if (null != configXmlControl.ConfigList)
            {
                panelConfigs.Controls.Add(configXmlControl);
                configXmlControl.Location = new Point(3, lastLocationY);
                configXmlControl.Width = this.Width - 20;
                configXmlControl.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
                lastLocationY += configXmlControl.Height + 25;
            }
            else
            {
                configXmlControl = null;
            }
        }

        private void loadConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateConfigControls();
        }

        private void saveConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.SaveAll();
        }

        private void addNewConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNewConfig formNew=new FormNewConfig();
            if  (formNew.ShowDialog()==DialogResult.OK)
            {
                AddConfigXmlControl(Config.NewConfigXmlFilePath);
            }
        }
    }
}
