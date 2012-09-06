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
        private ConfigFile _dataContext;
        private readonly OperationsController _controller;

        public ConfigXmlControl(ConfigFile file, OperationsController controller)
        {
            InitializeComponent();

            _dataContext = file;
            _controller = controller;
            labelFileName.Text = file.Path;
            dgvConfig.DataSource = file.ConfigElements;
        }

        private void BtnAddConfigClick(object sender, EventArgs e)
        {
            _controller.AddNewConfigRec(_dataContext);
        }

        private void BtnDeleteConfigClick(object sender, EventArgs e)
        {
            _controller.DeleteConfig(_dataContext);
        }

        private void BtnSaveClick(object sender, EventArgs e)
        {
            _controller.SaveConfig(_dataContext);
        }
    }
}
