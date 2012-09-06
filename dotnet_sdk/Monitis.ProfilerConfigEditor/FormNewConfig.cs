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

        public string ConfigFileName { get { return tbConfigName.Text + ".xml"; } }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormNewConfig_Shown(object sender, EventArgs e)
        {
            tbConfigName.Focus();
        }
    }
}
