using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Monitis.ProfilerConfigEditor
{
    public partial class ConfigXmlControl : UserControl
    {
        public Config.ConfigList ConfigList { get; set; } 

        public ConfigXmlControl(string configPath)
        {
            InitializeComponent();

            ConfigList = Config.GetConfigElementsForFile(configPath); 
            labelFileName.Text = configPath;
            dgvConfig.DataSource = ConfigList;
        }

        private void btnAddConfig_Click(object sender, EventArgs e)
        {
            ConfigList.Add(new ConfigElement(ConfigList.Instrumentation,newElement:true));
        }

        private void btnDeleteConfig_Click(object sender, EventArgs e)
        {
            if (dgvConfig.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvConfig.SelectedRows)
                {
                   if (!row.IsNewRow)
                   {
                       dgvConfig.Rows.Remove(row);
                   }
                    
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ConfigList.Save();
        }
    }
}
