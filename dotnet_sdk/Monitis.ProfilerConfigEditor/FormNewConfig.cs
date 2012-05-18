using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Monitis.ProfilerConfigEditor.Properties;

namespace Monitis.ProfilerConfigEditor
{
    public partial class FormNewConfig : Form
    {
        public FormNewConfig()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CreateFile();
        }

        /// <summary>
        /// Create new config (.xml) file
        /// </summary>
        private void CreateFile()
        {
            //Check if current file is not exist
            if (!string.IsNullOrEmpty(tbConfigName.Text))
            {
                string filename = tbConfigName.Text;
                char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                if (filename.IndexOfAny(invalidChars) > -1)
                {
                    MessageBox.Show("File name contains invalid chars");
                }
                string configDir = Config.InstalledPath;
                if (!string.IsNullOrEmpty(configDir))
                {
                    if (Directory.Exists(configDir))
                    {
                        string filepath = configDir + "\\" + filename + ".xml";
                        if (!File.Exists(filepath))
                        {
                            File.WriteAllText(filepath, Resources.newXmlContent);
                            Config.NewConfigXmlFilePath = filepath;
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show("File " + filepath + " already exists");
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Cannot find path to config folder. Check [HKLM\SOFTWARE\Monitis] 'InstalledPath' key.");
                    }
                }
                else
                {
                    MessageBox.Show(@"Cannot find path to config folder. Check [HKLM\SOFTWARE\Monitis] 'InstalledPath' key.");
                }
            }
            else
            {
                MessageBox.Show("Please, input name of config file");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void tbConfigName_KeyPress(object sender, KeyPressEventArgs e)
        {
            //On press enter try to create file
            if (e.KeyChar == '\r')
            {
                CreateFile();
            }
        }

        private void FormNewConfig_Shown(object sender, EventArgs e)
        {
            tbConfigName.Focus();
        }
    }
}
